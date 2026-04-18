using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class ECG : MonoBehaviour
{
    
    #region Singleton
    private static ECG m_instance;
    public static ECG instance
    {
        get
        {
            if (!m_instance)
            {
                m_instance = FindAnyObjectByType<ECG>();
            }
            return m_instance;
        }
    }

    public delegate void SimpleEvent();
    public static event SimpleEvent OnBip;
    public static float bpm => instance.m_bpm;
    #endregion

    [SerializeField] private TextMeshPro m_bpmText;
    [SerializeField] private int m_frequency = 10;
    [SerializeField] private float m_speed;
    [SerializeField] private float m_boxLength = 100f;
    [SerializeField] private float m_boxRefresh = 0.2f;
    [Header("Helper")]
    [SerializeField] private Transform m_helper;
    [SerializeField] private int m_helperBPM;
    [Header("History")]
    [SerializeField] private float m_bpmRefreshCD;
    [SerializeField] private int m_maxBipHistory;
    [SerializeField] private int m_maxDurationHistory;
    [Header("Bip")]
    [SerializeField] private Vector2 m_bipSize;
    [SerializeField] private AnimationCurve m_bip;
    [SerializeField] private TrailRenderer m_originalTrail;
    [SerializeField] private float m_minBipDuration;
    
    private List<TrailRenderer> m_trails;
    private List<float> m_history;

    private int m_currentId;
    private TrailRenderer m_currentTrail;
    private float m_bipTimer;
    private float m_bpmRefreshTime;
    private bool m_bipReceived;
    
    private float m_computeBPM => m_history.Count <= 1 ? 0f : m_history.Count * 60f / ((m_history.Count < m_maxBipHistory)? Time.time - m_history[0] : (float)m_maxDurationHistory);
    private float m_bpm;
    
    private void Awake()
    {
        m_currentId = 0;
        m_history = new List<float>();
        
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
        InputManager.OnBipPress += BipReceived;
    }
    private void OnDisable()
    {
        InputManager.OnBipPress -= BipReceived;        
    }

    private void FixedUpdate()
    {
        if (m_bipReceived && m_bipTimer > m_minBipDuration)
        {
            Bip();
        }

        if (m_history.Count > 0 && Time.time - m_history[^1] > 3f) m_history.Clear();
        
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

        m_bpmRefreshTime += Time.fixedDeltaTime;
        if (m_bpmRefreshTime >= m_bpmRefreshCD)
        {
            m_bpmRefreshTime -= m_bpmRefreshCD;
            while(m_history.Count > 0 && Time.time - m_history[0] > m_maxDurationHistory) m_history.RemoveAt(0);
            // if(m_history.Count > 0) Debug.Log(m_history.Count + " / " + (Time.time - m_history[0]));
            
            m_bpm = m_computeBPM;
            m_bpmText.text = ((int)math.floor(bpm)).ToString();
        }
    }
    
    private void BipReceived()
    {
        m_bipReceived = true;
    }

    private void Bip()
    {
        OnBip?.Invoke();
        float dist = m_speed * 60f / m_helperBPM;
        m_helper.localPosition = new Vector3((m_currentTrail.transform.localPosition.x + dist) % m_boxLength, 0f);
        
        m_bipReceived = false;
        m_bipTimer = 0f;
        m_history.Add(Time.time);
        if(m_history.Count > m_maxBipHistory) m_history.RemoveAt(0);
    }
    
    private void Clear(Vector3 _newPos)
    {
        m_currentId = (m_currentId + 1) % m_trails.Count;
        m_currentTrail = m_trails[m_currentId];
        m_currentTrail.transform.localPosition = _newPos;
        m_currentTrail.Clear();
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.right * m_boxLength, Color.green);
    }
#endif
}
