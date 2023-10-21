using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class InputReader : MonoBehaviour, Controls.IPlayerActions
{
    //public event Action OnUndoPerformed;
    public event Action OnSelectPerformed;
    public Vector2 touchPosition;
    private Controls control;

    private void OnEnable(){
        if (control != null){
            control.Enable();
            return;
        }
        control = new Controls();
        control.Player.SetCallbacks(this);
        control.Player.Enable();
    }

    private void OnDisable(){
        control.Disable();
    }

    public void OnPosition(InputAction.CallbackContext context)
    {
        touchPosition = context.ReadValue<Vector2>();
    }

    public void OnPress(InputAction.CallbackContext context)
    {
        // release
        if (context.canceled){
            OnSelectPerformed?.Invoke();
        }
    }

    public void OnUndo(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        //OnUndoPerformed?.Invoke();
        CommandInvoker.UndoCommand();
    }
}
