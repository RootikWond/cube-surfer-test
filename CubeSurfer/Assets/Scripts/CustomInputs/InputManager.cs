using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class InputManager : MonoBehaviour
{
    public event Action<Vector2, double> Pressed;
    public event Action<Vector2, double> Dragged;
    public event Action<Vector2, double> Released;

    public InputAction Delta
    {
        get { return m_Controls.Touch.Delta; }
        private set { }
    }

    public PlayerControls Controls => m_Controls;
    private PlayerControls m_Controls;

    private void Awake()
    {
        m_Controls = new PlayerControls();
        m_Controls.Touch.Contact.performed += OnTouchPerformed;
        m_Controls.Touch.Contact.canceled += OnTouchCanceled;
        m_Controls.Touch.Delta.performed += OnTouchDragged;
    }

    private void OnTouchPerformed(InputAction.CallbackContext context)
    {
        Pressed?.Invoke(m_Controls.Touch.Position.ReadValue<Vector2>(), context.time);

    }
    private void OnTouchCanceled(InputAction.CallbackContext context)
    {
        
        Released?.Invoke(m_Controls.Touch.Position.ReadValue<Vector2>(), context.duration);
    }
    private void OnTouchDragged(InputAction.CallbackContext context)
    {
       
        Dragged?.Invoke(context.ReadValue<Vector2>(), context.time);
    }
  
    private void OnEnable()
    {
        m_Controls?.Enable();
    }
    private void OnDisable()
    {
        m_Controls?.Disable();
    }




}
