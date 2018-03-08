using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainUIManager : MonoBehaviour {

	public GameObject mainMenu;
	public GameObject player;
	public Text loggedInUser;

	PlayerController playerController;


	// Use this for initialization
	void Start () {
		playerController = player.GetComponent<PlayerController>();
		string savedUser = PlayerPrefs.GetString("user_username", "No logged in user");
        loggedInUser.text = loggedInUser.text.Replace("%username%", savedUser);
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (mainMenu.activeSelf)
			{
				playerController.disableControls = false;
				playerController.cameraController.lockCamera = false;
				playerController.cameraController.DisableCursor();

				mainMenu.SetActive(false);
			} else
			{
				playerController.disableControls = true;
				playerController.cameraController.lockCamera = true;
				playerController.cameraController.EnableCursor();

				mainMenu.SetActive(true);
			}
		}
	}
}
