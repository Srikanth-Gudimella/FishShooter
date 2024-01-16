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
        bool IsFetched = false;
        public void FetchData()
        {
            Debug.LogError("---------- FetchData userID=" + StoreManager.UserID+"---Database="+ FirebaseHandler.Instance.DataBase);
           // if (IsFetched)
                //return;
            IsFetched = true;
            Debug.Log("---- Fetch 11111");
            if (FirebaseHandler.Instance != null && FirebaseHandler.Instance.DataBase != null)
            {
                Debug.Log("---- Fetch 22222");
                //FirebaseFirestore _docRef = FirebaseHandler.Instance.DataBase;//.Collection("users").Document(StoreManager.UserID);
                //if(FirebaseHandler.Instance.DataBase.Collection)
                docRef = FirebaseHandler.Instance.DataBase.Collection("users").Document(StoreManager.UserID);
            }
            Debug.Log("---- Fetch 333333");

            return;
            try
            {
                if (FirebaseHandler.Instance.DataBase != null)
                {
                    docRef = FirebaseHandler.Instance.DataBase.Collection("users").Document(StoreManager.UserID);
                }
                else
                {
                    Debug.LogError("---- Database issue");
                }
           

                docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
                {
                    DocumentSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        Debug.Log(String.Format("Document data for {0} document:", snapshot.Id));
                        Dictionary<string, object> UserData = snapshot.ToDictionary();
                        foreach (KeyValuePair<string, object> data in UserData)
                        {
                            Debug.Log(String.Format("{0}: {1}", data.Key, data.Value));
                        }
                        Debug.LogError("---- Fetch data userCredits 1111 =" + StoreManager.UserCredits + "::wins=" + StoreManager.UserWins);
                    
                        try
                        {
                            //check these with local data
                            StoreManager.UserCredits = (int)((long)UserData[StoreManager.CreditsStr]);
                            StoreManager.UserWins = (int)((long)UserData[StoreManager.WinsStr]);
                        }
                        catch(Exception e)
                        {
                            Debug.Log("--------- fetch error="+e);
                        }


                        Debug.LogError("---- Fetch data userCredits 222 ="+StoreManager.UserCredits+"::wins="+StoreManager.UserWins);

                       // if(StoreManager.UserName==string.Empty)
                        {
                            StoreManager.UserName= (string)UserData[StoreManager.DispalyNameStr];
                        }

                        //if (StoreManager.UserEmail == string.Empty)
                        {
                            StoreManager.UserEmail = (string)UserData[StoreManager.DispalyNameStr];
                        }
                        //if (StoreManager.UserID == string.Empty)
                        {
                            StoreManager.UserID = (string)UserData[StoreManager.UserIDStr];
                        }

                        Debug.LogError("---- Fetch data 33333 ");

                        //SetScore(250);
                        //get this data and set data
                    }
                    else
                    {
                        Debug.Log(String.Format("Document {0} does not exist!", snapshot.Id));
                        StoreManager.UserCredits = StoreManager.DefaultUserScore;
                        StoreManager.UserWins = 0;
                        //Set init values
                        SetDataToFirestore();
                    }
                });
            }
            catch (Exception e)
            {
                Debug.LogError("---------- FetchData exception=" + e);

            }

        }
        void SetDataToFirestore()
        {
            //{ "userid", UIManager.Instance.UserID },
        //        { "email", UIManager.Instance.UserEmail },
        //        { "coins", 10000 },
        //        { "score", 0 }

            CollectionReference citiesRef = FirebaseHandler.Instance.DataBase.Collection("users");
            citiesRef.Document(StoreManager.UserID).SetAsync(new Dictionary<string, object>(){
            { "Email", StoreManager.UserEmail },
            { "Name", StoreManager.UserName },
            { "PlayerID", StoreManager.UserID },
            { "Score", StoreManager.UserCredits },
            { "Winnings", StoreManager.UserWins }
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
        public void SetWinnings()
        {
            Debug.Log("------ Set User winns");
            CollectionReference citiesRef = FirebaseHandler.Instance.DataBase.Collection("users");
            Dictionary<string, object> updateUserDB = new Dictionary<string, object>
            {
                    { StoreManager.WinsStr, StoreManager.UserWins }
            };
            citiesRef.Document(StoreManager.UserID).SetAsync(updateUserDB,SetOptions.MergeAll).ContinueWithOnMainThread(task =>
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
        public void SetScore()//nothing but credits
        {
            Debug.Log("------ Set User credit");
            CollectionReference citiesRef = FirebaseHandler.Instance.DataBase.Collection("users");
            Dictionary<string, object> updateUserDB = new Dictionary<string, object>
            {
                    { StoreManager.CreditsStr, StoreManager.UserCredits }
            };
            citiesRef.Document(StoreManager.UserID).SetAsync(updateUserDB, SetOptions.MergeAll).ContinueWithOnMainThread(task =>
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
