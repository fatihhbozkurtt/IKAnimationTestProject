using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DitzelGames.FastIK;
using StarterAssets;
using UnityEngine;

namespace Customized.Scripts
{
    public class PlayerManager : MonoSingleton<PlayerManager>
    {
        public event Action PlayerSuccessfullyExitedCarEvent;
        public event Action PerfomEnterindEndedEvent;
        
        [Header("Config")] public List<CarManager> CarList;

        [Space(10)] [Header("References")] public ThirdPersonController tpc;
        public CharacterController cc;
        public FastIKFabric LeftHandIK;
        public FastIKFabric RightHandIK;
        public Animator animator;

        [Space(10)] [Header("Debug")] [HideInInspector]
        public bool isDrive = false;

        [HideInInspector] public bool isWalk;
        private CarManager _currentActiveCar;
        private bool _isThereCarInRange;
        private static readonly int EnterCarLeftAnim = Animator.StringToHash("EnterCarLeft");
        private static readonly int EnterCarRightAnim = Animator.StringToHash("EnterCarRight");
        private static readonly int ExitCarAnim = Animator.StringToHash("ExitCar");

        protected override void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            isWalk = true;
            UICanvasControllerInput.instance.DriveButtonClickedEvent += OnDriveButtonClicked;
            UICanvasControllerInput.instance.WalkButtonClickedEvent += OnWalkButtonClicked;
        }

        public void FixedUpdate()
        {
            if (!isWalk) return;

            ControlCarInRange();
        }

        private void ControlCarInRange()
        {
            float closestCarDistance = 999f;
            _currentActiveCar = null;

            for (int i = 0; i < CarList.Count; i++)
            {
                float characterCarDistance = Vector3.Distance(transform.position, CarList[i].transform.position);
                if (characterCarDistance < closestCarDistance && CarList[i].GetAvailableDoorData() != null)
                {
                    closestCarDistance = characterCarDistance;
                    _currentActiveCar = CarList[i];
                }
            }

            if (_isThereCarInRange)
            {
                if (closestCarDistance >= 4f)
                {
                    _isThereCarInRange = false;
                    UICanvasControllerInput.instance.OnPlayerExitedTriggerZone();
                }
            }
            else
            {
                if (closestCarDistance < 4f)
                {
                    _isThereCarInRange = true;
                    UICanvasControllerInput.instance.OnPlayerEnteredTriggerZone();
                }
            }
        }

        private void OnDriveButtonClicked()
        {
            StartCoroutine(PerformEntering(_currentActiveCar.GetAvailableDoorData()));
            isDrive = true;
            isWalk = false;
        }

        private void OnWalkButtonClicked()
        {
            if (_currentActiveCar.GetAvailableDoorData() == null) return;

            Debug.LogWarning("Performing exit");
            PlayerSuccessfullyExitedCarEvent?.Invoke();
            UICanvasControllerInput.instance.OnPlayerGotOutTheCar();

            LeftHandIK.enabled = false;
            RightHandIK.enabled = false;

            transform.SetParent(null);
            _currentActiveCar.isUsingByPlayer = false;
            isDrive = false;
            isWalk = true;

            transform.position = _currentActiveCar.GetAvailableDoorData().standOffset.position;

            tpc.enabled = true;
            cc.enabled = true;
            _currentActiveCar = null;

            animator.SetTrigger(ExitCarAnim);
        }

        private IEnumerator PerformEntering(DoorData doorData)
        {
            tpc.enabled = false;
            cc.enabled = false;

            Vector3 enterPosition = doorData.standOffset.position;
            transform.position = enterPosition;
            transform.forward = doorData.side == Side.Left
                ? doorData.doorMesh.transform.forward
                : -doorData.doorMesh.transform.forward;

            GameObject handIK = new GameObject("HandTarget");
            Transform handIKTransform = handIK.transform;
            GameObject handIKLook = new GameObject("HandLook");
            Transform handIKLookTransform = handIKLook.transform;
            handIKTransform.transform.SetParent(doorData.doorMesh.transform);

            FastIKFabric choosenFastIKFabric = null;
            if (doorData.side == Side.Left)
            {
                choosenFastIKFabric = LeftHandIK;
                animator.SetTrigger(EnterCarLeftAnim);
            }

            if (doorData.side == Side.Right)
            {
                choosenFastIKFabric = RightHandIK;
                animator.SetTrigger(EnterCarRightAnim);
            }


            handIKTransform.position = choosenFastIKFabric.transform.position;
            handIKLookTransform.position =
                enterPosition + new Vector3(0, 2, 0)
                              + (doorData.side == Side.Left
                                  ? doorData.doorMesh.transform.right
                                  : -doorData.doorMesh.transform.right) * 0.3f;
            choosenFastIKFabric.enabled = true;
            choosenFastIKFabric.Target = handIKTransform;
            choosenFastIKFabric.Pole = handIKLookTransform;

            yield return new WaitForSeconds(0.3f);
            handIKTransform.DOMove(doorData.OutHandlePoint.position, 0.72f);
            yield return new WaitForSeconds(0.72f);
            doorData.doorMesh.transform.DOLocalRotate(new Vector3(0, doorData.DoorOpeningYRot, 0), 0.6f)
                .SetEase(Ease.OutCubic);
            yield return new WaitForSeconds(0.6f);
            choosenFastIKFabric.enabled = false;
            handIKTransform.SetParent(choosenFastIKFabric.transform);
            handIKTransform.transform.localPosition = Vector3.zero;
            yield return new WaitForSeconds(1.5f);
            handIKTransform.SetParent(null);
            choosenFastIKFabric.enabled = true;
            handIKTransform.DOMove(doorData.InHandlePoint.position, 0.5f).SetEase(Ease.OutCubic);
            yield return new WaitForSeconds(0.5f);
            handIKTransform.SetParent(doorData.doorMesh);
            doorData.doorMesh.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.6f).SetEase(Ease.OutCubic);
            yield return new WaitForSeconds(1.5f);
            choosenFastIKFabric.enabled = false;


            LeftHandIK.enabled = false;
            RightHandIK.enabled = false;
            Destroy(handIK);
            Destroy(handIKLook);


            #region Seat Swapping

            if (doorData.side == Side.Right) transform.position = _currentActiveCar.RightSeatEnterPosTransform.position;

            #endregion


            LeftHandIK.Target = _currentActiveCar.CarSteerLeftHandTransform;
            LeftHandIK.Pole = null;
            RightHandIK.Target = _currentActiveCar.CarSteerRightHandTransform;
            RightHandIK.Pole = null;
            LeftHandIK.enabled = true;
            RightHandIK.enabled = true;

            transform.SetParent(_currentActiveCar.transform);
            _currentActiveCar.isUsingByPlayer = true;
            
            PerfomEnterindEndedEvent?.Invoke();
        }
    }
}