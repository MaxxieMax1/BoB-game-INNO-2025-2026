using System;
using System.Collections.Generic;

[Serializable]
public class MeasureResultDto
{
    public string measureName;
    public List<int> scoresPerParty;
    public double averageScore;
}

[Serializable]
public class ResultExportDto
{
    public List<string> partyNames;
    public List<MeasureResultDto> measures;

    public List<double> sentimentPerParty;      
    public List<double> goalPercentagePerParty;  
    public List<double> budgetPercentagePerParty;

    public double averageGoalPercentage;         
    public double averageBudgetPercentage;       
}
