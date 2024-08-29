using System;
using Customized.Scripts;
using Unity.VisualScripting;
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
        public event Action WalkButtonClickedEvent;

        [Header("References")] [SerializeField]
        private Button driveButton;

        [SerializeField] private Button walkButton;
        [SerializeField] private GameObject playerJoystickSystem;

        private void Start()
        {
            driveButton.onClick.AddListener(OnDriveButtonClicked);
            walkButton.onClick.AddListener(OnWalkButtonClicked);
        }

        #region Action Events
        
        public void OnPlayerEnteredTriggerZone()
        {
            driveButton.gameObject.SetActive(true);
        }
        public void OnPlayerExitedTriggerZone()
        {
            driveButton.gameObject.SetActive(false);
        }
      
        public void OnPlayerGotOutTheCar()
        {
            walkButton.gameObject.SetActive(false);
            playerJoystickSystem.SetActive(true);
        }

        #endregion

        #region Button Events

        private void OnDriveButtonClicked()
        {
            DriveButtonClickedEvent?.Invoke();
            
            walkButton.gameObject.SetActive(true);
            driveButton.gameObject.SetActive(false);
            playerJoystickSystem.SetActive(false);
        }

        private void OnWalkButtonClicked()
        {
            WalkButtonClickedEvent?.Invoke();
        }

        #endregion
    }
}