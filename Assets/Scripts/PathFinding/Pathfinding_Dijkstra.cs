using System.Collections.Generic;
using System.Linq;
using Unity.Loading;
using UnityEngine;

[System.Serializable]
public class Pathfinding_Dijkstra : PathFinding
{
	[System.Serializable]
	class NodeInformation
	{
		public GridNode node { get; private set; }
		public NodeInformation parent { get; private set; }
		public float cost { get; private set; }

		public NodeInformation(GridNode node, NodeInformation parent, float cost)
		{
			this.node = node;
			this.parent = parent;
			this.cost = cost;
		}

		public void UpdateNodeInformation(NodeInformation parent, float cost)
		{
			this.parent = parent;
			this.cost = cost;
		}
	}

	public Pathfinding_Dijkstra(bool allowDiagonal, bool cutCorners) : base(allowDiagonal, cutCorners) { }

	public override void GeneratePath(GridNode start, GridNode end)
	{
		//clears the current path
		m_Path.Clear();
		
		//lists to track visited and none visited nodes
		List<NodeInformation> visited = new List<NodeInformation>();
		List<NodeInformation> notVisited = new List<NodeInformation>();

		NodeInformation startingNode = new NodeInformation(start, null, 0);
		notVisited.Add(startingNode);

		NodeInformation current = startingNode;

		int maxIternation = 0;

		//loop while there is a node selected
		while (current != null)
		{
			maxIternation++;
			if (maxIternation > m_MaxPathCount)
			{
				Debug.LogError("Max Iteration Reached");
				break;
			}

			//if the current node is the end node, a path has been found.
			if (current.node == end)
			{
				Debug.Log("Path found, start pos = " + start.transform.position + " - end pos = " + end.transform.position);
				SetPath(current);
				DrawPath(visited, notVisited);
				return;
			}

			//check all 8 neighbours of the current tile. 0 = up and then goes clockwise.
			for (int i = 0; i < 8; ++i)
			{
				GridNode neighbour = current.node.Neighbours[i];
				if (neighbour == null || !neighbour.m_Walkable) continue;

				// if already visited
				bool cont = false;
				foreach(NodeInformation nodeInfo in visited)
				{
					if(nodeInfo.node == neighbour) cont = true;
				}
				if (cont) continue;

				float cost = current.cost + neighbour.m_Cost + Maths.Magnitude(neighbour.transform.position - current.node.transform.position);
				foreach(NodeInformation nodeInfo in notVisited)
				{
                    if (nodeInfo.node == neighbour)
					{
						cont = true;
						if(nodeInfo.cost > cost)
						{
							nodeInfo.UpdateNodeInformation(current, cost);
						}
					}
                }

				if(cont) continue;
				NodeInformation neighbourInfo = new NodeInformation(neighbour, current, cost);
				notVisited.Add(neighbourInfo);
			}

            notVisited.Remove(current);
			visited.Add(current);
            if (notVisited.Count > 0)
			{
				int cheapestIndex = -1;
				float cheapestCost = float.MaxValue;
				for(int i = 0; i < notVisited.Count; i++)
				{
					if (notVisited[i].cost < cheapestCost)
					{
						cheapestCost = notVisited[i].cost;
						cheapestIndex = i;
					}
				}

				current = notVisited[cheapestIndex];
			}
			else
			{
				break;
			}
		}

		Debug.LogError("No path found, start pos = " + start.transform.position + " - end pos = " + end.transform.position);
	}

	/// <summary>
	/// pass in the final node information and sets m_Path
	/// </summary>
	private void SetPath(NodeInformation end)
	{
		NodeInformation curNode = end;
		while(curNode.parent != null)
		{
            m_Path.Add(curNode.node.transform.position);
			curNode = curNode.parent;
        }
		m_Path.Reverse();
	}

	/// <summary>
	/// Returns the cheapest node in the list calculated by cost
	/// </summary>
	private NodeInformation GetCheapestNode(List<NodeInformation> nodes)
	{
		return nodes.OrderBy(n => n.cost).First();
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
	/// Changest the colour of the grid based on the values passed in
	/// </summary>
	void DrawPath(List<NodeInformation> visited, List<NodeInformation> notVisited)
	{
		//drawPath
		if (m_Debug_ChangeTileColours)
		{
			Grid.ResetGridNodeColours();

			foreach (NodeInformation node in notVisited)
			{
				node.node.SetOpenInPathFinding();
			}

			foreach (NodeInformation node in visited)
			{
				node.node.SetClosedInPathFinding();
			}
		}
	}
}

