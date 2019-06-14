using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
{
    [SerializeField] private float m_Acceleration;
    [SerializeField] private float m_Decelleration;
    [SerializeField] private float m_MaxSpeed;
    [SerializeField] private float m_TurnSpeed;
    [SerializeField] private KeyCode m_TopKey;
    [SerializeField] private KeyCode m_Leftkey;
    [SerializeField] private KeyCode m_RightKey;
    [SerializeField] private KeyCode m_BottomKey;

    private float m_Speed;
    Vector3 m_Direction;
    private Quaternion StartRotation;
    private Vector3 m_NewPosition;
    private Transform m_PlayerTransform;
    private Rigidbody m_PlayerRigidBody;


    // Use this for initialization
    void Start()
    {
        m_PlayerTransform = GetComponent<Transform>();
        m_PlayerRigidBody = GetComponent<Rigidbody>();
        m_NewPosition = m_PlayerTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        m_PlayerRigidBody.velocity = Vector3.zero;
        m_PlayerRigidBody.angularVelocity = Vector3.zero;

        SetSpeed();
        m_Direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        SetNewPosition();

        if (Input.GetKey(m_TopKey) || Input.GetKey(m_BottomKey) || Input.GetKey(m_RightKey) || Input.GetKey(m_Leftkey))
        {
            Vector3 direction = m_NewPosition - transform.position;
            Vector3 newDir = Vector3.RotateTowards(m_PlayerTransform.forward, direction, m_TurnSpeed * Time.deltaTime, 0.0f);
            m_PlayerTransform.rotation = Quaternion.LookRotation(newDir);
        }


        m_NewPosition.y = m_PlayerTransform.position.y;
        Debug.DrawLine(transform.position, m_NewPosition, Color.red);
        m_PlayerRigidBody.MovePosition(m_NewPosition);
    }

    private void Accellerate()
    {
        if (m_Speed <= m_MaxSpeed)
        {
            m_Speed = Mathf.Clamp(m_Speed + m_Acceleration, 0, m_MaxSpeed);
        }
    }

    private void Decellerate()
    {
        if (m_Speed >= 0)
        {
            m_Speed = Mathf.Clamp(m_Speed - m_Decelleration, 0, m_MaxSpeed);
        }
    }

    private void SetNewPosition()
    {
        if (m_Speed >= 0)
        {
        }
        else
        {
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
}
