using StarterAssets.FirstPersonController.Scripts;
using UnityEngine;
namespace Rb.Move
{
    public class RbCharacterController : IRbCharacterController
    {
        private readonly Rigidbody _rb;
        private readonly float _groundFriction;
        private readonly float _maxSpeed;

        public RbCharacterController(Rigidbody rb, FPSControllerBase fpsController)
        {
            _rb = rb;
            _groundFriction = fpsController.GroundFriction;
            _maxSpeed = fpsController.SprintSpeed;
            SetupRigidbody();
        }

        private void SetupRigidbody()
        {
            _rb.freezeRotation = true;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        public void Move(Vector3 direction, ForceMode forceMode, bool isGrounded)
        {
            ApplyMovement(direction, forceMode);
            if (isGrounded) ApplyFriction();
            ClampHorizontalSpeed();
        }

        private void ApplyMovement(Vector3 direction, ForceMode forceMode) => 
            _rb.AddForce(direction, forceMode);

        private void ApplyFriction()
        {
            Vector3 lateralVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
            _rb.AddForce(-lateralVelocity * _groundFriction, ForceMode.VelocityChange);
        }

        private void ClampHorizontalSpeed()
        {
            Vector3 horizontalVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
            if (horizontalVelocity.magnitude > _maxSpeed)
            {
                horizontalVelocity = horizontalVelocity.normalized * _maxSpeed;
                _rb.velocity = new Vector3(horizontalVelocity.x, _rb.velocity.y, horizontalVelocity.z);
            }
        }

        public Vector3 ProjectDirection(Vector3 direction, Vector3 normal) => 
            Vector3.ProjectOnPlane(direction, normal).normalized;
    }
    public interface IRbCharacterController
    {
        void Move(Vector3 direction, ForceMode forceMode, bool isGrounded);
        Vector3 ProjectDirection(Vector3 direction, Vector3 normal);
    }
}