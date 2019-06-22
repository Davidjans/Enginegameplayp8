using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOff : MonoBehaviour
{
    [SerializeField] private Transform m_Treassure;
    [SerializeField] private MatchManager m_MatchManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("thief"))
        {
            EnemyMovement thief = other.GetComponent<EnemyMovement>();
            if (thief.m_HasTreassure)
            {
                thief.m_TargetTransform = m_Treassure;
                thief.m_HasTreassure = false;
                m_MatchManager.m_score++;
            }
        }
    }
}
