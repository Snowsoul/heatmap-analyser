using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public bool disableControls = false;
	public bool hasKey = false;
	public bool isRunning = false;
	public bool gameFinished = false;

	public ThirdPersonCamera cameraController;
	public GameObject Level1;
	public GameObject Level2;

	public GameObject LevelFinishedPanelUI;
	public GameObject ChatBubble;
	public QuestManager questManager;
	Text ChatBubbleText;

	public GameObject Level1Spawn;
	public GameObject Level2Spawn;

	bool ongoingMessage = false;

	HeatmapPlayer hPlayer;
	PlayerHealthManager healthManager;

	public float walkSpeed = 2;
	public float runSpeed = 0;
	public float gravity = -12;
	public float jumpHeight = 1;

	[Range(0, 1)]
	public float airControlPercent;

	public float turnSmoothTime = 0.2f;
	float turnSmoothVelocity;

	public float speedSmoothTime = 0.1f;
	float speedSmoothVelocity;

	float curentSpeed;
	float velocityY;

	Animator animator;
	Transform cameraT;
	CharacterController controller;

	void Start () {
		animator = GetComponent<Animator>();
		cameraT = Camera.main.transform;
		controller = GetComponent<CharacterController>();
		ChatBubbleText = ChatBubble.GetComponentInChildren<Text>();
		StartCoroutine(SayAndHideBubble("I need to get out of this dungeon !"));
		hPlayer = transform.GetComponent<HeatmapPlayer>();
		healthManager = transform.GetComponent<PlayerHealthManager>();
		int lastLevel = PlayerPrefs.GetInt("user_level", 1);

		switch(lastLevel)
		{
			case 1:
				SpawnToLevel1();
				break;

			case 2:
				SpawnToLevel2();
				break;
		}
	}

	public void SpawnToLevel1 ()
	{
		Level2.SetActive(false);
		hPlayer.level = 1;
		transform.position = Level1Spawn.transform.position;
		PlayerPrefs.SetInt("user_level", 1);
	}

	public void SpawnToLevel2()
	{
		Level2.SetActive(true);
		Level1.SetActive(false);

		hPlayer.level = 2;
		transform.position = Level2Spawn.transform.position;
		questManager.ResetObjective();
		healthManager.ResetHealth();
		PlayerPrefs.SetInt("user_level", 2);
	}


	void ShowChatBubble ()
	{
		ChatBubble.SetActive(true);
	}

	void HideChatBubble()
	{
		ChatBubble.SetActive(false);
	}

	void ChatBubbleMessage(string msg)
	{
		ChatBubbleText.text = msg;
	}


	IEnumerator SayAndHideBubble(string msg)
	{
		ChatBubbleMessage(msg);
		ShowChatBubble();
		ongoingMessage = true;

		yield return new WaitForSeconds(3f);

		ongoingMessage = false;
		HideChatBubble();
	}

	IEnumerator LevelFinishedUIPopUp()
	{
		yield return new WaitForSeconds(1f);
		LevelFinishedPanelUI.SetActive(true);
	}

	public void SayMessage(string msg)
	{
		if (!ongoingMessage)
		{
			StartCoroutine(SayAndHideBubble(msg));
		}
    }

	public void LevelFinished()
	{
		cameraController.lockCamera = true;
		cameraController.EnableCursor();
		disableControls = true;

		SayMessage("I shall be free now !");

		StartCoroutine("LevelFinishedUIPopUp");

		gameFinished = true;
	}

	public void emptyChest()
	{
		SayMessage("This chest seems to be empty...");
	}

	public void KeyRecieved ()
	{
		hasKey = true;
		SayMessage("I've finally got the key! I should now head out towards the exit.");
		questManager.nextObjective();
	}

	void Update () {
		if (disableControls)
		{
			animator.SetFloat("speedPercent", 0f);
            return;
		}

		// Input
		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		Vector2 inputDir = input.normalized;

		bool running = Input.GetKey(KeyCode.LeftShift);
		isRunning = running;

		Move(inputDir, running);


		if (Input.GetKeyDown(KeyCode.Space))
		{
			Jump();
		}

		// Animator
		float animationSpeedPercent = ((running) ? curentSpeed / runSpeed : curentSpeed / walkSpeed * .5f);
		animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);


	}


	void Move (Vector2 inputDir, bool running)
	{
		if (inputDir != Vector2.zero)
		{
			float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
			transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
		}


		float targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude;
		curentSpeed = Mathf.SmoothDamp(curentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

		velocityY += Time.deltaTime * gravity;
		Vector3 velocity = transform.forward * curentSpeed + Vector3.up * velocityY;
		controller.Move(velocity * Time.deltaTime);

		curentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

		if (controller.isGrounded)
		{
			velocityY = 0;
		}


	}

	void Jump()
	{
		if (controller.isGrounded)
		{
			float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
			velocityY = jumpVelocity;
		}
	}

	float GetModifiedSmoothTime(float smoothTime)
	{
		if (controller.isGrounded)
		{
			return smoothTime;
		}

		if (airControlPercent == 0)
		{
			return float.MaxValue;
		}

		return smoothTime / airControlPercent;
	}


	public void MainExitGame()
	{

	}
}
