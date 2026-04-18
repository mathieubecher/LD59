using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEffect : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> m_particles;
    [SerializeField] private float m_shakeIntensity;

    public float intensity => m_shakeIntensity;

    
    public void StartParticle(int _id)
    {
        m_particles[_id].Play();
    }

    public void StopParticle(int _id)
    {
        m_particles[_id].Stop();
    }

    private void Update()
    {
        Shake(m_shakeIntensity);
    }
    public void Shake(float _intensity)
    {
    }
}
