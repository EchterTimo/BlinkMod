using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;

[assembly: MelonInfo(typeof(BlinkMod.Core), "BlinkMod", "1.0.0", "szymky", null)]
[assembly: MelonGame("KeepsakeGames", "Jump Ship")]

namespace BlinkMod
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
        }

        public override void OnUpdate()
        {

            if (Keyboard.current.f1Key.wasPressedThisFrame || Mouse.current.forwardButton.wasPressedThisFrame)
            {
                PlayerController playerController = Resources.FindObjectsOfTypeAll<PlayerHandler>().FirstOrDefault().localPlayerController;
                Camera mainCamera = Camera.main;
                RaycastHit hit;
                if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, 10000f))
                {
                    MelonLogger.Msg($"Hit: {hit.collider.gameObject.name} at {hit.point}");
                    Vector3 targetPosition = hit.point;
                    Quaternion targetRotation = mainCamera.transform.rotation;
                    playerController.Teleport(ref targetPosition, ref targetRotation);
                }
            }
            if (Keyboard.current.f3Key.wasPressedThisFrame)
            {
                PlayerController playerController = Resources.FindObjectsOfTypeAll<PlayerHandler>().FirstOrDefault().localPlayerController;
                playerController.RespawnLocalPlayer();
            }
            if (Keyboard.current.f2Key.wasPressedThisFrame)
            {
                var playerHandler = Resources.FindObjectsOfTypeAll<PlayerHandler>().FirstOrDefault();
                var playerController = playerHandler.localPlayerController;
                var playerObject = playerHandler.localPlayerGameobject;
                var capsule = playerObject.GetComponent<UnityEngine.CapsuleCollider>();

                if (capsule == null)
                {
                    MelonLogger.Msg("No CapsuleCollider found on player.");
                    return;
                }

                Camera mainCamera = Camera.main;
                Vector3 start = mainCamera.transform.position;
                Vector3 direction = mainCamera.transform.forward;

                RaycastHit hit;
                if (Physics.Raycast(start, direction, out hit, 10000f))
                {
                    MelonLogger.Msg($"Hit: {hit.collider.gameObject.name} at {hit.point}");

                    float step = 0.1f;
                    float maxSearchDistance = 10f;
                    float current = 0f;

                    Vector3 probePoint = hit.point + direction * step;

                    float capsuleRadius = capsule.radius;
                    float capsuleHeight = Mathf.Max(capsule.height, capsuleRadius * 2f);
                    Vector3 capsuleCenter = capsule.center;

                    bool foundSpace = false;

                    while (current < maxSearchDistance)
                    {
                        Vector3 worldCenter = probePoint + playerObject.transform.rotation * capsuleCenter;

                        Vector3 bottom = worldCenter + Vector3.up * -(capsuleHeight / 2f - capsuleRadius);
                        Vector3 top = worldCenter + Vector3.up * (capsuleHeight / 2f - capsuleRadius);

                        if (!Physics.CheckCapsule(bottom, top, capsuleRadius, ~0, QueryTriggerInteraction.Ignore))
                        {
                            foundSpace = true;
                            break;
                        }

                        probePoint += direction * step;
                        current += step;
                    }

                    if (foundSpace)
                    {
                        Vector3 teleportPosition = probePoint;
                        Quaternion teleportRotation = mainCamera.transform.rotation;
                        playerController.Teleport(ref teleportPosition, ref teleportRotation);
                        MelonLogger.Msg($"Teleported to open space at: {teleportPosition}");
                    }
                    else
                    {
                        MelonLogger.Msg("No open space found beyond the wall.");
                    }
                }
                else
                {
                    MelonLogger.Msg("No wall was hit.");
                }

            }
        }
    }
}
