using UnityEngine;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Random = UnityEngine.Random;
using PlayerInterfases;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class FirstPersonController : FPSControllerBase
	{
		private IAnimationController _AnimationControllerController;
		private IMove _MoveController;
        private MoveController _moveController;
        private JumpAndGravityController jumpAndGravityController;
		private IJumpAndGravity _JumpAndGravityController;
		private ICameraController _CameraController;
		private ICheckGrounded _CheckGroundedController;
        protected override void InitializeCamera()
        {
            if (_mainCamera == null)
            {
                SetMainCamera(GameObject.FindGameObjectWithTag("MainCamera"));
            }
        }

        protected override void InitializeStart()
        {
	        _JumpAndGravityController = new JumpAndGravityController(this);
            jumpAndGravityController= _JumpAndGravityController as JumpAndGravityController;
            
	        _MoveController = new MoveController(this, jumpAndGravityController);
            _moveController = _MoveController as MoveController;
            
	        _AnimationControllerController = new AnimationController(this, _moveController, jumpAndGravityController);
	        _CameraController = new CameraController(this);
	        _CheckGroundedController = new GroundCheck(this);
            
            SetInput(GetComponent<StarterAssetsInputs>());
            SetDefultHipsPosition(headsImpact.transform.localPosition);
            Debug.Log(defultHipsPosition);
            _AnimationControllerController.AssignAnimationIDs();
#if ENABLE_INPUT_SYSTEM
            SetPlayerInput(GetComponent<PlayerInput>());
#else
            Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
            SetHeadDefultPosition(headsImpact.transform.localPosition);
        }

        protected override void UpdateLogic()
        {
			_CheckGroundedController.GroundedCheck();
            _JumpAndGravityController.JumpAndGravity();
            _MoveController.Move();
            _AnimationControllerController.UpdateAnimations();
        }

        protected override void LateUpdateLogic()
        {
            _CameraController.ExtrudeHeadPointAndAiming();
            _CameraController.CameraRotation();
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
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_moveController.controllerCenter), FootstepAudioVolume);
                }
            }
        }
        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_moveController.controllerCenter), FootstepAudioVolume);
            }
        }
    }
}