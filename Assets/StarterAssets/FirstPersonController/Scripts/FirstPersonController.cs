using UnityEngine;
using Random = UnityEngine.Random;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using StarterAssets.FirstPersonController.Scripts.PlayerActions;

namespace StarterAssets.FirstPersonController.Scripts
{
	public class FirstPersonController : FPSControllerBase
	{
		private IAnimationController _iAnimationControllerController;
		private IMove _iMoveController;
        private MoveController _moveController;
        private JumpAndGravityController _jumpAndGravityController;
		private IJumpAndGravity _iJumpAndGravityController;
		private ICameraController _iCameraController;
		private ICheckGrounded _iCheckGroundedController;
        protected override void InitializeCamera()
        {
            if (MainCamera == null)
            {
                SetMainCamera(GameObject.FindGameObjectWithTag("MainCamera"));
            }
        }

        protected override void InitializeStart()
        {
	        _iJumpAndGravityController = new JumpAndGravityController(this);
            _jumpAndGravityController= _iJumpAndGravityController as JumpAndGravityController;
            
	        _iMoveController = new MoveController(this, _jumpAndGravityController);
            _moveController = _iMoveController as MoveController;
            
	        _iAnimationControllerController = new AnimationController(this, _moveController, _jumpAndGravityController);
	        _iCameraController = new CameraController(this);
	        _iCheckGroundedController = new GroundCheck(this);
            
            SetInput(GetComponent<StarterAssetsInputs>());
            SetDefultHipsPosition(headsImpact.transform.localPosition);
            Debug.Log(DefultHipsPosition);
            _iAnimationControllerController.AssignAnimationIDs();
#if ENABLE_INPUT_SYSTEM
            SetPlayerInput(GetComponent<PlayerInput>());
#else
            Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
            SetHeadDefultPosition(headsImpact.transform.localPosition);
        }

        protected override void UpdateLogic()
        {
			_iCheckGroundedController.GroundedCheck();
            _iJumpAndGravityController.JumpAndGravity();
            _iMoveController.Move();
            _iAnimationControllerController.UpdateAnimations();
            
        }

        protected override void LateUpdateLogic()
        {
            _iCameraController.ExtrudeHeadPointAndAiming();
            _iCameraController.CameraRotation();
            _iCameraController.LookAtTarget();
        }
        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            Gizmos.color = Grounded ? transparentGreen : transparentRed;

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
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_moveController.ControllerCenter), FootstepAudioVolume);
                }
            }
        }
        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_moveController.ControllerCenter), FootstepAudioVolume);
            }
        }
    }
}