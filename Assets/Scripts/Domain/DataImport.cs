using SFB;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataImport : MonoBehaviour
{
    public List<Measure> measures = new List<Measure>();
    public List<Party> parties = new List<Party>();
    public string description;
    public double budget;
    public double peopleNeedingHelp;
    private StreamReader file;

    // Import session settings data from a CSV file
    public void ImportCSV()
    {
        measures.Clear();
        parties.Clear();

        var extensions = new[]
        {
            new ExtensionFilter("Text Files", "txt", "csv"),
            new ExtensionFilter("All Files", "*"),
        };

        StandaloneFileBrowser.OpenFilePanelAsync("Open File", "", extensions, false, (string[] paths) =>
        {
            if (paths.Length == 0) return;

            GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
            using StreamReader file = new(paths[0]);

            bool pastEmptyLine = false;
            bool pastSecondEmptyLine = false;
            int section = 0;

            while (!file.EndOfStream)
            {
                string fileLine = file.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(fileLine))
                {
                    if (!pastEmptyLine)
                    {
                        pastEmptyLine = true;
                        section = 1;
                    }
                    else if (!pastSecondEmptyLine)
                    {
                        pastSecondEmptyLine = true;
                        section = 2;
                    }
                }

                string[] data = fileLine.Split(';');
                switch (section)
                {
                    case 0:
                        if (data.Length >= 2 && double.TryParse(data[0], out double budgetValue) &&
                            double.TryParse(data[1], out double peopleValue))
                        {
                            budget = budgetValue;
                            peopleNeedingHelp = peopleValue;
                            description = data[2];
                        }
                        break;

                    case 1:
                        if (data.Length >= 4 &&
                            double.TryParse(data[1], out double cost) &&
                            double.TryParse(data[2], out double helpedPeople) &&
                            double.TryParse(data[3], out double volunteers))
                        {
                            Measure measure = new(data[0], cost, helpedPeople, volunteers);
                            measures.Add(measure);
                        }
                        break;

                    case 2:
                        if (data.Length >= 2)
                        {
                            Party party = new(data[0], data[1]);
                            parties.Add(party);
                        }
                        break;
                }
            }

            gameManager.updateAfterImport(this);
        });
    }

    public List<Measure> GetMeasures()
    {
        return measures;
    }

    public List<Party> GetParties()
    {
        return parties;
    }
}
