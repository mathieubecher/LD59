using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Event Node")]
public class EventNode : ScriptableObject
{
    public string name;
    public Sprite icon;
    public AnimationCurve timeline;
    public float transition;
    public AnimationCurve safezone;

    public List<EventTransition> transitions;
}
[System.Serializable]
public class EventTransition
{
    public EventNode next;
    public float minScore;
}
public class EventManager : MonoBehaviour
{
    #region Singleton
    private static EventManager m_instance;
    public static EventManager instance
    {
        get
        {
            if (!m_instance)
            {
                m_instance = FindAnyObjectByType<EventManager>();
            }
            return m_instance;
        }
    }
    public static bool isRunning => instance.m_isRunning;
    #endregion
    
    [SerializeField] private BPMGauge m_gauge;
    [SerializeField] private EventNode m_start;
    [SerializeField] private TextMeshPro m_title;
    private EventNode m_current;
    private float m_timer;
    private float m_score;
    
    private float m_startGauge;
    private float m_currentGauge;
    private bool m_isRunning;
    private bool m_isStarted;
    private void Awake()
    {
        m_isRunning = true;
        m_isStarted = false;
    }
    private void FixedUpdate()
    {
        if (ECG.totalBit < 2) return;
        if(!m_isStarted)
        {
            m_isStarted = true;
            m_currentGauge = ECG.bpm;
            StartEvent(m_start);
        }

        UpdateEvent();
    }

    private void UpdateEvent()
    {
        m_timer += Time.deltaTime;
        
        float gaugeValue = m_current.timeline.Evaluate(m_timer - m_current.transition);
        if(m_current.transition > 0f && m_timer < m_current.transition) 
            gaugeValue = (gaugeValue - m_startGauge) * math.min(m_timer/m_current.transition, 1f) + m_startGauge;
        
        float safezone = m_current.safezone.Evaluate(m_timer - m_current.transition);
        m_currentGauge = gaugeValue;
        
        m_gauge.UpdateGauge(gaugeValue, safezone * 2f);

        float delta = math.min(math.abs(ECG.bpm - gaugeValue) - safezone, 0f);
    
        if (m_timer >= m_current.timeline.keys[^1].time + m_current.transition)
        {
            EndEvent(m_score);
        }
    }

    private void StartEvent(EventNode node)
    {
        m_startGauge = m_currentGauge;
        m_current = node;
        m_title.text = node.name;
        m_timer = 0f;
        m_score = 1f;
    }
    
    private void EndEvent(float _score)
    {
        if (!m_isRunning) return;
        EventNode next = ChooseNextEvent(_score);

        if (next != null)
            StartEvent(next);
        else
            ShowResult();
    }

    private EventNode ChooseNextEvent(float _score)
    {
        foreach (var transition in m_current.transitions)
        {
            if (_score >= transition.minScore)
                return transition.next;
        }
        return null;
    }

    private void ShowResult()
    {
        m_isRunning = false;
        Debug.Log("Fin");
    }
}
