using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;
using System.Linq;

public class TwitchOAuth : MonoBehaviour
{
    [SerializeField] private string twitchAuthUrl = "http://www.strava.com/oauth/authorize";
    [SerializeField] private string twitchClientId = "PUT YOUR CLIENT ID HERE";
    [SerializeField] private string twitchClientSecret = "PUT YOUR CLIENT SECRET HERE";
    [SerializeField] private string twitchRedirectUrl = "http://localhost:8080/";
    [SerializeField] private TwitchApiCallHelper twitchApiCallHelper = null;
    [SerializeField] private TextMeshProUGUI uiTextToken = null;
    [SerializeField] private Button RunButton = null;
    [SerializeField] private Button LoginButton = null;
    [SerializeField] private Image Loader = null;

    private string _twitchAuthStateVerify;
    private string _authToken;
    private JSONNode jsonResult;
    private ArrayList RunList;
    private string measurement = "feet"; //fix me later

    public GameObject StatLayoutGroup;
    public GameObject RunListObject;

    private string runName;
    private string runDate;
    private string runDistance;
    private string runTime;
    private string runPace;
    private string runElevation;

    private void Start()
    {
        _authToken = "";
        RunButton.gameObject.SetActive(false);
        LoginButton.gameObject.SetActive(true);
        Loader.gameObject.SetActive(false);
        RunListObject.SetActive(false);
        RunButton.onClick.AddListener(delegate { StartCoroutine(GetActivities()); });
        UpdateTokenDisplay();
        StartCoroutine(DisplayUpdater());
    }

    private void Update()
    {
        if(_authToken != "")
        {
            RunButton.gameObject.SetActive(true);
            LoginButton.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Starts the Twitch OAuth flow by constructing the Twitch auth URL based on the scopes you want/need.
    /// </summary>
    public void InitiateTwitchAuth()
    {
        string[] scopes;
        string s;

        // list of scopes we want
        scopes = new[]
        {
            "read_all",
            "profile:read_all",
            "activity:read_all"
        };

        // query parameters for the Twitch auth URL
        s = "client_id=" + twitchClientId + "&" +
            "response_type=code&" +
            "redirect_uri=" + twitchRedirectUrl + "&" +
            "approval_prompt=force&" +
            "scope=" + String.Join(",", scopes);

        // start our local webserver to receive the redirect back after Twitch authenticated
        StartLocalWebserver();

        // open the users browser and send them to the Twitch auth URL
        Application.OpenURL(twitchAuthUrl + "?" + s);
    }

    /// <summary>
    /// Opens a simple "webserver" like thing on localhost:8080 for the auth redirect to land on.
    /// Based on the C# HttpListener docs: https://docs.microsoft.com/en-us/dotnet/api/system.net.httplistener
    /// </summary>
    private void StartLocalWebserver()
    {
        HttpListener httpListener = new HttpListener();

        httpListener.Prefixes.Add(twitchRedirectUrl);
        httpListener.Start();
        httpListener.BeginGetContext(new AsyncCallback(IncomingHttpRequest), httpListener);
    }

    /// <summary>
    /// Handles the incoming HTTP request
    /// </summary>
    /// <param name="result"></param>
    private void IncomingHttpRequest(IAsyncResult result)
    {
        string code;
        string state;
        HttpListener httpListener;
        HttpListenerContext httpContext;
        HttpListenerRequest httpRequest;
        HttpListenerResponse httpResponse;
        string responseString;

        // get back the reference to our http listener
        httpListener = (HttpListener) result.AsyncState;

        // fetch the context object
        httpContext = httpListener.EndGetContext(result);

        // if we'd like the HTTP listener to accept more incoming requests, we'd just restart the "get context" here:
        // httpListener.BeginGetContext(new AsyncCallback(IncomingHttpRequest),httpListener);
        // however, since we only want/expect the one, single auth redirect, we don't need/want this, now.
        // but this is what you would do if you'd want to implement more (simple) "webserver" functionality
        // in your project.

        // the context object has the request object for us, that holds details about the incoming request
        httpRequest = httpContext.Request;

        code = httpRequest.QueryString.Get("code");
        //Debug.Log(code);

        // check that we got a code value and the state value matches our remembered one
        if (code.Length > 0)
        {
            // if all checks out, use the code to exchange it for the actual auth token at the API
            GetTokenFromCode(code);
        }

        // build a response to send an "ok" back to the browser for the user to see
        httpResponse = httpContext.Response;
        responseString = "<html><body><b>DONE!</b><br>(You can close this tab/window now)</body></html>";
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

        // send the output to the client browser
        httpResponse.ContentLength64 = buffer.Length;
        System.IO.Stream output = httpResponse.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();

        // the HTTP listener has served it's purpose, shut it down
        httpListener.Stop();
        // obv. if we had restarted the waiting for more incoming request, above, we'd not Stop() it here.
    }

    /// <summary>
    /// Makes the API call to exchange the received code for the actual auth token
    /// </summary>
    /// <param name="code">The code parameter received in the callback HTTP reuqest</param>
    private void GetTokenFromCode(string code)
    {
        string apiUrl;
        string apiResponseJson;
        ApiCodeTokenResponse apiResponseData;

        // construct full URL for API call
        apiUrl = "https://www.strava.com/oauth/token" +
                 "?client_id=" + twitchClientId +
                 "&client_secret=" + twitchClientSecret +
                 "&code=" + code +
                 "&grant_type=authorization_code";

        // make sure our API helper knows our client ID (it needed for the HTTP headers)
        twitchApiCallHelper.TwitchClientId = twitchClientId;

        // make the call!
        apiResponseJson = twitchApiCallHelper.CallApi(apiUrl, "POST");

        // parse the return JSON into a more usable data object
        apiResponseData = JsonUtility.FromJson<ApiCodeTokenResponse>(apiResponseJson);

        // fetch the token from the response data
        _authToken = apiResponseData.access_token;
    }

    IEnumerator GetActivities()
    {
        Loader.gameObject.SetActive(true);

        string url = "https://www.strava.com/api/v3/athlete/activities?access_token=" + _authToken;

        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        string rawJson = Encoding.Default.GetString(www.downloadHandler.data);
        jsonResult = JSON.Parse(rawJson);

        //CS0618: 'UnityWebRequest.isNetworkError' is obsolete: 'UnityWebRequest.isNetworkError is deprecated. Use (UnityWebRequest.result == UnityWebRequest.Result.ConnectionError) instead.'
        //CS0618: 'UnityWebRequest.isHttpError' is obsolete: 'UnityWebRequest.isHttpError is deprecated. Use (UnityWebRequest.result == UnityWebRequest.Result.ProtocolError) instead.'
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
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
            if (jsonResult[i]["type"] == "Run" || jsonResult[i]["type"] == "Walk")
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
                    newRunstat.elevgain = jsonResult[i]["total_elevation_gain"] + " m";
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
            GameObject newStat = Instantiate(Resources.Load("Prefabs/Templates/template_stat_row")) as GameObject;
            newStat.GetComponent<StatTemplate>().date.text = tempStat.date;
            newStat.GetComponent<StatTemplate>().title.text = tempStat.title;
            newStat.GetComponent<StatTemplate>().distance.text = tempStat.distance;
            newStat.GetComponent<StatTemplate>().time.text = tempStat.time;
            newStat.GetComponent<StatTemplate>().pace.text = tempStat.pace;
            newStat.GetComponent<StatTemplate>().elevgain.text = tempStat.elevgain;

            newStat.transform.SetParent(StatLayoutGroup.transform, true);
            newStat.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            newStat.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);

            //newStat.GetComponent<StatTemplate>().button.onClick.AddListener(delegate { ClickRow(tempStat); });
        }
        RunListObject.SetActive(true);
        Loader.gameObject.SetActive(false);
    }

    /// <summary>
    /// Updates the UI in the scene with the current auth data
    /// </summary>
    private void UpdateTokenDisplay()
    {
        uiTextToken.text = _authToken;
    }

    /// <summary>
    /// This is a small necessary evil, as we can't update the UI directly from the HTTP handler.
    /// It's a Unity thing and has to do with the fact that internally the HTTP handler runs on a separate thread.
    /// Nothing you need to worry about. Just Unity stuff, lol.
    /// </summary>
    /// <returns></returns>
    IEnumerator DisplayUpdater()
    {
        while(true)
        {
            yield return new WaitForSeconds(1);
            UpdateTokenDisplay();
        }
    }
}
