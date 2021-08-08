using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public static class Utilities
{
    public static Vector2 SizeToParent(this RawImage image, float padding = 0)
    {
        var parent = image.transform.parent.GetComponent<RectTransform>();
        var imageTransform = image.GetComponent<RectTransform>();
        if (!parent) { return imageTransform.sizeDelta; } //if we don't have a parent, just return our current width;
        padding = 1 - padding;
        float w = 0, h = 0;
        float ratio = image.texture.width / (float)image.texture.height;
        var bounds = new Rect(0, 0, parent.rect.width, parent.rect.height);
        if (Mathf.RoundToInt(imageTransform.eulerAngles.z) % 180 == 90)
        {
            //Invert the bounds if the image is rotated
            bounds.size = new Vector2(bounds.height, bounds.width);
        }
        //Size by height first
        h = bounds.height * padding;
        w = h * ratio;
        if (w > bounds.width * padding)
        { //If it doesn't fit, fallback to width;
            w = bounds.width * padding;
            h = w / ratio;
        }
        imageTransform.sizeDelta = new Vector2(w, h);
        return imageTransform.sizeDelta;
    }

    public static double ConvertToKilometers(float miles)
    {
        double kilometers;
        kilometers = miles * 1.609;

        return kilometers;
    }

    public static double ConvertToMiles(float kilometers)
    {
        double miles;
        miles = kilometers / 1.609;

        return miles;
    }

    /*
    export const getMilesFromMeters = (meters) => {
    return Math.floor((meters * 0.00062137119224) * 100) / 100;
    }
    */

    public static double ConvertMetersToMiles(float meters)
    {
        double miles;
        miles = meters * 0.00062137119224;
        float rounded = Mathf.Floor((float)(miles * 100.0f)) / 100.0f;

        return rounded;
    }

    public static double ConvertMetersToKilometers(float meters)
    {
        double kilometers;
        kilometers = meters / 1000;
        float rounded = Mathf.Round((float)(kilometers * 100.0f)) / 100.0f;

        return rounded;
    }

    public static string FormatTime(float time)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        string timeText = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

        // Remove extra zeroes
        if (timeText.Substring(0, 2) == "00")
        {
            timeText = timeText.Substring(3);
        }

        if (timeText.Substring(0, 1) == "0")
        {
            timeText = timeText.Substring(1);
        }

        return timeText;
    }

    public static string CalculatePace(float time, float distance)
    {
        //float timeInSeconds = float.Parse(ConvertSeconds(time));
        float CalculatedPace = time / distance;


        string pace = Utilities.FormatTime(CalculatedPace);
        return pace;

        /*
        TotalSeconds = (hours * 3600) + (minutes * 60) + seconds;

        if (distance != 0)
        {
            float CalculatedPace = TotalSeconds / distance;

            pace = Utilities.FormatTime(CalculatedPace);
            PaceText.text = pace;
        }
        */
}

public static string ConvertSeconds(float time)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        string timeText = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

        return timeText;
    }

    public static string ConvertMetersToFeet(float time)
    {
        float CalculatedElevGain = (float)(time * 3.28084);
        return Math.Round(CalculatedElevGain).ToString();
    }

    public static string ConvertDate(String date)
    {
        DateTime d2 = DateTime.Parse(date, null, System.Globalization.DateTimeStyles.RoundtripKind);

        string dayOfWeek = d2.ToString("ddd", CultureInfo.InvariantCulture);
        string month = d2.ToString("MMM", CultureInfo.InvariantCulture);
        string day = d2.Day.ToString();
        string year = d2.Year.ToString();

        string formattedDate = dayOfWeek + "\n" + month + " " + day;

        //return d2.ToString("MM/dd/yyyy");
        return formattedDate;
    }
}
