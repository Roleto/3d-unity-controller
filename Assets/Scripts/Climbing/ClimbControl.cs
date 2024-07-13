using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbControl : MonoBehaviour
{
    [SerializeField] float delayTime = .2f;
    [SerializeField] float startTime = .41f;
    [SerializeField] float targetTime = .54f;
    EnviromentScanner _enviromentScanner;
    PlayerCpntroller _playerController;

    private void Awake()
    {
        _enviromentScanner = GetComponent<EnviromentScanner>();
        _playerController = GetComponent<PlayerCpntroller>();
    }
    private void Update()
    {
        if (!_playerController.IsHanging)
        {
            if (Input.GetButton("Jump") && !_playerController.InAction)
            {
                if (_enviromentScanner.ClimbLedgeCheck(transform.forward, out RaycastHit ledgeHit))
                {
                    _playerController.SetControl(false);
                    

                    StartCoroutine(JumpToLedge("IdleToHang", ledgeHit.transform, startTime, targetTime));
                }
            }
        }
        else
        {
            //ledgeJump
        }
    }

    IEnumerator JumpToLedge(string anim, Transform Ledge, float matchStartTime, float matchTargetTime)
    {
        var matchParam = new MatchTargetParams()
        {
            pos = GetHandPos(Ledge),
            bodyPart = AvatarTarget.RightHand,
            startTime = matchStartTime,
            targetTime = matchTargetTime,
            posWieght = Vector3.one
        };
        Debug.Log(matchParam.pos);
        var targetRot = Quaternion.LookRotation(-Ledge.forward);
            
            yield return _playerController.DoAction(anim, matchParam, targetRot, true, delayTime);
        _playerController.IsHanging = true;
    }

    Vector3 GetHandPos(Transform ledge) 
    {
        return ledge.position - ledge.forward * .1f + Vector3.up * .1f - Vector3.right * .25f;
    }
}
