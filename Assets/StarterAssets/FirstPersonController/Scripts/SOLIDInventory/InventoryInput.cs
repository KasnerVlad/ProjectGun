using UnityEngine;

namespace StarterAssets.FirstPersonController.Scripts.SOLIDInventory
{
    public static class InventoryInput
    {
        public static bool PressedG => UnityEngine.Input.GetKeyDown(KeyCode.G);
        public static bool PressedR => UnityEngine.Input.GetKeyDown(KeyCode.R);
        public static bool PressedC => UnityEngine.Input.GetKeyDown(KeyCode.C);
        public static bool PressedE => UnityEngine.Input.GetKeyDown(KeyCode.E);
        public static bool StartDragging => UnityEngine.Input.GetMouseButtonDown(0);
        public static bool EndDragging => UnityEngine.Input.GetMouseButtonUp(0);
        public static float  Scroll => UnityEngine.Input.GetAxis("Mouse ScrollWheel");
    }
}
