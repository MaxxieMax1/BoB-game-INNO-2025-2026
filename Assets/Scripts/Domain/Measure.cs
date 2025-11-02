using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Measure
{
    public string measureName;
    public double cost;
    public double peopleHelped;
    public double volunteers;
    public List<ValuePerParty> valuePerParty;

    public Measure(string measureName, double cost, double peopleHelped, double volunteers)
    {
        this.measureName = measureName;
        this.cost = cost;
        this.peopleHelped = peopleHelped;
        this.volunteers = volunteers;
        valuePerParty = new List<ValuePerParty>() {
            new(0, 0), 
            new(1, 0), 
            new(2, 0), 
            new(3, 0)
        };
    }

    public void UpdateValues(int partyId, int partyValue)
    {
        if (valuePerParty.Any(v => v.PartyId == partyId))
        {
            valuePerParty.Find(v => v.PartyId == partyId).Value = partyValue;
        }
        else
        {
            valuePerParty.Add(new ValuePerParty(partyId, partyValue));
        }
    }

    public void GetValues(int partyId)
    {
        if (valuePerParty.Any(v => v.PartyId == partyId))
        {
            valuePerParty.Find(v => v.PartyId == partyId).Value = valuePerParty.Find(v => v.PartyId == partyId).Value;
        }
    }

    public double CalculateCost(int value)
    {
        double calculateValue = value * cost;
        return calculateValue;
    }

    public double CalculateVolunteers(int value)
    {
        double calculateVolunteers = value * volunteers;
        return calculateVolunteers;
    }

    public double CalculatePeopleHelped(int value)
    {
        double calculatePeople = value * peopleHelped;
        return calculatePeople;
    }

    public double PeopleHelped
    {
        get { return peopleHelped; }
        set { peopleHelped = value; }
    }

    public double Volunteers
    {
        get { return volunteers; }
        set { volunteers = value; }
    }

    public string MeasureName
    {
        get { return measureName; }
        set { measureName = value; }
    }

    public double Cost
    {
        get { return cost; }
        set { cost = value; }
    }

    public List<ValuePerParty> ValuePerParty
    {
        get { return valuePerParty; }
        set { valuePerParty = value; }
    }

    public override string ToString()
    {
        return "Measure: " + measureName + " Cost: " + cost + " People Helped: " + peopleHelped + " Volunteers: " + volunteers + " Value per party " + valuePerParty;
    }

}
