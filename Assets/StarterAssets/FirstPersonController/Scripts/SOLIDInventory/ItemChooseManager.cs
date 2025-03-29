using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace StarterAssets.FirstPersonController.Scripts.SOLIDInventory
{
    public class ItemChooseManager : MonoBehaviour
    {
        public static ItemChooseManager instance;
        [SerializeField] private List<GameObject> items = new List<GameObject>();
        [SerializeField] private List<UnityEvent> itemActionsOnActive = new List<UnityEvent>();
        [SerializeField] private List<UnityEvent> itemActionsOnDisable = new List<UnityEvent>();
        private List<ObjectLicens> itemsLicenses = new List<ObjectLicens>();
        [SerializeField]private GameObject itemChoosePanel;

        private void Awake() { StartLogic(); }

        private void StartLogic()
        {
            if(instance == null){instance = this;}
            else { Destroy(this); }
            DontDestroyOnLoad(this);
            for (int i = 0; i < items.Count; i++)
            {
                itemsLicenses.Add(items[i].GetComponent<ObjectLicens>());
            }
        }
        public void UpdateChosenItems(int selctedSlot, List<InventorySlots> hotBarSlots)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if(itemActionsOnDisable[i]!=null){ itemActionsOnDisable[i].Invoke();}
            }
            for (int i = 0; i < itemsLicenses.Count; i++)
            {
                if (hotBarSlots[selctedSlot].Item == itemsLicenses[i].GetItem())
                {
                    if(itemActionsOnActive[i]!=null){ itemActionsOnActive[i].Invoke();}
                }   
            }
        }
    }
}