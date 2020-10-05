using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using RPG.Control;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            A, B, C, D, E
        }

        [Header("Components")]
        [SerializeField] private Transform spawnPoint;

        [Header("Configuration")]
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] private int sceneBuildIndex = -1;
        [SerializeField] private float fadeOutDelay = 2f;
        [SerializeField] private float fadeInDelay = 1f;
        [SerializeField] private float fadeWaitTime = .5f;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;

            print("Portal triggered !");

            StartCoroutine(Transition());
        }

        private IEnumerator Transition()
        {
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
            Fader fader = FindObjectOfType<Fader>();
            PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

            if (sceneBuildIndex < 0)
            {
                Debug.LogError("No scene linked to the " + gameObject.name + ", be sure to provide one.");
                yield break;
            }

            DontDestroyOnLoad(gameObject);

            // Remove control
            playerController.enabled = false;

            yield return fader.FadeOut(fadeOutDelay);

            // save current level state
            wrapper.Save();

            yield return SceneManager.LoadSceneAsync(sceneBuildIndex);

            // Remove control
            PlayerController newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            newPlayerController.enabled = false;

            // load current level state
            wrapper.Load();

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            wrapper.Save();

            yield return new WaitForSeconds(fadeWaitTime);
            fader.FadeIn(fadeInDelay);

            // Restore control
            newPlayerController.enabled = true;

            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().enabled = false;

            player.transform.position = otherPortal.spawnPoint.position;
            player.transform.rotation = otherPortal.spawnPoint.rotation;

            player.GetComponent<NavMeshAgent>().enabled = true;
        }

        private Portal GetOtherPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;
                if (portal.destination != destination) continue;

                return portal;
            }

            return null;
        }
    }
}
