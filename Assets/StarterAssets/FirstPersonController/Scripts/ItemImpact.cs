
using UnityEngine;

public class ItemImpact : MonoBehaviour
{
    [SerializeField] private Item testItem;
    [SerializeField] private Item testItem2;
    [SerializeField] private GameObject inventory;
    private void Update()
    {
        if (InventoryInput.PressedG)
        {
            InventoryEvents.InvokeItemAdded(testItem, 1);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            inventory.SetActive(!inventory.activeInHierarchy);
        }
        if (InventoryInput.PressedR)
        {
            InventoryEvents.InvokeItemRemoved(1);
        }

        if (InventoryInput.PressedC)
        {
            InventoryEvents.InvokeClearInventory();
        }
        Cursor.visible = inventory.activeSelf;
        Cursor.lockState = !inventory.activeSelf ? CursorLockMode.Locked : CursorLockMode.None;
        /*fpsController._input.cursorInputForLook = inventory.activeSelf;
        fpsController._input.look = inventory.activeSelf?Vector2.zero:fpsController._input.look;*/
        
    }
}