using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class ExitGameManager : MonoBehaviour {

	public GameObject player;

	PlayerController playerController;
	HeatmapPlayer hPlayer;
	PlayerHealthManager hpManager;

	[Serializable]
	struct DropOffPoint
	{
		public Vector3 position;
		public Quaternion rotation;
		public int playerHealth;
		public string gameFinished;
		public string gotKey;
		public int deathCount;
	}

	[Serializable]
	struct DropOffReq
	{
		public DropOffPoint point;
		public string token;
	}

	void Start () {
		hPlayer = player.GetComponent<HeatmapPlayer>();
		hpManager = player.GetComponent<PlayerHealthManager>();
		playerController = player.GetComponent<PlayerController>();
    }

	DropOffReq getCurentState()
	{
		int health = hpManager.GetHealth();
		string token = PlayerPrefs.GetString("user_token");

		DropOffPoint dropOffPointData = new DropOffPoint();

		dropOffPointData.position = player.transform.position;
		dropOffPointData.rotation = player.transform.rotation;
		dropOffPointData.playerHealth = health;
		dropOffPointData.deathCount = hpManager.DeathCount;
		dropOffPointData.gotKey = playerController.hasKey.ToString().ToUpper();
		dropOffPointData.gameFinished = playerController.gameFinished.ToString().ToUpper();

		DropOffReq reqData = new DropOffReq();

		reqData.token = token;
		reqData.point = dropOffPointData;

		return reqData;
	}

	public void LogOut()
	{
		int gameSessionID = PlayerPrefs.GetInt("user_gameplaySessionID");
		DropOffReq reqData = getCurentState();

		PlayerPrefs.DeleteKey("user_gameplaySessionID");
		PlayerPrefs.DeleteKey("user_token");
		PlayerPrefs.SetInt("user_level", 1);

		hPlayer.api.post("/create/drop_off_entry/" + gameSessionID, JsonUtility.ToJson(reqData)).on("success", LogOffCallback);
	}

	public void QuitGame()
	{
		int gameSessionID = PlayerPrefs.GetInt("user_gameplaySessionID");
		DropOffReq reqData = getCurentState();
		PlayerPrefs.SetInt("user_level", 1);
		PlayerPrefs.DeleteKey("user_token");

		hPlayer.api.post("/create/drop_off_entry/" + gameSessionID, JsonUtility.ToJson(reqData)).on("success", QuitCallback);
	}

	void LogOffCallback(string jsonData)
	{
		SceneManager.LoadScene("Menu");
	}

	void QuitCallback(string jsonData)
	{
		Application.Quit();
	}

}
