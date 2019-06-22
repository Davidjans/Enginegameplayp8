using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyMovementState
{
    void OnEnter();
    void Update();
    void OnExit();
}

[System.Serializable]
public class WanderSettings
{
    public float m_WanderCircleDistance;
    public float m_WanderCircleRadius;
    public float m_WanderNoiseAngle;
}

[System.Serializable]
public class AvoidanceSettings
{
    public float m_AvoidanceDistance;
    public float m_MaxAvoidanceForce;
}

public class EnemyMovement : MonoBehaviour
{
    public List<IEnemyMovementState> m_MovementState = new List<IEnemyMovementState>();
    public float m_Mass = 70;
    public float m_MaxForce = 5 / 3.6f;
    public float m_MaxSpeed = 5 / 3.6f;

    [Tooltip("0 = seek, 1 = flee, 2 = wander, 3 = pursue, 4 = evade, 5 = path following")]
    public int m_StartState;

    public Transform m_TargetTransform;

    public float m_TimeAhead;

    [HideInInspector]
    public bool m_HasTreassure;
    [HideInInspector]
    public Vector3 m_Position;
    [HideInInspector]
    public Vector3 m_Velocity;
    [HideInInspector]
    public Vector3 m_VelocityDesired;
    [HideInInspector]
    public Vector3 m_Steering;
    [HideInInspector]
    public bool m_Arrival;


    public WanderSettings m_WanderSettings;

    public AvoidanceSettings m_AvoidanceSettings;

    [SerializeField] private float m_ArrivalDistance;

    public List<Transform> m_PathFollowingWaypoints;

    // Start is called before the first frame update
    void Start()
    {
        m_Position = transform.position;
        SelectStartState();
    }

    private void FixedUpdate()
    {
        m_Steering = Vector3.zero;
        m_Arrival = false;
        for (int i = 0; i < m_MovementState.Count; i++)
        {
            m_MovementState[i].Update();
            m_Steering += m_VelocityDesired;

        }
        SetRotation();

        Debug.DrawRay(transform.position, 2 * m_Velocity, Color.red);
        Debug.DrawRay(transform.position, 2 * m_VelocityDesired, Color.red);
        transform.position = m_Position;

    }

    public void SetRotation()
    {
        float maxSpeed = m_MaxSpeed;
        if (m_Arrival)
        {
            maxSpeed = Arrival(maxSpeed);
        }

        //m_Steering = m_VelocityDesired - m_Velocity;

        m_Steering = Vector3.ClampMagnitude(m_Steering, m_MaxForce);
        m_Steering = m_Steering / m_Mass;
        m_Velocity = Vector3.ClampMagnitude(m_Velocity + m_Steering, maxSpeed);
        m_Position += m_Velocity * Time.deltaTime;
    }

    public void AddState(IEnemyMovementState StateToAdd)
    {
        bool notIn = true;
        if (m_MovementState.Count != 0)
        {
            for (int i = 0; i < m_MovementState.Count; i++)
            {
                if (StateToAdd == m_MovementState[i])
                    notIn = false;
            }
        }
        if (notIn)
        {
            m_MovementState.Add(StateToAdd);
            if (StateToAdd != null)
                m_MovementState[m_MovementState.Count - 1].OnEnter();
        }
    }

    public void RemoveState(IEnemyMovementState StateToRemove)
    {
        for (int i = 0; i < m_MovementState.Count; i++)
        {
            if (StateToRemove == m_MovementState[i])
            {
                if (StateToRemove != null)
                    m_MovementState[m_MovementState.Count].OnEnter();
                m_MovementState.Add(StateToRemove);
            }
        }
    }

    private float Arrival(float maxSpeed)
    {
        float targetDistance = (m_TargetTransform.position - transform.position).magnitude - m_ArrivalDistance;
        float stopTime = m_Velocity.magnitude / m_MaxForce;
        return maxSpeed = Mathf.Min(targetDistance / stopTime, m_MaxSpeed);
    }

    public Transform GetClosestTarget(List<Transform> enemies)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (Transform t in enemies)
        {
            float dist = Vector3.Distance(t.position, currentPos);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }
        return tMin;
    }

    private void SelectStartState()
    {

        if (m_StartState == 0)
        {
            AddState(new Seek(this));
        }
        else if (m_StartState == 1)
        {
            AddState(new Flee(this));
        }
        else if (m_StartState == 2)
        {
            AddState(new Wander(this));
        }
        else if (m_StartState == 3)
        {
            AddState(new Pursue(this));
        }
        else if (m_StartState == 4)
        {
            AddState(new Evade(this));
        }
        else if (m_StartState == 5)
        {
            AddState(new PathFollowing(this));
        }
        AddState(new CollisionAvoidance(this));
        AddState(new WallAvoidance(this));

    }
}

public class Seek : IEnemyMovementState
{
    EnemyMovement m_EnemyMovement;
    public Seek(EnemyMovement refObject)
    {
        m_EnemyMovement = refObject;
    }

    public void OnEnter()
    {

    }

    public void Update()
    {
        m_EnemyMovement.m_VelocityDesired = (m_EnemyMovement.m_TargetTransform.position - m_EnemyMovement.m_Position).normalized * m_EnemyMovement.m_MaxSpeed;
        m_EnemyMovement.m_Arrival = true;
    }

    public void OnExit()
    {

    }
}

public class Flee : IEnemyMovementState
{
    EnemyMovement m_EnemyMovement;
    public Flee(EnemyMovement refObject)
    {
        m_EnemyMovement = refObject;
    }

    public void OnEnter()
    {

    }

    public void Update()
    {
        m_EnemyMovement.m_VelocityDesired = (m_EnemyMovement.m_Position - m_EnemyMovement.m_TargetTransform.position).normalized * m_EnemyMovement.m_MaxSpeed;
    }

    public void OnExit()
    {

    }
}

public class Wander : IEnemyMovementState
{
    EnemyMovement m_EnemyMovement;
    float m_WanderAngle;
    public Wander(EnemyMovement refObject)
    {
        m_EnemyMovement = refObject;
    }

    public void OnEnter()
    {

    }

    public void Update()
    {
        m_WanderAngle += Random.Range(-0.5f * m_EnemyMovement.m_WanderSettings.m_WanderNoiseAngle * Mathf.Deg2Rad,
            0.5f * m_EnemyMovement.m_WanderSettings.m_WanderNoiseAngle * Mathf.Deg2Rad);

        Vector3 centerOfCircle = m_EnemyMovement.m_Position + m_EnemyMovement.m_Velocity.normalized * m_EnemyMovement.m_WanderSettings.m_WanderCircleRadius;
        Vector3 offset = new Vector3(m_EnemyMovement.m_WanderSettings.m_WanderCircleRadius * (float)Mathf.Cos(m_WanderAngle),
            0.0f,
            m_EnemyMovement.m_WanderSettings.m_WanderCircleRadius * (float)Mathf.Sin(m_WanderAngle));


        m_EnemyMovement.m_VelocityDesired = centerOfCircle + offset;
    }

    public void OnExit()
    {

    }
}


public class Pursue : IEnemyMovementState
{
    EnemyMovement m_EnemyMovement;
    EnemyMovement m_Target;
    public Pursue(EnemyMovement refObject)
    {
        m_EnemyMovement = refObject;
    }

    public void OnEnter()
    {
        m_Target = m_EnemyMovement.m_TargetTransform.GetComponent<EnemyMovement>();
        if (m_Target == null)
        {
            m_EnemyMovement.AddState(new Seek(m_EnemyMovement));
            m_EnemyMovement.RemoveState(new Pursue(m_EnemyMovement));
        }
    }

    public void Update()
    {
        Vector3 predict = m_EnemyMovement.m_TargetTransform.position + m_EnemyMovement.m_TimeAhead * m_Target.m_Velocity;
        m_EnemyMovement.m_VelocityDesired = (predict - m_EnemyMovement.m_Position).normalized * m_EnemyMovement.m_MaxSpeed;
        m_EnemyMovement.m_Arrival = true;
    }

    public void OnExit()
    {

    }
}

public class Evade : IEnemyMovementState
{
    EnemyMovement m_EnemyMovement;
    EnemyMovement m_Target;

    public Evade(EnemyMovement refObject)
    {
        m_EnemyMovement = refObject;
    }

    public void OnEnter()
    {
        m_Target = m_EnemyMovement.m_TargetTransform.GetComponent<EnemyMovement>();
        if (m_Target == null)
        {
            m_EnemyMovement.AddState(new Seek(m_EnemyMovement));
            m_EnemyMovement.RemoveState(new Evade(m_EnemyMovement));
        }
    }

    public void Update()
    {
        Vector3 predict = m_EnemyMovement.m_TargetTransform.position + m_EnemyMovement.m_TimeAhead * m_Target.m_Velocity;
        m_EnemyMovement.m_VelocityDesired = (predict - m_EnemyMovement.m_Position).normalized * m_EnemyMovement.m_MaxSpeed;
        m_EnemyMovement.m_Arrival = true;
    }

    public void OnExit()
    {

    }
}

public class PathFollowing : IEnemyMovementState
{
    EnemyMovement m_EnemyMovement;
    private bool m_Reverse;
    private int m_CurrentWaypoint = 0;
    public PathFollowing(EnemyMovement refObject)
    {
        m_EnemyMovement = refObject;
    }

    public void OnEnter()
    {
        m_EnemyMovement.m_TargetTransform = m_EnemyMovement.GetClosestTarget(m_EnemyMovement.m_PathFollowingWaypoints);
        for (int i = 0; i < m_EnemyMovement.m_PathFollowingWaypoints.Count; i++)
        {
            if (m_EnemyMovement.m_TargetTransform == m_EnemyMovement.m_PathFollowingWaypoints[i])
            {
                m_CurrentWaypoint = i;
            }
        }
    }

    public void Update()
    {
        m_EnemyMovement.m_VelocityDesired = (m_EnemyMovement.m_TargetTransform.position - m_EnemyMovement.m_Position).normalized * m_EnemyMovement.m_MaxSpeed;

        if (Vector3.Distance(m_EnemyMovement.m_Position, m_EnemyMovement.m_TargetTransform.position) <= 0.5f)
        {
            if (m_CurrentWaypoint + 1 >= m_EnemyMovement.m_PathFollowingWaypoints.Count || (m_CurrentWaypoint - 1 < 0 && m_Reverse == true))
            {
                m_Reverse = !m_Reverse;
            }
            Debug.Log(m_CurrentWaypoint);
            if (m_Reverse)
            {
                m_CurrentWaypoint--;
            }
            else
            {
                m_CurrentWaypoint++;
            }
            Debug.Log(m_CurrentWaypoint);
            m_EnemyMovement.m_TargetTransform = m_EnemyMovement.m_PathFollowingWaypoints[m_CurrentWaypoint];
        }
    }

    public void OnExit()
    {

    }
}

public class CollisionAvoidance : IEnemyMovementState
{
    private EnemyMovement m_EnemyMovement;

    private const int m_ObstacleLayer = 1 << 8;

    public CollisionAvoidance(EnemyMovement refObject)
    {
        m_EnemyMovement = refObject;
    }

    public void OnEnter()
    {
    }

    public void Update()
    {
        RaycastHit m_Hit;

        Vector3 m_SensorPosition;
        if (Physics.Raycast(m_EnemyMovement.m_Position, m_EnemyMovement.m_Velocity, out m_Hit,
            m_EnemyMovement.m_AvoidanceSettings.m_AvoidanceDistance, m_ObstacleLayer, QueryTriggerInteraction.Ignore))
        {

            m_SensorPosition = m_EnemyMovement.m_Position + m_EnemyMovement.m_Velocity.normalized * m_EnemyMovement.m_AvoidanceSettings.m_AvoidanceDistance;

            Vector3 velocityDesired = (m_SensorPosition - m_Hit.collider.transform.position).normalized * m_EnemyMovement.m_AvoidanceSettings.m_MaxAvoidanceForce;
            Vector3 positionTarget = m_SensorPosition + velocityDesired;
            m_EnemyMovement.m_VelocityDesired = velocityDesired;
        }
    }

    public void OnExit()
    {

    }
}

public class WallAvoidance : IEnemyMovementState
{
    private EnemyMovement m_EnemyMovement;

    private const int m_ObstacleLayer = 1 << 8;

    public WallAvoidance(EnemyMovement refObject)
    {
        m_EnemyMovement = refObject;
    }

    public void OnEnter()
    {
    }

    public void Update()
    {
        RaycastHit m_Hit;

        Vector3 m_SensorPosition;
        if (Physics.Raycast(m_EnemyMovement.m_Position, m_EnemyMovement.m_Velocity, out m_Hit,
            m_EnemyMovement.m_AvoidanceSettings.m_AvoidanceDistance, m_ObstacleLayer, QueryTriggerInteraction.Ignore))
        {

            m_SensorPosition = m_EnemyMovement.m_Position + m_EnemyMovement.m_Velocity.normalized * m_EnemyMovement.m_AvoidanceSettings.m_AvoidanceDistance;

            Vector3 velocityDesired = (m_SensorPosition - m_Hit.collider.transform.position).normalized * m_EnemyMovement.m_AvoidanceSettings.m_MaxAvoidanceForce;
            velocityDesired += m_Hit.normal * 10000000000;
            Vector3 positionTarget = m_SensorPosition + velocityDesired;
            m_EnemyMovement.m_VelocityDesired = velocityDesired;
        }
    }

    public void OnExit()
    {

    }
}
