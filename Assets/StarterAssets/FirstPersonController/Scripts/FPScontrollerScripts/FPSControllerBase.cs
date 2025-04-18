using System;
using StarterAssets.FirstPersonController.Scripts.PlayerHpSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace StarterAssets.FirstPersonController.Scripts
{
    /*[RequireComponent(typeof(CharacterController))]*/
    public abstract class FPSControllerBase : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        [SerializeField] private float moveSpeed;
        public float MoveSpeed { get { return moveSpeed; }private set { moveSpeed = Mathf.Clamp(value, 1, SprintSpeed/2);  } }

        [Tooltip("Sprint speed of the character in m/s")]
        [SerializeField] private float sprintSpeed;
        public float SprintSpeed { get { return sprintSpeed; } private set { sprintSpeed = Mathf.Clamp(value, MoveSpeed, 6); } }
		
        [Tooltip("Rotation speed of the character")] 
        [Range(0, 1)]
        [SerializeField] private float rotationSpeed;
        public float RotationSpeed{ get { return rotationSpeed; } private set { rotationSpeed = Mathf.Clamp(value, 0, 1); } }

        [Tooltip("Acceleration and deceleration")] 
        [Range(1, 15)] 
        [SerializeField] private float speedChangeRate;
        public float SpeedChangeRate{ get { return speedChangeRate; } private set { speedChangeRate = Mathf.Clamp(value, 1, 15); } }

        [Tooltip("Acceleration and deceleration")] 
        [SerializeField] private AudioClip landingAudioClip;
        public AudioClip LandingAudioClip=>landingAudioClip;

        [SerializeField] private AudioClip[] footstepAudioClips;
        public AudioClip[] FootstepAudioClips=>footstepAudioClips;
        [Range(0, 1)] 
        [SerializeField] private float footstepAudioVolume;
        public float FootstepAudioVolume
        {
            get => footstepAudioVolume;
            private set => footstepAudioVolume = Mathf.Clamp(value, 0, 1); 
        }

        [Space(10)]
        [Tooltip("The height the player can jump")] 
        [Range(0.1f, 2)]
        [SerializeField] private float jumpHeight;
        public float JumpHeight { get { return jumpHeight;} private set{jumpHeight = Mathf.Clamp(value, 0.1f, 2);} }
        [Range(-9.8f, -15)]
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")] 
        [SerializeField] private float gravity;
        public float Gravity{ get { return gravity; } private set { gravity = Mathf.Clamp(value, -15,-9.8f); } }
        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public readonly float JumpTimeout = 0.2f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        [SerializeField] private float fallTimeout = 0.5f;
        public float FallTimeout => fallTimeout;

        /*[Header("Player Grounded")]*/
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded {private set; get;}
        [Range(0, -1)]
        [Tooltip("Useful for rough ground")]
        [SerializeField] private float groundedOffset;
        public float GroundedOffset=>groundedOffset;
        [SerializeField]private float secondGroundOffset;
        public float SecondGroundOffset => secondGroundOffset;
        [Range(0, 1)]
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        [SerializeField] private float groundRadius;
        public float GroundedRadius=>groundRadius;
        [SerializeField] protected Slider hpScrollbar;
        [Tooltip("What layers the character uses as ground")] 
        [SerializeField] private LayerMask groundLayer;
        public LayerMask GroundLayers => groundLayer;

        /*[Header("Cinemachine")]*/
        [FormerlySerializedAs("CinemachineCameraTarget")] [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject cinemachineCameraTarget;

        public GameObject minHitDistance;
        public GameObject maxHitDistance;
        [SerializeField] private GameObject mainCamera;
        public GameObject MainCamera => mainCamera;
        public GameObject cameraPoint;
        [Tooltip("How far in degrees can you move the camera up")] 
        [SerializeField] private Vector2 cinemachineCameraAngleCalm;

        public float TopClamp => cinemachineCameraAngleCalm.x;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp => cinemachineCameraAngleCalm.y;

        [SerializeField] protected GameObject headsImpact;
        public readonly float Threshold = 0.01f;
        [SerializeField] public GameObject aimingPoint;
        [SerializeField] public GameObject head;
        /*public bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }*/
        protected virtual void Start() { InitializeStart(); }
        protected virtual void Update() { UpdateLogic(); }
        protected virtual void LateUpdate() { LateUpdateLogic(); }
        protected abstract void InitializeStart();
        protected abstract void UpdateLogic();
        protected abstract void LateUpdateLogic();
        protected void SetGrounded(bool grounded) => Grounded = grounded;
        public static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
    }
}