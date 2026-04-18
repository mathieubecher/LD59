using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Life : MonoBehaviour
{
    public delegate void SimpleEvent();
    public event SimpleEvent OnDead;
    
    [SerializeField] private SpriteRenderer m_background;
    [SerializeField] private SpriteRenderer m_progress;
    [SerializeField] private AnimationCurve m_bpmToLifeProgress;

    private float m_life = 1f;
    
    void Update()
    {
        if (GameManager.dead) return;
        if (ECG.totalBit <= 2) return;
        m_life = math.clamp(m_life + m_bpmToLifeProgress.Evaluate(ECG.bpm) * Time.deltaTime, 0f, 1f);
        UpdateBar();

        if (m_life == 0f)
        {
            Dead();
        }
    }

    public void Dead()
    {
        m_life = 0;
        UpdateBar();
        OnDead?.Invoke();
    }

    private void UpdateBar()
    {
        m_progress.size = new Vector3(m_life, 1f, 1f) * m_background.size;
        m_progress.transform.localPosition = new Vector3(- (1f - m_life)/2 * m_background.size.x, 0f, 0f);
    }
}
