using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

[Serializable]
public class SessionManager : MonoBehaviour
{
[SerializeField] public Session currentSession;
[SerializeField] private TMP_InputField startScreenSessionCode;
[SerializeField] private Toggle hostOrJoinCheckbox;
[SerializeField] private TextMeshProUGUI sessionErrorText;

private readonly string url = "http://localhost:8080/api/session"; // aangepast naar juiste endpoint

// Create or join a session
public void Begin()
{
    if (hostOrJoinCheckbox.isOn)
    {
        CreateSession();
    }
    else
    {
        StartCoroutine(JoinSessionRequest());
    }
}

public void JoinSession(string sessionCode)
{
    StartCoroutine(JoinSessionRequest());
}

// Update a specific session
public void UpdateSession(Session session)
{
    StartCoroutine(PutRequest(session));
}

// Update the current session
public void UpdateCurrentSession()
{
    StartCoroutine(PutRequest(currentSession));
}

// Update specific party slider values from the given partyID
public void UpdateCurrentSessionPartyValues(int partyID)
{
    StartCoroutine(UpdatePartValueRequest(partyID, currentSession));
}

// Reload data from the current session
public void ResyncCurrentSession()
{
    Debug.Log("Resyncing " + currentSession.sessionCode);
    StartCoroutine(SyncCurrentSession());
}

// Create a new session
public void CreateSession()
{
    Session newSession = new(startScreenSessionCode.text)
    {
        measures = new List<Measure>() {
            new("Voorbeeld 1", 1000, 100, 10),
            new("Voorbeeld 2", 1000, 100, 10),
            new("Voorbeeld 3", 1000, 100, 10),
            new("Voorbeeld 4", 1000, 100, 10),
        },
        parties = new List<Party>()
        {
            new("Partij 1", "Partij Beschrijving"),
            new("Partij 2", "Partij Beschrijving"),
            new("Partij 3", "Partij Beschrijving"),
            new("Partij 4", "Partij Beschrijving"),
        },
        budget = 3000000,
        peopleNeedingHelp = 10000
    };

    StartCoroutine(CreateSessionRequest(newSession));
}

// Change the session code
internal void SetNewSessionCode(string newSessionCode)
{
    currentSession.newSessionCode = newSessionCode;
}

public void SetDescription(string description)
{
    currentSession.description = description;
}

public void SetBudget(double budget)
{
    currentSession.budget = budget;
}

public void SetPeopleNeedingHelp(double peopleNeedingHelp)
{
    currentSession.peopleNeedingHelp = peopleNeedingHelp;
}

public void ClearMeasures()
{
    currentSession.measures.Clear();
}

public void SetMeasures(List<Measure> measures)
{
    currentSession.Measures = measures;
}

public void SetParties(List<Party> parties)
{
    currentSession.Parties = parties;
}

public void SyncMeasures()
{

}

// Reload the current session data
IEnumerator SyncCurrentSession()
{
    UnityWebRequest request = UnityWebRequest.Get(url + "?sessionCode=" + startScreenSessionCode.text);
    request.SetRequestHeader("Content-Type", "application/json");

    yield return request.SendWebRequest();

    if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
    {
        Debug.LogError("Error: " + request.error);
    }
    else
    {
        string jsonResponse = request.downloadHandler.text;
        Session session = JsonUtility.FromJson<Session>(jsonResponse);

        if (session != null)
        {
            currentSession = session;
            GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
            gameManager.LoadSessionData();
            gameManager.CallInsertToTable();
            gameManager.LoadPartyValuesInSlider();
        }
        else
        {
            Debug.LogError("Failed to parse JSON or no sessions found.");
        }
    }
}

// Find and join a session
IEnumerator JoinSessionRequest()
{
    UnityWebRequest request = UnityWebRequest.Get(url + "?sessionCode=" + startScreenSessionCode.text);
    request.SetRequestHeader("Content-Type", "application/json");

    yield return request.SendWebRequest();

    if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
    {
        switch (request.responseCode)
        {
            case 404:
                sessionErrorText.text = "Sessie niet gevonden";
                Debug.LogError("Conflict: " + request.error);
                break;
            case 422:
                sessionErrorText.text = "Vul A.U.B. een sessie code in";
                Debug.LogError("Conflict: " + request.error);
                break;
            default:
                sessionErrorText.text = "Server error probeer a.u.b. later";
                Debug.LogError("Error: " + request.error);
                break;
        }
    }
    else
    {
        string jsonResponse = request.downloadHandler.text;

        Session session = JsonUtility.FromJson<Session>(jsonResponse);

        if (session != null)
        {
            sessionErrorText.text = "";
            currentSession = session;
            GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
            gameManager.LoadSessionData();
            ScreenManager screenManager = GameObject.FindObjectOfType<ScreenManager>();
            screenManager.ChangeScreen(2);
        }
        else
        {
            Debug.LogError("Failed to parse JSON or no sessions found.");
        }
    }
}

// Update session data
IEnumerator PutRequest(Session session)
{
    string json = JsonUtility.ToJson(session);
    byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);

    UnityWebRequest request = UnityWebRequest.Put(url, jsonBytes);
    request.SetRequestHeader("Content-Type", "application/json");

    yield return request.SendWebRequest();

    if (request.result != UnityWebRequest.Result.Success)
    {
        Debug.LogError("Error: " + request.error);
    }
    else
    {
        if (currentSession.newSessionCode != null)
        {
            currentSession.sessionCode = currentSession.newSessionCode;
        }

        Debug.Log("Session saved successfully!");
    }
    currentSession.newSessionCode = null;
}

// Update specific party slider values from the given partyID
IEnumerator UpdatePartValueRequest(int partyID, Session session)
{
    string json = JsonUtility.ToJson(session);
    byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);

    UnityWebRequest request = UnityWebRequest.Put(url + "/party/" + partyID, jsonBytes);
    request.SetRequestHeader("Content-Type", "application/json");

    yield return request.SendWebRequest();

    if (request.result != UnityWebRequest.Result.Success)
    {
        Debug.LogError("Error: " + request.error);
    }
    else
    {
        Debug.Log("Session saved successfully!");
    }
}

// Create a new session
IEnumerator CreateSessionRequest(Session session)
{
    string json = JsonUtility.ToJson(session);
    byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);

    UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
    request.uploadHandler = new UploadHandlerRaw(jsonBytes);
    request.downloadHandler = new DownloadHandlerBuffer();
    request.SetRequestHeader("Content-Type", "application/json");

    yield return request.SendWebRequest();

    if (request.result != UnityWebRequest.Result.Success)
    {
        switch (request.responseCode)
        {
            case 409:
                sessionErrorText.text = "Een sessie met deze code bestaat al";
                Debug.LogError("Conflict: " + request.error);
                break;
            case 422:
                sessionErrorText.text = "Vul A.U.B. een sessie code in";
                Debug.LogError("Conflict: " + request.error);
                break;
            default:
                sessionErrorText.text = "Server error probeer a.u.b. later";
                Debug.LogError("Error: " + request.error);
                break;
        }
    }
    else
    {
        sessionErrorText.text = "";
        Debug.Log("Session created successfully!");
        currentSession = session;
        ScreenManager screenManager = GameObject.FindObjectOfType<ScreenManager>();
        screenManager.ChangeScreen(2);
    }
}


}
