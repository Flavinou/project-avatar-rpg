using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using GameDevTV.Utils;

namespace RPG.Resources
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] private float regenerationPercentage = 70f;

        private LazyValue<float> healthPoints;

        private Animator _animator;
        private ActionScheduler _actionScheduler;
        private BaseStats _baseStats;

        private bool isDead;
         
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _actionScheduler = GetComponent<ActionScheduler>();
            _baseStats = GetComponent<BaseStats>();

            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        {
            return _baseStats.GetStat(Stat.Health);
        }

        private void Start()
        {
            healthPoints.ForceInit();
        }

        private void OnEnable() 
        {
            _baseStats.onLevelUp += RegenerateHealth;
        }

        private void OnDisable() 
        {
            _baseStats.onLevelUp -= RegenerateHealth;
        }

        public bool IsDead()
        {
            return isDead;
        }

        public float GetPercentage() 
        {
            return 100 * (healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health));
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            print(gameObject.name + " took damage: " + damage);

            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0f);
            if (healthPoints.value <= 0f)
            {
                Die();
                AwardExperience(instigator);
            }
        }

        public float GetHealthPoints()
        {
            return healthPoints.value;
        }

        public float GetMaxHealthPoints()
        {
            return _baseStats.GetStat(Stat.Health);
        }

        private void Die()
        {
            if (isDead) return;

            isDead = true;
            _animator.SetTrigger("Die");
            _actionScheduler.CancelCurrentAction();
        }

        private void AwardExperience(GameObject instigator)
        {
            print("Instigator : " + instigator.name);
            Experience experience = instigator.GetComponent<Experience>();

            if (experience == null) return;

            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        private void RegenerateHealth()
        {
            float regenHealthPoints = _baseStats.GetStat(Stat.Health) * (regenerationPercentage / 100);
            healthPoints.value = Mathf.Max(healthPoints.value, regenHealthPoints);
        }

        public object CaptureState()
        {
            return healthPoints.value;
        }

        public void RestoreState(object state)
        {
            healthPoints.value = (float)state;

            if (healthPoints.value <= 0f)
            {
                Die();
            }
        }
    }
}
