using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FishShooting
{
    public class ScoreAnimPoolManager : MonoBehaviour
    {
        public ScoreAnimHandler[] ScoreAnimObjsList;
        public static ScoreAnimPoolManager Instance;
        private void Awake()
        {
            Instance = this;
        }
        public ScoreAnimHandler GetScoreAnimObj()
        {
            ScoreAnimHandler _ScoreAnimHandler = null;
            foreach(ScoreAnimHandler obj in ScoreAnimObjsList)
            {
                if(obj.IsReady)
                {
                    _ScoreAnimHandler=obj;
                    break;
                }
            }
            if (_ScoreAnimHandler == null)
                _ScoreAnimHandler = ScoreAnimObjsList[0];//forcefully given
            return _ScoreAnimHandler;
        }
    }
}
