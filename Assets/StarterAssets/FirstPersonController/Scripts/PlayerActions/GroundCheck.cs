using UnityEngine;

namespace StarterAssets.FirstPersonController.Scripts.PlayerActions
{
    public class GroundCheck : ICheckGrounded
    {
        private readonly FPSControllerBase _fpsController;
        private Vector3 startPos;
        Vector3 endPos;

        public GroundCheck(FPSControllerBase fpsController)
        {
            _fpsController = fpsController;
        }

        public void GroundedCheck()
        {
            startPos = new Vector3(_fpsController.transform.position.x, _fpsController.transform.position.y - _fpsController.GroundedOffset, _fpsController.transform.position.z);
            endPos = new Vector3(_fpsController.transform.position.x, _fpsController.transform.position.y - _fpsController.SecondGroundOffset, _fpsController.transform.position.z);
            _fpsController.SetGrounded(Physics.CheckCapsule(startPos, endPos, _fpsController.GroundedRadius, _fpsController.GroundLayers, QueryTriggerInteraction.Ignore));
        }
    }
}