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

	void Start()
	{
		webSocketRouter = GameObject.Find("WebSocketRouter").GetComponent<WebSocketRouter>();
		if (webSocketRouter == null)
		{
			Debug.LogWarning("WebSocketRouter not found in scene. Ensure it is present.");
		}
		BeginGame();
	}

	public void ProcessMessage(string message)
	{
		if (message.StartsWith("Player") && message.Contains("action:") && (message.Contains("move") || message.Contains("aim") || message.Contains("fire")))
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
		Debug.Log("Beginning game");
		Dictionary<int, string> ids = webSocketRouter.connectedIDs;
		Debug.Log(ids.Count);
		int length = ids.Count;
		HashSet<string> usedCombinations = new HashSet<string>();
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
					break;
				}
			}
			string message = $"{uuid} {role}";
			webSocketRouter.SendInput(message);
			
			Debug.Log($"Assigned ID: {id}, UUID: {uuid}, Role: {role}");
		}
		Invoke("speedIncrease", 10);
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
		tankMovement.moveSpeed += 2f;
		tankAiming.projectileSpeed += 20f;
		Invoke("speedIncrease", 10);
	}
}