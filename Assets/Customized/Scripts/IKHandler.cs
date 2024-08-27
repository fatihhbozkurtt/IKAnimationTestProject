using DG.Tweening;
using StarterAssets; 
using UnityEngine; 

namespace Customized.Scripts
{
    public class IKHandler : MonoBehaviour
    {
        [Header("Debug")] public Transform leftStandTr;
        [SerializeField] private Animator animator;
        private static readonly int EnterCar = Animator.StringToHash("EnterCar");

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            UICanvasControllerInput.instance.DriveButtonClickedEvent += OnDriveButtonClicked;
        }

        private void OnDriveButtonClicked()
        {
            animator.enabled = false;
            transform.DOMove(leftStandTr.position, 0.1f);  
            transform.forward = leftStandTr.forward;
            
            // animator.enabled = true;
            // animator.applyRootMotion = true;
            // animator.SetTrigger(EnterCar);
        }
    }
}