using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FishShooting
{
    public class GameUIHandler : MonoBehaviour
    {
        public TextMeshProUGUI t_Credits, t_Score;
        public static GameUIHandler Instance;
        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            UpdateCredits();
        }
        public void UpdateCredits()
        {
            t_Credits.text = StoreManager.UserCredits+"";
            t_Score.text = StoreManager.UserWins+"";
        }
        public void AddScore()
        {
            StoreManager.UserCredits += 100;
            FirebaseDataBaseHandler.Instance?.SetScore();
        }
    }
}
