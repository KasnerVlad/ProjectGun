using UnityEngine;

namespace StarterAssets.FirstPersonController.Scripts.PlayerActions
{
    public class CameraController : ICameraController
    {
        private readonly FPSControllerBase _fpsController;
        private float _cinemaMachineTargetPitch;
        private float _rotationVelocity=0.01f;
        private readonly float _minDistance;
        private readonly int layerMask = ~LayerMask.GetMask("Player"); 
        public CameraController(FPSControllerBase fpsController)
        {
            _fpsController = fpsController;
            _minDistance = _fpsController.minHitDistance.transform.localPosition.z;
        }
        public void CameraRotation()
        {
            if (Input2.Look.sqrMagnitude >= _fpsController.Threshold)
            {
                float deltaTimeMultiplier = 1.0f/*_fpsController.IsCurrentDeviceMouse ? 1.0f : Time.deltaTime*/;

                _cinemaMachineTargetPitch +=Input2.Look.y * _fpsController.RotationSpeed * deltaTimeMultiplier;
                _rotationVelocity = Input2.Look.x * _fpsController.RotationSpeed * deltaTimeMultiplier;

                _cinemaMachineTargetPitch = FPSControllerBase.ClampAngle(_cinemaMachineTargetPitch, _fpsController.BottomClamp, _fpsController.TopClamp);

                _fpsController.transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        public void ExtrudeHeadPointAndAiming()
        {
            Vector3 basePosition = new Vector3(_fpsController.transform.position.x, _fpsController.transform.position.y + 1.69f, _fpsController.transform.position.z);
            Vector3 offset = new Vector3(0, 0, 5);
            Vector3 rotation = new Vector3(_cinemaMachineTargetPitch, 0, 0);

            _fpsController.aimingPoint.transform.position = basePosition + (_fpsController.transform.rotation * Quaternion.Euler(rotation)) * offset;
            _fpsController.head.transform.LookAt(_fpsController.aimingPoint.transform.position);
        }

        public void LookAtTarget()
        {
            Ray ray = new Ray(_fpsController.MainCamera.transform.position, 
                _fpsController.MainCamera.transform.forward);
            if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, 1000, layerMask))
            {
                float distance = hit.distance;
                if (distance < _minDistance) { _fpsController.cameraPoint.transform.position = _fpsController.minHitDistance.transform.position; }
                else { _fpsController.cameraPoint.transform.position = hit.point; }
            }
            else { _fpsController.cameraPoint.transform.position = _fpsController.maxHitDistance.transform.position; }
        }
    }
}