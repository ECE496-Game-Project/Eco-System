using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Pathfinding;
using System;

public class LChaserFSM : MonoBehaviour
{
    public Transform _AstarTarget;

    private Timer _timer;
    private StateMachine _fsmLc;
    private LanternMeadow _targetLantern;

    #region Event Trigger
    // when any LightRegion change, will trigger this function
    public void OnRegionTypeChangeTrigger(RegionType type, LanternMeadow lantern) {
        if (type == RegionType.Dark && lantern.transform.position == _AstarTarget.position) {
            _targetLantern = null;

            _fsmLc.Trigger("RegionChangeDark");
        }

        // if further than current target far LC won't go
        if (_targetLantern != null &&
            Vector3.Distance(this.transform.position, lantern.transform.position) > Vector3.Distance(this.transform.position, _AstarTarget.position)
        ) {
            return;
        }
        _AstarTarget.position = lantern.transform.position;
        _targetLantern = lantern;

        _fsmLc.Trigger("RegionChangeBright");
    }
    #endregion

    #region FSM onLogic
    public void RandomChangeAStarTarget() {
        Vector2 rand = HelperLib.generateAnimalRandom("LChaser");
        Vector3 tmp = new Vector3(rand.x, rand.y, 0f);
        Debug.Log("RandomChangeAStarTarget"+ tmp);
        _AstarTarget.position = tmp;
    }
    public void EatGrass() {
        _targetLantern.Eaten(1);
    }
    #endregion

    // Start is called before the first frame update
    void Start() {
        _timer = new Timer();
        _targetLantern = null;

        _fsmLc = new StateMachine();
        _fsmLc.AddState("Random",
            new State(
                onEnter: (state) => {
                    _timer.SetTimer(5.0f, RandomChangeAStarTarget, true);
                },
                onLogic: (state) => {
                    if (_timer.ManualUpdate()) {
                        _timer.SetTimer(5.0f, RandomChangeAStarTarget, true);
                    }
                },
                onExit: (state) => {
                    _timer.StopTimer();
                }
            )
        );
        //_fsmLc.AddState("ChaseLight",
        //    new State(
        //        onEnter: (state) => {
        //            Debug.Log("ChaseLight");
        //        }
        //    )
        //);

        //_fsmLc.AddState("EatGrass",
        //    new State(
        //        onEnter: (state) => {
        //        },
        //        onLogic: (state) => {
        //        },
        //        onExit: (state) => {
        //        }
        //    )
        //);

        //_fsmLc.AddTriggerTransition(
        //    "RegionChangeBright",
        //    "Random",
        //    "ChaseLight"
        //);

        //_fsmLc.AddTriggerTransition(
        //    "RegionChangeDark",
        //    "ChaseLight",
        //    "Random"
        //);

        //_fsmLc.AddTransition(
        //    "ChaseLight",
        //    "EatGrass",
        //    transition => Vector3.Distance(this.transform.position, _AstarTarget.position) < 0.5f
        //);

        //_fsmLc.AddTransition(
        //    "EatGrass",
        //    "Random",
        //    transition => _targetLantern.Current <= 0
        //);

        _fsmLc.SetStartState("Random");
        _fsmLc.Init();
    }

    // Update is called once per frame
    void Update()
    {
        _fsmLc.OnLogic();
    }
}

public static class HelperLib {
    public static Vector2 generateAnimalRandom(string animal) {
        LayerMask groundLayer = 1 << LayerMask.NameToLayer("Ground");
        LayerMask obstacleLayer = 1 << LayerMask.NameToLayer("Obstacle");
        LayerMask bregionLayer = 1 << LayerMask.NameToLayer("BrightRegion");
        LayerMask lhunterLayer = 1 << LayerMask.NameToLayer("LHunter");

        while (true) {
            
            Vector3 rand = new Vector2(
                UnityEngine.Random.Range(-15f, 15f),
                UnityEngine.Random.Range(-10f, 10f)
            );
            if (!isPointInRegion(rand, groundLayer)) continue;
            if (isPointInRegion(rand, obstacleLayer)) continue;

            switch (animal) {
                case "LChaser":
                    if (isPointInRegion(rand, lhunterLayer)) break;
                    return rand;

                case "LHunter":
                    if (isPointInRegion(rand, bregionLayer)) break;
                return rand;
            }
        }
    }
    
    public static bool isPointInRegion(Vector2 point, LayerMask layermask) {
        return Physics2D.OverlapPoint(point, layermask) != null;
    }
}

public class Timer {

    private float _currTime;
    private float _timeLimit;
    private bool _pause;

    private Action _timerOperation;
    private Func<bool> _updateOperation;


    public Timer(){
        _currTime = 0f;
        _timeLimit = 0f;

        _timerOperation = null;
        _updateOperation = null;

        _pause= true;
    }

    public void SetTimer(float timeLimit, Action timerOperation, bool loopOnce) {
        Debug.Log("SetTimer: " + timeLimit);
        _currTime = 0f;
        _timeLimit = timeLimit;

        _timerOperation = timerOperation;

        _updateOperation = loopOnce ? LoopOnce : LoopInfinite;
        _pause = false;
    }

    public void ResetTimer() {
        _currTime = 0f;
    }

    public void StopTimer() {
        _currTime = 0f;
        _timeLimit = 0f;

        _timerOperation = null;
        _updateOperation = null;

        _pause = true;
    }

    public bool LoopOnce() {
        if (!_pause) {
            _currTime += Time.deltaTime;
            if (_currTime >= _timeLimit) {
                _timerOperation?.Invoke();
                StopTimer();
                return true;
            }
        }
        return false;
    }

    public bool LoopInfinite() {
        if (!_pause) {
            _currTime += Time.deltaTime;
            if (_currTime >= _timeLimit) {
                _timerOperation?.Invoke();
                ResetTimer();
                return true;
            }
        }
        return false;
    }

    public bool ManualUpdate() {
        return _updateOperation();
    }
}