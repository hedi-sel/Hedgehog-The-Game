using System;
using System.Collections;
using CotcSdk;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_5_4_OR_NEWER
using UnityEngine.Networking;
#else
using UnityEngine.Experimental.Networking;
#endif

public class NetworkManager : MonoBehaviour {

    // The cloud allows to make generic operations (non user related)
    private Cloud Cloud;
    // The gamer is the base to perform most operations. A gamer object is obtained after successfully signing in.
    private Gamer Gamer;
    // When a gamer is logged in, the loop is launched for domain private. Only one is run at once.
    private DomainEventLoop Loop;

    void Awake () {
        var cb = FindObjectOfType<CotcGameObject>();
        if (cb == null) {
            Debug.LogError("Please put a Clan of the Cloud prefab in your scene!");
            return;
        }
        // Log unhandled exceptions (.Done block without .Catch -- not called if there is any .Then)
        Promise.UnhandledException += (object sender, ExceptionEventArgs e) => {
            Debug.LogError("Unhandled exception: " + e.Exception.ToString());
        };
        // Initiate getting the main Cloud object
        cb.GetCloud().Done(cloud => {
            Cloud = cloud;
            // Retry failed HTTP requests once
            Cloud.HttpRequestFailedHandler = (HttpRequestFailedEventArgs e) => {
                if (e.UserData == null) {
                    e.UserData = new object();
                    e.RetryIn(1000);
                } else
                    e.Abort();
            };
            Debug.Log("Setup done");
        });
    }

    public void Login (Action afterLogin = null) {
        Cloud.LoginAnonymously().Then(gamer => {
            DidLogin(gamer);
            if (afterLogin != null) afterLogin();
        })
        .Catch(ex => {
            CotcException error = (CotcException) ex;
            Debug.LogError("Failed to login: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
        });
    }

    public void ResumeSession () {
        Cloud.ResumeSession(
                gamerId: "5873a117b69fa8c942c7df08",
                gamerSecret: "c3b7c6fab599919b0c24487bf46d0e6069472df0")
            .Done(gamer => DidLogin(gamer), ex => {
                CotcException error = (CotcException) ex;
                Debug.LogError("Failed to login: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
            });

    }

    // Invoked when any sign in operation has completed
    void DidLogin (Gamer newGamer) {
        if (Gamer != null) {
            Debug.LogWarning("Current gamer " + Gamer.GamerId + " has been dismissed");
            Loop.Stop();
        }
        Gamer = newGamer;
        Loop = Gamer.StartEventLoop();
        Loop.ReceivedEvent += Loop_ReceivedEvent;
        Debug.Log("Signed in successfully (ID = " + Gamer.GamerId + ")");
    }

    void Loop_ReceivedEvent (DomainEventLoop sender, EventLoopArgs e) {
        Debug.Log("Received event of type " + e.Message.Type + ": " + e.Message.ToJson());
    }

    bool RequireGamer () {
        if (Gamer == null)
            Debug.LogError("You need to login first");
        return Gamer != null;
    }

    //// Leader Board code
    long TimeToScore (float time) {
        return (long) (time * 1000);
    }
    float ScoreToTime (long score) {
        return score * 1.0f / 1000;
    }

    public void PostScore (string name, float time, Action afterPost = null) {
        if (!RequireGamer()) return;
        Gamer.Scores.Domain("private").Post(TimeToScore(time), "mode", ScoreOrder.LowToHigh,
                name, false)
            .Done(postScoreRes => {
                Debug.Log("Post score: " + postScoreRes.ToString());
                if (afterPost != null) afterPost();
            }, ex => {
                // The exception should always be CotcException
                CotcException error = (CotcException) ex;
                Debug.LogError("Could not post score: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
            });
    }

    public void GetRank (float time) {
        if (!RequireGamer()) return;

        Gamer.Scores.Domain("private").GetRank(TimeToScore(time), "mode")
            .Done(getRankRes => {
                Debug.Log("Rank for score: " + getRankRes);
            }, ex => {
                // The exception should always be CotcException
                CotcException error = (CotcException) ex;
                Debug.LogError("Could not get rank for score: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
            });
    }

    public void UpdateLeaderboard (Text Leaderboard) {
        if (!RequireGamer()) {
            Leaderboard.text = UIManager.lbFail;
            return;
        }
        Gamer.Scores.Domain("private").BestHighScores("mode", 10, 1)
            .Done(bestHighScoresRes => {
                string displayText = UIManager.lbTitle;
                foreach (var score in bestHighScoresRes)
                    displayText += score.Rank + ". " + score.Info + ": " + ScoreToTime(score.Value) + "\n";
                Leaderboard.text = displayText;
            }, ex => {
                // The exception should always be CotcException
                CotcException error = (CotcException) ex;
                Debug.LogError("Could not get best high scores: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
            });
    }
}