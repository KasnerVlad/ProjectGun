using StarterAssets.FirstPersonController.Scripts;
using UnityEngine;
namespace Rb.Move
{
    public class RbCharacterController : IRbCharacterController
    {
        private readonly Rigidbody _rb;
        private readonly FPSControllerBase _fpsController;
        public RbCharacterController(Rigidbody rb, FPSControllerBase fpsController)
        {
            _rb = rb;
            _fpsController = fpsController;
            SetupRigidbody();
        }
        private void SetupRigidbody()
        {
            _rb.freezeRotation = true;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
        private void ApplyGroundFriction()
        {
            Vector3 lateralVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
            _rb.AddForce(-lateralVelocity * _fpsController.GroundFriction, ForceMode.VelocityChange);
        }
        public Vector3 ProjectDirection(Vector3 direction, Vector3 normal)
        {
            return Vector3.ProjectOnPlane(direction, normal).normalized;
        }
        public void Move(Vector3 dir, ForceMode forceMode)
        {
            _rb.AddForce(dir , ForceMode.VelocityChange);
            ApplyGroundFriction();
        }
    }
    public interface IRbCharacterController
    {
        void Move(Vector3 dir, ForceMode forceMode);
        Vector3 ProjectDirection(Vector3 direction, Vector3 normal);
    }
}