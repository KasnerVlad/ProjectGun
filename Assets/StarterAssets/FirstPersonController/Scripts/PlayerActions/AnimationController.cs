using PlayerInterfases;
using UnityEngine;

namespace StarterAssets
{
    public class AnimationController : IAnimationController
    {
        private FPSControllerBase FPSController;
        private MoveController _moveController;
        private JumpAndGravityController _jumpAndGravityController;
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _dirX;
        private int _dirZ;
        private float _animationBlend;
        private Vector2 lerpSpeed;
        private bool _hasAnimator;
        private Animator _animator;
        
        public AnimationController(FPSControllerBase FPSController, MoveController moveController, JumpAndGravityController jumpAndGravityController)
        {
            this.FPSController = FPSController;
            this._moveController = moveController;
            this._jumpAndGravityController = jumpAndGravityController;
        }
        public void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _dirX = Animator.StringToHash("X");
            _dirZ = Animator.StringToHash("Z");
        }
        public void UpdateAnimations()
        {
            
            _hasAnimator = FPSController.TryGetComponent(out _animator);
            if(!_hasAnimator)return;
            _animationBlend = Mathf.Lerp(_animationBlend, _moveController.targetSpeed, Time.deltaTime * FPSController.SpeedChangeRate);
            if (_animationBlend < 0.01f)_animationBlend = 0;

            lerpSpeed = new Vector2(Mathf.Lerp(lerpSpeed.x, 
                    _moveController.targetSpeed * FPSController._input.move.x, Time.deltaTime * FPSController.SpeedChangeRate), 
                Mathf.Lerp(lerpSpeed.y, _moveController.targetSpeed * FPSController._input.move.y, Time.deltaTime * FPSController.SpeedChangeRate));

            _animator.SetFloat(_dirX, lerpSpeed.x);
            _animator.SetFloat(_dirZ, lerpSpeed.y);
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, _moveController.inputMagnitude);
            if (FPSController.Grounded)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
                if (FPSController._input.jump && _jumpAndGravityController._jumpTimeoutDelta <= 0.0f)
                {
                    _animator.SetBool(_animIDJump, true);
                }
                else
                {
                    if (_jumpAndGravityController._fallTimeoutDelta < 0.0f)
                    {
                        _animator.SetBool(_animIDFreeFall, true);					
                    }
                }
            }
            _animator.SetBool(_animIDGrounded, FPSController.Grounded);
        }
    }
}