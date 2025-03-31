using UnityEngine;
using System.Collections.Generic;
using Rb.Move;
namespace StarterAssets.FirstPersonController.Scripts.PlayerActions
{
    public class RbMoveController : IMove
    {
private readonly FPSControllerBase _fpsController;
        public readonly IRbCharacterController _rbController;
        private readonly ISurfaceCheck _surfaceCheck;

        public bool HasGroundContacts => _surfaceCheck.HasGroundContacts;
        public float TargetSpeed { get; private set; }
        public float InputMagnitude { get; private set; }

        public RbMoveController(FPSControllerBase fpsController)
        {
            _fpsController = fpsController;
            _surfaceCheck = new SurfaceCheck(_fpsController);
            _rbController = new RbCharacterController(
                fpsController.GetComponent<Rigidbody>(), 
                fpsController
            );
        }

        public void Move()
        {
            _surfaceCheck.UpdateContacts();
            UpdateMovementParameters(out Vector3 inputDirection, out Vector3 slopeDirection);
            Vector3 targetDirection = slopeDirection * (TargetSpeed * _fpsController.SpeedMultiplier);
            _rbController.Move(
                direction: targetDirection,
                forceMode: ForceMode.VelocityChange,
                isGrounded: _fpsController.Grounded
            );
        }

        private void UpdateMovementParameters(out Vector3 inputDirection, out Vector3 slopeDirection)
        {
            TargetSpeed = CalculateTargetSpeed();
            inputDirection = GetRelativeInputDirection();
            slopeDirection = _rbController.ProjectDirection(inputDirection, _surfaceCheck.CalculateAverageNormal());
            InputMagnitude = Input2.analogMovement ? Input2.Move.magnitude : 1f;
        }

        private float CalculateTargetSpeed()
        {
            if (Input2.Move == Vector2.zero) return 0f;
            return Input2.Sprint ? _fpsController.SprintSpeed : _fpsController.MoveSpeed;
        }

        private Vector3 GetRelativeInputDirection()
        {
            return _fpsController.transform.TransformDirection(
                new Vector3(Input2.Move.x, 0, Input2.Move.y)
            ).normalized;
        }

        public void OnCollisionStay(Collision collision) => 
            _surfaceCheck.OnCollisionStay(collision);  
    }
}

