// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using VRBuilder.Core.Registry;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Utils.Audio;

namespace VRBuilder.Core.User
{
    /// <summary>
    /// Provides access to the user in the current scene and to the audio player used to play instructions to them.
    /// </summary>
    public interface IUserService : IService<IUserConfiguration>
    {
        /// <summary>
        /// The scene object representing the user.
        /// </summary>
        IUserSceneObject User { get; set; }

        /// <summary>
        /// The audio player used to play instructions to the user.
        /// </summary>
        IAudioData InstructionPlayer { get; }
    }
}