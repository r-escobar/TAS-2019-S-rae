using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    
    #region External References
    public GameObject chunkPrefab;
    public GameObject target;
    #endregion

    #region Public Tuning Variables
    public int numChunksX = 10;
    public int numChunksZ = 10;
    public float maxViewDist = 100f;
    #endregion
     
    #region Internal Constants
    private int numVisibleChunks;
    private float chunkSize;
    #endregion

    #region Persistent Internal Outputs
    private Vector2 oldIntPos;
    private Dictionary<Vector3, GameObject> chunkDict = new Dictionary<Vector3, GameObject>();
    #endregion

    private bool firstGen = true;
       
    void Start()
    {
        chunkSize = chunkPrefab.GetComponent<ChunkExample>().sizeSquare;
        numVisibleChunks = Mathf.RoundToInt(maxViewDist / chunkSize);
    }

    void UpdateVisibleChunks()
    {
        // integer coordinates of where the target is in the chunk grid
        int curChunkCoordX = Mathf.RoundToInt(target.transform.position.x / chunkSize);
        int curChunkCoordZ = Mathf.RoundToInt(target.transform.position.z / chunkSize);

        Vector2 curIntPos = new Vector2(curChunkCoordX, curChunkCoordZ);

        // only update chunks if the target has moved to a new gric location (or if the scene has just started)
        if (curIntPos != oldIntPos || firstGen)
        {
            // border around numVisibleChunks that deactivates chunks as target moves
            int deactivateRadius = numVisibleChunks + 1;

            for (int i = -deactivateRadius; i <= deactivateRadius; i++)
            {
                for (int j = -deactivateRadius; j <= deactivateRadius; j++)
                {
                    Vector3 curChunkInViewPos = new Vector3(curChunkCoordX + j, 0f, curChunkCoordZ + i);

                    // check if we're on the deactivation border
                    if (Mathf.Abs(i) == deactivateRadius || Mathf.Abs(j) == deactivateRadius)
                    {
                        if (chunkDict.ContainsKey(curChunkInViewPos))
                        {
                            chunkDict[curChunkInViewPos].SetActive(false);
                        }
                    }
                    else
                    {
                        if (!chunkDict.ContainsKey(curChunkInViewPos))
                        {
                            // there isn't a chunk at curChunkInViewPos. create it and add it to the dictionary
                            GameObject newChunk = Instantiate(chunkPrefab, curChunkInViewPos * chunkSize,
                                Quaternion.identity, transform);
                            chunkDict.Add(curChunkInViewPos, newChunk);
                        }
                        else
                        {
                            // there's already a chunk at curChunkInViewPos. make sure it's active
                            if (!chunkDict[curChunkInViewPos].activeSelf)
                            {
                                chunkDict[curChunkInViewPos].SetActive(true);
                            }
                        }
                    }
                }
            }

            oldIntPos = curIntPos;
            firstGen = false;
        }
    }

    void Update()
    {
        UpdateVisibleChunks();
    }
}
