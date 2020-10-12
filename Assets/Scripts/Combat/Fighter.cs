using UnityEngine;
using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        [Header("Stats")]
        [SerializeField] private float attackRate = 2f; // time to wait between attacks

        [Header("Weapons")]
        [SerializeField] private Transform rightHandTransform = null;
        [SerializeField] private Transform leftHandTransform = null;
        [SerializeField] private WeaponConfig defaultWeapon = null;

        private Animator _animator;
        private Mover _movementController;
        private BaseStats _baseStats;
        private Health _attackTarget;
        private WeaponConfig _currentWeaponConfig;
        private LazyValue<Weapon> currentWeapon;

        float timeSinceLastAttack = Mathf.Infinity;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _movementController = GetComponent<Mover>();
            _baseStats = GetComponent<BaseStats>();

            _currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        private void Start() 
        {
            currentWeapon.ForceInit();
        }

        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (_attackTarget == null) return;
            if (_attackTarget.IsDead()) return;

            if (!IsInRange(_attackTarget.transform))
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

        private bool IsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) < _currentWeaponConfig.GetRange();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            if (!_movementController.CanMoveTo(combatTarget.transform.position) && IsInRange(combatTarget.transform)) return false;

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

            float damage = _baseStats.GetStat(Stat.Damage);

            if (currentWeapon.value != null)
            {
                currentWeapon.value.OnHit();
            }

            if (_currentWeaponConfig.HasProjectile())
            {
                _currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, _attackTarget, gameObject, damage);
            }
            else
            {
                _attackTarget.TakeDamage(gameObject, damage);
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

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return _currentWeaponConfig.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return _currentWeaponConfig.GetPercentageBonus();
            }
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            _currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            return weapon.Spawn(rightHandTransform, leftHandTransform, _animator);
        }

        public Health GetTarget()
        {
            return _attackTarget;
        }

        public object CaptureState()
        {
            return _currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            WeaponConfig weapon = UnityEngine.Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }
    }
}
