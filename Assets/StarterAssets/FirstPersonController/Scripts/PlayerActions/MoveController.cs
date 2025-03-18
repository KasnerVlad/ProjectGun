using UnityEngine;

namespace StarterAssets.FirstPersonController.Scripts.PlayerActions
{
    public class MoveController : IMove
    {
        private readonly FPSControllerBase _fpsController;
        private readonly JumpAndGravityController _jumpAndGravityController;
        public float TargetSpeed{get; private set;} 
        public float InputMagnitude{get; private set;}

        private float _speed;

        private readonly CharacterController _controller;
        public Vector3 ControllerCenter =>_controller.center;
        public MoveController(FPSControllerBase fpsController, JumpAndGravityController jumpAndGravityController)
        {
            this._fpsController = fpsController;
            this._jumpAndGravityController = jumpAndGravityController;
            _controller = fpsController.GetComponent<CharacterController>();
        }
        public void Move()
        {

            TargetSpeed = Input.inistate.sprint ? _fpsController.SprintSpeed : _fpsController.MoveSpeed;
            if (Input.inistate.move == Vector2.zero) TargetSpeed = 0.0f;
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            InputMagnitude = Input.inistate.analogMovement ? Input.inistate.move.magnitude : 1f;

            if (currentHorizontalSpeed < TargetSpeed - speedOffset || currentHorizontalSpeed > TargetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, TargetSpeed * InputMagnitude, Time.deltaTime * _fpsController.SpeedChangeRate);

                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            { 
                _speed = TargetSpeed;
            }
            Vector3 inputDirection = new Vector3(Input.inistate.move.x, 0.0f, Input.inistate.move.y).normalized;

            if (Input.inistate.move != Vector2.zero)
            {

                inputDirection = _fpsController.transform.right * Input.inistate.move.x + _fpsController.transform.forward * Input.inistate.move.y;
            }

            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _jumpAndGravityController.VerticalVelocity, 0.0f) * Time.deltaTime);
        }
    }
}