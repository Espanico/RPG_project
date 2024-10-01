using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private LayerMask solidObjectLayer;
    private LayerMask playerLayer;
    private LayerMask interactableLayer;


    private GridMap<PathNode> grid;
    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public bool isWalkableNode;
    public PathNode cameFromNode;
    public PathNode(GridMap<PathNode> grid, int x, int y) {
        this.grid = grid;
        this.x = x;
        this.y = y;

        solidObjectLayer = LayerMask.GetMask("SolidObject");
        playerLayer = LayerMask.GetMask("Player");
        interactableLayer = LayerMask.GetMask("Interactable");
                                                            // ORIGIN POSITION
        if(Physics2D.OverlapCircle((new Vector2(x,y)*.32f)+(new Vector2(grid.originPosition.x,grid.originPosition.y)), 0.1f, solidObjectLayer | interactableLayer ) != null) {
            isWalkableNode = false;
            //Debug.Log((x*.32f-.32f) + " || " + (y*.32f-.32f));
        } else {isWalkableNode = true;}
    }

    public void CalculateFCost() {
        fCost = gCost + hCost;
    }

    public override string ToString()
    {
        return x + ", " + y;
    }
}
