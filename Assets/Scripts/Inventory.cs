using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public delegate void OnItemChange();
    public OnItemChange onItemChangedCallback;
    public int space = 20;
    public List<Item> items = new List<Item>();

    void Awake() {
        if(instance != null) {
            Debug.Log("ERROR: more than one inventory!");
            return;
        }
        instance = this;
    }

    public bool Add(Item item) {
        if(items.Count > space) {
            Debug.Log("Too many items");
            return false;
        }
        items.Add(item);

        if(onItemChangedCallback != null) {
            onItemChangedCallback.Invoke();
        }
        
        return true;
    }

    public void Remove(Item item) {
        items.Remove(item);
        
        if(onItemChangedCallback != null) {
            onItemChangedCallback.Invoke();
        }
    }
}
