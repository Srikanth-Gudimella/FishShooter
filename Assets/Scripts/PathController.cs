using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FishShooting
{
    public class PathController : MonoBehaviour
    {
        public List<Transform> PathPoints;
        //public Transform CurrentPoint;
        int ID;

        void Awake()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                PathPoints.Add(transform.GetChild(i));
            }
        }
    }
}
