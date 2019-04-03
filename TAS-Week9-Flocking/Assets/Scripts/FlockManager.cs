using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    public GameObject myAutoAgentPrefab;

    [Range(1, 5000)] public int numberOfSpawns;
    public float density = 0.01f;
    public float detectionRadius = 5f;

    List<GameObject> _allMyAgents = new List<GameObject>();
    
    void Start()
    {
        float rCubed = 3 * numberOfSpawns / (4 * Mathf.PI * density);
        float r = Mathf.Pow(rCubed, .33f);

        for (int i = 0; i < numberOfSpawns; i++)
        {
            _allMyAgents.Add(Instantiate(myAutoAgentPrefab, Random.insideUnitSphere * r, Quaternion.identity, transform));
        }
    }

    Collider[] collInRad = new Collider[1];

    void Update()
    {
        foreach(GameObject g in _allMyAgents)
        {
            AutoAgentBehavior a = g.GetComponent<AutoAgentBehavior>();
            
            Physics.OverlapSphereNonAlloc(g.transform.position, detectionRadius, collInRad);

            // Currently getting a ref to itself so may do something weird

            a.PassArrayOfContext(collInRad);
        }
    }
}
