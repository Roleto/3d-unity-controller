using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
    bool inAction;

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
        if (Input.GetButton("Jump") && !inAction)
        {

            var hitdata = _scanner.ObstackleCheck();
            if (hitdata.forwardHitFound)
            {
                StartCoroutine(DoParkorAction());
            }
        }
        
    }
    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator DoParkorAction()
    {
        inAction = true;
        _playerController.SetControl(false);

        _animator.CrossFade("StepUp", .2f);
        yield return null;

        var animaState = _animator.GetNextAnimatorStateInfo(0);

        yield return new WaitForSeconds(animaState.length);

        _playerController.SetControl(true);
        inAction = false;
    }
}
