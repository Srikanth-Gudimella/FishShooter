using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBulkItemHandler : MonoBehaviour
{
    public Transform TargetObj;
    public void MoveToTargetPos()
    {
        Debug.Log("---- CoinBulkItemHandler MoveToTargetPos Targetobj="+ TargetObj);
        iTween.MoveTo(gameObject, iTween.Hash("position", TargetObj.position, "time", 0.2f, "easetype", iTween.EaseType.linear,"onComplete", "DisableThis","onCompleteTarget", gameObject));
    }
    public void DisableThis()
    {
        Debug.Log("----CoinBulkItemHandler DisableThis");
        gameObject.SetActive(false);
    }
}
