using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FishShooting
{
    public class ScoreAnimBulkPoolManager : MonoBehaviour
    {
        public ScoreAnimBulkHandler[] ScoreAnimBulkObjsList;
        public static ScoreAnimBulkPoolManager Instance;
        private void Awake()
        {
            Instance = this;
        }
        public ScoreAnimBulkHandler GetScoreAnimBulkObj()
        {
            ScoreAnimBulkHandler _ScoreAnimBulkHandler = null;
            foreach(ScoreAnimBulkHandler obj in ScoreAnimBulkObjsList)
            {
                if(obj.IsReady)
                {
                    _ScoreAnimBulkHandler = obj;
                    break;
                }
            }
            return _ScoreAnimBulkHandler;
        }
    }
}
