using System;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

namespace Customized.Scripts
{
    public class CarManager : MonoBehaviour
    {
        public List<DoorData> DoorDataList = new List<DoorData>();
        
        public event Action<CarManager,bool> PlayerEnteredTriggerZoneEvent;
        public event Action <CarManager>PlayerExitedTriggerZoneEvent;
        public event Action<Transform> PlayerGetOutOfCarEvent;
        public event Action<Transform> PlayerGetInTheCarEvent;

        [Space(10)] [Header("CHECK COLLIDER")] public float rayDistance;
        public Vector3 _rayOffset;

        public DoorData WhichSide()
        {
            for (int i = 0; i < DoorDataList.Count; i++)
            {
                if (DoorDataList[i].side == Side.Left && CheckIfDoorAvailable(DoorDataList[i]))
                {
                    return DoorDataList[i];
                }
            }

            for (int i = 0; i < DoorDataList.Count; i++)
            {
                if (CheckIfDoorAvailable(DoorDataList[i]))
                {
                    return DoorDataList[i];
                }
            }

            return null;
        }

        private bool CheckIfDoorAvailable(DoorData DT)
        {
            Transform doorMeshTransform = DT.doorMesh.transform;
            Vector3 rayOrigin = doorMeshTransform .position + doorMeshTransform .forward * -0.4f;
            Vector3 dir = DT.side == Side.Left ? -doorMeshTransform .right : doorMeshTransform .right;
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
    }

    [System.Serializable]
    public class DoorData
    {
        public Side side;

        public float DoorOpeningYRot = 0;

        public Vector3 standOffset; // The transform where player should stand before opening the left door
        public Transform doorMesh;
        public Transform OutHandlePoint;
        public Transform InHandlePoint;
    }
}