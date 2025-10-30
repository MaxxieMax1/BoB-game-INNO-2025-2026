using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Old  method to load data instead from a server side CRUD service
public class LoadData : MonoBehaviour
{

    List<Measure> measures = new List<Measure>();

    public LoadData()
    {
       
    }

    public void Initialize()
    {
        measures.Add(new Measure("Online Cursussen", 10, 100, 500));
        measures.Add(new Measure("Inloopspreekuren", 10, 50, 200));
        measures.Add(new Measure("Digisterker cursus", 10, 50, 150));
        measures.Add(new Measure("Digi-hulplijn", 10, 200, 100));
        measures.Add(new Measure("Digi-hulplijn", 10, 1, 1));

    }


    public List<Measure> GetData()
    {
        return measures;
    }

    public void ChangeMeasure(Measure measure, int id)
    {
        measures[id] = measure;
    }


    public void AddMeasure(Measure measure)
    {
        measures.Add(measure);
    }

    public void ClearMeasures()
    {
        measures.Clear();
    }
}
