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
            _fpsController = fpsController;
            _jumpAndGravityController = jumpAndGravityController;
            _controller = fpsController.GetComponent<CharacterController>();
        }
        public void Move()
        {

            TargetSpeed = Input2.Sprint ? _fpsController.SprintSpeed : _fpsController.MoveSpeed;
            if (Input2.Move == Vector2.zero) TargetSpeed = 0.0f;
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            InputMagnitude = Input2.analogMovement ? Input2.Move.magnitude : 1f;

            if (currentHorizontalSpeed < TargetSpeed - speedOffset || currentHorizontalSpeed > TargetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, TargetSpeed * InputMagnitude, Time.deltaTime * _fpsController.SpeedChangeRate);

                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            { 
                _speed = TargetSpeed;
            }
            Vector3 inputDirection = new Vector3(Input2.Move.x, 0.0f, Input2.Move.y).normalized;

            if (Input2.Move != Vector2.zero)
            {

                inputDirection = _fpsController.transform.right * Input2.Move.x + _fpsController.transform.forward * Input2.Move.y;
            }

            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _jumpAndGravityController.VerticalVelocity, 0.0f) * Time.deltaTime);
        }

        public void OnCollisionStay(Collision collision) { }
    }
}