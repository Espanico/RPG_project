using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingGrid : MonoBehaviour
{
    public GameObject player;
    private void Start()
    {
        EnemyPathfinding pathfinding = new EnemyPathfinding(13, 8);
        
    }
}
