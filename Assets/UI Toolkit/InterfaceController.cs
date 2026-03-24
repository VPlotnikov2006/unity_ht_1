using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

enum StartMenu
{
    Game,
    MainMenu
}

[RequireComponent(typeof(UIDocument))]
public class InterfaceController : MonoBehaviour
{
    [SerializeField] private StartMenu start;
    [SerializeField] private Player player;

    private UIDocument document;

    private VisualElement gameUI;
    private VisualElement healthbar;
    private Label scorebar;

    private VisualElement pauseMenu;

    private VisualElement mainMenu;
    private ListView scoreBoard;

    private float startTime;

    private List<float> highScores = new();

    private bool isGameUISetup;
    private bool isPauseMenuSetup;
    private bool isMainMenuSetup;

    void EnsureDocument()
    {
        if (document == null)
            document = GetComponent<UIDocument>();
    }

    void EnsureGameUI()
    {
        if (isGameUISetup) return;

        EnsureDocument();
        SetupGameUI();
        isGameUISetup = true;
    }

    void EnsurePauseMenu()
    {
        if (isPauseMenuSetup) return;

        EnsureDocument();
        SetupPauseMenu();
        isPauseMenuSetup = true;
    }

    void EnsureMainMenu()
    {
        if (isMainMenuSetup) return;

        EnsureDocument();
        SetupMainMenu();
        isMainMenuSetup = true;
    }

    void Awake()
    {
        LoadScores();
    }

    void Start()
    {
        switch (start)
        {
            case StartMenu.Game:
                StartGame();
                break;
            case StartMenu.MainMenu:
                EnterMainMenu();
                break;
        }
    }

    void SetupGameUI()
    {
        gameUI = document.rootVisualElement.Q("GameUI");
        healthbar = gameUI.Q("Healthbar");
        scorebar = gameUI.Q("Scorebar").Q<Label>();

        healthbar.Clear();

        healthbar.style.width = 64 * player.maxHp;

        for (int i = 0; i < player.maxHp; i++)
        {
            var heart = new VisualElement();
            heart.AddToClassList("heart");

            healthbar.Add(heart);
        }
    }

    void SetupPauseMenu()
    {
        pauseMenu = document.rootVisualElement.Q("PauseMenu");

        pauseMenu.Q<Button>("ContinueButton").clicked += ContinueGame;
        pauseMenu.Q<Button>("MainMenuButton").clicked += GameOver;
    }

    void SetupMainMenu()
    {
        mainMenu = document.rootVisualElement.Q("MainMenu");

        scoreBoard = mainMenu.Q<ListView>();

        scoreBoard.fixedItemHeight = 60;
        scoreBoard.makeItem = () => new Label();
        scoreBoard.bindItem = (element, i) =>
        {
            (element as Label).text = $"{i + 1}. {highScores[i]:0.0}";
        };
        scoreBoard.itemsSource = highScores;
        

        mainMenu.Q<Button>("StartButton").clicked += StartGame;

        mainMenu.Q<Button>("ExitButton").clicked += () =>
        {
            SaveScores();

#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
        };
    }

    void AddScore(float score)
    {
        highScores.Add(score);

        highScores = highScores
            .OrderByDescending(s => s)
            .Take(5)
            .ToList();

        UpdateScoreBoard();
    }

    void UpdateScoreBoard()
    {
        EnsureMainMenu();

        scoreBoard.itemsSource = null;
        scoreBoard.itemsSource = highScores;
        scoreBoard.Rebuild();
    }

    void ContinueGame()
    {
        EnsurePauseMenu();
        EnsureGameUI();
        EnsureMainMenu();

        pauseMenu.style.display = DisplayStyle.None;
        mainMenu.style.display = DisplayStyle.None;
        gameUI.style.display = DisplayStyle.Flex;

        Time.timeScale = 1;
    }

    void StartGame()
    {
        Restart();
        player.Restart();

        ContinueGame();
    }

    void Pause()
    {
        EnsurePauseMenu();

        pauseMenu.style.display = DisplayStyle.Flex;
        // gameUI.style.display = DisplayStyle.None;

        Time.timeScale = 0;
    }

    void EnterMainMenu()
    {
        EnsurePauseMenu();
        EnsureGameUI();
        EnsureMainMenu();

        pauseMenu.style.display = DisplayStyle.None;
        gameUI.style.display = DisplayStyle.None;
        mainMenu.style.display = DisplayStyle.Flex;

        Time.timeScale = 0;
    }

    public void GameOver()
    {
        AddScore(Time.time - startTime);
        SaveScores();
        EnterMainMenu();
    }

    public void OnExit()
    {
        Pause();
    }

    void FixedUpdate()
    {
        EnsureGameUI();

        scorebar.text = $"Счёт: {Time.time - startTime:0.0}";
    }

    public void Restart()
    {
        startTime = Time.time;
    }

    public void FixHealthbar()
    {
        EnsureGameUI();

        for (int i = 0; i < player.currentHp; i++)
        {
            healthbar.Children().ElementAt(i).style.visibility = Visibility.Visible;
        }

        for (int i = player.currentHp; i < player.maxHp; i++)
        {
            healthbar.Children().ElementAt(i).style.visibility = Visibility.Hidden;
        }
    }

    [Serializable]
    private class SaveData
    {
        public List<float> highScores;
    }

    void SaveScores()
    {
        SaveData data = new SaveData
        {
            highScores = highScores
        };
        string json = JsonUtility.ToJson(data, true);

        string path = Path.Combine(Application.persistentDataPath, "save.json");

        File.WriteAllText(path, json);
    }

    void LoadScores()
    {
        string path = Path.Combine(Application.persistentDataPath, "save.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);

            SaveData data = JsonUtility.FromJson<SaveData>(json);

            highScores = data.highScores
                .OrderByDescending(s => s)
                .Take(5)
                .ToList();
        }
    }
}
