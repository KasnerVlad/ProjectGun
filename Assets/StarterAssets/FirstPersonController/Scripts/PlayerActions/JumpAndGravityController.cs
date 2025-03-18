using UnityEngine;

namespace StarterAssets.FirstPersonController.Scripts.PlayerActions
{
    public class JumpAndGravityController : IJumpAndGravity
    {
        private readonly FPSControllerBase _fpsController;
        public float JumpTimeoutDelta{private set; get;}
        public float FallTimeoutDelta{private set; get;}
        private readonly float _terminalVelocity = 53f;
        public float VerticalVelocity{private set; get;}
        public JumpAndGravityController(FPSControllerBase fpsController)
        {
            _fpsController = fpsController;
            
            JumpTimeoutDelta = _fpsController.JumpTimeout;
            FallTimeoutDelta = _fpsController.FallTimeout;
        }

        public void JumpAndGravity()
        {
            if (_fpsController.Grounded)
            {
                FallTimeoutDelta = _fpsController.FallTimeout;
                if (VerticalVelocity < 0.0f)
                {
                    VerticalVelocity = -2f;
                }
                if (Input.inistate.jump && JumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    VerticalVelocity = Mathf.Sqrt(_fpsController.JumpHeight * -2f * _fpsController.Gravity);
                }
                if (JumpTimeoutDelta >= 0.0f)
                {
                    JumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                JumpTimeoutDelta = _fpsController.JumpTimeout;
                if (FallTimeoutDelta >= 0.0f)
                {
                    FallTimeoutDelta -= Time.deltaTime;
                }
            }
            if (VerticalVelocity < _terminalVelocity)
            {
                VerticalVelocity += _fpsController.Gravity * Time.deltaTime;
            }
        }
    }
}