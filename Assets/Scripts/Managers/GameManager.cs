using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    //--------------------------------------------------------
    // Game variables

    public static int Level = 0;
    public static int lives = 3;

    public enum GameState { Init, Wait,Game, Dead, Scores, Loading }
    public static GameState gameState;

    private GameObject pacman;
    public static bool scared;
    static public int score;

    public float scareLength;
    private float _timeToCalm;

    public float SpeedPerLevel;

    private List<Checkpoint> checkpoints;
    private List<GhostMove> ghosts;
    private int currentCheckpoint = 0;

    private string[] sceneNames = {"game", "pacmanLvl2", "pacmanLvl3"};
    private int sceneCounter = 0;

    private SoundManager source;

    public GameObject popup;

    //-------------------------------------------------------------------
    // singleton implementation
    private static GameManager _instance;
    private GameObject _rightPopup;
    private GameObject _leftPopup;
    private HeartController _heartController;

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
          Debug.Log("I AM  AWAKE");
            _instance = this;
            DontDestroyOnLoad(this);
            AssignGhosts();
            Init();
        }
        else
        {
            if(this != _instance)
                Destroy(this.gameObject);
        }

    }

	//void Start ()
	void Init ()
	{
        _heartController = GameObject.FindObjectOfType<HeartController>();
        InitPopups();
        gameState = GameState.Wait;
        InitCheckpoints();
        source = GameObject.Find("Audio Source").GetComponent<SoundManager>();
        OnLevelLoaded();
	}

    private void InitPopups()
    {
        _leftPopup = GameObject.Instantiate(popup);
        _rightPopup = GameObject.Instantiate(popup);
        _leftPopup.transform.parent = transform;
        _rightPopup.transform.parent = transform;
        ChangeCameras();
    }

    private void ChangeCameras() {
      Debug.Log("Changing cameras");
        Camera leftCamera = GameObject.Find("Left Camera").GetComponent<Camera>();
        Camera rightCamera = GameObject.Find("Right Camera").GetComponent<Camera>();
        _leftPopup.GetComponent<Canvas>().worldCamera = leftCamera;
        _rightPopup.GetComponent<Canvas>().worldCamera = rightCamera;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
      OnLevelLoaded();
      ResetScene();
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>

    private void ListenForAnyKey()
    {
        if(Input.anyKeyDown){
            gameState = GameState.Game;
            _rightPopup.SetActive(false);
            _leftPopup.SetActive(false);
        }
    }

    void OnLevelLoaded()
    {
        if (Level == 0) lives = 3;

        Debug.Log("Level " + Level + " Loaded!");
        AssignGhosts();
        ResetVariables();

        foreach(var ghost in ghosts){
            ghost.speed += Level * SpeedPerLevel;
        }
        pacman.GetComponent<PlayerController>().speed += Level*SpeedPerLevel/2;

        // Play lvl music
        if(!source.isPlaying()) {
          source.PlayLvlTheme(Level);
        }
    }

    private void ResetVariables()
    {
        _timeToCalm = 0.0f;
        scared = false;
        PlayerController.killstreak = 0;
    }

    // Update is called once per frame
	void Update ()
	{
    if(gameState == GameState.Wait) {
      ListenForAnyKey();
    }

    if(gameState == GameState.Wait) {
      _leftPopup.SetActive(true);
      _rightPopup.SetActive(true);
    }
    if(scared && _timeToCalm <= Time.time)
      CalmGhosts();

    // FIXME Ugly solution to loop theme music
    if(!source.isPlaying()) {
      source.PlayLvlTheme(Level);
    }
	}

  public void ResetScene()
  {
    CalmGhosts();

    pacman.transform.position = new Vector3(15f, 11f, 0f);

    pacman.GetComponent<PlayerController>().ResetDestination();
    foreach(var ghost in ghosts){
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
		if(!scared)	ScareGhosts();
		else 		CalmGhosts();
	}

	public void ScareGhosts()
	{
		scared = true;
        foreach(var ghost in ghosts){
    		ghost.Frighten();
        }
		_timeToCalm = Time.time + scareLength;

        Debug.Log("Ghosts Scared");
	}

	public void CalmGhosts()
	{
		scared = false;
        foreach(var ghost in ghosts){
    		ghost.Calm();
        }
	    PlayerController.killstreak = 0;
    }

    void AssignGhosts()
    {
        pacman = GameObject.Find("pacman");
        ghosts = new List<GhostMove>(FindObjectsOfType<GhostMove>());



        if (pacman == null) Debug.Log("Pacman is NULL");

    }

    public void InitCheckpoints(){
        var unorderedCheckpoints = FindObjectsOfType<Checkpoint>();
        checkpoints = new List<Checkpoint>(unorderedCheckpoints);
        checkpoints.Sort((x,y) => x.GetComponent<CheckpointEditor>().Number.CompareTo(y.GetComponent<CheckpointEditor>().Number));
        ResetCheckpoints();
       }

    private void ResetCheckpoints(){
     currentCheckpoint = 0;
        foreach(var checkpoint in checkpoints){
            checkpoint.gameObject.SetActive(false);
            checkpoint.name = "checkpoint";
            Canvas canvas = checkpoint.GetComponentInChildren<Canvas>();
            if(canvas)
                canvas.gameObject.SetActive(false);
        }
        if (checkpoints.Count > 0) {
          checkpoints[0].gameObject.SetActive(true);
        }
    }

    public void NextCheckpoint(){
        // Play checkpoint sound
        source.PlayPickup(currentCheckpoint);
        currentCheckpoint++;
      //  if(currentCheckpoint < checkpoints.Count){
      //      checkpoints[currentCheckpoint].gameObject.SetActive(true);
      //  }
      //  else{
            Debug.Log("Alla samlade!");
            // TODO Play finish music
            // TODO Show blackhole
            // FIXME TODO Change scene
            sceneCounter++;
            if (sceneCounter > 2)
              sceneCounter = 0;
            gameState = GameState.Loading;
            checkpoints = new List<Checkpoint>();
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(sceneNames[sceneCounter]);
            //ResetScene();
      //  }
    }

    public void LoseLife()
    {
        lives--;
        _heartController.SetLives(lives);
        gameState = GameState.Dead;
    }


    public static void DestroySelf()
    {

        score = 0;
        Level = 0;
        lives = 3;
        Destroy(GameObject.Find("Game Manager"));
    }
}
