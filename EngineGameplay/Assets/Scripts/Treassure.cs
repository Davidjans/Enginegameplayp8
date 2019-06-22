using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treassure : MonoBehaviour
{
    [SerializeField] private Transform m_DropOff;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.CompareTag("thief"))
        {
            Debug.Log("hi");
            EnemyMovement thief = other.GetComponent<EnemyMovement>();
            thief.m_TargetTransform = m_DropOff;
            thief.m_HasTreassure = true;
           
        }
    }
}
