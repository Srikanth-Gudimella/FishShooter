using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IDamagable
{
    void ApplyDamage(int val);
}
namespace FishShooting
{
    public class Aquatic : MonoBehaviour,IDamagable
    {
        public PathController Path;
        public Transform CurrentPoint;
        [Space(10)]
        public float Speed;
        public int Health;
        public bool Is_Dead;
        int PathID;

        public void SetInitials()
        {
            Is_Dead = false;
            Health = 100;
            PathID = 0;
            CurrentPoint = Path.PathPoints[PathID];
            transform.SetPositionAndRotation(CurrentPoint.position, CurrentPoint.rotation);
        }

        void Start()
        {
            
        }
       
        void Update()
        {
            if (Is_Dead)
                return;
            if(CheckDist()<0.1f)
            {
                if (PathID < Path.PathPoints.Count - 1)
                    PathID++;
                else
                {
                    PathID = 0;
                    transform.SetPositionAndRotation(Path.PathPoints[PathID].position, Path.PathPoints[PathID].rotation);
                }

                CurrentPoint = Path.PathPoints[PathID];
            }
            transform.position = Vector3.MoveTowards(transform.position, CurrentPoint.position, Speed * Time.deltaTime);
            var targetRotation = Quaternion.LookRotation(CurrentPoint.transform.position - transform.position);
            targetRotation.y = 0;
            targetRotation.x = 0;
            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Speed*2 * Time.deltaTime);
        }

        float _dist;

        float CheckDist()
        {
            Vector3 dir = transform.position - CurrentPoint.position;
            //if(transform.position < dir*dir)
            _dist = Vector3.Distance(transform.position, CurrentPoint.position);
            return _dist;
        }

        public void ApplyDamage(int val)
        {
            //Debug.Log("---- Apply Damage -= " + val);
            Health = Health > 0 ? Health -= val : 0;
            Is_Dead = Health <= 0;
            Debug.Log("---- Health = " + Health);
            if(Is_Dead)
                gameObject.SetActive(false);
        }
    }
}
