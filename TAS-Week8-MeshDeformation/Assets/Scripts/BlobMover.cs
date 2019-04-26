using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BlobMover : MonoBehaviour
{
    public Vector3 moveAmount;
    public float moveDuration;
    public Ease moveEase;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.DOMove(transform.position + moveAmount, moveDuration).SetEase(moveEase).OnComplete(KillSelf);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void KillSelf()
    {
        Destroy(gameObject);
    }
}
