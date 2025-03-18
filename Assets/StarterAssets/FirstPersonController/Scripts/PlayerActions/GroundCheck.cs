using UnityEngine;
using CustomDelegats;
namespace StarterAssets.FirstPersonController.Scripts.PlayerActions
{
    public class GroundCheck : ICheckGrounded
    {
        private readonly FPSControllerBase _fpsController;
        private Vector3 _startPos;
        private Vector3 _endPos;
        private Vm<bool> _setGrounded;
        public GroundCheck(FPSControllerBase fpsController, Vm<bool> setGrounded)
        {
            _fpsController = fpsController;
            _setGrounded = setGrounded;
        }

        public void GroundedCheck()
        {
            _startPos = new Vector3(_fpsController.transform.position.x, _fpsController.transform.position.y - _fpsController.GroundedOffset, _fpsController.transform.position.z);
            _endPos = new Vector3(_fpsController.transform.position.x, _fpsController.transform.position.y - _fpsController.SecondGroundOffset, _fpsController.transform.position.z);
            _setGrounded(Physics.CheckCapsule(_startPos, _endPos, _fpsController.GroundedRadius, _fpsController.GroundLayers, QueryTriggerInteraction.Ignore));
        }
    }
}