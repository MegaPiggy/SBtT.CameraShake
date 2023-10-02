using OWML.Common;
using OWML.ModHelper;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CameraShake.BuiltInShakes
{
    public static class PatchShakes
    {
        public static void SetUp(ModMain main) //Doing it this way because got errors with attributes.
        {
            IHarmonyHelper harmony = main.ModHelper.HarmonyHelper;
            var t = typeof(PatchShakes);

            //---------------- Ship ----------------//
            harmony.AddPostfix<ShipAudioController>("PlayEject", t, nameof(PatchShakes.PlayEject));
            harmony.AddPostfix<RumbleManager>("PlayShipIgnition", t, nameof(PatchShakes.PlayShipIgnition));
            harmony.AddPostfix<ShipDamageController>("OnImpact", t, nameof(PatchShakes.OnImpact_Ship));
            harmony.AddPostfix<ShipDamageController>("Explode", t, nameof(PatchShakes.Explode_Ship));
            harmony.AddPostfix<ShipFluidDetector>("OnEnterFluidType_Internal", t, nameof(PatchShakes.OnEnterFluidType_Internal_Ship));
            harmony.AddPostfix<TornadoFluidVolume>("GetPointFluidAngularVelocity", t, nameof(PatchShakes.GetPointFluidAngularVelocity));
            harmony.AddPostfix<NomaiShuttleController>("OnLaunchSlotActivated", t, nameof(PatchShakes.OnShuttleLaunch));
            harmony.AddPostfix<GravityCannonController>("PlayEndOfRecallEffect", t, nameof(PatchShakes.OnShuttleRecall));

            harmony.AddPostfix<ShipThrusterAudio>("OnStartShipIgnition", t, nameof(PatchShakes.OnStartShipIgnition));
            harmony.AddPostfix<ShipThrusterAudio>("OnCancelShipIgnition", t, nameof(PatchShakes.OnCancelShipIgnition));

            //---------------- Player ----------------//
            harmony.AddPostfix<PlayerCameraFluidDetector>("OnEnterFluidType_Internal", t, nameof(PatchShakes.OnEnterFluidType_Internal_Player));
            harmony.AddPostfix<PlayerCameraFluidDetector>("OnExitFluidType_Internal", t, nameof(PatchShakes.OnExitFluidType_Internal_Player));

            harmony.AddPostfix<ElectricityVolume>("ApplyShock", t, nameof(PatchShakes.ApplyShock));
            harmony.AddPostfix<PlayerResources>("ApplySuitPuncture", t, nameof(PatchShakes.ApplySuitPuncture));
            harmony.AddPrefix<DeathManager>("KillPlayer", t, nameof(PatchShakes.KillPlayer));

            //---------------- Scout ----------------//
            harmony.AddPostfix<ProbeLauncher>("LaunchProbe", t, nameof(PatchShakes.LaunchProbe));
#if RETRIEVE_PROBE
            harmony.AddPostfix<ProbeLauncher>("RetrieveProbe", t, nameof(PatchShakes.RetrieveProbe));
#endif

            //---------------- Environment/Explosions ----------------//
            harmony.AddPostfix<IslandController>("OnSpawnSplash", t, nameof(PatchShakes.OnSpawnSplash));
            harmony.AddPostfix<IslandAudioController>("OnIslandEnteredTornado", t, nameof(PatchShakes.OnIslandEnteredTornado));
            harmony.AddPostfix<RingWorldController>("BeginDeploySails", t, nameof(PatchShakes.BeginDeploySails));
            harmony.AddPostfix<RingWorldController>("DamageDam", t, nameof(PatchShakes.DamageDam));
            harmony.AddPostfix<RingWorldController>("BreakDam", t, nameof(PatchShakes.BreakDam));
            harmony.AddPostfix<FloodImpactEffect>("PlayEffects", t, nameof(PatchShakes.FloodImpactEffect_PlayEffects));

            harmony.AddPrefix<SunController>("CheckPlayerSawSunExplode", t, nameof(PatchShakes.SunExplode));

            harmony.AddPostfix<MeteorController>("Launch", t, nameof(PatchShakes.Launch_Meteor));
            harmony.AddPostfix<MeteorController>("Impact", t, nameof(PatchShakes.Impact_Meteor));
            harmony.AddPostfix<DetachableFragment>("Detach", t, nameof(PatchShakes.Detach_BHFragment));

            harmony.AddPostfix<EmergencyHatch>("OnOpenHatch", t, nameof(PatchShakes.OnOpenHatch));

            harmony.AddPrefix<AnglerfishAudioController>("OnChangeAnglerState", t, nameof(PatchShakes.OnChangeAnglerState));
            //PlayEndOfRecallEffect
            //---------------- Advanced Shakes ----------------//
            harmony.AddPrefix<JetpackThrusterAudio>("Update", t, nameof(PatchShakes.JetpackThrusterAudio_Update));
            harmony.AddPostfix<SandFunnelController>("Update", t, nameof(PatchShakes.SandFunnelUpdate));

            harmony.AddPostfix<RingRiverController>("StartFlood", t, nameof(PatchShakes.StartFlood));
            harmony.AddPostfix<RingRiverController>("Update", t, nameof(PatchShakes.RingRiver_Update));

            harmony.AddPostfix<SunSurfaceAudioController>("Update", t, nameof(PatchShakes.SunSurfaceAudioController_Update));
            harmony.AddPostfix<SupernovaEffectController>("FixedUpdate", t, nameof(PatchShakes.SupernovaEffectController_FixedUpdate));

#if NOMAI_DOOR
            harmony.AddPrefix<NomaiMultiPartDoor>("Open", t, nameof(PatchShakes.NomaiMultiPartDoor_Open));
            harmony.AddPrefix<NomaiMultiPartDoor>("FixedUpdate", t, nameof(PatchShakes.NomaiMultiPartDoor_FixedUpdate));
            harmony.AddPrefix<NomaiMultiPartDoor>("Close", t, nameof(PatchShakes.NomaiMultiPartDoor_Close));
            harmony.AddPrefix<NomaiGateway>("OpenGate", t, nameof(PatchShakes.NomaiGateway_OpenGate));
            harmony.AddPrefix<NomaiGateway>("FixedUpdate", t, nameof(PatchShakes.NomaiGateway_FixedUpdate));
            harmony.AddPrefix<NomaiGateway>("CloseGate", t, nameof(PatchShakes.NomaiGateway_CloseGate));
#endif

            //---------------- Extras ----------------//
            harmony.AddPostfix<OWAudioMixer>("MixMemoryUplink", t, nameof(PatchShakes.MixMemoryUplink));

            harmony.AddPostfix<TimelineObliterationController>("TriggerCrackEffect", t, nameof(PatchShakes.TriggerCrackEffect));
            harmony.AddPostfix<RumbleManager>("StartVesselWarp", t, nameof(PatchShakes.StartVesselWarp));
            harmony.AddPostfix<CosmicInflationController>("StartHotBigBang", t, nameof(PatchShakes.StartHotBigBang));

            harmony.AddPostfix<PlayerState>("OnEyeStateChanged", t, nameof(PatchShakes.OnEyeStateChanged));

            //---------------- Misc ----------------//
            harmony.AddPostfix<Locator>("LocateSceneObjects", t, nameof(PatchShakes.LocateSceneObjects));
        }

        //--------------------------------------------- Ship ---------------------------------------------//
        //---------------- Hearthian ----------------//
        public static void PlayEject(ShipAudioController __instance) => CameraShaker.MediumShake(ShakeSettings.Ship);
        static float GetTakeOffIntensityAmount()
        {
            //Getting gravity from player doesn't work in ship.
            var shipDetectorObj = Locator.GetShipDetector();
            if (!shipDetectorObj.TryGetComponent(out ForceDetector fd)) return 1f;

            var _netAcceleration = Vector3.zero; //fd.GetForceAcceleration();
            for (int i = fd._activeVolumes.Count - 1; i >= 0; i--)
            {
                if (fd._activeVolumes[i] == null) break;
                _netAcceleration += (fd._activeVolumes[i] as ForceVolume).CalculateForceAccelerationOnBody(fd._attachedBody);
            }
            //Doesn't work on Hourglass Twins??? Gravity higher than it should be.

            float gravity = _netAcceleration.magnitude / 17f;
            return Mathf.Clamp01(gravity);
        }

        public static void PlayShipIgnition(RumbleManager __instance)
        {
            float t = GetTakeOffIntensityAmount();
            CameraShaker.MediumShake(ShakeSettings.Ship * 0.75f * t);
        }
        static PerlinShake shipStartIgnitionShake;
        public static void OnStartShipIgnition(ShipThrusterAudio __instance)
        {
            float t = GetTakeOffIntensityAmount();
            shipStartIgnitionShake = CameraShaker.ExplosionShake(2f * ShakeSettings.Ship * t, 2f, null, 2f);
        }
        public static void OnCancelShipIgnition(ShipThrusterAudio __instance)
        {
            if (shipStartIgnitionShake != null) shipStartIgnitionShake.ForceDecay(0.2f);
        }

        public static void OnImpact_Ship(ShipDamageController __instance, ImpactData impact)
        {
            if (!PlayerState.IsInsideShip()) return;

            float t = Mathf.InverseLerp(5f, 120f, impact.speed);
            if (t < Mathf.Epsilon) return;

            CameraShaker.ExplosionShake(t * 12f * ShakeSettings.Ship, 2f);
        }
        public static void Explode_Ship(ShipDamageController __instance, bool debug)
        {
            var pos = __instance.transform.position;
            CameraShaker.ExplosionShake(30f * ShakeSettings.Explosions, 1.5f, pos, 10f, 20f, 150f);
        }

        public static void OnEnterFluidType_Internal_Ship(ShipFluidDetector __instance, FluidVolume fluid)
        {
            if (!PlayerState.IsInsideShip()) return;

            var speed = GetSpeed(__instance, fluid);
            float t = Mathf.InverseLerp(5f, 25f, speed);

            switch (fluid.GetFluidType())
            {
                case FluidVolume.Type.AIR:
                    t = Mathf.InverseLerp(70f, 200f, speed);
                    CameraShaker.ExplosionShake(t * 5f * ShakeSettings.Ship, 2.5f);
                    break;
                case FluidVolume.Type.WATER:
                    CameraShaker.ExplosionShake(t * 5f * ShakeSettings.Ship, 1.5f);
                    break;
                case FluidVolume.Type.CLOUD:
                    CameraShaker.ExplosionShake(t * 5f * ShakeSettings.Ship, 2.5f, null, 4f);
                    break;
            }
        }
        static float shipLastEnterTornadoTime;
        public static void GetPointFluidAngularVelocity(TornadoFluidVolume __instance, Vector3 worldPosition, FluidDetector detector)
        {
            if (Time.time < shipLastEnterTornadoTime + 1.5f) return;

            if (PlayerState.IsInsideShip() && detector.CompareTag("ShipDetector"))
            {
                CameraShaker.ExplosionShake(3f * ShakeSettings.Ship, 5f, null, 2f);
                shipLastEnterTornadoTime = Time.time;
            }
        }

        static float GetSpeed(FluidDetector __instance, FluidVolume fluid)
        {
            return (__instance._owRigidbody.GetVelocity() - fluid.GetAttachedOWRigidbody().GetPointVelocity(__instance.transform.position)).magnitude;
        }

        //---------------- Nomai ----------------//
        public static void OnShuttleLaunch(NomaiShuttleController __instance, NomaiInterfaceSlot slot)
            => CameraShaker.MediumShake(ShakeSettings.Ship * 0.6f);

        public static void OnShuttleRecall(GravityCannonController __instance)
            => CameraShaker.MediumShake(ShakeSettings.Ship * 0.4f);

        //--------------------------------------------- Player ---------------------------------------------//
        public static void OnEnterFluidType_Internal_Player(PlayerCameraFluidDetector __instance, FluidVolume fluid)
        {
            if (PlayerState.IsInsideShip()) return;
            float speed = GetSpeed(__instance, fluid);
            float t = Mathf.InverseLerp(5f, 25f, speed);

            switch (fluid.GetFluidType())
            {
                case FluidVolume.Type.AIR:
                    t = Mathf.InverseLerp(70f, 200f, speed);
                    CameraShaker.ExplosionShake(t * 3f * ShakeSettings.Player, 2f, null, 8f);
                    break;
                case FluidVolume.Type.WATER:
                    CameraShaker.ExplosionShake(t * 3f * ShakeSettings.Player, 1.25f);
                    break;
                case FluidVolume.Type.CLOUD:
                    CameraShaker.ExplosionShake(t * 3f * ShakeSettings.Player, 2f, null, 4f);
                    break;
                case FluidVolume.Type.GEYSER:
                    CameraShaker.ExplosionShake(6f * ShakeSettings.Player, 3f, null, 8f);
                    break;
            }
        }
        public static void OnExitFluidType_Internal_Player(PlayerCameraFluidDetector __instance, FluidVolume fluid)
        {
            if (PlayerState.IsInsideShip()) return;
            switch (fluid.GetFluidType())
            {
                case FluidVolume.Type.GEYSER:
                    CameraShaker.ExplosionShake(3f * ShakeSettings.Player, 1f, null, 8f);
                    break;
            }
        }
        public static void ApplyShock(ElectricityVolume __instance, HazardDetector detector)
        {
            if (detector.CompareTag("PlayerDetector"))
            {
                CameraShaker.MediumShake(ShakeSettings.Player * 0.65f);
            }
            if (detector.CompareTag("ShipDetector"))
            {
                CameraShaker.MediumShake(ShakeSettings.Ship * 0.5f);
            }
        }
        public static void ApplySuitPuncture(PlayerResources __instance) => CameraShaker.MediumShake(ShakeSettings.Player * 0.3f);
        public static void KillPlayer(DeathManager __instance, DeathType deathType)
        {
            switch (deathType)
            {
                //Neck snap shake doesn't work?
                case DeathType.Default: CameraShaker.ExplosionShake(ShakeSettings.Player * 8f, 2.25f, null, 10f); break;
                case DeathType.Impact: CameraShaker.ExplosionShake(ShakeSettings.Player * 8f, 2.25f, null, 10f); break;
                case DeathType.Asphyxiation: break;
                case DeathType.Energy: break;
                case DeathType.Supernova: CameraShaker.ExplosionShake(ShakeSettings.Explosions * 8f, 4f, null, 7f); break;
                case DeathType.Digestion: CameraShaker.ExplosionShake(ShakeSettings.Player * 1f, 10f, null, 6f); break;
                case DeathType.BigBang: break;
#if CRUSHED
                case DeathType.Crushed: CameraShaker.ExplosionShake(ShakeSettings.Environment * 3f, 20f, null, 0.1f); break;
#endif
                //Not the right place
                case DeathType.Meditation: break;
                case DeathType.TimeLoop: CameraShaker.ExplosionShake(ShakeSettings.Player * 8f, 10f, null, 0.3f); break;
                case DeathType.Lava: CameraShaker.ExplosionShake(ShakeSettings.Player * 8f, 4f, null, 10f); break;
                case DeathType.BlackHole: break;
                case DeathType.Dream: break;
                case DeathType.DreamExplosion: CameraShaker.ExplosionShake(ShakeSettings.Explosions * 8f, 2f, null, 7f); break;
                case DeathType.CrushedByElevator: break;
            }
        }

        //--------------------------------------------- Scout ---------------------------------------------//
        public static void LaunchProbe(ProbeLauncher __instance)
        {
            if (!PlayerState.IsInsideShip()) CameraShaker.SubtleShake(ShakeSettings.Scout);
        }
#if RETRIEVE_PROBE
        public static void RetrieveProbe(ProbeLauncher __instance) => CameraShaker.SubtleShake(ShakeSettings.Scout); //Too much
#endif

        //--------------------------------------------- Environment ---------------------------------------------//
        public static void OnIslandEnteredTornado(IslandAudioController __instance)
        {
            var pos = __instance.transform.position;
            //if (!PlayerState.IsInsideShip())
            CameraShaker.ExplosionShake(7f * ShakeSettings.Environment, 2.5f, pos, 4f, 150f, 250f);
        }
        public static void OnSpawnSplash(IslandController __instance, FluidVolume fluidVol)
        {
            var pos = __instance._transform.position;
            if (fluidVol.GetFluidType() == FluidVolume.Type.CLOUD)
            {
                CameraShaker.ExplosionShake(2f * ShakeSettings.Environment, 1.5f, pos, 10f, 150f, 250f);
            }
            if (fluidVol.GetFluidType() == FluidVolume.Type.WATER)
            {
                CameraShaker.ExplosionShake(10f * ShakeSettings.Environment, 2.5f, pos, 2f, 150f, 500f);
            }
        }
        public static void BeginDeploySails(RingWorldController __instance)
        {
            var pos = __instance._ringWorldBody._transform.position;
            if (PlayerState.InDreamWorld()) CameraShaker.ExplosionShake(1.75f * ShakeSettings.Environment, 5f, null, 1.75f);
            else CameraShaker.ExplosionShake(2.5f * ShakeSettings.Environment, 16f, pos, 1f, 1000f, 2000f);
        }
        public static void DamageDam(RingWorldController __instance) //Shake at end of Deploy Sails
        {
            if (InRingworld) CameraShaker.ExplosionShake(3.5f * ShakeSettings.Environment, 5.5f, null, 0.6f);
        }
        public static void FloodImpactEffect_PlayEffects(FloodImpactEffect __instance) //Shake from Wave Damage
        {
            if (InRingworld)
            {
                if (__instance._audioSource != null)
                {
                    var pos = __instance.transform.position;

                    switch (__instance._audioType)
                    {
                        case AudioType.WaterSpray_Small:
                        case AudioType.WoodImpact_Small: CameraShaker.ExplosionShake(5f * ShakeSettings.Explosions, 0.8f, pos, 10f, 20f, 150f); break;

                        case AudioType.WaterSpray_Large:
                        case AudioType.GeneralDestruction:
                        case AudioType.StiltDestruction:
                        case AudioType.HouseDestruction:
                        case AudioType.WoodImpact_Large: CameraShaker.ExplosionShake(8f * ShakeSettings.Explosions, 1.2f, pos, 10f, 20f, 200f); break;

                        case AudioType.HouseCollapse_Zone3: CameraShaker.ExplosionShake(6f * ShakeSettings.Explosions, 4.5f, pos, 5f, 20f, 125f); break;
                    }
                }
            }
        }
        static bool InRingworld => PlayerState.InCloakingField() && !PlayerState.InDreamWorld();

        public static void BreakDam(RingWorldController __instance)
        {
            if (PlayerState.InDreamWorld()) CameraShaker.ExplosionShake(2.2f * ShakeSettings.Explosions, 4f, null, 5f);
            else if (__instance._playerInsideRingWorld)
            {
                var pos = __instance._damController._intactDamRoot.transform.position;
                CameraShaker.ExplosionShake(13f * ShakeSettings.Explosions, 2.6f, pos, 8f, 200f, 1000f);
            }
        }

        public static void SunExplode(RingWorldController __instance)
        {
            if (!PlayerState.InCloakingField()) CameraShaker.ExplosionShake(2.5f * ShakeSettings.Explosions, 4f, null, 3.5f);
        }

        public static void Launch_Meteor(MeteorController __instance,
            Transform parent, Vector3 worldPosition, Quaternion worldRotation, Vector3 linearVelocity, Vector3 angularVelocity)
        {
            CameraShaker.ExplosionShake(8f * ShakeSettings.Explosions, 2f, worldPosition, 4f, 50f, 200f);
        }
        public static void Impact_Meteor(MeteorController __instance,
            GameObject hitObject, Vector3 impactPoint, Vector3 impactVel)
        {
            float strength = 10f * ShakeSettings.Explosions;
            if (PlayerState.IsInsideShip()) strength *= 0.5f;
            //if (!Locator.GetPlayerController().IsGrounded()) strength *= 0.5f;

            CameraShaker.ExplosionShake(strength, 2f, impactPoint, 4f, 40f, 450f);
        }
        public static void Detach_BHFragment(DetachableFragment __instance) //, ref string __result)
        {
            if (__instance.TryGetComponent(out EmergencyHatch h)) return;

            var pos = __instance.transform.TransformPoint(__instance._localCenterOfMass);
            string name = __instance.gameObject.name;
            if (name.Contains("Prefab_NOM_DetachableBridge") || name.Contains("Platform"))
            {
                CameraShaker.ExplosionShake(1f * ShakeSettings.Environment, 5f, pos, 3f, 7f, 14f);
            }
            else
            {
                float strength = 3f * ShakeSettings.Explosions;
                if (PlayerState.IsInsideShip()) strength *= 0.5f;
                CameraShaker.ExplosionShake(strength, 10f, pos, 0.5f, 40f, 450f);
            }
        }
        public static void OnOpenHatch(EmergencyHatch __instance, NomaiInterfaceSlot slot)
        {
            CameraShaker.MediumShake(ShakeSettings.Ship * 0.66f);
        }
        public static void OnChangeAnglerState(AnglerfishAudioController __instance, AnglerfishController.AnglerState anglerState)
        {
            if (anglerState == AnglerfishController.AnglerState.Chasing)
            {
                if (Time.time > AnglerfishAudioController.s_lastDetectTime + 2f)
                {
                    CameraShaker.ExplosionShake(ShakeSettings.Environment * 7f, 1.25f, null, 10f);
                }
            }
        }

        //--------------------------------------------- Advanced Shakes ---------------------------------------------//
        public static void SandFunnelUpdate(SandFunnelController __instance) => AdvancedShakes.SandFunnelUpdate(__instance);
        public static void JetpackThrusterAudio_Update(JetpackThrusterAudio __instance) => AdvancedShakes.JetpackThrusterUpdate(__instance);
        public static void StartFlood(RingRiverController __instance) => AdvancedShakes.ActivateRiverShake(__instance);
        public static void RingRiver_Update(RingRiverController __instance) => AdvancedShakes.UpdateRiverShake(__instance);
        public static void SunSurfaceAudioController_Update(SunSurfaceAudioController __instance) => AdvancedShakes.HandleSunShake(__instance);
        public static void SupernovaEffectController_FixedUpdate(SupernovaEffectController __instance) => AdvancedShakes.HandleSupernovaShake(__instance);

#if NOMAI_DOOR
        static PerlinShake doorShake;
        public static void NomaiMultiPartDoor_Open(NomaiMultiPartDoor __instance, NomaiInterfaceSlot slot)
        {
            DoMultiPartDoorShake(__instance);
            Log.Success("Open Multi-part Door");
        }
        public static void NomaiMultiPartDoor_FixedUpdate(NomaiMultiPartDoor __instance)
        {
            if (doorShake != null) doorShake.UpdateSourcePosition(__instance.transform.position);
        }
        public static void NomaiMultiPartDoor_Close(NomaiMultiPartDoor __instance, NomaiInterfaceSlot slot)
        {
            DoMultiPartDoorShake(__instance);
        }
        static void DoMultiPartDoorShake(NomaiMultiPartDoor __instance)
        {
            if (doorShake != null) doorShake.ForceDecay(0.5f);
            bool isAirlock = __instance is NomaiAirlock;
            var pos = __instance.transform.position;

            if (isAirlock)
            {
                doorShake = CameraShaker.ExplosionShake(0.5f * ShakeSettings.Environment, 5f, pos, 0.5f, 3f, 15f);
            }
            else
            {
                doorShake = CameraShaker.ExplosionShake(0.5f * ShakeSettings.Environment, 6f, pos, 0.5f, 3f, 15f);
            }
        }

        static PerlinShake gateShake;
        public static void NomaiGateway_OpenGate(NomaiGateway __instance, NomaiInterfaceSlot slot)
        {
            DoGateShake(__instance);
            Log.Success("Open Gateway");
        }
        public static void NomaiGateway_FixedUpdate(NomaiGateway __instance)
        {
            if (gateShake != null) gateShake.UpdateSourcePosition(__instance.transform.position);
        }
        public static void NomaiGateway_CloseGate(NomaiGateway __instance, NomaiInterfaceSlot slot)
        {
            DoGateShake(__instance);
        }
        static void DoGateShake(NomaiGateway __instance)
        {
            if (gateShake != null) gateShake.ForceDecay(0.5f);
            var pos = __instance.transform.position;

            gateShake = CameraShaker.ExplosionShake(0.75f * ShakeSettings.Environment, 8f, pos, 0.5f, 5f, 20f);
        }
#endif

        //--------------------------------------------- Misc ---------------------------------------------//
        public static void MixMemoryUplink(OWAudioMixer __instance, float duration) => CameraShaker.ExplosionShake(1f, 8.2f, null, 2f);

        public static void TriggerCrackEffect(TimelineObliterationController __instance) => CameraShaker.ExplosionShake(30f, 25f, null, 0.15f);
        public static void StartVesselWarp(RumbleManager __instance) => CameraShaker.ExplosionShake(6f, 12f, null, 1f);
        public static void StartHotBigBang(CosmicInflationController __instance)
            => CameraShaker.ExplosionShake(6f * ShakeSettings.Explosions, 6f, null, 6f);

        public static void OnEyeStateChanged(PlayerState __instance, EyeState state)
        {
            switch (state)
            {
                case EyeState.AboardVessel: CameraShaker.ExplosionShake(6f, 3f, null, 10f); break;
                case EyeState.IntoTheVortex: CameraShaker.ExplosionShake(1f * ShakeSettings.Environment, 2f, null, 5f); break;

                //Don't do much?
                case EyeState.WarpedToSurface: CameraShaker.ExplosionShake(1f * ShakeSettings.Environment, 1f, null, 10f); break;
                case EyeState.ZoomOut: CameraShaker.ExplosionShake(1f * ShakeSettings.Environment, 1f, null, 10f); break;
            }
        }

        public static void LocateSceneObjects(Locator __instance)
        {
            var cam = Locator.GetPlayerCamera();
            if (cam == null)
            {
                var player = Object.FindObjectOfType<PlayerCharacterController>();
                if (player == null) return;
                cam = player.GetComponentInChildren<OWCamera>();
                if (cam == null) return;
            }

            var obj = cam.gameObject;
            if (obj.TryGetComponent(out CameraShaker s)) return; //Might not be needed, but just in case.

            var shaker = obj.AddComponent<CameraShaker>();
            GameObject shakeRoot = new GameObject("ShakeRoot");
            var shakeTF = shakeRoot.transform;
            var camTF = cam.mainCamera.transform;
            shakeTF.parent = camTF.parent;
            shakeTF.localPosition = Vector3.zero;
            shakeTF.localRotation = Quaternion.identity;
            camTF.parent = shakeTF;
            //Far camera is under main camera, so should be shook as well

            shaker.Initialize(shakeTF);
            AdvancedShakes.Initialize();

            Log.Success("Created Shake Root.");
        }
    }
}