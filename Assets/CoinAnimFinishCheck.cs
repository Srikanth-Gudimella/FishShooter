using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinAnimFinishCheck : MonoBehaviour
{
    public CoinBulkItemHandler _ParentCoinBulkItemHandler;
    public void AnimFinish()
    {
        _ParentCoinBulkItemHandler.MoveToTargetPos();
    }
}
