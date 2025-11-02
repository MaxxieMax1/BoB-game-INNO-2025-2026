using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class ButtonToApi : MonoBehaviour
{
    [Header("Average velden")]
    public TMP_Text average1;
    public TMP_Text average2;
    public TMP_Text average3;
    public TMP_Text average4;
    public TMP_Text average5;

    [Header("Endpoint instellingen")]
    public string setAttributesUrl = "https://engine.tygron.com/api/session/event/editorparametric/set_attributes/?token=214aebafisIjmAq8v6VXHqJEW2n0U8Ds";
    public string generateUrl = "https://engine.tygron.com/api/session/event/editorparametric/generate/?token=214aebafisIjmAq8v6VXHqJEW2n0U8Ds";

    public void OnSendButtonClick()
    {
        StartCoroutine(SendAveragesAndGenerate());
    }

    private IEnumerator SendAveragesAndGenerate()
    {
        // Zet tekstwaarden om naar floats
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

        // Bouw de JSON handmatig
        string jsonData = $@"
[
  [14, 14, 14, 14, 14],
  [""FRACTION_BUILDINGS"", ""FRACTION_GARDENS"", ""FRACTION_PARKING"", ""FRACTION_PUBLIC_GREEN"", ""FRACTION_ROADS""],
  [[{val1}], [{val2}], [{val3}], [{val4}], [{val5}]]
]";

        Debug.Log("Verzenden JSON:\n" + jsonData);

        // === Eerste call: set_attributes ===
        using (UnityWebRequest www = new UnityWebRequest(setAttributesUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Set attributes succesvol: " + www.downloadHandler.text);

                // === Tweede call: generate ===
                yield return GenerateParametric();
            }
            else
            {
                Debug.LogError("Fout bij set_attributes: " + www.error);
                Debug.LogError("Response: " + www.downloadHandler.text);
            }
        }
    }

        private IEnumerator GenerateParametric()
    {
        // Bouw JSON body met Parametric Design ID
        int parametricDesignId = 14; // dit kan je ook dynamisch maken
        string jsonData = $"[{parametricDesignId}]";

        Debug.Log("Start generate request met JSON: " + jsonData);

        using (UnityWebRequest www = new UnityWebRequest(generateUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
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
}
