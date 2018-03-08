using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class HeatmapViewerUIManager : MonoBehaviour {

	public APIManager api;
	public HeatmapAnalyser analyser;

	public Dropdown userDropdown;
	public Dropdown loginSessionDropdown;
	public Dropdown gameSessionDropdown;
	public Dropdown heatMapTypeDropdown;

	public Button ShowHeatMapBtn;
	public PlayerController playerController;

	int heatmapType = 0;

	UserData dbData;

	[Serializable]
	public struct GameSession
	{
		public int id;
		public string startDate;
	}

	[Serializable]
	public struct LoginSessionRes
	{
		public int id;
		public string date;
		public List<GameSession> game_sessions;
	}

	[Serializable]
	public struct HeatmapUser
	{
		public int id;
		public string username;
		public List<LoginSessionRes> login_sessions;
	}

	[Serializable]
	public struct UserData
	{
		public List<HeatmapUser> users;
    } 

	public void Show()
	{
		playerController.disableControls = true;
		playerController.cameraController.lockCamera = true;
		playerController.cameraController.EnableCursor();
		heatMapTypeDropdown.value = -1;
		heatMapTypeDropdown.value = 0;
		transform.gameObject.SetActive(true);
		LoadUserData();
	}

	public void Hide()
	{
		userDropdown.options.Clear();
		loginSessionDropdown.options.Clear();
		gameSessionDropdown.options.Clear();
		dbData.users.Clear();

		playerController.disableControls = false;
		playerController.cameraController.lockCamera = false;
		playerController.cameraController.DisableCursor();

		transform.gameObject.SetActive(false);
	}

	void LoadUserData()
	{
		api.get("/get/users/").on("success", UserDataLoaded);
	}

	void UserDataLoaded(string jsonData)
	{
		UserData data = JsonUtility.FromJson<UserData>(jsonData);
		dbData = data;

		foreach(HeatmapUser user in data.users)
		{
			userDropdown.options.Add(new Dropdown.OptionData(user.username));
		}

		userDropdown.value = -1;
		userDropdown.value = 0;
	}


	public void UserChanged ()
	{
		if (userDropdown.value != -1)
		{
			loginSessionDropdown.options.Clear();
			loginSessionDropdown.interactable = true;

			foreach (LoginSessionRes logSession in dbData.users[userDropdown.value].login_sessions)
			{
				loginSessionDropdown.options.Add(new Dropdown.OptionData("Login Session " + logSession.id + " at " + logSession.date));
			}

			loginSessionDropdown.value = -1;
			loginSessionDropdown.value = 0;
		}
	}

	public void LoginSessionChanged()
	{
		if (loginSessionDropdown.value != -1)
		{
			gameSessionDropdown.options.Clear();
			gameSessionDropdown.interactable = true;

			foreach (GameSession gameSession in dbData.users[userDropdown.value].login_sessions[loginSessionDropdown.value].game_sessions)
			{
				gameSessionDropdown.options.Add(new Dropdown.OptionData("Game Session " + gameSession.id + " at " + gameSession.startDate));
			}

			gameSessionDropdown.value = -1;
			gameSessionDropdown.value = 0;
		}
	}

	public void GameSessionChanged()
	{
		ShowHeatMapBtn.interactable = true;
	}

	public void HeatMapTypeChanged()
	{
		if (heatMapTypeDropdown.value != -1)
		{
			heatmapType = heatMapTypeDropdown.value;
		}
	}

	public void ShowHeatMap()
	{

		int gameSessionID = dbData.users[userDropdown.value].login_sessions[loginSessionDropdown.value].game_sessions[gameSessionDropdown.value].id;

		if (heatmapType == 0)
		{
			// Overall map
			Debug.Log("Show overall heatmap");
			analyser.ShowOverallMapFromDB(gameSessionID);
			playerController.cameraController.gameObject.GetComponent<HeatmapTopDownCamera>().ClearFloors();
			Hide();
			return;
		}

		playerController.cameraController.gameObject.GetComponent<HeatmapTopDownCamera>().ClearFloors();
		analyser.ShowHeatMapFromDB(gameSessionID, heatmapType - 1);
		Hide();
	}
}
