using PlayerInterfases;
using UnityEngine;

namespace StarterAssets
{
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
}