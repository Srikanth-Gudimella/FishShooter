using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private GameObject GamePanel;

    private void Awake()
    {
        CreateInstance();
    }

    private void CreateInstance()
    {
        if(Instance == null)
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
        DisableAllPanels();
        registrationPanel.SetActive(true);
    }
    public void OpenEmailVerificationPanel()
    {
        DisableAllPanels();
        emailVerificationPanel.SetActive(true);
    }
    public void OpenGame()
    {
        DisableAllPanels();
        SceneManager.LoadScene("Menu");
        //GamePanel.SetActive(true);
    }
    public void DisableAllPanels()
    {
        registrationPanel.SetActive(false);
        loginPanel.SetActive(false);
        emailVerificationPanel.SetActive(false);
        GamePanel.SetActive(false);
    }
}
