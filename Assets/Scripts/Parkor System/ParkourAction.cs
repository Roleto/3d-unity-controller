using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Parkour System/new parkour Action")]
public class ParkourAction : ScriptableObject
{
    [SerializeField] string animName;
    [SerializeField] string obstacleTag;
    
    [SerializeField] float minHeight;
    [SerializeField] float maxHeight;
    [SerializeField] float delayTime =0.2f;
    [SerializeField] float postActionDelay;
    
    [SerializeField] bool rotateToObstacle;


    [Header("Target Matching")]
    [SerializeField] bool enableTargetMatching = true;
    [SerializeField] protected AvatarTarget matchBodyPart;
    [SerializeField] float matchStartTime;
    [SerializeField] float matchTargetTime;
    [SerializeField] Vector3 matchPositionWeight = new Vector3(0,1,0);

    public string AnimName { get => animName; }
    public float DelayTime { get => delayTime; }
    public float PostActionDelay { get => postActionDelay; }
    public bool RotateToObstacle { get => rotateToObstacle; }
    public bool EnableTargetMatching { get => enableTargetMatching; }
    public AvatarTarget MatchBodyPart { get => matchBodyPart; }
    public float MatchStartTime { get => matchStartTime; }
    public float MatchTargetTime { get => matchTargetTime; }
    public Vector3 MatchPositionWeight { get => matchPositionWeight; }



    public Quaternion TargetRotation { get; set; }
    public Vector3 MatchPos { get; set; }
    public bool Mirror{ get; set; }
    public virtual bool CheckIfPossible(ObstactleHitData hitData, Transform player)
    {
        if(!string.IsNullOrEmpty(obstacleTag) && hitData.forwardHit.transform.tag != obstacleTag)
            return false;

        float height = hitData.heightdHit.point.y - player.position.y;
        if (height < minHeight || height > maxHeight)
            return false;

        if (rotateToObstacle)
            TargetRotation = Quaternion.LookRotation(-hitData.forwardHit.normal);

        if (enableTargetMatching)
            MatchPos = hitData.heightdHit.point;

        return true;
    }



}
