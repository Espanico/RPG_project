using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int enemiesPerformingAction = 0;
    //int actions;
    List<GameObject> enemyList = new List<GameObject>();

    void Start()
    {
        //actions = 1;
        FindEnemies();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FindEnemies() {
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

    public void PerformEnemiesTurn() {
        if(enemyList.Count != 0) {

           for (int i = 0; i < enemyList.Count; i++)
            {
                if(enemyList[i].IsDestroyed()) {
                    enemyList.RemoveAt(i);
                } else {
                    enemiesPerformingAction++;
                    enemyList[i].GetComponent<Enemy>().PerformAction();
                }
            }
        }
    }

}
