using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using RPG.Core;
using RPG.Control;

namespace RPG.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        private PlayableDirector _director;
        private GameObject _player;

        private void Awake()
        {
            _director = GetComponent<PlayableDirector>();
            _player = GameObject.FindWithTag("Player");
        }

        private void OnEnable() 
        {
            _director.played += DisableControl;
            _director.stopped += EnableControl;
        }

        private void OnDisable() 
        {
            _director.played -= DisableControl;
            _director.stopped -= EnableControl;
        }

        void DisableControl(PlayableDirector director)
        {
            print("DisableControl");
            _player.GetComponent<ActionScheduler>().CancelCurrentAction();
            _player.GetComponent<PlayerController>().enabled = false;
        }

        void EnableControl(PlayableDirector director)
        {
            print("EnableControl");
            _player.GetComponent<PlayerController>().enabled = true;
        }
    }
}
