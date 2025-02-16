using UnityEngine;
using System.Threading.Tasks;
using Random = UnityEngine.Random;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

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
		public float MoveSpeed		
		{
			get
			{
				if (_moveSpeedCache == null)
				{
					_moveSpeedCache = moveSpeed;
				}
				return _moveSpeedCache.Value;
			}
			private set
			{
				if (_moveSpeedCache == null)
				{
					_moveSpeedCache = Mathf.Clamp(value, 0.5f, SprintSpeed/2);;
				}
			}
		}
		public float _animationBlend {private set; get;}
		[Tooltip("Sprint speed of the character in m/s")]
		[SerializeField] private float sprintSpeed;
		private float? _sprintSpeedCache = null;
		public float SprintSpeed
		{
			get
			{
				if (_sprintSpeedCache == null)
				{
					_sprintSpeedCache = sprintSpeed;
				}
				return _sprintSpeedCache.Value;
			}
			private set
			{
				if (_sprintSpeedCache == null)
				{
					_sprintSpeedCache = Mathf.Clamp(value, 2, 6);;
				}
			}
		}
		
		[Tooltip("Rotation speed of the character")] 
		[SerializeField] private float rotationSpeed;
		private float? _rotationSpeedCache = null;
		public float RotationSpeed
		{
			get
			{
				if (_rotationSpeedCache == null)
				{
					_rotationSpeedCache = rotationSpeed;
				}
				return _rotationSpeedCache.Value;
			}
			private set
			{
				if (_rotationSpeedCache == null)
				{
					_rotationSpeedCache = value;
				}
			}
		}

		[Tooltip("Acceleration and deceleration")] 
		[Range(1, 15)] 
		[SerializeField] private float speedChangeRate;
		private float? _speedChangeRateCache = null;
		public float SpeedChangeRate
		{
			get
			{
				if (_speedChangeRateCache == null)
				{
					_speedChangeRateCache = speedChangeRate;
				}
				return _speedChangeRateCache.Value;
			}
			private set
			{
				if (_speedChangeRateCache == null)
				{
					_speedChangeRateCache = Mathf.Clamp(value, 1, 15);;
				}
			}
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
		public float JumpHeight
		{
			get
			{
				if (_jumpHeightCache == null)
				{
					_jumpHeightCache = jumpHeight;
				}
				return _jumpHeightCache.Value;
			}
			private set
			{
				if (_jumpHeightCache == null)
				{
					_jumpHeightCache = value;
				}
			}
		}
		[Range(-1, -15)]
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")] 
		[SerializeField] private float gravity;
		private float? _gravityCache = null; 
		public float Gravity
		{
			get
			{
				if (_gravityCache == null)
				{
					_gravityCache = gravity;
				}
				return _gravityCache.Value;
			}
			private set
			{
				if (_gravityCache == null)
				{
					_gravityCache = Mathf.Clamp(value, -15, -1);;
				}
			}
		}
		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public readonly float JumpTimeout = 0.1f;

		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		[SerializeField] private float fallTimeout = 0.5f;
		private float? _fallTimeoutCache = null; // Хранит зафиксированное значение

		public float FallTimeout
		{
			get
			{
				if (_fallTimeoutCache == null)
				{
					_fallTimeoutCache = fallTimeout;
				}
				return _fallTimeoutCache.Value;
			}
			private set
			{
				if (_fallTimeoutCache == null)
				{
					_fallTimeoutCache = value;
				}
			}
		}

		/*[Header("Player Grounded")]*/
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded {private set; get;}
		[Range(0, -1)]
		[Tooltip("Useful for rough ground")]
		[SerializeField] private float groundedOffset;
		private float? _groundedOffsetCache = null;
		public float GroundedOffset
		{
			get
			{
				if (_groundedOffsetCache == null)
				{
					_groundedOffsetCache = groundedOffset;
				}
				return _groundedOffsetCache.Value;
			}
			private set
			{
				if (_groundedOffsetCache == null)
				{
					_groundedOffsetCache = value;
				}
			}
		}
		[Range(0, 1)]
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		[SerializeField] private float groundRadius;
		private float? _groundRadiusCache = null;
		public float GroundedRadius
		{
			get
			{
				if (_groundRadiusCache == null)
				{
					_groundRadiusCache = groundRadius;
				}
				return _groundRadiusCache.Value;
			}
			private set
			{
				if (_groundRadiusCache == null)
				{
					_groundRadiusCache = value;
				}
			}
		}

		[Tooltip("What layers the character uses as ground")] 
		[SerializeField] protected LayerMask GroundLayers;

		/*[Header("Cinemachine")]*/
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		
		[Tooltip("How far in degrees can you move the camera up")] 
		[SerializeField] protected Vector2 cinemachineCameraAngleCalm;
		private float? _topClampCache = null;
		private float? _bottomClampCache = null;
		public float TopClamp
		{
			get
			{
				if (_topClampCache == null)
				{
					_topClampCache = cinemachineCameraAngleCalm.x;
				}
				return _topClampCache.Value;
			}
			private set
			{
				if (_topClampCache == null)
				{
					_topClampCache = value;
				}
			}
		}

		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp
		{
			get
			{
				if (_bottomClampCache == null)
				{
					_bottomClampCache = cinemachineCameraAngleCalm.y;
				}
				return _bottomClampCache.Value;
			}
			private set
			{
				if (_bottomClampCache == null)
				{
					_bottomClampCache = value;
				}
			}
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
		protected abstract void GroundedCheck();
		
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

		public virtual float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}
	}

	public interface IMove
	{
		public void Move();
	}
	public interface IJumpAndGravity
	{
		void JumpAndGravity();
	}
	public interface ICameraController
	{
		void CameraRotation();
		void ExtrudeHeadPointAndAiming();
	}
	public interface IAnimationController
	{
		void AssignAnimationIDs();
		void UpdateAnimations();
	}

	public class CameraController : ICameraController
	{
		private FPSControllerBase _fpsController;
		public CameraController(FPSControllerBase fpsController)
		{
			_fpsController = fpsController;
		}
		public void CameraRotation()
		{
			// if there is an input
			if (_fpsController._input.look.sqrMagnitude >= _fpsController._threshold)
			{
				//Don't multiply mouse input by Time.deltaTime
				float deltaTimeMultiplier = _fpsController.IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

				_fpsController.SetCinemachineTargetPitch(_fpsController._cinemachineTargetPitch + _fpsController._input.look.y * _fpsController.RotationSpeed * deltaTimeMultiplier);
				_fpsController.SetRotationVelocity(_fpsController._input.look.x * _fpsController.RotationSpeed * deltaTimeMultiplier);

				// clamp our pitch rotation
				_fpsController.SetCinemachineTargetPitch(_fpsController.ClampAngle(_fpsController._cinemachineTargetPitch, _fpsController.BottomClamp, _fpsController.TopClamp));

				// rotate the player left and right
				_fpsController.transform.Rotate(Vector3.up * _fpsController._rotationVelocity);
			}
		}

		public void ExtrudeHeadPointAndAiming()
		{
			Vector3 basePosition = new Vector3(_fpsController.transform.position.x, _fpsController.transform.position.y + 1.69f, _fpsController.transform.position.z);
			Vector3 offset = new Vector3(0, 0, 5);
			Vector3 rotation = new Vector3(_fpsController._cinemachineTargetPitch, 0, 0);

			_fpsController.aimingPoint.transform.position = basePosition + (_fpsController.transform.rotation * Quaternion.Euler(rotation)) * offset;

			_fpsController.head.transform.LookAt(_fpsController.aimingPoint.transform.position);
			_fpsController.CinemachineCameraTarget.transform.LookAt(_fpsController.aimingPoint.transform.position);
		}
	}
	public class JumpAndGravityController : IJumpAndGravity
	{
		private FPSControllerBase FPSController;
		public JumpAndGravityController(FPSControllerBase FPSController) => this.FPSController = FPSController;
		public void JumpAndGravity()
        {
            if (FPSController.Grounded)
            {
                FPSController.SetFallTimeoutDelta(FPSController.FallTimeout);
                if (FPSController._verticalVelocity < 0.0f)
                {
	                FPSController.SetVerticalVelocity(-2f);
                }
                if (FPSController._input.jump && FPSController._jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    FPSController.SetVerticalVelocity(Mathf.Sqrt(FPSController.JumpHeight * -2f * FPSController.Gravity));
                }
                if (FPSController._jumpTimeoutDelta >= 0.0f)
                {
	                FPSController.SetJumpTimeoutDelta(FPSController._jumpTimeoutDelta - Time.deltaTime);
                }
            }
            else
            {
                FPSController.SetJumpTimeoutDelta(FPSController.JumpTimeout);
                if (FPSController._fallTimeoutDelta >= 0.0f)
                {
	                FPSController.SetFallTimeoutDelta(FPSController._fallTimeoutDelta - Time.deltaTime);
                }
                FPSController._input.jump = false;
            }
            if (FPSController._verticalVelocity < FPSController._terminalVelocity)
            {
	            FPSController.SetVerticalVelocity(FPSController._verticalVelocity + (FPSController.Gravity * Time.deltaTime));
            }
        }
	}
	public class MoveController : IMove
	{
		private FPSControllerBase FPSController;
		public float targetSpeed{get; private set;} 
		public float inputMagnitude{get; private set;}
		public MoveController(FPSControllerBase FPSController)
		{
			this.FPSController = FPSController;
		}
		public void Move()
        {

            targetSpeed = FPSController._input.sprint ? FPSController.SprintSpeed : FPSController.MoveSpeed;
            if (FPSController._input.move == Vector2.zero) targetSpeed = 0.0f;
            float currentHorizontalSpeed = new Vector3(FPSController._controller.velocity.x, 0.0f, FPSController._controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            inputMagnitude = FPSController._input.analogMovement ? FPSController._input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                FPSController.SetSpeed(Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * FPSController.SpeedChangeRate));

                FPSController.SetSpeed(Mathf.Round(FPSController._speed * 1000f) / 1000f);
            }
            else
            {
	            FPSController.SetSpeed(targetSpeed);
            }
            Vector3 inputDirection = new Vector3(FPSController._input.move.x, 0.0f, FPSController._input.move.y).normalized;

            if (FPSController._input.move != Vector2.zero)
            {

                inputDirection = FPSController.transform.right * FPSController._input.move.x + FPSController.transform.forward * FPSController._input.move.y;
            }

            FPSController._controller.Move(inputDirection.normalized * (FPSController._speed * Time.deltaTime) + new Vector3(0.0f, FPSController._verticalVelocity, 0.0f) * Time.deltaTime);
        }
	}

	public class AnimationController : IAnimationController
	{
		private FPSControllerBase FPSController;
		private MoveController _moveController;
		public AnimationController(FPSControllerBase FPSController, MoveController moveController)
		{
			this.FPSController = FPSController;
			this._moveController = moveController;
		}
		public void AssignAnimationIDs()
		{
			FPSController.SetAnimIDSpeed(Animator.StringToHash("Speed"));
			FPSController.SetAnimIDGrounded(Animator.StringToHash("Grounded"));
			FPSController.SetAnimIDJump(Animator.StringToHash("Jump"));
			FPSController.SetAnimIDFreeFall(Animator.StringToHash("FreeFall"));
			FPSController.SetAnimIDMotionSpeed(Animator.StringToHash("MotionSpeed"));
			FPSController.SetDirX(Animator.StringToHash("X"));
			FPSController.SetDirZ(Animator.StringToHash("Z"));
		}
		public void UpdateAnimations()
		{
			if(!FPSController._hasAnimator)return;
			FPSController.SetAnimationBlend(Mathf.Lerp(FPSController._animationBlend, _moveController.targetSpeed, Time.deltaTime * FPSController.SpeedChangeRate));
			if (FPSController._animationBlend < 0.01f) FPSController.SetAnimationBlend(0f);

			FPSController.SetLerpSpeed(new Vector2(Mathf.Lerp(FPSController.lerpSpeed.x, 
						_moveController.targetSpeed * FPSController._input.move.x, Time.deltaTime * FPSController.SpeedChangeRate), 
					Mathf.Lerp(FPSController.lerpSpeed.y, _moveController.targetSpeed * FPSController._input.move.y, Time.deltaTime * FPSController.SpeedChangeRate)));

			FPSController._animator.SetFloat(FPSController._dirX, FPSController.lerpSpeed.x);
			FPSController._animator.SetFloat(FPSController._dirZ, FPSController.lerpSpeed.y);
			FPSController._animator.SetFloat(FPSController._animIDSpeed, FPSController._animationBlend);
			FPSController._animator.SetFloat(FPSController._animIDMotionSpeed, _moveController.inputMagnitude);
			if (FPSController.Grounded)
			{
				FPSController._animator.SetBool(FPSController._animIDJump, false);
				FPSController._animator.SetBool(FPSController._animIDFreeFall, false);
				if (FPSController._input.jump && FPSController._jumpTimeoutDelta <= 0.0f)
				{
						FPSController._animator.SetBool(FPSController._animIDJump, true);
				}
				else
				{
					if (FPSController._fallTimeoutDelta < 0.0f)
					{
						FPSController._animator.SetBool(FPSController._animIDFreeFall, true);					
					}
				}
				
			}
		}
	}
	public class FirstPersonController : FPSControllerBase
	{
		private IAnimationController _AnimationControllerController;
		private IMove _MoveController;
		private MoveController _moveController;
		private IJumpAndGravity _JumpAndGravityController;
		private ICameraController _CameraController;
        protected override void InitializeCamera()
        {
            if (_mainCamera == null)
            {
                SetMainCamera(GameObject.FindGameObjectWithTag("MainCamera"));
            }
        }

        protected override void InitializeStart()
        {
	        FPSControllerBase fpsC = GetComponent<FirstPersonController>();
	        _JumpAndGravityController = new JumpAndGravityController(fpsC);
	        _MoveController = new MoveController(fpsC);
	        _moveController = _MoveController as MoveController;
	        _AnimationControllerController = new AnimationController(fpsC, _moveController);
	        _CameraController = new CameraController(fpsC);
            SetController(GetComponent<CharacterController>());
            SetInput(GetComponent<StarterAssetsInputs>());
            SetDefultHipsPosition(headsImpact.transform.localPosition);
            Debug.Log(defultHipsPosition);
            _AnimationControllerController.AssignAnimationIDs();
#if ENABLE_INPUT_SYSTEM
            SetPlayerInput(GetComponent<PlayerInput>());
#else
            Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            // reset our timeouts on start
            SetJumpTimeoutDelta(JumpTimeout);
            SetFallTimeoutDelta(FallTimeout);
            SetHeadDefultPosition(headsImpact.transform.localPosition);
        }

        protected override void UpdateLogic()
        {
            SetHasAnimator(TryGetComponent(out _animator));
			
            GroundedCheck();
            _JumpAndGravityController.JumpAndGravity();
            _MoveController.Move();
            _AnimationControllerController.UpdateAnimations();
        }

        protected override void LateUpdateLogic()
        {
            _CameraController.ExtrudeHeadPointAndAiming();
            _CameraController.CameraRotation();
        }

        protected override void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            SetGrounded(Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore));
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }
}