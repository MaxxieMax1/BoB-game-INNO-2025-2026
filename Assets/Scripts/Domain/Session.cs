using System;
using System.Collections.Generic;

[Serializable]
public class Session
{
    public string sessionCode;
    public string? newSessionCode;

    public string description;

    public long lastChange;
    
    public double budget;
    public double peopleNeedingHelp;
    
    public List<Measure> measures = new();
    public List<Party> parties = new();

    public Session()
    {
        
    }

    public Session(string sessionCode)
    {
        this.sessionCode = sessionCode;
        lastChange = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public List<ValuePerParty> GetAllValues(int partyID)
    {
        List<ValuePerParty> valueList = new();
        foreach (Measure measure in measures)
        {
            foreach (ValuePerParty value in measure.valuePerParty)
            {
                if (value.PartyId == partyID)
                {
                    valueList.Add(value);
                }
            }
        }
        return valueList;
    }

    public string SessionCode
    {
        get { return sessionCode; }
        set { sessionCode = value; }
    }

    public string? NewSessionCode
    {
        get { return newSessionCode; }
        set { newSessionCode = value; }
    }

    public List<Measure> Measures
    {
        get { return measures; }
        set { measures = value; }
    }

    public List<Party> Parties
    {
        get { return parties; }
        set { parties = value; }
    }

    public long LastChange
    {
        get { return lastChange; }
        set { lastChange = value; }
    }

    public double Budget
    {
        get { return budget; }
        set { budget = value; }
    }

    public double PeopleNeedingHelp
    {
        get { return peopleNeedingHelp; }
        set { peopleNeedingHelp = value; }
    }

}
