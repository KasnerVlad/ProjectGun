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
            if (Input2.PressedG) {_=InventoryEvents.InvokeItemAdded(testItem, Amount); }
            if (Input.GetKeyDown(KeyCode.T)) { _=InventoryEvents.InvokeItemAdded(testItem2, Amount); }
            if (Input2.PressedE) { inventory.SetActive(!inventory.activeSelf); }
            if (Input2.PressedR) { InventoryEvents.InvokeItemRemoved(Amount); }
            if (Input2.PressedC) { InventoryEvents.InvokeClearInventory(); }
            Cursor.visible = inventory.activeSelf;
            Cursor.lockState = !inventory.activeSelf ? CursorLockMode.Locked : CursorLockMode.None;
            /*if(InputSystem.inistate.cursorInputForLook!=!inventory.activeSelf){ InputSystem.inistate.SetCursorStateLocked(!inventory.activeSelf);}
            if (InputSystem.inistate.look!=Vector2.zero&&inventory.activeSelf) { InputSystem.inistate.ZeroVector(); }*/
            if(inventory.activeSelf&&Input2.canLook) {Input2.canLook = false;  }
            else if(!inventory.activeSelf&&!Input2.canLook) {Input2.canLook = true;  }
        }
    }
}
