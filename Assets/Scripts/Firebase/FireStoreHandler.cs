using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections.Generic;

namespace FishShooting
{
    public class FireStoreHandler : MonoBehaviour
    {
        FirebaseFirestore db;

        void Start()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                db = FirebaseFirestore.GetInstance(app);
                Debug.LogError("-----------------db="+db);
                SetDataToFirestore();
                // Call the method to fetch data
                FetchDataFromFirestore();
            });
        }
        void SetDataToFirestore()
        {
            CollectionReference citiesRef = db.Collection("users");
            citiesRef.Document("SrikanthTestID").SetAsync(new Dictionary<string, object>(){
        { "Email", "srikanth.gamezeniq@gmail.com" },
        { "Name", "Srikanth" },
        { "PlayerID", "IND" },
        { "Score", 1000 },
        { "Winnings", 100 }
        }).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Data has been successfully stored in the database.");
                // FetchData();
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Error storing data: " + task.Exception);
            }
        });

            //CollectionReference citiesRef2 = db.Collection("users");
            //citiesRef.Document("SrikanthTestID").SetAsync(new Dictionary<string, object>()).ContinueWithOnMainThread(task => 
            //{

            //});


            //CollectionReference citiesRef2 = db.Collection("users");
            //citiesRef.Document("SrikanthTestID").SetAsync(new Dictionary<string, object>()).ContinueWithOnMainThread(task =>
            //{

            //});

        }
        void FetchDataFromFirestore()
        {
            DocumentReference docRef = db.Collection("users").Document("30p9ZtGCapZFQKSvnAV2l5g9Wa92");
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
                }
                else
                {
                    Debug.Log(String.Format("Document {0} does not exist!", snapshot.Id));
                }
            });
            return;
            // Specify the collection and document to fetch data from
            CollectionReference collection = db.Collection("users");
            DocumentReference document = collection.Document("30p9ZtGCapZFQKSvnAV2l5g9Wa92");

            // Fetch the data
            document.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("Failed to fetch data: " + task.Exception);
                    return;
                }

                DocumentSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    // Access data using snapshot.Get method
                    //string data = snapshot.Get("your_field_name").ToString();
                    //string data = snapshot.Get("your_field_name").ToString();

                    Debug.Log("Fetched data: ");
                }
                else
                {
                    Debug.LogWarning("Document does not exist!");
                }
            });
        }
    }
}
