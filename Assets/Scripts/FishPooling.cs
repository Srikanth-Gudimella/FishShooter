using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FishShooting
{
    public class FishPooling : MonoBehaviour
    {
        public List<GameObject> FishPrefabs;
        public List<GameObject> AllFishes;
        public List<PathController> AllPaths;

        public int FishStage;

        void Start()
        {
            InvokeRepeating(nameof(PoolFishes), 1, Random.Range(2,4));
        }
        void PoolFishes()
        {
            Debug.Log("---- Pooling Fish");
            GO = GetFish();
            Aquatic Fish = GO.GetComponent<Aquatic>();
            Fish.Path = AllPaths[Random.Range(0, AllPaths.Count)];
            Fish.SetInitials();
        }
        GameObject GO;

        GameObject GetFish()
        {
            for (int i = 0; i < AllFishes.Count; i++)
            {
                if (!AllFishes[i].gameObject.activeInHierarchy)
                {
                    GO = AllFishes[i].gameObject;
                    GO.SetActive(true);
                    //GO.transform.SetPositionAndRotation(BulletInitPos.position, BulletInitPos.rotation);
                    return GO;
                }
            }
            GO = Instantiate(FishPrefabs[Random.Range(0,FishStage*3)]);
            AllFishes.Add(GO);
            GO.SetActive(true);
            //GO.transform.SetPositionAndRotation(BulletInitPos.position, BulletInitPos.rotation);
            return GO;
        }
    }
}
