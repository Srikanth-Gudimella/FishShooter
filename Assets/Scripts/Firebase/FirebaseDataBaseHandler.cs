using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FishShooting
{
    public class FirebaseDataBaseHandler : MonoBehaviour
    {
        public static FirebaseDataBaseHandler Instance;
        DocumentReference docRef;
        private void Awake()
        {
            Instance = this;
        }
        public void FetchData()
        {
            Debug.LogError("---------- FetchData userID=" + UIManager.Instance.UserID+"---Database="+ FirebaseHandler.Instance.DataBase);
            try
            {
                docRef = FirebaseHandler.Instance.DataBase.Collection("users").Document("SrikanthTestID2");
            }
            catch(Exception e)
            {
                Debug.LogError("---------- FetchData exception="+e);

            }

            //return;
            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                DocumentSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    Debug.Log(String.Format("Document data for {0} document:", snapshot.Id));
                    Dictionary<string, object> city = snapshot.ToDictionary();
                    foreach (KeyValuePair<string, object> pair in city)
                    {
                        Debug.Log(String.Format("{0}: {1}", pair.Key, pair.Value));
                    }
                    //SetScore(250);
                    //get this data and set data
                }
                else
                {
                    Debug.Log(String.Format("Document {0} does not exist!", snapshot.Id));
                    //Set init values
                    SetDataToFirestore();
                }
            });

        }
        void SetDataToFirestore()
        {
            CollectionReference citiesRef = FirebaseHandler.Instance.DataBase.Collection("users");
            citiesRef.Document("SrikanthTestID2").SetAsync(new Dictionary<string, object>(){
            { "Email", "srikanth.gamezeniq@gmail.com" },
            { "Name", "Srikanth" },
            { "PlayerID", "IND" },
            { "Score", 1000 },
            { "Winnings", 200 }
            }).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Data has been successfully stored in the database.");
                    FetchData();
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("Error storing data: " + task.Exception);
                }
            });

            
        }
        public void SetScore(int score)
        {
            Debug.Log("------ Set User Data");
            CollectionReference citiesRef = FirebaseHandler.Instance.DataBase.Collection("users");
            Dictionary<string, object> updateUserDB = new Dictionary<string, object>
            {
                    { "Winnings", score }
            };
            citiesRef.Document("SrikanthTestID2").SetAsync(updateUserDB,SetOptions.MergeAll).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Data has been successfully stored in the database.");
                    FetchData();
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("Error storing data: " + task.Exception);
                }
            });
        }
        //public void FetchData1()
        //{
        //    Debug.LogError("FetchData userID=" + UIManager.Instance.UserID);
        //    //var task = 
        //    FirebaseDatabase.DefaultInstance
        // .GetReference("users").Child(UIManager.Instance.UserID).GetValueAsync().ContinueWithOnMainThread(task =>
        // {
        //     //await task;
        //     if (task.IsFaulted)
        //     {
        //         Debug.LogError("Error fetching user data: " + task.Exception);
        //         //SetUserData();//set user data if there is no user
        //         return;
        //     }
        //     else
        //     {
        //         Debug.LogError("no Error fetching user data: ");
        //     }

        //     DataSnapshot snapshot = task.Result;
        //     Debug.Log("snapshot=" + snapshot.ToString());
        //     if (snapshot.Exists)
        //     {
        //         string userid = snapshot.Child("userid").Value.ToString();

        //         string email = snapshot.Child("email").Value.ToString();
        //         //int coins = int.Parse(snapshot.Child("coins").Value.ToString());
        //         UIManager.Instance.userCoins = int.Parse(snapshot.Child("coins").Value.ToString());
        //         Debug.Log("--- UserCoins=" + UIManager.Instance.userCoins);
        //         Debug.Log("--- coins check=" + snapshot.Child("coins"));
        //         Debug.Log("--- score check=" + snapshot.Child("score"));
        //         UIManager.Instance.userScore = int.Parse(snapshot.Child("score").Value.ToString());
        //         Debug.Log("userid:" + userid + " Email: " + email + ", coins: " + UIManager.Instance.userCoins + "::score=" + UIManager.Instance.userScore);
        //         UIManager.Instance.SetCoinsValue();

        //         //UIManager.Instance.StartCoroutine(UIManager.Instance.SetValues());
        //     }
        //     else
        //     {
        //         Debug.Log("User data not found.");
        //         SetUserData();
        //     }
        // });

        //}
        //public void SetUserData()
        //{
        //    Debug.Log("------ Set User Data");
        //    DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference("users");

        //    Dictionary<string, object> data = new Dictionary<string, object>
        //    {
        //        { "userid", UIManager.Instance.UserID },
        //        { "email", UIManager.Instance.UserEmail },
        //        { "coins", 10000 },
        //        { "score", 0 }
        //    };

        //    reference.Child(UIManager.Instance.UserID).SetValueAsync(data)
        //    .ContinueWith(task =>
        //    {
        //        if (task.IsCompleted)
        //        {
        //            Debug.Log("Data has been successfully stored in the database.");
        //            FetchData();
        //        }
        //        else if (task.IsFaulted)
        //        {
        //            Debug.LogError("Error storing data: " + task.Exception);
        //        }
        //    });
        //}
        //public void SetScore(int score)
        //{
        //    Debug.Log("------ Set User Data");
        //    DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference("users");

        //    reference.Child(UIManager.Instance.UserID).Child("score").SetValueAsync(score)
        //    .ContinueWith(task =>
        //    {
        //        if (task.IsCompleted)
        //        {
        //            Debug.Log("Data has been successfully stored in the database.");
        //            FetchData();
        //        }
        //        else if (task.IsFaulted)
        //        {
        //            Debug.LogError("Error storing data: " + task.Exception);
        //        }
        //    });
        //}
        //public void SetUserData2()
        //{
        //    Debug.Log("------ Set User Data");
        //    DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference("users");


        //    reference.Child("srikanth").Child("name").SetValueAsync("Srikanth").ContinueWith(task =>
        //    {
        //        if (task.IsCompleted)
        //        {
        //            Debug.Log("Data has been successfully stored in the database.");
        //        }
        //        else if (task.IsFaulted)
        //        {
        //            Debug.LogError("Error storing data: " + task.Exception);
        //        }
        //    });

        //}
    }
}
