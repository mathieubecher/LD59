using Unity.Mathematics;
using UnityEngine;

public class BPMGauge : MonoBehaviour
{
    [SerializeField] private Transform m_bpm;
    [SerializeField] private SpriteRenderer m_safeZone;
    [SerializeField] private float m_size = 10f;
    [SerializeField] private int m_maxBPM = 240;

    private void Update()
    {
        if (!EventManager.isRunning) return;
        
        Vector3 pos = m_bpm.localPosition;
        m_bpm.localPosition = new Vector3(math.clamp(ECG.bpm / (float)m_maxBPM, 0f, 1f) * m_size, pos.y, pos.z);

    }

    public void UpdateGauge(float _value, float _safeZone)
    {
        Vector3 safePos = m_safeZone.transform.localPosition;
        m_safeZone.transform.localPosition = new Vector3(math.clamp(_value / m_maxBPM, 0f, 1f) * m_size, safePos.y, safePos.z);
        m_safeZone.size = new Vector2(_safeZone / m_maxBPM * m_size, m_safeZone.size.y);
    }
}
