using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfBallController : MonoBehaviour
{
    public float power = 100f;
    public float maxPower = 100f;
    public Rigidbody2D rb;
    private bool isShooting = false;
    private Vector2 shotDirection;
    public Transform pivot;
    public GameObject pointer;
    public int thisBall = 1;

    public bool inHole = false;
    public bool changing = false;

    bool invokedCalled = false;
    public AudioSource audioSource;
    public AudioClip[] audioClips;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if(GameObject.Find("GolfGameConnection").GetComponent<GolfGameConnection>().activeBall == thisBall)
        {
            pointer.SetActive(true);
        }
        if (audioSource == null) {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (isShooting)
        {
            isShooting = false;
        }
        pivot.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(shotDirection.y, shotDirection.x) * Mathf.Rad2Deg - 90);
        pivot.localScale = new Vector3(shotDirection.magnitude * 3, shotDirection.magnitude * 3, 1);
        
        if(rb.velocity.magnitude <= 0.1f && rb.velocity.magnitude >= -0.1f && !inHole && GameObject.Find("GolfGameConnection").GetComponent<GolfGameConnection>().activeBall == thisBall)
        {
            rb.velocity = Vector2.zero;
            if(GameObject.Find("GolfGameConnection").GetComponent<GolfGameConnection>().activeBall == thisBall)
            {
                PointerActivate();
            }
        }
        else
        {
            pointer.SetActive(false);
        }

        if (rb.velocity.magnitude > 100f)
        {
            rb.velocity = rb.velocity.normalized * 100f;
        }
    }

    void PointerActivate()
    {
        pointer.SetActive(true);
        invokedCalled = false;
    }

    public void UpdateInput(Vector2 input)
    {
        changing = true;
        if(input == Vector2.zero)
        {
            changing = false;
            return;
        }
        shotDirection = input;
    }

    public void ShootBall()
    {
        rb.AddForce(shotDirection * power * shotDirection.magnitude * 2, ForceMode2D.Impulse);
        isShooting = true;
        audioSource.clip = audioClips[0];
        audioSource.Play();
        Debug.Log("Ball shot");
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Hole")) {
            inHole = true;
            audioSource.clip = audioClips[1];
            audioSource.Play();
            StartCoroutine(AnimateBallToHole(other.transform.position));
        }
    }

    private IEnumerator AnimateBallToHole(Vector3 holePosition)
    {
        while (Vector3.Distance(transform.position, holePosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, holePosition, 5 * Time.deltaTime);
            yield return null;
        }
        transform.position = holePosition;
        rb.velocity = Vector2.zero;

        while (transform.localScale.x > 0.1f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 5 * Time.deltaTime);
            yield return null;
        }

        GetComponent<SpriteRenderer>().enabled = false;
        Debug.Log("Ball has been removed after shrinking.");
        GameObject.Find("GolfGameConnection").GetComponent<GolfGameConnection>().BallInHole(thisBall);
    }
}
