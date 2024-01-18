using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Pathfinding_AStar : PathFinding
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

    public Pathfinding_AStar(bool allowDiagonal, bool cutCorners) : base(allowDiagonal, cutCorners) { }

	float Heuristic_Caller(GridNode start, GridNode end, bool allowDiagonal)
	{
		if(allowDiagonal)
		{
			return Heuristic_Octile(start, end);
		}
        else
        {
            return Heuristic_Manhattan(start, end);
        }
    }

    public override void GeneratePath(GridNode start, GridNode end)
    {
		//clears the current path
		m_Path.Clear();

		//lists to track the open and closed nodes
        List<NodeInformation> openList = new List<NodeInformation>();
		List<NodeInformation> closedList = new List<NodeInformation>();

        NodeInformation startingNode = new NodeInformation(start, null, 0, Heuristic_Caller(start, end, m_AllowDiagonal));
		openList.Add(startingNode);

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
				DrawPath(openList, closedList);
				return;
			}

			for (int i = 0; i < 8; ++i)
            {
                // ignore diagonals if not allowing them
                if (!m_AllowDiagonal && i % 2 != 0) continue;

                GridNode neighbour = current.node.Neighbours[i];
                if (neighbour == null || !neighbour.m_Walkable || DoesListContainNode(closedList, neighbour)) continue;

                // disallows cutting corners
                if (!m_CanCutCorners && i % 2 != 0)
                {
                    int indexLeft = (i + 7) % 8;
                    int indexRight = (i + 1) % 8;

                    if (!current.node.Neighbours[indexLeft].m_Walkable) continue;
                    if (!current.node.Neighbours[indexRight].m_Walkable) continue;
                }

				//float gCost = current.gCost + neighbour.m_Cost + Maths.Magnitude(neighbour.transform.position - current.node.transform.position);
				// commented out as neighbour.m_Cost overbears the heuristic calculation causing A* to perform similarly to dijkstra
				// as gCost has greater bearing over hCost the further the pathfinding travels, due to the extra addition each tile travelled
				// so if start is 10 movements ago, gCost contains 10*neighbour.m_cost values
				float gCost = current.gCost + Maths.Magnitude(neighbour.transform.position - current.node.transform.position);
				float hCost = Heuristic_Caller(neighbour, end, m_AllowDiagonal);

                if (DoesListContainNode(openList, neighbour))
                {
                    NodeInformation neighbourInfo = GetNodeInformationFromList(openList, neighbour);
                    if (neighbourInfo.fCost > (gCost + hCost))
                    {
                        neighbourInfo.UpdateNodeInformation(current, gCost, hCost);
                    }
                }
                else
                {
                    NodeInformation neighbourInfo = new NodeInformation(neighbour, current, gCost, hCost);
                    openList.Add(neighbourInfo);
                }
            }

			openList.Remove(current);
			closedList.Add(current);
            if (openList.Count > 0)
            {
				current = GetCheapestNode(openList);
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
        while (curNode != null)
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

