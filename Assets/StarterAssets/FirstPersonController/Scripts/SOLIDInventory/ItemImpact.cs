using StarterAssets;
using UnityEngine;

namespace StarterAssets.FirstPersonController.Scripts.SOLIDInventory
{
    public class ItemImpact : MonoBehaviour
    {
        [SerializeField] private Item testItem;
        [SerializeField] private Item testItem2;
        [SerializeField] private GameObject inventory;
        [SerializeField] private FPSControllerBase fpsController;
        [SerializeField] private int Amount;
        private void Update()
        {
            if (InventoryInput.PressedG)
            {
                _=InventoryEvents.InvokeItemAdded(testItem, Amount);
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                _=InventoryEvents.InvokeItemAdded(testItem2, Amount);
            }
            if (InventoryInput.PressedE)
            {
                inventory.SetActive(!inventory.activeInHierarchy);
            }
            if (InventoryInput.PressedR)
            {
                InventoryEvents.InvokeItemRemoved(Amount);
            }
    
            if (InventoryInput.PressedC)
            {
                InventoryEvents.InvokeClearInventory();
            }
            Cursor.visible = inventory.activeSelf;
            Cursor.lockState = !inventory.activeSelf ? CursorLockMode.Locked : CursorLockMode.None;
            fpsController.Input.cursorInputForLook = !inventory.activeSelf;
            if (inventory.activeSelf) { fpsController.Input.look = Vector2.zero; }
            
        }
    }
}
