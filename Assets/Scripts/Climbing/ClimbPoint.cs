using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClimbPoint : MonoBehaviour
{
    [SerializeField] List<Neigbor> neigbors;
    private void Awake()
    {
        var twoWayNeigbors = neigbors.Where(x => x.isTwoWay);
        foreach (var neigbor in twoWayNeigbors)
        {
            //if (neigbor.point == null) 
            //    return;

            neigbor.point?.CreateConnection(this, -neigbor.direction, neigbor.connectionType, neigbor.isTwoWay);
        }
    }

    public void CreateConnection(ClimbPoint point, Vector2 direction, ConnectionType connectionType, bool isTwoWay = true)
    {
        var neigbor = new Neigbor(point, direction, connectionType, isTwoWay);

        neigbors.Add(neigbor);
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.blue);
        foreach (var neightbor in neigbors)
        {
            if (neightbor.point != null)
            {
                Debug.DrawLine(transform.position, neightbor.point.transform.position, (neightbor.isTwoWay) ? Color.green : Color.gray);
            }
        }
    }

}

[System.Serializable]
public class Neigbor
{
    public ClimbPoint point;
    public Vector2 direction;
    public ConnectionType connectionType;
    public bool isTwoWay = true;
    public Neigbor()
    {

    }
    public Neigbor(ClimbPoint point, Vector2 direction, ConnectionType connectionType, bool isTwoWay)
    {
        this.point = point;
        this.direction = direction;
        this.connectionType = connectionType;
        this.isTwoWay = isTwoWay;
    }
}

public enum ConnectionType
{
    Jump = 0,
    Move = 1
}
