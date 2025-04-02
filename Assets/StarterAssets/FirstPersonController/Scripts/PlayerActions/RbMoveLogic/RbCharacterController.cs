using StarterAssets.FirstPersonController.Scripts;
using UnityEngine;
using CustomDelegats;
namespace Rb.Move
{
    public class RbCharacterController : IRbCharacterController
    {
        private readonly Rigidbody _rb;
        private readonly float _groundFriction;
        private readonly float _maxSpeed;
        private readonly FPSControllerBase _controller;
        private Rm<Vector3> _avarageNormal;
        public RbCharacterController(Rigidbody rb, FPSControllerBase fpsController, Rm<Vector3> avarageNormal)
        {
            _rb = rb;
            _groundFriction = fpsController.GroundFriction;
            _maxSpeed = fpsController.SprintSpeed;
            _controller = fpsController;
            _avarageNormal = avarageNormal;
            SetupRigidbody();
        }

        private void SetupRigidbody()
        {
            _rb.freezeRotation = true;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        public void Move(Vector3 direction, ForceMode forceMode, bool isGrounded, bool IsJumping)
        {
            
            if(!isGrounded)
            {
                // Ограничиваем силу в воздухе
                direction *= _controller.AirControlForce;
                ApplyMovement(direction, forceMode, IsJumping);
                ClampAirSpeed();
                return;
            }
            ApplyMovement(direction, forceMode, IsJumping);
            if (isGrounded) ApplyFriction();
            ClampHorizontalSpeed();
        }

        private void ApplyMovement(Vector3 direction, ForceMode forceMode,bool isJumping) {
            if (IsValidSlope(_avarageNormal.Invoke()))
            {
                _rb.AddForce(direction, forceMode);
            }
        }

        private void ApplyFriction() {
            Vector3 lateralVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
            _rb.AddForce(-lateralVelocity * _groundFriction, ForceMode.VelocityChange);
        }
        private bool IsValidSlope(Vector3 normal) => 
            Vector3.Angle(normal, Vector3.up) <= _controller.MaxSlopeAngle;
        private void ClampAirSpeed()
        {
            Vector3 horizontalVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
            if (horizontalVelocity.magnitude > _controller.AirControlForce)
            {
                horizontalVelocity = horizontalVelocity.normalized * _controller.AirControlForce;
                _rb.velocity = new Vector3(horizontalVelocity.x, _rb.velocity.y, horizontalVelocity.z);
            }
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

        public Vector3 ProjectDirection(Vector3 direction, Vector3 normal)
        {    // Дополнительная стабилизация вертикальной составляющей
            Vector3 projected = Vector3.ProjectOnPlane(direction, normal);
    
            // Усиленное ограничение вертикальной составляющей
            return new Vector3(
                projected.x,
                0,
                projected.z
            ).normalized;
        }
    }
    public interface IRbCharacterController
    {
        void Move(Vector3 direction, ForceMode forceMode, bool isGrounded, bool IsJumping);
        Vector3 ProjectDirection(Vector3 direction, Vector3 normal);
    }
}