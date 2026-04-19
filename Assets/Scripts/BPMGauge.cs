using Unity.Mathematics;
using UnityEngine;

public class BPMGauge : MonoBehaviour
{
    [SerializeField] private Transform m_bpm;
    [SerializeField] private Transform m_safeZone;
    [SerializeField] private AnimationCurve m_timeline;
    [SerializeField] private float m_size = 10f;
    [SerializeField] private int m_maxBPM = 240;

    private float m_timer;

    private void Update()
    {
        Vector3 pos = m_bpm.localPosition;
        m_bpm.localPosition = new Vector3(math.clamp(ECG.bpm / (float)m_maxBPM, 0f, 1f) * m_size, pos.y, pos.z);

        if(ECG.totalBit > 2) m_timer += Time.deltaTime;
        Vector3 safePos = m_safeZone.localPosition;
        m_safeZone.localPosition = new Vector3(math.clamp(m_timeline.Evaluate(m_timer) / (float)m_maxBPM, 0f, 1f) * m_size, safePos.y, safePos.z);
    }
}
