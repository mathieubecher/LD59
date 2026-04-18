using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DetectCharacter : MonoBehaviour
{
    public delegate void SimpleEvent();
    public event SimpleEvent OnCharacterSpotted;
    public event SimpleEvent OnCharacterLost;
    public Character character => m_detect ? m_character : null;
    
    [SerializeField] private float m_lostDistance = 5f;
    
    private Character m_character;
    private bool m_detect;
    
    private void FixedUpdate()
    {
        if (m_character && !m_detect)
        {
            m_detect = Math.Abs(math.sign((m_character.transform.position - transform.position).x) - math.sign(transform.lossyScale.x)) < 0.1f;
            if(m_detect) OnCharacterSpotted?.Invoke();
        }
        if (m_detect)
        {
            if ((transform.position - m_character.transform.position).magnitude >= m_lostDistance)
            {
                m_character = null;
                m_detect = false;
                OnCharacterLost?.Invoke();
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (m_character) return;
        m_character = _other.transform.parent.GetComponent<Character>();
    }
    private void OnTriggerExit2D(Collider2D _other)
    {
        if (m_detect) return;
        m_character = null;
    }
}
