using System;

[Serializable]
public class ValuePerParty
{
    public int partyId;
    public int value;

    public ValuePerParty(int partyId, int value)
    {
        this.partyId = partyId;
        this.value = value;
    }

    public int PartyId
    {
        get { return partyId; }
        set { partyId = value; }
    }

    public int Value
    {
        get { return value; }
        set { this.value = value; }
    }
}
