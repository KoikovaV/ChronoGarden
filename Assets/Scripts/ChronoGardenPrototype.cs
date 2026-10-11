
using System.Collections.Generic;
using UnityEngine;

public class ChronoGardenPrototype : MonoBehaviour
{
    public Font russianFont;
    enum PlantType
    {
        Bamboo,
        Vine,
        Fern
    }

    class Plant
    {
        public PlantType type;
        public Vector2 position;
        public int age;
        public float changedAt;
        public GameObject root;
        public GameObject body;
        public GameObject platform;
    }

    readonly List<Plant> plants = new List<Plant>();

    Sprite squareSprite;
    Camera gameCamera;
    GameObject player;
    Rigidbody2D playerRb;

    Plant selectedPlant;
    PlantType chosenType = PlantType.Bamboo;

    int plantingStep;
    bool won;
    bool touchLeft;
    bool touchRight;
    bool jumpRequested;

    float nextBounceTime;

    string message = "Посадите первый бамбук у края.";

    const float MoveSpeed = 4f;
    const float JumpSpeed = 7f;
    const float BounceSpeed = 12f;

    void Start()
    {
        CreateSprite();
        CreateWorld();
    }

    void CreateSprite()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        squareSprite = Sprite.Create(
            texture,
            new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f),
            1f
        );
    }

    GameObject CreateRectangle(
        string objectName,
        Vector2 position,
        Vector2 size,
        Color color,
        bool solid)
    {
        GameObject obj = new GameObject(objectName);

        obj.transform.position = position;
        obj.transform.localScale =
            new Vector3(size.x, size.y, 1);

        SpriteRenderer renderer =
            obj.AddComponent<SpriteRenderer>();

        renderer.sprite = squareSprite;
        renderer.color = color;

        if (solid)
        {
            obj.AddComponent<BoxCollider2D>();
        }

        return obj;
    }

    void ChangeRectangle(
        GameObject obj,
        Vector2 position,
        Vector2 size,
        Color color)
    {
        if (obj == null)
            return;

        obj.transform.position = position;
        obj.transform.localScale =
            new Vector3(size.x, size.y, 1);

        obj.GetComponent<SpriteRenderer>().color = color;
    }

    void CreateWorld()
    {
        gameCamera = Camera.main;

        if (gameCamera == null)
        {
            GameObject cameraObject =
                new GameObject("Main Camera");

            gameCamera = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
        }

        gameCamera.orthographic = true;
        gameCamera.orthographicSize = 6.3f;
        gameCamera.backgroundColor =
            new Color(0.82f, 0.92f, 0.85f);

        gameCamera.transform.position =
            new Vector3(4, 3, -10);

        // Стартовая платформа.
        CreateRectangle(
            "StartPlatform",
            new Vector2(-2.5f, -0.5f),
            new Vector2(5, 1),
            new Color(0.35f, 0.55f, 0.38f),
            true
        );

        // Финишная платформа.
        CreateRectangle(
            "FinishPlatform",
            new Vector2(10.5f, 2.5f),
            new Vector2(3, 1),
            new Color(0.35f, 0.55f, 0.38f),
            true
        );

        // Финишный маяк.
        CreateRectangle(
            "FinishMarker",
            new Vector2(10.5f, 3.8f),
            new Vector2(0.4f, 1.6f),
            new Color(1f, 0.8f, 0.3f),
            false
        );

        // Персонаж.
        player = CreateRectangle(
            "Player",
            new Vector2(-3, 1),
            new Vector2(0.65f, 1.1f),
            new Color(0.95f, 0.55f, 0.3f),
            true
        );

        playerRb = player.AddComponent<Rigidbody2D>();
        playerRb.gravityScale = 2.3f;
        playerRb.freezeRotation = true;
        playerRb.collisionDetectionMode =
            CollisionDetectionMode2D.Continuous;
        playerRb.interpolation =
            RigidbodyInterpolation2D.Interpolate;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            chosenType = PlantType.Bamboo;

        if (Input.GetKeyDown(KeyCode.Alpha2))
            chosenType = PlantType.Vine;

        if (Input.GetKeyDown(KeyCode.Alpha3))
            chosenType = PlantType.Fern;

        if (Input.GetKeyDown(KeyCode.Q))
            ChangeAge(-1);

        if (Input.GetKeyDown(KeyCode.E))
            ChangeAge(1);

        if (Input.GetKeyDown(KeyCode.R))
            RestartLevel();

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouse = Input.mousePosition;

            // Игнорируем нажатия на верхнюю и нижнюю панели.
            if (mouse.y > 115 &&
                mouse.y < Screen.height - 115)
            {
                Vector3 world =
                    gameCamera.ScreenToWorldPoint(mouse);

                HandleWorldClick(world);
            }
        }

        if (won)
            return;

        if (Input.GetKeyDown(KeyCode.Space) ||
            jumpRequested)
        {
            Jump();
            jumpRequested = false;
        }

        if (Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.UpArrow))
        {
            Climb();
        }

        CheckSpring();
        CheckOldPlants();
        CheckFinish();

        if (player.transform.position.y < -6)
        {
            RespawnPlayer();
        }

        // Камера следит за игроком по горизонтали.
        float targetX = Mathf.Clamp(
            player.transform.position.x + 3f,
            4f,
            7f
        );

        Vector3 cameraPos = gameCamera.transform.position;

        cameraPos.x = Mathf.Lerp(
            cameraPos.x,
            targetX,
            Time.deltaTime * 2f
        );

        gameCamera.transform.position = cameraPos;
    }

    void FixedUpdate()
    {
        if (won)
        {
            playerRb.linearVelocity = Vector2.zero;
            return;
        }

        float horizontal = 0;

        if (Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.LeftArrow) ||
            touchLeft)
        {
            horizontal -= 1;
        }

        if (Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.RightArrow) ||
            touchRight)
        {
            horizontal += 1;
        }

        horizontal = Mathf.Clamp(horizontal, -1, 1);

        Vector2 velocity = playerRb.linearVelocity;
        velocity.x = horizontal * MoveSpeed;

        playerRb.linearVelocity = velocity;
    }

    bool IsGrounded()
    {
        Vector2 feet = (Vector2)player.transform.position
                       + Vector2.down * 0.59f;

        Collider2D[] hits =
            Physics2D.OverlapCircleAll(feet, 0.12f);

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject != player && !hit.isTrigger)
                return true;
        }

        return false;
    }

    void Jump()
    {
        if (!IsGrounded())
            return;

        Vector2 velocity = playerRb.linearVelocity;
        velocity.y = JumpSpeed;
        playerRb.linearVelocity = velocity;
    }

    void Climb()
    {
        foreach (Plant plant in plants)
        {
            if (plant.type != PlantType.Bamboo)
                continue;

            if (plant.age < 3 || plant.age > 5)
                continue;

            Vector2 pos = player.transform.position;

            if (Mathf.Abs(pos.x - plant.position.x) > 0.9f)
                continue;

            if (pos.y < plant.position.y - 0.3f)
                continue;

            float top = plant.position.y + 3f;

            if (pos.y > top + 0.8f)
                continue;

            // Упрощённый подъём для первого прототипа.
            playerRb.position =
                new Vector2(plant.position.x, top + 0.75f);

            playerRb.linearVelocity = Vector2.zero;
            return;
        }
    }

    void HandleWorldClick(Vector2 click)
    {
        if (TryGetPlantingPoint(
            out Vector2 point,
            out PlantType requiredType))
        {
            if (Vector2.Distance(click, point) < 0.8f)
            {
                if (chosenType != requiredType)
                {
                    message = "Выберите нужный вид семян.";
                    return;
                }

                Plant plant = CreatePlant(requiredType, point);
                plants.Add(plant);

                selectedPlant = plant;
                plantingStep++;

                message = "Выделено растение. Измените его возраст.";
                return;
            }
        }

        // Выбор существующего растения.
        for (int i = plants.Count - 1; i >= 0; i--)
        {
            Plant plant = plants[i];
            float x = plant.position.x;
            float y = plant.position.y;

            if (plant.type == PlantType.Bamboo)
            {
                float height = GetBambooHeight(plant.age);

                if (Mathf.Abs(click.x - x) < 0.8f &&
                    click.y >= y - 0.5f &&
                    click.y <= y + height + 0.5f)
                {
                    selectedPlant = plant;
                    return;
                }
            }
            else if (plant.type == PlantType.Vine)
            {
                float length = GetVineLength(plant.age);

                if (click.x >= x - 0.5f &&
                    click.x <= x + length + 0.5f &&
                    Mathf.Abs(click.y - y) < 0.8f)
                {
                    selectedPlant = plant;
                    return;
                }
            }
            else
            {
                if (Vector2.Distance(click, plant.position) < 1f)
                {
                    selectedPlant = plant;
                    return;
                }
            }
        }
    }

    bool TryGetPlantingPoint(
        out Vector2 point,
        out PlantType type)
    {
        point = Vector2.zero;
        type = PlantType.Bamboo;

        if (plantingStep == 0)
        {
            point = new Vector2(0, 0);
            type = PlantType.Bamboo;
            return true;
        }

        if (plantingStep == 1 &&
            plants[0].age >= 3 &&
            plants[0].age <= 5)
        {
            point = new Vector2(0, 3);
            type = PlantType.Vine;
            return true;
        }

        if (plantingStep == 2 &&
            plants[1].age >= 4 &&
            plants[1].age <= 7)
        {
            point = new Vector2(5, 3);
            type = PlantType.Bamboo;
            return true;
        }

        if (plantingStep == 3 &&
            plants[2].age >= 3 &&
            plants[2].age <= 5)
        {
            point = new Vector2(5, 6);
            type = PlantType.Fern;
            return true;
        }

        return false;
    }

    Plant CreatePlant(PlantType type, Vector2 position)
    {
        Plant plant = new Plant();

        plant.type = type;
        plant.position = position;
        plant.age = 0;
        plant.changedAt = Time.time;

        plant.root = new GameObject(type.ToString());

        plant.body = CreateRectangle(
            "PlantBody",
            position,
            new Vector2(0.4f, 1),
            Color.green,
            false
        );

        plant.body.transform.SetParent(plant.root.transform);

        if (type != PlantType.Fern)
        {
            plant.platform = CreateRectangle(
                "PlantPlatform",
                position,
                Vector2.one,
                Color.green,
                true
            );

            plant.platform.transform.SetParent(
                plant.root.transform
            );
        }

        RefreshPlant(plant);
        return plant;
    }

    float GetBambooHeight(int age)
    {
        if (age < 3) return 1;
        if (age < 6) return 3;
        return 5;
    }

    float GetVineLength(int age)
    {
        if (age < 4) return 2;
        if (age < 8) return 5;
        return 7;
    }

    void ChangeAge(int amount)
    {
        if (selectedPlant == null)
            return;

        selectedPlant.age = Mathf.Clamp(
            selectedPlant.age + amount,
            0,
            9
        );

        selectedPlant.changedAt = Time.time;

        RefreshPlant(selectedPlant);
    }

    void RefreshPlant(Plant plant)
    {
        int age = plant.age;
        Vector2 origin = plant.position;

        if (plant.type == PlantType.Bamboo)
        {
            float height = GetBambooHeight(age);

            Color color = age >= 6
                ? new Color(0.6f, 0.5f, 0.3f)
                : new Color(0.2f, 0.65f, 0.35f);

            ChangeRectangle(
                plant.body,
                origin + Vector2.up * (height / 2),
                new Vector2(0.35f, height),
                color
            );

            ChangeRectangle(
                plant.platform,
                origin + Vector2.up * height,
                new Vector2(1.2f, 0.22f),
                color
            );
        }
        else if (plant.type == PlantType.Vine)
        {
            float length = GetVineLength(age);

            Color color = age >= 8
                ? new Color(0.65f, 0.55f, 0.3f)
                : new Color(0.35f, 0.75f, 0.3f);

            ChangeRectangle(
                plant.body,
                origin + Vector2.right * (length / 2),
                new Vector2(length, 0.15f),
                color
            );

            ChangeRectangle(
                plant.platform,
                origin + Vector2.right * (length / 2),
                new Vector2(length, 0.24f),
                color
            );
        }
        else
        {
            Color color = (age >= 3 && age <= 4)
                ? new Color(0.3f, 0.9f, 0.5f)
                : new Color(0.5f, 0.55f, 0.35f);

            ChangeRectangle(
                plant.body,
                origin + Vector2.up * 0.3f,
                new Vector2(1f, 0.6f),
                color
            );

            plant.body.SetActive(age < 5);
        }

        UpdateFragility(plant);
    }

    void UpdateFragility(Plant plant)
    {
        if (plant.platform == null)
            return;

        BoxCollider2D collider =
            plant.platform.GetComponent<BoxCollider2D>();

        bool fragile =
            (plant.type == PlantType.Bamboo && plant.age >= 6) ||
            (plant.type == PlantType.Vine && plant.age >= 8);

        collider.enabled =
            !fragile || Time.time - plant.changedAt < 2f;
    }

    void CheckOldPlants()
    {
        foreach (Plant plant in plants)
        {
            UpdateFragility(plant);
        }
    }

    void CheckSpring()
    {
        if (Time.time < nextBounceTime)
            return;

        foreach (Plant plant in plants)
        {
            if (plant.type != PlantType.Fern)
                continue;

            if (plant.age < 3 || plant.age > 4)
                continue;

            Vector2 pos = player.transform.position;

            if (Mathf.Abs(pos.x - plant.position.x) > 0.65f)
                continue;

            if (pos.y < plant.position.y + 0.25f ||
                pos.y > plant.position.y + 1.3f)
                continue;

            if (playerRb.linearVelocity.y > 0.1f)
                continue;

            Vector2 velocity = playerRb.linearVelocity;
            velocity.y = BounceSpeed;
            playerRb.linearVelocity = velocity;

            nextBounceTime = Time.time + 0.8f;
            message = "Пружина! Двигайтесь вправо к выходу.";
            return;
        }
    }

    void CheckFinish()
    {
        Vector2 pos = player.transform.position;

        if (pos.x >= 9.4f &&
            pos.y >= 3.45f &&
            pos.y <= 4.4f)
        {
            won = true;
            message = "Уровень пройден!";

            PlayerPrefs.SetInt(
                "ChronoGarden_Level05",
                1
            );

            PlayerPrefs.Save();
        }
    }

    void RespawnPlayer()
    {
        playerRb.position = new Vector2(-3, 1);
        playerRb.linearVelocity = Vector2.zero;

        message = "Вы упали. Попробуйте снова.";
    }

    void RestartLevel()
    {
        foreach (Plant plant in plants)
        {
            Destroy(plant.root);
        }

        plants.Clear();

        selectedPlant = null;
        chosenType = PlantType.Bamboo;

        plantingStep = 0;
        won = false;
        nextBounceTime = 0;

        RespawnPlayer();
        message = "Уровень сброшен. Посадите бамбук.";
    }

    void OnGUI()
    {
        if (russianFont != null)
        {
            GUI.skin.font = russianFont;
            GUI.skin.button.font = russianFont;
            GUI.skin.label.font = russianFont;
            GUI.skin.box.font = russianFont;
        }

        GUI.skin.button.fontSize = 14;
        GUI.skin.label.fontSize = 14;

        float buttonWidth = (Screen.width - 20f) / 6f;

        GUI.Box(new Rect(5, 5, Screen.width - 10, 108), "");

        string[] labels =
        {
            "Б", "Л", "П", "-1", "+1", "Сброс"
        };

        for (int i = 0; i < labels.Length; i++)
        {
            if (GUI.Button(
                new Rect(10 + i * buttonWidth, 10,
                         buttonWidth - 3, 35),
                labels[i]))
            {
                if (i == 0) chosenType = PlantType.Bamboo;
                if (i == 1) chosenType = PlantType.Vine;
                if (i == 2) chosenType = PlantType.Fern;
                if (i == 3) ChangeAge(-1);
                if (i == 4) ChangeAge(1);
                if (i == 5) RestartLevel();
            }
        }

        string selectedName = selectedPlant == null
            ? "Нет"
            : selectedPlant.type.ToString();

        string ageText = selectedPlant == null
            ? "-"
            : selectedPlant.age.ToString();

        GUI.Label(
            new Rect(15, 48, Screen.width - 30, 25),
            "Растение: " + selectedName +
            " | Возраст: " + ageText + " сек."
        );

        GUI.Label(
            new Rect(15, 75, Screen.width - 30, 30),
            message
        );

        // Подсказка над доступным местом посадки.
        if (TryGetPlantingPoint(
            out Vector2 point,
            out PlantType requiredType))
        {
            Vector3 screen =
                gameCamera.WorldToScreenPoint(point);

            GUI.Box(
                new Rect(
                    screen.x - 40,
                    Screen.height - screen.y - 43,
                    80,
                    30
                ),
                "Посадка"
            );
        }

        // Сенсорные кнопки.
        float bottom = Screen.height - 83;

        touchLeft = GUI.RepeatButton(
            new Rect(10, bottom, 75, 65),
            "Влево"
        );

        touchRight = GUI.RepeatButton(
            new Rect(90, bottom, 75, 65),
            "Вправо"
        );

        if (GUI.Button(
            new Rect(Screen.width - 170, bottom, 78, 65),
            "Прыжок"))
        {
            jumpRequested = true;
        }

        if (GUI.Button(
            new Rect(Screen.width - 85, bottom, 75, 65),
            "Вверх"))
        {
            Climb();
        }

        if (won)
        {
            GUI.Box(
                new Rect(
                    Screen.width / 2f - 140,
                    Screen.height / 2f - 55,
                    280,
                    110
                ),
                "УРОВЕНЬ ПРОЙДЕН!"
            );
        }
    }
}
