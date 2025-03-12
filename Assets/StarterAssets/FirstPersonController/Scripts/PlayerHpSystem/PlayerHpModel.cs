using UnityEngine;
namespace StarterAssets.FirstPersonController.Scripts.PlayerHpSystem
{
    public class PlayerHpModel
    {
        public readonly int maxHp = 100;
        private readonly int _minHp = 0;
        public int CurrentHealth{get; private set;}
        public PlayerHpModel() {CurrentHealth = maxHp; }

        public void TakeDamage(int damage)
        {
            CurrentHealth -= damage;
            CurrentHealth = Mathf.Clamp(CurrentHealth, _minHp, maxHp);
        }
    }
}
