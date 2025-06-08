using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;

[assembly: MelonInfo(typeof(BlinkMod.Core), "BlinkMod", "1.1.0", "szymky", null)]
[assembly: MelonGame("KeepsakeGames", "Jump Ship")]

namespace BlinkMod
{
    public class Core : MelonMod
    {
        private PlayerHandler PlayerHandler => Resources.FindObjectsOfTypeAll<PlayerHandler>().FirstOrDefault();
        private PlayerController PlayerController => PlayerHandler?.localPlayerController;
        private GameObject PlayerObject => PlayerHandler?.localPlayerGameobject;
        private Camera MainCamera => Camera.main;

        private Vector3? savedPosition;
        private Quaternion? savedRotation;

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("BlinkMod initialized.");
        }

        public override void OnUpdate()
        {
            if (Keyboard.current.f1Key.wasPressedThisFrame || Mouse.current.forwardButton.wasPressedThisFrame)
                BlinkForward();

            if (Keyboard.current.f2Key.wasPressedThisFrame)
                BlinkThroughWall();

            if (Keyboard.current.f3Key.wasPressedThisFrame)
                RespawnPlayer();

            if (Keyboard.current.f4Key.wasPressedThisFrame)
                SaveLocation();

            if (Keyboard.current.f5Key.wasPressedThisFrame)
                ReturnToSavedLocation();
        }

        private void BlinkForward()
        {
            if (PlayerController == null || PlayerObject == null || MainCamera == null) return;

            if (Physics.Raycast(MainCamera.transform.position, MainCamera.transform.forward, out RaycastHit hit, 10000f))
            {
                Vector3 targetPos = hit.point;
                Quaternion targetRot = MainCamera.transform.rotation;

                PlayerController.Teleport(ref targetPos, ref targetRot);
                MelonLogger.Msg($"Teleported forward to: {targetPos}");
            }
        }

        private void BlinkThroughWall()
        {
            if (PlayerController == null || PlayerObject == null || MainCamera == null) return;

            CapsuleCollider capsule = PlayerObject.GetComponent<CapsuleCollider>();
            if (capsule == null)
            {
                MelonLogger.Msg("No CapsuleCollider found on player.");
                return;
            }

            if (!Physics.Raycast(MainCamera.transform.position, MainCamera.transform.forward, out RaycastHit hit, 10000f))
            {
                MelonLogger.Msg("No wall was hit.");
                return;
            }

            const float step = 0.1f;
            const float maxDistance = 10f;

            Vector3 probe = hit.point + MainCamera.transform.forward * step;
            float radius = capsule.radius;
            float height = Mathf.Max(capsule.height, radius * 2f);
            Vector3 center = capsule.center;
            bool spaceFound = false;

            for (float dist = 0; dist < maxDistance; dist += step)
            {
                Vector3 worldCenter = probe + PlayerObject.transform.rotation * center;
                Vector3 bottom = worldCenter + Vector3.up * -(height / 2f - radius);
                Vector3 top = worldCenter + Vector3.up * (height / 2f - radius);

                if (!Physics.CheckCapsule(bottom, top, radius, ~0, QueryTriggerInteraction.Ignore))
                {
                    spaceFound = true;
                    break;
                }

                probe += MainCamera.transform.forward * step;
            }

            if (spaceFound)
            {
                Vector3 teleportPos = probe;
                Quaternion teleportRot = MainCamera.transform.rotation;

                PlayerController.Teleport(ref teleportPos, ref teleportRot);
                MelonLogger.Msg($"Teleported through wall to open space at: {teleportPos}");
            }
            else
            {
                MelonLogger.Msg("No open space found beyond the wall.");
            }
        }

        private void RespawnPlayer()
        {
            if (PlayerController == null) return;
            PlayerController.RespawnLocalPlayer();
            MelonLogger.Msg("Player respawned.");
        }

        private void SaveLocation()
        {
            if (PlayerObject == null) return;
            savedPosition = PlayerObject.transform.position;
            savedRotation = PlayerObject.transform.rotation;
            MelonLogger.Msg("Position saved.");
        }

        private void ReturnToSavedLocation()
        {
            if (PlayerController == null || savedPosition == null || savedRotation == null) return;
            Vector3 pos = savedPosition.Value;
            Quaternion rot = savedRotation.Value;
            PlayerController.Teleport(ref pos, ref rot);
            MelonLogger.Msg("Returned to saved position.");
        }
    }
}
