using System;
using UnityEngine;
using UnityEngine.UI;

namespace StarterAssets
{
    public class UICanvasControllerInput : MonoSingleton<UICanvasControllerInput>
    {
        #region Constant Implementations

        [Header("Output")] public StarterAssetsInputs starterAssetsInputs;

        public void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            starterAssetsInputs.MoveInput(virtualMoveDirection);
        }

        public void VirtualLookInput(Vector2 virtualLookDirection)
        {
            starterAssetsInputs.LookInput(virtualLookDirection);
        }

        public void VirtualJumpInput(bool virtualJumpState)
        {
            starterAssetsInputs.JumpInput(virtualJumpState);
        }

        public void VirtualSprintInput(bool virtualSprintState)
        {
            starterAssetsInputs.SprintInput(virtualSprintState);
        }

        #endregion

        public event Action DriveButtonClickedEvent;

        [Header("References")] [SerializeField]
        private Button driveButton;

        private void Start()
        {
            driveButton.onClick.AddListener((() => DriveButtonClickedEvent?.Invoke()));
        }
    }
}