using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
namespace StarterAssets
{
    public class UIInput : MonoBehaviour
    {
        public bool pressedE;
        public float scroll;
        public Vector2 mouseDelta;
        public bool startDragging;
        public bool endDragging;
#if ENABLE_INPUT_SYSTEM
    
#endif
        private void EInput(bool pressed)=>pressedE = pressed;
        private void ScrollInput(float scroll)=>this.scroll = scroll;
        private void StartDraggingInput(bool pressed)=>this.startDragging = pressed;
        private void EndDraggingInput(bool pressed)=>this.endDragging = pressed;
        private void MouseInput(Vector2 mouseDelta)=>this.mouseDelta = mouseDelta;
    }
}