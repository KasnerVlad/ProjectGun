using System;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets.FirstPersonController.Scripts.SOLIDInventory;
[Serializable]
public class GameData
{
    public Vector3 PlayerPosition;
    public List<Item> items;
    public List<int> itemsAmount;
    public string SaveName = "";
    public Quaternion PlayerRotation;
}
