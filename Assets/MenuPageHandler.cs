using Firebase.Sample.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FishShooting
{
    public class MenuPageHandler : MonoBehaviour
    {
        public GameObject SettingsPanel, AddFriendsPanel;
        public GameObject[] TickObjs;
        public GameObject[] Canons;
        public Text t_Balance, t_Winnings;

        private void Start()
        {
            SetCanonToggle();
            Debug.LogError("------------ MenuPageHandler Start usercredits="+StoreManager.UserCredits+"::wins="+StoreManager.UserWins);
            t_Balance.text = StoreManager.UserCredits+"";
            t_Winnings.text = StoreManager.UserWins+"";
            Invoke(nameof(UpdateCredits), 2);
        }
        void UpdateCredits()
        {
            Debug.LogError("------------ UpdateCredits=" + StoreManager.UserCredits + "::wins=" + StoreManager.UserWins);

            t_Balance.text = StoreManager.UserCredits + "";
            t_Winnings.text = StoreManager.UserWins + "";
        }
        public void PlayBtnClick()
        {
            SceneManager.LoadScene("InGame");
        }
        public void Call_SettingsPanel(bool _b)
        {
            SettingsPanel.SetActive(_b);
            for(int i=0;i<Canons.Length;i++)
            {
                Canons[i].SetActive(!_b);
            }
        }
        public void Call_AddFriendsPanel(bool _b)
        {
            AddFriendsPanel.SetActive(_b);
            for (int i = 0; i < Canons.Length; i++)
            {
                Canons[i].SetActive(!_b);
            }
        }
        public void LogOutClick()
        {
             FirebaseHandler.Instance.Signout();
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
