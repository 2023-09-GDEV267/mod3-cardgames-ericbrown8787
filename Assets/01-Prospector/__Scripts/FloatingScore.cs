using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;

public enum eFSState
{
    idle, 
    pre,
    active,
    post
}

// FloatingScore can move itself on the screen following a bezier curve

public class FloatingScore : MonoBehaviour
{
    [Header("Set Dynamically")]
    public eFSState state = eFSState.idle;

    [SerializeField]
    protected int _score = 0;
    public string scoreString;

    //the score property sets both _score and scoreString
    public int score
    {
        get { return _score; }
        set { _score = value;
            scoreString = _score.ToString("N0"); // N0 adds commas to the num
            GetComponent<TextMeshProUGUI>().text = scoreString;
        }
    }

    public List<Vector2> bezierPts;
    public List<float> fontSizes;
    public float timeStart = -1f;
    public float timeDuration = 1f;
    public string easingCurve = Easing.InOut;

    // The GameObject will receive the SendMessage when this is done moving
    public GameObject reportFinishTo = null;
    private RectTransform rectTrans;
    private TextMeshProUGUI txt;

    // Set up FloatingScore and movement
    // Note the use of parameter defaults for eTimeS and eTimeD
    public void Init(List<Vector2> ePts, float eTimeS = 0, float eTimeD = 1)
    {
        rectTrans = GetComponent<RectTransform>();
        rectTrans.anchoredPosition = Vector2.zero;
        txt = GetComponent<TextMeshProUGUI>();
        bezierPts = new List<Vector2>(ePts);

        if (ePts.Count == 1)
        {
            // If there's only one point
            //Just go there
            transform.position = ePts[0];
            return;
        }

        // If eTimeS is the default, just start at the current time
        if (eTimeS == 0) eTimeS = Time.time;
        timeStart = eTimeS;
        timeDuration = eTimeD;

        state = eFSState.pre;



    }

    public void FSCallback(FloatingScore fs)
    {
        // When this callback is called by SendMessage,
        // add the score from calling FloatingScore

        score += fs.score;
    }

    // Update is called once per frame
    void Update()
    {
        // If this is not moving, just return
        if (state==eFSState.idle)
        {
            return;
        }
        // Get u from the current time and duration
        // u ranges from 0 to 1(usually)
        float u = (Time.time - timeStart) / timeDuration;
        // Use easing class from utils to curve the u value
        float uC = Easing.Ease(u, easingCurve);

        if (u < 0)
        {
            // If u < 0 then we shouldn't move yet
            state = eFSState.pre;
            txt.enabled = false; // Hide the score initially
        }
        else
        {
            if (u >= 1)
            {
                // If u >= 1 we're done moving
                uC = 1; // Set uC to 1 so we don't overshoot
                state = eFSState.post;
                if (reportFinishTo != null)
                {
                    // If there's a callback gameobject
                    //Use SendMessage to call the FSCallback method
                    //with this as the parameter
                    reportFinishTo.SendMessage("FSCallback", this);
                    // Now that the message has been send, destroy this gameobject
                    Destroy(gameObject);
                }
                else
                {
                    // If there is nothing to callback
                    //then don't destroy this, just let it stay still
                    state = eFSState.idle;

                }


            }
            else
            {
                //                    0<=u<1, which means that this is is active and moving
                state = eFSState.active;
                txt.enabled = true; // Show the score once more
            }

            // Use bezier curve to move this to the right point
            Vector2 pos = Utils.Bezier(uC, bezierPts);

            // RectTransform anchors can be used to position UI objects relative to total size of screen
            rectTrans.anchorMin = rectTrans.anchorMax = pos;
            if (fontSizes != null && fontSizes.Count> 0)
            {
                // If fontsizes has values in it
                // adjust the fontSize of this GUIText

                int size = Mathf.RoundToInt(Utils.Bezier(uC, fontSizes));
                GetComponent<TextMeshProUGUI>().fontSize = size;
            }
        }
    }
}
