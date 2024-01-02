using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FishShooting
{
    public class BulletController : NetworkBehaviour
    {
        public float Speed = 15f;
        public int PlayerID;
        
        //private void OnEnable()
        //{
        //    Invoke(nameof(DisableIt), 2f);
        //}
        [Networked] private TickTimer life { get; set; }

        public override void Spawned()
        {
            //Debug.LogError("---- Bullet Spawned"+Object.InputAuthority);
            PlayerID = Object.InputAuthority;
            Init();
        }
        public void Init()
        {
            //Debug.Log("------ Ball Init");
            life = TickTimer.CreateFromSeconds(Runner, 1.5f);
        }

        public override void FixedUpdateNetwork()
        {
            
            transform.Translate(Vector3.up * Speed * Runner.DeltaTime);
            if (life.Expired(Runner))
            {
                Runner.Despawn(Object);
            }
            //else
            //	transform.position += 5 * transform.forward * Runner.DeltaTime;
        }
        //void DisableIt()
        //{
        //    gameObject.SetActive(false);
        //}
        //void Update()
        //{
        //    transform.Translate(Vector3.up * Speed * Time.deltaTime);
        //}
        //private void OnCollisionEnter2D(Collision2D collision)
        //{
        //    //Debug.Log("----Bullet OnCollisionEnter2D-");
        //   // if (GameManager.Instance.IsMaster)
        //    {
        //        IDamagable damagable = collision.transform.GetComponent<IDamagable>();
        //        if (damagable != null && GameManager.Instance.IsMaster)
        //            damagable.ApplyDamage(50,PlayerID);

        //        damagable.AnimMaterial();

        //        GameManager.Instance.InstantiateEffect(collision.GetContact(0).point);

        //        //CancelInvoke("DisableIt");
        //        //Debug.Log("bullet runner="+Runner);
        //        //Debug.Log("bullet Object="+Object);
        //        if (Runner != null && Object != null)
        //        {
        //            //Debug.LogError("--- Despawn this bullet");
        //            Runner.Despawn(Object);
        //        }
        //        else
        //        {
        //            Debug.LogError("--- Despawn not worked");
        //        }

        //        // gameObject.SetActive(false);
        //    }
        //}
        private void OnTriggerEnter2D(Collider2D collision)
        {
            {
                IDamagable damagable = collision.transform.GetComponent<IDamagable>();
                if (damagable != null && GameManager.Instance.IsMaster)
                    damagable.ApplyDamage(50, PlayerID);

                damagable.AnimMaterial();

                //GameManager.Instance.InstantiateEffect(collision.GetContact(0).point);
                GameManager.Instance.InstantiateEffect(collision.ClosestPoint(transform.position));

                //CancelInvoke("DisableIt");
                //Debug.Log("bullet runner="+Runner);
                //Debug.Log("bullet Object="+Object);
                if (Runner != null && Object != null)
                {
                    //Debug.LogError("--- Despawn this bullet");
                    Runner.Despawn(Object);
                }
                else
                {
                    Debug.LogError("--- Despawn not worked");
                }

                // gameObject.SetActive(false);
            }
        }
        
    }
}
