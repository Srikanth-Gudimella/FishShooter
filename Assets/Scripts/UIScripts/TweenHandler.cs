using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FishShooting
{
    public class TweenHandler : MonoBehaviour
    {
        public List<GameObject> TweenObjs;

        private void OnEnable()
        {
            float _delay = 0f;
            for (int i = 0; i < TweenObjs.Count; i++)
            {
                Debug.LogError("----");
                iTween.ScaleFrom(TweenObjs[i].gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", _delay, "easetype", iTween.EaseType.spring));
                _delay += 0.2f;
            }
        }

    }
}

