using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MovingEntity
{
	SteeringBehaviour_Manager m_SteeringBehaviours;

	SteeringBehaviour_Seek m_Seek;
	SteeringBehaviour_CollisionAvoidance m_Avoidance;

	Pathfinding_AStar m_AStarPathFinding;

	bool m_PlayerInSight;

	protected override void Awake()
	{
		base.Awake();

		m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();

		if (!m_SteeringBehaviours)
			Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

		m_Seek = GetComponent<SteeringBehaviour_Seek>();

		if (!m_Seek)
			Debug.LogError("Object doesn't have a Seek Steering Behaviour attached", this);

		m_Avoidance = GetComponent<SteeringBehaviour_CollisionAvoidance>();

		if (!m_Avoidance)
			Debug.LogError("Object doesn't have a Collision Avoidance Steering Behaviour attached", this);

		m_AStarPathFinding = new Pathfinding_AStar(true, false);
	}

	protected void Start()
	{
		m_Seek.m_TargetPosition = Grid.GetRandomWalkableTile().transform.position;
	}

	protected override Vector2 GenerateVelocity()
	{
		return m_SteeringBehaviours.GenerateSteeringForce();
	}

	protected void Update()
	{
		if (!m_PlayerInSight)
		{
			if (m_AStarPathFinding.m_Path.Count == 0)
			{
				Rect size = Grid.m_GridSize;
				float x1 = Random.Range(size.xMin, size.xMax);
				float y1 = Random.Range(size.yMin, size.yMax);

				m_AStarPathFinding.GeneratePath(Grid.GetNodeClosestWalkableToLocation(transform.position), Grid.GetNodeClosestWalkableToLocation(new Vector2(x1, y1)));
			}
			else
			{
				if (m_AStarPathFinding.m_Path.Count > 0)
				{
					Vector2 closestPoint = m_AStarPathFinding.GetClosestPointOnPath(transform.position);

					if (Maths.Magnitude(closestPoint - (Vector2)transform.position) < 0.5f)
						closestPoint = m_AStarPathFinding.GetNextPointOnPath(transform.position);

					m_Seek.m_TargetPosition = closestPoint;
				}
			}

			MoveAndRotate();
		}
	}

	void OnDrawGizmosSelected()
	{
		if (Application.isPlaying)
		{
			Gizmos.DrawLine(transform.position, m_Seek.m_TargetPosition);

			if (m_AStarPathFinding.m_Path.Count > 1)
			{
				for (int i = 0; i < m_AStarPathFinding.m_Path.Count - 1; ++i)
				{
					Gizmos.DrawLine(m_AStarPathFinding.m_Path[i], m_AStarPathFinding.m_Path[i + 1]);
				}
			}
		}
	}
}
