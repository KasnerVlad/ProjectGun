using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.Animations.Rigging;
using UnityEngine.PlayerLoop;

namespace StarterAssets.FirstPersonController.Scripts
{
    public abstract class WeaponControllerBase : MonoBehaviour
    {
        [SerializeField] private int damage;
        [SerializeField] protected AudioClip FireSound;
        [SerializeField] protected AudioClip reloadSound;
        [SerializeField] protected GameObject inventoryPanel;
        public int Damage => damage;
        [Tooltip("Time in seconds (Write here your fire animation lenght)")]
        [SerializeField] private float fireRate;
        protected float FireRate => fireRate;
        [SerializeField] private float range;
        public float Range => range;
        [SerializeField] private Transform firePoint;
        public Transform FirePoint => firePoint;
        [SerializeField] private int startBulletCount;
        [SerializeField] protected float changeStateSpeed;
        public int StartBulletCount => startBulletCount;
        [SerializeField] private int lastBulletsCount;
        [Tooltip("Time in milliseconds (Write here your reload animation lenght)")]
        [SerializeField]private int reloadTime;        
        [SerializeField] protected GameObject AimPosRig;
        [SerializeField] protected GameObject DefultPosRig;
        private bool _isFired;
        public int ReloadTime => reloadTime;
        public bool IsFire { get; private set; }
        public bool IsReloading { get; private set; }
        public int BulletCount { get; private set; }
        public int LastBulletsCount { get; private set; }
        public bool Hide { get; private set; }
        public bool Take { get; private set; }

        [SerializeField] protected Text ammoText;
        private bool _singleFireMode;
        protected MultiParentConstraint _defaultPosConstraints;
        protected MultiParentConstraint _aimPosConstraints;
        protected WeaponView _weaponView;
        protected IWeaponAnimationsController _iAnimationsController;
        protected WeaponPlayerModelBase _pistolPlayerModel;
        protected virtual void Start()
        {
            BulletCount = StartBulletCount;
            LastBulletsCount = lastBulletsCount;
            StartLogic();
        }
        protected virtual void Update()
        {
            _=FireManager();
            UpdateLogic();
        }
        protected abstract void StartLogic();
        protected abstract void UpdateLogic();
        private void MinusBullet() { if (BulletCount > 0) { BulletCount--; } }
        public void ReloadBullets() 
        {
            if(LastBulletsCount > 0 )
            {
                int l = Math.Min(LastBulletsCount, startBulletCount-BulletCount);
                LastBulletsCount -= l;
                BulletCount += l;
            } 
        }
        private void DisableFireState() { IsFire=false; _isFired = false; }
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
            Destroy(gameObject, clip.length * (Time.timeScale < 0.009999999776482582 ? 0.01f : Time.timeScale));
            return gameObject;
        }
        public void SetReloadingState(bool isOn) { IsReloading = isOn; }
        public void SetTake(bool isOn){Take=isOn;}
        public void SetHide(bool isOn){Hide=isOn;}
        private async Task FireManager()
        {
            if (!inventoryPanel.activeSelf)
            {
                if (WeaponInput.ToggleFireMode) { _singleFireMode = !_singleFireMode; }
                if (BulletCount > 0 && (_singleFireMode? WeaponInput.SingleFire:WeaponInput.MultipleFire) && !IsReloading &&!_isFired&&!Hide)
                {
                    _isFired = true;
                    IsFire = true;
                    MinusBullet();
                    _pistolPlayerModel.Fire();
                    Invoke(nameof(DisableFireState), FireRate);
                    if(ammoText != null){ _weaponView.UpdateText();}
                }
                if (BulletCount < StartBulletCount && WeaponInput.Reload&&LastBulletsCount > 0)
                {
                    await _pistolPlayerModel.Reload();
                    if(ammoText != null){ _weaponView.UpdateText();}
                }
                if (WeaponInput.Take) { await _pistolPlayerModel.Take(); }
                if (WeaponInput.Hide) { _pistolPlayerModel.Hide(); }
            }
            if (WeaponInput.Aiming&&(_aimPosConstraints.weight <1||_defaultPosConstraints.weight >0)&&!inventoryPanel.activeSelf) {
                _aimPosConstraints.weight = Mathf.MoveTowards(_aimPosConstraints.weight, 1, Time.deltaTime*changeStateSpeed);
                _defaultPosConstraints.weight = Mathf.MoveTowards(_defaultPosConstraints.weight, 0, Time.deltaTime*changeStateSpeed);
            }
            else if(!WeaponInput.Aiming&&(_aimPosConstraints.weight >0||_defaultPosConstraints.weight <1)||inventoryPanel.activeSelf){
                _aimPosConstraints.weight = Mathf.MoveTowards(_aimPosConstraints.weight, 0, Time.deltaTime*changeStateSpeed);
                _defaultPosConstraints.weight = Mathf.MoveTowards(_defaultPosConstraints.weight, 1, Time.deltaTime*changeStateSpeed);
            } 
        }
    }
    
    public class WeaponView
    {
        private readonly WeaponControllerBase _weaponController;
        private readonly Text _ammoText;
        public WeaponView(WeaponControllerBase weaponController, Text ammoText)
        {
            _weaponController = weaponController;
            _ammoText = ammoText;
        }
        public void UpdateText()
        {
            _ammoText.text = $"Ammo       {_weaponController.BulletCount}\n" +
                             $"LastBullet {_weaponController.LastBulletsCount}";
        }
    }

    public abstract class WeaponPlayerModelBase
    {
        public abstract void Fire();
        public abstract Task Reload();
        public abstract Task Take();
        public abstract void Hide();
    }

    public class PistolPlayerModel : WeaponPlayerModelBase
    {
        private readonly WeaponControllerBase _weapon;
        public PistolPlayerModel(WeaponControllerBase weapon) { _weapon = weapon; }
        public override void Fire()
        {
            Ray ray = new Ray(_weapon.FirePoint.transform.position, _weapon.FirePoint.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray.origin, ray.direction, out hit, _weapon.Range)) { }
        }
        public override async Task Take(){if(!_weapon.Take){_weapon.SetTake(true); _weapon.SetHide(false);await Task.Delay(10); _weapon.SetTake(false);}}
        public override void Hide(){if(!_weapon.Hide){_weapon.SetHide(true); _weapon.SetTake(false);}}
        public override async Task Reload()
        {
            if (!_weapon.IsFire&&!_weapon.IsReloading && _weapon.LastBulletsCount > 0 && _weapon.BulletCount < _weapon.StartBulletCount)
            {
                _weapon.SetReloadingState(true); 
                await Task.Delay(_weapon.ReloadTime); 
                _weapon.ReloadBullets(); 
                _weapon.SetReloadingState(false);
            }
        } 
    }
    public interface IWeaponAnimationsController
    {
        void SetAnimationID();
        void UpdateAnimations();
    }
    public class WeaponAnimationsController : IWeaponAnimationsController
    {
        private readonly Animator _animator;
        private int _animationFireID;
        private int _animationReloadID;
        private int _animationTakeID;
        private int _animationHideID;
        private readonly WeaponControllerBase _weapon;
        public WeaponAnimationsController(WeaponControllerBase weapon) { _weapon = weapon; _animator = _weapon.GetComponent<Animator>(); }
        public void SetAnimationID()
        {
            _animationFireID = Animator.StringToHash("Fire");
            _animationReloadID = Animator.StringToHash("Reload");
            _animationTakeID = Animator.StringToHash("Take");
            _animationHideID = Animator.StringToHash("Hide");
        }
        public void UpdateAnimations()
        {
            if(_weapon.IsFire!=_animator.GetBool(_animationFireID)) _animator.SetBool(_animationFireID, _weapon.IsFire);
            if(_weapon.IsReloading!=_animator.GetBool(_animationReloadID))_animator.SetBool(_animationReloadID, _weapon.IsReloading);
            if(_weapon.Take!=_animator.GetBool(_animationTakeID))_animator.SetBool(_animationTakeID, _weapon.Take);
            if(_weapon.Hide!=_animator.GetBool(_animationHideID))_animator.SetBool(_animationHideID, _weapon.Hide);
        }
    }
    public static class WeaponInput
    {
        public static bool Aiming => UnityEngine.Input.GetMouseButton(1);
        public static bool ToggleFireMode => UnityEngine.Input.GetKeyDown(KeyCode.B);
        public static bool SingleFire => UnityEngine.Input.GetMouseButtonDown(0);
        public static bool MultipleFire => UnityEngine.Input.GetMouseButton(0);
        public static bool Reload => UnityEngine.Input.GetKeyDown(KeyCode.R);
        public static bool Take => UnityEngine.Input.GetKeyDown(KeyCode.T);
        public static bool Hide => UnityEngine.Input.GetKeyDown(KeyCode.H);
    }
}
