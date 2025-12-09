using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Text;

public class ButtonToApi : MonoBehaviour
{
    [Header("Gemiddelde velden Tygron")]
    public TMP_Text average1;
    public TMP_Text average2;
    public TMP_Text average3;
    public TMP_Text average4;
    public TMP_Text average5;

    [Header("Extra averages voor dashboard")]
    public TMP_Text draagvlakAverage;  
    public TMP_Text doelAverage; 
    public TMP_Text budgetAverage;     

    [Header("Token invoerveld")]
    public TMP_InputField apiTokenInput;

    [Header("Endpoint basis URLs (zonder token)")]
    public string setAttributesBaseUrl = "https://engine.tygron.com/api/session/event/editorparametric/set_attributes/";
    public string generateBaseUrl = "https://engine.tygron.com/api/session/event/editorparametric/generate/";

    [Header("Dashboard endpoint (VOLLEDIGE URL)")]
    public string dashboardUrl = "https://httpbin.org/post";

    private string apiToken = "";

    public void OnSendButtonClick()
    {

        apiToken = apiTokenInput.text.Trim();

        if (string.IsNullOrEmpty(apiToken))
        {
            Debug.LogError("Geen API-token ingevuld!");
            return;
        }

        string setAttributesUrl = $"{setAttributesBaseUrl}?token={apiToken}";
        string generateUrl = $"{generateBaseUrl}?token={apiToken}";

        StartCoroutine(SendAveragesAndGenerate(setAttributesUrl, generateUrl));

        StartCoroutine(SendDashboardData());
    }

    private IEnumerator SendAveragesAndGenerate(string setAttributesUrl, string generateUrl)
    {
        float.TryParse(average1.text.Replace(',', '.'), out float val1);
        float.TryParse(average2.text.Replace(',', '.'), out float val2);
        float.TryParse(average3.text.Replace(',', '.'), out float val3);
        float.TryParse(average4.text.Replace(',', '.'), out float val4);
        float.TryParse(average5.text.Replace(',', '.'), out float val5);

        val1 /= 10f;
        val2 /= 10f;
        val3 /= 10f;
        val4 /= 10f;
        val5 /= 10f;

        string jsonData = $@"
[
  [8, 8, 8, 8, 8],
  [""FRACTION_BUILDINGS"", ""FRACTION_GARDENS"", ""FRACTION_PARKING"", ""FRACTION_PUBLIC_GREEN"", ""FRACTION_ROADS""],
  [[{val1}], [{val2}], [{val3}], [{val4}], [{val5}]]
]";

        Debug.Log("Verzenden JSON naar Tygron:\n" + jsonData);

        using (UnityWebRequest www = new UnityWebRequest(setAttributesUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Set attributes succesvol: " + www.downloadHandler.text);

                yield return GenerateParametric(generateUrl);
            }
            else
            {
                Debug.LogError("Fout bij set_attributes: " + www.error);
                Debug.LogError("Response: " + www.downloadHandler.text);
            }
        }
    }

    private IEnumerator GenerateParametric(string generateUrl)
    {
        int parametricDesignId = 8;
        string jsonData = $"[{parametricDesignId}]";

        Debug.Log("Start generate request met JSON: " + jsonData);

        using (UnityWebRequest www = new UnityWebRequest(generateUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Generate succesvol: " + www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Fout bij generate: " + www.error);
                Debug.LogError("Response: " + www.downloadHandler.text);
            }
        }
    }


    [System.Serializable]
    private class DashboardPayload
    {
        public float average1;
        public float average2;
        public float average3;
        public float average4;
        public float average5;
        public float draagvlakAverage;
        public float doelAverage;
        public float budgetAverage;
    }

    private IEnumerator SendDashboardData()
{
    if (string.IsNullOrEmpty(dashboardUrl))
    {
        Debug.LogWarning("Geen dashboardUrl ingesteld.");
        yield break;
    }

    Debug.Log("=== Start SendDashboardData ===");

    float.TryParse(average1.text.Replace(',', '.'), out float a1);
    float.TryParse(average2.text.Replace(',', '.'), out float a2);
    float.TryParse(average3.text.Replace(',', '.'), out float a3);
    float.TryParse(average4.text.Replace(',', '.'), out float a4);
    float.TryParse(average5.text.Replace(',', '.'), out float a5);

    float.TryParse(draagvlakAverage.text.Replace("%", "").Replace(',', '.'), out float draagvlak);
    float.TryParse(doelAverage.text.Replace("%", "").Replace(',', '.'), out float doel);
    float.TryParse(budgetAverage.text.Replace("%", "").Replace(',', '.'), out float budget);

    Debug.Log($"Dashboard values:");
    Debug.Log($"Average1: {a1}, Average2: {a2}, Average3: {a3}, Average4: {a4}, Average5: {a5}");
    Debug.Log($"Draagvlak: {draagvlak}, Doel: {doel}, Budget: {budget}");

    DashboardPayload payload = new DashboardPayload
    {
        average1 = a1,
        average2 = a2,
        average3 = a3,
        average4 = a4,
        average5 = a5,
        draagvlakAverage = draagvlak,
        doelAverage = doel,
        budgetAverage = budget
    };

    string json = JsonUtility.ToJson(payload, true);

    Debug.Log("JSON dat naar dashboard wordt verstuurd:\n" + json);
    Debug.Log("Dashboard endpoint URL: " + dashboardUrl);

    using (UnityWebRequest www = new UnityWebRequest(dashboardUrl, "POST"))
    {
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("Versturen...");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Dashboard request SUCCESVOL!");
            Debug.Log("Response:\n" + www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Dashboard request MISLUKT: " + www.error);
            Debug.LogError("Response:\n" + www.downloadHandler.text);
        }

        Debug.Log("=== Einde SendDashboardData ===");
    }
}
}
