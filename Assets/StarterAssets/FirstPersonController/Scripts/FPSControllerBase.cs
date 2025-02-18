using UnityEngine;
using UnityEngine.InputSystem;

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public abstract class FPSControllerBase : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        [SerializeField] private float moveSpeed;
        private float? _moveSpeedCache = null;
        public float MoveSpeed {
            get { if (_moveSpeedCache == null) { _moveSpeedCache = moveSpeed; } return _moveSpeedCache.Value; }
            private set { if (_moveSpeedCache == null) { _moveSpeedCache = Mathf.Clamp(value, 0.5f, SprintSpeed/2);; } }
        }
        public float _animationBlend {private set; get;}
        [Tooltip("Sprint speed of the character in m/s")]
        [SerializeField] private float sprintSpeed;
        private float? _sprintSpeedCache = null;
        public float SprintSpeed {
            get { if (_sprintSpeedCache == null) { _sprintSpeedCache = sprintSpeed; } return _sprintSpeedCache.Value; }
            private set { if (_sprintSpeedCache == null) { _sprintSpeedCache = Mathf.Clamp(value, 2, 6);; } }
        }
		
        [Tooltip("Rotation speed of the character")] 
        [SerializeField] private float rotationSpeed;
        private float? _rotationSpeedCache = null;
        public float RotationSpeed {
            get { if (_rotationSpeedCache == null) { _rotationSpeedCache = rotationSpeed; } return _rotationSpeedCache.Value; }
            private set { if (_rotationSpeedCache == null) { _rotationSpeedCache = value; } }
        }

        [Tooltip("Acceleration and deceleration")] 
        [Range(1, 15)] 
        [SerializeField] private float speedChangeRate;
        private float? _speedChangeRateCache = null;
        public float SpeedChangeRate {
            get { if (_speedChangeRateCache == null) { _speedChangeRateCache = speedChangeRate; } return _speedChangeRateCache.Value; }
            private set { if (_speedChangeRateCache == null) { _speedChangeRateCache = Mathf.Clamp(value, 1, 15);; } }
        }

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
        [SerializeField] private float jumpHeight;
        private float? _jumpHeightCache = null; 
        public float JumpHeight { 
            get { if (_jumpHeightCache == null) { _jumpHeightCache = jumpHeight; } return _jumpHeightCache.Value; }
            private set { if (_jumpHeightCache == null) { _jumpHeightCache = value; } }
        }
        [Range(-1, -15)]
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")] 
        [SerializeField] private float gravity;
        private float? _gravityCache = null; 
        public float Gravity {
            get { if (_gravityCache == null) { _gravityCache = gravity; } return _gravityCache.Value; }
            private set { if (_gravityCache == null) { _gravityCache = Mathf.Clamp(value, -15, -1);; } }
        }
        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public readonly float JumpTimeout = 0.1f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        [SerializeField] private float fallTimeout = 0.5f;
        private float? _fallTimeoutCache = null; // Хранит зафиксированное значение

        public float FallTimeout {
            get { if (_fallTimeoutCache == null) { _fallTimeoutCache = fallTimeout; } return _fallTimeoutCache.Value; }
            private set { if (_fallTimeoutCache == null) { _fallTimeoutCache = value; } }
        }

        /*[Header("Player Grounded")]*/
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded {private set; get;}
        [Range(0, -1)]
        [Tooltip("Useful for rough ground")]
        [SerializeField] private float groundedOffset;
        private float? _groundedOffsetCache = null;
        public float GroundedOffset {
            get { if (_groundedOffsetCache == null) { _groundedOffsetCache = groundedOffset; } return _groundedOffsetCache.Value; }
            private set { if (_groundedOffsetCache == null) { _groundedOffsetCache = value; } }
        }
        [Range(0, 1)]
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        [SerializeField] private float groundRadius;
        private float? _groundRadiusCache = null;
        public float GroundedRadius {
            get { if (_groundRadiusCache == null) { _groundRadiusCache = groundRadius; } return _groundRadiusCache.Value; }
            private set { if (_groundRadiusCache == null) { _groundRadiusCache = value; } }
        }

        [Tooltip("What layers the character uses as ground")] 
        [SerializeField] private LayerMask groundLayer;
        private LayerMask? _groundPositionCache = null;
        public LayerMask GroundLayers {
            get{if (_groundPositionCache == null) { _groundPositionCache = groundLayer; } return _groundPositionCache.Value;}
            private set { if (_groundPositionCache == null) { _groundPositionCache = value; } }
        }

        /*[Header("Cinemachine")]*/
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
		
        [Tooltip("How far in degrees can you move the camera up")] 
        [SerializeField] protected Vector2 cinemachineCameraAngleCalm;
        private float? _topClampCache = null;
        private float? _bottomClampCache = null;
        public float TopClamp {
            get { if (_topClampCache == null) { _topClampCache = cinemachineCameraAngleCalm.x; } return _topClampCache.Value; }
            private set { if (_topClampCache == null) { _topClampCache = value; } }
        }

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp {
            get { if (_bottomClampCache == null) { _bottomClampCache = cinemachineCameraAngleCalm.y; } return _bottomClampCache.Value; }
            private set { if (_bottomClampCache == null) { _bottomClampCache = value; } }
        }

        [SerializeField] protected GameObject headsImpact;
        public Vector3 defultHipsPosition{private set; get;}

        // cinemachine
        public float _cinemachineTargetPitch{private set; get;}

        // player
        public float _speed{private set; get;}
        public float _rotationVelocity{private set; get;}
        public float _verticalVelocity{private set; get;}
		
        public readonly float _terminalVelocity = 53f;

        // timeout deltatime
        public float _jumpTimeoutDelta{private set; get;}
        public float _fallTimeoutDelta{private set; get;}
		
        // animation IDs
        public int _animIDSpeed{private set; get;}
        public int _animIDGrounded{private set; get;}
        public int _animIDJump{private set; get;}
        public int _animIDFreeFall{private set; get;}
        public int _animIDMotionSpeed{private set; get;}
        public int _dirX{private set; get;}
        public int _dirZ{private set; get;}
	
#if ENABLE_INPUT_SYSTEM
        public PlayerInput _playerInput{private set; get;}
#endif
        public CharacterController _controller{private set; get;}
        public StarterAssetsInputs _input{private set; get;}
        public GameObject _mainCamera{private set; get;}
        public readonly float _threshold = 0.01f;
        public Vector2 lerpSpeed{private set; get;}
        public bool _hasAnimator{private set; get;}
        public Animator _animator;
        [SerializeField] public GameObject aimingPoint;
        [SerializeField] public GameObject head;
        public Vector3 _headDefultPosition{private set; get;}
        public bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }

        protected virtual void Awake()
        {
            InitializeCamera();
        }
        protected virtual void Start()
        {
            InitializeStart();
        }

        protected virtual void Update()
        {
            UpdateLogic();
        }

        protected virtual void LateUpdate()
        {
            LateUpdateLogic();
        }
        protected abstract void InitializeCamera();
        protected abstract void InitializeStart();
        protected abstract void UpdateLogic();
        protected abstract void LateUpdateLogic();
        public virtual void SetAnimationBlend(float animationBlend) => _animationBlend = animationBlend;
        public virtual void SetGrounded(bool grounded) => Grounded = grounded;
        public virtual void SetDefultHipsPosition(Vector3 defultHipsPosition) => this.defultHipsPosition = defultHipsPosition;
        public virtual void SetCinemachineTargetPitch(float cinemachineTargetPitch) => _cinemachineTargetPitch = cinemachineTargetPitch;
        public virtual void SetSpeed(float speed) => _speed = speed;
        public virtual void SetRotationVelocity(float rotationVelocity) => _rotationVelocity = rotationVelocity;
        public virtual void SetVerticalVelocity(float verticalVelocity) => _verticalVelocity = verticalVelocity;
        public virtual void SetJumpTimeoutDelta(float jumpTimeoutDelta) => _jumpTimeoutDelta = jumpTimeoutDelta;
        public virtual void SetFallTimeoutDelta(float fallTimeoutDelta) => _fallTimeoutDelta = fallTimeoutDelta;
        public virtual void SetAnimIDSpeed(int animIDSpeed) => _animIDSpeed = animIDSpeed;
        public virtual void SetAnimIDGrounded(int animIDGrounded) => _animIDGrounded = animIDGrounded;
        public virtual void SetAnimIDJump(int animIDJump) => _animIDJump = animIDJump;
        public virtual void SetAnimIDFreeFall(int animIDFreeFall) => _animIDFreeFall = animIDFreeFall;
        public virtual void SetAnimIDMotionSpeed(int animIDMotionSpeed) => _animIDMotionSpeed = animIDMotionSpeed;
        public virtual void SetDirX(int dirX) => _dirX = dirX;
        public virtual void SetDirZ(int dirZ) => _dirZ = dirZ;
        public virtual void SetPlayerInput(PlayerInput playerInput) => _playerInput = playerInput;
        public virtual void SetInput(StarterAssetsInputs input) => _input = input;
        public virtual void SetMainCamera(GameObject mainCamera) => _mainCamera = mainCamera;
        public virtual void SetLerpSpeed(Vector2 lerpSpeed) => this.lerpSpeed = lerpSpeed;
        public virtual void SetHasAnimator(bool hasAnimator) => _hasAnimator = hasAnimator;
        public virtual void SetHeadDefultPosition(Vector3 headDefultPosition) => _headDefultPosition = headDefultPosition;
        public virtual void SetController(CharacterController characterController) => _controller = characterController;

        public static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
    }
}