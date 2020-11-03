using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishMask : MonoBehaviour {
    //This class handles the appearance of the mask that appears at the end of each game

    static FinishMask instance;
    public static FinishMask Instance { get { return instance; } }

    // Components oof the Object that we will use
    Image myMask;
    Text myText;
    private void Awake () {
        instance = this;
        myMask = GetComponentInChildren<Image>();
        myText = GetComponentInChildren<Text>();
    }

    public void WinMessage () {
        EnableFinishMask();
        myText.text = "You Win!";
    }
    public void LoseMessage () {
        EnableFinishMask();
        myText.text = "You lost ...";
    }

    void EnableFinishMask () {
        myMask.enabled = true;
        myText.enabled = true;
    }

}
