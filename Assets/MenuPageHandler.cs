using Firebase.Sample.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FishShooting
{
    public class MenuPageHandler : MonoBehaviour
    {
        public GameObject[] TickObjs;
        private void Start()
        {
            SetCanonToggle();
        }
        public void PlayBtnClick()
        {
            SceneManager.LoadScene("InGame");
        }
        public void LogOutClick()
        {
            // FirebaseHandler.Instance.Signout();
        }
        public void canon_0_Select()
        {
            GameManager.SelectedCanonIndex = 0;
            SetCanonToggle();
        }
        public void canon_1_Select()
        {
            GameManager.SelectedCanonIndex = 1;
            SetCanonToggle();
        }
        public void canon_2_Select()
        {
            GameManager.SelectedCanonIndex = 2;
            SetCanonToggle();
        }
        public void SetCanonToggle()
        {
            for(int i=0;i< TickObjs.Length;i++)
            {
                if(GameManager.SelectedCanonIndex==i)
                {
                    TickObjs[i].SetActive(true);
                }
                else
                {
                    TickObjs[i].SetActive(false);
                }
            }
        }
    }
}
