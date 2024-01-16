

#if (UNITY_IOS || UNITY_TVOS)
using UnityEngine.SocialPlatforms.GameCenter;
#endif

namespace FishShooting
{
    using Firebase;
    using Firebase.Extensions;
    using Firebase.Firestore;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.UI;

    public class FirebaseHandler : MonoBehaviour
    {
        protected Firebase.Auth.FirebaseAuth auth;
        protected Firebase.Auth.FirebaseAuth otherAuth;
        protected Dictionary<string, Firebase.Auth.FirebaseUser> userByAuth =
      new Dictionary<string, Firebase.Auth.FirebaseUser>();

        private Firebase.AppOptions otherAuthOptions = new Firebase.AppOptions
        {
            ApiKey = "",
            AppId = "",
            ProjectId = ""
        };
        Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

        private bool fetchingToken = false;

        protected string displayName = "";
        //protected string email = "";
        //protected string password = "";

        // Registration Variables
        [Space]
        [Header("Registration")]
        public InputField nameRegisterField;
        public InputField emailRegisterField;
        public InputField passwordRegisterField;
        public InputField confirmPasswordRegisterField;
        // Login Variables
        [Space]
        [Header("Login")]
        public InputField emailLoginField;
        public InputField passwordLoginField;

        public static FirebaseFirestore DataBase;

        //public static FirebaseHandler Instance;

        private static FirebaseHandler _instance;
        public static FirebaseHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<FirebaseHandler>();
                    if (_instance == null)
                    {
                        Debug.LogError("FirebaseHandler not found! Please add a FirebaseHandler to your scene.");
                    }
                }
                return _instance;
            }
        }

        private void Awake()
        {
            //Instance = this;
            if (_instance != null && _instance != this)
            {
                Destroy(this);
            }
            else
            {
                DontDestroyOnLoad(this);
                //FirebaseApp app = FirebaseApp.DefaultInstance;
                //DataBase = FirebaseFirestore.GetInstance(app);
                DataBase = FirebaseFirestore.DefaultInstance;
                DataBase.ClearPersistenceAsync();
            }
        }
        public virtual void Start()
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    InitializeFirebase();
                    FirebaseApp app = FirebaseApp.DefaultInstance;
                   // DataBase = FirebaseFirestore.DefaultInstance;// GetInstance(app);
                    Debug.LogError("-----------------Database=" + DataBase);
                }
                else
                {
                    Debug.LogError(
                      "Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            });

            //FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            //{
            //    FirebaseApp app = FirebaseApp.DefaultInstance;
            //    DataBase = FirebaseFirestore.GetInstance(app);
            //    Debug.LogError("-----------------Database=" + DataBase);
            //    //SetDataToFirestore();
            //    //// Call the method to fetch data
            //    //FetchDataFromFirestore();
            //});
        }

        protected void InitializeFirebase()
        {
            Debug.Log(" InitializeFirebase Setting up Firebase Auth");
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            auth.StateChanged += AuthStateChanged;
            auth.IdTokenChanged += IdTokenChanged;
            // Specify valid options to construct a secondary authentication object.
            if (otherAuthOptions != null &&
                !(String.IsNullOrEmpty(otherAuthOptions.ApiKey) ||
                  String.IsNullOrEmpty(otherAuthOptions.AppId) ||
                  String.IsNullOrEmpty(otherAuthOptions.ProjectId)))
            {
                Debug.Log(" InitializeFirebase Setting up Firebase Auth otherauthoptions not null");
                
                try
                {
                    otherAuth = Firebase.Auth.FirebaseAuth.GetAuth(Firebase.FirebaseApp.Create(
                      otherAuthOptions, "Secondary"));
                    otherAuth.StateChanged += AuthStateChanged;
                    otherAuth.IdTokenChanged += IdTokenChanged;
                }
                catch (Exception)
                {
                    Debug.Log("ERROR: Failed to initialize secondary authentication object.");
                }
            }
            else
            {
                Debug.Log(" InitializeFirebase Setting up Firebase Auth otherauthoptions null");
                //AuthStateChanged(this, null);
            }
            //AuthStateChanged(this, null);
            //email = "srikanth.gamezeniq@gmail.com";
            //password = "sri123";
            //CreateUserWithEmailAsync();
        }
        void AuthStateChanged(object sender, System.EventArgs eventArgs)
        {
            Debug.LogError("---- AuthStateChanged");
            Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
            Firebase.Auth.FirebaseUser user = null;
            if (senderAuth != null) userByAuth.TryGetValue(senderAuth.App.Name, out user);
            if (senderAuth == auth && senderAuth.CurrentUser != user)
            {
                bool signedIn = user != senderAuth.CurrentUser && senderAuth.CurrentUser != null;
                if (!signedIn && user != null)
                {
                    Debug.Log("Signed out " + user.UserId);
                    //UIManager.Instance.OpenRegistrationPanel();//Srikanth
                    UIManager.Instance.OpenLoginPanel();
                }
                user = senderAuth.CurrentUser;
                userByAuth[senderAuth.App.Name] = user;
                if (signedIn)
                {
                    Debug.Log("AuthStateChanged Signed in " + user.UserId + "::isverified=" + user.IsEmailVerified+"::email="+user.Email);
                    displayName = user.DisplayName ?? "";
                    Debug.LogError("------- AuthStateChanged Display Detailed User Info");

                    // DisplayDetailedUserInfo(user, 1);
                    Debug.LogError("***************** Reload User Call 11111111");
                    ReloadUser();
                }
            }
            else
            {
                Debug.LogError("---- AuthStateChanged Not loggedin");
                //UIManager.Instance.OpenRegistrationPanel();//Srikanth
                UIManager.Instance.OpenLoginPanel();

            }
        }
        void IdTokenChanged(object sender, System.EventArgs eventArgs)
        {
            Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
            if (senderAuth == auth && senderAuth.CurrentUser != null && !fetchingToken)
            {
                senderAuth.CurrentUser.TokenAsync(false).ContinueWithOnMainThread(
                  task => Debug.Log(String.Format("Token[0:8] = {0}", task.Result.Substring(0, 8))));
            }
        }
        public void ReloadUser()
        {
            Debug.Log("------- ReloadUser");
            if (auth.CurrentUser == null)
            {
                Debug.Log("Not signed in, unable to reload user.");
                return;
            }
            Debug.Log("Reload User Data");
            auth.CurrentUser.ReloadAsync().ContinueWithOnMainThread(task => {

                if (task.IsCanceled)
                {
                    Debug.Log("ReloadUser canceled.");
                }
                else if (task.IsFaulted)
                {
                    Debug.Log("ReloadUser IsFaulted.");
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("ReloadUser IsCompleted.");
                    //Debug.Log(operation + " completed");
                    //complete = true;
                    if (auth.CurrentUser.IsEmailVerified)
                    {
                        StoreManager.UserName = auth.CurrentUser.DisplayName;
                        //Debug.Log("--------- before open game call 11111111 name="+auth.CurrentUser.DisplayName);
                        //Debug.LogError("------- Reload user EmailVerified ="+auth.CurrentUser.Email+"::UserID="+ auth.CurrentUser.UserId);
                        //Debug.Log("--------- before open game call 22222");
                        StoreManager.UserEmail = auth.CurrentUser.Email;
                        //Debug.Log("--------- before open game call 3333333");
                        StoreManager.UserID = auth.CurrentUser.UserId;
                        //Debug.Log("--------- before open game call 444444");
                        UIManager.Instance.DisableAllPanels();
                        //Debug.Log("--------- before open game call 55555");
                        //load game scene
                        Debug.Log("--------- before open game call");
                        UIManager.Instance.StartCoroutine(UIManager.Instance.OpenGame());
                    }
                    else
                    {
                        Debug.LogError("------- Reload user Email Not Verified ");
                        UIManager.Instance.OpenEmailVerificationPanel();
                    }
                }
                else
                {
                    Debug.Log("----------- LogTaskCompletion else");

                }
                return;
                if (LogTaskCompletion(task, "Reload"))
                {
                    Debug.LogError("------- Display Detailed User Info");
                    if(auth.CurrentUser.IsEmailVerified)
                    {
                        StoreManager.UserName = auth.CurrentUser.DisplayName;
                        //Debug.Log("--------- before open game call 11111111 name="+auth.CurrentUser.DisplayName);
                        //Debug.LogError("------- Reload user EmailVerified ="+auth.CurrentUser.Email+"::UserID="+ auth.CurrentUser.UserId);
                        //Debug.Log("--------- before open game call 22222");
                        StoreManager.UserEmail = auth.CurrentUser.Email;
                        //Debug.Log("--------- before open game call 3333333");
                        StoreManager.UserID = auth.CurrentUser.UserId;
                        //Debug.Log("--------- before open game call 444444");
                        UIManager.Instance.DisableAllPanels();
                        //Debug.Log("--------- before open game call 55555");
                        //load game scene
                        Debug.Log("--------- before open game call");
                        //UIManager.Instance.OpenGame();
                    }
                    else
                    {
                        Debug.LogError("------- Reload user Email Not Verified ");
                        UIManager.Instance.OpenEmailVerificationPanel();
                    }
                    //DisplayDetailedUserInfo(auth.CurrentUser, 1);
                }
            });
        }
        protected bool LogTaskCompletion(Task task, string operation)
        {
            bool complete = false;
            try
            {
                if (task.IsCanceled)
                {
                    Debug.Log(operation + " canceled.");
                }
                else if (task.IsFaulted)
                {
                    Debug.Log(operation + " encounted an error.");
                    foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                    {
                        string authErrorCode = "";
                        Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                        if (firebaseEx != null)
                        {
                            authErrorCode = String.Format("AuthError.{0}: ",
                              ((Firebase.Auth.AuthError)firebaseEx.ErrorCode).ToString());
                        }
                        Debug.Log(authErrorCode + exception.ToString());
                    }
                }
                else if (task.IsCompleted)
                {
                    Debug.Log(operation + " completed");
                    complete = true;
                }
                else
                {
                    Debug.Log("----------- LogTaskCompletion else");

                }
            }
            catch(Exception e)
            {
                Debug.LogError("LogTaskCompletion Exception");
            }
            return complete;
        }
        public void Register()
        {
            StartCoroutine(CreateUserWithEmailAsync(nameRegisterField.text, emailRegisterField.text, passwordRegisterField.text, confirmPasswordRegisterField.text));
        }
        public IEnumerator CreateUserWithEmailAsync(string name, string email, string password, string confirmPassword)
        {
            yield return new WaitForSeconds(0);
            if (name == "")
            {
                Debug.LogError("User Name is empty");
            }
            else if (email == "")
            {
                Debug.LogError("email field is empty");
            }
            else if (passwordRegisterField.text != confirmPasswordRegisterField.text)
            {
                Debug.LogError("Password does not match");
            }
            else
            {
                //email = EmailField.text;
                //password = PasswordField.text;
                Debug.Log("CreateUserWithEmailAsync email=" + email + "::password=" + password);
                // DisableUI();

                // This passes the current displayName through to HandleCreateUserAsync
                // so that it can be passed to UpdateUserProfile().  displayName will be
                // reset by AuthStateChanged() when the new user is created and signed in.
                string _DisplayName = name;
                auth.CreateUserWithEmailAndPasswordAsync(email, password)
                  .ContinueWithOnMainThread((task) =>
                  {
                      Debug.Log("create user task finished");
                      //EnableUI();
                      if (LogTaskCompletion(task, "User Creation"))
                      {
                          Debug.Log("created user successfully");
                          var user = task.Result.User;
                          StartCoroutine(SendEmailForVerificationAsyn(user));
                          //DisplayDetailedUserInfo(user, 1);
                          Debug.LogError("DisplayDetailedUserInfo 333333");
                          return UpdateUserProfileAsync(newDisplayName: _DisplayName);
                      }
                      return task;
                  }).Unwrap();
            }
        }
        public void LoginAgain()
        {
            UIManager.Instance.OpenLoginPanel();
        }
        public void Login()
        {
            //StartCoroutine(LoginAsync(emailLoginField.text, passwordLoginField.text));
            SigninWithEmailCredentialAsync(emailLoginField.text, passwordLoginField.text);
        }
        public Task SigninWithEmailCredentialAsync(string email, string password)
        {
            Debug.Log(String.Format("Attempting to sign in as {0}...", email));
            //DisableUI();
            //if (signInAndFetchProfile)
            //{
            //    return auth.SignInAndRetrieveDataWithCredentialAsync(
            //      Firebase.Auth.EmailAuthProvider.GetCredential(email, password)).ContinueWithOnMainThread(
            //        HandleSignInWithAuthResult);
            //}
            //else
            {
                return auth.SignInWithCredentialAsync(
                  Firebase.Auth.EmailAuthProvider.GetCredential(email, password)).ContinueWithOnMainThread(
                    HandleSignInWithUser);
            }
        }
        void HandleSignInWithUser(Task<Firebase.Auth.FirebaseUser> task)
        {
            //EnableUI();
            if (LogTaskCompletion(task, "Sign-in"))
            {
                Debug.Log("------- HandleSignInwithuser ReloadUser call");
                    Debug.LogError("***************** Reload User Call 222222222");
                ReloadUser();
            }
        }
        public Task UpdateUserProfileAsync(string newDisplayName = null)
        {
            if (auth.CurrentUser == null)
            {
                Debug.Log("Not signed in, unable to update user profile");
                return Task.FromResult(0);
            }
            displayName = newDisplayName ?? displayName;
            Debug.Log("Updating user profile");
            //DisableUI();
            return auth.CurrentUser.UpdateUserProfileAsync(new Firebase.Auth.UserProfile
            {
                DisplayName = displayName,
                PhotoUrl = auth.CurrentUser.PhotoUrl,
            }).ContinueWithOnMainThread(task => {
                //EnableUI();
                if (LogTaskCompletion(task, "User profile"))
                {
                      Debug.LogError("DisplayDetailedUserInfo 444444");
                    //DisplayDetailedUserInfo(auth.CurrentUser, 1);
                }
            });
        }
        private IEnumerator SendEmailForVerificationAsyn(Firebase.Auth.FirebaseUser user)
        {
            yield return new WaitForSeconds(0);
            if (user != null)
            {
                Debug.LogError("--------- Email verification 11111");
                var sendEmailTask = user.SendEmailVerificationAsync();
                if (sendEmailTask.Exception != null)
                {
                    Debug.LogError("--------- Email verification sending error");

                }
                else
                {
                    Debug.LogError("--------- Email verification sent successfully");
                    UIManager.Instance.OpenEmailVerificationPanel();
                    Debug.LogError("***************** Reload User Call 33333333");
                    ReloadUser();

                    //ReloadUser();
                }
            }
        }
        public void Resend()
        {
            StartCoroutine(SendEmailForVerificationAsyn(auth.CurrentUser));
        }
        public void Signout()
        {
            auth.SignOut();
        }
        public void DeleteUser()
        {
            DeleteUserAsync();
        }
        protected Task DeleteUserAsync()
        {
            if (auth.CurrentUser != null)
            {
                Debug.Log(String.Format("Attempting to delete user {0}...", auth.CurrentUser.UserId));
               // DisableUI();
                return auth.CurrentUser.DeleteAsync().ContinueWithOnMainThread(task => {
                    //EnableUI();
                    //LogTaskCompletion(task, "Delete user");
                    if (LogTaskCompletion(task, "Delete user"))
                    {
                        UIManager.Instance.OpenRegistrationPanel();
                    }
                    else
                    {
                        Debug.LogError("Something wrong in deleting user");
                    }
                });
            }
            else
            {
                Debug.Log("Sign-in before deleting user.");
                UIManager.Instance.OpenLoginPanel();//Srikanth
                // Return a finished task.
                return Task.FromResult(0);
            }
        }
    }
    
}
