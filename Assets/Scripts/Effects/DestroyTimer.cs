using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    [SerializeField] private float m_timer;
    

    private void Awake()
    {
        StartCoroutine(DestroyAfterTimer());
    }
    
    private IEnumerator DestroyAfterTimer()
    {
        yield return new WaitForSeconds(m_timer);
        Destroy(gameObject);
    }
}
