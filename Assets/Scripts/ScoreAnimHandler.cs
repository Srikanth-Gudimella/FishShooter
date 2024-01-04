using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FishShooting
{
    public class ScoreAnimHandler : MonoBehaviour
    {
        public GameObject CoinObj;
        public Canvas ScoreTxtCanvas;
        public TextMeshProUGUI ScoreTxt;
        public Transform TargetObj;
        public bool IsReady = true;
        public int Score = 100;
        public int HitByPlayerID;
        public SkeletonAnimation CoinSkeletonAnimation;
        private void OnEnable()
        {
            IsReady = false;
            ScoreTxtCanvas.worldCamera = Camera.main;
            ScoreTxt.text = Score + "";
            CoinObj.transform.localPosition = ScoreTxt.transform.localPosition;// new Vector3(0, 1, 0);
            if (HitByPlayerID>=3)
            {
                CoinObj.transform.localRotation = Quaternion.Euler(0, 0, 180);
            }
        }
        public void SetCoinPosition()
        {
            Debug.Log("----- SetCoinPosition");
            SoundManager.Instance?.PlaySound(0);

            iTween.Stop(CoinObj);
            if (CoinSkeletonAnimation!=null)
                CoinSkeletonAnimation.AnimationState.SetAnimation(0, "loop", true).TrackTime = 0;
            CoinObj.transform.localPosition = ScoreTxt.transform.localPosition;// new Vector3(0, 1, 0);
        }
        public void MoveToTargetPos()
        {
            Debug.Log("---- MoveToTargetPos");
            if (TargetObj != null)
                iTween.MoveTo(CoinObj, iTween.Hash("position", TargetObj.transform.position, "time", 0.5f, "easetype", iTween.EaseType.linear));//,"onComplete", "DisableThis","onCompleteTarget", gameObject));
            else
                DisableThis();//Srikanth need to check this, this case is when coin anim time if user left
        }
        public void DisableThis()
        {
            Debug.Log("---- DisableThis");
            gameObject.SetActive(false);
            IsReady = true;
        }
        //public void ActivateCoin()
        //{
        //    ScoreTxtCanvas.gameObject.SetActive(false);
        //    CoinObj.SetActive(true);
        //}
    }
}
