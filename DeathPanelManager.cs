using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DeathPanelManager : MonoBehaviour {
	
	public void Reload()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void ExitGame()
	{
		Debug.Log("Exit Game");
	}
}
