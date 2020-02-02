using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TwistRotate : MonoBehaviour
{
    private bool rotating;
    private Vector2 startVector;
    private readonly float rotGestureWidth;
    private readonly float rotAngleMinimum;

    //public Text sampleText;

    void Update()
    {
        if (Input.touchCount == 2)
        {
            if (!rotating)
            {
                startVector = Input.GetTouch(1).position - Input.GetTouch(0).position;
                rotating = startVector.sqrMagnitude > rotGestureWidth * rotGestureWidth;
            }
            else
            {
                var currVector = Input.GetTouch(1).position - Input.GetTouch(0).position;
                var angleOffset = Vector2.Angle(startVector, currVector);
                var LR = Vector3.Cross(startVector, currVector);

                if (angleOffset > rotAngleMinimum)
                {
                    if (LR.z > 0)
                    {
                        //sampleText.text = "rotate left";
                        startVector = currVector;
                    }
                    else if (LR.z < 0)
                    {
                        //sampleText.text = "rotate right";
                        startVector = currVector;
                    }
                }

            }

        }
        else
        {
            rotating = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
}
