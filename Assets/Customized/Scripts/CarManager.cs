using System.Collections.Generic;
using UnityEngine;

namespace Customized.Scripts
{
    public class CarManager : MonoBehaviour
    {
        public List<DoorData> DoorDataList = new List<DoorData>();
        public Transform CarSteerLeftHandTransform = null;
        public Transform steer = null;
        public Transform CarSteerRightHandTransform = null;
        public Transform RightSeatEnterPosTransform = null;
        public Transform wheelRefernce = null;
        public bool isUsingByPlayer;

        [Space(10)] [Header("CHECK COLLIDER")] public float rayDistance;

        public DoorData GetAvailableDoorData()
        {
            foreach (var dt in DoorDataList)
            {
                if (dt.side == Side.Left && CheckIfDoorAvailable(dt))
                {
                    return dt;
                }
            }

            foreach (var dt in DoorDataList)
            {
                if (CheckIfDoorAvailable(dt))
                {
                    return dt;
                }
            }

            Debug.LogError("No available door");
            return null;
        }

        private bool CheckIfDoorAvailable(DoorData DT)
        {
            Transform doorMeshTransform = DT.doorMesh.transform;
            Vector3 rayOrigin = doorMeshTransform.position + doorMeshTransform.right * 0.4f;
            Vector3 dir = DT.side == Side.Left ? -doorMeshTransform.forward : doorMeshTransform.forward;
            Ray ray = new Ray(rayOrigin, dir);

            Debug.DrawRay(rayOrigin, dir * rayDistance, Color.red, rayDistance);

            if (Physics.Raycast(ray, out var hit, rayDistance))
            {
                if (hit.collider.TryGetComponent(out ObstacleController _))
                {
                    Debug.Log("Door is blocked by an obstacle. Door is: " + DT.doorMesh.gameObject.name);
                    return false;
                }
            }

            return true;
        }

        private void FixedUpdate()
        {
            if (!isUsingByPlayer) return;

            float wheelYRotation = wheelRefernce.transform.localEulerAngles.y - 90;
            steer.localRotation = Quaternion.Euler(wheelYRotation, 0, 15);
        }
    }

    [System.Serializable]
    public class DoorData
    {
        public Side side;

        public float DoorOpeningYRot = 0;

        public Transform standOffset; // The transform where player should stand before opening the left door
        public Transform doorMesh;
        public Transform OutHandlePoint;
        public Transform InHandlePoint;
    }
}