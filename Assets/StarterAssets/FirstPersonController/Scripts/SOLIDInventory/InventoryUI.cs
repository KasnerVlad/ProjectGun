using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

namespace StarterAssets.FirstPersonController.Scripts.SOLIDInventory
{
    public class InventoryUI : MonoBehaviour
    {
        private List<InventorySlots> _slots;
        private readonly List<Text> _slotTexts = new List<Text>();
        private readonly List<Image> _slotImages = new List<Image>();
        
        private void Start()
        {
            InitializeSlots();
            InventoryEvents.OnInventoryUpdated += UpdateUI;
    
        }
    
        private void InitializeSlots()
        {
            DragAndDrop dragAndDrop = GetComponent<DragAndDrop>();
            _slots = new List<InventorySlots>(dragAndDrop.InventorySlots);
            for (int i = 0; i < 32; i++)
            {
                _slotTexts.Add(_slots[i].Slot.transform.GetChild(0).GetChild(0).GetComponent<Text>());
                _slotImages.Add(_slots[i].Slot.transform.GetChild(0).GetComponent<Image>());
            }
        }
        private void OnDestroy()
        {
            InventoryEvents.OnInventoryUpdated -= UpdateUI;
        }
        
        private async Task UpdateUI(List<InventorySlots> slots)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                _slotTexts[i].text = slots[i].Amount<=1?"" : slots[i].Amount.ToString();
                _slotImages[i].sprite =slots[i].Amount<=0? null : slots[i].Item.icon;
                _slotImages[i].enabled = !(slots[i].Amount<=0);
            }
            
            await Task.Yield();
        }
    }
}
