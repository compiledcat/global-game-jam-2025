using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

// yoinked from https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/InputSystemComponents.html
public class CinemachinePlayerInputHandler : InputAxisControllerBase<CinemachinePlayerInputHandler.Reader>
{
    [Header("Input Source Override")] public PlayerInput PlayerInput;

    private void Awake()
    {
        // When the PlayerInput receives an input, send it to all the controllers
        if (PlayerInput == null)
            TryGetComponent(out PlayerInput);
        if (PlayerInput == null)
            Debug.LogError("Cannot find PlayerInput component");
        else
        {
            PlayerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
            PlayerInput.onActionTriggered += (value) =>
            {
                foreach (var c in Controllers)
                    c.Input.ProcessInput(value.action);
            };
        }
    }

    // We process user input on the Update clock
    private void Update()
    {
        if (Application.isPlaying)
            UpdateControllers();
    }

    // Controllers will be instances of this class.
    [Serializable]
    public class Reader : IInputAxisReader
    {
        public InputActionReference Input;
        private Vector2 _value; // the cached value of the input

        public void ProcessInput(InputAction action)
        {
            // If it's my action then cache the new value
            if (Input != null && Input.action.id == action.id)
            {
                if (action.expectedControlType == "Vector2")
                    _value = action.ReadValue<Vector2>();
                else
                    _value.x = _value.y = action.ReadValue<float>();
            }
        }

        // IInputAxisReader interface: Called by the framework to read the input value
        public float GetValue(UnityEngine.Object context, IInputAxisOwner.AxisDescriptor.Hints hint)
        {
            return (hint == IInputAxisOwner.AxisDescriptor.Hints.Y ? _value.y : _value.x);
        }
    }
}