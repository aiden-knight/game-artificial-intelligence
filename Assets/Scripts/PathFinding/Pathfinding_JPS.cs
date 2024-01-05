using System.Collections.Generic;
using System.Linq;
using Unity.Loading;
using UnityEngine;

[System.Serializable]
public class Pathfinding_JSP : PathFinding
{
	[System.Serializable]
	class NodeInformation
	{
		public GridNode node;
		public NodeInformation parent;
		public float gCost;
		public float hCost;
		public float fCost;

		public NodeInformation(GridNode node, NodeInformation parent, float gCost, float hCost)
		{
			this.node = node;
			this.parent = parent;
			this.gCost = gCost;
			this.hCost = hCost;
			fCost = gCost + hCost;
		}

		public void UpdateNodeInformation(NodeInformation parent, float gCost, float hCost)
		{
			this.parent = parent;
			this.gCost = gCost;
			this.hCost = hCost;
			fCost = gCost + hCost;
		}
	}

	public Pathfinding_JSP(bool allowDiagonal, bool cutCorners) : base(allowDiagonal, cutCorners) { }

	public override void GeneratePath(GridNode start, GridNode end)
	{
		//clears the current path
		m_Path.Clear();
		
		//lists to track visited and none visited nodes
		List<NodeInformation> openList = new List<NodeInformation>();
		List<NodeInformation> closedList = new List<NodeInformation>();


		Debug.LogError("No path found, start pos = " + start.transform.position + " - end pos = " + end.transform.position);
	}

	/// <summary>
	/// pass in the final node information and sets m_Path
	/// </summary>
	private void SetPath(NodeInformation end)
	{
		NodeInformation current = end;
		while (current != null)
		{
			m_Path.Add(current.node.transform.position);
			current = current.parent;
		}

		m_Path.Reverse();
	}

	/// <summary>
	/// Returns the cheapest node in the list calculated by cost
	/// </summary>
	private NodeInformation GetCheapestNode(List<NodeInformation> nodes)
	{
		return nodes.OrderBy(n => n.fCost).First();
	}

	/// <summary>
	/// checks if a grid node reference is held within a list of Node Informations
	/// </summary>
	bool DoesListContainNode(List<NodeInformation> nodeInformation, GridNode gridNode)
	{
		return nodeInformation.Any(x => x.node == gridNode);
	}

	/// <summary>
	/// Returns a Node Information if a grid node reference is within the list
	/// </summary>
	NodeInformation GetNodeInformationFromList(List<NodeInformation> nodeInformation, GridNode gridNode)
	{
		return nodeInformation.Find(x => x.node == gridNode);
	}

	/// <summary>
	/// returns the next node in a set direction
	/// </summary>
	GridNode GetNeighbourInDirection(GridNode current, int direction)
	{
		return current.Neighbours[direction];
	}

	/// <summary>
	/// returns a node either clockwise or anticlockwise in a set direction
	/// </summary>
	GridNode GetDiagonalNeighbours(GridNode current, int direction, bool clockwise)
	{
		int modifier = direction;

		if (clockwise) { modifier++; }
		else { modifier--; }

		int neighbour = (modifier % 8 + 8) % 8;

		return current.Neighbours[neighbour];
	}

	/// <summary>
	/// Changest the colour of the grid based on the values passed in
	/// </summary>
	void DrawPath(List<NodeInformation> open, List<NodeInformation> closed)
	{
		//drawPath
		if (m_Debug_ChangeTileColours)
		{
			Grid.ResetGridNodeColours();

			foreach (NodeInformation node in closed)
			{
				node.node.SetOpenInPathFinding();
			}

			foreach (NodeInformation node in open)
			{
				node.node.SetClosedInPathFinding();
			}
		}
	}
}

