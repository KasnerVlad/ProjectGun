using UnityEngine;

namespace StarterAssets.FirstPersonController.Scripts.PlayerActions
{
    public class AnimationController : IAnimationController
    {
        private readonly FPSControllerBase _fpsController;
        private readonly MoveController _moveController;
        private readonly JumpAndGravityController _jumpAndGravityController;
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _dirX;
        private int _dirZ;
        private float _animationBlend;
        private Vector2 _lerpSpeed;
        private bool _hasAnimator;
        private Animator _animator;
        
        public AnimationController(FPSControllerBase fpsController, MoveController moveController, JumpAndGravityController jumpAndGravityController)
        {
            this._fpsController = fpsController;
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
            
            _hasAnimator = _fpsController.TryGetComponent(out _animator);
            if(!_hasAnimator)return;
            _animationBlend = Mathf.Lerp(_animationBlend, _moveController.TargetSpeed, Time.deltaTime * _fpsController.SpeedChangeRate);
            if (_animationBlend < 0.01f)_animationBlend = 0;

            _lerpSpeed = new Vector2(Mathf.Lerp(_lerpSpeed.x, 
                    _moveController.TargetSpeed * Input.inistate.move.x, Time.deltaTime * _fpsController.SpeedChangeRate), 
                Mathf.Lerp(_lerpSpeed.y, _moveController.TargetSpeed * Input.inistate.move.y, Time.deltaTime * _fpsController.SpeedChangeRate));

            _animator.SetFloat(_dirX, _lerpSpeed.x);
            _animator.SetFloat(_dirZ, _lerpSpeed.y);
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, _moveController.InputMagnitude);
            if (_fpsController.Grounded)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
                if (Input.inistate.jump && _jumpAndGravityController.JumpTimeoutDelta <= 0.0f)
                {
                    _animator.SetBool(_animIDJump, true);
                }
                else
                {
                    if (_jumpAndGravityController.FallTimeoutDelta < 0.0f)
                    {
                        _animator.SetBool(_animIDFreeFall, true);					
                    }
                }
            }
            else if (!_animator.GetBool(_animIDFreeFall))
            {
                _animator.SetBool(_animIDFreeFall, true);
            }
            _animator.SetBool(_animIDGrounded, _fpsController.Grounded);
        }
    }
}