using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FishShooting
{
    public class StoreManager
    {
        public static string CreditsStr = "Score";
        public static string WinsStr = "Winnings";
        public static string DispalyNameStr = "Name";
        public static string EmailStr = "Email";
        public static string UserIDStr = "PlayerID";

        public static int DefaultUserScore = 10000;

        public static int UserCredits
        {
            get
            {
                return PlayerPrefs.GetInt(CreditsStr, 0);
            }
            set
            {
                PlayerPrefs.SetInt(CreditsStr, value);
            }
        }
        public static int UserWins
        {
            get
            {
                return PlayerPrefs.GetInt(WinsStr, 0);
            }
            set
            {
                PlayerPrefs.SetInt(WinsStr, value);
            }
        }
        public static string UserName
        {
            get
            {
                return PlayerPrefs.GetString(DispalyNameStr, string.Empty);
            }
            set
            {
                PlayerPrefs.SetString(DispalyNameStr, value);
            }
        }
        public static string UserEmail
        {
            get
            {
                return PlayerPrefs.GetString(EmailStr, string.Empty);
            }
            set
            {
                PlayerPrefs.SetString(EmailStr, value);
            }
        }
        public static string UserID
        {
            get
            {
                return PlayerPrefs.GetString(UserIDStr, string.Empty);
            }
            set
            {
                PlayerPrefs.SetString(UserIDStr, value);
            }
        }
    }
}
