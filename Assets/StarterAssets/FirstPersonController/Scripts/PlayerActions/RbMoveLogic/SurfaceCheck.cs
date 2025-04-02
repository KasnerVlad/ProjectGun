using UnityEngine;
using System.Collections.Generic;
using StarterAssets.FirstPersonController.Scripts;
using StarterAssets.FirstPersonController.Scripts.PlayerActions;

namespace Rb.Move
{
    public class SurfaceCheck : ISurfaceCheck
    {
        public void Draw()
        {
            Debug.DrawLine(_controller.transform.position, _controller.transform.position + CalculateAverageNormal() * 3, Color.green);
            foreach (var contact in _contacts)
            {
                Debug.DrawLine(_controller.transform.position, _controller.transform.position + contact * 3, Color.red);
            }
        }
        private readonly List<Vector3> _contacts = new List<Vector3>();
        private readonly RbMoveController _rbMoveController;
        private readonly FPSControllerBase _controller;
        public bool HasGroundContacts => _contacts.Count > 0;

        public SurfaceCheck(FPSControllerBase fpsController, RbMoveController rbMoveController)
        {
            _controller = fpsController;
            _rbMoveController = rbMoveController;
            _rbMoveController.onStay += OnCollisionStay;
            _rbMoveController.onExit += OnCollisionExit;
        }

        public Vector3 CalculateAverageNormal()
        {
            if (_contacts.Count == 0) return Vector3.up;
    
            Vector3 sum = Vector3.zero;
            int validContacts = 0;
    
            foreach (var contact in _contacts)
            {
                // Ослаблен фильтр до 25 градусов
                if(Vector3.Angle(contact, Vector3.up) < 25f) 
                {
                    sum += contact;
                    validContacts++;
                }
            }
    
            return validContacts > 0 
                ? (sum / validContacts).normalized 
                : Vector3.up;
        }

        private void OnCollisionStay(Collision collision)
        {
            ClearContactNormals();
            foreach (ContactPoint contact in collision.contacts)
            {
                if (IsValidSlope(contact.normal))
                {
                    AddContact(contact.normal);
                }
            }
        }
        private bool IsValidSlope(Vector3 normal) => 
            Vector3.Angle(normal, Vector3.up) <= 90;
        private void OnCollisionExit(Collision collision)
        {
            ClearContactNormals();
        }
        private void AddContact(Vector3 normal)
        {
            _contacts.Add(normal);
        }

        public void ClearContactNormals() => _contacts.Clear();
    }

    public interface ISurfaceCheck
    {
        public bool HasGroundContacts { get; }
        Vector3 CalculateAverageNormal();
        void ClearContactNormals();
        void Draw();
    }
}