using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;


public class PartyValues {

    private List<int> values = new List<int>();

    public PartyValues(int amountOfValues) 
    {
        for (int i = 0; i < amountOfValues; i++)
        {
            values.Add(0);
        }
    }

    public List<int> Values { get {  return values; } }

    public void addValue(int value) {
        values.Add(value);
    }

    public void changeValue(int value, int valueId) {

        int valueToChange = values.ElementAt(valueId);

        if (valueToChange !< 0)
        {
            values[valueToChange] = value;
        }
    }

    public void ClearValues()
    {
        values.Clear();
    }
}
