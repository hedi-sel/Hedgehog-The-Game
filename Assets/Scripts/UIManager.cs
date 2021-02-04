using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    //This class handles the appearance of the mask that appears at the end of each game

    static UIManager instance;
    public static UIManager Instance { get { return instance; } }

    // Components oof the Object that we will use
    [SerializeField] Image myMask;
    [SerializeField] Text myText;
    [SerializeField] Text myLeaderboard;
    [SerializeField] Text myTimer;
    [SerializeField] InputField inputName;
    [SerializeField] Button loginButton;
    [SerializeField] Button playButton;

    [SerializeField] GameObject LoginScreen;
    [SerializeField] GameObject LeaderboardScreen;
    [SerializeField] GameObject InGameUI;

    public const string lbTitle = "Leaderboard:\n";
    public const string lbWait = lbTitle + "Just a moment...\nLoading ...";
    public const string lbFail = lbTitle + "Impossible to connect to the database";

    private void Awake () {
        instance = this;
        HideAll();
        myLeaderboard.text = lbWait;
        playButton.onClick.AddListener(() => { Gameplay.Instance.StartGame(); });
        loginButton.onClick.AddListener(() => { Gameplay.Instance.PressLogin(); });
    }

    public void HideAll () {
        myMask.gameObject.SetActive(false);
        myText.gameObject.SetActive(false);
        LoginScreen.SetActive(false);
        LeaderboardScreen.SetActive(false);
        InGameUI.SetActive(false);
    }

    public string GetPlayerName () {
        return inputName.text;
    }

    public void ShowLoginScreen () {
        UIManager.Instance.HideAll();
        myMask.gameObject.SetActive(true);
        LoginScreen.SetActive(true);
    }

    public void ShowLeaderboard () {
        UIManager.Instance.HideAll();
        myMask.gameObject.SetActive(true);
        LeaderboardScreen.SetActive(true);
    }

    public void UpdateLeaderboard () {
        myLeaderboard.text = lbWait;
        Gameplay.NetworkInstance.UpdateLeaderboard(myLeaderboard);
    }

    public void ShowInGameUI () {
        UIManager.Instance.HideAll();
        InGameUI.SetActive(true);
    }

    public void ShowText (string text) {
        HideAll();
        myMask.gameObject.SetActive(true);
        myText.text = text;
        myText.gameObject.SetActive(true);
    }

    public void UpdateTimer (float newTime) {
        myTimer.text = "" + newTime;
    }

}