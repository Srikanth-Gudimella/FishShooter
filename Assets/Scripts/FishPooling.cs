using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FishShooting
{
    public class FishPooling : MonoBehaviour
    {
        public List<GameObject> FishPrefabs;
        public List<GameObject> AllFishes;
        //public List<GameObject> CreaturePrefabs;
        //public List<GameObject> AllCreatures;

        public List<PathController> AllPaths,CreaturePaths;

        public int FishStage;

        void Start()
        {
            InvokeRepeating(nameof(PoolFishes), 0.1f, 2);
        }
        void PoolFishes()
        {
            Debug.Log("---- Pooling Fish 000");
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
            Debug.Log("---- Pooling Fish 111");

            //GO.transform.SetPositionAndRotation(BulletInitPos.position, BulletInitPos.rotation);
            return GO;
        }
    }
}
