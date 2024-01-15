using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FishShooting
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [SerializeField]
        private GameObject loginPanel;

        [SerializeField]
        private GameObject registrationPanel;

        [SerializeField]
        private GameObject emailVerificationPanel;

        [SerializeField]
        private GameObject forgetPasswordPanel, settingsPanel, friendsPanel;

        [SerializeField]
        private GameObject GamePanel;
        public GameObject LogoutBtn;
        public Text CoinsTxt, ScoreTxt;
        public string UserEmail;
        public string UserID;

        public int userCoins, userScore;
        private void Awake()
        {
            CreateInstance();
        }
        private void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }

            //CoinsTxt.text = "Coins : " + userCoins;
            //ScoreTxt.text = "Score : " + userScore;

            //CoinsTxt.text = "Coins : " + tempcoins;

        }
        private void CreateInstance()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void OpenLoginPanel()
        {
            DisableAllPanels();
            loginPanel.SetActive(true);
        }

        public void OpenRegistrationPanel()
        {
            Debug.LogError("---- open Registration panel");
            DisableAllPanels();
            registrationPanel.SetActive(true);
        }
        public void OpenEmailVerificationPanel()
        {
            DisableAllPanels();
            emailVerificationPanel.SetActive(true);
        }
        public void OpenForgetPasswordPanel()
        {
            DisableAllPanels();
            forgetPasswordPanel.SetActive(true);
        }
        public void CloseForgetPasswordPanel()
        {
            DisableAllPanels();
            loginPanel.SetActive(true);
        }

        public void OpenSettingsPanel()
        {
            DisableAllPanels();
            settingsPanel.SetActive(true);
        }
        public void CloseSettingsPanel()
        {
            DisableAllPanels();
            OpenLoginPanel();
        }
        public void OpenFriendsPanel()
        {
            DisableAllPanels();
            friendsPanel.SetActive(true);
        }

        public void OpenGame()
        {
            Debug.Log("---- OpenGame 11111");
            DisableAllPanels();
            //GamePanel.SetActive(true);
            //LogoutBtn.SetActive(true);
            //FirebaseDataBaseHandler.Instance.SetUserData();
            FirebaseDataBaseHandler.Instance.FetchData();
            Debug.Log("---- OpenGame 22222");

            //Use set data to save user data in database
            SceneManager.LoadScene("Menu");
        }
        public void DisableAllPanels()
        {
            Debug.Log("----- Desable All Panels");
            registrationPanel.SetActive(false);
            loginPanel.SetActive(false);
            emailVerificationPanel.SetActive(false);
            forgetPasswordPanel.SetActive(false);
            settingsPanel.SetActive(false);
            friendsPanel.SetActive(false);
            registrationPanel.SetActive(false);
            GamePanel.SetActive(false);
            LogoutBtn.SetActive(false);
        }
        public void SetCoinsValue()
        {
            Debug.LogError("------ SetCoinsValue coins=" + userCoins + "::userScore=" + userScore);
            //UnityThread.QueueOnMainThread(() =>
            //{
            //    textField.text = data; // Update the text field with the fetched data
            //});
            CoinsTxt.text = "Coins : " + userCoins;
            ScoreTxt.text = "Score : " + userScore;

            //CoinsTxt.gameObject.SetActive(false);
            //CoinsTxt.gameObject.SetActive(true);


            //ScoreTxt.gameObject.SetActive(false);
            //ScoreTxt.gameObject.SetActive(true);

        }
        public IEnumerator SetValues()
        {
            yield return new WaitForSeconds(0);
            //Debug.LogError("------ SetValues coins=" + userCoins + "::userScore=" + userScore);
            //CoinsTxt.text = "Coins : " + userCoins;
            //ScoreTxt.text = "Score : " + userScore;

            //CoinsTxt.gameObject.SetActive(false);
            //CoinsTxt.gameObject.SetActive(true);


            //ScoreTxt.gameObject.SetActive(false);

            //ScoreTxt.gameObject.SetActive(true);

        }
    }

}
