using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Pathfinding;
using System;

public class LChaserFSM : MonoBehaviour
{
    private Transform _AstarTarget;

    private Transform AstarTarget
    {
        get { return _AstarTarget; }
        set {

            GetComponent<AIDestinationSetter>().target = value;
            _AstarTarget = value;
        
        }
    }

    public Transform _randomTarget;

    private Timer _timer;
    private StateMachine _fsmLc;
    private LanternMeadow _targetLantern;

    [SerializeField]
    private float _eatDuration;

    #region Event Trigger
    // when any LightRegion change, will trigger this function
    public void OnRegionTypeChangeTrigger(RegionType type, Transform location) {

        if (type == RegionType.Dark) {
            //Debug.Log(location.name + " becomes Dark");
            if (location == AstarTarget)
            {
                _targetLantern = null;
                _fsmLc.Trigger("RegionChangeDark");
                //Debug.Log(location.name + "Trigger Dark");
            }

            
            return;
        }

        //Debug.Log(location.name + " becomes Bright");
        // if further than current target far LC won't go
        if (AstarTarget != _randomTarget && AstarTarget != null &&
            Vector3.Distance(this.transform.position, location.position) > Vector3.Distance(this.transform.position, AstarTarget.position)
        ) {
            //Debug.Log(location.name + " is too far");
            return;
        }
        AstarTarget = location;
        _targetLantern = location.GetComponent<LanternMeadow>();

        //Debug.Log(location.name + "Trigger Bright");
        _fsmLc.Trigger("RegionChangeBright");
    }
    #endregion

    #region FSM onLogic
    public void RandomChangeAStarTarget() {
        Vector2 rand = HelperLib.generateAnimalRandom("LChaser");
        Vector3 tmp = new Vector3(rand.x, rand.y, 0f);
        //Debug.Log("RandomChangeAStarTarget"+ tmp);
        AstarTarget.position = tmp;
    }
    public void EatGrass() {
        _targetLantern.Eaten(1);
    }
    #endregion

    // Start is called before the first frame update
    void Start() {
        _timer = new Timer();
        _targetLantern = null;

        SubscribeToLaternMeadow();

        _fsmLc = new StateMachine();
        _fsmLc.AddState("Random",
            new State(
                onEnter: (state) => {
                    Debug.Log("Enter Random");
                    AstarTarget = _randomTarget;
                    _timer.SetTimer(5.0f, RandomChangeAStarTarget, true);
                },
                onLogic: (state) => {
                    if (_timer.ManualUpdate()) {
                        _timer.SetTimer(5.0f, RandomChangeAStarTarget, true);
                    }
                },
                onExit: (state) => {
                    
                    _timer.StopTimer();
                    if (AstarTarget == _randomTarget)
                        AstarTarget = null;
                }
            )
        );
        _fsmLc.AddState("ChaseLight",
            new State(
                onEnter: (state) =>
                {
                    Debug.Log("ChaseLight");
                }
            )
        );

        _fsmLc.AddState("EatGrass",
            new State(
                onEnter: (state) =>
                {
                    Debug.Log("Eat");
                },
                onLogic: (state) =>
                {
                    transform.Rotate(Vector3.forward, 360 * Time.deltaTime / _eatDuration);
                    if (state.timer.Elapsed > _eatDuration)
                    {
                        state.timer.Reset();
                        EatGrass();
                        
                    }
                },
                onExit: (state) =>
                {
                }
            )
        );

        _fsmLc.AddTriggerTransition(
            "RegionChangeBright",
            "Random",
            "ChaseLight"
        );

        _fsmLc.AddTriggerTransition(
            "RegionChangeDark",
            "ChaseLight",
            "Random"
        );

        _fsmLc.AddTransition(
            "ChaseLight",
            "EatGrass",
            transition => AstarTarget != null && 
                          Vector3.Distance(this.transform.position, AstarTarget.position) < 0.5f
        );

        _fsmLc.AddTransition(
            "EatGrass",
            "Random",
            transition => _targetLantern == null || _targetLantern.Current <= 0
        );

        _fsmLc.SetStartState("Random");
        _fsmLc.Init();
    }

    private void SubscribeToLaternMeadow()
    {
        RegionLayerManager[] regions = GameObject.FindObjectsOfType<RegionLayerManager>();

        foreach(var region in regions)
        {
            region.OnRegionTypeChanged.AddListener(OnRegionTypeChangeTrigger);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(_fsmLc.ActiveStateName);
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
        //Debug.Log("SetTimer: " + timeLimit);
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
        if (_updateOperation == null) return false;
        return _updateOperation();
    }
}