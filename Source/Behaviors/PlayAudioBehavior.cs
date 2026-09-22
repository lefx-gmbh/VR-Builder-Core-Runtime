// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Configuration.Modes;
using VRBuilder.Core.Properties;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Utils.Audio;

namespace VRBuilder.Core.Behaviors
{
    /// <summary>
    /// A behavior that plays audio.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://www.mindport.co/vr-builder/manual/default-behaviors/play-audio-file")]
    public class PlayAudioBehavior : Behavior<PlayAudioBehavior.EntityData>, IOptional
    {
        /// <summary>
        /// Creates an empty play audio behavior, used by the JSON deserializer.
        /// </summary>
        [JsonConstructor]
        protected PlayAudioBehavior() : this(Guid.Empty, null, BehaviorExecutionStages.None)
        {
        }

        /// <summary>
        /// Creates a behavior that plays <paramref name="audioData"/> on the audio player identified by <paramref name="audioPlayer"/> at the given <paramref name="executionStages"/>.
        /// </summary>
        /// <param name="audioPlayer">Unique id of the audio player scene property that plays the audio.</param>
        /// <param name="audioData">Audio data containing the clip to play.</param>
        /// <param name="executionStages">Stages at which the audio is played.</param>
        public PlayAudioBehavior(Guid audioPlayer, IAudioData audioData, BehaviorExecutionStages executionStages)
        {
            Data.AudioData = audioData;
            Data.ExecutionStages = executionStages;
            Data.AudioProperty = new SingleScenePropertyReference<IAudioPlayer>(audioPlayer);
            Data.IsBlocking = true;
        }

        /// <summary>
        /// Creates a behavior that plays <paramref name="audioData"/> on the audio player identified by <paramref name="audioPlayer"/> at the given <paramref name="executionStages"/>, optionally blocking step completion.
        /// </summary>
        /// <param name="audioPlayer">Unique id of the audio player scene property that plays the audio.</param>
        /// <param name="audioData">Audio data containing the clip to play.</param>
        /// <param name="executionStages">Stages at which the audio is played.</param>
        /// <param name="isBlocking">If <c>true</c>, the behavior prevents step completion until the audio has finished playing.</param>
        public PlayAudioBehavior(Guid audioPlayer, IAudioData audioData, BehaviorExecutionStages executionStages, bool isBlocking) : this(audioPlayer, audioData, executionStages)
        {
            Data.IsBlocking = isBlocking;
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new PlayAudioProcess(BehaviorExecutionStages.Activation, Data);
        }

        /// <inheritdoc />
        public override IStageProcess GetDeactivatingProcess()
        {
            return new PlayAudioProcess(BehaviorExecutionStages.Deactivation, Data);
        }

        /// <inheritdoc />
        public override IStageProcess GetAbortingProcess()
        {
            return new AbortingProcess(Data);
        }

        /// <summary>
        /// The "play audio" behavior's data.
        /// </summary>
        [DataContract(IsReference = true)]
        public class EntityData : IBackgroundBehaviorData, IBehaviorExecutionStages
        {
            /// <summary>
            /// The Unity's audio source to play the sound. If not set, it will fallback to Camera Locked Audio/>.
            /// </summary>
            [DataMember]
            [DisplayName("Audio Player")]
            public SingleScenePropertyReference<IAudioPlayer> AudioProperty { get; set; }

            /// <summary>
            /// An audio data that contains an audio clip to play.
            /// </summary>
            [DataMember]
            public IAudioData AudioData { get; set; }

            /// <summary>
            /// Audio volume this audio file should be played with.
            /// </summary>
            [DataMember]
            [DisplayName("Audio Volume (from 0 to 1)")]
            [UsesSpecificProcessDrawer("NormalizedFloatDrawer")]
            public float Volume { get; set; } = 1.0f;

            /// <inheritdoc />
            public Metadata Metadata { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            public string Name
            {
                get
                {
                    string executionStages = "";

                    switch (ExecutionStages)
                    {
                        case BehaviorExecutionStages.Activation:
                            executionStages = " on activation";
                            break;
                        case BehaviorExecutionStages.Deactivation:
                            executionStages = " on deactivation";
                            break;
                        case BehaviorExecutionStages.ActivationAndDeactivation:
                            executionStages = " on activation and deactivation";
                            break;
                    }

                    return $"Play audio{executionStages}";
                }
            }

            /// <inheritdoc />
            public bool IsBlocking { get; set; }

            /// <inheritdoc />
            [DataMember]
            public BehaviorExecutionStages ExecutionStages { get; set; }
        }

        private class PlayAudioProcess : StageProcess<EntityData>
        {
            private readonly BehaviorExecutionStages executionStages;

            public PlayAudioProcess(BehaviorExecutionStages executionStages, EntityData data) : base(data)
            {
                this.executionStages = executionStages;
            }

            /// <inheritdoc />
            public override void Start()
            {
                Data.AudioProperty.Value.ResetAudio();
                Data.Volume = Math.Clamp(Data.Volume, 0.0f, 1.0f);
                Data.AudioData.Initialize();
            }

            /// <inheritdoc />
            public override IEnumerator Update()
            {
                if ((Data.ExecutionStages & executionStages) > 0)
                {
                    //wait for loading
                    while (!Data.AudioData.IsReady && Data.AudioData.IsLoading)
                    {
                        yield return null;
                    }

                    //start playing
                    if (Data.AudioData.HasAudio)
                    {
                        Data.AudioProperty.Value.PlayAudio(Data.AudioData, Data.Volume);
                    }

                    // Wait for playback, but keep the process blocked while the application is frozen.
                    while (Data.AudioProperty.Value.IsPlaying)
                    {
                        yield return null;
                    }
                }
            }

            /// <inheritdoc />
            public override void End()
            {
                if ((Data.ExecutionStages & executionStages) > 0)
                {
                    Data.AudioProperty.Value.ResetAudio();
                }
            }

            /// <inheritdoc />
            public override void FastForward()
            {
                if ((Data.ExecutionStages & executionStages) > 0 && Data.AudioProperty.Value.IsPlaying)
                {
                    Data.AudioProperty.Value.StopAudio();
                }
            }
        }

        private class AbortingProcess : InstantProcess<EntityData>
        {
            public AbortingProcess(EntityData data) : base(data)
            {
            }

            public override void Start()
            {
                ForwardingLogger.Log("Aborting");
                Data.AudioProperty.Value.StopAudio();
            }
        }
    }
}