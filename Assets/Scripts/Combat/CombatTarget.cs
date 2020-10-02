﻿using UnityEngine;
using RPG.Resources;
using RPG.Control;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (!callingController.GetComponent<Fighter>().CanAttack(gameObject))
                return false;

            if (Input.GetMouseButton(1))
            {
                callingController.GetComponent<Fighter>().Attack(gameObject);
            }

            // If we found one, return true
            return true;
        }
    }
}
