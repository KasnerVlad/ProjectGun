using PlayerInterfases;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StarterAssets
{
    public class MoveController : IMove
    {
        private FPSControllerBase FPSController;
        private JumpAndGravityController JumpAndGravityController;
        public float targetSpeed{get; private set;} 
        public float inputMagnitude{get; private set;}

        private float _speed;

        private CharacterController _controller;
        public Vector3 controllerCenter =>_controller.center;
        public MoveController(FPSControllerBase FPSController, JumpAndGravityController jumpAndGravityController)
        {
            this.FPSController = FPSController;
            this.JumpAndGravityController = jumpAndGravityController;
            _controller = FPSController.GetComponent<CharacterController>();
        }
        public void Move()
        {

            targetSpeed = FPSController._input.sprint ? FPSController.SprintSpeed : FPSController.MoveSpeed;
            if (FPSController._input.move == Vector2.zero) targetSpeed = 0.0f;
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            inputMagnitude = FPSController._input.analogMovement ? FPSController._input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * FPSController.SpeedChangeRate);

                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            { 
                _speed = targetSpeed;
            }
            Vector3 inputDirection = new Vector3(FPSController._input.move.x, 0.0f, FPSController._input.move.y).normalized;

            if (FPSController._input.move != Vector2.zero)
            {

                inputDirection = FPSController.transform.right * FPSController._input.move.x + FPSController.transform.forward * FPSController._input.move.y;
            }

            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, JumpAndGravityController._verticalVelocity, 0.0f) * Time.deltaTime);
        }
    }
}