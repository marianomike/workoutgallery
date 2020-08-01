using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;
using System.Linq;
using SimpleJSON;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class GetData : MonoBehaviour
{
    public JSONNode jsonResult;
    private ArrayList RunList;
    private string measurement;

    public GameObject StatLayoutGroup;

    private string runName;
    private string runDate;
    private string runDistance;
    private string runTime;
    private string runPace;
    private string runElevation;

    private readonly string access_token = "6bb385aa7ff8379c3b5fc9410fce86574e6231bb";

    //private string url = "https://www.strava.com/api/v3/activities/{id}?include_all_efforts";
    /*
    // resulting JSON from an API request
    

    private IEnumerator Post()
    {
        Debug.Log("post called");

        UnityWebRequest request = new UnityWebRequest();
        request.url = "https://www.strava.com/api/v3/athlete/activities?access_token=24e18cd680c3e97953c38eba3a40c746d78c41d8";
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        string rawJson = Encoding.Default.GetString(request.downloadHandler.data);
        jsonResult = JSON.Parse(rawJson);

        Debug.Log(jsonResult);
    }
    */

    /*
    string Authenticate(string athleteId, string token)
    {
        string auth = athleteId + ":" + token;
        auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
        auth = "Bearer " + auth;
        return auth;
    }
    */

    IEnumerator MakeRequest()
    {
        string authorization = "Bearer " + access_token;
        string url = "www.strava.com/api/v3/athlete/";

        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("Authorization", authorization);

        yield return www.SendWebRequest();

        string rawJson = Encoding.Default.GetString(www.downloadHandler.data);
        jsonResult = JSON.Parse(rawJson);

        Debug.Log(jsonResult["date_preference"]);

        // get measurement preference meters or feet
        measurement = jsonResult["measurement_preference"];
    }

    IEnumerator GetAccessToken()
    {
        string url = "https://www.strava.com/oauth/authorize?";

        WWWForm form = new WWWForm();
        form.AddField("client_id", "47258");
        form.AddField("client_secret", "1b444f5711eff4e25dbe5ad273f4177290f7c6b8");
        form.AddField("code", "46b5afa042fd34034c5df4358bb62fc6bd8efe5d");
        form.AddField("grant_type", "authorization_code");

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        string rawJson = Encoding.Default.GetString(www.downloadHandler.data);
        jsonResult = JSON.Parse(rawJson);

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(jsonResult);
        }
    }

    IEnumerator GetActivities()
    {
        string url = "https://www.strava.com/api/v3/athlete/activities?access_token=" + access_token;

        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        string rawJson = Encoding.Default.GetString(www.downloadHandler.data);
        jsonResult = JSON.Parse(rawJson);

        if(www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(jsonResult);
        }

        int count = jsonResult.Children.Count();

        RunList = new ArrayList();

        for (int i = 0; i < count; i++)
        {
            // store string data in list
            if(jsonResult[i]["type"] == "Run" || jsonResult[i]["type"] == "Walk")
            {
                RunStat newRunstat = new RunStat
                {
                    date = Utilities.ConvertDate(jsonResult[i]["start_date_local"]),
                    title = jsonResult[i]["name"]
                };

                // check what measurement preference user has, then calculate based on that
                float rawDistance;
                if (measurement == "feet")
                {
                    rawDistance = (float)Utilities.ConvertMetersToMiles(jsonResult[i]["distance"]);
                    newRunstat.distance = rawDistance.ToString();
                    newRunstat.pace = Utilities.CalculatePace(jsonResult[i]["moving_time"], rawDistance) + "/m";
                    newRunstat.elevgain = Utilities.ConvertMetersToFeet(jsonResult[i]["total_elevation_gain"]) + " ft";
                }
                else
                {
                    rawDistance = (float)Utilities.ConvertMetersToKilometers(jsonResult[i]["distance"]);
                    newRunstat.distance = rawDistance.ToString();
                    newRunstat.pace = Utilities.CalculatePace(jsonResult[i]["moving_time"], rawDistance) + "/km";
                    newRunstat.elevgain= jsonResult[i]["total_elevation_gain"] + " m";
                }

                newRunstat.time = Utilities.FormatTime(jsonResult[i]["moving_time"]);
                RunList.Add(newRunstat);
            }
        }

        // Now populate the UI
        PopulateRuns(RunList);
    }

    private void PopulateRuns(ArrayList RunList)
    {
        for (int i = 0; i < RunList.Count; i++)
        {
            RunStat tempStat = (RunStat)RunList[i];
            GameObject newStat = Instantiate(Resources.Load("Prefabs/template_stat_row")) as GameObject;
            newStat.GetComponent<StatTemplate>().date.text = tempStat.date;
            newStat.GetComponent<StatTemplate>().title.text = tempStat.title;
            newStat.GetComponent<StatTemplate>().distance.text = tempStat.distance;
            newStat.GetComponent<StatTemplate>().time.text = tempStat.time;
            newStat.GetComponent<StatTemplate>().pace.text = tempStat.pace;
            newStat.GetComponent<StatTemplate>().elevgain.text = tempStat.elevgain;

            newStat.transform.SetParent(StatLayoutGroup.transform, true);
            newStat.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            newStat.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);

            newStat.GetComponent<StatTemplate>().button.onClick.AddListener(delegate { ClickRow(tempStat); });
        }
    }

    private void ClickRow(RunStat stat)
    {
        // store the clicked run stats
        runDate = stat.date;
        runName = stat.title;
        runDistance = stat.distance;
        runTime = stat.time;
        runPace = stat.pace;
        runElevation = stat.elevgain;
        Debug.Log(runName + " on " + runDate + ": " + runDistance + "/" + runTime + "/" + runPace + "/" + runElevation);
    }

    private void SetTempData()
    {
        runDate = "July 24";
        runName = "Morning Run";
        runDistance = "20.04";
        runTime = "2:54:30";
        runPace = "8:45/m";
        runElevation = "350ft";
    }

    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(MakeRequest());
        //StartCoroutine(GetAccessToken());
        StartCoroutine(GetActivities());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
