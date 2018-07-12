using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class AI : MonoBehaviour {

	public Transform target;

	public GhostMove ghost;

	public Vector3Int nextTile;
	public Vector3Int targetTile;
	Vector3Int currentTile;

	public Tilemap TileMap;
	public TileBase redTile;

	private const float RADIUS = 1.5f;
	private GridLayout gridLayout;
	private const string DEBUG_TILE = "red";
	private const float COLLISION_FRACTION = 0.15f;

	void Awake()
	{
		if(ghost == null)	Debug.Log ("game object ghost not found");
		target = GameObject.Find("pacman").transform;
	}

	private bool IsIntersection(Vector3 pos){
		int count = 0;
		if(!IsWallDown(pos)) count++;
		if(!IsWallUp(pos)) count++;
		if(!IsWallLeft(pos)) count++;
		if(!IsWallRight(pos)) count++;
		return count > 2;
	}
	private bool IsWall(Vector3Int pos){
		TileBase tile = TileMap.GetTile(pos);
		return tile != null;
	}
	public bool IsWallDown(Vector3 pos){
		if (pos == null)
			return false;
		if(gridLayout == null)
			gridLayout = TileMap.GetComponentInParent<GridLayout>();
		Vector3 nextWorldPos = pos + Vector3.down * RADIUS;
		Vector3 nextWorldPos1 = nextWorldPos + Vector3.left * COLLISION_FRACTION;
		Vector3 nextWorldPos2 = nextWorldPos + Vector3.right * COLLISION_FRACTION;
		Vector3Int down1 = gridLayout.WorldToCell(nextWorldPos1);
		Vector3Int down2 = gridLayout.WorldToCell(nextWorldPos2);
		TileBase tile1 = TileMap.GetTile(down1);
		TileBase tile2 = TileMap.GetTile(down2);
	//	TileMap.SetTile(down1, redTile);
	//	TileMap.SetTile(down2, redTile);
		return tile1 != null || tile2 != null;
	}
	public bool IsWallUp(Vector3 pos){
		if (pos == null)
			return false;
		if(gridLayout == null)
			gridLayout = TileMap.GetComponentInParent<GridLayout>();
		Vector3 nextWorldPos = pos + Vector3.up * RADIUS;
		Vector3 nextWorldPos1 = nextWorldPos + Vector3.left * COLLISION_FRACTION;
		Vector3 nextWorldPos2 = nextWorldPos + Vector3.right * COLLISION_FRACTION;
		Vector3Int up1 = gridLayout.WorldToCell(nextWorldPos1);
		Vector3Int up2 = gridLayout.WorldToCell(nextWorldPos2);
		TileBase tile1 = TileMap.GetTile(up1);
		TileBase tile2 = TileMap.GetTile(up2);
	//	TileMap.SetTile(up1, redTile);
	//	TileMap.SetTile(up2, redTile);
		return tile1 != null || tile2 != null;
	}
	private bool IsWall(TileBase tile){
		if(tile == null)
			return false;
		return tile.name != DEBUG_TILE;
	}
	public bool IsWallLeft(Vector3 pos){
		if (pos == null)
			return false;
		if(gridLayout == null)
			gridLayout = TileMap.GetComponentInParent<GridLayout>();

		Vector3 nextWorldPos = pos + Vector3.left * RADIUS;
		Vector3 nextWorldPos1 = nextWorldPos + Vector3.up * COLLISION_FRACTION;
		Vector3 nextWorldPos2 = nextWorldPos + Vector3.down * COLLISION_FRACTION;
		Vector3Int left1 = gridLayout.WorldToCell(nextWorldPos1);
		Vector3Int left2 = gridLayout.WorldToCell(nextWorldPos2);
		TileBase tile1 = TileMap.GetTile(left1);
		TileBase tile2 = TileMap.GetTile(left2);
		// TileMap.SetTile(left1, redTile);
		// TileMap.SetTile(left2, redTile);
		return 	IsWall(tile1) || IsWall(tile2);
		}
	public bool IsWallRight(Vector3 pos){
		if (pos == null)
			return false;
		if(gridLayout == null)
			gridLayout = TileMap.GetComponentInParent<GridLayout>();
		Vector3 nextWorldPos = pos + Vector3.right * RADIUS;
		Vector3 nextWorldPos1 = nextWorldPos + Vector3.up * COLLISION_FRACTION;
		Vector3 nextWorldPos2 = nextWorldPos + Vector3.down * COLLISION_FRACTION;
		Vector3Int right1 = gridLayout.WorldToCell(nextWorldPos1);
		Vector3Int right2 = gridLayout.WorldToCell(nextWorldPos2);
		TileBase tile1 = TileMap.GetTile(right1);
		TileBase tile2 = TileMap.GetTile(right2);
	//	TileMap.SetTile(right1, redTile);
	//	TileMap.SetTile(right2, redTile);
		return tile1 != null || tile2 != null;
	}

	public void AILogic(){
		targetTile = GetTargetTilePerGhost();
		FollowTarget(targetTile);
	}

	public void FollowTargetOld(Vector3 target){
		Vector3 currentPos = new Vector3(transform.position.x, transform.position.y);
		GridLayout gridLayout = TileMap.GetComponentInParent<GridLayout>();
		Vector3Int tilePos = gridLayout.WorldToCell(currentPos);
		float dist1, dist2, dist3, dist4;
		float max_dist = 999999f;
		dist1 = dist2 = dist3 = dist4 = max_dist;
		if(!IsWallUp(currentPos)) dist1 = distance(targetTile, new Vector3Int(tilePos.x, tilePos.y+1, tilePos.z));
        if(!IsWallDown(currentPos)) dist2 = distance(targetTile, new Vector3Int(tilePos.x, tilePos.y - 1, tilePos.z));
        if(!IsWallLeft(currentPos)) dist3 = distance(targetTile, new Vector3Int(tilePos.x - 1, tilePos.y, tilePos.z));
        if(!IsWallRight(currentPos)){
			dist4 = distance(targetTile, new Vector3Int(tilePos.x + 1, tilePos.y, tilePos.z));
		}


        float min = Mathf.Min(dist1, dist2, dist3, dist4);
        if (min == dist1) ghost.direction = Vector3.up;
        if (min == dist2) ghost.direction = Vector3.down;
        if (min == dist3) ghost.direction = Vector3.left;
        if (min == dist4) ghost.direction = Vector3.right;

		if (min == max_dist) ghost.direction = Vector3.zero;

    }
	public void FollowTarget(Vector3Int targetTile)
	{
		// get current tile
		//Vector3 currentPos = new Vector3(transform.position.x + 0.499f, transform.position.y + 0.499f);
		Vector3 currentPos = new Vector3(transform.position.x, transform.position.y);
		if(gridLayout == null)
			gridLayout = TileMap.GetComponentInParent<GridLayout>();
		Vector3Int tilePos = gridLayout.WorldToCell(currentPos);
		currentTile = tilePos;
		Vector3Int nextPos = Vector3Int.zero;

		Vector3 nextWorldPos = Vector3.zero;
		// get the next tile according to direction
		if(ghost.direction.x > 0){
			nextWorldPos = currentPos + Vector3.right * RADIUS;
			nextPos = gridLayout.WorldToCell(nextWorldPos);
		}
		if(ghost.direction.x < 0){
			nextWorldPos = currentPos + Vector3.left * RADIUS;
			nextPos = gridLayout.WorldToCell(nextWorldPos);
		}
		if(ghost.direction.y > 0){
			nextWorldPos = currentPos + Vector3.up * RADIUS;
			nextPos = gridLayout.WorldToCell(nextWorldPos);
		}
		if(ghost.direction.y < 0){
			nextWorldPos = currentPos + Vector3.down * RADIUS;
			nextPos = gridLayout.WorldToCell(nextWorldPos);
		}
		Debug.DrawLine(currentPos, nextWorldPos);
		//TileMap.SetTile(nextPos, redTile);
		if(IsWall(nextPos) || IsIntersection(currentPos))
		{
			//---------------------
			// IF WE BUMP INTO WALL
			if(IsWall(nextPos) && !IsIntersection(currentPos))
			{
				// if ghost moves to right or left and there is wall next tile
				if(ghost.direction.x != 0)
				{
					if(IsWallDown(currentPos))	ghost.direction = Vector3.up;
					else 							ghost.direction = Vector3.down;

				}

				// if ghost moves to up or down and there is wall next tile
				else if(ghost.direction.y != 0)
				{
					if(IsWallLeft(currentPos))	ghost.direction = Vector3.right;
					else 							ghost.direction = Vector3.left;

				}

			}

			//---------------------------------------------------------------------------------------
			// IF WE ARE AT INTERSECTION
			// calculate the distance to target from each available tile and choose the shortest one
			if(IsIntersection(currentPos))
			{
				float dist1, dist2, dist3, dist4;
				dist1 = dist2 = dist3 = dist4 = 999999f;
				if(!IsWallUp(currentPos) && !(ghost.direction.y < 0)) 		dist1 = distance(targetTile, new Vector3Int(tilePos.x, tilePos.y+1, tilePos.z));
				if(!IsWallDown(currentPos) &&  !(ghost.direction.y > 0)) 	dist2 = distance(targetTile, new Vector3Int(tilePos.x, tilePos.y-1, tilePos.z));
				if(!IsWallLeft(currentPos) && !(ghost.direction.x > 0)) 	dist3 = distance(targetTile, new Vector3Int(tilePos.x-1, tilePos.y, tilePos.z));
				if(!IsWallRight(currentPos) && !(ghost.direction.x < 0))	dist4 = distance(targetTile, new Vector3Int(tilePos.x+1, tilePos.y, tilePos.z));

				float min = Mathf.Min(dist1, dist2, dist3, dist4);
				if(min == dist1) ghost.direction = Vector3.up;
				if(min == dist2) ghost.direction = Vector3.down;
				if(min == dist3) ghost.direction = Vector3.left;
				if(min == dist4) ghost.direction = Vector3.right;

			}

		}

		// if there is no decision to be made, designate next waypoint for the ghost
		else
		{
			ghost.direction = ghost.direction;	// setter updates the waypoint
		}
	}

	public float distance(Vector3Int v1, Vector3Int v2)
	{
		return Mathf.Sqrt( Mathf.Pow(v1.x - v2.x, 2) + Mathf.Pow(v1.y - v2.y, 2));
	}
	public void RunLogic()
	{
		// get current tile

		Vector3 currentPos = new Vector3(transform.position.x + 0.499f, transform.position.y + 0.499f);
		GridLayout gridLayout = TileMap.GetComponentInParent<GridLayout>();
		Vector3Int tilePos = gridLayout.WorldToCell(currentPos);
		Vector3Int nextPos = Vector3Int.zero;
		currentTile = tilePos;

		// get the next tile according to direction
		if(ghost.direction.x > 0){
			nextPos = new Vector3Int(tilePos.x + 1, tilePos.y, tilePos.z);
		}
		if(ghost.direction.x < 0){
			nextPos =new Vector3Int(tilePos.x - 1, tilePos.y, tilePos.z);
		}
		if(ghost.direction.y > 0){
			nextPos = new Vector3Int(tilePos.x, tilePos.y+1, tilePos.z);
		}
		if(ghost.direction.y < 0){
			nextPos = new Vector3Int(tilePos.x, tilePos.y-1, tilePos.z);
		}
		//Debug.Log (ghost.direction.x + " " + ghost.direction.y);
		//Debug.Log (ghost.name + ": Next Tile (" + nextTile.x + ", " + nextTile.y + ")" );

		if(IsWall(nextPos) || IsIntersection(currentPos))
		{
			//---------------------
			// IF WE BUMP INTO WALL
			if(IsWall(nextPos) && !IsIntersection(currentPos))
			{
				// if ghost moves to right or left and there is wall next tile
				if(ghost.direction.x != 0)
				{
					if(IsWallDown(currentPos))	ghost.direction = Vector3.up;
					else 							ghost.direction = Vector3.down;

				}

				// if ghost moves to up or down and there is wall next tile
				else if(ghost.direction.y != 0)
				{
					if(IsWallLeft(currentPos))	ghost.direction = Vector3.right;
					else 							ghost.direction = Vector3.left;

				}

			}

			//---------------------------------------------------------------------------------------
			// IF WE ARE AT INTERSECTION
			// choose one available option at random
			if(IsIntersection(currentPos))
			{
				List<Vector3Int> availableTiles = new List<Vector3Int>();
				Vector3Int chosenTile;

				if(!IsWallUp(currentPos) && !(ghost.direction.y < 0)) 		availableTiles.Add(new Vector3Int(tilePos.x, tilePos.y+1, tilePos.z));
				if(!IsWallDown(currentPos) &&  !(ghost.direction.y > 0)) 	availableTiles.Add(new Vector3Int(tilePos.x, tilePos.y-1, tilePos.z));
				if(!IsWallLeft(currentPos) && !(ghost.direction.x > 0)) 	availableTiles.Add(new Vector3Int(tilePos.x-1, tilePos.y, tilePos.z));
				if(!IsWallRight(currentPos) && !(ghost.direction.x < 0))	availableTiles.Add(new Vector3Int(tilePos.x+1, tilePos.y, tilePos.z));

				int rand = Random.Range(0, availableTiles.Count);
				chosenTile = availableTiles[rand];
				ghost.direction = Vector3.Normalize(new Vector3(chosenTile.x - tilePos.x, chosenTile.y - tilePos.y, 0));
				//Debug.Log (ghost.name + ": Chosen Tile (" + chosenTile.x + ", " + chosenTile.y + ")" );
			}

		}

		// if there is no decision to be made, designate next waypoint for the ghost
		else
		{
			ghost.direction = ghost.direction;	// setter updates the waypoint
		}
	}


	Vector3Int GetTargetTilePerGhost()
	{
		Vector3Int dir;

        Vector3 targetPos = new Vector3(target.position.x + 0.499f, target.position.y + 0.499f);
		GridLayout gridLayout = TileMap.GetComponentInParent<GridLayout>();
		Vector3Int targetTile = gridLayout.WorldToCell(targetPos);
		Vector2 vecDir = target.GetComponent<PlayerController>().getDir();
		dir = new Vector3Int((int)vecDir.x, (int)vecDir.y, 0);

		// get the target tile position (round it down to int so we can reach with Index() function)
		switch(name)
		{
		case "blinky":	// target = pacman
			break;
		case "pinky":	// target = pacman + 4*pacman's direction (4 steps ahead of pacman)
			targetPos = targetTile + dir + dir + dir + dir;

			// if pacmans going up, not 4 ahead but 4 up and 4 left is the target
			// read about it here: http://gameinternals.com/post/2072558330/understanding-pac-man-ghost-behavior
			// so subtract 4 from X coord from target position
			if(dir == Vector3Int.up)	targetTile -= new Vector3Int(4, 0, 0);

			break;
		case "inky":	// target = ambushVector(pacman+2 - blinky) added to pacman+2
			Vector3 blinkyPos = GameObject.Find ("blinky").transform.position;
			Vector3Int blinkyTile = gridLayout.WorldToCell(blinkyPos);
			Vector3Int ambushVector = targetTile + dir + dir - blinkyTile ;
			targetTile = targetTile+ dir+dir + ambushVector;
			break;
		case "clyde":
			if(distance(targetTile, currentTile) < 9)
				targetTile = new Vector3Int(0, 2, 0);
			break;
		default:
			targetTile = Vector3Int.zero;
			break;

		}
		return targetTile;
	}
}
