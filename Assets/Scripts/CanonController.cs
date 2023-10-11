using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FishShooting
{
    public class CanonController : GenericSingleton<CanonController>
    {
        #region Publice Members
        public GameObject Pointer;
        public Transform CanonPivot,BulletInitPos;
        public float rotationSpeed;

        public List<GameObject> AllBullets;
        public GameObject BulletPrefab;
        #endregion

        Vector3 touchStartPos;
        Camera mainCamera;
        void Start()
        {
            mainCamera = Camera.main;
            //GetBullet();
            //InvokeRepeating(nameof(ShootFrequently), 1, 1);
        }

        GameObject GO;

        GameObject GetBullet()
        {
            for (int i = 0; i < AllBullets.Count; i++)
            {
                if (!AllBullets[i].gameObject.activeInHierarchy)
                {
                    GO = AllBullets[i].gameObject;
                    GO.SetActive(true);
                    GO.transform.position = BulletInitPos.position;
                    GO.transform.rotation = BulletInitPos.rotation;
                    return GO;
                }
            }
            GO = Instantiate(BulletPrefab);
            GO.SetActive(true);
            GO.transform.position = BulletInitPos.position;
            GO.transform.rotation = BulletInitPos.rotation;
            return GO;
        }

        RaycastHit hit;
        void ShootFrequently()
        {
            if(Physics.Raycast(BulletInitPos.position,BulletInitPos.transform.up,20))
            {
                Debug.Log("<color=yellow> ---- Shoot :::: </color> ");
            }
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchStartPos = Input.mousePosition;
            }
            Debug.DrawRay(BulletInitPos.position, BulletInitPos.transform.up * 20, Color.red);
            if (Input.GetMouseButton(0))
            {
                Vector2 mousePosition = Input.mousePosition;
                float swipeValue = touchStartPos.x - mousePosition.x;
                //touchStartPos = mousePosition;

                Vector3 targetPosition = mainCamera.ScreenToWorldPoint(mousePosition);

                // Calculate the direction from the current position to the target position
                Vector3 direction = targetPosition - CanonPivot.transform.position;

                // Calculate the rotation angle in degrees
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Apply the rotation to the CanonPivot
                CanonPivot.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle-90));
                Pointer.transform.position =  new Vector3(targetPosition.x, targetPosition.y, -1f);
            }
        }
    }
}
