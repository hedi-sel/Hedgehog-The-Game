using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gameplay : MonoBehaviour {
    // This class handles the overall functionning of the game

    static Gameplay instance;
    public static Gameplay Instance { get { return instance; } }
    private void Awake () {
        instance = this;
    }

    enum State {
        Play, //In this state, the player can move his character
        Finished, // This state is triggered when the current game is fnished (the character cannot move)
        ReadyForNext // This state comes after Finished, when the player can click to start a new game
    }
    State currentState = State.Play; //Default state

    //Code that checks if the player is clicking somewhere
    // The button "Touch" is used for debugging purposes and won't affect the final product
    public static bool InputTouch { get { return Input.touchCount > 0 || Input.GetButton("Touch"); } }

    private void Update () {
        if (InputTouch) { // Start a new game once we finish this one
            if (currentState == State.ReadyForNext)
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        } else if (currentState == State.Finished) // We avoid starting a new game immediatly, by forcing the player to stop touching first
            currentState = State.ReadyForNext;
    }

    //////
    // Methods triggered by external objects :
    //////
    public void CrossFinishLine () {
        currentState = State.Finished;
        FinishMask.Instance.WinMessage();
    }

    public void HitObstacle () {
        currentState = State.Finished;
        FinishMask.Instance.LoseMessage();
    }

}
