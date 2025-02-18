using PlayerInterfases;
using UnityEngine;

namespace StarterAssets
{
    public class JumpAndGravityController : IJumpAndGravity
    {
        private FPSControllerBase FPSController;
        public float _jumpTimeoutDelta{private set; get;}
        public float _fallTimeoutDelta{private set; get;}
        private readonly float _terminalVelocity = 53f;
        public float _verticalVelocity{private set; get;}
        public JumpAndGravityController(FPSControllerBase FPSController)
        {
            this.FPSController = FPSController;
            
            _jumpTimeoutDelta = this.FPSController.JumpTimeout;
            _fallTimeoutDelta = this.FPSController.FallTimeout;
        }

        public void JumpAndGravity()
        {
            if (FPSController.Grounded)
            {
                _fallTimeoutDelta = FPSController.FallTimeout;
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }
                if (FPSController._input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(FPSController.JumpHeight * -2f * FPSController.Gravity);
                }
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta = FPSController.JumpTimeout;
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                FPSController._input.jump = false;
            }
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += FPSController.Gravity * Time.deltaTime;
            }
        }
    }
}