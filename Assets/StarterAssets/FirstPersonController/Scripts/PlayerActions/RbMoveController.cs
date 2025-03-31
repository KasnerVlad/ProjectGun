using UnityEngine;
using System.Collections.Generic;
using Rb.Move;
namespace StarterAssets.FirstPersonController.Scripts.PlayerActions
{
    public class RbMoveController : IMove
    {
        private readonly FPSControllerBase _fpsController;
        public readonly IRbCharacterController _rbCharacterController; 
        private readonly ISurfaceCheck _surfaceCheck;
        public bool HasGroundContacts => _surfaceCheck.HasGroundContacts;
        public float TargetSpeed { get; private set; }
        public float InputMagnitude { get; private set; }
    
        public RbMoveController(FPSControllerBase fpsController)
        {
            _fpsController = fpsController;

            _surfaceCheck = new SurfaceCheck(_fpsController);
            _rbCharacterController = new RbCharacterController(_fpsController.GetComponent<Rigidbody>(), fpsController);
        }
        public void Move()
        {
            TargetSpeed = Input2.Sprint ? _fpsController.SprintSpeed : _fpsController.MoveSpeed;
            if (Input2.Move == Vector2.zero) TargetSpeed = 0.0f;
    
            Vector3 inputDirection = new Vector3(Input2.Move.x, 0, Input2.Move.y).normalized;
            InputMagnitude = Input2.analogMovement ? Input2.Move.magnitude : 1f;
    
            if (_fpsController.Grounded)
            {
                Vector3 averageNormal = _surfaceCheck.CalculateAverageNormal();
                Vector3 slopeDirection = _rbCharacterController.ProjectDirection(inputDirection, averageNormal);
                _rbCharacterController.Move(slopeDirection * TargetSpeed,ForceMode.VelocityChange);
            }
            else
            {
                _rbCharacterController.Move(inputDirection * _fpsController.AirControlForce, ForceMode.Acceleration);
            }
    
            _surfaceCheck.ClearContactNormals();
        }
        public void OnCollisionStay(Collision collision) { _surfaceCheck.OnCollisionStay(collision); }    
    }
}

