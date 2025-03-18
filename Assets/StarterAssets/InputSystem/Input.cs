using UnityEngine;
using CInvoke;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class Input : MonoBehaviour
	{
		public static Input inistate;
		/*[Header("Character Input Values")]*/
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;

		/*[Header("Movement Settings")]*/
		public bool analogMovement;

		/*
		[Header("Mouse Cursor Settings")]*/
		public bool cursorLocked = true;
		public bool cursorInputForLook= true;
		
		public bool pressedE;
		public float scroll;
		public Vector2 mouseDelta;
		public bool startDragging;
		public bool endDragging;
		private void Start()
		{
			if (inistate == null) { inistate = this; }
			else { Destroy(this); }
			DontDestroyOnLoad(this);
		}
#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)=> MoveInput(value.Get<Vector2>());
		public void OnLook(InputValue value) { if(cursorInputForLook) {LookInput(value.Get<Vector2>()); } }
		public void OnJump(InputValue value)=>JumpInput(value.isPressed);
		public void OnSprint(InputValue value)=> SprintInput(value.isPressed);
		public void OnStartDragging(InputValue value){StartDraggingInput(value.isPressed);_=CustomInvoke.Invoke(StartDraggingInput, !value.isPressed, 10); }
		public void OnEndDragging(InputValue value){EndDraggingInput(value.isPressed);}
		public void OnScroll(InputValue value){ScrollInput(value.Get<float>());}
		public void OnMouse(InputValue value){MouseInput(value.Get<Vector2>());}
		public void OnToggleInventory(InputValue value){EInput(pressedE?!value.isPressed:value.isPressed);}
#endif
		private void MoveInput(Vector2 newMoveDirection) => move = newMoveDirection;
		private void LookInput(Vector2 newLookDirection) => look = newLookDirection;
		private void JumpInput(bool newJumpState) => jump = newJumpState;
		private void SprintInput(bool newSprintState) => sprint = newSprintState;
		private void EInput(bool pressed)=>pressedE = pressed;
		private void ScrollInput(float scroll)=>this.scroll = scroll;
		private void StartDraggingInput(bool pressed)=>this.startDragging = pressed;
		private void EndDraggingInput(bool pressed)=>this.endDragging = pressed;
		private void MouseInput(Vector2 mouseDelta)=>this.mouseDelta = mouseDelta;
		private void OnApplicationFocus(bool hasFocus) => SetCursorState(cursorLocked);
		
		private void SetCursorState(bool newState) => Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		public void SetCursorStateLocked(bool value) => cursorInputForLook = value;
		public void ZeroVector() => look = Vector2.zero;
	}
	
}