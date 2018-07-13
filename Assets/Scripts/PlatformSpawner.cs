using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlatformSpawner : MonoBehaviour
{
    public Tilemap platformTM;
    public Tile singleTile, leftTile, middleTile, rightTile;
    float leftBorder, rightBorder;
    int currentRow, chunkHeight;
    public int chunkAmount = 5;
    Dictionary<int, List<List<Vector3>>> platformChunks = new Dictionary<int, List<List<Vector3>>>();
    List<List<List<Vector3>>> chunkList = new List<List<List<Vector3>>>();

    private void Awake()
    {
        InitializeChunks();
        ChunkRandomizer();
    }

    private void Start()
    {
        // leftBorder = PlatformerCamera.instance.leftBorder.transform.position.x + PlatformerCamera.instance.leftBorder.GetComponent<BoxCollider2D>().bounds.extents.x;
        //rightBorder = PlatformerCamera.instance.rightBorder.transform.position.x - PlatformerCamera.instance.rightBorder.GetComponent<BoxCollider2D>().bounds.extents.x;


        SpawnChunks();
    }

    private void Update()
    {

    }

    void ChunkRandomizer()
    {
        print("Chunk order:");
        int rand = 0, lastRand = 0;

        for (int i = 0; i < chunkAmount; i++)
        {
            while(rand == lastRand)     //to prevent the same chunking from repeating twice (or more) in a row.
            {
                rand = Random.Range(0, chunkList.Count);
            }
            platformChunks.Add(i, chunkList[rand]);
            print(rand);
            lastRand = rand;
        }
    }

    void SpawnChunks()
    {
        int currentRow = 0;
        Tile platformTile = singleTile;

        for (int iChunk = 0; iChunk < platformChunks.Count; iChunk++)       //Runs for every chunk of platforms (16 tiles high).
        {
            for (int iPlat = 0; iPlat < platformChunks[iChunk].Count; iPlat++)      //Runs for every platform in each chunk. 
            {
                for(int iTile = 0; iTile < platformChunks[iChunk][iPlat].Count; iTile++)    //Runs for every tile in each platform.
                {                                                                           //iTile is the index of the current tile of the platform (from left to right).
                    if (platformChunks[iChunk][iPlat].Count == 1)
                    {
                        platformTile = singleTile;
                    }
                    else if (platformChunks[iChunk][iPlat].Count == 2)
                    {
                        if (iTile == 0)
                        {
                            platformTile = leftTile;
                        }
                        else if(iTile == 1)
                            platformTile = rightTile;
                    }
                    else if (platformChunks[iChunk][iPlat].Count == 3)
                    {
                        if (iTile == 0)
                        {
                            platformTile = leftTile;
                        }
                        else if (iTile == 1)
                        {
                            platformTile = middleTile;
                        }
                        else if (iTile == 2)
                        {
                            platformTile = rightTile;
                        }
                    }
                    else
                    {
                        Debug.LogError("check amount of tiles spawned on same platform.", this);
                    }
                    SpawnTile(platformTile, new Vector3(platformChunks[iChunk][iPlat][iTile].x, platformChunks[iChunk][iPlat][iTile].y + currentRow));

                }

            }
            currentRow += chunkHeight;
        }

    }

    void InitializeChunks()
    {
        //Start and end of chunk should be in approximate middle.
        //Start chunk on 1 or 2 tile long platforms, end chunk on 3.
        //12 tiles high chunks, i.e yPos = 12 at last platform of chunk.
        //Tiles on the same platform are spawned from left to right..
        //..meaning that xPos is the xPos of the most left tile of the platform.
        //If the platform is to the left, xPos of the most right tile is xPos + tileAmount (aka platform length).

        chunkHeight = 12 + 4;
        int yPos = 0;
        int xPos = 0;

        ///chunk 1
        {
            List<List<Vector3>> chunk1 = new List<List<Vector3>>();
            yPos = 0;
            xPos = 3;
            AddToChunk(xPos, yPos, 2, chunk1);

            yPos += 3;
            xPos -= 6;
            AddToChunk(xPos, yPos, 2, chunk1);

            yPos += 4;
            xPos -= 3;
            AddToChunk(xPos, yPos, 2, chunk1);

            yPos += 2;
            xPos += 4;
            AddToChunk(xPos, yPos, 1, chunk1);

            yPos += 3;
            xPos += 2;
            AddToChunk(xPos, yPos, 3, chunk1);

            xPos = 7;
            AddToChunk(xPos, yPos, 1, chunk1);
            xPos = -8;
            AddToChunk(xPos, yPos, 1, chunk1);

            chunkList.Add(chunk1);
            //platformChunks.Add(0, chunk1);
        }
        ///chunk 1

        ///chunk 2
        {
            List<List<Vector3>> chunk2 = new List<List<Vector3>>();
            yPos = 0;
            xPos = 2;
            AddToChunk(xPos, yPos, 1, chunk2);

            yPos += 2;
            xPos -= 7;
            AddToChunk(xPos, yPos, 3, chunk2);

            yPos += 3;
            xPos += 7;
            AddToChunk(xPos, yPos, 2, chunk2);

            yPos += 4;
            xPos += 3;
            AddToChunk(xPos, yPos, 3, chunk2);

            yPos += 3;
            xPos -= 4;
            AddToChunk(xPos, yPos, 3, chunk2);

            xPos = 7;
            AddToChunk(xPos, yPos, 1, chunk2);
            xPos = -8;
            AddToChunk(xPos, yPos, 1, chunk2);

            chunkList.Add(chunk2);
            //platformChunks.Add(1, chunk2);
        }
        ///chunk 2

        ///chunk 3
        {
            List<List<Vector3>> chunk3 = new List<List<Vector3>>();
            yPos = 0;
            xPos = -2;
            AddToChunk(xPos, yPos, 2, chunk3);

            yPos += 2;
            xPos += 7;
            AddToChunk(xPos, yPos, 2, chunk3);

            yPos += 3;
            xPos -= 5;
            AddToChunk(xPos, yPos, 1, chunk3);

            yPos += 2;
            xPos -= 6;
            AddToChunk(xPos, yPos, 3, chunk3);

            yPos += 4;
            xPos += 2;
            AddToChunk(xPos, yPos, 2, chunk3);

            yPos += 1;
            xPos += 6;
            AddToChunk(xPos, yPos, 3, chunk3);

            xPos = 7;
            AddToChunk(xPos, yPos, 1, chunk3);
            xPos = -8;
            AddToChunk(xPos, yPos, 1, chunk3);

            chunkList.Add(chunk3);
            //platformChunks.Add(2, chunk3);
        }
        ///chunk 3

    }

    void AddToChunk(int mostLeftX, int y, int tileAmount, List<List<Vector3>> chunk)
    {
        List<Vector3> vBatch = new List<Vector3>();

        for (int i = 0; i < tileAmount; i++)
        {
            vBatch.Add(new Vector3(mostLeftX + i, y));
        }
        chunk.Add(vBatch);
    }

    void SpawnTile(Tile tile, Vector3 SpawnPos)
    {
        Vector3Int tileSpawnPos = platformTM.WorldToCell(SpawnPos);

        platformTM.SetTile(tileSpawnPos, tile);
    }

    /*
    void CheckTileJumpability()
    {
        //max distances, y = n. tiles upward, x = n. tiles sideways.
        //4y, 1x. 4y, 6x. 7x.
        //3 tiles free above/below a tile for it to be jumpable.
        //The tile above the last tile in a platform (directed towards the nearest jumpable tile) needs to be free.
        //Ex. *marks the tile that needs to be free.
        ///--   *---///
        ///     ----///
        Vector2Int maxDiff = new Vector2Int(6, 4);
        Vector2Int minDistToNearest = new Vector2Int(0, 3);



    }

    float DistanceToTile(Vector2Int orgin, Vector2Int next)
    {
        //Distance formula
        float diff = Mathf.Sqrt(Mathf.Pow(next.x - orgin.x, 2) + Mathf.Pow(next.y - orgin.y, 2));
        return diff;
    }

    Vector2Int CheckNearestTile(Vector2Int fromTile)
    {

        Vector2Int nearestPos = new Vector2Int(10, 10);
        int yDiff = 0;

        for (int i = 0; i < 6 * 4; i++)
        {
            if (i == 6)
            {
                yDiff++;
            }

            //need to check all around the fromTile... ()
            //not correct..
            Vector2Int checkingPos = (new Vector2Int(i - (6 * yDiff), yDiff + fromTile.y + 1));

            if (CheckTilePos(checkingPos))
            {
                if(DistanceToTile(fromTile, checkingPos) < DistanceToTile(fromTile, nearestPos))
                {
                    nearestPos = checkingPos;
                }
            }
        }



        return nearestPos;
    }

    bool CheckTilePos(Vector3 tilePos)
    {
        Vector3Int checkPos = platformTM.WorldToCell(tilePos);
        return platformTM.HasTile(checkPos);

    }

    bool CheckTilePos(Vector2Int tilePos)
    {
        Vector3Int checkingPos = new Vector3Int(tilePos.x, tilePos.y, 0);
        return platformTM.HasTile(checkingPos);
    }
    */


}
