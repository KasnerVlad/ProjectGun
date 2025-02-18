using PlayerInterfases;
using UnityEngine;

namespace StarterAssets
{
    public class MoveController : IMove
    {
        private FPSControllerBase FPSController;
        public float targetSpeed{get; private set;} 
        public float inputMagnitude{get; private set;}
        public MoveController(FPSControllerBase FPSController)
        {
            this.FPSController = FPSController;
        }
        public void Move()
        {

            targetSpeed = FPSController._input.sprint ? FPSController.SprintSpeed : FPSController.MoveSpeed;
            if (FPSController._input.move == Vector2.zero) targetSpeed = 0.0f;
            float currentHorizontalSpeed = new Vector3(FPSController._controller.velocity.x, 0.0f, FPSController._controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            inputMagnitude = FPSController._input.analogMovement ? FPSController._input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                FPSController.SetSpeed(Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * FPSController.SpeedChangeRate));

                FPSController.SetSpeed(Mathf.Round(FPSController._speed * 1000f) / 1000f);
            }
            else
            {
                FPSController.SetSpeed(targetSpeed);
            }
            Vector3 inputDirection = new Vector3(FPSController._input.move.x, 0.0f, FPSController._input.move.y).normalized;

            if (FPSController._input.move != Vector2.zero)
            {

                inputDirection = FPSController.transform.right * FPSController._input.move.x + FPSController.transform.forward * FPSController._input.move.y;
            }

            FPSController._controller.Move(inputDirection.normalized * (FPSController._speed * Time.deltaTime) + new Vector3(0.0f, FPSController._verticalVelocity, 0.0f) * Time.deltaTime);
        }
    }
}