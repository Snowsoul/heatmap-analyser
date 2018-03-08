using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class PlayerHealthManager : MonoBehaviour {

	public Slider HealthSlider;
	public Text HealthTextCurrent;
	public Text HealthTextMax;
	public GameObject DeathPanel;
	PlayerController playerController;
	public APIManager api;
	public int DeathCount = 0;

	[Serializable]
	struct DeathPoint
	{
		public Vector3 position;
		public Quaternion rotation;
		public string reason;
	}

	[Serializable]
	struct HealthLossPoint
	{
		public Vector3 position;
		public Quaternion rotation;
		public int amount;
		public int playerHealth;
		public string reason;
	}


	[Serializable]
	struct HealthLossReq
	{
		public HealthLossPoint point;
		public string token;
	}

	[Serializable]
	struct DeathPointReq
	{
		public DeathPoint point;
		public string token;
	}

	int Health = 100;
	public bool dead = false;

	void Start ()
	{
		HealthTextMax.text = Health.ToString();
		HealthTextCurrent.text = Health.ToString();
		ChangeHealthSliderValue(Health);
		playerController = transform.GetComponent<PlayerController>();
	}

	public void ResetHealth()
	{
		Health = 100;
		ChangeHealthText(Health.ToString());
		ChangeHealthSliderValue(Health);
	}

	void SetHealth(int amount, string reason = "NONE")
	{
		if (dead)
		{
			return;
		}

		if (amount <= 0 && !dead)
		{
			amount = 0;
			Health = amount;
			ChangeHealthText(Health.ToString());
			ChangeHealthSliderValue(Health);

			KillPlayer(reason);
			return;
		}

		Health = amount;

		ChangeHealthSliderValue(Health);
		ChangeHealthText(Health.ToString());
    }

	void ChangeHealthSliderValue(int value)
	{
		HealthSlider.value = (float) value / 100;
	}

	void ChangeHealthText(string value)
	{
		HealthTextCurrent.text = value;
	}

	public void ShowDeathPanel ()
	{
		DeathPanel.SetActive(true);
    }

	public void HideDeathPanel()
	{
		DeathPanel.SetActive(false);
	}

	void KillPlayer(string reason = "NONE")
	{
		// Trigger Death Animation

		// Trigger Death UI
		ShowDeathPanel();

		// Trigger Death Sound

		// Trigger Death Analytics at the curent position to DB
		int gameSessionID = PlayerPrefs.GetInt("user_gameplaySessionID");
		string token = PlayerPrefs.GetString("user_token");

		DeathPoint dPointData = new DeathPoint();

		dPointData.position = transform.position;
		dPointData.rotation = transform.rotation;
		dPointData.reason = reason;

		DeathPointReq reqData = new DeathPointReq();
		reqData.token = token;
		reqData.point = dPointData;


		api.post("/create/death_point_entry/" + gameSessionID.ToString(), JsonUtility.ToJson(reqData));


		// Disable movement
		playerController.disableControls = true;
		playerController.cameraController.lockCamera = true;
		playerController.cameraController.EnableCursor();

		dead = true;
		DeathCount += 1;
	}

	public int GetHealth()
	{
		return Health;
	}

	public void RemoveHealth(int amount, string reason = "NONE")
	{
		int health = GetHealth();
		int gameSessionID = PlayerPrefs.GetInt("user_gameplaySessionID");
		string token = PlayerPrefs.GetString("user_token");

		HealthLossPoint hPointData = new HealthLossPoint();

		hPointData.position = transform.position;
		hPointData.rotation = transform.rotation;
		hPointData.amount = amount;
		hPointData.playerHealth = health;
		hPointData.reason = reason;

		HealthLossReq reqData = new HealthLossReq();
		reqData.token = token;
		reqData.point = hPointData;

		api.post("/create/health_point_entry/" + gameSessionID.ToString(), JsonUtility.ToJson(reqData));

		SetHealth(health - amount, reason);
	}

	public void InstantKill(string reason = "NONE")
	{
		SetHealth(0, reason);
	}
}
