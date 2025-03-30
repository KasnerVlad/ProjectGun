using UnityEngine;
using StarterAssets.FirstPersonController.Scripts.SOLIDInventory;

namespace StarterAssets.FirstPersonController.Scripts.SOLIDInventory
{
    public class ObjectLicens : MonoBehaviour
    {
        [SerializeField] private Item item;
        [SerializeField] private int amount;
        public void SetAmount(int itemAmount){this.amount = itemAmount;}
        public Item GetItem(){return this.item;}
        public int GetAmount(){return this.amount;}
    }
}
