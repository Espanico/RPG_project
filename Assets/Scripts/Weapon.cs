using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Equipment
{
    public int damage;

    void Awake() {
        damageModifier = damage;
    }
}
