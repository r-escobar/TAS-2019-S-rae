using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobSpawner : MonoBehaviour
{

    public GameObject blobObject;

    public float spawnInterval = 1f;
    public float initialSpawnTime = 1f;
    private float nextSpawnTime;

    public Vector3 blobMoveAmount;
    // Start is called before the first frame update
    void Start()
    {
        //spawnInterval = Random.Range(1f, 3f);
        nextSpawnTime = Time.timeSinceLevelLoad + initialSpawnTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeSinceLevelLoad > nextSpawnTime)
        {
            SpawnBlob();
            nextSpawnTime = Time.timeSinceLevelLoad + spawnInterval;
        }
    }

    void SpawnBlob()
    {
        GameObject newBlob = Instantiate(blobObject);
        newBlob.transform.position = transform.position;
        newBlob.GetComponent<BlobMover>().moveAmount = blobMoveAmount;
    }
}
