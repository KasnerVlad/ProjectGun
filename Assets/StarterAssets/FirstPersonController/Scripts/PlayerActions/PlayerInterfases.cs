namespace PlayerInterfases
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