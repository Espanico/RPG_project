using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool isPlayerTurn = true;
    //int actions;
    List<GameObject> enemyList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        //actions = 1;
        findEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isPlayerTurn) {
            for (int i = 0; i < enemyList.Count; i++)
            {
                if(enemyList[i].IsDestroyed()) {
                    enemyList.RemoveAt(i);
                } else {
                    enemyList[i].GetComponent<Enemy>().performAction();
                }
            }
            isPlayerTurn = true;
        }
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
        }
    }

}