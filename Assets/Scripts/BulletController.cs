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
        [Networked] public NetworkBehaviourId TargetObjID { get; set; }

        public override void Spawned()
        {
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
            //Debug.LogError("---- Bullet init TargetAquaticID" + TargetAquaticID);

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
        public void OnTriggerEnter2D(Collider2D collision)
        {
            //{
                IDamagable damagable = collision.transform.GetComponent<IDamagable>();
                if (damagable != null)
                {
                    if(GameManager.Instance.IsMaster)
                    damagable.ApplyDamage(50, PlayerID);

                    damagable.AnimMaterial();
                    GameManager.Instance.InstantiateEffect(collision.ClosestPoint(transform.position));
                    

                // if(TargetObjID.Equals(collision.gameObject.GetComponent<Aquatic>().ObjectID))
                //Debug.Log("--- hitobj TargetAquaticID=" + collision.gameObject.GetComponent<Aquatic>().AquaticID);

                //if (TargetAquaticID == collision.gameObject.GetComponent<Aquatic>().AquaticID)
                if (TargetObjID.Equals(collision.gameObject.GetComponent<Aquatic>().ObjectID))
                {
                    Debug.LogError("----- Hit with selected target");
                }
                else
                {
                    Debug.LogError("------ wrong hit");
                }

                if (Runner != null && Object != null)
                {
                    //Debug.LogError("--- Despawn this bullet");
                    Runner.Despawn(Object);
                }
                else
                {
                    Debug.LogError("--- Despawn not worked");
                }
            }
               

                //GameManager.Instance.InstantiateEffect(collision.GetContact(0).point);

                //CancelInvoke("DisableIt");
                //Debug.Log("bullet runner="+Runner);
                //Debug.Log("bullet Object="+Object);
                

                // gameObject.SetActive(false);
            //}
        }
        
    }
}
