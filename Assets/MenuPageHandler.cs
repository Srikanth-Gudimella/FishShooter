using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPageHandler : MonoBehaviour
{
    //private void Start()
    //{
        
    //}
    public void PlayBtnClick()
    {
        SceneManager.LoadScene("InGame");
    }
    public void LogOutClick()
    {
        FirebaseHandler.Instance.Signout();
    }
}
