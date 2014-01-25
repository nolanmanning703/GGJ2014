using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace gamejam2014
{
    /// <summary>
    /// All the sound assets used in the game.
    /// </summary>
    public static class SoundAssets
    {
        public static float LevelMusicVolume = 1.0f;

        public static Dictionary<ZoomLevels, SoundEffectInstance> LevelMusic = new Dictionary<ZoomLevels, SoundEffectInstance>()
        {
            { ZoomLevels.One, null },
            { ZoomLevels.Two, null },
            { ZoomLevels.Three, null },
            { ZoomLevels.Four, null },
            { ZoomLevels.Five, null },
        };
        public static void SwitchZoomMusic(ZoomLevels newZoom)
        {
            foreach (ZoomLevels zoom in WorldData.AscendingZooms) if (LevelMusic[zoom] != null)
            {
                if (zoom == newZoom) LevelMusic[zoom].Volume = LevelMusicVolume;
                else LevelMusic[zoom].Volume = 0.0f;
            }
        }

        public static void Initialize(Microsoft.Xna.Framework.Content.ContentManager content)
        {

        }
    }
}
