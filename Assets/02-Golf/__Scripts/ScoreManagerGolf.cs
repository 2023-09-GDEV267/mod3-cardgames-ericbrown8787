using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handle all possible scoring events
public enum eGolfScoreEvent
{
     draw,
     mine,
     mineGold,
     gameWin,
     gameLoss
}

// ScoreManager handles all of the scoring
public class ScoreManagerGolf : MonoBehaviour
{
    public static ScoreManagerGolf S;
    static public int SCORE_FROM_PREV_ROUND = 0;
    static public int HIGH_SCORE = 0;

    [Header("Set Dynamically")]
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
            Debug.Log("ERROR: ScoreManagerGolf.Awake(): S is already set");
        }

        // Check for a high score in playerprefs
        if (PlayerPrefs.HasKey("GolfHighScore"))
        {
            HIGH_SCORE = PlayerPrefs.GetInt("GolfHighScore");
            Debug.Log("HIGH_SCORE in ScoreManager: " +HIGH_SCORE);
        }

        // Add the score from the last roung, which will be >0 if it was a win
        score += SCORE_FROM_PREV_ROUND;
        // And reset the score from previous round
        SCORE_FROM_PREV_ROUND = 0;
    }


    static public void EVENT(eGolfScoreEvent evt)
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

    void Event(eGolfScoreEvent evt)
    {
        switch (evt)
        {
            // Same things need to happen whether it's a draw, a win or a loss
            case eGolfScoreEvent.draw: //drawing a card
            case eGolfScoreEvent.gameWin: // Won the round
            case eGolfScoreEvent.gameLoss: // Lost the round
                chain = 0;//Remove the mine card
                score += scoreRun; //increase the score chain
                scoreRun = 0; //add score for this card to run
                break;

            case eGolfScoreEvent.mine: // Remove a mine card
                chain++; // Increase the score chain
                scoreRun += chain; // Add score for this card to run
                break;
        }

        //This second switch statement handles round wins and losses
        switch (evt)
        {
            case eGolfScoreEvent.gameWin:
                // If it's a win, add the score to the next round
                // Static fields are not reset by SceneManager.LoadScene()
                SCORE_FROM_PREV_ROUND = score;
                print($"You won this round! Round Score: {score}");
                break;

            case eGolfScoreEvent.gameLoss:
                // If it's a loss, check against the high score
                if (HIGH_SCORE <= score)
                {
                    print($"You got the high score! High Score: {score}");
                    PlayerPrefs.SetInt("GolfHighScore", score);
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
