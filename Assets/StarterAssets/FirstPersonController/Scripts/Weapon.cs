using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using StarterAssets;
using UnityEngine.Serialization;

public abstract class WeaponControllerBase : MonoBehaviour
{
    [SerializeField] private int _damage;
    public int damage => _damage;
    [Tooltip("Time in miliseconds")] [SerializeField]
    private float _fireRate;
    protected float FireRate => _fireRate;
    [SerializeField] private float _range;
    public float range => _range;
    [SerializeField] private GameObject _muzzleFlash;
    public GameObject muzzleFlash => _muzzleFlash;
    [SerializeField] private Transform _firePoint;
    public Transform firePoint => _firePoint;
    [SerializeField] private int _startBulletCount;
    public int startBulletCount => _startBulletCount;
    [SerializeField] private int _lastBulletsCount;
    [SerializeField] private int _reloadTime;
    public int reloadTime => _reloadTime;
    public bool isFire { get; private set; }
    public bool isReloading { get; private set; }
    public int BulletCount { get; private set; }
    public int LastBulletsCount { get; private set; }
    public bool Hide { get; private set; }
    public bool Take { get; private set; }
    [SerializeField] protected Text ammoText;
    protected virtual void Start()
    {
        BulletCount = startBulletCount;
        LastBulletsCount = _lastBulletsCount;
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
            int l = Math.Min(LastBulletsCount, _startBulletCount-BulletCount);
            LastBulletsCount -= l;
            BulletCount += l;
        } 
    }
    protected virtual void SetFireState(bool isOn) { isFire = isOn; }
    public virtual void SetReloadingState(bool isOn) { isReloading = isOn; }
    public virtual void SetTake(bool isOn){Take=isOn;}
    public virtual void SetHide(bool isOn){Hide=isOn;}
}

public class WeaponView
{
    private readonly int _bulletCount;
    private readonly int _magCount;
    private readonly Text _ammoText;
    public WeaponView(int bulletCount, int magCount, Text ammoText)
    {
        _bulletCount = bulletCount;
        _magCount = magCount;
        _ammoText = ammoText;
    }
    public void UpdateText()
    {
        _ammoText.text = $"Ammo       {_bulletCount}" +
                         $"LastBullet {_magCount}";
    }
}
public class WeaponPlayerModel
{
    private readonly WeaponControllerBase _weapon;
    public WeaponPlayerModel(WeaponControllerBase weapon) { _weapon = weapon; }
    public virtual void Fire()
    {
        Ray ray = new Ray(_weapon.firePoint.transform.position, _weapon.firePoint.transform.forward);
        RaycastHit _hit;
        if (Physics.Raycast(ray.origin, ray.direction, out _hit, _weapon.range)) { }
    }
    public void Take(){if(!_weapon.Take){_weapon.SetTake(true); _weapon.SetHide(false);}}
    public void Hide(){if(!_weapon.Hide){_weapon.SetHide(true); _weapon.SetTake(false);}}

    public async Task Reload()
    {
        if (!_weapon.isFire && _weapon.LastBulletsCount > 0 && _weapon.BulletCount < _weapon.startBulletCount)
        {
            _weapon.SetReloadingState(true); 
            await Task.Delay(_weapon.reloadTime); 
            _weapon.ReloadBullets(); 
            _weapon.SetReloadingState(false);
        }
    } 
}
public interface IWeaponAmimationsController
{
    void SetAnimationID();
    void UpdateAnimations();
}
public class WeaponAmimationsController : IWeaponAmimationsController
{
    private readonly Animator _animator;
    private int _animationFireID;
    private int _animationReloadID;
    private int _animationTakeID;
    private int _animationHideID;
    private readonly WeaponControllerBase _weapon;
    public WeaponAmimationsController(WeaponControllerBase weapon) { _weapon = weapon; _animator = _weapon.GetComponent<Animator>(); }
    public void SetAnimationID()
    {
        _animationFireID = Animator.StringToHash("Fire");
        _animationReloadID = Animator.StringToHash("Reload");
        _animationTakeID = Animator.StringToHash("Take");
        _animationHideID = Animator.StringToHash("Hide");
    }
    public void UpdateAnimations()
    {
        _animator.SetBool(_animationFireID, _weapon.isFire);
        _animator.SetBool(_animationReloadID, _weapon.isReloading);
        _animator.SetBool(_animationTakeID, _weapon.Take);
        _animator.SetBool(_animationHideID, _weapon.Hide);
    }
}
public static class WeaponInput
{
    public static bool Fire => Input.GetMouseButtonDown(0);
    public static bool Reload => Input.GetKeyDown(KeyCode.R);
    public static bool Take => Input.GetKeyDown(KeyCode.T);
    public static bool Hide => Input.GetKeyDown(KeyCode.H);
}