using UnityEngine;
namespace StarterAssets.FirstPersonController.Scripts.PlayerActions
{
    public class RbJumpAndGravityController : IJumpAndGravity
    {
        private readonly FPSControllerBase _fpsController;
        private readonly RbMoveController _moveController;
        public float JumpTimeoutDelta{private set; get;}
        public float FallTimeoutDelta{private set; get;}
        private readonly float _terminalVelocity = 53f;
        public float VerticalVelocity{private set; get;}
        public RbJumpAndGravityController(FPSControllerBase fpsController, RbMoveController moveController)
        {
            _fpsController = fpsController;
            _moveController = moveController;
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
                if (Input2.Jump && JumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    VerticalVelocity = Mathf.Sqrt(_fpsController.JumpHeight * -2f * _fpsController.Gravity); 
                    _moveController._rbController.Move(Vector3.up * (VerticalVelocity*_fpsController.JumpForce), 
                        ForceMode.VelocityChange, _fpsController.Grounded);
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