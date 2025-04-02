using UnityEngine;
using Rb.Move;
using CustomDelegats;
namespace StarterAssets.FirstPersonController.Scripts.PlayerActions
{
    public class RbMoveController : IMove
    {
private readonly FPSControllerBase _fpsController;
        public readonly IRbCharacterController _rbController;
        public readonly ISurfaceCheck _surfaceCheck;
        public event Vm<Collision> onStay;
        public event Vm<Collision> onExit;
        public bool HasGroundContacts => _surfaceCheck.HasGroundContacts;
        public float TargetSpeed { get; private set; }
        public float InputMagnitude { get; private set; }

        public RbMoveController(FPSControllerBase fpsController)
        {
            _fpsController = fpsController;
            _surfaceCheck = new SurfaceCheck(_fpsController, this);
            _rbController = new RbCharacterController(
                fpsController.GetComponent<Rigidbody>(), 
                fpsController,
            _surfaceCheck.CalculateAverageNormal
            );
        }

        public void Move()
        {
            UpdateMovementParameters(out Vector3 inputDirection, out Vector3 slopeDirection);
            Vector3 targetDirection = slopeDirection * (TargetSpeed * _fpsController.SpeedMultiplier);
            _rbController.Move(
                direction: targetDirection,
                forceMode: ForceMode.VelocityChange,
                isGrounded: _fpsController.Grounded,
                false
            );_surfaceCheck.Draw();
        }

        private void UpdateMovementParameters(out Vector3 inputDirection, out Vector3 slopeDirection)
        {
            TargetSpeed = CalculateTargetSpeed();
            inputDirection = GetRelativeInputDirection();
            slopeDirection = _rbController.ProjectDirection(inputDirection, _surfaceCheck.CalculateAverageNormal());
            Debug.DrawLine(_fpsController.transform.position, _fpsController.transform.position+slopeDirection, Color.red);
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

        public void InvokeOnStay(Collision collision)
        {
            onStay?.Invoke(collision);
        }

        public void InvokeOnExit(Collision collision)
        {
            onExit?.Invoke(collision);
        }
    }
}

