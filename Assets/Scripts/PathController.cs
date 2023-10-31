using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FishShooting
{
    public class PathController : MonoBehaviour
    {
        public List<Transform> PathPoints;
        // public enum e_StartFrom
        // {
        //     Left,Right,Up,Down
        // }
        // public e_StartFrom m_StartFrom;
        public Vector3 InitialRotatioon = Vector3.zero;
        //public Vector3 InitialScale = Vector3.one;
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
