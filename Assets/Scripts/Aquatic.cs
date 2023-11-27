using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

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
        public bool Is_Dead,Is_Reached;

        private Rigidbody2D rb;
        int PathID;


        [Space(15)]
        public SkeletonAnimation SkeltonAnim;
        // Spine.AnimationState and Spine.Skeleton are not Unity-serialized objects. You will not see them as fields in the inspector.
        public Spine.AnimationState spineAnimationState;
        [SpineAnimation]
        public string Idle;
        [SpineAnimation]
        public string Die;

        public void SetInitials()
        {
            rb = GetComponent<Rigidbody2D>();
            Is_Dead = Is_Reached = false;
            Health = 100;
            PathID = 0;
            CurrentPoint = Path.PathPoints[PathID];
            transform.SetPositionAndRotation(CurrentPoint.position, CurrentPoint.rotation);
            rb.bodyType = RigidbodyType2D.Kinematic;
            transform.GetChild(0).transform.localEulerAngles = Path.InitialRotatioon;
            //transform.GetChild(0).transform.localScale = Path.InitialScale;
        }

        void Start()
        {
            spineAnimationState = SkeltonAnim.AnimationState;
            spineAnimationState.SetAnimation(1, Idle, true);
        }

        void Update()
        {
            if (Is_Dead || Is_Reached || !GameManager.Instance.IsMaster)
                return;

            if(CheckDist()<0.1f)
            {
                if (PathID < Path.PathPoints.Count - 1)
                    PathID++;
                else
                {
                    Is_Reached = true;
                    gameObject.SetActive(false);
                    //PathID = 0;
                    //transform.SetPositionAndRotation(Path.PathPoints[PathID].position, Path.PathPoints[PathID].rotation);
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
            if(Is_Dead)
            {
                GameObject[] otherObjects = GameObject.FindGameObjectsWithTag("Aquatic");

                foreach (GameObject obj in otherObjects)
                {
                    Physics2D.IgnoreCollision(obj.GetComponent<Collider2D>(), GetComponent<Collider2D>());
                }
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.AddTorque(100);
                Invoke(nameof(DeActivate), 4f);
                //spineAnimationState.SetAnimation(2,Die , false);
            }
        }

        void DeActivate()
        {
            gameObject.SetActive(false);
        }
    }
}
