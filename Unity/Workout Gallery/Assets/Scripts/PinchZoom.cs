﻿using UnityEngine;

public class PinchZoom : MonoBehaviour
{
    public Canvas canvas; // The canvas
    public RectTransform rt;
    public float zoomSpeed = 0.01f;        // The rate of change of the canvas scale factor

    void Update()
    {
        // If there are two touches on the device...
        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // ... change the canvas size based on the change in distance between the touches.
            float currentZoom = deltaMagnitudeDiff * zoomSpeed;
            rt.localScale -= new Vector3(currentZoom, currentZoom, 0);
            //canvas.scaleFactor -= deltaMagnitudeDiff * zoomSpeed;

            // Make sure the canvas size never drops below 0.1
            //canvas.scaleFactor = Mathf.Max(canvas.scaleFactor, 0.1f);
            //rt.localScale = Mathf.Max(rt.localScale.x, 0.1f);
            currentZoom = Mathf.Clamp(currentZoom, 0.1f,2f);
        }
    }
}