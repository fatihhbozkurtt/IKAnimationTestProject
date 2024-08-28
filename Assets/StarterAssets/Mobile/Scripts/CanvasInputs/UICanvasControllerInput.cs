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

        

        private void OnPlayerGotOutTheCar(Transform availableDoorTr)
        {
            walkButton.gameObject.SetActive(false);
        }

        #endregion

        #region Button Events

        private void OnDriveButtonClicked()
        {
            DriveButtonClickedEvent?.Invoke();
            driveButton.gameObject.SetActive(false);
            walkButton.gameObject.SetActive(true);
        }

        private void OnWalkButtonClicked()
        {
            WalkButtonClickedEvent?.Invoke();
        }

        #endregion
    }
}