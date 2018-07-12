using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GhostMove : MonoBehaviour {

    // ----------------------------
    // Navigation variables
	private Vector3 waypoint;			// AI-determined waypoint
	private Queue<Vector3> waypoints;

	// direction is set from the AI component
	public Vector3 _direction;
	public Vector3 direction 
	{
		get
		{
			return _direction;
		}

		set
		{
			_direction = value;
			Vector3 pos = new Vector3((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
			waypoint = pos + _direction;
			Debug.DrawLine(transform.position, waypoint);
		}
	}

	public float speed = 0.3f;

    // ----------------------------
    // Ghost mode variables
	public float scatterLength = 5f;
	public float waitLength = 0.0f;

	private float timeToEndScatter;

	enum State { Scatter, Chase, Run };
	State state;

    private Vector3 _startPos;
    private float _timeToWhite;
    private float _timeToToggleWhite;
    private float _toggleInterval;
    private bool isWhite = false;

	// handles
    public PlayerController pacman;
    private GameManager _gm;

	//-----------------------------------------------------------------------------------------
	// variables end, functions begin
	void Start()
	{
	    _gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        _toggleInterval = _gm.scareLength * 0.33f * 0.20f;  
	    _startPos = transform.position;
		InitializeGhost();
	}

    public float DISTANCE;

	void FixedUpdate ()
	{
	    DISTANCE = Vector3.Distance(transform.position, waypoint);
		count++;
		// if( count % 30 != 0)
		// return;

		if(GameManager.gameState == GameManager.GameState.Game){
			animate ();

			switch(state)
			{
			case State.Scatter:
				Scatter();
				break;

			case State.Chase:
				ChaseAI();
				break;

			case State.Run:
				RunAway();
				break;
			}
		}
	}

	//-----------------------------------------------------------------------------------
	// Start() functions

	public void InitializeGhost()
	{
		if(_startPos == Vector3.zero)
			_startPos = transform.position;
		
		InitializeGhost(_startPos);

	}

	public void ResetPosition(){
		if(_startPos == Vector3.zero)
			_startPos = transform.position;
		transform.position = _startPos;
	}

    public void InitializeGhost(Vector3 pos)
    {
        transform.position = pos;
        waypoint = transform.position;	// to avoid flickering animation
        state = State.Chase;
		direction = Vector3.right;		
    }
	
   
	//------------------------------------------------------------------------------------
	// Update functions
	void animate()
	{
		Vector3 dir = waypoint - transform.position;
		GetComponent<Animator>().SetFloat("DirX", dir.x);
		GetComponent<Animator>().SetFloat("DirY", dir.y);
		GetComponent<Animator>().SetBool("Run", false);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.name == "pacman")
		{
			//Destroy(other.gameObject);
		    if (state == State.Run)
		    {
		        Calm();
		        InitializeGhost(_startPos);
                pacman.UpdateScore();
		    }
		       
		    else
		    {
		        _gm.LoseLife();
		    }

		}
	}

	//-----------------------------------------------------------------------------------
	// State functions
	void Scatter()
	{
		if(Time.time >= timeToEndScatter)
		{
			state = State.Chase;
		    return;
		}
  // if not at waypoint, move towards it
        if (Vector3.Distance(transform.position, waypoint) > 0.000000000001)
		{
			Vector2 p = Vector2.MoveTowards(transform.position, waypoint, speed);
			GetComponent<Rigidbody2D>().MovePosition(p);
			return;
		}
		// get the next waypoint and move towards it
		GetComponent<AI>().FollowTarget(Vector3Int.zero);

	}
	int count = 0;
    void ChaseAI()
	{

        // if not at waypoint, move towards it
        if (Vector3.Distance(transform.position, waypoint) > 0.000000000001)
		{
			Vector2 p = Vector2.MoveTowards(transform.position, waypoint, speed);
			GetComponent<Rigidbody2D>().MovePosition(p);
		}

		// if at waypoint, run AI module
		else {
			GetComponent<AI>().AILogic();
		}

	}

	void RunAway()
	{
		GetComponent<Animator>().SetBool("Run", true);

        if(Time.time >= _timeToWhite && Time.time >= _timeToToggleWhite)   ToggleBlueWhite();

		// if not at waypoint, move towards it
        if (Vector3.Distance(transform.position, waypoint) > 0.000000000001)
		{
			Vector2 p = Vector2.MoveTowards(transform.position, waypoint, speed);
			GetComponent<Rigidbody2D>().MovePosition(p);
		}
		
		// if at waypoint, run AI run away logic
		else GetComponent<AI>().RunLogic();

	}

	//------------------------------------------------------------------------------
	// Utility functions
	void MoveToWaypoint(bool loop = false)
	{
		if(waypoints == null)
			return;
		waypoint = waypoints.Peek();		// get the waypoint (CHECK NULL?)
        if (Vector3.Distance(transform.position, waypoint) > 0.000000000001)	// if its not reached
		{									                        // move towards it
			_direction = Vector3.Normalize(waypoint - transform.position);	// dont screw up waypoint by calling public setter
			Vector2 p = Vector2.MoveTowards(transform.position, waypoint, speed);
			GetComponent<Rigidbody2D>().MovePosition(p);
		}
		else 	// if waypoint is reached, remove it from the queue
		{
			if(loop)	waypoints.Enqueue(waypoints.Dequeue());
			else		waypoints.Dequeue();
		}
	}

	public void Frighten()
	{
		state = State.Run;
		_direction *= -1;

        _timeToWhite = Time.time + _gm.scareLength * 0.66f;
        _timeToToggleWhite = _timeToWhite;
        GetComponent<Animator>().SetBool("Run_White", false);

	}

	public void Calm()
	{
        // if the ghost is not running, do nothing
	    if (state != State.Run) return;

		waypoints.Clear ();
		state = State.Chase;
	    _timeToToggleWhite = 0;
	    _timeToWhite = 0;
        GetComponent<Animator>().SetBool("Run_White", false);
        GetComponent<Animator>().SetBool("Run", false);
	}

    public void ToggleBlueWhite()
    {
        isWhite = !isWhite;
        GetComponent<Animator>().SetBool("Run_White", isWhite);
        _timeToToggleWhite = Time.time + _toggleInterval;
    }

}
