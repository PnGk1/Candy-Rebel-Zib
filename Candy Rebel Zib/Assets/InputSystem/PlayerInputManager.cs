using UnityEngine;

namespace Baseplate.InputManager
{
    public class PlayerInputManager : MonoBehaviour
    {
        //Input System
        private PlayerControls playerControls;
        private void Awake()
        {
            playerControls = new PlayerControls();
        }

        private void OnEnable()
        {
            //Enable the InputSystem
            playerControls.Enable();
        }

        private void OnDisable()
        {
            //Disable the InputSystem
            playerControls.Disable();
        }

        public PlayerControls ExposePlayerControls()
        {
            return playerControls;
        }
    }
}
