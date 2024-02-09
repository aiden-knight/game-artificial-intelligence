using System.Collections.Generic;
using System.Linq;
using Unity.Loading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using System.Threading;
using static UnityEngine.RuleTile.TilingRuleOutput;

[System.Serializable]
public class Pathfinding_JPS : PathFinding
{
	JPS_Routines routineCaller;

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

		public void UpdateNodeInformation(NodeInformation parent, float gCost, float hCost, int direction)
		{
			this.parent = parent;
			this.gCost = gCost;
			this.hCost = hCost;
			fCost = gCost + hCost;
			this.direction = direction;
		}
	}

	public Pathfinding_JPS(bool allowDiagonal, bool cutCorners) : base(allowDiagonal, cutCorners) { }

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
		// calculates cost to get to node (currently treating all grid nodes as same cost to travel through)
        float gCost = current.gCost + Maths.Magnitude(jumpNode.transform.position - current.node.transform.position);
        float hCost = Heuristic_Caller(jumpNode, end);

        if (DoesListContainNode(openList, jumpNode))
        {
            NodeInformation nodeInfo = GetNodeInformationFromList(openList, jumpNode);
            if (nodeInfo.fCost > (gCost + hCost))
            {
                nodeInfo.UpdateNodeInformation(current, gCost, hCost, direction);
            }
        }
        else
        {
            NodeInformation nodeInfo = new NodeInformation(jumpNode, current, gCost, hCost, direction);
            openList.Add(nodeInfo);
        }
    }

	bool CheckForcedNeighbour(GridNode node, int direction, bool antiClockwise, bool diagonal)
	{
		int dirMod;
		int dirModSide;
        if (diagonal) // if diagonal it's left or below (+/- 3 then the one to the side of that which we could reach)
        {
			dirMod = 3;
			dirModSide = 2;
        }
        else // if not diagonal it's above or below with diagonal up left / down right
        {
			dirMod = 2;
			dirModSide = 1;
        }

		if(antiClockwise) // anti-clockwise check subtracts from direction which in mod 8 is adding 8 minus value
		{
			dirMod = 8 - dirMod;
			dirModSide = 8 - dirModSide;
		}

		// if non walkable neighbour with walkable neighbour to its side then is a forced neighbour
		return (!GetNeighbourInDirection(node, (direction + dirMod) % 8).m_Walkable && GetNeighbourInDirection(node, (direction + dirModSide) % 8).m_Walkable);
    }

	void DiagonalJump(NodeInformation current, GridNode end, List<NodeInformation> openList, int direction)
	{
        GridNode jumpNode = GetNeighbourInDirection(current.node, direction);
        while (jumpNode != null)
        {
            if (!jumpNode.m_Walkable) break; // if hit a wall just count direction as pointless
            if (jumpNode == end)
            {
                AddNode(current, jumpNode, end, openList, direction);
                break;
            }

            if (m_Debug_ChangeTileColours)
            {
                jumpNode.SetJumpedInPathFinding();
            }

            // check for diagonal forced neighbours, if so add current jump node to open list
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

			// also jump out in the orthogonal directions
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
        if (diagonal != null) // if jumping from diagonal don't use current as start point
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
            if (jumpNode == end)
			{
				addNode = true;
				break;
			}
            if (m_Debug_ChangeTileColours)
            {
                jumpNode.SetJumpedInPathFinding();
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
			// if was jumping diagonally add the diagonal jump node as the node of interest to the open list
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
		
		GeneratePathThread(start, end);
    }

    public void GeneratePathThread(GridNode start, GridNode end)
	{
		//clears the current path
		m_Path.Clear();
		
		//lists to track visited and none visited nodes
		List<NodeInformation> openList = new List<NodeInformation>();
		List<NodeInformation> closedList = new List<NodeInformation>();

		// setup starting node
		NodeInformation startingNode = new NodeInformation(start, null, 0, 0, -1);
        NodeInformation current = startingNode;

		// jump through all 8 directions from start
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

			// if found end node we are done
			if(current.node == end)
			{
				SetPath(current);
				DrawPath(openList, closedList);
				return;
			}

			// start by jumping in the direction we were already going when we found the node of interest
			int direction = current.direction;
			if(direction % 2 == 0)
			{
                OrthogonalJump(current, end, openList, direction);

				// do diagonal jump in direction of forced neighbours
				if(CheckForcedNeighbour(current.node, direction, true, false))
				{
					int diagDir = (direction + 7) % 8;
                    DiagonalJump(current, end, openList, diagDir);
                }
                if (CheckForcedNeighbour(current.node, direction, false, false))
                {
                    int diagDir = (direction + 1) % 8;
                    DiagonalJump(current, end, openList, diagDir);
                }
            }
			else
			{
				// jumps to left and right side of diagonal direction
                int left = (direction + 7) % 8;
                int right = (direction + 1) % 8;
                OrthogonalJump(current, end, openList, left);
                OrthogonalJump(current, end, openList, right);

				// jump diagonally in intended direction
                DiagonalJump(current, end, openList, direction);

                // do diagonal jump in direction of forced neighbours
                if (CheckForcedNeighbour(current.node, direction, true, true))
                {
                    int diagDir = (direction + 6) % 8;
                    DiagonalJump(current, end, openList, diagDir);
                }
                if (CheckForcedNeighbour(current.node, direction, false, true))
                {
                    int diagDir = (direction + 2) % 8;
                    DiagonalJump(current, end, openList, diagDir);
                }
            }

            openList.Remove(current);
            closedList.Add(current);
            if (openList.Count > 0)
            {
				// get next cheapest node
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

