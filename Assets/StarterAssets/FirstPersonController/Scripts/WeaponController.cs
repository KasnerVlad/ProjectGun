using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Animations.Rigging;

namespace StarterAssets.FirstPersonController.Scripts
{
    public class WeaponController : WeaponControllerBase
    {
        private WeaponView _weaponView;
        private IWeaponAnimationsController _iAnimationsController;
        private WeaponPlayerModel _weaponPlayerModel;
        private bool _isFired;
        private bool _singleFireMode;
        private MultiParentConstraint _defaultPosConstraints;
        private MultiParentConstraint _aimPosConstraints;
        protected override void StartLogic()
        {
            _weaponView = new WeaponView(this,ammoText);
            _iAnimationsController = new WeaponAnimationsController(this);
            _weaponPlayerModel = new WeaponPlayerModel(this);
            _iAnimationsController.SetAnimationID();
            if(ammoText != null){ _weaponView.UpdateText();}
            _aimPosConstraints = AimPosRig.GetComponent<MultiParentConstraint>();
            _defaultPosConstraints = DefultPosRig.GetComponent<MultiParentConstraint>();
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

            if (WeaponInput.Aiming&&(_aimPosConstraints.weight <1||_defaultPosConstraints.weight >0)) {
                _aimPosConstraints.weight = Mathf.MoveTowards(_aimPosConstraints.weight, 1, Time.deltaTime*changeStateSpeed);
                _defaultPosConstraints.weight = Mathf.MoveTowards(_defaultPosConstraints.weight, 0, Time.deltaTime*changeStateSpeed);
                Debug.Log("Aiming");
            }
            else if(!WeaponInput.Aiming&&(_aimPosConstraints.weight >0||_defaultPosConstraints.weight <1)){
                _aimPosConstraints.weight = Mathf.MoveTowards(_aimPosConstraints.weight, 0, Time.deltaTime*changeStateSpeed);
                _defaultPosConstraints.weight = Mathf.MoveTowards(_defaultPosConstraints.weight, 1, Time.deltaTime*changeStateSpeed);
                Debug.Log("NotAiming");
            }
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
                GameObject g = PlayAudioAtPointWithReturn(FireSound, transform.TransformPoint(Vector3.zero), 1f);
                g.transform.SetParent(transform);
            }
        }

        private static GameObject PlayAudioAtPointWithReturn(AudioClip clip, Vector3 position, [UnityEngine.Internal.DefaultValue("1.0F")] float volume)
        {
            GameObject gameObject = new GameObject("One shot audio");
            gameObject.transform.position = position;
            AudioSource audioSource = (AudioSource) gameObject.AddComponent(typeof (AudioSource));
            audioSource.clip = clip;
            audioSource.spatialBlend = 1f;
            audioSource.volume = volume;
            audioSource.Play();
            Object.Destroy((Object) gameObject, clip.length * ((double) Time.timeScale < 0.009999999776482582 ? 0.01f : Time.timeScale));
            return gameObject;
        }
    }
}
