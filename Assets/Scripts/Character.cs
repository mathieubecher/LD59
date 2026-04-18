using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Character : MonoBehaviour
{
    
    public delegate void SimpleEvent();
    public event SimpleEvent OnDead;
    public event SimpleEvent OnSpotted;
    public bool hidden => m_hidden;
    
    [SerializeField] private float m_moveSpeed = 5f;
    [SerializeField] private AnimationCurve m_bpmToMoveSpeedScale;
    [SerializeField] private TextMeshPro m_speedText;
    [SerializeField] private GameObject m_ring;
    [SerializeField] private Animator m_animator;

    [SerializeField] private LayerMask dangerZoneMask;
    [SerializeField] private LayerMask interactMask;

    private Rigidbody2D m_rigidbody;
    private float m_speedScale;
    private List<Transform> m_interacts = new ();
    private bool m_hidden;
    
    void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_speedScale = 0f;
    }

    private void OnEnable()
    {
        InputManager.OnInteractPress += Interact;
        GameManager.life.OnDead += Dead;
        ECG.OnBip += Bip;
    }
    private void OnDisable()
    {
        InputManager.OnInteractPress -= Interact;
        GameManager.life.OnDead -= Dead;
        ECG.OnBip -= Bip;
    }

    private void FixedUpdate()
    {
        if (GameManager.dead) return;
        float bpmSpeedScale = m_bpmToMoveSpeedScale.Evaluate(ECG.bpm);

        m_speedScale += (bpmSpeedScale - m_speedScale) * 0.2f;
        m_speedText.text = bpmSpeedScale < 0.2f ? "0" : bpmSpeedScale < 0.8f ? ">" : bpmSpeedScale < 1.2f ? ">>" : bpmSpeedScale < 2f ? ">>>" : "!!!";
        m_rigidbody.velocity = new Vector2(InputManager.move.x * m_moveSpeed * bpmSpeedScale, m_rigidbody.velocity.y);
        m_animator.SetFloat("move", m_rigidbody.velocity.x);
    }

    private void Bip()
    {
        Instantiate(m_ring, transform.position, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (GameManager.dead) return;
        if ((dangerZoneMask & (1 << _other.gameObject.layer)) != 0) GameManager.life.Dead();
        if((interactMask & (1 << _other.gameObject.layer)) != 0) m_interacts.Add(_other.transform);
    }

    private void OnTriggerExit2D(Collider2D _other)
    {
        if (GameManager.dead) return;
        if((interactMask & (1 << _other.gameObject.layer)) != 0) m_interacts.Remove(_other.transform);
    }

    private void Interact()
    {
        if (GameManager.dead) return;
        if (m_hidden)
        {
            m_hidden = false;
            m_animator.SetBool("hidden", false);
            m_rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }
        else
        {
            if (m_interacts.Count == 0) return;
            transform.position = m_interacts[0].position;
            m_hidden = true;
            m_animator.SetBool("hidden", true);
            m_rigidbody.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    public void Spotted()
    {
        if (GameManager.dead) return;
        m_hidden = false;
        m_animator.SetBool("hidden", false);
        
        OnSpotted?.Invoke();
        m_animator.SetTrigger("Spotted");
    }

    public void Dead()
    {
        if (GameManager.dead) return;
        m_hidden = false;
        m_animator.SetBool("hidden", false);
        
        OnDead?.Invoke();
        m_animator.SetTrigger("Dead");
    }
}
