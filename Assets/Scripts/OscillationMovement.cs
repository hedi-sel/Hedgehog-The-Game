using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscillationMovement : MonoBehaviour {
    // This class creates an oscillation movement between two given positions

    // Parameters
    public float HalfPeriod = 1; //Time the object will take to go from Start to End (aka. half-oscillation period)
    public Vector3 StartPosition; // Starting Postion
    public Vector3 EndPosition; // Ending position
    public MovementPattern Pattern;
    public bool RandomizeInitialPosition = true; // The initial position will be chosen at random between Start and End

    //Variables
    float currentTime = 0; //Enlapsed time since the start of the half-oscillation
    bool goingBack = false; //Is true when going from End to Start (and vice-versa)

    public enum MovementPattern {
        Linear,
        Quadratic
    }

    private void Start () {
        if (RandomizeInitialPosition) {
            currentTime = Random.Range(0, HalfPeriod);
            goingBack = Random.Range(0, 1) > 0.5;
            transform.localPosition = GetInterpolatePosition();
        }
    }

    // Update is called once per frame
    void Update () {
        // Update current position
        transform.localPosition = GetInterpolatePosition();

        // Update current Time and state
        currentTime += (goingBack) ? -Time.deltaTime : Time.deltaTime;
        if (currentTime > HalfPeriod) {
            currentTime = 2 * HalfPeriod - currentTime;
            goingBack = true;
        } else if (currentTime < 0) {
            currentTime = -currentTime;
            goingBack = false;
        }
    }

    Vector3 GetInterpolatePosition () { // Returns an interpolation between Start and End of factor p between 0 and 1
        float p = currentTime / HalfPeriod;
        switch (Pattern) {
            case MovementPattern.Linear:
                return p * StartPosition + (1 - p) * EndPosition;
            case MovementPattern.Quadratic:
                return (p * p * StartPosition + (1 - p) * (1 - p) * EndPosition) / (p * p + (1 - p) * (1 - p));
            default:
                return Vector3.zero;
        }
    }
}
