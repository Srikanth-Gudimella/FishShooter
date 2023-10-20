using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FishShooting
{
    public class BulletController : MonoBehaviour
    {
        public float Speed = 15f;
        
        private void OnEnable()
        {
            Invoke(nameof(DisableIt), 2f);
        }

        void DisableIt()
        {
            gameObject.SetActive(false);
        }
        void Update()
        {
            transform.Translate(Vector3.up * Speed * Time.deltaTime);
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            IDamagable damagable = collision.transform.GetComponent<IDamagable>();
            if (damagable != null)
                damagable.ApplyDamage(50);
            CancelInvoke("DisableIt");
            gameObject.SetActive(false);
        }

    }
}
