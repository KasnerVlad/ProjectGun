using System;
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

        private void Awake() { StartLogic(); }

        private void StartLogic()
        {
            if(instance == null){instance = this;}
            else { Destroy(this); }
            DontDestroyOnLoad(this);
            foreach (var child in itemChoosePanel.GetComponentsInChildren<ObjectLicens>())
            {
                itemsLicenses.Add(child);
                items.Add(child.gameObject);
            }
        }
        public void UpdateChosenItems(int selctedSlot, List<InventorySlots> hotBarSlots)
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].SetActive(false);
            }
            for (int i = 0; i < itemsLicenses.Count; i++)
            {
                if (hotBarSlots[selctedSlot].Item == itemsLicenses[i].GetItem())
                {
                    items[i].SetActive(true);
                }   
            }
        }
    }
}