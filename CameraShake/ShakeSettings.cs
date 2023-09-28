using OWML.Common;
using System.Collections.Generic;

namespace CameraShake
{
    public static class ShakeSettings
    {
        public static float Master { get; private set; }
        public static float Explosions { get; private set; }
        public static float Environment { get; private set; }
        public static float Ship { get; private set; }
        public static float Player { get; private set; }
        public static float Scout { get; private set; }
        public static float Jetpack { get; private set; }

        public static void UpdateSettings(IModConfig config)
        {
            Master = config.GetSettingsValue<float>("Master Multiplier");
            Explosions = config.GetSettingsValue<float>("Explosions");
            Environment = config.GetSettingsValue<float>("Environment");
            Ship = config.GetSettingsValue<float>("Ship");
            Player = config.GetSettingsValue<float>("Player");
            Scout = config.GetSettingsValue<float>("Scout");
            Jetpack = config.GetSettingsValue<float>("Jetpack Boost");
        }
    }
}