using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;

//using System.Numerics;
using UnityEngine;

public class EnemyPathfinding
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    public static EnemyPathfinding Instance { get; private set; }

    private GridMap<PathNode> grid;
    private List<PathNode> openList;
    private List<PathNode> closedList;

    public EnemyPathfinding(int width, int height) {
        Instance = this;
        Vector3 originPos = new Vector3(-.32f, -.32f); // ORIGIN POSITION MEANING THE (0,0) OF THE GRID

        grid = new GridMap<PathNode>(width, height, .32f, originPos, (GridMap<PathNode> g, int x, int y) => new PathNode(g,x,y));
    }

    public GridMap<PathNode> GetGrid() {
        return grid;
    }

    public Vector3 FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition) {
        grid.GetXY(startWorldPosition, out int startX, out int startY);
        grid.GetXY(endWorldPosition, out int endX, out int endY);

        List<PathNode> path = FindPath(startX, startY, endX, endY);
        if(path == null) {
            return Vector2.zero;
        } else {
            //int n = path.Count;

            return new Vector3(path[1].x, path[1].y)*grid.cellSize + grid.originPosition;
        }
    }

    private List<PathNode> FindPath(int startX, int startY, int endX, int endY) {
        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);

        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>();

        for(int x = 0; x < grid.GetWidth(); x++) {
            for(int y = 0; y < grid.GetHeight(); y++) {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = 999999;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while(openList.Count > 0) {
            PathNode currentNode = GetLowestFCostNode(openList);
            if(currentNode == endNode) {
                //REACHED FINAL NODE
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach(PathNode neighbourNode in GetNeighbourList(currentNode)) {
                if(closedList.Contains(neighbourNode)) continue;
                if(!neighbourNode.isWalkableNode) {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if(tentativeGCost < neighbourNode.gCost) {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if(!openList.Contains(neighbourNode)) {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        // out of nodes on the open list
        return null;
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode) {
        List<PathNode> neighbourList = new List<PathNode>();

        if(currentNode.x - 1 >= 0) {
            // left
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
            // left down
            if(currentNode.y-1 >= 0) neighbourList.Add(GetNode(currentNode.x-1, currentNode.y-1));
            // left up
            if(currentNode.y+1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x-1, currentNode.y+1));
        }
        if(currentNode.x + 1 >= 0) {
            // right
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
            // right down
            if(currentNode.y-1 >= 0) neighbourList.Add(GetNode(currentNode.x+1, currentNode.y-1));
            // right up
            if(currentNode.y+1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x+1, currentNode.y+1));
        }
        // down
        if(currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x, currentNode.y-1));
        // up
        if(currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x, currentNode.y+1));

        return neighbourList;
    }

    private PathNode GetNode(int x, int y) {
        return grid.GetGridObject(x,y);
    }

    private List<PathNode> CalculatePath(PathNode endNode) {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while(currentNode.cameFromNode != null) {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    private int CalculateDistanceCost(PathNode a, PathNode b) {
        int xDistance = Mathf.Abs(a.x-b.x);
        int yDistance = Mathf.Abs(a.y-b.y);
        int remaining = Mathf.Abs(xDistance- yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList) {
        PathNode lowestFCostNode = pathNodeList[0];
        for(int i=0; i < pathNodeList.Count; i++) {
            if(pathNodeList[i].fCost < lowestFCostNode.fCost) {
                lowestFCostNode= pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }
}
