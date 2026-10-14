using UnityEngine;
using UnityEngine.InputSystem;

namespace FootballElite
{
    /// <summary>Unified PC, gamepad and multitouch controls with no legacy Input dependency.</summary>
    public sealed class FootballInput : MonoBehaviour
    {
        [SerializeField] private TouchJoystick joystick;
        [SerializeField] private TouchButton passButton;
        [SerializeField] private TouchButton shootButton;
        [SerializeField] private TouchButton sprintButton;

        public Vector2 Move { get; private set; }
        public bool Sprint { get; private set; }
        public bool PassPressed { get; private set; }
        public bool ShootPressed { get; private set; }

        public void Configure(TouchJoystick movement, TouchButton passing, TouchButton shooting, TouchButton sprinting)
        {
            joystick = movement; passButton = passing; shootButton = shooting; sprintButton = sprinting;
        }

        private void Update()
        {
            Vector2 keyboardMove = Vector2.zero;
            bool pcPass = false;
            bool pcShoot = false;
            bool pcSprint = false;

            Keyboard kb = Keyboard.current;
            if (kb != null)
            {
                if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) keyboardMove.x -= 1f;
                if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) keyboardMove.x += 1f;
                if (kb.wKey.isPressed || kb.upArrowKey.isPressed) keyboardMove.y += 1f;
                if (kb.sKey.isPressed || kb.downArrowKey.isPressed) keyboardMove.y -= 1f;
                pcPass = kb.jKey.wasPressedThisFrame;
                pcShoot = kb.spaceKey.wasPressedThisFrame;
                pcSprint = kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed;
            }

            Gamepad pad = Gamepad.current;
            Vector2 gamepadMove = pad != null ? pad.leftStick.ReadValue() : Vector2.zero;
            bool padPass = pad != null && pad.buttonSouth.wasPressedThisFrame;
            bool padShoot = pad != null && pad.buttonEast.wasPressedThisFrame;
            bool padSprint = pad != null && pad.rightTrigger.ReadValue() > .3f;

            Vector2 touchMove = joystick != null ? joystick.Value : Vector2.zero;
            Move = Vector2.ClampMagnitude(
                touchMove.sqrMagnitude > .01f ? touchMove :
                (gamepadMove.sqrMagnitude > .05f ? gamepadMove : keyboardMove), 1f);
            Sprint = pcSprint || padSprint || (sprintButton != null && sprintButton.IsHeld);
            PassPressed = pcPass || padPass || (passButton != null && passButton.ConsumePress());
            ShootPressed = pcShoot || padShoot || (shootButton != null && shootButton.ConsumePress());
        }
    }
}
