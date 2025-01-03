using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class KnockoutController : MonoBehaviour
{
    private Vector2 aimDirection;
    private float aimPower;
    private float rotationSpeed = 0.4f;
    private float launchDelay = 1f;

    private bool wasLaunched = false;

    private SpriteRenderer spriteRenderer;
    public Sprite closedSprite;
    public Sprite openSprite;

    public Tilemap tilemap;
    public RuleTile waterTile;
    private CircleCollider2D circleCollider;
    public int thisone;

    private bool isAlive = true;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void Update() {
        if(wasLaunched) {
            spriteRenderer.sprite = openSprite;
            if(GetComponent<Rigidbody2D>().velocity.magnitude == 0) {
                wasLaunched = false;
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
        } else {
            spriteRenderer.sprite = closedSprite;
        }
        if(GetComponent<Rigidbody2D>().velocity.magnitude <= 0.1f) {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        DetectCollision();
    }

    void DetectCollision() {
        foreach(var position in tilemap.cellBounds.allPositionsWithin) {
            if(tilemap.HasTile(position) && tilemap.GetTile(position) == waterTile) {
                float distance = Vector3.Distance(circleCollider.transform.position, tilemap.CellToWorld(position));
                if(distance < circleCollider.radius && isAlive) {
                    Debug.Log("Collision detected");
                    GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    transform.localScale = new Vector3(0.01f, 0.1f, 1f);
                    GameObject.Find("KnockoutGameConnection").GetComponent<KnockoutGameConnection>().PlayerDied(thisone);
                    isAlive = false;
                }
            }
        }
    }

    public void UpdateInput(Vector2 input)
    {
        if(input.magnitude != 0) {
            aimDirection = input;
        }
    }

    public void UpdatePower(float power)
    {
        aimPower = power;
    }

    public void Launch()
    {
        StartCoroutine(RotateAndLaunch());
    }

    private IEnumerator RotateAndLaunch()
    {
        Vector2 currentDirection = transform.up;
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, -aimDirection);

        float timeElapsed = 0f;

        while (timeElapsed < rotationSpeed)
        {
            transform.rotation = Quaternion.Slerp(Quaternion.LookRotation(Vector3.forward, currentDirection), targetRotation, timeElapsed / rotationSpeed);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;

        yield return new WaitForSeconds(launchDelay);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = aimDirection.normalized * aimPower * 0.2f;
            wasLaunched = true;
        }
    }
}
