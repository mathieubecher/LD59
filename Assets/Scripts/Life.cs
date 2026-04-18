using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Life : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_background;
    [SerializeField] private SpriteRenderer m_progress;
    [SerializeField] private AnimationCurve m_bpmToLifeProgress;

    private float m_life = 1f;
    
    void Update()
    {
        m_life = math.clamp(m_life + m_bpmToLifeProgress.Evaluate(ECG.bpm) * Time.deltaTime, 0f, 1f);
        m_progress.size = new Vector3(m_life, 1f, 1f) * m_background.size;
        m_progress.transform.localPosition = new Vector3(- (1f - m_life)/2 * m_background.size.x, 0f, 0f);
    }
}
