using UnityEngine;
using CustomDelegats;
namespace StarterAssets.FirstPersonController.Scripts.PlayerActions
{
    public class RbGroundCheck : ICheckGrounded
    {
        private readonly RbMoveController _moveController;
        private readonly Vm<bool> _setGrounded;
        public RbGroundCheck(Vm<bool> setGrounded, RbMoveController moveController)
        {
            _setGrounded = setGrounded;
            _moveController = moveController;
        }
        public void GroundedCheck() => _setGrounded(_moveController.HasGroundContacts); 
    }
}