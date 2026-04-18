using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private float m_moveSpeed = 5f;
    [SerializeField] private AnimationCurve m_bpmToMoveSpeedScale;
    [SerializeField] private TextMeshPro m_speedText;
    [SerializeField] private GameObject m_ring;

    private Rigidbody2D m_rigidbody;
    private float m_speedScale;
    void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_speedScale = 0f;
    }

    private void OnEnable()
    {
        ECG.OnBip += Bip;
    }
    private void OnDisable()
    {
        ECG.OnBip -= Bip;
    }

    private void FixedUpdate()
    {
        float bpmSpeedScale = m_bpmToMoveSpeedScale.Evaluate(ECG.bpm);

        m_speedScale += (bpmSpeedScale - m_speedScale) * 0.2f;
        m_speedText.text = bpmSpeedScale < 0.2f ? "0" : bpmSpeedScale < 0.8f ? ">" : bpmSpeedScale < 1.2f ? ">>" : bpmSpeedScale < 2f ? ">>>" : "!!!";
        m_rigidbody.velocity = new Vector2(InputManager.move.x * m_moveSpeed * bpmSpeedScale, m_rigidbody.velocity.y);
    }

    private void Bip()
    {
        Instantiate(m_ring, transform.position, Quaternion.identity);
    }
}
