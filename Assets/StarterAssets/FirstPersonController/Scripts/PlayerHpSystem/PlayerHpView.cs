using UnityEngine;
using UnityEngine.UI;

namespace StarterAssets.FirstPersonController.Scripts.PlayerHpSystem
{
    public class PlayerHpView
    {
        private readonly Slider _healthSlider;
        public PlayerHpView(Slider healthSlider) { _healthSlider = healthSlider; }
        public void UpdateHp(int hp, int maxhp) { _healthSlider.value = Mathf.Clamp(hp, 0, maxhp)/maxhp; ;}
    }
}