using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Pathfinding;
using System;

public class LChaserFSM : MonoBehaviour
{
    private Timer _timer;
    private StateMachine _fsmLc;
    public Transform _astarTarget;
    private LanternMeadow _targetLantern;

    public void RandomChangeAStarTarget() {
        HelperLib.generateAnimalRandom("LChaser", _astarTarget);
    }

    public void OnRegionTypeChangeChaser(RegionType type, LanternMeadow lantern) {
        if (type == RegionType.Dark && lantern.transform.position == _astarTarget.position) {
            _targetLantern = null;
        }

        // if further than current target far LC won't go
        if (_targetLantern != null && 
            Vector3.Distance(this.transform.position, lantern.transform.position) > Vector3.Distance(this.transform.position, _astarTarget.position)
        ) {
            return;
        }
        _astarTarget.position = lantern.transform.position;
        _targetLantern = lantern;
    }

    // Start is called before the first frame update
    void Start()
    {
        _timer = new Timer();
        _targetLantern = null;

        _fsmLc = new StateMachine();
        _fsmLc.AddState("Random",
            new State(
                onEnter: (state) => { 
                    _timer.SetRandomTimer(RandomChangeAStarTarget); 
                },
                onLogic: (state) => {
                    // will call RandomChangeAStarTarget once time reach
                    _timer.manualUpdate();
                },
                onExit: (state) => {
                    _timer.StopTimer();
                }
            )
        );
        _fsmLc.AddState("ChaseLight",
            new State(
                onEnter: (state) => {
                    Debug.Log("Start Chase Light");
                }
            )
        );

        _fsmLc.AddState("EatGrass",
            new State(
                onEnter: (state) => { _timer.StopTimer(); }
            )
        );


    }

    // Update is called once per frame
    void Update()
    {

    }
}

public static class HelperLib {
    public static void generateAnimalRandom(string animal, Transform target) {

        while (true) {
            
            Vector3 rand = new Vector3(
                UnityEngine.Random.Range(-15f, 15f),
                UnityEngine.Random.Range(-10f, 10f),
                0.0f
            );

            if (isPointInRegion(rand, "Obstacle")) continue;

                switch (animal) {
                    case "LChaser":
                        if (isPointInRegion(rand, "LHunter")) break;
                        target.position = rand;
                        return;

                    case "LHunter":
                        if (isPointInRegion(rand, "BrightRegion")) break;
                        target.position = rand;
                        return;
            }
        }
    }
    
    public static bool isPointInRegion(Vector3 point, string tag) {
        // Get all objects with the "Obstacle" tag
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag(tag);

        // Loop through each obstacle and check if the point is within its box collider
        foreach (GameObject obstacle in obstacles) {
            BoxCollider2D collider = obstacle.GetComponent<BoxCollider2D>();

            if (collider != null && collider.bounds.Contains(point)) {
                // The point is within the obstacle's box collider
                return true;
            }
        }

        return false;
    }
}

public class Timer {
    private float _currTime;
    private float _timeLimit;
    private bool manual;
    private bool pause;

    private Action _timerOperation;

    public Timer() {
        _currTime = 0f;
        _timeLimit = 0f;

        _timerOperation = null;

        manual = false;
        pause= true;
    }

    public void SetTimer(float timeLimit, Action timerOperation) {
        _currTime = 0f;
        _timeLimit = timeLimit;

        _timerOperation = timerOperation;
        
        manual = false;
        pause = false;
    }

    public void SetManualTimer(float timeLimit, Action timerOperation) {
        _currTime = 0f;
        _timeLimit = timeLimit;

        _timerOperation = timerOperation;

        manual = true;
    }

    public void SetRandomTimer(Action timerOperation) {
        _currTime = 0f;
        _timeLimit = UnityEngine.Random.Range(1.0f, 5.0f);

        _timerOperation = timerOperation;
        
        manual = true;
    }

    public void ResetTimer() {
        _currTime = 0f;
    }

    public void StopTimer() {
        _currTime = 0f;
        _timeLimit = 0f;

        _timerOperation = null;

        manual = false;
        pause = true;
    }

    // automaticly update
    private void Update() {
        if (manual) return;
        if (!pause) {
            _currTime += Time.deltaTime;
            if (_currTime >= _timeLimit) {
                _timerOperation?.Invoke();
                ResetTimer();
            }
        }
    }

    public void manualUpdate() {
        _currTime += Time.deltaTime;
        if (_currTime >= _timeLimit) {
            _timerOperation?.Invoke();
            ResetTimer();
        }
    }
}