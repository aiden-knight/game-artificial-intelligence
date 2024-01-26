using System.Collections.Generic;
using System.Linq;
using Unity.Loading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using static UnityEngine.RuleTile.TilingRuleOutput;

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
		public int direction;

		public NodeInformation(GridNode node, NodeInformation parent, float gCost, float hCost, int direction)
		{
			this.node = node;
			this.parent = parent;
			this.gCost = gCost;
			this.hCost = hCost;
			this.direction = direction;
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

    float Heuristic_Caller(GridNode start, GridNode end)
    {
        if (m_AllowDiagonal)
        {
            return Heuristic_Octile(start, end);
        }
        else
        {
            return Heuristic_Chebyshev(start, end);
        }
    }

	void AddNode(NodeInformation current, GridNode jumpNode, GridNode end, List<NodeInformation> openList, int direction)
	{
        float gCost = current.gCost + Maths.Magnitude(jumpNode.transform.position - current.node.transform.position);
        float hCost = Heuristic_Caller(jumpNode, end);

        if (DoesListContainNode(openList, jumpNode))
        {
            NodeInformation nodeInfo = GetNodeInformationFromList(openList, jumpNode);
            if (nodeInfo.fCost > (gCost + hCost))
            {
                nodeInfo.UpdateNodeInformation(current, gCost, hCost);
            }
        }
        else
        {
            NodeInformation nodeInfo = new NodeInformation(jumpNode, current, gCost, hCost, direction);
            openList.Add(nodeInfo);
        }
    }

	bool CheckForcedNeighbour(GridNode node, int direction, bool clockwise, bool diagonal)
	{
		int dirMod;
		int dirModSide;
        if (diagonal)
        {
			dirMod = 3;
			dirModSide = 2;
        }
        else
        {
			dirMod = 2;
			dirModSide = 1;
        }

		if(clockwise)
		{
			dirMod = 8 - dirMod;
			dirModSide = 8 - dirModSide;
		}

		return (!GetNeighbourInDirection(node, (direction + dirMod) % 8).m_Walkable && GetNeighbourInDirection(node, (direction + dirModSide) % 8).m_Walkable);
    }

	void DiagonalJump(NodeInformation current, GridNode end, List<NodeInformation> openList, int direction)
	{
        GridNode jumpNode = GetNeighbourInDirection(current.node, direction);
        while (jumpNode != null)
        {
            if (!jumpNode.m_Walkable) break;
            if (jumpNode == end)
            {
                AddNode(current, jumpNode, end, openList, direction);
                break;
            }

            if (CheckForcedNeighbour(jumpNode, direction, true, true))
            {
                AddNode(current, jumpNode, end, openList, direction);
                break;
            }

            if (CheckForcedNeighbour(jumpNode, direction, false, true))
            {
                AddNode(current, jumpNode, end, openList, direction);
                break;
            }

            int orthogDir = (direction + 7) % 8;
			if (OrthogonalJump(current, end, openList, orthogDir, jumpNode, direction))
				break;

            orthogDir = (direction + 1) % 8;
			if (OrthogonalJump(current, end, openList, orthogDir, jumpNode, direction))
				break;

            jumpNode = GetNeighbourInDirection(jumpNode, direction);
        }
    }

    bool OrthogonalJump(NodeInformation current, GridNode end, List<NodeInformation> openList, int direction, GridNode diagonal = null, int diagonalDirection = -1)
	{
		GridNode jumpNode;
        if (diagonal != null)
        {
            jumpNode = GetNeighbourInDirection(diagonal, direction);
        }
		else
		{
			jumpNode = GetNeighbourInDirection(current.node, direction);
		}

		bool addNode = false;
        while (jumpNode != null)
        {
            if (!jumpNode.m_Walkable) break;
			if(jumpNode == end)
			{
				addNode = true;
				break;
			}

			if (CheckForcedNeighbour(jumpNode, direction, true, false))
            {
				addNode = true;
                break;
            }

            if (CheckForcedNeighbour(jumpNode, direction, false, false))
            {
                addNode = true;
                break;
            }
            jumpNode = GetNeighbourInDirection(jumpNode, direction);
        }

		if(addNode)
		{
			if(diagonal != null)
			{
                AddNode(current, diagonal, end, openList, diagonalDirection);
            }
			else
			{
                AddNode(current, jumpNode, end, openList, direction);
            }
		}
		return addNode;
    }

    public override void GeneratePath(GridNode start, GridNode end)
	{
		//clears the current path
		m_Path.Clear();
		
		//lists to track visited and none visited nodes
		List<NodeInformation> openList = new List<NodeInformation>();
		List<NodeInformation> closedList = new List<NodeInformation>();

		NodeInformation startingNode = new NodeInformation(start, null, 0, 0, -1);
        NodeInformation current = startingNode;

        for (int i = 0; i < 8; i += 2)
        {
            OrthogonalJump(current, end, openList, i);
        }

        for (int i = 1; i < 8; i += 2)
        {
            DiagonalJump(current, end, openList, i);
        }

        int maxIteration = 0;
        openList.Remove(current);
        closedList.Add(current);
        if (openList.Count > 0)
        {
            current = GetCheapestNode(openList);
        }
		else
		{
			current = null;
		}

        while (current != null)
		{
			if (maxIteration > m_MaxPathCount) break;

			if(current.node == end)
			{
				SetPath(current);
				DrawPath(openList, closedList);
				return;
			}

			int direction = current.direction;
			if(direction % 2 == 0)
			{
                OrthogonalJump(current, end, openList, direction);

                int above = (direction + 6) % 8;
                int left = (direction + 7) % 8;
                if (!GetNeighbourInDirection(current.node, above).m_Walkable && GetNeighbourInDirection(current.node, left).m_Walkable)
                {
                    DiagonalJump(current, end, openList, left);
                }

                int below = (direction + 2) % 8;
                int right = (direction + 1) % 8;
                if (!GetNeighbourInDirection(current.node, below).m_Walkable && GetNeighbourInDirection(current.node, right).m_Walkable)
                {
                    DiagonalJump(current, end, openList, right);
                }
            }
			else
			{
                DiagonalJump(current, end, openList, direction);
                int left = (direction + 7) % 8;
                int right = (direction + 1) % 8;
                OrthogonalJump(current, end, openList, left);
                OrthogonalJump(current, end, openList, right);
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

