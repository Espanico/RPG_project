using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public List<Item> items = new List<Item>();

    void Awake() {
        if(instance != null) {
            Debug.Log("ERROR: more than one inventory!");
            return;
        }
        instance = this;
    }

    public void Add(Item item) {
        items.Add(item);
    }

    public void Remove(Item item) {
        items.Remove(item);
    }
}
