using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Resources;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        private Mover _movementController;
        private Fighter _fighter;
        private Health _health;

        void Awake()
        {
            _movementController = GetComponent<Mover>();
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!enabled) return;

            if (_health.IsDead()) return;

            if (InteractWithCombat()) return;
            if (InteractWithMovement()) return;
        }

        private bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());

            // Trying to find a target to attack
            foreach (RaycastHit hit in hits)
            {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();

                if (target == null) continue;
                if (!_fighter.CanAttack(target.gameObject)) continue;

                if (Input.GetMouseButton(1))
                {
                    _fighter.Attack(target.gameObject);
                }

                // If we found one, return true
                return true;
            }

            return false;
        }

        private bool InteractWithMovement()
        {
            RaycastHit hit;

            if (Physics.Raycast(GetMouseRay(), out hit))
            {
                if (Input.GetMouseButton(1))
                {
                    _movementController.StartMoveAction(hit.point, 1f);
                }

                return true;
            }

            return false;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
