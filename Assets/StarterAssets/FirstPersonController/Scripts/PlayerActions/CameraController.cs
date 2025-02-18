using PlayerInterfases;
using UnityEngine;

namespace StarterAssets
{
    public class CameraController : ICameraController
    {
        private FPSControllerBase _fpsController;
        public CameraController(FPSControllerBase fpsController)
        {
            _fpsController = fpsController;
        }
        public void CameraRotation()
        {
            if (_fpsController._input.look.sqrMagnitude >= _fpsController._threshold)
            {
                float deltaTimeMultiplier = _fpsController.IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _fpsController.SetCinemachineTargetPitch(_fpsController._cinemachineTargetPitch + _fpsController._input.look.y * _fpsController.RotationSpeed * deltaTimeMultiplier);
                _fpsController.SetRotationVelocity(_fpsController._input.look.x * _fpsController.RotationSpeed * deltaTimeMultiplier);

                _fpsController.SetCinemachineTargetPitch(FPSControllerBase.ClampAngle(_fpsController._cinemachineTargetPitch, _fpsController.BottomClamp, _fpsController.TopClamp));

                _fpsController.transform.Rotate(Vector3.up * _fpsController._rotationVelocity);
            }
        }

        public void ExtrudeHeadPointAndAiming()
        {
            Vector3 basePosition = new Vector3(_fpsController.transform.position.x, _fpsController.transform.position.y + 1.69f, _fpsController.transform.position.z);
            Vector3 offset = new Vector3(0, 0, 5);
            Vector3 rotation = new Vector3(_fpsController._cinemachineTargetPitch, 0, 0);

            _fpsController.aimingPoint.transform.position = basePosition + (_fpsController.transform.rotation * Quaternion.Euler(rotation)) * offset;

            _fpsController.head.transform.LookAt(_fpsController.aimingPoint.transform.position);
            _fpsController.CinemachineCameraTarget.transform.LookAt(_fpsController.aimingPoint.transform.position);
        }
    }
}