using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class ECG : MonoBehaviour
{
    [SerializeField] private int m_frequency = 10;
    [SerializeField] private float m_speed;
    [SerializeField] private float m_boxLength = 100f;
    [SerializeField] private float m_boxRefresh = 0.2f;
    [Header("Bip")]
    [SerializeField] private Vector2 m_bipSize;
    [SerializeField] private AnimationCurve m_bip;
    [SerializeField] private TrailRenderer m_originalTrail;
    [SerializeField] private float m_minBipDuration;
    
    private List<TrailRenderer> m_trails;

    private int m_currentId;
    private TrailRenderer m_currentTrail;
    private float m_bipTimer;
    private bool m_bipReceived;
    
    private void Awake()
    {
        m_currentId = 0;
        m_trails = new List<TrailRenderer>();
        for (int i = 0; i < 2; ++i)
        {
            var trailObject = Instantiate(m_originalTrail.gameObject, transform);
            trailObject.transform.localPosition = m_originalTrail.transform.localPosition;
            var trail = trailObject.GetComponent<TrailRenderer>();
            trail.time = (m_boxLength - m_boxRefresh) / m_speed;
            m_trails.Add(trail);
        }
        
        m_originalTrail.gameObject.SetActive(false);
        m_currentTrail = m_trails[m_currentId];
        m_bipTimer = m_bipSize.x;
    }

    private void OnEnable()
    {
        InputManager.OnBipPress += Bip;
    }
    private void OnDisable()
    {
        InputManager.OnBipPress -= Bip;        
    }

    private void FixedUpdate()
    {
        if (m_bipReceived && m_bipTimer > m_minBipDuration)
        {
            m_bipReceived = false;
            m_bipTimer = 0f;
        }

        float delta = Time.fixedDeltaTime / m_frequency;
        for (int i = 0; i < m_frequency; ++i)
        {
            Vector3 current = m_currentTrail.transform.localPosition;
            float x = current.x;
            x += m_speed * delta;
        
            float y = 0f;
            if (m_bipTimer <= m_bipSize.x)
            {
                y = m_bip.Evaluate(m_bipTimer / m_bipSize.x) * m_bipSize.y;
                m_bipTimer += delta;
            }

            if (x > m_boxLength)
            {
                x -= m_boxLength;
                Clear(new Vector3(x, y));
            }
            else m_currentTrail.transform.localPosition = new Vector3(x, y);
        }
        
    }

    private void Bip()
    {
        m_bipReceived = true;

    }

    private void Clear(Vector3 _newPos)
    {
        m_currentId = (m_currentId + 1) % m_trails.Count;
        m_currentTrail = m_trails[m_currentId];
        m_currentTrail.transform.localPosition = _newPos;
        m_currentTrail.Clear();
    }
}
