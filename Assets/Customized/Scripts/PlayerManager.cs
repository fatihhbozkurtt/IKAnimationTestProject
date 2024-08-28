using System.Collections.Generic;
using DG.Tweening;
using DitzelGames.FastIK;
using StarterAssets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Sequence = DG.Tweening.Sequence;

namespace Customized.Scripts
{
    public class PlayerManager : MonoSingleton<PlayerManager>
    {
        public bool IsDrive = false;
        public bool IsWalk = false;

        [Space(10)] public List<CarManager> CarList;
        private CarManager CurrentActiveCar = null;
        private bool IsThereCarInRange = false;

        [Space(10)] public ThirdPersonController tpc;
        public CharacterController cc;
        public FastIKFabric LeftHandIK;
        public FastIKFabric RightHandIK;

        [Header("Debug")] [SerializeField] private Animator animator;
        private static readonly int EnterCar = Animator.StringToHash("EnterCar");


        protected override void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            IsWalk = true;
            UICanvasControllerInput.instance.DriveButtonClickedEvent += OnDriveButtonClicked;
            UICanvasControllerInput.instance.WalkButtonClickedEvent += OnWalkButtonClicked;
        }

        public void FixedUpdate()
        {
            if (IsWalk)
            {
                ControlCarInRange();
            }
        }

        public void ControlCarInRange()
        {
            float ClosestCarDistance = 999f;
            CurrentActiveCar = null;

            for (int i = 0; i < CarList.Count; i++)
            {
                float CharacterCarDistance = Vector3.Distance(transform.position, CarList[i].transform.position);
                if (CharacterCarDistance < ClosestCarDistance && CarList[i].WhichSide() != null)
                {
                    ClosestCarDistance = CharacterCarDistance;
                    CurrentActiveCar = CarList[i];
                }
            }

            if (IsThereCarInRange)
            {
                if (ClosestCarDistance >= 4f)
                {
                    IsThereCarInRange = false;
                    UICanvasControllerInput.instance.OnPlayerExitedTriggerZone();
                }
            }
            else
            {
                if (ClosestCarDistance < 4f)
                {
                    IsThereCarInRange = true;
                    UICanvasControllerInput.instance.OnPlayerEnteredTriggerZone();
                }
            }
        }

        private void OnDriveButtonClicked()
        {
            PerformEntering(CurrentActiveCar.WhichSide());
            IsDrive = true;
            IsWalk = false;
        }

        private void OnWalkButtonClicked()
        {
            PerformExit(CurrentActiveCar.WhichSide());
            CurrentActiveCar = null;
            IsDrive = false;
            IsWalk = true;
        }

        private void PerformEntering(DoorData doorData)
        {
            Vector3 EnterPosition = CurrentActiveCar.transform.position + doorData.standOffset;
            tpc.enabled = false;
            cc.enabled = false;
            transform.position = EnterPosition;
            transform.forward = doorData.doorMesh.transform.forward;
            //tpc.enabled = true;
            animator.SetTrigger(EnterCar);

            GameObject HandIkTransform = new GameObject("HandTarget");
            HandIkTransform.transform.SetParent(doorData.doorMesh.transform);
            Transform HandTransform = HandIkTransform.transform;
            HandTransform.position = LeftHandIK.transform.position;
            LeftHandIK.enabled = true;
            LeftHandIK.Target = HandTransform;
            Sequence sq = DOTween.Sequence();

            sq.Append(HandTransform.DOMove(doorData.OutHandlePoint.position, 1.02f));
            sq.Append(doorData.doorMesh.transform.DOLocalRotate(new Vector3(0, doorData.DoorOpeningYRot, 0), 1f)
                .OnComplete(() => { LeftHandIK.enabled = false; }));
        }

        private void PerformExit(DoorData doorData)
        {
        }
    }
}