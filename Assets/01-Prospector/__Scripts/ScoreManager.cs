using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handle all possible scoring events
public enum eScoreEvent
{
     draw,
     mine,
     mineGold,
     gameWin,
     gameLoss
}

// ScoreManager handles all of the scoring
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager S;
    static public int SCORE_FROM_PREV_ROUND = 0;
    static public int HIGH_SCORE = 0;

    [Header("Set Dyunamically")]
    // Fields to track the score info
    public int chain = 0;
    public int scoreRun = 0;
    public int score = 0;

    private void Awake()
    {
        if (S == null)
        {
            S = this;
        } else
        {
            Debug.Log("ERROR: ScoreManager.Awake(): S is already set");
        }

        // Check for a high score in playerprefs
        if (PlayerPrefs.HasKey("ProspectorHighScore"))
        {
            HIGH_SCORE = PlayerPrefs.GetInt("ProspectorHighScore");
            Debug.Log("HIGH_SCORE in ScoreManager: " +HIGH_SCORE);
        }

        // Add the score from the last roung, which will be >0 if it was a win
        score += SCORE_FROM_PREV_ROUND;
        // And reset the score from previous round
        SCORE_FROM_PREV_ROUND = 0;
    }


    static public void EVENT(eScoreEvent evt)
    {
        try
        {
            // try-catch stops an error from breaking your program
            S.Event(evt);
        } catch(System.NullReferenceException nre)
        {
            Debug.LogError($"ScoreManager:EVENT() called while S=null.\n{nre}");
        }
    }

    void Event(eScoreEvent evt)
    {
        switch (evt)
        {
            // Same things need to happen whether it's a draw, a win or a loss
            case eScoreEvent.draw: //drawing a card
            case eScoreEvent.gameWin: // Won the round
            case eScoreEvent.gameLoss: // Lost the round
                chain = 0;//Remove the mine card
                score += scoreRun; //increase the score chain
                scoreRun = 0; //add score for this card to run
                break;

            case eScoreEvent.mine: // Remove a mine card
                chain++; // Increase the score chain
                scoreRun += chain; // Add score for this card to run
                break;
        }

        //This second switch statement handles round wins and losses
        switch (evt)
        {
            case eScoreEvent.gameWin:
                // If it's a win, add the score to the next round
                // Static fields are not reset by SceneManager.LoadScene()
                SCORE_FROM_PREV_ROUND = score;
                print($"You won this round! Round Score: {score}");
                break;

            case eScoreEvent.gameLoss:
                // If it's a loss, check against the high score
                if (HIGH_SCORE <= score)
                {
                    print($"You got the high score! High Score: {score}");
                    PlayerPrefs.SetInt("ProspectorHighScore", score);
                }
                else
                {
                    print($"Your final score from the game was: {score}");
                }
                break;

            default:
                print($"score: {score}  scoreRun: {scoreRun}  chain: {chain}");
                break;
        }
    }
    static public int CHAIN { get { return S.chain; } }
    static public int SCORE { get { return S.score; } }

    static public int SCORE_RUN { get { return S.scoreRun; } }
}
