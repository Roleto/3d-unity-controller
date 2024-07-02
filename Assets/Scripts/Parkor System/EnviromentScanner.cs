using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentScanner : MonoBehaviour
{
    //[Header("Ground Settings")]
    [SerializeField] Vector3 forwardRayOffset = new Vector3(0, .5f, 0);
    [SerializeField] float forwardRayLenght = .8f;
    [SerializeField] float heightRayLenght = .8f;
    [SerializeField] LayerMask ObstacleLayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public ObstactleHitData ObstackleCheck()
    {
        var hitData = new ObstactleHitData();
        var forwardOrigin = transform.position + forwardRayOffset;
        hitData.forwardHitFound=Physics.Raycast(transform.position + forwardRayOffset, transform.forward, out hitData.forwardHit, forwardRayLenght, ObstacleLayer);

        Debug.DrawRay(forwardOrigin, transform.forward * forwardRayLenght, (hitData.forwardHitFound) ? Color.red : Color.green);

        if (hitData.forwardHitFound) 
        {
            var heightOrigin = hitData.forwardHit.point + Vector3.up * heightRayLenght;
            hitData.heightHitFound = Physics.Raycast(heightOrigin, Vector3.down, out hitData.heightdHit, heightRayLenght, ObstacleLayer);
            
        Debug.DrawRay(heightOrigin, Vector3.down * heightRayLenght, (hitData.heightHitFound) ? Color.red : Color.green);
        }

        return hitData;
    }

}

public struct ObstactleHitData
{
    public bool forwardHitFound;
    public bool heightHitFound;
    public RaycastHit forwardHit;
    public RaycastHit heightdHit;
}