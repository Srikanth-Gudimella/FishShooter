using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FishShooting
{
    public class PathController : MonoBehaviour
    {
        public List<Transform> PathPoints;
        public Vector3 InitialRotatioon = Vector3.zero;
        [Header("Creature Region==================================")]
        [Space(15)]
        public bool Is_CreaturePool;
        public GameObject CreaturePrefab;
        public List<GameObject> AllCreatures;

        private void OnEnable()
        {
            PathPoints.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                PathPoints.Add(transform.GetChild(i));
            }
            if (Is_CreaturePool)
            {
                InvokeRepeating(nameof(PoolCreatures), 0.1f, 1.5f);
                //Invoke(nameof(DisableIt), Random.Range(15, 20));
            }
        }
        void DisableIt()
        {
            gameObject.SetActive(false);
            CancelInvoke(nameof(PoolCreatures));
        }

        private void OnDisable()
        {
            if (Is_CreaturePool)
            {
                CancelInvoke(nameof(PoolCreatures));
                //for (int i = 0; i < AllCreatures.Count; i++)
                //{
                //    Destroy(AllCreatures[i].gameObject);
                //}
                //AllCreatures.Clear();
            }
        }
        void PoolCreatures()
        {
            Debug.Log("---- Pooling Creature 000");
            GO = GetFish();
            Aquatic Fish = GO.GetComponent<Aquatic>();
            Fish.Path = this;//AllCreatures[Random.Range(0, AllCreatures.Count)];
            Fish.SetInitials();
        }
        GameObject GO;

        GameObject GetFish()
        {
            for (int i = 0; i < AllCreatures.Count; i++)
            {
                if (!AllCreatures[i].gameObject.activeInHierarchy)
                {
                    GO = AllCreatures[i].gameObject;
                    GO.SetActive(true);
                    //GO.transform.SetPositionAndRotation(BulletInitPos.position, BulletInitPos.rotation);
                    return GO;
                }
            }
            GO = Instantiate(CreaturePrefab);
            AllCreatures.Add(GO);
            GO.SetActive(true);
            Debug.Log("---- Pooling Fish 111");

            //GO.transform.SetPositionAndRotation(BulletInitPos.position, BulletInitPos.rotation);
            return GO;
        }
    }
}
