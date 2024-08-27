using System;
using StarterAssets;
using UnityEngine;

namespace Customized.Scripts
{
    public class CarColliderHandler : MonoSingleton<CarColliderHandler>
    {
        public event Action<bool> PlayerEnteredZoneEvent;
        public event Action PlayerExitedZoneEvent;
        public event Action<Transform> PlayerGetOutOfCarEvent;
        
        [Header("References")] [SerializeField]
        private Transform leftDoor;

        [SerializeField] private Transform rightDoor;

        [Header("Config")] [SerializeField] private float rayDistance;
        private Vector3 _rayOffset;

        private void Start()
        {
            UICanvasControllerInput.instance.WalkButtonClickedEvent += OnWalkButtonClicked;
        }

        private void OnWalkButtonClicked()
        {
            bool hasAvailableDoor = CheckIfDoorAvailable(leftDoor) ||
                                    CheckIfDoorAvailable(rightDoor);

            if (!hasAvailableDoor)
            {
                Debug.LogWarning("NO AVAILABLE DOOR, PLAYER CANNOT GET OUT OF THE CAR");
                return;
            }
            
            bool isLeftDoorAvailable = CheckIfDoorAvailable(leftDoor);
            PlayerGetOutOfCarEvent?.Invoke(isLeftDoorAvailable ? leftDoor : rightDoor);
            if(isLeftDoorAvailable)
                Debug.Log("LEFTTT");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.TryGetComponent(out ThirdPersonController player))
            {
                Transform closestDoor = GetClosestDoor(player.transform.position);
                Transform alternateDoor = closestDoor == leftDoor ? rightDoor : leftDoor;

                bool hasAvailableDoor = CheckIfDoorAvailable(closestDoor) ||
                                        CheckIfDoorAvailable(alternateDoor);

                bool usingAlternate = !CheckIfDoorAvailable(closestDoor) &&
                                      CheckIfDoorAvailable(alternateDoor);
                if (hasAvailableDoor)
                {
                    Debug.Log("Door is available to open: " +
                              (usingAlternate ? alternateDoor.gameObject.name : closestDoor.gameObject.name));
                }

                PlayerEnteredZoneEvent?.Invoke(hasAvailableDoor);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform.TryGetComponent(out ThirdPersonController _))
            {
                PlayerExitedZoneEvent?.Invoke();
            }
        }

        private bool CheckIfDoorAvailable(Transform closestDoorTr)
        {
            Vector3 rayOrigin = closestDoorTr.position + closestDoorTr.forward * -0.4f;
            Vector3 dir = closestDoorTr == leftDoor ? -closestDoorTr.right : closestDoorTr.right;
            Ray ray = new Ray(rayOrigin, dir);

            Debug.DrawRay(rayOrigin, dir * rayDistance, Color.red, rayDistance);

            if (Physics.Raycast(ray, out var hit, rayDistance))
            {
                if (hit.collider.TryGetComponent(out ObstacleController obstacle))
                {
                    Debug.Log("Door is blocked by an obstacle. Door is: " + closestDoorTr.gameObject.name);
                    return false;
                }
            }

            return true; // Door is available
        }

        private Transform GetClosestDoor(Vector3 playerPos)
        {
            float leftDoorDist = Vector3.Distance(playerPos, leftDoor.position);
            float rightDoorDist = Vector3.Distance(playerPos, rightDoor.position);

            return leftDoorDist < rightDoorDist ? leftDoor : rightDoor;
        }
    }
}