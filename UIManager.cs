using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

	public InputField LoginUser;
	public InputField LoginPass;

	public InputField RegisterUser;
	public InputField RegisterPass;
	public InputField RegisterPassConfirm;

	public Text ErrorNotification;
	public GameObject LoadingScreen;
	Slider LoadingSlider;
	AsyncOperation loadingScene;

	string savedUsername;

	public string APIUrl = "https://adrian-design.com/api";
	string AuthURL;
	string SaveURL;
	string CheckAuthURL;

	List<InputField> LoginNav = new List<InputField>();
	int LoginNavIndex = 0;

	[System.Serializable]
	public struct APILoginErrorData
	{
		public bool login;
		public string message;
	}

	public struct APIError {
		public int status;
		public APILoginErrorData error;
	}

	public struct RegisterComplete
	{
		public int id;
		public string username;
		public string token;
	}

	public struct LoginComplete
	{
		public int id;
		public string token;
		public string expireDate;
	}

	public struct AuthComplete
	{
		public string token;
		public string expireDate;
	}

	public class UserPrefsData
	{
		public int id;
		public string username;
		public string token;
		public string tokenExpireDate;
	}

	public void SendErrorNotification(string error)
	{
		ErrorNotification.enabled = true;
		ErrorNotification.text = error;
	}

	public void SetUserData(int id, string username, string token, string token_expire_date)
	{
		PlayerPrefs.SetInt("user_id", id);
		PlayerPrefs.SetString("user_username", username);
		PlayerPrefs.SetString("user_token", token);
		PlayerPrefs.SetString("user_token_expireDate", token_expire_date);
	}

	public void ClearUserData()
	{
		PlayerPrefs.DeleteAll();
	}


	public UserPrefsData GetUserData()
	{
		UserPrefsData localInfo = new UserPrefsData();

		localInfo.id = PlayerPrefs.GetInt("user_id", -1);
		
		if (localInfo.id == -1)
		{
			return null;
		}

		localInfo.username = PlayerPrefs.GetString("user_username");
		localInfo.token = PlayerPrefs.GetString("user_token");
		localInfo.tokenExpireDate = PlayerPrefs.GetString("user_token_expireDate");

		return localInfo;
	}
	IEnumerator CheckAuth(RegisterComplete data, bool alreadySet = false)
	{
		WWW request = new WWW(CheckAuthURL + data.token);

		yield return request;

		if (!string.IsNullOrEmpty(request.error))
		{

			if (!string.IsNullOrEmpty(request.text))
			{
				APIError errData = JsonUtility.FromJson<APIError>(request.text);
				SendErrorNotification(errData.error.message);
			}

		}
		else
		{


			// Set Player Prefs
			// user_id -> id (Int, Id of the account)
			// user_username -> savedUsername (String, username of the account)
			// user_token -> token (String, login token)
			// user_token_expireDate -> expire_date (String, token expire date)

			AuthComplete tokenDetails = JsonUtility.FromJson<AuthComplete>(request.text);

			if (!alreadySet)
			{
				SetUserData(data.id, savedUsername, tokenDetails.token, tokenDetails.expireDate);
			}

			LoadPostLoginLevel();
		}
	}

	IEnumerator RegisterPostRequest(string username, string password)
	{
		WWWForm form = new WWWForm();

		form.AddField("username", username);
		form.AddField("password", password);

		WWW request = new WWW(SaveURL, form);

		yield return request;

		if (!string.IsNullOrEmpty(request.error))
		{

			if (!string.IsNullOrEmpty(request.text))
			{
				APIError data = JsonUtility.FromJson<APIError>(request.text);
				SendErrorNotification(data.error.message);
			}

		}
		else
		{
			RegisterComplete data = JsonUtility.FromJson<RegisterComplete>(request.text);
			savedUsername = username;
			StartCoroutine(CheckAuth(data));
		}
	}

	IEnumerator LoginPostRequest(string username, string password)
	{
		WWWForm form = new WWWForm();

		form.AddField("username", username);
		form.AddField("password", password);

		WWW request = new WWW(AuthURL, form);

		yield return request;

		if (!string.IsNullOrEmpty(request.error))
		{
			
			if (!string.IsNullOrEmpty(request.text))
			{
				APIError data = JsonUtility.FromJson<APIError>(request.text);
				SendErrorNotification(data.error.message);
			}

		} else
		{
			LoginComplete loginDetails = JsonUtility.FromJson<LoginComplete>(request.text);
			SetUserData(loginDetails.id, username, loginDetails.token, loginDetails.expireDate);
			LoadPostLoginLevel();
		}
	}

	void LoadPostLoginLevel()
	{
		LoadLevel("First_Level");
	}


	// Use this for initialization
	void Start () {
		AuthURL = APIUrl + "/auth";
		SaveURL = APIUrl + "/save";
		CheckAuthURL = AuthURL + "/check/";
		LoadingSlider = LoadingScreen.GetComponentInChildren<Slider>();

		LoginNav.Add(LoginUser);
		LoginNav.Add(LoginPass);

		UserPrefsData localInfo = GetUserData();

		if (localInfo != null)
		{
			RegisterComplete data = new RegisterComplete();

			data.id = localInfo.id;
			data.token = localInfo.token;
			data.username = localInfo.username;

			if (data.token != null && data.token.Length > 0)
			{
				StartCoroutine(CheckAuth(data, true));
			}
		}
	}

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			if (LoginNavIndex < LoginNav.Count - 1)
			{
				LoginNav[LoginNavIndex + 1].Select();
				LoginNavIndex = LoginNavIndex + 1;
			}
			else
			{
				LoginNav[LoginNavIndex - 1].Select();
				LoginNavIndex = LoginNavIndex - 1;
			}
		}

		if (Input.GetKeyDown(KeyCode.Return))
		{
			LogIn();
		}
	}

	public void LogIn()
	{
		StartCoroutine(LoginPostRequest(LoginUser.text, LoginPass.text));
	}

	public void CreateAccount()
	{
		if (RegisterPass.text != RegisterPassConfirm.text)
		{
			SendErrorNotification("Passwords do not match !");
			return;
		}

		StartCoroutine(RegisterPostRequest(RegisterUser.text, RegisterPass.text));
	}


	void UpdateLoadingSlider()
	{
		if (loadingScene.progress < 0.9f)
		{
			LoadingSlider.value = loadingScene.progress;
		} else
		{
			LoadingSlider.value = 1f;
			loadingScene.allowSceneActivation = true;
		}
	}

	void CheckSceneProgress (AsyncOperation scene)
	{
		loadingScene = scene;
		InvokeRepeating("UpdateLoadingSlider", 0.1f, 0.5f);
	}

	IEnumerator AsyncLoadScene(string sceneName)
	{
		AsyncOperation sceneToLoad = SceneManager.LoadSceneAsync(sceneName);
		sceneToLoad.allowSceneActivation = false;
		CheckSceneProgress(sceneToLoad);
		yield return sceneToLoad;

	}

	public void LoadLevel(string sceneName)
	{
		LoadingScreen.SetActive(true);
        StartCoroutine(AsyncLoadScene(sceneName));
	}

	public void ExitGame()
	{
		Application.Quit();
	}
}
