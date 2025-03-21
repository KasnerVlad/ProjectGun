using UnityEngine.Animations.Rigging;
using UnityEngine;
using UnityEngine.Serialization;

namespace StarterAssets.FirstPersonController.Scripts
{
    public class PistolController : WeaponControllerBase
    {
        [SerializeField] private float maxDefZ=0.4475f;
        [SerializeField] private float minDefZ=0.1829f;
        [SerializeField] private float maxAimZ=0.4475f;
        [SerializeField] private float minAimZ=0.1829f;
        [SerializeField] private float maxZCameraAngle;
        [SerializeField] private float minZCameraAngle;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Transform defultPos;
        [SerializeField] private Transform aimPos;
        public AnimationCurve interpolationCurve = AnimationCurve.Linear(0, 0, 1, 1); // Кривая интерполяции

        protected override void StartLogic()
        {
            _weaponView = new WeaponView(this,ammoText);
            _iAnimationsController = new WeaponAnimationsController(this);
            _pistolPlayerModel = new PistolPlayerModel(this);
            _iAnimationsController.SetAnimationID();
            if(ammoText != null){ _weaponView.UpdateText();}
            _aimPosConstraints = AimPosRig.GetComponent<MultiParentConstraint>();
            _defaultPosConstraints = DefultPosRig.GetComponent<MultiParentConstraint>();
        }

        protected override void UpdateLogic()
        {
            _iAnimationsController.UpdateAnimations();
            float cameraAngle = playerCamera.transform.localEulerAngles.x;
            cameraAngle = cameraAngle > 180 ? cameraAngle - 360 : cameraAngle; // Преобразуем в диапазон [-180, 180]

            // Нормализуем угол камеры в диапазон [0, 1]
            float t = Mathf.InverseLerp(minZCameraAngle, maxZCameraAngle, cameraAngle);

            // Применяем кривую интерполяции
            t = interpolationCurve.Evaluate(t);

            // Интерполируем между minZ и maxZ
            float newDefZ = Mathf.Lerp(minDefZ, maxDefZ, t);
            float newAimZ = Mathf.Lerp(minAimZ, maxAimZ, t);
            // Применяем новое значение Z к локальной позиции объекта
            Vector3 newDefPosition = defultPos.localPosition;
            Vector3 newAimPosition = aimPos.localPosition;
            newDefPosition.z = newDefZ;
            newAimPosition.z = newAimZ;
            defultPos.localPosition = newDefPosition;
            aimPos.localPosition = newAimPosition;
        }

        protected override void HideM() { _pistolPlayerModel.Hide(); }
        protected override void TakeM(){_=_pistolPlayerModel.Take();}
    }
}
