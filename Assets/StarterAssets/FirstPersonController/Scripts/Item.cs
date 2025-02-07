using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Item

public enum ItemType { Weapon, Armor, Consumable, Resource }
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]

public class Item : ScriptableObject
{
    public int id;
    public string name;
    public string description;
    public Sprite icon;
    public ItemType type;
    public int maxStackSize = 99;
    public GameObject prefab;
}


#endregion

#region Resources
[CreateAssetMenu(fileName = "New FoodItem", menuName = "Inventory/FoodItem")]
public class FoodItem : Item
{
    public ItemType type = ItemType.Consumable;
    public int healAmount;
    public int addEnergyAmount;
    public int addThirthyAmount;
}

#endregion