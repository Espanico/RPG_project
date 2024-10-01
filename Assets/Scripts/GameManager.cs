using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isPlayerTurn = true;
    //int actions;
    List<GameObject> enemyList = new List<GameObject>();

    void Start()
    {
        //actions = 1;
        findEnemies();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void findEnemies() {
        Object[] tempList = Resources.FindObjectsOfTypeAll(typeof(GameObject));
        GameObject temp;

        foreach(Object obj in tempList){
			if(obj is GameObject){
				temp = (GameObject)obj;
				if(temp.tag == "Enemy")
				    enemyList.Add((GameObject)obj);
			}
		}
    }

    public void enemiesTurn() {
        if(enemyList.Count != 0) {
            isPlayerTurn = false;

           for (int i = 0; i < enemyList.Count; i++)
            {
                if(enemyList[i].IsDestroyed()) {
                    enemyList.RemoveAt(i);
                } else {
                    enemyList[i].GetComponent<Enemy>().PerformAction();
                }
            }
        }
        isPlayerTurn = true;
    }

}
