using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Gameplay : MonoBehaviour {
    // This class handles the overall functionning of the game

    static Gameplay instance;
    public static Gameplay Instance { get { return instance; } }
    private void Awake () {
        instance = this;
        networkInstance = GetComponent<NetworkManager> ();
    }

    NetworkManager networkInstance;
    public static NetworkManager NetworkInstance { get { return instance.networkInstance; } }

    string playerName;
    public static string PlayerName { get { return instance.playerName; } }

    public UnityEvent Restart = new UnityEvent ();

    enum State {
        ShowLeaderboard, //In this state, the player can check the leaderboard before starting
        Play, //In this state, the player can move his character
        Finished, // This state is triggered when the current game is fnished (the character cannot move)
        ReadyForNext // This state comes after Finished, when the player can click to start a new game
    }
    State currentState = State.ShowLeaderboard; //Default state

    float time = 0.0f;

    //Code that checks if the player is clicking somewhere
    // The button "Touch" is used for debugging purposes and won't affect the final product
    public static bool InputTouch { get { return Input.touchCount > 0 || Input.GetButton ("Touch"); } }

    private void Start () {
        UIManager.Instance.ShowLoginScreen ();
    }

    private void Update () {
        if (InputTouch) { // Start a new game once we finish this one
            if (currentState == State.ReadyForNext)
                RestartGame ();
        } else if (currentState == State.Finished)
            currentState = State.ReadyForNext;

        if (currentState == State.Play) {
            time += Time.deltaTime;
            UIManager.Instance.UpdateTimer (time);
        }
    }

    public void PressLogin () {
        playerName = UIManager.Instance.GetPlayerName ();
        networkInstance.Login (() => {
            UIManager.Instance.ShowLeaderboard ();
            if (currentState == State.ShowLeaderboard)
                UIManager.Instance.UpdateLeaderboard ();
        });
    }

    public void StartGame () {
        UIManager.Instance.ShowInGameUI ();
        currentState = State.Play;
    }

    public void CrossFinishLine () {
        if (currentState != State.Play) return;
        currentState = State.Finished;
        if (playerName != "") {
            UIManager.Instance.ShowText ("You Win!\nPosting score, please wait ...");
            NetworkInstance.PostScore (playerName, time, () => {
                UIManager.Instance.ShowText ("You Win!\nYour score is online!");
            });
        } else {
            UIManager.Instance.ShowText ("You Win!\nYou need to write a name to save your score online");
        }
    }

    public void HitObstacle () {
        if (currentState != State.Play) return;
        currentState = State.Finished;
        UIManager.Instance.ShowText ("You lose ...");
    }

    public bool CharacterCanMove () {
        return currentState == State.Play;
    }

    public void RestartGame () {
        UIManager.Instance.ShowText ("Reconnecting to server ...");
        Restart.Invoke ();
        networkInstance.Login (() => {
            UIManager.Instance.ShowLeaderboard ();
            UIManager.Instance.UpdateLeaderboard ();
        });
        currentState = State.ShowLeaderboard;
        time = 0.0f;

    }
}