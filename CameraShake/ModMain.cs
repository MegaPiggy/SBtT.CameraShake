using CameraShake.DebugStuff;
using CameraShake.BuiltInShakes;
using OWML.Common;
using OWML.ModHelper;

namespace CameraShake
{
    public sealed class ModMain : ModBehaviour
    {
        public static bool isDevelopmentVersion => false;
        public static bool IsLoaded { get; private set; }
        
        void Start()
        {
            //---------------- Start Up ----------------//
            Log.Initialize(ModHelper.Console);
            if (IsLoaded) {
                Log.Warning($"Error: Multiple Instances of Camera Shake Detected.");
                return;
            }
            else Log.Success($"Camera Shake Loaded!");
            
            IsLoaded = true;
            if (isDevelopmentVersion)
            {
                Log.Error("Debug activated. THIS MESSAGE SHOULD NOT APPEAR!");
                DebugControls debugControls = new DebugControls(ModHelper);
                ModHelper.Events.Unity.OnUpdate += debugControls.OnUpdate;
            }

            //ModHelper.Events.Unity.OnUpdate += solarSystem.OnUpdate;
            //ModHelper.Events.Unity.OnUpdate += titleScreen.OnUpdate;

            PatchShakes.SetUp(this);
            ShakeSettings.UpdateSettings(ModHelper.Config);
        }
        public override void Configure(IModConfig config) => ShakeSettings.UpdateSettings(config);
    }
}