using System;
using UnityEngine;
using Random = UnityEngine.Random;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using StarterAssets.FirstPersonController.Scripts.PlayerActions;
using StarterAssets.FirstPersonController.Scripts.PlayerHpSystem;
namespace StarterAssets.FirstPersonController.Scripts
{
	public class FirstPersonController : FPSControllerBase
	{
		private IAnimationController _iAnimationControllerController;
		private IMove _iMoveController;
		private IJumpAndGravity _iJumpAndGravityController;
		private ICameraController _iCameraController;
		private ICheckGrounded _iCheckGroundedController;
        private PlayerHpModel _playerHpModel;
        private PlayerHpView _playerHpView;
        protected override void InitializeStart()
        {
	        _iJumpAndGravityController = new JumpAndGravityController(this);
	        _iMoveController = new MoveController(this, _iJumpAndGravityController as JumpAndGravityController);
	        _iAnimationControllerController = new AnimationController(this, _iMoveController as MoveController, _iJumpAndGravityController as JumpAndGravityController);
	        _iCameraController = new CameraController(this);
	        _iCheckGroundedController = new GroundCheck(this, setGrounded:SetGrounded);

            _playerHpModel = new PlayerHpModel();
            _playerHpView = new PlayerHpView(hpScrollbar);
            _playerHpView.UpdateHp(_playerHpModel.CurrentHealth, _playerHpModel.maxHp);
            _iAnimationControllerController.AssignAnimationIDs();
            SetPlayerInput(Input.inistate.gameObject.GetComponent<PlayerInput>());
        }
        public void TakeDamage(int damage){_playerHpModel.TakeDamage(damage); _playerHpView.UpdateHp(_playerHpModel.CurrentHealth, _playerHpModel.maxHp);}
        private void OnDisable() => SaveManager._GameSaveManager.OnSave();
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
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - SecondGroundOffset, transform.position.z), GroundedRadius);
        }
        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint((_iMoveController as MoveController).ControllerCenter), FootstepAudioVolume);
                }
            }
        }
        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint((_iMoveController as MoveController).ControllerCenter), FootstepAudioVolume);
            }
        }
    }
}