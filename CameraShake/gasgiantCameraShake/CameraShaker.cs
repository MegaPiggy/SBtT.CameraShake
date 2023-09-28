using System.Collections.Generic;
using UnityEngine;

namespace CameraShake
{
    /// <summary>
    /// <para> Global Functions: ShortShake, ExplosionShake, CustomShake, StopShake, StopAllShakes.</para>
    /// <para> Applies shakes additively. </para>
    /// </summary>
    public sealed class CameraShaker : MonoBehaviour
    {
        static PerlinShake empty => new PerlinShake(new PerlinShake.Params());

        /// <summary>
        /// A constant shake that you can turn on and off and control the strength of.
        /// </summary>
        /// <param name="shake">Reference to a PerlinShake field.</param>
        /// <param name="isActive">If the shake is currently active.</param>
        /// <param name="strength">Strength of the shake. Should ideally be zero when isActive changes.</param>
        public static void AdvancedShake(ref PerlinShake shake, bool isActive, float strength)
        {
            if (isActive)
            {
                if (shake == null) shake = ExplosionShakeConstant(1f, null, 1000f);

                var d = PerlinShake.GetRotationDisplacement(strength);
                shake.SetStrength(d);
            }
            else if (shake != null)
            {
                StopShake(shake);
                shake = null;
            }
        }

        /// <summary>Preset for "ShortShake".</summary>
        public static void MediumShake(float multiplier = 1f)
        {
            ShortShake(multiplier * 1.35f, 17f, 12);
        }
        /// <summary>Preset for "ShortShake".</summary>
        public static void SubtleShake(float multiplier = 1f)
        {
            ShortShake(multiplier * 0.18f);
        }

        /// <summary>Suitable for shorter shakes. Uses a bounce back and forth method. Only rotates camera.</summary>
        /// <param name="strength">Strength of the shake.</param>
        /// <param name="freq">Frequency of shaking.</param>
        /// <param name="numBounces">Number of vibrations before stop.</param>
        public static BounceShake ShortShake(float strength = 0.3f, float freq = 25f, int numBounces = 5)
        {
            BounceShake.Params pars = new BounceShake.Params
            {
                axesMultiplier = new Displacement(Vector3.zero, new Vector3(1f, 1f, 0.4f)),
                rotationStrength = strength,
                freq = freq,
                numBounces = numBounces
            };
            var shake = new BounceShake(pars);
            CustomShake(shake);
            return shake;
        }

        /// <summary> <para> Has to be stopped manually.</para>
        /// <para>Either with thisShake.ForceDecay(newDecayLength) or with CameraShaker.StopShake(thisShake).</para> </summary>
        public static PerlinShake ExplosionShakeConstant(float strength = 8f, Vector3? sourcePosition = null,
            float fadeInSpeed = 10f, float minDist = 10f, float maxDist = 60f)
        {
            var shake = ExplosionShake(strength, 100000f, sourcePosition, fadeInSpeed, minDist, maxDist);
            shake.NoDecay();
            return shake;
        }

        /// <summary>Suitable for longer and stronger shakes. Uses a Perlin noise method. Only rotates camera.</summary>
        /// <param name="strength">Strength of the shake.</param>
        /// <param name="duration">Duration of the shake.</param>
        /// <param name="sourcePosition">Origin of the explosion. Leave null if don't want.</param>
        public static PerlinShake ExplosionShake(float strength = 8f, float duration = 0.7f, Vector3? sourcePosition = null,
            float fadeInSpeed = 10f, float minDist = 10f, float maxDist = 60f)
        {
            if (duration < 15f) //Only allow skipping if short shake.
            {
                if (strength <= Mathf.Epsilon) return empty;
                if (sourcePosition != null) //Skip doing shake if too far from source to affect.
                {
                    if (Vector3.Distance(Instance.camTF.position, sourcePosition.Value) > maxDist) return empty;
                }
            }

            PerlinShake.NoiseMode[] modes =
            {
                new PerlinShake.NoiseMode(6f, 1f),
                new PerlinShake.NoiseMode(20f, 0.2f)
            };
            Envelope.EnvelopeParams envelopePars = new Envelope.EnvelopeParams();
            envelopePars.decay = duration <= 0f ? 1f : 1f / duration;
            envelopePars.attack = fadeInSpeed;
            PerlinShake.Params pars = new PerlinShake.Params
            {
                strength = new Displacement(Vector3.zero, new Vector3(1f, 1f, 0.5f) * strength),
                noiseModes = modes,
                envelope = envelopePars,
            };

            if (sourcePosition != null)
            {
                pars.attenuation = new Attenuator.StrengthAttenuationParams()
                {
                    clippingDistance = minDist,
                    axesMultiplier = new Vector3(1f, 1f, 1f),
                    falloffDegree = Degree.Cubic,
                    falloffScale = Mathf.Max(minDist, maxDist - minDist),
                };
            }

            var shake = new PerlinShake(pars, 1f, sourcePosition);
            CustomShake(shake);
            return shake;
        }

        private static CameraShaker Instance;
        private readonly List<ICameraShake> activeShakes = new List<ICameraShake>();
        private Transform camTF;

        /// <summary>
        /// <para> Add a custom shake to the list of active shakes. </para>
        /// <para> See: <see cref="BounceShake"/>, <see cref="PerlinShake"/>, <see cref="KickShake"/> </para></summary>
        public static void CustomShake(ICameraShake shake)
        {
            if (!IsInstanceNull()) Instance.RegisterShake(shake);
        }
        /// <summary>Stop a shake from the list of active shakes.</summary>
        public static void StopShake(ICameraShake shake)
        {
            if (!IsInstanceNull()) Instance.RemoveShake(shake);
        }
        /// <summary>Stop all active shakes.</summary>
        public static void StopAllShakes()
        {
            if (!IsInstanceNull()) Instance.ClearShakes();
        }


        /// <summary>
        /// <para> Add a custom shake to the list of active shakes. </para>
        /// <para> See: <see cref="BounceShake"/>, <see cref="PerlinShake"/>, <see cref="KickShake"/> </para></summary>
        void RegisterShake(ICameraShake shake)
        {
            shake.Initialize(camTF.position, camTF.rotation);
            activeShakes.Add(shake);

            Log.Print($"{shake.GetType()}");
        }

        /// <summary>Sets the transform which will be affected by the shakes.</summary>
        public void Initialize(Transform cameraTransform)
        {
            if (Instance != null) { Log.Warning("Camera Shaker already initialized."); return; }

            camTF = cameraTransform;
            Instance = this;
        }

        private void Update()
        {
            if (camTF == null) return;

            Displacement cameraDisplacement = Displacement.Zero;
            for (int i = activeShakes.Count - 1; i >= 0; i--)
            {
                if (activeShakes[i].IsFinished)
                {
                    activeShakes.RemoveAt(i);
                }
                else
                {
                    activeShakes[i].Update(Time.deltaTime, camTF.position, camTF.rotation);
                    cameraDisplacement += activeShakes[i].CurrentDisplacement;
                }
            }
            camTF.localPosition = ShakeSettings.Master * cameraDisplacement.position;
            camTF.localRotation = Quaternion.Euler(ShakeSettings.Master * cameraDisplacement.eulerAngles);
        }

        /// <summary>Stop a shake from the list of active shakes.</summary>
        public void RemoveShake(ICameraShake shake)
        {
            activeShakes.Remove(shake);
        }

        /// <summary>Stop all active shakes.</summary>
        public void ClearShakes()
        {
            activeShakes.Clear();
        }

        private static bool IsInstanceNull()
        {
            if (Instance == null)
            {
                Log.Error("CameraShaker Instance is missing!");
                return true;
            }
            return false;
        }
    }
}