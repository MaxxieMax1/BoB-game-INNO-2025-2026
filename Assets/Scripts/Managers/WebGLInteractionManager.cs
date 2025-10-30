using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WebGLInteractionManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField startScreenSessionCode;
    private string paramValue;

    // In the browser there is a JS script that checks the url parameter
    // and calls this if the url contains a session code
    public void JoinGameFromUrlParameter(string paramValue)
    {
        SessionManager sessionManager = FindObjectOfType<SessionManager>();
        sessionManager.JoinSession(paramValue);
        startScreenSessionCode.text = paramValue;
    }
}
