using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void FadeOutImmediate()
        {
            _canvasGroup.alpha = 1;
        }

        public IEnumerator FadeOut(float time)
        {
            while (_canvasGroup.alpha < 1) // only for a limited amount of time : alpha is not 1
            {
                // moving alpha towards 1
                _canvasGroup.alpha += Time.deltaTime / time;

                yield return null;
            }
        }

        public IEnumerator FadeIn(float time)
        {
            while (_canvasGroup.alpha > 0) // only for a limited amount of time : alpha is not 1
            {
                // moving alpha towards 1
                _canvasGroup.alpha -= Time.deltaTime / time;

                yield return null;
            }
        }
    }
}