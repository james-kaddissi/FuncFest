using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GolfGameConnection : MonoBehaviour
{
    private WebSocketRouter webSocketRouter;
    [SerializeField] private GolfBallController golfController;
    public int activeBall = 1;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI strokesText;
    private int thisHole = 1;

    public GameObject hole2;
    public GameObject hole3;

    private int p1strokes = 0;
    private int p2strokes = 0;
    private int p3strokes = 0;
    private int p4strokes = 0;
    private int p5strokes = 0;
    private int p6strokes = 0;
    private int p7strokes = 0;
    private int p8strokes = 0;
    private int p1totalstrokes = 0;
    private int p2totalstrokes = 0;
    private int p3totalstrokes = 0;
    private int p4totalstrokes = 0;
    private int p5totalstrokes = 0;
    private int p6totalstrokes = 0;
    private int p7totalstrokes = 0;
    private int p8totalstrokes = 0;
    private bool bypassPlayer1 = false;
    private bool bypassPlayer2 = false;
    private bool bypassPlayer3 = false;
    private bool bypassPlayer4 = false;
    private bool bypassPlayer5 = false;
    private bool bypassPlayer6 = false;
    private bool bypassPlayer7 = false;
    private bool bypassPlayer8 = false;

    public Transform hole1center;
    public Transform hole2center;
    public Transform hole3center;

    private Vector3 cameraTargetPosition;
    private int ballPass;

    void Start() {
        webSocketRouter = GameObject.Find("WebSocketRouter").GetComponent<WebSocketRouter>();
        if (webSocketRouter == null)
        {
            Debug.LogWarning("WebSocketRouter not found in scene. Ensure it is present.");
        }
        cameraTargetPosition = hole1center.transform.position + new Vector3(0, 0, -15);
        BeginGame();
        bypassPlayer2 = true;
        bypassPlayer3 = true;
        bypassPlayer4 = true;
        bypassPlayer5 = true;
        bypassPlayer6 = true;
        bypassPlayer7 = true;
        bypassPlayer8 = true;
    }

    public void ProcessMessage(string message) {
        Debug.LogError(message);
        if (message.StartsWith("Player") && message.Contains("action:") && (message.Contains("aim") || message.Contains("shoot") ))
        {
            string[] parts = message.Split(' ');
            if (parts.Length >= 2 && int.TryParse(parts[1], out int playerId))
            {
                if(playerId == activeBall) {
                    golfController = GameObject.Find("Player" + playerId).GetComponent<GolfBallController>();
                    if(message.Contains("aim"))
                    {
                        string[] aimParts = message.Split(' ');
                        if (aimParts.Length >= 6 && float.TryParse(aimParts[4], out float x) && float.TryParse(aimParts[5], out float y))
                        {
                            Vector2 input = new Vector2(x, y);
                            golfController.UpdateInput(input);
                        }
                        else
                        {
                            Debug.LogWarning("Failed to parse aim values. Ensure the message format is correct.");
                        }
                    }
                    if(message.Contains("shoot"))
                    {
                        golfController.ShootBall();
                        if (playerId == 1) {
                            p1strokes++;
                            SendBall(2);
                        } else if (playerId == 2) {
                            p2strokes++;
                            SendBall(3);
                        } else if (playerId == 3) {
                            p3strokes++;
                            SendBall(4);
                        } else if (playerId == 4) {
                            p4strokes++;
                            SendBall(5);
                        } else if (playerId == 5) {
                            p5strokes++;
                            SendBall(6);
                        } else if (playerId == 6) {
                            p6strokes++;
                            SendBall(7);
                        } else if (playerId == 7) {
                            p7strokes++;
                            SendBall(8);
                        } else if (playerId == 8) {
                            p8strokes++;
                            SendBall(1);
                        }
                    }
                }
            }
        }
    }

    void BeginGame() {
        activeBall = 0;
        Countdown();
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
		Invoke("GameStart", 1);
	}
    
    void GameStart() {
        countdownText.text = "";
        activeBall = 1;
        cameraTargetPosition = GameObject.Find("Player" + activeBall).transform.position + new Vector3(0, 0, -10);
        GetComponent<AudioSource>().Stop();
    }

    void SendBall(int ball) {
        if(thisHole == 1){
            cameraTargetPosition = hole1center.position + new Vector3(0, 0, -15);
        } else if(thisHole == 2){
            cameraTargetPosition = hole2center.position + new Vector3(0, 0, -15);
        } else if(thisHole == 3){
            cameraTargetPosition = hole3center.position + new Vector3(0, 0, -25);
        }
        StartCoroutine(WaitForBallToStop(ball));
    }

    IEnumerator WaitForBallToStop(int ball) {
        Rigidbody2D rb = GameObject.Find("Player" + activeBall).GetComponent<GolfBallController>().rb;

        while (rb.velocity.magnitude > 0.01f) {
            yield return null;
        }
        ballPass = ball;
        Invoke("PassBall", 2f);
    }

    void PassBall() {
        if(activeBall != 0) {
            if(bypassPlayer1 && ballPass == 1) {
                ballPass = 2;
                PassBall();
            } else if(bypassPlayer2 && ballPass == 2) {
                ballPass = 3;
                PassBall();
            } else if(bypassPlayer3 && ballPass == 3) {
                ballPass = 4;
                PassBall();
            } else if(bypassPlayer4 && ballPass == 4) {
                ballPass = 5;
                PassBall();
            } else if(bypassPlayer5 && ballPass == 5) {
                ballPass = 6;
                PassBall();
            } else if(bypassPlayer6 && ballPass == 6) {
                ballPass = 7;
                PassBall();
            } else if(bypassPlayer7 && ballPass == 7) {
                ballPass = 8;
                PassBall();
            } else if(bypassPlayer8 && ballPass == 8) {
                ballPass = 1;
                PassBall();
            } else {
                activeBall = ballPass;
                cameraTargetPosition = GameObject.Find("Player" + activeBall).transform.position + new Vector3(0, 0, -10);
            }
        }
    }

    public void BallInHole(int ball) {
        if (ball == 1) {
            p1totalstrokes += p1strokes;
            bypassPlayer1 = true;
        } else if (ball == 2) {
            p2totalstrokes += p2strokes;
            bypassPlayer2 = true;
        } else if (ball == 3) {
            p3totalstrokes += p3strokes;
            bypassPlayer3 = true;
        } else if (ball == 4) {
            p4totalstrokes += p4strokes;
            bypassPlayer4 = true;
        } else if (ball == 5) {
            p5totalstrokes += p5strokes;
            bypassPlayer5 = true;
        } else if (ball == 6) {
            p6totalstrokes += p6strokes;
            bypassPlayer6 = true;
        } else if (ball == 7) {
            p7totalstrokes += p7strokes;
            bypassPlayer7 = true;
        } else if (ball == 8) {
            p8totalstrokes += p8strokes;
            bypassPlayer8 = true;
        }
    }


    void Update()
    {
        if (activeBall == 1)
        {
            strokesText.text = "Player 1 strokes: " + p1strokes;
        }
        else if (activeBall == 2)
        {
            strokesText.text = "Player 2 strokes: " + p2strokes;
        }
        else if (activeBall == 3)
        {
            strokesText.text = "Player 3 strokes: " + p3strokes;
        }
        else if (activeBall == 4)
        {
            strokesText.text = "Player 4 strokes: " + p4strokes;
        }
        else if (activeBall == 5)
        {
            strokesText.text = "Player 5 strokes: " + p5strokes;
        }
        else if (activeBall == 6)
        {
            strokesText.text = "Player 6 strokes: " + p6strokes;
        }
        else if (activeBall == 7)
        {
            strokesText.text = "Player 7 strokes: " + p7strokes;
        }
        else if (activeBall == 8)
        {
            strokesText.text = "Player 8 strokes: " + p8strokes;
        }
        if(bypassPlayer1 && bypassPlayer2 && bypassPlayer3 && bypassPlayer4 && bypassPlayer5 && bypassPlayer6 && bypassPlayer7 && bypassPlayer8){
            NextHole();
        }
        SmoothCamera();
    }

    void NextHole() {
        thisHole++;
        activeBall = 0;
        GameObject.Find("Player1").GetComponent<SpriteRenderer>().enabled = true;
        GameObject.Find("Player2").GetComponent<SpriteRenderer>().enabled = true;
        GameObject.Find("Player3").GetComponent<SpriteRenderer>().enabled = true;
        GameObject.Find("Player4").GetComponent<SpriteRenderer>().enabled = true;
        GameObject.Find("Player5").GetComponent<SpriteRenderer>().enabled = true;
        GameObject.Find("Player6").GetComponent<SpriteRenderer>().enabled = true;
        GameObject.Find("Player7").GetComponent<SpriteRenderer>().enabled = true;
        GameObject.Find("Player8").GetComponent<SpriteRenderer>().enabled = true;
        GameObject.Find("Player1").transform.localScale = new Vector3(1, 1, 1);
        GameObject.Find("Player2").transform.localScale = new Vector3(1, 1, 1);
        GameObject.Find("Player3").transform.localScale = new Vector3(1, 1, 1);
        GameObject.Find("Player4").transform.localScale = new Vector3(1, 1, 1);
        GameObject.Find("Player5").transform.localScale = new Vector3(1, 1, 1);
        GameObject.Find("Player6").transform.localScale = new Vector3(1, 1, 1);
        GameObject.Find("Player7").transform.localScale = new Vector3(1, 1, 1);
        GameObject.Find("Player8").transform.localScale = new Vector3(1, 1, 1);
        if (thisHole == 2) {
            cameraTargetPosition = hole2center.transform.position + new Vector3(0, 0, -15);
            GameObject.Find("Player1").transform.position = hole2.transform.position;
            GameObject.Find("Player2").transform.position = hole2.transform.position;
            GameObject.Find("Player3").transform.position = hole2.transform.position;
            GameObject.Find("Player4").transform.position = hole2.transform.position;
            GameObject.Find("Player5").transform.position = hole2.transform.position;
            GameObject.Find("Player6").transform.position = hole2.transform.position;
            GameObject.Find("Player7").transform.position = hole2.transform.position;
            GameObject.Find("Player8").transform.position = hole2.transform.position;
        }
        if (thisHole == 3) {
            cameraTargetPosition = hole3center.transform.position + new Vector3(0, 0, -25);
            GameObject.Find("Player1").transform.position = hole3.transform.position;
            GameObject.Find("Player2").transform.position = hole3.transform.position;
            GameObject.Find("Player3").transform.position = hole3.transform.position;
            GameObject.Find("Player4").transform.position = hole3.transform.position;
            GameObject.Find("Player5").transform.position = hole3.transform.position;
            GameObject.Find("Player6").transform.position = hole3.transform.position;
            GameObject.Find("Player7").transform.position = hole3.transform.position;
            GameObject.Find("Player8").transform.position = hole3.transform.position;
        }
        
        bypassPlayer1 = false;
        // bypassPlayer2 = false;
        // bypassPlayer3 = false;
        // bypassPlayer4 = false;
        // bypassPlayer5 = false;
        // bypassPlayer6 = false;
        // bypassPlayer7 = false;
        // bypassPlayer8 = false;
        p1strokes = 0;
        p2strokes = 0;
        p3strokes = 0;
        p4strokes = 0;
        p5strokes = 0;
        p6strokes = 0;
        p7strokes = 0;
        p8strokes = 0;
        Invoke("StartHole", 5f);
    }

    void StartHole()
    {
        GameObject.Find("Player1").GetComponent<GolfBallController>().inHole = false;
        GameObject.Find("Player2").GetComponent<GolfBallController>().inHole = false;   
        GameObject.Find("Player3").GetComponent<GolfBallController>().inHole = false;
        GameObject.Find("Player4").GetComponent<GolfBallController>().inHole = false;
        GameObject.Find("Player5").GetComponent<GolfBallController>().inHole = false;
        GameObject.Find("Player6").GetComponent<GolfBallController>().inHole = false;
        GameObject.Find("Player7").GetComponent<GolfBallController>().inHole = false;
        GameObject.Find("Player8").GetComponent<GolfBallController>().inHole = false;
        activeBall = 1;
        cameraTargetPosition = GameObject.Find("Player" + activeBall).transform.position + new Vector3(0, 0, -10);
    }

    void SmoothCamera() 
    {
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraTargetPosition, 3f * Time.deltaTime);
    }
}