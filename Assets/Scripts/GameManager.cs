using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager m_instance;
    public static GameManager instance
    {
        get
        {
            if (!m_instance)
            {
                m_instance = FindAnyObjectByType<GameManager>();
            }
            return m_instance;
        }
    }

    public static  Character character => instance.m_character;
    public static Life life => instance.m_life;
    public static  ECG ech => instance.m_ecg;
    public static bool dead => instance.m_dead;
    #endregion

    [SerializeField] private Character m_character;
    [SerializeField] private Life m_life;
    [SerializeField] private ECG m_ecg;

    private bool m_dead;
    
    private void OnEnable()
    {
        character.OnDead += Dead;
        character.OnSpotted += Dead;
    }

    private void OnDisable()
    {
        character.OnDead -= Dead;
        character.OnSpotted -= Dead;
    }

    private void Dead()
    {
        Debug.Log("Dead");
        m_dead = true;
    }

}
