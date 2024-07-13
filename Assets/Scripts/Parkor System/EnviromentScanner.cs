using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnviromentScanner : MonoBehaviour
{
    [Header("Vectors")]
    [SerializeField] Vector3 forwardRayOffset = new Vector3(0, .5f, 0);

    [Header("Lenghts")]
    [SerializeField] float forwardRayLenght = .8f;
    [SerializeField] float heightRayLenght = 5f;
    [SerializeField] float ledgeRayLenght = 10f;
    [SerializeField] float climbLedgeRayLength = 1.5f;
    [SerializeField] float ledgeHeightThrashold = 0.75f;

    [Header("Layer Masks")]
    [SerializeField] LayerMask ObstacleLayer;
    [SerializeField] LayerMask climbLedgeLayer;

    [Header("Debug")]
    [SerializeField] bool ObstackleVector;
    [SerializeField] bool LedgeVectorVector;
    [SerializeField] bool ClimbLedgeVectorVector;


    public ObstactleHitData ObstackleCheck()
    {
        var hitData = new ObstactleHitData();
        var forwardOrigin = transform.position + forwardRayOffset;
        hitData.forwardHitFound = Physics.Raycast(transform.position + forwardRayOffset, transform.forward, out hitData.forwardHit, forwardRayLenght, ObstacleLayer);

        //if (debugDraw)
        //    Debug.DrawRay(forwardOrigin, transform.forward * forwardRayLenght, (hitData.forwardHitFound) ? Color.red : Color.green);

        if (hitData.forwardHitFound)
        {
            var heightOrigin = hitData.forwardHit.point + Vector3.up * heightRayLenght;
            hitData.heightHitFound = Physics.Raycast(heightOrigin, Vector3.down, out hitData.heightdHit, heightRayLenght, ObstacleLayer);

            if (ObstackleVector)
                Debug.DrawRay(heightOrigin, Vector3.down * heightRayLenght, (hitData.heightHitFound) ? Color.red : Color.green);
        }

        return hitData;
    }

    public bool ClimbLedgeCheck(Vector3 dir, out RaycastHit ledgeHit)
    {
        ledgeHit = new RaycastHit();

        if (dir == Vector3.zero)
            return false;

        var origin = transform.position + Vector3.up * 1.5f;
        var offset = new Vector3(0, 0.18f, 0);

        for (int i = 0; i < 10; i++)
        {
            Debug.DrawRay(origin + offset * i, dir);
            if (Physics.Raycast(origin + offset * i, dir, out RaycastHit hit, climbLedgeRayLength, climbLedgeLayer))
            {
                ledgeHit = hit;
                return true;
            }
        }

        return false;
    }
    public bool ObstackleLedgeCheck(Vector3 moveDir, out LedgeDataStruct ledgeData)
    {
        ledgeData = new LedgeDataStruct();
        if (moveDir == Vector3.zero)
            return false;

        float originOffset = .55f;
        var origin = transform.position + moveDir * originOffset + Vector3.up;

        if (PhisycsUtil.ThreeRayCasts(origin, Vector3.down, .25f, transform, out List<RaycastHit> hits, ledgeRayLenght, ObstacleLayer, ObstackleVector))
        {
            var validHits = hits.Where(h => transform.position.y - h.point.y > ledgeHeightThrashold);

            if ((validHits.Count() > 0))
            {

                var surfaceRayOrigin = validHits.First().point;
                surfaceRayOrigin.y = transform.position.y - .1f;
                //var surfaceRayOrigin = transform.position + moveDir - new Vector3(0, .1f, 0);
                if (Physics.Raycast(surfaceRayOrigin, transform.position - surfaceRayOrigin, out RaycastHit surfaceHit, 2, ObstacleLayer))
                {
                    Debug.DrawLine(surfaceRayOrigin, transform.position, Color.cyan);
                    var height = transform.position.y - validHits.First().point.y;

                    if (height > ledgeHeightThrashold)
                    {
                        // we are on ledge
                        ledgeData.height = height;
                        ledgeData.angle = Vector3.Angle(transform.forward, surfaceHit.normal);
                        ledgeData.surfacehit = surfaceHit;
                        return true;
                    }
                }
            }
        }
        return false;
    }

}
public struct ObstactleHitData
{
    public bool forwardHitFound;
    public bool heightHitFound;
    public RaycastHit forwardHit;
    public RaycastHit heightdHit;
}
public struct LedgeDataStruct
{
    public float height;
    public float angle;
    public RaycastHit surfacehit;
}