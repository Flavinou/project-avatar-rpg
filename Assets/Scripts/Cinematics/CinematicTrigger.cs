using System;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        private PlayableDirector _director;

        [SerializeField] private bool alreadyTriggered = false;

        private void Awake()
        {
            _director = GetComponent<PlayableDirector>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!alreadyTriggered && other.gameObject.CompareTag("Player"))
            {
                alreadyTriggered = true;
                _director.Play();
            }
        }
    }
}