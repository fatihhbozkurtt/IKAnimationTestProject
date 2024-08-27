using DG.Tweening;
using DitzelGames.FastIK;
using StarterAssets;
using UnityEngine;
using UnityEngine.Serialization;

namespace Customized.Scripts
{
    public class IKHandler : MonoSingleton<IKHandler>
    {
        [Header("Left Side")] public FastIKFabric leftIkFabric; // Fast IK Component on the left hand
        public Transform leftTarget; // target transform that left hand follows
        public Transform leftStandTr; // The transform where player should stand before opening the left door
        public Transform leftDoorMesh;
        public Transform leftDoorHolding1;
        public Transform leftDoorHolding2;
        public Transform leftDoorHolding3;


        [Header("Debug")] [SerializeField] private Animator animator;
        private static readonly int EnterCar = Animator.StringToHash("EnterCar");

        protected override void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            UICanvasControllerInput.instance.DriveButtonClickedEvent += OnDriveButtonClicked;
        }


        private void OnDriveButtonClicked()
        {
            //animator.enabled = false;
            transform.position = leftStandTr.position;
            transform.forward = leftStandTr.forward;
            animator.applyRootMotion = true;
            animator.SetTrigger(EnterCar);
            Quaternion targetRotation = leftDoorMesh.rotation * Quaternion.Euler(0, 38, 0);

            Sequence sq = DOTween.Sequence();
            sq.Append(leftTarget.DOMove(leftDoorHolding1.position, 1f));
            sq.Append(leftDoorMesh.DORotateQuaternion(targetRotation, 1f));
            sq.Append(leftTarget.DOMove(leftDoorHolding2.position, 1f));
            sq.OnComplete((OnCompleted));

            return;

            void OnCompleted()
            {
                //leftIkFabric.enabled = false;
            }
        }
    }
}