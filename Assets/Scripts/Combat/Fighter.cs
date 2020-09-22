using UnityEngine;
using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using RPG.Resources;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        [Header("Stats")]
        [SerializeField] private float attackRate = 2f; // time to wait between attacks

        [Header("Weapons")]
        [SerializeField] private Transform rightHandTransform = null;
        [SerializeField] private Transform leftHandTransform = null;
        [SerializeField] private Weapon defaultWeapon = null;

        private Animator _animator;
        private Mover _movementController;
        private Health _attackTarget;
        [SerializeField] private Weapon _currentWeapon = null;

        float timeSinceLastAttack = Mathf.Infinity;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _movementController = GetComponent<Mover>();

            if (_currentWeapon == null)
                EquipWeapon(defaultWeapon);
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (_attackTarget == null) return;
            if (_attackTarget.IsDead()) return;

            if (!IsInRange())
            {
                //Debug.Log(gameObject.name + " is moving !");
                _movementController.MoveTo(_attackTarget.transform.position, 1f);
            }
            else
            {
                //Debug.Log(gameObject.name + " is going to attack !");
                _movementController.Cancel();
                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            transform.LookAt(_attackTarget.transform);

            if (timeSinceLastAttack > attackRate)
            {
                // This will trigger the "Hit" event
                TriggerAttack();
                timeSinceLastAttack = 0f;
            }
        }

        private void TriggerAttack()
        {
            _animator.ResetTrigger("StopAttack");
            _animator.SetTrigger("Attack");
        }

        private bool IsInRange()
        {
            return Vector3.Distance(transform.position, _attackTarget.transform.position) < _currentWeapon.GetRange();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;

            Health targetHealth = combatTarget.GetComponent<Health>();
            return (targetHealth != null && !targetHealth.IsDead());
        }

        public void Attack(GameObject target)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            _attackTarget = target.GetComponent<Health>();
        }

        // Animation event - used in the motion (editor)
        void Hit()
        {
            if (_attackTarget == null) return;

            if (_currentWeapon.HasProjectile())
            {
                _currentWeapon.LaunchProjectile(rightHandTransform, leftHandTransform, _attackTarget, gameObject);
            }
            else
            {
                _attackTarget.TakeDamage(gameObject, _currentWeapon.GetDamage());
            }
        }

        void Shoot()
        {
            Hit();
        }

        public void Cancel()
        {
            StopAttack();
            _attackTarget = null;
            _movementController.Cancel();
        }

        private void StopAttack()
        {
            _animator.ResetTrigger("Attack");
            _animator.SetTrigger("StopAttack");
        }

        public void EquipWeapon(Weapon weapon)
        {
            _currentWeapon = weapon;
            weapon.Spawn(rightHandTransform, leftHandTransform, _animator);
        }

        public Health GetTarget()
        {
            return _attackTarget;
        }

        public object CaptureState()
        {
            return _currentWeapon.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            Weapon weapon = UnityEngine.Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }
    }
}
