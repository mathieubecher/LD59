using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Nurse : MonoBehaviour
{

    public delegate void SimpleEvent(int _id);

    public event SimpleEvent OnPointReached;

    [SerializeField] private SpriteRenderer m_sprite;
    [SerializeField] private DetectCharacter m_detect;
    [SerializeField] private float m_defaultSpeed;
    [SerializeField] private float m_pursueSpeed;
    [SerializeField] private List<Transform> m_path;

    private Rigidbody2D m_rigidbody;
    private int m_currentIndex = 0;
    private int m_direction = 1;
    private List<Vector3> m_points;
    
    private Vector3 m_signalPos;
    private bool m_detectSignal;
    
    private bool m_stop;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_points = new List<Vector3>();
        foreach (var point in m_path)
        {
            m_points.Add(point.position);
        }
    }

    private void OnEnable()
    {
        m_detect.OnCharacterSpotted += Spotted;
    }

    private void OnDisable()
    {
        m_detect.OnCharacterSpotted -= Spotted;
    }

    void FixedUpdate()
    {
        if (GameManager.dead) return;
        if (m_stop) return;
        FollowPoint();
    }

    private void FollowPoint()
    {
        Vector3 target = m_points[m_currentIndex];
        float speed = m_defaultSpeed;
        if (m_detectSignal)
        {
            target = m_signalPos;
        }

        float distance = target.x - transform.position.x;
        float velocity = math.sign(distance) * speed;

        m_rigidbody.velocity = new Vector2(velocity, m_rigidbody.velocity.y);

        if (math.abs(distance) < 0.05f)
        {
            ReachPoint();
        }
    }

    private void ReachPoint()
    {
        Vector3 scale = m_sprite.transform.localScale;
        float direction = m_direction;
        
        if (m_detectSignal)
        {
            ReachSignal();
            direction = (m_points[m_currentIndex] - transform.position).x;
        }
        else
        {
            m_rigidbody.velocity = new Vector2(0f, m_rigidbody.velocity.y);

            OnPointReached?.Invoke(m_currentIndex);

            m_currentIndex += m_direction;

            if (m_currentIndex >= m_points.Count)
            {
                m_direction = -1;
                m_currentIndex = m_points.Count - 2;
            }
            else if (m_currentIndex < 0)
            {
                m_direction = 1;
                m_currentIndex = 1;
            }
            direction = m_direction;
            
        }
        m_sprite.transform.localScale = new Vector3(math.abs(scale.x) * math.sign(direction), scale.y, scale.z);
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (!_other.isTrigger) return;
        DetectSignal(_other.transform.position);
    }

    private void DetectSignal(Vector3 _position)
    {
        if(!m_detectSignal) m_stop = true;
        m_detectSignal = true;
        m_signalPos = _position;
        float direction = (m_signalPos - transform.position).x;
        Vector3 scale = m_sprite.transform.localScale;
        m_sprite.transform.localScale = new Vector3(math.abs(scale.x) * math.sign(direction), scale.y, scale.z);
        StartCoroutine(Wait());
    }

    private void ReachSignal()
    {
        m_detectSignal = false;
        m_stop = true;
        StartCoroutine(Wait());
    }

    private void Spotted()
    {
        m_stop = true;
        m_rigidbody.velocity = Vector2.zero;
        float direction = (m_detect.character.transform.position - transform.position).x;
        Vector3 scale = m_sprite.transform.localScale;
        m_sprite.transform.localScale = new Vector3(math.abs(scale.x) * math.sign(direction), scale.y, scale.z);
    }

    private IEnumerator Wait()
    {
        m_rigidbody.velocity = Vector2.zero;
        yield return new WaitForSeconds(1f);
        m_stop = false;
    }
}
