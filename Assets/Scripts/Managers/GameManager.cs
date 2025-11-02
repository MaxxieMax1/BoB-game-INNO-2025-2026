using SFB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private List<PartyValues> partyValues = new List<PartyValues>();
    [SerializeField] private List<GameObject> partyButtons = new List<GameObject>();
    [SerializeField] private int currentPartyId;
    [SerializeField] private TMP_Text mainmenuDescription;
    public SessionManager sessionManager;
    private LoadData loadData = new LoadData();

    [Header("Slider Screen")]
    [SerializeField] private List<TextMeshProUGUI> meassurement = new List<TextMeshProUGUI>();
    [SerializeField] private List<TextMeshProUGUI> costs = new List<TextMeshProUGUI>();
    [SerializeField] private List<TextMeshProUGUI> peopleHelped = new List<TextMeshProUGUI>();
    [SerializeField] private List<TextMeshProUGUI> volunteers = new List<TextMeshProUGUI>();
    [SerializeField] private List<Slider> sliders = new List<Slider>();
    [SerializeField] private TextMeshProUGUI avgStats;

    [SerializeField] private GameObject measurePrefabSlider;
    private double budget;
    private double peopleNeedingHelp;
    [SerializeField] private TextMeshProUGUI budgetText;
    [SerializeField] private TextMeshProUGUI peopleNeedingHelpText;
    [SerializeField] private GameObject emojiBudget;
    [SerializeField] private GameObject emojiHelpedPeople;


    [Header("Parameter Screen")]
    [SerializeField] private List<TMP_InputField> parameterMeasurement = new List<TMP_InputField>();
    [SerializeField] private List<TMP_InputField> parameterMoney = new List<TMP_InputField>();
    [SerializeField] private List<TMP_InputField> parameterVolunteers = new List<TMP_InputField>();
    [SerializeField] private List<TMP_InputField> parameterPeopleHelped = new List<TMP_InputField>();
    [SerializeField] private TMP_InputField gameDescription;
    [SerializeField] private List<TMP_InputField> partyNames = new List<TMP_InputField>();
    [SerializeField] private List<TMP_InputField> partyDescriptions = new List<TMP_InputField>();

    [SerializeField] private TMP_InputField parameterBudget;
    [SerializeField] private TMP_InputField parameterPeopleNeedingHelp;
    [SerializeField] private TMP_InputField parameterSessionCode;

    [SerializeField] private GameObject measurePrefabParam;


    [Header("Result Screen")]
    [SerializeField] private List<TextMeshProUGUI> averageMessurementScore = new List<TextMeshProUGUI>();
    [SerializeField] private List<TextMeshProUGUI> tableMeasurements = new List<TextMeshProUGUI>();
    [SerializeField] private List<TextMeshProUGUI> tableParty1 = new List<TextMeshProUGUI>();
    [SerializeField] private List<TextMeshProUGUI> tableParty2 = new List<TextMeshProUGUI>();
    [SerializeField] private List<TextMeshProUGUI> tableParty3 = new List<TextMeshProUGUI>();
    [SerializeField] private List<TextMeshProUGUI> tableParty4 = new List<TextMeshProUGUI>();
    [SerializeField] private List<TextMeshProUGUI> tableAverage = new List<TextMeshProUGUI>();
    [SerializeField] private List<TextMeshProUGUI> sentimentScores = new List<TextMeshProUGUI>();
    [SerializeField] private TextMeshProUGUI averageGoal;
    [SerializeField] private TextMeshProUGUI averageMoney;


    private List<double> avgScores = new List<double>();
    private bool roundStatus =  false;

    [SerializeField] private GameObject measurePrefabTable;
    [SerializeField] private GameObject sentimentObj;

    [SerializeField] private List<TextMeshProUGUI> tableGoals = new List<TextMeshProUGUI>();
    [SerializeField] private List<TextMeshProUGUI> tableBudgets = new List<TextMeshProUGUI>();

    private void Start()
    {
        Initialize(8);
    }

    private void Initialize(int amountOfValues)
    {
        loadData.Initialize();
        foreach (GameObject item in partyButtons)
        {
            partyValues.Add(new PartyValues(amountOfValues));

        }
    }

    public void UpdateSessionPartyValue()
    {
        sessionManager.UpdateCurrentSessionPartyValues(currentPartyId);
    }

    public void LoadSessionData()
    {
        Session session = sessionManager.currentSession;
        clearParamters();

        List<Measure> measures = session.Measures;
        List<Party> parties = session.Parties;

        for (int i = 0; i < partyButtons.Count; i++)
        {
            partyButtons[i].GetComponentInChildren<TMP_Text>().text = parties[i].PartyName;
        }

        budget = session.Budget;
        peopleNeedingHelp = session.PeopleNeedingHelp;

        mainmenuDescription.text = session.description.ToString();
        gameDescription.text = session.description.ToString();

        parameterSessionCode.text = session.sessionCode.ToString();
        parameterBudget.text = session.budget.ToString();
        parameterPeopleNeedingHelp.text = session.peopleNeedingHelp.ToString();

        for (int i = 0; i < measures.Count; i++)
        {
            Measure measure = measures[i];
            sessionManager.currentSession.measures[i] = measure;
            parameterMeasurement[i].text = measure.MeasureName;
            parameterMoney[i].text = measure.cost.ToString();
            parameterVolunteers[i].text = measure.volunteers.ToString();
            parameterPeopleHelped[i].text = measure.PeopleHelped.ToString();

            for (int p = 0; p < partyValues.Count; p++)
            {
                partyValues[p].Values[i] = measure.valuePerParty[p].Value;
            }
        }

        for (int i = 0; i < parties.Count; i++)
        {
            Party party = parties[i];
            partyNames[i].text = party.PartyName.ToString();
            partyDescriptions[i].text = party.Description.ToString();
        }

        CalculateGoal();
        //CallInsertToTable();
    }

    public void MapValuesToSliders(int id)
    {
        Slider currentSlider = sliders[id];

        int value = (int)currentSlider.value;

        partyValues[currentPartyId].Values[id] = value;

        // get the current measure
        Measure measure = sessionManager.currentSession.measures[id];
        // function to get cost
        double cost = measure.CalculateCost(value);
        // function to get volunteers
        costs[id].text = "€ " + cost.ToString();
        peopleHelped[id].text = measure.CalculatePeopleHelped(value).ToString();
        volunteers[id].text = measure.CalculateVolunteers(value).ToString();

        for (int i = 0; i < sessionManager.currentSession.measures.Count; i++)
        {
            meassurement[i].text = sessionManager.currentSession.measures[i].measureName; 
        }

        measure.UpdateValues(currentPartyId, value);

        GetAverageScorePerMessurement();
        CalculateGoal();
        CallInsertToTable();
    }

    //Calculates amount of money used and amount of people helped
    private void CalculateGoal()
    {
        double moneyUsed = 0;

        for (int i = 0; i < sessionManager.currentSession.measures.Count(); i++)
        {
            Measure measure = sessionManager.currentSession.measures[i];
            
            moneyUsed = moneyUsed + measure.CalculateCost(partyValues[currentPartyId].Values[i]);

        }
        double budgetPercentage = moneyUsed / sessionManager.currentSession.budget * 100;

        Emoji budgetEmoji = emojiBudget.GetComponent<Emoji>();
        Emoji peopleEmoji = emojiHelpedPeople.GetComponent<Emoji>();

        budgetText.text = string.Format("{0}% van het budget", budgetPercentage.ToString("0"));
        if (budgetPercentage > 100)
        {
            budgetText.color = Color.red;
            budgetEmoji.Sad();
        }
        else
        {
            budgetText.color = Color.green;
            budgetEmoji.Happy();
        }

        double peopleHelped = 0;

        for (int i = 0; i < sessionManager.currentSession.measures.Count; i++)
        {
            Measure measure = sessionManager.currentSession.measures[i];

            peopleHelped = peopleHelped + measure.CalculatePeopleHelped(partyValues[currentPartyId].Values[i]);

        }
        double helpedPercentage = peopleHelped / peopleNeedingHelp * 100;

        peopleNeedingHelpText.text = string.Format("{0}% van het doel", helpedPercentage.ToString("0"));

        if (helpedPercentage >= 100)
        {
            peopleNeedingHelpText.color = Color.green;
            peopleEmoji.Happy();
        }
        else
        {
            peopleNeedingHelpText.color = Color.red;
            peopleEmoji.Sad();
        }
    }

    public void LoadPartyValuesInSlider()
    {
        Session currentSession = sessionManager.currentSession;
        List<ValuePerParty> values = currentSession.GetAllValues(currentPartyId);

        for (int i = 0; i < sliders.Count; i++)
        {
            if (values[i] != null)
            {
                sliders[i].value = values[i].Value;
            } else
            {
                sliders[i].value = 0;
            }
        }

    }

    // Saves the values inputted in the settings screen and resets the sliders
    public void SaveSettings()
    {
        Session session = sessionManager.currentSession;

        List<Measure> measureList = new List<Measure>();
        List<Party> partiesList = new List<Party>();
        String newSessionCode = parameterSessionCode.text;

        // Update session values
        sessionManager.ClearMeasures();
        for (int i = 0; i < parameterMeasurement.Count; i++)
        {
            measureList.Add(new Measure(parameterMeasurement[i].text,
                Convert.ToDouble(parameterMoney[i].text),
                Convert.ToDouble(parameterPeopleHelped[i].text),
                Convert.ToDouble(parameterVolunteers[i].text)));
            ChangeMeasurements(i);
        }

        for (int i = 0; i < partyButtons.Count; i++)
        {
            partiesList.Add(new Party(partyNames[i].text, partyDescriptions[i].text));
            partyButtons[i].GetComponentInChildren<TMP_Text>().text = partyNames[i].text;
        }

        if (session.sessionCode != newSessionCode)
        {
            sessionManager.SetNewSessionCode(newSessionCode);
        }
        sessionManager.SetMeasures(measureList);
        sessionManager.SetParties(partiesList);
        sessionManager.SetDescription(gameDescription.text);
        sessionManager.SetBudget(Convert.ToDouble(parameterBudget.text));
        sessionManager.SetPeopleNeedingHelp(Convert.ToDouble(parameterPeopleNeedingHelp.text));

        // Update Game Values
        budget = Convert.ToDouble(parameterBudget.text);
        peopleNeedingHelp = Convert.ToDouble(parameterPeopleNeedingHelp.text);

        mainmenuDescription.text = gameDescription.text;

        sessionManager.UpdateCurrentSession();
        CallInsertToTable();
    }

    // Saves the values inputted in the settings screen while keeping the sliders
    public void UpdateSettings()
    {
        Session session = sessionManager.currentSession;

        List<Measure> measureList = session.measures;
        List<Party> partiesList = session.parties;
        String newSessionCode = parameterSessionCode.text;
        
        if (measureList.Count != parameterMeasurement.Count)
        {
            for (int i = 0; i < parameterMeasurement.Count; i++)
            {
                measureList[i] = new Measure(parameterMeasurement[i].text,
                    Convert.ToDouble(parameterMoney[i].text),
                    Convert.ToDouble(parameterPeopleHelped[i].text),
                    Convert.ToDouble(parameterVolunteers[i].text));
            }
        }
        else
        {
            for (int i = 0; i < parameterMeasurement.Count; i++)
            {
                measureList[i].MeasureName = parameterMeasurement[i].text;
                measureList[i].cost = Convert.ToDouble(parameterMoney[i].text);
                measureList[i].PeopleHelped = Convert.ToDouble(parameterPeopleHelped[i].text);
                measureList[i].volunteers = Convert.ToDouble(parameterVolunteers[i].text);
 
            }
        }

        for (int i = 0; i < partyButtons.Count; i++)
        {
            partiesList[i] = new Party(partyNames[i].text, partyDescriptions[i].text);
            partyButtons[i].GetComponentInChildren<TMP_Text>().text = partyNames[i].text;
        }

        if (session.sessionCode != newSessionCode)
        {
            sessionManager.SetNewSessionCode(newSessionCode);
        }
        sessionManager.SetMeasures(measureList);
        sessionManager.SetParties(partiesList);
        sessionManager.SetDescription(gameDescription.text);
        sessionManager.SetBudget(Convert.ToDouble(parameterBudget.text));
        sessionManager.SetPeopleNeedingHelp(Convert.ToDouble(parameterPeopleNeedingHelp.text));

        // Update Game Values
        budget = Convert.ToDouble(parameterBudget.text);
        peopleNeedingHelp = Convert.ToDouble(parameterPeopleNeedingHelp.text);

        mainmenuDescription.text = gameDescription.text;

        sessionManager.UpdateCurrentSession();

        CallInsertToTable();
    }

    //New meassurement in the game
    public void ChangeMeasurements(int id)
    {
        Measure measure = new Measure(parameterMeasurement[id].text, Convert.ToDouble(parameterMoney[id].text), Convert.ToDouble(parameterPeopleHelped[id].text), Convert.ToDouble(parameterVolunteers[id].text));
        sessionManager.currentSession.measures.Add(measure);
        meassurement[id].text = sessionManager.currentSession.measures[id].MeasureName;

        MapValuesToSliders(id);
    }

    //Updates parameter and other values after importing csv file
    public void updateAfterImport(DataImport data)
    {
        clearParamters();
        List<Measure> measures = data.GetMeasures();
        List<Party> parties = data.GetParties();
        gameDescription.text = data.description;
        budget = data.budget;
        peopleNeedingHelp = data.peopleNeedingHelp;

        for (int i = 0; i < measures.Count; i++)
        {
            Measure measure = measures[i];
            sessionManager.currentSession.measures[i] = measure;
            parameterMeasurement[i].text = measure.MeasureName;
            parameterMoney[i].text = measure.cost.ToString();
            parameterVolunteers[i].text = measure.volunteers.ToString();
            parameterPeopleHelped[i].text = measure.PeopleHelped.ToString();
        }

        for (int i = 0; i < parties.Count; i++)
        {
            partyNames[i].text = parties[i].PartyName;
            partyDescriptions[i].text = parties[i].Description;
        }

        sessionManager.UpdateCurrentSession();
    }

    //Exports settings to csv file
    public void ExportSettings()
    {
        var extensions = new[] {
            new ExtensionFilter("Text Files", "txt", "csv" ),
            new ExtensionFilter("All Files", "*" ),
        };

        string filePath = StandaloneFileBrowser.SaveFilePanel("Save File", "", "", "csv");
        if (!string.IsNullOrEmpty(filePath))
        {
            using (StreamWriter file = new(filePath))
            {
                file.WriteLine($"{budget};{parameterPeopleNeedingHelp.text};{gameDescription.text};");
                file.WriteLine("");

                for (int i = 0; i < parameterMeasurement.Count; i++)
                {
                    string line = $"{parameterMeasurement[i].text};{parameterMoney[i].text};{parameterPeopleHelped[i].text};{parameterVolunteers[i].text}";
                    file.WriteLine(line);
                }
                file.WriteLine("");
                for (int i = 0; i < partyNames.Count; i++)
                {
                    string line = $"{partyNames[i].text};{partyDescriptions[i].text}";
                    file.WriteLine(line);
                }
            }
        }
    }

    // Export results
    public void ExportResult()
    {
        var extensions = new[] {
            new ExtensionFilter("Text Files", "txt", "csv" ),
            new ExtensionFilter("All Files", "*" ),
        };

        string filePath = StandaloneFileBrowser.SaveFilePanel("Save File", "", "", "csv");
        if (!string.IsNullOrEmpty(filePath))
        {
            using (StreamWriter file = new(filePath))
            {
                file.WriteLine($";{partyNames[0].text};{partyNames[1].text};{partyNames[2].text};{partyNames[3].text};");

                foreach (Measure measure in sessionManager.currentSession.measures)
                {
                    file.WriteLine($"{measure.MeasureName};{measure.valuePerParty[0].value};{measure.valuePerParty[1].value};{measure.valuePerParty[2].value};{measure.valuePerParty[3].value}");
                }

                file.WriteLine($"Draagvlak;{sentimentScores[0].text};{sentimentScores[1].text};{sentimentScores[2].text};{sentimentScores[3].text};");
            }
        }
    }

    //Calculates average score for each meassurement
    private void GetAverageScorePerMessurement()
    {
        avgScores.Clear();

        for (int i = 0; i < tableMeasurements.Count; i++)
        {
            int totalPerMessure = 0;

            for (int j = 0; j < partyValues.Count; j++)
            {
                totalPerMessure += partyValues[j].Values[i];
            }

            float averageScore = (float)totalPerMessure / (float)partyValues.Count;

            avgScores.Add(averageScore);

        }

        for (int i = 0; i < tableMeasurements.Count; i++)
        {
            tableAverage[i].text = avgScores[i].ToString("0.0");
        }
    }

    //Calls the data to insert into the final table
    public void CallInsertToTable()
    {
        InsertDataInTable(tableParty1, 0);
        InsertDataInTable(tableParty2, 1);
        InsertDataInTable(tableParty3, 2);
        InsertDataInTable(tableParty4, 3);

        GetAverageScorePerMessurement();

        List<double> doubles = partyValues[0].Values.Select<int, double>(i => i).ToList();
        sentimentScores[0].text = calculateSentiment(doubles, avgScores).ToString("0.0");

        doubles = partyValues[1].Values.Select<int, double>(i => i).ToList();
        sentimentScores[1].text = calculateSentiment(doubles, avgScores).ToString("0.0");

        doubles = partyValues[2].Values.Select<int, double>(i => i).ToList();
        sentimentScores[2].text = calculateSentiment(doubles, avgScores).ToString("0.0");

        doubles = partyValues[3].Values.Select<int, double>(i => i).ToList();
        sentimentScores[3].text = calculateSentiment(doubles, avgScores).ToString("0.0");


        for (int i = 0; i < sentimentScores.Count; i++)
        {
            if (Convert.ToDouble(sentimentScores[i].text) < 8)
            {
                sentimentScores[i].color = Color.red;
                sentimentScores[i].GetComponentInChildren<Emoji>().Sad();
            }
            else
            {
                sentimentScores[i].color = Color.green;
                sentimentScores[i].GetComponentInChildren<Emoji>().Happy();
            }
        }
        if (roundStatus == true)
        {
            avgStats.text = string.Format("Draagvlak:\n" +
                                          "{4}: {0}\n" +
                                          "{5}: {1}\n" +
                                          "{6}: {2}\n" +
                                          "{7}: {3}\n",
                                          sentimentScores[0].text, sentimentScores[1].text, sentimentScores[2].text,
                                          sentimentScores[3].text, partyButtons[0].GetComponentInChildren<TMP_Text>().text,
                                          partyButtons[1].GetComponentInChildren<TMP_Text>().text,
                                          partyButtons[2].GetComponentInChildren<TMP_Text>().text,
                                          partyButtons[3].GetComponentInChildren<TMP_Text>().text);
        }
        else
        {
            avgStats.text = "Dit is de eerste ronde";
        }
    }

    //Clears all parameters
    private void clearParamters()
    {
        for (int i = 0; i < parameterMeasurement.Count; i++)
        {
            parameterMeasurement[i].text = "";
            parameterMoney[i].text = "";
        }
    }

    //Inserts data into the table
    private void InsertDataInTable(List<TextMeshProUGUI> tableParty, int id)
    {
        for (int i = 0; i < tableMeasurements.Count; i++)
        {
            tableMeasurements[i].text = meassurement[i].text;
        }

        for (int i = 0; i < tableParty.Count; i++)
        {
            if (i == 0)
            {
                tableParty[i].text = partyButtons[id].GetComponentInChildren<TextMeshProUGUI>().text;
            }
            else
            {
                tableParty[i].text = partyValues[id].Values[i - 1].ToString();
            }
        }

        for (int i = 0; i < tableGoals.Count; i++)
        {
            if (i == 0)
            {
                tableGoals[i].text = "Percentage, doel";
            }
            else
            {
                tableGoals[i].text = string.Format("{0}%", CalculateGoalPercentage(i - 1).ToString("0"));
            }
        }

        for (int i = 0; i < tableBudgets.Count; i++)
        {
            if (i == 0)
            {
                tableBudgets[i].text = "Budget";
            }
            else
            {
                tableBudgets[i].text = string.Format("{0}%", CalculateBudgetPercentage(i - 1).ToString("0"));
            }
        }

        CalculateAverageGoalAndMoneyUsedInPercentage();

    }

    //Calculates the sentimentscore
    public double calculateSentiment(List<double> stakeholderVals, List<double> averages)
    {
        var deviations = new List<double>();
        for (int i = 0; i < averages.Count; i++)
        {
            double deviation = Math.Abs(averages[i] - stakeholderVals[i]);
            deviations.Add(deviation);
        }
        double sumOfDeviations = deviations.Sum();
        double averageDeviation = sumOfDeviations / deviations.Count;
        double maxDeviation = 10;
        double score = 10 - (averageDeviation / maxDeviation * 9);
        score = Math.Max(1, Math.Min(score, 10));
        return score;
    }

    //Calculates the percentage of the people you have helped in relation of the people needing help
    private double CalculateGoalPercentage(int partyId)
    {
        double totalPeopleHelped = 0;

        for (int i = 0; i < sessionManager.currentSession.measures.Count; i++)
        {
            Measure measure = sessionManager.currentSession.measures[i];
            totalPeopleHelped += measure.CalculatePeopleHelped(partyValues[partyId].Values[i]);
        }

        return (totalPeopleHelped / peopleNeedingHelp) * 100;
    }

    private double CalculateBudgetPercentage(int partyId)
    {
        double totalCost = 0;

        for (int i = 0; i < sessionManager.currentSession.measures.Count; i++)
        {
            Measure measure = sessionManager.currentSession.measures[i];
            totalCost += measure.CalculateCost(partyValues[partyId].Values[i]);
        }

        return (totalCost / budget) * 100;

    }

    private void CalculateAverageGoalAndMoneyUsedInPercentage()
    {
        double budget = 0;
        double goal = 0;

        for (int i = 0; i < 4; i++)
        {
            budget = budget + CalculateBudgetPercentage(i);
            goal = goal + CalculateBudgetPercentage(i);
        }

        budget = budget / 4;
        goal = goal / 4;

        averageGoal.text = string.Format("{0}%", goal.ToString("0")); ;
       averageMoney.text = string.Format("{0}%", budget.ToString("0"));
    }

    //Calulates percentage of budget being used
    private double CalculateBudgetUsed(int partyId)
    {
        double totalCost = 0;

        for (int i = 0; i < sessionManager.currentSession.measures.Count; i++)
        {
            Measure measure = sessionManager.currentSession.measures[i];
            totalCost += measure.CalculateCost(partyValues[partyId].Values[i]);
        }

        return totalCost;
    }

    //Adds extra parameter to the settingscreen
    public void addMeasureParam(GameObject measureParent)
    {
        Vector3 measurePos = new Vector3(parameterMeasurement[^1].transform.position.x + 1.5f, parameterMeasurement[^1].transform.position.y - 25f, parameterMeasurement[^1].transform.position.z);

        if (parameterMeasurement.Count < 8)
        {
            GameObject newMeasureParam = Instantiate(measurePrefabParam, measurePos, Quaternion.identity, measureParent.transform);
            Searcher.FindGameObjectInChildWithTag(newMeasureParam, "Text").GetComponent<TMP_Text>().text = "Maatregel " + (parameterMeasurement.Count + 1);
            parameterMeasurement.Add(Searcher.FindGameObjectInChildWithTag(newMeasureParam, "Measure").GetComponent<TMP_InputField>());
            parameterMoney.Add(Searcher.FindGameObjectInChildWithTag(newMeasureParam, "Money").GetComponent<TMP_InputField>());
            parameterVolunteers.Add(Searcher.FindGameObjectInChildWithTag(newMeasureParam, "Volunteers").GetComponent<TMP_InputField>());
            parameterPeopleHelped.Add(Searcher.FindGameObjectInChildWithTag(newMeasureParam, "People").GetComponent<TMP_InputField>());

        }

    }

    //adds extra slider to the sliderscreen
    public void addMeasureSlider(GameObject measureParent)
    {

        Vector3 measurePos = new Vector3(meassurement[^1].transform.position.x, meassurement[^1].transform.position.y - 30f, meassurement[^1].transform.position.z);

        if (meassurement.Count < 8)
        {
            foreach (PartyValues item in partyValues)
            {
                item.addValue(0);
            }

            GameObject newMeasureSlider = Instantiate(measurePrefabSlider, measurePos, Quaternion.identity, measureParent.transform);


            meassurement.Add(Searcher.FindGameObjectInChildWithTag(newMeasureSlider, "Measure").GetComponent<TextMeshProUGUI>());
            costs.Add(Searcher.FindGameObjectInChildWithTag(newMeasureSlider, "Money").GetComponent<TextMeshProUGUI>());
            Slider sliderComponent = newMeasureSlider.GetComponentInChildren<Slider>();
            sliders.Add(sliderComponent);

            int index = sliders.Count - 1;
            sliderComponent.onValueChanged.AddListener((value) => MapValuesToSliders(index));


            volunteers.Add(Searcher.FindGameObjectInChildWithTag(newMeasureSlider, "Volunteers").GetComponent<TextMeshProUGUI>());
            peopleHelped.Add(Searcher.FindGameObjectInChildWithTag(newMeasureSlider, "People").GetComponent<TextMeshProUGUI>());

           
        }

    }

    //Adds extra table entry 
    public void addMeasureTable(GameObject measureParent)
    {
        Vector3 measurePos = new Vector3(0.001f, tableMeasurements[^1].transform.position.y - 100f, tableMeasurements[^1].transform.position.z);

        if (tableMeasurements.Count < 8)
        {
            GameObject newMeasureTable = Instantiate(measurePrefabTable, measurePos, Quaternion.identity, measureParent.transform);
            newMeasureTable.transform.SetSiblingIndex(sentimentObj.transform.parent.childCount - 4);
            tableMeasurements.Add(Searcher.FindGameObjectInChildWithTag(newMeasureTable, "Measure").GetComponent<TextMeshProUGUI>());
            tableParty1.Add(Searcher.FindGameObjectInChildWithTag(newMeasureTable, "1").GetComponent<TextMeshProUGUI>());
            tableParty2.Add(Searcher.FindGameObjectInChildWithTag(newMeasureTable, "2").GetComponent<TextMeshProUGUI>());
            tableParty3.Add(Searcher.FindGameObjectInChildWithTag(newMeasureTable, "3").GetComponent<TextMeshProUGUI>());
            tableParty4.Add(Searcher.FindGameObjectInChildWithTag(newMeasureTable, "4").GetComponent<TextMeshProUGUI>());
            tableAverage.Add(Searcher.FindGameObjectInChildWithTag(newMeasureTable, "avg").GetComponent<TextMeshProUGUI>());

            //sentimentObj.transform.SetSiblingIndex(sentimentObj.transform.parent.childCount);
        }
    }

    public void ResyncSession()
    {
        sessionManager.ResyncCurrentSession();
    }

    public void FirstRoundStatus(int round)
    {
        if (round == 1)
        {
            roundStatus = true;
        }
        else
        {
            roundStatus = false;
        }
    }

    public int CurrentPartyId
    {
        set
        {
            currentPartyId = value;
            ResyncSession();
        }
    }

}
