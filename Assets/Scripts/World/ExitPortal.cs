
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ExitPortal : MonoBehaviour
{
    private LevelManager level;

    private void Awake()
    {
        BoxCollider2D collider =
            GetComponent<BoxCollider2D>();

        collider.isTrigger = true;
        collider.size = new Vector2(0.9f, 1.6f);
    }

    private void Start()
    {
        level = FindFirstObjectByType<LevelManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player =
            other.GetComponent<PlayerController>();

        if (player == null || level == null)
            return;

        level.CompleteLevel();
    }
}
