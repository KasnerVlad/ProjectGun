using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLicens : MonoBehaviour
{
    [SerializeField]private Item item;
    [SerializeField]private int amount;
    
    public void SetAmount(int amount){this.amount = amount;}
    public Item GetItem(){return this.item;}
    public int GetAmount(){return this.amount;}
}
