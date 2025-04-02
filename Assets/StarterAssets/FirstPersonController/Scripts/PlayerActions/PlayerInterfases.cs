using CustomDelegats;
using UnityEngine;
using Rb.Move;
namespace StarterAssets.FirstPersonController.Scripts.PlayerActions
{
    public interface IMove
    {
        void InvokeOnStay(Collision collision);
        void InvokeOnExit(Collision collision);
        void Move();
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