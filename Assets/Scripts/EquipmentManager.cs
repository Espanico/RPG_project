using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    #region Singleton
    public static EquipmentManager instance;
    
    void Awake() {
        instance = this;
    }

    #endregion

    public Equipment[] currentEquipment;

    Inventory inventory;

    void Start() {
        inventory = Inventory.instance;
        int numSlots = System.Enum.GetNames(typeof(EquipmentType)).Length;
        currentEquipment = new Equipment[numSlots];
    }

    public void Equip(Equipment newItem) {
        int slotIndex = (int)newItem.equipmentSlot;

        if (currentEquipment[slotIndex] != null) {
            Equipment oldItem = currentEquipment[slotIndex];
            inventory.Add(oldItem);
        }



        currentEquipment[slotIndex] = newItem;
    }

    public void Unequip(int slotIndex) {
        if(currentEquipment[slotIndex] != null) {
            Equipment oldItem = currentEquipment[slotIndex];
            inventory.Add(oldItem);

            currentEquipment[slotIndex] = null; 
        }
    }

    public void UnequipAll() {
        for (int i = 0; i < currentEquipment.Length; i++)
        {
            Unequip(i);
        }
    }
}
