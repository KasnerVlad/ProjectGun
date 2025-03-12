using UnityEngine;
using UnityEngine.UI;

namespace StarterAssets.FirstPersonController.Scripts.PlayerHpSystem
{
    public class PlayerHpView
    {
        private Scrollbar _healthSlider;
        public PlayerHpView(Scrollbar healthSlider) { _healthSlider = healthSlider; }
        public void UpdateHp(int hp, int maxhp) { _healthSlider.size = Mathf.Clamp(hp, 0, maxhp)/maxhp; ;}
    }
}