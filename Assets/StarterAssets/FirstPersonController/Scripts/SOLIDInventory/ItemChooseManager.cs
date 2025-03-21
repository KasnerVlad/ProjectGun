using System.Collections.Generic;
using UnityEngine;

namespace StarterAssets.FirstPersonController.Scripts.SOLIDInventory
{
    public class ItemChooseManager : MonoBehaviour
    {
        public static ItemChooseManager instance;
        private List<GameObject> items = new List<GameObject>();
        private List<ObjectLicens> itemsLicenses = new List<ObjectLicens>();
        [SerializeField]private GameObject itemChoosePanel;
        
        private void Start() => StartLogic();
        private void StartLogic()
        {
            if(instance == null){instance = this;}
            else { Destroy(this.gameObject); }
            DontDestroyOnLoad(this);
            foreach (var child in itemChoosePanel.GetComponentsInChildren<ObjectLicens>())
            {
                itemsLicenses.Add(child);
                items.Add(child.gameObject);
            }
            /*UpdateChosenItems(selectedItem);*/
        }
        public void UpdateChosenItems(int selctedSlot, List<InventorySlots> hotBarSlots)
        {
            foreach (var slot in hotBarSlots)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (slot.Item == itemsLicenses[i].GetItem()&&slot == hotBarSlots[selctedSlot]) { items[i].SetActive(true); }
                    else if (items[i].activeSelf) { items[i].SetActive(false); }
                }
            }
        }
    }
}