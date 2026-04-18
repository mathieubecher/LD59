using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    #region Singleton
    private static InputManager m_instance;
    public static InputManager instance
    {
        get
        {
            if (!m_instance)
            {
                m_instance = FindAnyObjectByType<InputManager>();
            }
            return m_instance;
        }
    }
    public static Vector2 move => instance.m_move;
    public static Vector2 lastValidMove => instance.m_lastValidMove;
    #endregion
    
    [SerializeField] private AnimationCurve m_deadzone;
    private Vector2 m_move;
    private Vector2 m_lastValidMove;
    
    public delegate void SimpleEvent();
    public static event SimpleEvent OnInteractPress;
    public static event SimpleEvent OnInteractRelease;
    public static event SimpleEvent OnBipPress;
    public static event SimpleEvent OnBipRelease;
    
    private void Awake()
    {
        m_lastValidMove = Vector2.right;
    }
    
    public void ReadMoveInput(InputAction.CallbackContext _context)
    {
        Vector2 input = _context.ReadValue<Vector2>();
        float x = m_deadzone.Evaluate(input.x);
        float y = m_deadzone.Evaluate(input.y);
        if (math.abs(x) <= 0.01f) x = 0f;
        if (math.abs(y) <= 0.01f) y = 0f;

        if(x != 0.0f) m_lastValidMove.x = x;
        if(y != 0.0f) m_lastValidMove.y = y;
        m_move = new Vector2(x, y);
    }
    
    public void ReadInteractInput(InputAction.CallbackContext _context)
    {
        if (_context.performed)
        {
            OnInteractPress?.Invoke();
        }
        else if (_context.canceled)
        {
            OnInteractRelease?.Invoke();
        }
    }
    
    public void ReadBipInput(InputAction.CallbackContext _context)
    {
        if (_context.performed)
        {
            OnBipPress?.Invoke();
        }
        else if (_context.canceled)
        {
            OnBipRelease?.Invoke();
        }
    }

}
