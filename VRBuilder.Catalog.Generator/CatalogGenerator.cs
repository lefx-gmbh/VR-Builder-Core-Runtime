using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace VRBuilder.Catalog.Generator;

/// <summary>
/// Emits a static catalog of all VR Builder behaviors and conditions: class name,
/// XML summary, help link and a property stub of the nested EntityData type.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class CatalogGenerator : IIncrementalGenerator
{
    private const string BehaviorBaseType = "VRBuilder.Core.Behaviors.Behavior`1";
    private const string ConditionBaseType = "VRBuilder.Core.Conditions.Condition`1";
    private const string DisplayNameAttr = "VRBuilder.Core.Attributes.DisplayNameAttribute";
    private const string HelpLinkAttr = "VRBuilder.Core.Attributes.HelpLinkAttribute";
    private const string DataMemberAttr = "System.Runtime.Serialization.DataMemberAttribute";

    /// <summary>Composite constructs are described at their child level, not cataloged.</summary>
    private static readonly HashSet<string> SkippedTypes = new(StringComparer.Ordinal)
    {
        "BehaviorSequence",
    };

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var candidates = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (node, _) => IsCandidateNode(node),
            transform: static (ctx, ct) => BuildDescriptor(ctx, ct))
            .Where(static d => d != null);

        var catalog = candidates.Collect();

        context.RegisterSourceOutput(catalog, static (spc, descriptors) =>
            Emit(spc, descriptors.Where(d => d != null).Select(d => d!)));
    }

    private static bool IsCandidateNode(SyntaxNode node)
    {
        if (node is not ClassDeclarationSyntax { BaseList: not null } classDecl)
        {
            return false;
        }

        // Symbol-level analysis does the real filtering; here we only pre-select
        // classes with any generic base type (cheap structural check).
        foreach (var baseType in classDecl.BaseList.Types)
        {
            if (baseType.Type is GenericNameSyntax)
            {
                return true;
            }
        }

        return false;
    }

    private static Descriptor BuildDescriptor(GeneratorSyntaxContext context, CancellationToken ct)
    {
        var classSymbol = context.SemanticModel.GetDeclaredSymbol((ClassDeclarationSyntax)context.Node, ct)
            as INamedTypeSymbol;
        if (classSymbol == null)
        {
            return null;
        }

        // Top-level classes only, public, concrete, non-generic.
        if (classSymbol.ContainingType != null ||
            classSymbol.DeclaredAccessibility != Accessibility.Public ||
            classSymbol.IsAbstract ||
            classSymbol.IsGenericType)
        {
            return null;
        }

        if (SkippedTypes.Contains(classSymbol.Name))
        {
            return null;
        }

        string kind = null;
        for (var baseType = classSymbol.BaseType; baseType != null; baseType = baseType.BaseType)
        {
            var constructedFrom = baseType.OriginalDefinition.ConstructedFrom ?? baseType;
            var metadataName = constructedFrom.MetadataName;
            var ns = constructedFrom.ContainingNamespace.ToDisplayString();

            if (ns == "VRBuilder.Core.Behaviors" && metadataName == "Behavior`1")
            {
                kind = "behavior";
                break;
            }

            if (ns == "VRBuilder.Core.Conditions" && metadataName == "Condition`1")
            {
                kind = "condition";
                break;
            }
        }

        if (kind == null)
        {
            return null;
        }

        var entityData = classSymbol.GetTypeMembers("EntityData")
            .FirstOrDefault(t => t.DeclaredAccessibility == Accessibility.Public);
        if (entityData == null)
        {
            return null;
        }

        var displayName = GetAttributeString(entityData, DisplayNameAttr);
        var helpLink = GetAttributeString(classSymbol, HelpLinkAttr);
        var rawXml = classSymbol.GetDocumentationCommentXml(expandIncludes: true, cancellationToken: ct);
        var summary = ExtractSummary(rawXml);

        var properties = new List<PropertyDescriptor>();
        foreach (var member in entityData.GetMembers())
        {
            if (member is not IPropertySymbol property ||
                property.DeclaredAccessibility != Accessibility.Public ||
                !HasAttribute(property, DataMemberAttr))
            {
                continue;
            }

            var underlyingType = property.Type;
            string typeName = SimplifyTypeName(underlyingType, out var nullable);

            INamedTypeSymbol enumType = null;
            var named = underlyingType.GetNamedType();
            if (named != null && named.TypeKind == TypeKind.Enum)
            {
                enumType = named;
            }
            else if (nullable && property.Type.NullableAnnotation != NullableAnnotation.NotAnnotated &&
                     underlyingType.GetNamedType() is { TypeKind: TypeKind.Enum } innerEnum)
            {
                enumType = innerEnum;
            }

            var enumValues = enumType == null
                ? null
                : enumType.GetMembers()
                    .OfType<IFieldSymbol>()
                    .Where(f => f.HasConstantValue && f.Name != "value__")
                    .Select(f => f.Name)
                    .ToArray();

            properties.Add(new PropertyDescriptor(property.Name, typeName, enumValues));
        }

        return new Descriptor(
            classSymbol.Name,
            kind,
            summary,
            helpLink,
            displayName,
            properties.ToArray());
    }

    private static string GetAttributeString(ISymbol symbol, string attributeMetadataName)
    {
        foreach (var attribute in symbol.GetAttributes())
        {
            if (attribute.AttributeClass == null ||
                attribute.AttributeClass.ToDisplayString() != attributeMetadataName ||
                attribute.ConstructorArguments.Length == 0)
            {
                continue;
            }

            if (attribute.ConstructorArguments[0].Value is string value)
            {
                return value;
            }
        }

        return null;
    }

    private static bool HasAttribute(ISymbol symbol, string attributeMetadataName)
    {
        foreach (var attribute in symbol.GetAttributes())
        {
            if (attribute.AttributeClass != null &&
                attribute.AttributeClass.ToDisplayString() == attributeMetadataName)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Maps a property type to a simple C#-ish name; Nullable&lt;T&gt; becomes "T?".</summary>
    private static string SimplifyTypeName(ITypeSymbol type, out bool nullable)
    {
        nullable = false;

        if (type.OriginalDefinition is INamedTypeSymbol generic &&
            generic.ConstructedFrom != null &&
            generic.ConstructedFrom.ToDisplayString() == "System.Nullable<T>" &&
            type is INamedTypeSymbol closed &&
            closed.TypeArguments.Length == 1)
        {
            nullable = true;
            return SimplifyTypeName(closed.TypeArguments[0], out _) + "?";
        }

        if (type is INamedTypeSymbol named)
        {
            if (named.SpecialType != SpecialType.None)
            {
                return SpecialTypeAlias(named.SpecialType);
            }

            if (named.IsTupleType)
            {
                return "tuple";
            }

            return named.Name;
        }

        if (type.TypeKind == TypeKind.Array)
        {
            return ((IArrayTypeSymbol)type).ElementType.ToDisplayString() + "[]";
        }

        return type.ToDisplayString();
    }

    private static string SpecialTypeAlias(SpecialType specialType)
    {
        switch (specialType)
        {
            case SpecialType.System_Boolean: return "bool";
            case SpecialType.System_Byte: return "byte";
            case SpecialType.System_SByte: return "sbyte";
            case SpecialType.System_Char: return "char";
            case SpecialType.System_Decimal: return "decimal";
            case SpecialType.System_Double: return "double";
            case SpecialType.System_Single: return "float";
            case SpecialType.System_Int16: return "short";
            case SpecialType.System_UInt16: return "ushort";
            case SpecialType.System_Int32: return "int";
            case SpecialType.System_UInt32: return "uint";
            case SpecialType.System_Int64: return "long";
            case SpecialType.System_UInt64: return "ulong";
            case SpecialType.System_String: return "string";
            case SpecialType.System_Object: return "object";
            default: return "?";
        }
    }

    /// <summary>Extracts the top-level &lt;summary&gt; text; <see cref="..."/> crefs become identifier text.</summary>
    private static string ExtractSummary(string documentationXml)
    {
        if (string.IsNullOrEmpty(documentationXml))
        {
            return "";
        }

        // Match the top-level <summary> element only.
        var match = Regex.Match(documentationXml, "<summary>(.*?)</summary>", RegexOptions.Singleline);
        if (!match.Success)
        {
            return "";
        }

        var text = match.Groups[1].Value;
        text = Regex.Replace(text, @"<(see|paramref)(\s+(cref|name)=""([^""]+)"")?\s*/?>", match2 =>
        {
            var target = match2.Groups[4].Value;
            if (string.IsNullOrEmpty(target))
            {
                return "";
            }

            // Strip "T:Namespace.Type" style prefixes down to the identifier.
            var lastSeparator = target.LastIndexOfAny(new[] { ':', '.' });
            if (lastSeparator >= 0)
            {
                target = target.Substring(lastSeparator + 1);
            }

            // Strip generic arity markers, e.g. "Behavior`1".
            var arityIndex = target.IndexOf('`');
            if (arityIndex >= 0)
            {
                target = target.Substring(0, arityIndex);
            }

            return target;
        });
        text = Regex.Replace(text, @"<[^>]+>", "");
        text = System.Net.WebUtility.HtmlDecode(text);
        text = Regex.Replace(text, @"\s+", " ").Trim();
        return text;
    }

    private static void Emit(SourceProductionContext context, IEnumerable<Descriptor> descriptors)
    {
        var behaviors = descriptors.Where(d => d.Kind == "behavior")
            .OrderBy(d => d.Name, StringComparer.Ordinal)
            .ToArray();
        var conditions = descriptors.Where(d => d.Kind == "condition")
            .OrderBy(d => d.Name, StringComparer.Ordinal)
            .ToArray();

        var builder = new StringBuilder();
        builder.AppendLine("// <auto-generated/>");
        builder.AppendLine("#nullable enable");
        builder.AppendLine("namespace VRBuilder.Core.Generated");
        builder.AppendLine("{");

        builder.AppendLine("public sealed record PropertyStub(string Name, string TypeName, string[]? EnumValues);");
        builder.AppendLine("public sealed record EntityDataStub(string DisplayName, PropertyStub[] Properties);");
        builder.AppendLine("public sealed record EntityDescriptor(string Name, string Kind, string Summary, string? HelpLink, EntityDataStub Data);");

        EmitCatalog(builder, "BehaviorCatalog", behaviors);
        EmitCatalog(builder, "ConditionCatalog", conditions);

        builder.AppendLine("}");
        context.AddSource("BehaviorConditionCatalog.g.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
    }

    private static void EmitCatalog(StringBuilder builder, string className, Descriptor[] entries)
    {
        builder.AppendLine($"public static class {className}");
        builder.AppendLine("{");
        builder.AppendLine("    public static readonly EntityDescriptor[] All = new EntityDescriptor[]");
        builder.AppendLine("    {");
        foreach (var entry in entries)
        {
            builder.AppendLine($"        new EntityDescriptor({Literal(entry.Name)}, {Literal(entry.Kind)}, {Literal(entry.Summary ?? "")}, {NullableLiteral(entry.HelpLink)},");
            builder.AppendLine($"            new EntityDataStub({NullableLiteral(entry.DisplayName)}, new PropertyStub[]");
            builder.AppendLine("            {");
            foreach (var property in entry.Properties)
            {
                var enumValues = property.EnumValues == null
                    ? "null"
                    : "new string[] { " + string.Join(", ", property.EnumValues.Select(Literal)) + " }";
                builder.AppendLine($"                new PropertyStub({Literal(property.Name)}, {Literal(property.TypeName)}, {enumValues}),");
            }

            builder.AppendLine("            })),");
        }

        builder.AppendLine("    };");
        builder.AppendLine("}");
    }

    private static string Literal(string value) =>
        SymbolDisplay.FormatLiteral(value ?? "", quote: true);

    private static string NullableLiteral(string value) =>
        value == null ? "null" : Literal(value);

    private sealed class Descriptor
    {
        public Descriptor(string name, string kind, string summary, string helpLink, string displayName, PropertyDescriptor[] properties)
        {
            Name = name;
            Kind = kind;
            Summary = summary;
            HelpLink = helpLink;
            DisplayName = displayName;
            Properties = properties;
        }

        public string Name { get; }
        public string Kind { get; }
        public string Summary { get; }
        public string HelpLink { get; }
        public string DisplayName { get; }
        public PropertyDescriptor[] Properties { get; }

        public override bool Equals(object obj) => obj is Descriptor other && Equals(other);

        private bool Equals(Descriptor other) =>
            Name == other.Name &&
            Kind == other.Kind &&
            Summary == other.Summary &&
            HelpLink == other.HelpLink &&
            DisplayName == other.DisplayName &&
            StructuralEquals(Properties, other.Properties);

        private static bool StructuralEquals(PropertyDescriptor[] a, PropertyDescriptor[] b)
        {
            if (a == null || b == null)
            {
                return a == b;
            }

            if (a.Length != b.Length)
            {
                return false;
            }

            for (var i = 0; i < a.Length; i++)
            {
                if (a[i].Name != b[i].Name || a[i].TypeName != b[i].TypeName ||
                    !StructuralEquals(a[i].EnumValues, b[i].EnumValues))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool StructuralEquals(string[] a, string[] b)
        {
            if (a == null || b == null)
            {
                return a == b;
            }

            return a.Length == b.Length && a.SequenceEqual(b, StringComparer.Ordinal);
        }

        public override int GetHashCode() =>
            (Name, Kind, Summary, HelpLink, DisplayName).GetHashCode();
    }

    private sealed class PropertyDescriptor
    {
        public PropertyDescriptor(string name, string typeName, string[] enumValues)
        {
            Name = name;
            TypeName = typeName;
            EnumValues = enumValues;
        }

        public string Name { get; }
        public string TypeName { get; }
        public string[] EnumValues { get; }
    }
}

internal static class TypeSymbolExtensions
{
    public static INamedTypeSymbol GetNamedType(this ITypeSymbol type)
    {
        return type as INamedTypeSymbol;
    }
}
