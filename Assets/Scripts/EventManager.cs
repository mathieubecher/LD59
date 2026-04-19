using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Event Node")]
public class EventNode : ScriptableObject
{
    public string name;
    public Sprite icon;
    public AnimationCurve timeline;
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
    [SerializeField] private BPMGauge m_gauge;
    [SerializeField] private EventNode m_start;
    private EventNode m_current;
    private float m_timer;
    private float m_score;
    private void Awake()
    {
        StartEvent(m_start);
    }
    private void FixedUpdate()
    {
        if (m_current == null || ECG.totalBit < 2) return;

        UpdateEvent();
    }

    private void UpdateEvent()
    {
        m_timer += Time.deltaTime;
        
        float gaugeValue = m_current.timeline.Evaluate(m_timer);
        float safezone = m_current.safezone.Evaluate(m_timer);
        m_gauge.UpdateGauge(gaugeValue, safezone * 2f);

        float delta = math.min(math.abs(ECG.bpm - gaugeValue) - safezone, 0f);

        if (m_timer >= m_current.timeline.keys[^1].time)
        {
            EndEvent(m_score);
        }
    }

    private void StartEvent(EventNode node)
    {
        m_current = node;
        m_timer = 0f;
        m_score = 1f;
    }
    
    private void EndEvent(float _score)
    {
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
        Debug.Log("Fin");
    }
}
