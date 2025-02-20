namespace StarterAssets.FirstPersonController.Scripts.PlayerActions
{
    public interface IMove
    {
        public void Move();
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