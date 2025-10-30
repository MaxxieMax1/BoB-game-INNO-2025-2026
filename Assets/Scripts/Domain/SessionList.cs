using System.Collections.Generic;
using UnityEngine;


public class SessionList
{
    private readonly string filePath = Application.dataPath + "/Resources/attributes.json";

    public List<Session> sessions;

    public SessionList(List<Session> sessions)
    {
        this.sessions = sessions;
    }

    public void AddSession(Session session)
    {
        int index = sessions.FindIndex(s => s.SessionCode == session.SessionCode);
        if (index != -1)
        {
            sessions[index] = session;
        } else
        {
            sessions.Add(session);
        }
    }

    public Session FindSession(string sessionCode)
    {
        return sessions.Find(session => session.SessionCode == sessionCode);
    }

}
