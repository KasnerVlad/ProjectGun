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
            if (UnityEngine.Input.GetKeyDown(KeyCode.T))
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
            if(Input.inistate.cursorInputForLook!=!inventory.activeSelf){ Input.inistate.SetCursorStateLocked(!inventory.activeSelf);}
            if (Input.inistate.look!=Vector2.zero&&inventory.activeSelf) { Input.inistate.ZeroVector(); }
        }
    }
}
