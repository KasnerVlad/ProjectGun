using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
public class WeaponController : WeaponControllerBase
{
    private WeaponView view;
    private IWeaponAmimationsController animationsController;
    private WeaponPlayerModel playerModel;
    private Animator animator;
    private float lastShotTime;
    protected override void StartLogic()
    {
        view = new WeaponView(BulletCount,LastBulletsCount ,ammoText);
        animationsController = new WeaponAmimationsController(this);
        playerModel = new WeaponPlayerModel(this);
        animator = GetComponent<Animator>();
        animationsController.SetAnimationID();
    }
    protected override void UpdateLogic()
    {
        animationsController.UpdateAnimations();
        lastShotTime -= Time.deltaTime;
    }
    protected override async Task FireManager()
    {

        if (BulletCount > 0 && WeaponInput.Fire && !isReloading &&lastShotTime <= 0)
        {
            lastShotTime = FireRate;
            SetFireState(true);
            Debug.Log("Fierd");
            MinusBullet();
            playerModel.Fire();
            Debug.Log(BulletCount + "/" + LastBulletsCount);
            Invoke(nameof(LateFire), FireRate);
            /*view.UpdateText();*/
        }
        if (BulletCount < startBulletCount && WeaponInput.Reload&&LastBulletsCount > 0)
        {
            Debug.Log("Reload");
            await playerModel.Reload();
        }
        if (WeaponInput.Take)
        {
            playerModel.Take();
        }
        if (WeaponInput.Hide)
        {
            playerModel.Hide();
        }
    }
    private void LateFire()
    {
        SetFireState(false);
    }
}