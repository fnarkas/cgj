using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    //--------------------------------------------------------
    // Game variables

    public static int Level = 0;
    public static int lives = 3;

	public enum GameState { Init, Game, Dead, Scores }
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
    
    //-------------------------------------------------------------------
    // singleton implementation
    private static GameManager _instance;

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
        }
        else
        {
            if(this != _instance)   
                Destroy(this.gameObject);
        }

        AssignGhosts();
    }

	void Start () 
	{
		gameState = GameState.Game;
        InitCheckpoints();
	}

    void OnLevelWasLoaded()
    {
        if (Level == 0) lives = 3;

        Debug.Log("Level " + Level + " Loaded!");
        AssignGhosts();
        ResetVariables();

        foreach(var ghost in ghosts){
            ghost.speed += Level * SpeedPerLevel;
        }
        pacman.GetComponent<PlayerController>().speed += Level*SpeedPerLevel/2;
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
		if(scared && _timeToCalm <= Time.time)
			CalmGhosts();

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

        gameState = GameState.Game;  
        // TODO: Show ready screen
        ResetCheckpoints();

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
        checkpoints[0].gameObject.SetActive(true);
    }

    public void NextCheckpoint(){
        currentCheckpoint++;
        if(currentCheckpoint < checkpoints.Count){
            checkpoints[currentCheckpoint].gameObject.SetActive(true);
        }
        else{
            Debug.Log("Alla samlade!");
        }
    }

    public void LoseLife()
    {
        lives--;
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
