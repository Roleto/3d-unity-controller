using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
    [SerializeField] List<ParkourAction> parkourActions;
    [SerializeField] ParkourAction jumpingDown;
    [SerializeField] float autoDropHeightLimit = 1f;

    EnviromentScanner _scanner;
    Animator _animator;
    PlayerCpntroller _playerController;

    private void Awake()
    {
        _scanner = GetComponent<EnviromentScanner>();
        _animator = GetComponent<Animator>();
        _playerController = GetComponent<PlayerCpntroller>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }
    private void FixedUpdate()
    {
        var hitdata = _scanner.ObstackleCheck();
        if (Input.GetButton("Jump") && !_playerController.InAction && !_playerController.IsHanging) 
        {

            if (hitdata.forwardHitFound)
            {
                foreach (var action in parkourActions)
                {
                    if (action.CheckIfPossible(hitdata, transform))
                    {
                        StartCoroutine(DoParkorAction(action));
                        break;
                    }
                }
            }
        }

        if (_playerController.isOnLedge && !_playerController.InAction && !hitdata.forwardHitFound)
        {
            bool shouldJump = true;
            if (_playerController.LedgeData.height > autoDropHeightLimit && !Input.GetButton("Jump"))
                shouldJump = false;

            if (shouldJump && _playerController.LedgeData.angle <= 50)
            {

                _playerController.isOnLedge = false;
                StartCoroutine(DoParkorAction(jumpingDown));
            }
        }


    }
    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator DoParkorAction(ParkourAction action)
    {
        _playerController.SetControl(false);

        MatchTargetParams matchParams = null;
        if (action.EnableTargetMatching)
        {
            matchParams = new MatchTargetParams(action);
        }

        yield return _playerController.DoAction(action.AnimName, matchParams, action.TargetRotation, action.RotateToObstacle, action.PostActionDelay, action.DelayTime, action.Mirror);

        _playerController.SetControl(true);
    }

    void MatchTarget(ParkourAction action)
    {
        if (_animator.isMatchingTarget) return;

        _animator.MatchTarget(action.MatchPos, transform.rotation, action.MatchBodyPart, new MatchTargetWeightMask(action.MatchPositionWeight, 0), action.MatchStartTime, action.MatchTargetTime);
    }
}
