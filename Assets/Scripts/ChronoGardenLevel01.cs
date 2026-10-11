
using UnityEngine;

public class ChronoGardenLevel01 : MonoBehaviour
{
    [Header("Шрифт")]
    public Font russianFont;

    [Header("Графика (необязательно)")]
    public Sprite playerSprite;
    public Sprite portalSprite;
    public Sprite backgroundSprite;

    // Этапы обучения
    enum TutorialStep
    {
        Move,
        ChooseSeed,
        Plant,
        Grow,
        Climb,
        ReachPortal,
        Complete
    }

    TutorialStep step = TutorialStep.Move;

    // Игровые объекты
    GameObject player;
    GameObject bambooStem;
    GameObject bambooTop;
    GameObject bambooRoot;

    Rigidbody2D playerRb;
    BoxCollider2D bambooCollider;
    Camera gameCamera;
    Sprite squareSprite;

    // Настройки персонажа
    const float moveSpeed = 4.5f;
    const float jumpSpeed = 8f;
    const float bambooX = -0.7f;

    // Состояние растения
    int bambooAge = 0;
    bool bambooPlanted = false;
    bool bambooSelected = false;

    // Экранные кнопки
    bool moveLeft;
    bool moveRight;
    bool jumpPressed;
    bool climbPressed;

    string message = "";

    void Start()
    {
        CreateSquareSprite();
        CreateLevel();

        SetStep(TutorialStep.Move);
    }

    // --------------------------------
    // СОЗДАНИЕ УРОВНЯ
    // --------------------------------

    void CreateSquareSprite()
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

    GameObject CreateObject(
        string objectName,
        Vector2 position,
        Vector2 size,
        Color color,
        bool hasCollider,
        int sortingOrder,
        Sprite customSprite = null)
    {
        GameObject obj = new GameObject(objectName);

        SpriteRenderer renderer =
            obj.AddComponent<SpriteRenderer>();

        Sprite sprite = customSprite != null
            ? customSprite
            : squareSprite;

        renderer.sprite = sprite;
        renderer.color = color;
        renderer.sortingOrder = sortingOrder;

        obj.transform.position = position;

        // Подгоняем графику под нужный размер.
        obj.transform.localScale = new Vector3(
            size.x / sprite.bounds.size.x,
            size.y / sprite.bounds.size.y,
            1
        );

        if (hasCollider)
        {
            BoxCollider2D collider =
                obj.AddComponent<BoxCollider2D>();

            collider.size = sprite.bounds.size;
        }

        return obj;
    }

    void CreateLevel()
    {
        // Камера
        gameCamera = Camera.main;

        if (gameCamera == null)
        {
            GameObject cameraObject =
                new GameObject("Main Camera");

            cameraObject.tag = "MainCamera";

            gameCamera =
                cameraObject.AddComponent<Camera>();
        }

        gameCamera.orthographic = true;
        gameCamera.orthographicSize = 5.1f;

        gameCamera.backgroundColor =
            new Color(0.80f, 0.92f, 0.85f);

        gameCamera.transform.position =
            new Vector3(0.2f, 2f, -10f);

        // Фон
        if (backgroundSprite != null)
        {
            CreateObject(
                "Background",
                new Vector2(0.2f, 2f),
                new Vector2(18f, 10.2f),
                Color.white,
                false,
                -10,
                backgroundSprite
            );
        }

        // Стартовая платформа
        CreateObject(
            "Start Platform",
            new Vector2(-3.5f, -0.5f),
            new Vector2(7f, 1f),
            new Color(0.30f, 0.55f, 0.36f),
            true,
            0
        );

        // Верхняя платформа
        CreateObject(
            "Finish Platform",
            new Vector2(3.5f, 2.6f),
            new Vector2(4f, 0.6f),
            new Color(0.35f, 0.60f, 0.40f),
            true,
            0
        );

        // Место посадки
        CreateObject(
            "Planting Spot",
            new Vector2(bambooX, 0.04f),
            new Vector2(0.9f, 0.12f),
            new Color(0.45f, 0.30f, 0.15f),
            false,
            1
        );

        // Портал
        CreateObject(
            "Finish Portal",
            new Vector2(4.6f, 3.8f),
            new Vector2(0.9f, 1.6f),
            portalSprite == null
                ? new Color(0.25f, 0.9f, 0.85f)
                : Color.white,
            false,
            3,
            portalSprite
        );

        // Персонаж
        player = CreateObject(
            "Player",
            new Vector2(-5.5f, 1f),
            new Vector2(0.7f, 0.9f),
            playerSprite == null
                ? new Color(0.95f, 0.55f, 0.3f)
                : Color.white,
            true,
            5,
            playerSprite
        );

        playerRb = player.AddComponent<Rigidbody2D>();

        playerRb.gravityScale = 2.5f;
        playerRb.freezeRotation = true;

        playerRb.collisionDetectionMode =
            CollisionDetectionMode2D.Continuous;

        playerRb.interpolation =
            RigidbodyInterpolation2D.Interpolate;
    }

    // --------------------------------
    // ОБУЧЕНИЕ
    // --------------------------------

    void SetStep(TutorialStep newStep)
    {
        step = newStep;

        switch (step)
        {
            case TutorialStep.Move:
                message =
                    "Шаг 1/6. Идите вправо к грядке. " +
                    "Используйте A и D.";
                break;

            case TutorialStep.ChooseSeed:
                message =
                    "Шаг 2/6. Выберите семя бамбука. " +
                    "Нажмите 1 или кнопку «Бамбук».";
                break;

            case TutorialStep.Plant:
                message =
                    "Шаг 3/6. Нажмите на грядку " +
                    "или кнопку «Посадить».";
                break;

            case TutorialStep.Grow:
                message =
                    "Шаг 4/6. Нажмите E три раза. " +
                    "Вырастите бамбук до 3 секунд.";
                break;

            case TutorialStep.Climb:
                message =
                    "Шаг 5/6. Подойдите к бамбуку " +
                    "и нажмите W, чтобы подняться.";
                break;

            case TutorialStep.ReachPortal:
                message =
                    "Шаг 6/6. Прыгните вправо на " +
                    "платформу и дойдите до портала.";
                break;

            case TutorialStep.Complete:
                message = "Уровень пройден!";
                break;
        }
    }

    void SelectBamboo()
    {
        if (step != TutorialStep.ChooseSeed)
            return;

        bambooSelected = true;

        SetStep(TutorialStep.Plant);
    }

    // --------------------------------
    // ПОСАДКА БАМБУКА
    // --------------------------------

    void PlantBamboo()
    {
        if (step != TutorialStep.Plant)
            return;

        if (!bambooSelected || bambooPlanted)
            return;

        bambooPlanted = true;
        bambooAge = 0;

        bambooRoot = new GameObject("Bamboo");

        bambooStem = CreateObject(
            "Bamboo Stem",
            new Vector2(bambooX, 0.4f),
            new Vector2(0.35f, 0.8f),
            new Color(0.25f, 0.75f, 0.3f),
            false,
            2
        );

        bambooTop = CreateObject(
            "Bamboo Top",
            new Vector2(bambooX, 0.8f),
            new Vector2(1.1f, 0.2f),
            new Color(0.2f, 0.65f, 0.25f),
            true,
            3
        );

        bambooStem.transform.SetParent(
            bambooRoot.transform
        );

        bambooTop.transform.SetParent(
            bambooRoot.transform
        );

        bambooCollider =
            bambooTop.GetComponent<BoxCollider2D>();

        UpdateBamboo();

        SetStep(TutorialStep.Grow);
    }

    float GetBambooHeight()
    {
        if (bambooAge < 1)
            return 0.8f;

        if (bambooAge < 2)
            return 1.3f;

        if (bambooAge < 3)
            return 1.8f;

        if (bambooAge <= 5)
            return 3.2f;

        return 4.2f;
    }

    void ChangeAge(int amount)
    {
        if (!bambooPlanted ||
            step == TutorialStep.Complete)
            return;

        bambooAge = Mathf.Clamp(
            bambooAge + amount,
            0,
            6
        );

        UpdateBamboo();

        if (bambooAge >= 3 &&
            bambooAge <= 5 &&
            step == TutorialStep.Grow)
        {
            SetStep(TutorialStep.Climb);
        }

        if (bambooAge < 3 &&
            step == TutorialStep.Climb)
        {
            SetStep(TutorialStep.Grow);
        }

        if (bambooAge >= 6)
        {
            message =
                "Бамбук состарился и стал хрупким! " +
                "Нажмите Q, чтобы вернуть его возраст.";
        }
        else if (bambooAge >= 3 &&
                 step == TutorialStep.Climb)
        {
            message =
                "Бамбук вырос! Подойдите к нему " +
                "и нажмите W.";
        }
    }

    void UpdateBamboo()
    {
        float height = GetBambooHeight();

        Color color = bambooAge >= 6
            ? new Color(0.6f, 0.5f, 0.25f)
            : new Color(0.25f, 0.75f, 0.3f);

        // Высота стебля
        bambooStem.transform.position =
            new Vector2(bambooX, height / 2f);

        bambooStem.transform.localScale =
            new Vector3(0.35f, height, 1f);

        bambooStem.GetComponent<SpriteRenderer>()
            .color = color;

        // Верхняя площадка бамбука
        bambooTop.transform.position =
            new Vector2(bambooX, height);

        bambooTop.transform.localScale =
            new Vector3(1.1f, 0.2f, 1f);

        bambooTop.GetComponent<SpriteRenderer>()
            .color = color;

        // На молодом и старом бамбуке
        // стоять нельзя.
        bambooCollider.enabled =
            bambooAge >= 3 && bambooAge <= 5;
    }

    // --------------------------------
    // УПРАВЛЕНИЕ ПЕРСОНАЖЕМ
    // --------------------------------

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
            return;
        }

        if (step == TutorialStep.Complete)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            SelectBamboo();

        if (Input.GetKeyDown(KeyCode.E))
            ChangeAge(1);

        if (Input.GetKeyDown(KeyCode.Q))
            ChangeAge(-1);

        if (Input.GetKeyDown(KeyCode.Space) ||
            jumpPressed)
        {
            Jump();
            jumpPressed = false;
        }

        if (Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.UpArrow) ||
            climbPressed)
        {
            ClimbBamboo();
            climbPressed = false;
        }

        // Посадка мышью
        if (Input.GetMouseButtonDown(0) &&
            step == TutorialStep.Plant)
        {
            Vector3 mouse = Input.mousePosition;

            // Не обрабатываем клики по панелям UI.
            if (mouse.y > 115 &&
                mouse.y < Screen.height - 145)
            {
                Vector3 world =
                    gameCamera.ScreenToWorldPoint(mouse);

                if (Vector2.Distance(
                    new Vector2(world.x, world.y),
                    new Vector2(bambooX, 0)) < 0.8f)
                {
                    PlantBamboo();
                }
            }
        }

        // Следующий этап после движения
        if (step == TutorialStep.Move &&
            player.transform.position.x >= -2.7f)
        {
            SetStep(TutorialStep.ChooseSeed);
        }

        // Проверка падения
        if (player.transform.position.y < -5f)
        {
            Respawn();
        }

        CheckFinish();
    }

    void FixedUpdate()
    {
        if (step == TutorialStep.Complete)
        {
            playerRb.linearVelocity = Vector2.zero;
            return;
        }

        float horizontal = 0;

        if (Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.LeftArrow) ||
            moveLeft)
        {
            horizontal -= 1;
        }

        if (Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.RightArrow) ||
            moveRight)
        {
            horizontal += 1;
        }

        horizontal = Mathf.Clamp(
            horizontal,
            -1f,
            1f
        );

        Vector2 velocity = playerRb.linearVelocity;

        velocity.x = horizontal * moveSpeed;

        playerRb.linearVelocity = velocity;
    }

    bool IsGrounded()
    {
        Vector2 feet =
            (Vector2)player.transform.position +
            Vector2.down * 0.46f;

        Collider2D[] hits =
            Physics2D.OverlapCircleAll(feet, 0.13f);

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject != player &&
                !hit.isTrigger)
            {
                return true;
            }
        }

        return false;
    }

    void Jump()
    {
        if (!IsGrounded())
            return;

        Vector2 velocity = playerRb.linearVelocity;

        velocity.y = jumpSpeed;

        playerRb.linearVelocity = velocity;
    }

    void ClimbBamboo()
    {
        if (step != TutorialStep.Climb &&
            step != TutorialStep.ReachPortal)
            return;

        if (bambooAge < 3 || bambooAge > 5)
            return;

        Vector2 position = player.transform.position;

        // Игрок должен стоять рядом с бамбуком.
        if (Mathf.Abs(position.x - bambooX) > 1.2f)
        {
            message = "Подойдите ближе к бамбуку.";
            return;
        }

        float height = GetBambooHeight();

        // Упрощённый подъём для обучающего уровня.
        playerRb.position = new Vector2(
            bambooX,
            height + 0.65f
        );

        playerRb.linearVelocity = Vector2.zero;

        if (step == TutorialStep.Climb)
            SetStep(TutorialStep.ReachPortal);
    }

    // --------------------------------
    // ПОБЕДА И ПЕРЕЗАПУСК
    // --------------------------------

    void CheckFinish()
    {
        if (step != TutorialStep.ReachPortal)
            return;

        Vector2 position = player.transform.position;

        if (position.x >= 4.2f &&
            position.x <= 5.3f &&
            position.y >= 3.2f &&
            position.y <= 5.2f)
        {
            CompleteLevel();
        }
    }

    void CompleteLevel()
    {
        SetStep(TutorialStep.Complete);

        PlayerPrefs.SetInt(
            "ChronoGarden_Level01",
            1
        );

        PlayerPrefs.Save();
    }

    void Respawn()
    {
        playerRb.position = new Vector2(-5.5f, 1f);
        playerRb.linearVelocity = Vector2.zero;

        message =
            "Вы упали. Попробуйте ещё раз!";

        // Растение сохраняем.
        // Игрок может снова подняться на него.
    }

    void RestartLevel()
    {
        if (bambooRoot != null)
            Destroy(bambooRoot);

        bambooStem = null;
        bambooTop = null;
        bambooCollider = null;

        bambooAge = 0;
        bambooPlanted = false;
        bambooSelected = false;

        moveLeft = false;
        moveRight = false;
        jumpPressed = false;
        climbPressed = false;

        playerRb.position =
            new Vector2(-5.5f, 1f);

        playerRb.linearVelocity = Vector2.zero;

        SetStep(TutorialStep.Move);
    }

    // --------------------------------
    // ИНТЕРФЕЙС
    // --------------------------------

    void OnGUI()
    {
        // Подключаем русский шрифт.
        if (russianFont != null)
        {
            GUI.skin.font = russianFont;
            GUI.skin.button.font = russianFont;
            GUI.skin.label.font = russianFont;
            GUI.skin.box.font = russianFont;
        }

        GUI.skin.button.fontSize = 17;
        GUI.skin.label.fontSize = 17;
        GUI.skin.box.fontSize = 19;

        float screenW = Screen.width;
        float screenH = Screen.height;

        // Верхняя панель
        GUI.Box(
            new Rect(8, 8, screenW - 16, 125),
            ""
        );

        float buttonWidth = (screenW - 50f) / 4f;

        if (GUI.Button(
            new Rect(12, 14, buttonWidth, 43),
            "Бамбук [1]"))
        {
            SelectBamboo();
        }

        if (GUI.Button(
            new Rect(
                22 + buttonWidth,
                14,
                buttonWidth,
                43),
            "-1 [Q]"))
        {
            ChangeAge(-1);
        }

        if (GUI.Button(
            new Rect(
                32 + buttonWidth * 2,
                14,
                buttonWidth,
                43),
            "+1 [E]"))
        {
            ChangeAge(1);
        }

        if (GUI.Button(
            new Rect(
                42 + buttonWidth * 3,
                14,
                buttonWidth,
                43),
            "Сброс [R]"))
        {
            RestartLevel();
        }

        string plantInfo = bambooPlanted
            ? "Бамбук | Возраст: " + bambooAge + " сек."
            : "Бамбук ещё не посажен";

        GUI.Label(
            new Rect(20, 62, screenW - 40, 24),
            plantInfo
        );

        GUI.Label(
            new Rect(20, 91, screenW - 40, 35),
            message
        );

        // Кнопка посадки над грядкой
        if (step == TutorialStep.Plant)
        {
            Vector3 position =
                gameCamera.WorldToScreenPoint(
                    new Vector3(bambooX, 0.4f, 0));

            float x = position.x - 65;
            float y = screenH - position.y - 75;

            if (GUI.Button(
                new Rect(x, y, 130, 40),
                "Посадить"))
            {
                PlantBamboo();
            }
        }

        // Кнопки перемещения
        float bottom = screenH - 85f;

        moveLeft = GUI.RepeatButton(
            new Rect(10, bottom, 80, 65),
            "Влево"
        );

        moveRight = GUI.RepeatButton(
            new Rect(100, bottom, 80, 65),
            "Вправо"
        );

        if (GUI.Button(
            new Rect(screenW - 180, bottom, 80, 65),
            "Прыжок"))
        {
            jumpPressed = true;
        }

        if (GUI.Button(
            new Rect(screenW - 90, bottom, 80, 65),
            "Вверх"))
        {
            climbPressed = true;
        }

        // Окно завершения
        if (step == TutorialStep.Complete)
        {
            float centerX = screenW / 2f;
            float centerY = screenH / 2f;

            GUI.Box(
                new Rect(
                    centerX - 170,
                    centerY - 100,
                    340,
                    200),
                "УРОВЕНЬ ПРОЙДЕН!"
            );

            GUI.Label(
                new Rect(
                    centerX - 140,
                    centerY - 35,
                    280,
                    50),
                "Вы научились управлять ростом!"
            );

            if (GUI.Button(
                new Rect(
                    centerX - 100,
                    centerY + 35,
                    200,
                    45),
                "Играть снова"))
            {
                RestartLevel();
            }
        }
    }
}
