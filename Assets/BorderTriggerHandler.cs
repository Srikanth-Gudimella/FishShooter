using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FishShooting
{
    public class BorderTriggerHandler : MonoBehaviour
    {
        public bool IsExitTrigger;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.LogError("--------- Border Trigger Enter");
            if (collision.tag == "Aquatic")
            {
                collision.gameObject.GetComponent<Aquatic>().IsInsideGame = !IsExitTrigger;
                //if (IsExitTrigger)
                //{

                //}
                //else
                //{

                //}
            }
        }
    }
}
