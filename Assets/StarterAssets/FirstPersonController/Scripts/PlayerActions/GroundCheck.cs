using PlayerInterfases;
using UnityEngine;

namespace StarterAssets
{
    public class GroundCheck : ICheckGrounded
    {
        private FPSControllerBase _controller;
        public GroundCheck(FPSControllerBase controller)
        {
            _controller = controller;
        }
        public void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(_controller.transform.position.x, _controller.transform.position.y - _controller.GroundedOffset, _controller.transform.position.z);
            _controller.SetGrounded(Physics.CheckSphere(spherePosition, _controller.GroundedRadius, _controller.GroundLayers, QueryTriggerInteraction.Ignore));
        }
    }
}