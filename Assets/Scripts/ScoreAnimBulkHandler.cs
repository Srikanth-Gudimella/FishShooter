using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FishShooting
{
    public class ScoreAnimBulkHandler : MonoBehaviour
    {
        public List<CoinBulkItemHandler> CoinObjList=new List<CoinBulkItemHandler>();
        public Canvas ScoreTxtCanvas;
        public TextMeshProUGUI ScoreTxt;
        public Transform TargetObj;
        public bool IsReady = true;
        public int Score = 100;
        public int HitByPlayerID;
        public int TempScore = 0;
        public SkeletonAnimation[] CoinSkeletonAnimation;

        private void OnEnable()
        {
            IsReady = false;
            ScoreTxtCanvas.gameObject.SetActive(true);
            ScoreTxtCanvas.worldCamera = Camera.main;
            TempScore = 0;
            ScoreTxt.text = TempScore + "";
            //if(HitByPlayerID>=3)
            //{
            //    CoinObjList[0].transform.localRotation = Quaternion.Euler(0, 0, 180);
            //}
            foreach (CoinBulkItemHandler CoinObj in CoinObjList)
            {
                CoinObj.gameObject.SetActive(false);
            }
            Hashtable ht = iTween.Hash("from", 0, "to", Score, "time", 0.8f, "onupdate", "UpdateScore");

            //make iTween call:
            iTween.ValueTo(gameObject, ht);

            ScoreTxt.gameObject.transform.localPosition = Vector3.zero;// TargetObj.transform.localPosition;// new Vector3(0, 1, 0);
            //MoveScoreToTargetPos();
        }
        void UpdateScore(int currentScore)
        {
            Debug.Log("--- UpdateScore currentScore="+currentScore);
            TempScore = currentScore;

            if (TempScore>=Score)
            {
                TempScore = Score;
                MoveScoreToTargetPos();
            }
            ScoreTxt.text = currentScore + "";

        }
        public void MoveScoreToTargetPos()
        {
            Debug.Log("---- MoveToTargetPos");
            iTween.MoveTo(ScoreTxt.gameObject, iTween.Hash("position", TargetObj.transform.position+new Vector3(0,0.5f,0), "time",1f,"delay",0.1f, "easetype", iTween.EaseType.linear,"onComplete", "EnableCoinsAnim", "onCompleteTarget", gameObject));
        }
        void EnableCoinsAnim()
        {
            StartCoroutine(StartCoinsAnim());
        }
        public IEnumerator StartCoinsAnim()
        {
            yield return new WaitForSeconds(0.3f);
            ScoreTxtCanvas.gameObject.SetActive(false);
            for (int i=0;i< CoinObjList.Count;i++)// CoinBulkItemHandler CoinObj in CoinObjList)
            {
               // iTween.Stop(CoinObj);
                if (CoinSkeletonAnimation[i] != null)
                    CoinSkeletonAnimation[i].AnimationState.SetAnimation(0, "loop", true).TrackTime = 0;

                Vector3 RandPos = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.1f, 0f), 0);
                CoinObjList[i].transform.localPosition = ScoreTxt.gameObject.transform.localPosition + RandPos;
                CoinObjList[i].TargetObj = this.TargetObj;
                yield return new WaitForSeconds(Random.Range(0.05f,0.1f));
                CoinObjList[i].gameObject.SetActive(true);
            }
            yield return new WaitForSeconds(2);
            DisableThis();
        }
        //public void SetCoinPosition()
        //{
        //    Debug.Log("----- SetCoinPosition");
        //    CoinObjList[0].transform.localPosition = ScoreTxt.transform.localPosition;// new Vector3(0, 1, 0);
        //}
        
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
