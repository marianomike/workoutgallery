using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;
using System.Linq;
using SimpleJSON;

public class AppManager : MonoBehaviour
{
    public string athleteId;
    public string token;

    // resulting JSON from an API request
    public JSONNode jsonResult;

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

    void Start()
    {
        Debug.Log("start");
        StartCoroutine(Post());
    }

    /*
    // sends an API request - returns a JSON file
    private IEnumerator GetData()
    {
        // create the web request and download handler
        UnityWebRequest webReq = new UnityWebRequest();
        webReq.downloadHandler = new DownloadHandlerBuffer();

        // build the url and query
        webReq.url = url;
        Debug.Log(url);

        // send the web request and wait for a returning result
        yield return webReq.SendWebRequest();

        // convert the byte array to a string
        string rawJson = Encoding.Default.GetString(webReq.downloadHandler.data);

        // parse the raw string into a json result we can easily read
        jsonResult = JSON.Parse(rawJson);

        Debug.Log(jsonResult);

        // display the results on screen
        //UI.instance.SetSegments(jsonResult["result"]["records"]);
    }

    void Start()
    {
        url = "https://www.strava.com/api/v3/athlete/activities?access_token=24e18cd680c3e97953c38eba3a40c746d78c41d8";

        StartCoroutine(GetData);
    }
    */
}