using System;
using System.Collections;
using UnityEngine;

namespace UI
{
    public class SceneFaderUI : MonoBehaviour
    {
        [Header("场景渐入渐出设置")]
        [Tooltip("场景渐入的持续时间")] public float fadeInDurationTime;
        [Tooltip("场景渐出的持续时间")] public float fadeOutDurationTime;
        
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            DontDestroyOnLoad(this);
        }

        public IEnumerator FadeOut()
        {
            while (_canvasGroup.alpha < 1)
            {
                _canvasGroup.alpha += Time.deltaTime / fadeOutDurationTime;
                yield return null;
            }
        }

        public IEnumerator FadeIn()
        {
            while (_canvasGroup.alpha > 0)
            {
                _canvasGroup.alpha -= Time.deltaTime / fadeInDurationTime;
                yield return null;
            }
            Destroy(this);  // 避免一个场景中生成太多的SceneFaderUI对象
        }

        public IEnumerator FadeOutToIn()    // 渐出再渐入
        {
            yield return FadeOut();
            yield return FadeIn();
        }
        
        public IEnumerator FadeInToOut()    // 渐入再渐出
        {
            yield return FadeIn();
            yield return FadeOut();
        }
        
    }// class SceneFaderUI
}// namespace UI
