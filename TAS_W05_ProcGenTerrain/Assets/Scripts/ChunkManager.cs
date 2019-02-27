using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public GameObject chunkPrefab;
    public GameObject target;

    public int numChunksX = 10;
    public int numChunksZ = 10;
    
    public float maxViewDist = 100f;
    

    private Vector3 _intPos;
    private Vector3 _currentIntPos;
    private Vector3 _oldIntPos;
    
    private int numVisibleChunks;

    private float chunkSize;

    private Dictionary<Vector3, GameObject> chunkDict = new Dictionary<Vector3, GameObject>();

    private MeshFilter masterMF;

    private Vector2 oldIntPos;

    private bool firstGen = true;
    
    //public List<Vector3> chunkList = new List<Vector3>();

    void Awake()
    {
        masterMF = GetComponent<MeshFilter>();
    }
    
    void Start()
    {
        chunkSize = chunkPrefab.GetComponent<ChunkExample>().sizeSquare;
        numVisibleChunks = Mathf.RoundToInt(maxViewDist / chunkSize);
    }

    void UpdateVisibleChunks()
    {
        int curChunkCoordX = Mathf.RoundToInt(target.transform.position.x / chunkSize);
        int curChunkCoordZ = Mathf.RoundToInt(target.transform.position.z / chunkSize);

        Vector2 curIntPos = new Vector2(curChunkCoordX, curChunkCoordZ);

        if (curIntPos != oldIntPos || firstGen)
        {
            int deactivateRadius = numVisibleChunks + 1;

            List<CombineInstance> combineList = new List<CombineInstance>();

            for (int i = -deactivateRadius; i <= deactivateRadius; i++)
            {
                for (int j = -deactivateRadius; j <= deactivateRadius; j++)
                {
                    Vector3 curChunkInViewPos = new Vector3(curChunkCoordX + j, 0f, curChunkCoordZ + i);

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
                            GameObject newChunk = Instantiate(chunkPrefab, curChunkInViewPos * chunkSize,
                                Quaternion.identity, transform);
                            chunkDict.Add(curChunkInViewPos, newChunk);
                        }
                        else
                        {
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
