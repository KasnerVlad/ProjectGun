using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

namespace StarterAssets.FirstPersonController.Scripts
{
    public abstract class WeaponControllerBase : MonoBehaviour
    {
        [SerializeField] private int damage;
        [SerializeField] protected AudioClip FireSound;
        public int Damage => damage;
        [Tooltip("Time in seconds (Write here your fire animation lenght)")]
        [SerializeField] private float fireRate;
        protected float FireRate => fireRate;
        [SerializeField] private float range;
        public float Range => range;
        [SerializeField] private GameObject muzzleFlash;
        public GameObject MuzzleFlash => muzzleFlash;
        [SerializeField] private Transform firePoint;
        public Transform FirePoint => firePoint;
        [SerializeField] private int startBulletCount;
        public int StartBulletCount => startBulletCount;
        [SerializeField] private int lastBulletsCount;
        [Tooltip("Time in milliseconds (Write here your reload animation lenght)")]
        [SerializeField]private int reloadTime;
        public int ReloadTime => reloadTime;
        public bool IsFire { get; private set; }
        public bool IsReloading { get; private set; }
        public int BulletCount { get; private set; }
        public int LastBulletsCount { get; private set; }
        public bool Hide { get; private set; }
        public bool Take { get; private set; }
        [SerializeField] protected Text ammoText;
        protected virtual void Start()
        {
            BulletCount = StartBulletCount;
            LastBulletsCount = lastBulletsCount;
            StartLogic();
        }
        protected virtual void Update()
        {
            FireManager();
            UpdateLogic();
        }
        protected abstract Task FireManager();
        protected abstract void StartLogic();
        protected abstract void UpdateLogic();
        protected virtual void MinusBullet()
        {
            if (BulletCount > 0)
            {
                BulletCount--;
            }
        }
        public virtual void ReloadBullets() 
        {
            if(LastBulletsCount > 0 )
            {
                int l = Math.Min(LastBulletsCount, startBulletCount-BulletCount);
                LastBulletsCount -= l;
                BulletCount += l;
            } 
        }
        protected virtual void SetFireState(bool isOn) { IsFire = isOn; }
        public virtual void SetReloadingState(bool isOn) { IsReloading = isOn; }
        public virtual void SetTake(bool isOn){Take=isOn;}
        public virtual void SetHide(bool isOn){Hide=isOn;}
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
    public class WeaponPlayerModel
    {
        private readonly WeaponControllerBase _weapon;
        public WeaponPlayerModel(WeaponControllerBase weapon) { _weapon = weapon; }
        public virtual void Fire()
        {
            Ray ray = new Ray(_weapon.FirePoint.transform.position, _weapon.FirePoint.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray.origin, ray.direction, out hit, _weapon.Range)) { }
        }
        public async Task Take(){if(!_weapon.Take){_weapon.SetTake(true); _weapon.SetHide(false);await Task.Delay(10); _weapon.SetTake(false);}}
        public void Hide(){if(!_weapon.Hide){_weapon.SetHide(true); _weapon.SetTake(false);}}
    
        public async Task Reload()
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
        public static bool ToggleFireMode => Input.GetKeyDown(KeyCode.B);
        public static bool SingleFire => Input.GetMouseButtonDown(0);
        public static bool MultipleFire => Input.GetMouseButton(0);
        public static bool Reload => Input.GetKeyDown(KeyCode.R);
        public static bool Take => Input.GetKeyDown(KeyCode.T);
        public static bool Hide => Input.GetKeyDown(KeyCode.H);
    }
}
