using OWML.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CameraShake.DebugStuff
{
    public sealed class DebugControls
    {
        private IModHelper modHelper;
        public DebugControls(IModHelper modHelper)
        {
            this.modHelper = modHelper;
        }

        bool GetKeyDown(Key key)
        {
            return Keyboard.current[key].wasPressedThisFrame;
        }
        bool GetKeyUp(Key key)
        {
            return Keyboard.current[key].wasReleasedThisFrame;
        }
        public void OnUpdate()
        {
            if (GetKeyDown(Key.Y)) TeleportToVesselWithAdvancedWarpCore();
            if (GetKeyDown(Key.K)) ExplodeShip();

            if (GetKeyDown(Key.Comma)) Rewind60Seconds();
            if (GetKeyDown(Key.Period)) SkipForward60Seconds();

            if (Time.timeScale > 0.5f)
            {
                if (GetKeyDown(Key.Equals)) Time.timeScale = 20f;
                if (GetKeyUp(Key.Equals)) Time.timeScale = 1f;
            }
            if (GetKeyDown(Key.F1))
            {
                GUIMode.SetRenderMode(GUIMode.IsHiddenMode() ? GUIMode.RenderMode.FPS : GUIMode.RenderMode.Hidden);
            }
            if (GetKeyDown(Key.F2))
            {
                var suit = Locator.GetPlayerSuit();
                if (suit.IsWearingSuit()) suit.RemoveSuit();
                else suit.SuitUp();
            }
            if (GetKeyDown(Key.F4))
            {
                var resources = GameObject.FindObjectOfType<PlayerResources>();
                resources._currentFuel = PlayerResources._maxFuel;
                resources._currentOxygen = PlayerResources._maxOxygen;
                resources._currentHealth = PlayerResources._maxHealth;
            }
        }
        void Rewind60Seconds()
        {
            TimeLoop.SetSecondsRemaining(TimeLoop.GetSecondsRemaining() + 60f);
            PrintTimeAt("Rewind 60s to ");
        }
        void SkipForward60Seconds()
        {
            TimeLoop.SetSecondsRemaining(TimeLoop.GetSecondsRemaining() - 60f);
            PrintTimeAt("Skipped 60s to ");
        }
        void PrintTimeAt(string premessage)
        {
            var s = Mathf.FloorToInt(TimeLoop.GetSecondsElapsed());
            var m = Mathf.FloorToInt(TimeLoop.GetMinutesElapsed());
            Log.Print($"{premessage}{s}s ({m}m).");
        }

        //--------------------------------------------- Helper Functions ---------------------------------------------//
        void TeleportTo(OWRigidbody rb, Vector3 relativePosition = default, bool teleportShip = true)
        {
            var player = Locator._playerBody;
            var ship = Locator._shipBody;
            Vector3 pos = rb.GetPosition();
            Vector3 targetPos = pos + relativePosition;

            Quaternion rot = Quaternion.LookRotation(pos - targetPos);
            player.WarpToPositionRotation(targetPos, rot);
            if (teleportShip) ship.WarpToPositionRotation(targetPos, rot);

            var targetVelocity = rb.GetPointVelocity(targetPos);
            player.SetVelocity(targetVelocity);
            if (teleportShip) ship.SetVelocity(targetVelocity);
        }
        delegate bool ItemType<T>(T t);
        void GivePlayerItem<T>(ItemType<T> itemType) where T : OWItem
        {
            var items = Object.FindObjectsOfType<T>();
            foreach (T item in items)
            {
                if (itemType(item)) { GivePlayerItem(item); return; }
            }
            Log.Warning($"Could not find OWItem: {typeof(T)}.");
        }
        void GivePlayerItem(OWItem item) => Locator.GetToolModeSwapper().GetItemCarryTool().PickUpItemInstantly(item);

        void TeleportToVesselWithAdvancedWarpCore()
        {
            var vesselRB = GameObject.Find("DB_VesselDimension_Body").GetComponent<OWRigidbody>();
            TeleportTo(vesselRB, new Vector3(330f, -100f, 170f));
            
            GivePlayerItem<WarpCoreItem>(x => x.GetWarpCoreType() == WarpCoreType.Vessel);
        }
        void ExplodeShip()
        {
            GameObject.FindObjectOfType<ShipDamageController>().Explode();
        }
    }
}