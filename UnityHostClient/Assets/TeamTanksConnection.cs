using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TeamTanksConnection : MonoBehaviour
{
	private TankMovement tankMovement;
	private TankAiming tankAiming;
	private WebSocketRouter webSocketRouter;
	Dictionary<int, int> idTeamMap = new Dictionary<int, int>();
	int livingPlayers = 4;
	private List<int> livingPlayersTeam = new List<int> { 1, 2, 3, 4 };
	[SerializeField] private TextMeshProUGUI gameOverText;
	[SerializeField] private TextMeshProUGUI countdownText;
	[SerializeField] private TextMeshProUGUI timerText;
	[SerializeField] private TextMeshProUGUI roundCountdownText;

	private float timeElapsed = 0;

	private bool gameStarted = false;
	public AudioSource audioSource;
	private AudioSource thisAudioSource;

	void Start()
	{
		webSocketRouter = GameObject.Find("WebSocketRouter").GetComponent<WebSocketRouter>();
		if (webSocketRouter == null)
		{
			Debug.LogWarning("WebSocketRouter not found in scene. Ensure it is present.");
		}
		Countdown();
		thisAudioSource = GetComponent<AudioSource>();
		audioSource = GameObject.Find("Canvas").GetComponent<AudioSource>();
	}

	void Countdown() {
		countdownText.text = "5";
		Invoke("Countdown4", 1);
	}

	void Countdown4() {
		countdownText.text = "4";
		Invoke("Countdown3", 1);
	}

	void Countdown3() {
		countdownText.text = "3";
		Invoke("Countdown2", 1);
	}

	void Countdown2() {
		countdownText.text = "2";
		Invoke("Countdown1", 1);
	}

	void Countdown1() {
		countdownText.text = "1";
		Invoke("BeginGame", 1);
	}

	public void ProcessMessage(string message)
	{
		if (gameStarted && message.StartsWith("Player") && message.Contains("action:") && (message.Contains("move") || message.Contains("aim") || message.Contains("fire") || message.Contains("reverse")))
		{
			string[] parts = message.Split(' ');
			if (parts.Length >= 2 && int.TryParse(parts[1], out int playerId))
			{
				if (idTeamMap.TryGetValue(playerId, out int teamNumber))
				{
					GameObject playerTank = GameObject.Find($"TankPlayer{teamNumber}");
					if (playerTank != null)
					{
						tankMovement = playerTank.GetComponent<TankMovement>();
						tankAiming = playerTank.GetComponentInChildren<TankAiming>();
					}
					else
					{
						Debug.LogWarning($"TankPlayer{playerId} not found in the scene.");
						return;
					}
					if (message.Contains("move"))
					{
						string[] moveParts = message.Split(' ');
						if (moveParts.Length >= 6 && float.TryParse(moveParts[4], out float x) && float.TryParse(moveParts[5], out float y))
						{
							Vector2 input = new Vector2(x, y);
							tankMovement.MoveTank(input);
						}
						else
						{
							Debug.LogWarning("Failed to parse move values. Ensure the message format is correct.");
						}
					}
					else if (message.Contains("aim"))
					{
						string[] aimParts = message.Split(' ');
						if (aimParts.Length >= 6 && float.TryParse(aimParts[4], out float x) && float.TryParse(aimParts[5], out float y))
						{
							Vector2 input = new Vector2(x, y);
							tankAiming.AimTank(input);
						}
						else
						{
							Debug.LogWarning("Failed to parse aim values. Ensure the message format is correct.");
						}
					}
					else if (message.Contains("fire"))
					{
						tankAiming.Fire();
					}
					else if (message.Contains("reverse")){
						tankMovement.Reverse();
					}
				}
				else
				{
					Debug.Log(idTeamMap.Count);
					Debug.LogWarning($"Team number not found for player ID: {playerId}");
				}
			}
			else
			{
				Debug.LogWarning("Failed to parse player ID. Ensure the message format is correct.");
			}
		}
	}


	void BeginGame()
	{
		countdownText.text = "";
		thisAudioSource.Stop();
		audioSource.Play();
		Debug.Log("Beginning game");
		Dictionary<int, string> ids = webSocketRouter.connectedIDs;
		Debug.Log(ids.Count);
		int length = ids.Count;
		HashSet<string> usedCombinations = new HashSet<string>();

		Dictionary<int, int> driverIds = new Dictionary<int, int>();
    	Dictionary<int, int> gunnerIds = new Dictionary<int, int>();

		foreach (var KP in ids)
		{
			int id = KP.Key;
			string uuid = KP.Value;
			string role = null;

			while (true)
			{
				int teamNumber = UnityEngine.Random.Range(1, 5);

				role = UnityEngine.Random.Range(0, 2) == 0 ? "d" : "g";

				string combination = $"{teamNumber}{role}";

				if (!usedCombinations.Contains(combination))
				{
					usedCombinations.Add(combination);
					idTeamMap[id] = teamNumber;

					if (role == "d")
					{
						driverIds[teamNumber] = id;
					}
					else
					{
						gunnerIds[teamNumber] = id;
					}

					break;
				}
			}
			string message = $"{uuid} {role}";
			webSocketRouter.SendInput(message);
			
			Debug.Log($"Assigned ID: {id}, UUID: {uuid}, Role: {role}");
		}
		for (int teamNumber = 1; teamNumber <= 4; teamNumber++)
		{
			int driverId = driverIds.ContainsKey(teamNumber) ? driverIds[teamNumber] : -1;
			int gunnerId = gunnerIds.ContainsKey(teamNumber) ? gunnerIds[teamNumber] : -1;

			GameObject tank = GameObject.Find($"TankPlayer{teamNumber}");
			if (tank != null)
			{
				var setLabels = tank.GetComponent<SetLabels>();
				if (setLabels != null)
				{
					setLabels.SetText(driverId, gunnerId);
				}
			}
		}
		gameStarted = true;
		Invoke("PanelOff", 10);
		Invoke("fourteen", 1);
		TimerStart();
	}

	void TimerStart() {
        timeElapsed = 0; 
        timerText.text = "0:00";
        StartCoroutine(TimerIncrement());
    }

    System.Collections.IEnumerator TimerIncrement() {
        while (true) {
            timeElapsed += Time.deltaTime;
            int minutes = Mathf.FloorToInt(timeElapsed / 60);
            int seconds = Mathf.FloorToInt(timeElapsed % 60);
            timerText.text = $"{minutes}:{seconds.ToString("00")}";
            yield return null;
        }
    }

	void PanelOff()
	{
		GameObject.Find("TankPlayer1").GetComponent<SetLabels>().PanelOff();
		GameObject.Find("TankPlayer2").GetComponent<SetLabels>().PanelOff();
		GameObject.Find("TankPlayer3").GetComponent<SetLabels>().PanelOff();
		GameObject.Find("TankPlayer4").GetComponent<SetLabels>().PanelOff();
	}

	public void PlayerDeath(int player) {
		livingPlayers--;
		livingPlayersTeam.Remove(player);
		if (livingPlayers <= 1) {
			processGameOver();
		}
	}

	void processGameOver() {
		int winningTeamID = livingPlayersTeam[0];

		foreach (var id in idTeamMap) {
			if (id.Value == winningTeamID) {
				webSocketRouter.AddScore(id.Key, 1000);
			}
		}
		webSocketRouter.RouteToScene("MainRouter");
	}

	void speedIncrease()
	{
		GameObject.Find("TankPlayer1").GetComponent<TankMovement>().moveSpeed += 2f;
		GameObject.Find("TankPlayer1").GetComponentInChildren<TankAiming>().projectileSpeed += 20f;
	}

	void fifteen()
	{
		speedIncrease();
		roundCountdownText.text = "15";
		Invoke("fourteen", 1);
	}

	void fourteen()
	{
		roundCountdownText.text = "14";
		Invoke("thirteen", 1);
	}

	void thirteen()
	{
		roundCountdownText.text = "13";
		Invoke("twelve", 1);
	}

	void twelve()
	{
		roundCountdownText.text = "12";
		Invoke("eleven", 1);
	}

	void eleven()
	{
		roundCountdownText.text = "11";
		Invoke("ten", 1);
	}

	void ten()
	{
		roundCountdownText.text = "10";
		Invoke("nine", 1);
	}

	void nine()
	{
		roundCountdownText.text = "9";
		Invoke("eight", 1);
	}

	void eight()
	{
		roundCountdownText.text = "8";
		Invoke("seven", 1);
	}

	void seven()
	{
		roundCountdownText.text = "7";
		Invoke("six", 1);
	}

	void six()
	{
		roundCountdownText.text = "6";
		Invoke("five", 1);
	}

	void five()
	{
		roundCountdownText.text = "5";
		Invoke("four", 1);
	}

	void four()
	{
		roundCountdownText.text = "4";
		Invoke("three", 1);
	}

	void three()
	{
		roundCountdownText.text = "3";
		Invoke("two", 1);
	}

	void two()
	{
		roundCountdownText.text = "2";
		Invoke("one", 1);
	}

	void one()
	{
		roundCountdownText.text = "1";
		Invoke("zero", 1);
	}

	void zero()
	{
		roundCountdownText.text = "0";
		Invoke("fifteen", 1);
	}
}