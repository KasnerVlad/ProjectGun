using PlayerInterfases;
using UnityEngine;

namespace StarterAssets
{
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
            FPSController._animator.SetBool(FPSController._animIDGrounded, FPSController.Grounded);
        }
    }
}