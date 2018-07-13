using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    //--------------------------------------------------------
    // Game variables

    public static int Level = 0;
    public static int lives = 3;

    public enum GameState { Init, Wait, WaitTime, Game, Dead, Scores, Loading, GameOver }
    public static GameState gameState;

    private GameObject pacman;
    public static bool scared;
    static public int score;

    public float scareLength;
    private float _timeToCalm;

    public float SpeedPerLevel = 0.05f;

    private List<Checkpoint> checkpoints;
    private List<GhostMove> ghosts;
    private int currentCheckpoint = 0;

    private string[] sceneNames = { "game", "pacmanLvl2", "pacmanLvl3" };

    private static int nbrActiveGhosts = 0;
    private static bool shouldRandomizeControls = false;
    private static bool switchPlayerControls = false;

    private SoundManager sourceEffect;
    private SoundManagerMusic sourceMusic;

    public GameObject popup;

    //-------------------------------------------------------------------
    // singleton implementation
    private static GameManager _instance;
    private GameObject _rightPopup;
    private GameObject _leftPopup;
    private HeartController _heartController;

    public GameObject checkpointPrefab;
    Dictionary<Vector2Int, Vector2> freeTiles;
    private const int SCREEN1 = 10;
    private const int SCREEN2 = 11;

    public GameObject particleSystem;

    int nCheckpoints = 3;

    private static int CHECKPOINTDISTANCE = 10;

    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    //-------------------------------------------------------------------
    // function definitions

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
            SceneManager.sceneLoaded += OnSceneLoaded;
            Init();
        }
        else
        {
            if (this != _instance)
                Destroy(this.gameObject);
        }

    }

    //void Start ()
    void Init()
    {
        _heartController = GameObject.FindObjectOfType<HeartController>();
        InitPopups();
        sourceEffect = GameObject.Find("Audio Source Effects").GetComponent<SoundManager>();
        sourceMusic = GameObject.Find("Audio Source Music").GetComponent<SoundManagerMusic>();
    }

    private void InitPopups()
    {
        _leftPopup = GameObject.Instantiate(popup);
        _rightPopup = GameObject.Instantiate(popup);
        _leftPopup.transform.parent = transform;
        _rightPopup.transform.parent = transform;
        ChangeCameras();
    }

    private void ChangeCameras()
    {
        Camera leftCamera = GameObject.Find("Left Camera").GetComponent<Camera>();
        Camera rightCamera = GameObject.Find("Right Camera").GetComponent<Camera>();
        _leftPopup.GetComponent<Canvas>().worldCamera = leftCamera;
        _rightPopup.GetComponent<Canvas>().worldCamera = rightCamera;
    }

    private void setNbrCheckPointsText(int nbrLeft)
    {
      Text textLeft = GameObject.Find("TextLeft").GetComponent<Text>();
      String text = (nCheckpoints - nbrLeft) + "/" + nCheckpoints;
      textLeft.text =  text;
      Text textRight = GameObject.Find("TextRight").GetComponent<Text>();
      textRight.text = text;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        OnLevelLoaded();
        ResetScene();
        gameState = GameState.WaitTime;
        StartCoroutine(SleepTime());
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>

    private void ListenForAnyKey()
    {
        if (Input.anyKeyDown)
        {
            gameState = GameState.Game;
            _rightPopup.SetActive(false);
            _leftPopup.SetActive(false);
        }
    }

    void OnLevelLoaded()
    {
        AssignGhosts();
        AssignControls();
        FindFreeTiles();
        InitCheckpoints();
        if (Level == 0) lives = 3;
        Debug.Log("Level " + Level + " Loaded!");
        ResetVariables();

        foreach (var ghost in ghosts)
        {
            ghost.speed = 0.13f + Level * SpeedPerLevel;
        }
        pacman.GetComponent<PlayerController>().speed = 0.2f + Level * SpeedPerLevel;

        sourceMusic.PlayLvlStartTheme(Level);
    }

    private void AssignControls()
    {
        PlayerController.Controls controls = FindObjectOfType<PlayerController>().RandomizePlayerControls(shouldRandomizeControls);
        if (switchPlayerControls) {
          controls = FindObjectOfType<PlayerController>().SwitchPlayerControls();
        }
        PopupController popupControllerLeft = _leftPopup.GetComponent<PopupController>();
        PopupController popupControllerRight = _rightPopup.GetComponent<PopupController>();
        switch(controls){
            case PlayerController.Controls.LeftAll:
                popupControllerLeft.ShowAllLeft();
                popupControllerRight.ShowNone();
            break;
            case PlayerController.Controls.LeftHorizontal:
                popupControllerLeft.ShowHorizontalLeft();
                popupControllerRight.ShowVerticalRight();
            break;
            case PlayerController.Controls.LeftVertical:
                popupControllerLeft.ShowVerticalLeft();
                popupControllerRight.ShowHorizontalRight();
            break;
            case PlayerController.Controls.RightAll:
                popupControllerLeft.ShowNone();
                popupControllerRight.ShowAllRight();
            break;
        }
    }

    private void ResetGame()
    {
      Level = 0;
      lives = 3;
      nbrActiveGhosts = 0;
      shouldRandomizeControls = false;
      switchPlayerControls = false;
      _heartController.SetLives(lives);
      SceneManager.LoadScene(sceneNames[Level]);
      ResetScene();
    }

    private void ResetVariables()
    {
        _timeToCalm = 0.0f;
        scared = false;
        PlayerController.killstreak = 0;
    }

    IEnumerator ghostDistance()
    {
      Vector2 pacmanVector = new Vector2(pacman.transform.position.x, pacman.transform.position.y);
      foreach (var ghost in ghosts)
      {
        GameObject ghostObject = ghost.gameObject;
        if (ghostObject.activeSelf) {
          Vector2 ghostVector = new Vector2(ghost.transform.position.x, ghost.transform.position.y);
          float dist = Vector2.Distance(ghostVector, pacmanVector);
          if (dist < 3.2) {
            Debug.Log("Close!");
          }
        }
      }
      yield return null;
    }

    IEnumerator SleepTime()
    {
      yield return new WaitForSeconds(2);
      gameState = GameState.Wait;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState == GameState.GameOver)
        {
          Debug.Log("GAME OVER game");

          // TODO Print game over text
          StartCoroutine(SleepTime());
          ResetGame();
        }

        if (gameState == GameState.WaitTime)
        {
            _leftPopup.SetActive(true);
            _rightPopup.SetActive(true);
        }

        if (gameState == GameState.Wait)
        {
            ListenForAnyKey();
        }
        if (scared && _timeToCalm <= Time.time)
            CalmGhosts();

        // Play lvl music
        if (!sourceMusic.isPlaying() && !sourceEffect.isPlaying())
        {
            sourceMusic.PlayLvlTheme(Level);
        }

        if (gameState == GameState.Game)
        {
          StartCoroutine(ghostDistance());
        }
    }

    public void ResetScene()
    {
        CalmGhosts();

        pacman.transform.position = new Vector3(15f, 11f, 0f);

        pacman.GetComponent<PlayerController>().ResetDestination();
        foreach (var ghost in ghosts)
        {
            ghost.ResetPosition();
            ghost.InitializeGhost();
        }

        gameState = GameState.Wait;
        // TODO: Show ready screen
        ResetCheckpoints();
        //InitPopups();
        ChangeCameras();
    }

    public void ToggleScare()
    {
        if (!scared) ScareGhosts();
        else CalmGhosts();
    }

    public void ScareGhosts()
    {
        scared = true;
        foreach (var ghost in ghosts)
        {
            ghost.Frighten();
        }
        _timeToCalm = Time.time + scareLength;

        Debug.Log("Ghosts Scared");
    }

    public void CalmGhosts()
    {
        scared = false;
        foreach (var ghost in ghosts)
        {
            ghost.Calm();
        }
        PlayerController.killstreak = 0;
    }

    void AssignGhosts()
    {
        pacman = GameObject.Find("pacman");
        ghosts = new List<GhostMove>(FindObjectsOfType<GhostMove>());

        // Enable/Disable ghosts depending on difficulty
        int ghostCounter = 1; // We always have blinky active, do not count him
        List<GameObject> leftGhosts = new List<GameObject>();
        List<GameObject> rightGhosts = new List<GameObject>();
        foreach (var ghost in ghosts)
        {
            GameObject ghostObject = ghost.gameObject;
            if (ghost.name == "blinky")
            {
                // blinky is always activated
                ghostObject.SetActive(true);
                Debug.Log("Enable ghost: " + ghost.name);
                if (Level == 1) {
                  ghostObject.layer = SCREEN1;
                } else {
                  ghostObject.layer = SCREEN2;
                }
            }
            else if (nbrActiveGhosts > ghostCounter)
            {
                Debug.Log("Enable ghost: " + ghost.name);
                ghostObject.SetActive(true);
                ghostCounter++;
            }
            else
            {
                Debug.Log("Disable ghost: " + ghost.name);
                ghostObject.SetActive(false);
            }
            if (ghostObject.activeSelf)
            {
                if (ghostObject.layer == SCREEN1)
                {
                    leftGhosts.Add(ghostObject);
                    Debug.Log("Ghost " + ghost.name + " is shown on left screen");
                }
                else if (ghostObject.layer == SCREEN2)
                {
                    rightGhosts.Add(ghostObject);
                }
            }

        }

        _leftPopup.GetComponent<PopupController>().ShowGhosts(leftGhosts);
        _rightPopup.GetComponent<PopupController>().ShowGhosts(rightGhosts);

        if (pacman == null) Debug.Log("Pacman is NULL");

    }

    public void InitCheckpoints()
    {
        checkpoints = new List<Checkpoint>();
        for (int i = 0; i < nCheckpoints; i++)
        {
            var checkpoint = Instantiate(checkpointPrefab);
            if (Level == 1) {
              checkpoint.layer = SCREEN2;
              _rightPopup.GetComponent<PopupController>().ShowTacos();
              _leftPopup.GetComponent<PopupController>().HideTacos();
            } else {
              checkpoint.layer = SCREEN1;
              _leftPopup.GetComponent<PopupController>().ShowTacos();
              _rightPopup.GetComponent<PopupController>().HideTacos();
            }
            checkpoint.transform.parent = transform.parent;
            checkpoints.Add(checkpoint.GetComponent<Checkpoint>());
        }
        ResetCheckpoints();
    }

    private void ResetCheckpoints()
    {
        // Get position of player for initial state
        Vector2 previousVector = new Vector2(pacman.transform.position.x, pacman.transform.position.y);

        float distLastCheckpoint = 255.0f;

        currentCheckpoint = 0;
        List<Vector2Int> keyList = new List<Vector2Int>(freeTiles.Keys);
        foreach (var checkpoint in checkpoints)
        {
            Vector2 pos;
            int index = 0;
            // Make sure we have a proper distance from the reviously placed checkpoint.
            // Start measurements from the player
            do {
              index = UnityEngine.Random.Range(0, keyList.Count);
              pos = freeTiles[keyList[index]];
              distLastCheckpoint = Vector2.Distance(previousVector, pos);
            } while (distLastCheckpoint < CHECKPOINTDISTANCE);
            previousVector = pos;

            checkpoint.transform.position = pos;
            checkpoint.gameObject.SetActive(false);
            checkpoint.name = "checkpoint";
            Canvas canvas = checkpoint.GetComponentInChildren<Canvas>();
            if (canvas)
                canvas.gameObject.SetActive(false);
        }
        if (checkpoints.Count > 0)
        {
            checkpoints[0].gameObject.SetActive(true);
        }
        setNbrCheckPointsText(checkpoints.Count);
    }

    public void NextCheckpoint()
    {
        // Play checkpoint sound
        sourceEffect.PlayPickup(currentCheckpoint);
        currentCheckpoint++;

        // Update UI element showing number of checkpoints to gather
        setNbrCheckPointsText(checkpoints.Count - currentCheckpoint);

        if (currentCheckpoint < checkpoints.Count)
        {
            // particleSystem.particleEmitter = checkpoints[currentCheckpoint -1].transform.position;
             ParticleSystem.ShapeModule _editableShape = particleSystem.GetComponent<ParticleSystem>().shape;
            _editableShape.position = checkpoints[currentCheckpoint -1].transform.position;
            particleSystem.GetComponent<ParticleSystem>().Play();
            checkpoints[currentCheckpoint].gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("Alla samlade!");
            // TODO Play finish music
            // TODO Show blackhole
            gameState = GameState.Loading;
            checkpoints = new List<Checkpoint>();
            switchPlayerControls = !switchPlayerControls;
            // Update level, go back to zero if we are at end
            Level++;
            if (Level > 2)
            {
                Level = 0;
                nbrActiveGhosts++;
                shouldRandomizeControls = true;
            }

            SceneManager.LoadScene(sceneNames[Level]);
            ResetScene();
        }
    }

    public void LoseLife()
    {
        lives--;
        _heartController.SetLives(lives);
        gameState = GameState.Dead;
    }



    private void Step(AI ai, Vector3 pos)
    {
        bool isOk = false;
        Vector2Int posInt = new Vector2Int((int)Mathf.Round(pos.x), (int)Mathf.Round(pos.y));
        if (freeTiles.ContainsKey(posInt))
            return;
        freeTiles.Add(posInt, pos);
        if (!ai.IsWallDown(pos))
        {
            Step(ai, pos + Vector3.down);
            isOk = true;
        }
        if (!ai.IsWallUp(pos))
        {
            Step(ai, pos + Vector3.up);
            isOk = true;
        }
        if (!ai.IsWallLeft(pos))
        {
            Step(ai, pos + Vector3.left);
            isOk = true;
        }
        if (!ai.IsWallRight(pos))
        {
            Step(ai, pos + Vector3.right);
            isOk = true;
        }
        if (isOk)
        {
        }

    }

    private void FindFreeTiles()
    {
        freeTiles = new Dictionary<Vector2Int, Vector2>();
        AI ai = FindObjectOfType<AI>();
        Step(ai, pacman.transform.position);
    }


    public static void DestroySelf()
    {

        score = 0;
        lives = 3;
        Destroy(GameObject.Find("Game Manager"));
    }
}
