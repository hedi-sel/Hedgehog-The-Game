using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hedgehog : MonoBehaviour {
    //This class handles the character (a hedgehog)

    // Parameters
    public float WalkSpeed = 10;

    // Components of the gameObject that we will use
    public static string Name;
    Rigidbody myRigidbody;
    Animator myAnimator;
    void Awake () {
        myRigidbody = GetComponent<Rigidbody>();
        myAnimator = GetComponent<Animator>();
        Name = gameObject.name;
    }


    Vector3 WalkVector;
    void Start () {
        WalkVector = new Vector3(0, 0, WalkSpeed);
    }

    // Managing the state of the Hedgehog
    enum State {
        Walk,
        Idle
    }
    State currentState = State.Idle; //Default state
    State CurrentState { // Managing state toggling
        get { return currentState; }
        set {
            if (value == currentState) return;
            currentState = value;
            switch (currentState) {
                case State.Walk:
                    myAnimator.SetBool("isWalk", true);
                    break;
                case State.Idle:
                    myAnimator.SetBool("isWalk", false);
                    myRigidbody.velocity = Vector3.zero;
                    break;
            }
        }
    }

    void Update () {
        // Handling input and changing state accordingly
        if (Gameplay.InputTouch && Gameplay.Instance.CharacterCanMove()) {
            myRigidbody.velocity = WalkVector;
            CurrentState = State.Walk;
        } else {
            CurrentState = State.Idle;
        }

    }

}