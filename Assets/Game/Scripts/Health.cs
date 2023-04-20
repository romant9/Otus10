using System;
using TMPro;
using UnityEngine.UI;

namespace Bloodymary.Game
{
    [Serializable]
    public class Health
    {
        public Action OnDeath;

        public float MaxHealth;
        private float _currentHealth;

        //Слайдер здоровья вариативный
        private Slider HealthSlider; 
        private TextMeshProUGUI _healthIndicator;
        public bool IsAlive => _currentHealth > 0;

        public void InitHealth(Slider sl, CharacterController character = null)
        {
            //ВОПРОС! Почему character is null для ботов, активированных в скрипте SpawnController
            //HealthSlider = character.CurrentUIData.HealthSlider;
            HealthSlider = sl;
             _currentHealth = MaxHealth;
            HealthSlider.maxValue = MaxHealth;
            _healthIndicator = HealthSlider.transform.GetComponentInChildren<TextMeshProUGUI>();
            _healthIndicator.text = MaxHealth.ToString();
        }

        public void SetHealth(float damage)
        {
            _currentHealth -= damage;
            HealthSlider.value = MaxHealth - _currentHealth;

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                Die();
            }
            _healthIndicator.text = _currentHealth.ToString();

        }

        public void Heal()
        {
            _currentHealth = MaxHealth;
        }

        private void Die()
        {
            OnDeath?.Invoke();
        }
    }
}

