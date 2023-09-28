using OWML.Common;
using OWML.ModHelper;
using System;
using UnityEngine;

namespace CameraShake.BuiltInShakes
{
    public static class AdvancedShakes
    {
        static PerlinShake shakeEmber;
        static PerlinShake shakeAsh;
        public static void SandFunnelUpdate(SandFunnelController __instance)
        {
            if (__instance._scaleRoot.localScale.x < 0.1f) return;

            var posEmber = __instance._audioSources[0].transform.position;
            HandleSandFunnelShake(ref shakeEmber, posEmber, 3f, __instance);

            var posAsh = __instance._audioSources[1].transform.position;
            HandleSandFunnelShake(ref shakeAsh, posAsh, 1f, __instance);
        }
        static void HandleSandFunnelShake(ref PerlinShake shake, Vector3 pos, float strength, SandFunnelController __instance)
        {
            var player = Locator.GetPlayerBody()._transform;
            float dist = Vector3.Distance(pos, player.position);

            float maxDist = 150f;
            if (dist <= maxDist)
            {
                float s = strength * ShakeSettings.Explosions * __instance._audioSources[0].volume;
                if (shake == null)
                {
                    shake = CameraShaker.ExplosionShakeConstant(s, pos, 1000f, 20f, maxDist);
                }

                var d = new Displacement(Vector3.zero, PerlinShake.defaultEuler * s);
                if (__instance._playerInTimeLoop) d.eulerAngles *= 0f;
                if (PlayerState.IsInsideShip()) d.eulerAngles *= 0.8f;

                shake.SetStrength(d);
                shake.UpdateSourcePosition(pos);
            }
            else if (shake != null)
            {
                CameraShaker.StopShake(shake);
                shake = null;
            }
        }
        static PerlinShake sunShake;
        static PerlinShake supernovaShake;
        public static void HandleSunShake(SunSurfaceAudioController __instance)
        {
            var a = __instance._audioSource;
            if (a == null) return;
            bool isActive = a.isPlaying && a.volume > Mathf.Epsilon;
            float strength = ShakeSettings.Environment * a.volume * a.volume * 2f;
            CameraShaker.AdvancedShake(ref sunShake, isActive, strength);
        }
        public static void HandleSupernovaShake(SupernovaEffectController __instance)
        {
            var a = __instance._audioSource;
            if (a == null) return;
            bool isActive = a.isPlaying && a.volume > Mathf.Epsilon;
            float strength = ShakeSettings.Explosions * a.volume * a.volume * 4f;
            CameraShaker.AdvancedShake(ref supernovaShake, isActive, strength);
        }

        static PerlinShake playerBoostShake;
        static float t = 0f;
        public static void JetpackThrusterUpdate(JetpackThrusterAudio __instance)
        {
            var wasBoosting = __instance._wasBoosting;
            var nowBoosting = ((JetpackThrusterModel)__instance._thrusterModel).IsBoosterFiring();

            if (!wasBoosting && nowBoosting)
            {
                playerBoostShake = CameraShaker.ExplosionShake(1f * ShakeSettings.Jetpack, 0.3f, null, 5f);
            }
            if (wasBoosting && !nowBoosting)
            {
                if (t < 0.3f && playerBoostShake != null) playerBoostShake.ForceDecay();
                t = 0f;
            }

            if (nowBoosting)
            {
                t += Time.deltaTime;
                playerBoostShake.ForceSustain();
            }
        }

        static PerlinShake riverShake;
        static PerlinShake riverShakeDreamworld;
        static bool finishedRiverShake;
        public static void ActivateRiverShake(RingRiverController __instance)
        {
            if (!(PlayerState.InCloakingField() || PlayerState.InDreamWorld())) return;

            finishedRiverShake = false;
            riverShake = CameraShaker.ExplosionShakeConstant(3f * ShakeSettings.Environment, WavePos, 10f, 10f, 200f);
            riverShakeDreamworld = CameraShaker.ExplosionShakeConstant(1.5f * ShakeSettings.Environment);
        }
        public static void UpdateRiverShake(RingRiverController __instance)
        {
            if (finishedRiverShake) return;
            if (riverShake == null || riverShakeDreamworld == null) return;

            if (PlayerState.InDreamWorld())
            {
                var dreamWorldAudio = Locator.GetDreamWorldAudioController();
                var d = PerlinShake.GetRotationDisplacement(dreamWorldAudio._waveSource.volume * 1.5f * ShakeSettings.Environment);
                riverShakeDreamworld.SetStrength(d);
            }
            else
            {
                riverShakeDreamworld.SetStrength(Displacement.Zero);
            }

            riverShake.UpdateSourcePosition(WavePos);

            if (__instance._floodComplete)
            {
                riverShake.ForceDecay(1f);
                riverShakeDreamworld.ForceDecay(1f);
                finishedRiverShake = true;
                riverShake = null;
                riverShakeDreamworld = null;
            }
        }

        static Vector3 WavePos => Locator.GetRingWorldController()._riverController._waveAudio._audioSource.transform.position;
    }
}