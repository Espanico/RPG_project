using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ColliderChecker : MonoBehaviour
{
    public GameObject player;
    void Awake() {
        
    }
    private void OnTriggerEnter2D(Collider2D coll) {
        if(!player.GetComponent<Player>().colliderList.Contains(coll.gameObject) && coll.tag == "Enemy") {
            player.GetComponent<Player>().colliderList.Add(coll.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D coll) {
        if(player.GetComponent<Player>().colliderList.Contains(coll.gameObject)) {
            player.GetComponent<Player>().colliderList.Remove(coll.gameObject);
        }
    }
}
