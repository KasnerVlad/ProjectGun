using UnityEngine;

namespace StarterAssets.FirstPersonController.Scripts.SOLIDInventory
{
    public static class InventoryInput
    {
        public static bool PressedG => Input.GetKeyDown(KeyCode.G);
        public static bool PressedR => Input.GetKeyDown(KeyCode.R);
        public static bool PressedC => Input.GetKeyDown(KeyCode.C);
        public static bool PressedE => Input.GetKeyDown(KeyCode.E);
        public static bool StartDragging => Input.GetMouseButtonDown(0);
        public static bool Dragging => Input.GetMouseButton(0);
        public static bool EndDragging => Input.GetMouseButtonUp(0);
    }
}
