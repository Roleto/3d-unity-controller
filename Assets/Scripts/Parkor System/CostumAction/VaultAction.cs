using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Parkour System/Costum Actions/New Vault Action")]

public class VaultActions : ParkourAction
{
    public override bool CheckIfPossible(ObstactleHitData hitData, Transform player)
    {
        if( !base.CheckIfPossible(hitData, player))
            return false;

        var hitPoint= hitData.forwardHit.transform.InverseTransformPoint(hitData.forwardHit.point);
        if(hitPoint.z < 0 && hitPoint.x < 0 || hitPoint.z > 0 && hitPoint.x > 0)
        {
           Mirror = true;
            matchBodyPart = AvatarTarget.RightHand;
        }
        else
        {
           Mirror = false;
            matchBodyPart = AvatarTarget.LeftHand;
        }

        return true;
    }
}
