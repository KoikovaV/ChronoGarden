
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Движение")]
    public float moveSpeed = 4.5f;
    public float jumpSpeed = 8f;

    private Rigidbody2D rb;
    private BoxCollider2D playerCollider;

    private LevelManager level;
    private Vector2 spawnPosition;

    private bool touchLeft;
    private bool touchRight;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();

        rb.gravityScale = 2.5f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode =
            CollisionDetectionMode2D.Continuous;
        rb.interpolation =
            RigidbodyInterpolation2D.Interpolate;

        playerCollider.size =
            new Vector2(0.65f, 1.1f);

        // Временная графика, если нет спрайта.
        if (GetComponent<SpriteRenderer>() == null)
        {
            ObjectFactory.CreateRectangle(
                "Персонаж",
                transform,
                Vector2.zero,
                new Vector2(0.65f, 1.1f),
                new Color(0.95f, 0.55f, 0.3f),
                false,
                10
            );
        }

        spawnPosition = transform.position;
    }

    private void Start()
    {
        level = FindFirstObjectByType<LevelManager>();
    }

    private void Update()
    {
        if (level != null && level.IsCompleted)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        if (Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.UpArrow))
        {
            TryClimb();
        }
    }

    private void FixedUpdate()
    {
        if (level != null && level.IsCompleted)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float horizontal = 0f;

        if (Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.LeftArrow) ||
            touchLeft)
        {
            horizontal -= 1f;
        }

        if (Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.RightArrow) ||
            touchRight)
        {
            horizontal += 1f;
        }

        horizontal = Mathf.Clamp(horizontal, -1f, 1f);

        rb.linearVelocity = new Vector2(
            horizontal * moveSpeed,
            rb.linearVelocity.y
        );
    }

    public void SetTouchMovement(
        bool left,
        bool right)
    {
        touchLeft = left;
        touchRight = right;
    }

    public void Jump()
    {
        if (level != null && level.IsCompleted)
            return;

        if (!IsGrounded())
            return;

        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            jumpSpeed
        );
    }

    public bool IsGrounded()
    {
        Bounds bounds = playerCollider.bounds;

        Vector2 feet = new Vector2(
            bounds.center.x,
            bounds.min.y - 0.06f
        );

        Collider2D[] hits =
            Physics2D.OverlapCircleAll(feet, 0.13f);

        foreach (Collider2D hit in hits)
        {
            if (hit == playerCollider)
                continue;

            if (hit.isTrigger)
                continue;

            return true;
        }

        return false;
    }

    public void TryClimb()
    {
        if (level != null && level.IsCompleted)
            return;

        BambooPlant[] bambooPlants =
            FindObjectsByType<BambooPlant>(
                FindObjectsSortMode.None
            );

        foreach (BambooPlant bamboo in bambooPlants)
        {
            if (!bamboo.IsMature)
                continue;

            float distanceX = Mathf.Abs(
                transform.position.x -
                bamboo.transform.position.x
            );

            float bottomY =
                bamboo.transform.position.y - 1f;

            float topY =
                bamboo.transform.position.y +
                bamboo.Height + 1f;

            if (distanceX > 1.2f)
                continue;

            if (transform.position.y < bottomY ||
                transform.position.y > topY)
            {
                continue;
            }

            rb.position = new Vector2(
                bamboo.transform.position.x,
                bamboo.transform.position.y +
                bamboo.Height + 0.7f
            );

            rb.linearVelocity = Vector2.zero;

            if (level != null &&
                level.Tutorial != null)
            {
                level.Tutorial.OnPlayerClimbed();
            }

            return;
        }
    }

    public void Respawn()
    {
        rb.position = spawnPosition;
        rb.linearVelocity = Vector2.zero;
    }

    public void FreezeAtFinish()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }
}
