using UnityEngine;

namespace CameraShake
{
    public sealed class CameraShakerAPI : ICameraShaker
    {
        public void SubtleShake(float strength = 1f)
        {
            CameraShaker.ShortShake(strength * 0.18f);
        }
        public void MediumShake(float strength = 1f)
        {
            CameraShaker.ShortShake(strength, 17f, 12);
        }
        public void ShortShake(float strength = 0.3f, float freq = 25f, int numBounces = 5)
        {
            CameraShaker.ShortShake(strength, freq, numBounces);
        }
        public void ExplosionShake(float strength = 8f, float duration = 0.7f, Vector3? sourcePosition = null,
            float fadeInSpeed = 10f, float minDist = 10f, float maxDist = 60f)
        {
            CameraShaker.ExplosionShake(strength, duration, sourcePosition, fadeInSpeed, minDist, maxDist);
        }
        public void StopAllShakes()
        {
            CameraShaker.StopAllShakes();
        }

        public float Explosions => ShakeSettings.Explosions;
        public float Environment => ShakeSettings.Environment;
        public float Ship => ShakeSettings.Ship;
        public float Player => ShakeSettings.Player;
        public float Scout => ShakeSettings.Scout;
        public float Jetpack => ShakeSettings.Jetpack;
    }
}