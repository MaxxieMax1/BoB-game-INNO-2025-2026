using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Party
{
    public string partyName;
    public string description;

    public Party(string partyName, string description)
    {
        this.partyName = partyName;
        this.description = description;
    }

    public string PartyName
    {
        get { return partyName; }
        set { partyName = value; }
    }

    public string Description
    {
        get { return description; }
        set { description = value; }
    }
}
