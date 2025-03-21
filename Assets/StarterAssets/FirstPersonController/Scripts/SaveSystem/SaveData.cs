using System;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets.FirstPersonController.Scripts.SOLIDInventory;
[Serializable]
public class GameData
{
    public string SaveName = "";
    public Vector3 PlayerPosition;
    public Quaternion PlayerRotation;
    public List<Item> items;
    public List<int> itemsAmount;
    public int currentHotBarSlot = 0;
}
