using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;
    public void PickUp() {
        bool wasPickedUp = Inventory.instance.Add(item);
        if(wasPickedUp) {
            Destroy(gameObject);
        }
    }
}
