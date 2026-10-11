
using UnityEngine;

public enum PlantKind
{
    Bamboo,
    Vine,
    Fern
}

public abstract class PlantBase : MonoBehaviour
{
    public abstract PlantKind Kind { get; }

    public int Age { get; private set; }

    public abstract bool IsMature { get; }

    protected BoxCollider2D selectionCollider;

    protected virtual void Awake()
    {
        selectionCollider =
            gameObject.GetComponent<BoxCollider2D>();

        if (selectionCollider == null)
        {
            selectionCollider =
                gameObject.AddComponent<BoxCollider2D>();
        }

        // Этот коллайдер используется для выбора.
        selectionCollider.isTrigger = true;

        CreateVisuals();
        UpdateAppearance();
    }

    protected abstract void CreateVisuals();

    protected abstract void UpdateAppearance();

    public void SetAge(int newAge)
    {
        Age = Mathf.Clamp(newAge, 0, 9);
        UpdateAppearance();
    }

    public void ChangeAge(int amount)
    {
        SetAge(Age + amount);
    }

    protected void ConfigureSelection(
        Vector2 offset,
        Vector2 size)
    {
        selectionCollider.offset = offset;
        selectionCollider.size = size;
    }
}
