using System;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SocialPlatforms;
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
		/*[Header("Player")]*/
		[Tooltip("Move speed of the character in m/s")]
		[SerializeField] protected float MoveSpeed;
		[Tooltip("Sprint speed of the character in m/s")]
		public float _animationBlend {private set; get;}

		[SerializeField] protected float SprintSpeed;

		[Tooltip("Rotation speed of the character")] 
		[SerializeField] protected float RotationSpeed;

		[Tooltip("Acceleration and deceleration")] 
		[SerializeField]protected float SpeedChangeRate;

		[Tooltip("Acceleration and deceleration")] 
		[SerializeField] protected AudioClip LandingAudioClip;

		[SerializeField] protected AudioClip[] FootstepAudioClips;
		/*[Range(0, 1)] */
		[SerializeField] protected float FootstepAudioVolume;

		/*[Space(10)]*/
		[Tooltip("The height the player can jump")] 
		[SerializeField] protected float JumpHeight;

		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")] 
		[SerializeField] protected float Gravity;
		/*[Space(10)]*/
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		[SerializeField] protected float JumpTimeout;

		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		[SerializeField] protected float FallTimeout;

		/*[Header("Player Grounded")]*/
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		[SerializeField] public bool Grounded {private set; get;}

		[Tooltip("Useful for rough ground")] 
		[SerializeField] protected float GroundedOffset;

		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		[SerializeField] protected float GroundedRadius;

		[Tooltip("What layers the character uses as ground")] 
		[SerializeField] protected LayerMask GroundLayers;

		/*[Header("Cinemachine")]*/
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		[SerializeField] protected GameObject CinemachineCameraTarget;

		[Tooltip("How far in degrees can you move the camera up")] 
		[SerializeField] protected float TopClamp;

		[Tooltip("How far in degrees can you move the camera down")] 
		[SerializeField] protected float BottomClamp;

		[SerializeField] protected GameObject headsImpact;
		public Vector3 defultHipsPosition{private set; get;}

		// cinemachine
		public float _cinemachineTargetPitch{private set; get;}
		public Transform[] HeadsImpactTransform{private set; get;}
		// player
		public float _speed{private set; get;}
		public float _rotationVelocity{private set; get;}
		public float _verticalVelocity{private set; get;}
		public float _terminalVelocity;

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

		[SerializeField] protected Quaternion generalHeadOffset;
	
#if ENABLE_INPUT_SYSTEM
		public PlayerInput _playerInput{private set; get;}
#endif
		public CharacterController _controller{private set; get;}
		[SerializeField] public StarterAssetsInputs _input{private set; get;}
		public GameObject _mainCamera{private set; get;}
		public const float _threshold = 0.01f;
		public Vector2 lerpSpeed{private set; get;}
		public bool _hasAnimator{private set; get;}
		protected Animator _animator;
		[SerializeField] protected GameObject aimingPoint;
		[SerializeField] protected GameObject head;
		public Vector3 _headDefultPosition{private set; get;}
		
		/*[Header("InventorySystem")]*/
		[SerializeField] protected float dragSpeed;
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
		protected abstract void AssignAnimationIDs();
		protected abstract void GroundedCheck();
		protected abstract void CameraRotation();
		protected abstract void ExtrudeHeadPointAndAiming();
		protected abstract void Move();
		protected abstract void JumpAndGravity();
		
		protected virtual void SetControllet(CharacterController characterController){_controller = characterController;}
        protected virtual void SetAnimationBlend(float animationBlend) => _animationBlend = animationBlend;
        protected virtual void SetGrounded(bool grounded) => Grounded = grounded;
        protected virtual void SetDefultHipsPosition(Vector3 defultHipsPosition) => this.defultHipsPosition = defultHipsPosition;
        protected virtual void SetCinemachineTargetPitch(float cinemachineTargetPitch) => _cinemachineTargetPitch = cinemachineTargetPitch;
        protected virtual void SetSpeed(float speed) => _speed = speed;
        protected virtual void SetRotationVelocity(float rotationVelocity) => _rotationVelocity = rotationVelocity;
        protected virtual void SetVerticalVelocity(float verticalVelocity) => _verticalVelocity = verticalVelocity;
        protected virtual void SetJumpTimeoutDelta(float jumpTimeoutDelta) => _jumpTimeoutDelta = jumpTimeoutDelta;
        protected virtual void SetFallTimeoutDelta(float fallTimeoutDelta) => _fallTimeoutDelta = fallTimeoutDelta;
        protected virtual void SetAnimIDSpeed(int animIDSpeed) => _animIDSpeed = animIDSpeed;
        protected virtual void SetAnimIDGrounded(int animIDGrounded) => _animIDGrounded = animIDGrounded;
        protected virtual void SetAnimIDJump(int animIDJump) => _animIDJump = animIDJump;
        protected virtual void SetAnimIDFreeFall(int animIDFreeFall) => _animIDFreeFall = animIDFreeFall;
        protected virtual void SetAnimIDMotionSpeed(int animIDMotionSpeed) => _animIDMotionSpeed = animIDMotionSpeed;
        protected virtual void SetDirX(int dirX) => _dirX = dirX;
        protected virtual void SetDirZ(int dirZ) => _dirZ = dirZ;
        protected virtual void SetPlayerInput(PlayerInput playerInput) => _playerInput = playerInput;
        protected virtual void SetInput(StarterAssetsInputs input) => _input = input;
        protected virtual void SetMainCamera(GameObject mainCamera) => _mainCamera = mainCamera;
        protected virtual void SetLerpSpeed(Vector2 lerpSpeed) => this.lerpSpeed = lerpSpeed;
        protected virtual void SetHasAnimator(bool hasAnimator) => _hasAnimator = hasAnimator;
        protected virtual void SetHeadDefultPosition(Vector3 headDefultPosition) => _headDefultPosition = headDefultPosition;
		protected virtual void SetController(CharacterController characterController) => _controller = characterController;
	}
	
	public class FirstPersonController : FPSControllerBase
	{
        protected override void InitializeCamera()
        {
            if (_mainCamera == null)
            {
                SetMainCamera(GameObject.FindGameObjectWithTag("MainCamera"));
            }
        }

        protected override void InitializeStart()
        {
            SetController(GetComponent<CharacterController>());
            SetInput(GetComponent<StarterAssetsInputs>());
            SetDefultHipsPosition(headsImpact.transform.localPosition);
            Debug.Log(defultHipsPosition);
            AssignAnimationIDs();
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

        protected override void AssignAnimationIDs()
        {
            SetAnimIDSpeed(Animator.StringToHash("Speed"));
            SetAnimIDGrounded(Animator.StringToHash("Grounded"));
            SetAnimIDJump(Animator.StringToHash("Jump"));
            SetAnimIDFreeFall(Animator.StringToHash("FreeFall"));
            SetAnimIDMotionSpeed(Animator.StringToHash("MotionSpeed"));
            SetDirX(Animator.StringToHash("X"));
            SetDirZ(Animator.StringToHash("Z"));
        }

        // Остальные методы остаются без изменений

        protected override void UpdateLogic()
        {
            SetHasAnimator(TryGetComponent(out _animator));
            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        protected override void LateUpdateLogic()
        {
            CameraRotation();
            ExtrudeHeadPointAndAiming();
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

        protected override void CameraRotation()
        {
            // if there is an input
            if (_input.look.sqrMagnitude >= _threshold)
            {
                //Don't multiply mouse input by Time.deltaTime
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                SetCinemachineTargetPitch(_cinemachineTargetPitch + _input.look.y * RotationSpeed * deltaTimeMultiplier);
                SetRotationVelocity(_input.look.x * RotationSpeed * deltaTimeMultiplier);

                // clamp our pitch rotation
                SetCinemachineTargetPitch(ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp));

                // rotate the player left and right
                transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        protected override void ExtrudeHeadPointAndAiming()
        {
            Vector3 basePosition = new Vector3(transform.position.x, transform.position.y + 1.69f, transform.position.z);
            Vector3 offset = new Vector3(0, 0, 5);
            Vector3 rotation = new Vector3(_cinemachineTargetPitch, 0, 0);

            aimingPoint.transform.position = basePosition + (transform.rotation * Quaternion.Euler(rotation)) * offset;

            head.transform.LookAt(aimingPoint.transform.position);
            CinemachineCameraTarget.transform.LookAt(aimingPoint.transform.position);
        }

        protected override void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                SetSpeed(Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate));

                // round speed to 3 decimal places
                SetSpeed(Mathf.Round(_speed * 1000f) / 1000f);
            }
            else
            {
                SetSpeed(targetSpeed);
            }

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving

            if (_input.move != Vector2.zero)
            {
                // move
                inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
            }

            // move the player
            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            SetAnimationBlend(Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate));
            if (_animationBlend < 0.01f) SetAnimationBlend(0f);
            if (_hasAnimator)
            {
                SetLerpSpeed(new Vector2(Mathf.Lerp(lerpSpeed.x, targetSpeed * _input.move.x, Time.deltaTime * SpeedChangeRate), Mathf.Lerp(lerpSpeed.y, targetSpeed * _input.move.y, Time.deltaTime * SpeedChangeRate)));

                _animator.SetFloat(_dirX, lerpSpeed.x);
                _animator.SetFloat(_dirZ, lerpSpeed.y);

                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        protected override void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                SetFallTimeoutDelta(FallTimeout);
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                    Debug.Log("Jumping = false");
                }
                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    SetVerticalVelocity(-2f);
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    SetVerticalVelocity(Mathf.Sqrt(JumpHeight * -2f * Gravity));
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    SetJumpTimeoutDelta(_jumpTimeoutDelta - Time.deltaTime);
                }
            }
            else
            {
                // reset the jump timeout timer
                SetJumpTimeoutDelta(JumpTimeout);

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    SetFallTimeoutDelta(_fallTimeoutDelta - Time.deltaTime);
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                SetVerticalVelocity(_verticalVelocity + (Gravity * Time.deltaTime));
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