using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EquipmentUI : MonoBehaviour
{
    public Transform equipmentParent;
    Inventory inventory;
    EquipmentSlot[] equipSlots;

    void Start()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;

        equipSlots = equipmentParent.GetComponentsInChildren<EquipmentSlot>();
    }


    void Update()
    {
        
    }

    public void UpdateUI() {
        EquipmentManager inv = EquipmentManager.instance;
        for (int i = 0; i < System.Enum.GetNames(typeof(EquipmentType)).Length; i++)
        {
            if(inv.currentEquipment[i] != null) {
                equipSlots[i].AddItem(inv.currentEquipment[i]);
            } else {
                equipSlots[i].ClearSlot();
            }
        }
    }
}
