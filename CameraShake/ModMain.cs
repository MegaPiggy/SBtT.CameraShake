using CameraShake.DebugStuff;
using CameraShake.BuiltInShakes;
using OWML.Common;
using OWML.ModHelper;

namespace CameraShake
{
    public sealed class ModMain : ModBehaviour
    {
        public static bool isDevelopmentVersion =>
#if DEBUG
            true;
#else
            false;
#endif

        public static bool IsLoaded { get; private set; }

        public void Start()
        {
            //---------------- Start Up ----------------//
            Log.Initialize(ModHelper.Console);
            if (IsLoaded)
            {
                Log.Warning($"Error: Multiple Instances of Camera Shake Detected.");
                return;
            }
            else Log.Success($"Camera Shake Loaded!");

            IsLoaded = true;
            if (isDevelopmentVersion)
            {
                Log.Error("Debug activated. THIS MESSAGE SHOULD NOT APPEAR!");
                gameObject.AddComponent<DebugControls>();
            }

            PatchShakes.SetUp(this);
            ShakeSettings.UpdateSettings(ModHelper.Config);
        }

        public override void Configure(IModConfig config) => ShakeSettings.UpdateSettings(config);

        public override object GetApi() => new CameraShakerAPI();
    }
}