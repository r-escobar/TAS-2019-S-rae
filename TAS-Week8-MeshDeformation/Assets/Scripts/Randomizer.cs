using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomizer : MonoBehaviour
{
    public float posVar = 1f;
    public float sizeVar = 0.1f;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.position += new Vector3(Random.Range(-posVar, posVar), Random.Range(-posVar, posVar), 0f);
        transform.localScale *= Random.Range(1f - sizeVar, 1f + sizeVar);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
