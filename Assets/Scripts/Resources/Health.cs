using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;

namespace RPG.Resources
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] private float healthPoints = 100f;

        private Animator _animator;
        private ActionScheduler _actionScheduler;
        private BaseStats _baseStats;

        private bool isDead;

        public bool IsDead()
        {
            return isDead;
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _actionScheduler = GetComponent<ActionScheduler>();
            _baseStats = GetComponent<BaseStats>();
        }

        private void Start()
        {
            healthPoints = _baseStats.GetHealth();
        }

        public void TakeDamage(float damage)
        {
            healthPoints = Mathf.Max(healthPoints - damage, 0f);
            
            if (healthPoints <= 0f)
            {
                Die();
            }
        }

        public void Die()
        {
            if (isDead) return;

            isDead = true;
            _animator.SetTrigger("Die");
            _actionScheduler.CancelCurrentAction();
        }

        public object CaptureState()
        {
            return healthPoints;
        }

        public void RestoreState(object state)
        {
            healthPoints = (float)state;

            if (healthPoints <= 0f)
            {
                Die();
            }
        }
    }
}
