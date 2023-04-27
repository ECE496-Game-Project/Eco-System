using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Pathfinding;

public class LChaserFSM : MonoBehaviour
{
    private StateMachine _fsmLc;
    private Transform _astarTarget;
    // Start is called before the first frame update
    void Start()
    {
        _fsmLc = new StateMachine();

        _fsmLc.AddState("Random",
            new State(

            )
        );

    }

    // Update is called once per frame
    void Update()
    {

    }
}

public static class HelperLib {
    public static Vector3 generatePointInRegion(string tag) {
        return new Vector3(0, 0, 0);
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
