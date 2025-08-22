using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Data/Input/InputReader")]
    public class InputReader : ScriptableObject, IInputReader, InputActions.IPlayerActions
    {
        public InputActions InputActions { get; private set; }

        public event Action<float> Move = delegate { };

        public void EnablePlayerActions()
        {
            if (InputActions == null)
            {
                InputActions = new InputActions();
                InputActions.Player.SetCallbacks(this);
            }

            InputActions.Enable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                Move?.Invoke(context.ReadValue<Vector2>().x);
            }
        }
    }
}