using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClimbControl : MonoBehaviour
{

    [SerializeField] float delayTime = .2f;
    EnviromentScanner _enviromentScanner;
    PlayerCpntroller _playerController;

    ClimbPoint currentClimbPoin;

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
                    currentClimbPoin = ledgeHit.transform.GetComponent<ClimbPoint>();
                    _playerController.SetControl(false);


                    StartCoroutine(JumpToLedge("IdleToHang", ledgeHit.transform, .47f, .61f));
                }
            }
        }
        else
        {
            //ledgeJump

            float h = Mathf.Round(Input.GetAxisRaw("Horizontal"));
            float v = Mathf.Round(Input.GetAxisRaw("Vertical"));
            var inputDir = new Vector2(h, v);

            if (_playerController.InAction || inputDir == Vector2.zero) return;

            var neigbor = currentClimbPoin.GetNeigbor(inputDir);
            if (neigbor == null) return;

            if (neigbor.connectionType == ConnectionType.Jump && Input.GetButton("Jump"))
            {
                currentClimbPoin = neigbor.point;
                //TODO Do target matching for hand
                if (neigbor.direction.y == 1)
                    StartCoroutine(JumpToLedge("ClimbJump", currentClimbPoin.transform, .34f, .65f));
                else if (neigbor.direction.y == -1)
                    StartCoroutine(JumpToLedge("ClimbDrop", currentClimbPoin.transform, .31f, .65f));
                else if (neigbor.direction.x == 1)
                    StartCoroutine(JumpToLedge("HopRight", currentClimbPoin.transform, .20f, .50f));
                else if (neigbor.direction.x == -1)
                    StartCoroutine(JumpToLedge("HopLeft", currentClimbPoin.transform, .20f, .50f));
            }else if(neigbor.connectionType == ConnectionType.Move && !_playerController.InAction)
            {
                currentClimbPoin = neigbor.point;

                if (neigbor.direction.x == 1)
                    StartCoroutine(JumpToLedge("ShimmyRight", currentClimbPoin.transform, 0f, .38f, handOffset : new Vector3(.25f,0.170f,.1f)));
                else if (neigbor.direction.x == -1)
                    StartCoroutine(JumpToLedge("ShimmyLeft", currentClimbPoin.transform, 0f, .38f, AvatarTarget.LeftHand, handOffset: new Vector3(.25f, 0.170f, .1f)));
            }
        }
    }

    IEnumerator JumpToLedge(string anim, Transform Ledge, float matchStartTime, float matchTargetTime, AvatarTarget hand = AvatarTarget.RightHand,
        Vector3? handOffset = null)
    {
        var matchParam = new MatchTargetParams(GetHandPos(Ledge,hand,handOffset), hand, Vector3.one, matchStartTime, matchTargetTime);
        var targetRot = Quaternion.LookRotation(-Ledge.forward);

        yield return _playerController.DoAction(anim, matchParam, targetRot, true, delayTime);
        _playerController.IsHanging = true;
    }

    Vector3 GetHandPos(Transform ledge, AvatarTarget hand, Vector3? handOffset)
    {
        var offVal =  (handOffset != null) ? handOffset.Value : new Vector3(.25f, .175f, .125f);
        var hdir = hand == AvatarTarget.RightHand ? ledge.right : -ledge.right;
        
        return ledge.position + ledge.forward * offVal.z + Vector3.up * offVal.y - hdir * offVal.x;
    }
}
