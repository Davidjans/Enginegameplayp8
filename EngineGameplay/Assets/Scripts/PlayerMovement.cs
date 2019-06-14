using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour {

    [SerializeField] private float m_Acceleration;
    [SerializeField] private float m_Decelleration;
    [SerializeField] private float m_MaxSpeed;
    [SerializeField] private float m_TurnSpeed;
	[SerializeField] private KeyCode m_TopKey;
	[SerializeField] private KeyCode m_Leftkey;
	[SerializeField] private KeyCode m_RightKey;
	[SerializeField] private KeyCode m_BottomKey;

    private float m_Speed;
    private byte m_Direction = 1; // 0 = left 1 = right 2= top  3= bottom 4 = idle
    private Quaternion StartRotation;
	private Vector3 m_NewPosition;
	private Transform m_PlayerTransform;
	private Rigidbody m_PlayerRigidBody;


	// Use this for initialization
	void Start () {
		m_PlayerTransform = GetComponent<Transform>();
		m_PlayerRigidBody = GetComponent<Rigidbody>();
		m_NewPosition = m_PlayerTransform.position;
	}
	
	// Update is called once per frame
	void Update () {
		m_PlayerRigidBody.velocity = Vector3.zero;
        m_PlayerRigidBody.angularVelocity = Vector3.zero;

        SetSpeed();

        CheckDirection();

        SetNewPosition();

		if(Input.GetKey(m_TopKey) || Input.GetKey(m_BottomKey) || Input.GetKey(m_RightKey) || Input.GetKey(m_Leftkey))
		{
            Vector3 direction = m_NewPosition - transform.position ;
			Vector3 newDir = Vector3.RotateTowards(m_PlayerTransform.forward, direction, m_TurnSpeed * Time.deltaTime, 0.0f);
			m_PlayerTransform.rotation = Quaternion.LookRotation(newDir);
		}


		m_NewPosition.y = m_PlayerTransform.position.y;
        Debug.DrawLine(transform.position, m_NewPosition, Color.red);
        ResetSpeed();
		m_PlayerRigidBody.MovePosition(m_NewPosition);
	}

    private void Accellerate()
    {
        if(m_Speed <= m_MaxSpeed)
        {
            m_Speed = Mathf.Clamp(m_Speed + m_Acceleration,0,m_MaxSpeed);
        }
    }

    private void Decellerate()
    {
        if (m_Speed >= 0)
        {
            m_Speed = Mathf.Clamp(m_Speed - m_Decelleration, 0, m_MaxSpeed);
        }
    }

    private void ResetSpeed()
    {
        if(m_NewPosition == m_PlayerTransform.position)
        {
            m_Speed = 0;
        }
    }

    private void SetNewPosition()
    {
        if(m_Speed >= 0)
        {
            if(m_Direction == 2)
            {
                m_NewPosition.z = m_PlayerTransform.position.z + m_Speed * Time.deltaTime;
            }
            else if(m_Direction == 3)
            {
                m_NewPosition.z = m_PlayerTransform.position.z - m_Speed * Time.deltaTime;
            }
            else if(m_Direction == 1)
            {
                m_NewPosition.x = m_PlayerTransform.position.x + m_Speed * Time.deltaTime;
            }
            else if(m_Direction == 0)
            {
                m_NewPosition.x = m_PlayerTransform.position.x - m_Speed * Time.deltaTime;
            }
        }
        else
        {
            m_Direction = 4;
        }
    }

    private void SetSpeed()
    {
        if (Input.GetKey(m_Leftkey) || Input.GetKey(m_RightKey) || Input.GetKey(m_BottomKey) || Input.GetKey(m_TopKey))
        {
            Accellerate();
        }
        else
        {
            Decellerate();
        }
    }

    private void CheckDirection()
    {
        if (Input.GetKey(m_TopKey))
        {
            m_Direction = 2;
        }
        else if (Input.GetKey(m_BottomKey))
        {
            m_Direction = 3;
        }
        if (Input.GetKey(m_RightKey))
        {
            m_Direction = 1;
        }
        else if (Input.GetKey(m_Leftkey))
        {

            m_Direction = 0;
        }
    }
}
