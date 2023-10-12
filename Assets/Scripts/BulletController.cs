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
    }
}
