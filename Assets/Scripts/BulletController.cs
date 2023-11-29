using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FishShooting
{
    public class BulletController : NetworkBehaviour
    {
        public float Speed = 15f;
        
        //private void OnEnable()
        //{
        //    Invoke(nameof(DisableIt), 2f);
        //}
        [Networked] private TickTimer life { get; set; }

        public void Init()
        {
            //Debug.Log("------ Ball Init");
            life = TickTimer.CreateFromSeconds(Runner, 1.5f);
        }

        public override void FixedUpdateNetwork()
        {
            if (life.Expired(Runner))
                Runner.Despawn(Object);
            //else
            //	transform.position += 5 * transform.forward * Runner.DeltaTime;
        }
        //void DisableIt()
        //{
        //    gameObject.SetActive(false);
        //}
        void Update()
        {
            transform.Translate(Vector3.up * Speed * Time.deltaTime);
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
           // if (GameManager.Instance.IsMaster)
            {
                IDamagable damagable = collision.transform.GetComponent<IDamagable>();
                if (damagable != null && GameManager.Instance.IsMaster)
                    damagable.ApplyDamage(50);
                GameManager.Instance.InstantiateEffect(collision.GetContact(0).point);
                CancelInvoke("DisableIt");
                //Debug.Log("bullet runner="+Runner);
                //Debug.Log("bullet Object="+Object);
                if(Runner!=null && Object!=null)
                Runner.Despawn(Object);

                // gameObject.SetActive(false);
            }
        }

    }
}
