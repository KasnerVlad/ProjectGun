
using StarterAssets;
using UnityEngine;

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
            InventoryEvents.InvokeItemAdded(testItem, Amount);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            InventoryEvents.InvokeItemAdded(testItem2, Amount);
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
        fpsController._input.cursorInputForLook = !inventory.activeSelf;
        if (inventory.activeSelf) { fpsController._input.look = Vector2.zero; }
        
    }
}