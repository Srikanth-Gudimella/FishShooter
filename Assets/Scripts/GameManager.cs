using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FishShooting
{
    public class GameManager : GenericSingleton<GameManager>
    {
        #region Public Members
        public GameObject CanonPrefabs;
        public List<CanonController> AllCanons;
        public int myPositionID;
        public List<Transform> PlayerPositions;

        public GameObject HitEffect;
        #endregion

        public override void Awake()
        {
            base.Awake();
        }
        private void Start()
        {
            InstantiateCanon();
        }
        GameObject GO;

        public void InstantiateCanon()
        {
            for (int i = 0; i < PlayerPositions.Count; i++)
            {
                GO = Instantiate(CanonPrefabs.gameObject, PlayerPositions[i].position, PlayerPositions[i].rotation);
                GO.GetComponent<CanonController>().myID = i;
                AllCanons.Add(GO.GetComponent<CanonController>());
            }
        }

        public void InstantiateEffect(Vector3 pos)
        {
            GameObject _efct = Instantiate(HitEffect, pos, Quaternion.identity);
        }

    }
}
