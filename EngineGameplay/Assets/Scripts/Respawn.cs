using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    [SerializeField] private float m_OriginalTimer;
    [SerializeField] private Transform m_Treassure;
    private float m_RespawnTimer;
    private List<EnemyMovement> m_DeadBoys = new List<EnemyMovement>();
    // Start is called before the first frame update
    void Start()
    {
        m_RespawnTimer = m_OriginalTimer;
    }

    // Update is called once per frame
    void Update()
    {
        m_RespawnTimer -= Time.deltaTime;

        if(m_RespawnTimer <= 0)
        {
            for (int i = 0; i < m_DeadBoys.Count; i++)
            {
                m_DeadBoys[i].m_TargetTransform = m_Treassure;
            }
            m_RespawnTimer = m_OriginalTimer;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("thief"))
        {
            m_DeadBoys.Add(other.GetComponent<EnemyMovement>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("thief"))
        {
            m_DeadBoys.Remove(other.GetComponent<EnemyMovement>());
        }
    }
}
