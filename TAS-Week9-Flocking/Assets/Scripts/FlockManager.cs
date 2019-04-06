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
        float radius = Mathf.Pow(numberOfSpawns / (4 * Mathf.PI * density), 0.33f);

        for (int i = 0; i < numberOfSpawns; i++)
        {
            _allMyAgents.Add(Instantiate(myAutoAgentPrefab, Random.insideUnitSphere * radius, Quaternion.identity, transform));
        }
    }

    Collider[] collInRad = new Collider[10];

    void Update()
    {
        foreach(GameObject g in _allMyAgents)
        {
            AutoAgentBehavior a = g.GetComponent<AutoAgentBehavior>();
            
            Physics.OverlapSphereNonAlloc(g.transform.position, detectionRadius, collInRad);

            a.PassArrayOfContext(collInRad);
        }
    }
}
