using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task12_JpsPathfinding : MovingEntity
{
	SteeringBehaviour_Manager m_SteeringBehaviours;
	SteeringBehaviour_Seek m_Seek;

	[SerializeField]
	Pathfinding_JPS m_JPS;

	[Header("Debug")]
	[Tooltip("Draws the path of the agent")]
	[SerializeField]
	bool m_Debug_DrawPath;

	protected override void Awake()
	{
		base.Awake();

		m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();

		if (!m_SteeringBehaviours)
			Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

		m_Seek = GetComponent<SteeringBehaviour_Seek>();

		if (!m_Seek)
			Debug.LogError("Object doesn't have a Seek Steering Behaviour attached", this);

		m_JPS = new Pathfinding_JPS(true, false);
	}

	protected override Vector2 GenerateVelocity()
	{
		return m_SteeringBehaviours.GenerateSteeringForce();
	}

	protected void Update()
	{
		if (m_JPS.m_Path.Count == 0)
		{
			Rect size = Grid.m_GridSize;
			float x1 = Random.Range(size.xMin, size.xMax);
			float y1 = Random.Range(size.yMin, size.yMax);

			m_JPS.GeneratePath(Grid.GetNodeClosestWalkableToLocation(transform.position), Grid.GetNodeClosestWalkableToLocation(new Vector2(x1, y1)));
		}
		else
		{
			if (m_JPS.m_Path.Count > 0)
			{
				Vector2 closestPoint = m_JPS.GetClosestPointOnPath(transform.position);

				if (Maths.Magnitude(closestPoint - (Vector2)transform.position) < 0.5f)
					closestPoint = m_JPS.GetNextPointOnPath(transform.position);

				m_Seek.m_TargetPosition = closestPoint;
			}
		}
	}

	void OnDrawGizmosSelected()
	{
		if (Application.isPlaying)
		{
			if (m_Debug_DrawPath)
			{
				Gizmos.DrawLine(transform.position, m_Seek.m_TargetPosition);

				if (m_JPS.m_Path.Count > 1)
				{
					for (int i = 0; i < m_JPS.m_Path.Count - 1; ++i)
					{
						Gizmos.DrawLine(m_JPS.m_Path[i], m_JPS.m_Path[i + 1]);
					}
				}
			}
		}
	}
}
