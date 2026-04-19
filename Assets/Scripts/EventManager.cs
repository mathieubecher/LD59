using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Event Node")]
public class EventNode : ScriptableObject
{
    public string name;
    public Sprite icon;
    public AnimationCurve timeline;

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
    private EventNode m_current;
    private float m_time;
    public void StartEvent(EventNode node)
    {
        m_current = node;
        m_time = 0f;
    }
    
    private 
    
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
        
    }
}
