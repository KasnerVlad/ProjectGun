using UnityEngine;
using System.Threading.Tasks;

namespace StarterAssets.FirstPersonController.Scripts
{
    public class WeaponController : WeaponControllerBase
    {
        private WeaponView _weaponView;
        private IWeaponAnimationsController _iAnimationsController;
        private WeaponPlayerModel _weaponPlayerModel;
        private Animator _animator;
        private bool _isFired;
        private bool _singleFireMode;
        protected override void StartLogic()
        {
            _weaponView = new WeaponView(this,ammoText);
            _iAnimationsController = new WeaponAnimationsController(this);
            _weaponPlayerModel = new WeaponPlayerModel(this);
            _animator = GetComponent<Animator>();
            _iAnimationsController.SetAnimationID();
            if(ammoText != null){ _weaponView.UpdateText();}
        }
        protected override void UpdateLogic()
        {
            _iAnimationsController.UpdateAnimations();
        }
        protected override async Task FireManager()
        {
            if (WeaponInput.ToggleFireMode)
            {
                _singleFireMode = !_singleFireMode;
            }
            if (BulletCount > 0 && (_singleFireMode? WeaponInput.SingleFire:WeaponInput.MultipleFire) && !IsReloading &&!_isFired&&!Hide)
            {
                _isFired = true;
                SetFireState(true);
                MinusBullet();
                _weaponPlayerModel.Fire();
                Invoke(nameof(DisableFireState), FireRate);
                if(ammoText != null){ _weaponView.UpdateText();}
            }
            if (BulletCount < StartBulletCount && WeaponInput.Reload&&LastBulletsCount > 0)
            {
                await _weaponPlayerModel.Reload();
                if(ammoText != null){ _weaponView.UpdateText();}
            }
            if (WeaponInput.Take) { await _weaponPlayerModel.Take(); }
            if (WeaponInput.Hide) { _weaponPlayerModel.Hide(); }
        }
        private void DisableFireState()
        {
            SetFireState(false);
            _isFired = false;
        }

        private void OnFire(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(FireSound, transform.TransformPoint(characterController.center+new Vector3(0,1.69f,0)), 1f);
            }
        }
    }
}
