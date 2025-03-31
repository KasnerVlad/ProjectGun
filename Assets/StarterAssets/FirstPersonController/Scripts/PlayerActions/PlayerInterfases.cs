using UnityEngine;
namespace StarterAssets.FirstPersonController.Scripts.PlayerActions
{
    public interface IMove
    {
        void Move();
        void OnCollisionStay(Collision collision);
    }
    public interface IJumpAndGravity
    {
        void JumpAndGravity();
    }
    public interface ICameraController
    {
        void CameraRotation();
        void ExtrudeHeadPointAndAiming();
        void LookAtTarget();
    }
    public interface IAnimationController
    {
        void AssignAnimationIDs();
        void UpdateAnimations();
    }
        
    public interface ICheckGrounded
    {
        void GroundedCheck();
    }
}