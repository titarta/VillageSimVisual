using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class EnvStep
{
    public string action;
    public int timeStamp;
    public int agentID;
    public int foodReserved;
    public double saturation;
    public int plantID;
    public EnvStep(string action, int timeStamp, int agentID, int foodReserved, double saturation)
    {
        this.action = action;
        this.timeStamp = timeStamp;
        this.agentID = agentID;
        this.foodReserved = foodReserved;
        this.saturation = saturation;
        this.plantID = -1;
    }

    public EnvStep(string action, int timeStamp, int agentID, int foodReserved, double saturation, int plantID)
    {
        this.action = action;
        this.timeStamp = timeStamp;
        this.agentID = agentID;
        this.foodReserved = foodReserved;
        this.saturation = saturation;
        this.plantID = plantID;
    }


}

public class EnvManagerScript : MonoBehaviour
{
    private List<EnvStep> envSteps = new List<EnvStep>();
    public GameObject agent0;
    public GameObject agent1;
    public GameObject agent2;
    public GameObject agent3;
    public GameObject timeText;
    public GameObject farm;
    private double simulationTime;
    private double Second2HourConversion = 4;


    // Start is called before the first frame update
    void Start()
    {

        string[] lines = System.IO.File.ReadAllLines(@"C:\Users\trvca\Desktop\VillageSimulation\Outputs\OutputUI");
        envSteps = new List<EnvStep>();

        foreach(string line in lines)
        {
            string[] lineSplitted = line.Split(',');
            if(lineSplitted.Length == 5)
            {
                envSteps.Add(new EnvStep(lineSplitted[0], Int32.Parse(lineSplitted[1]), Int32.Parse(lineSplitted[2]), Int32.Parse(lineSplitted[3]), Double.Parse(lineSplitted[4], CultureInfo.InvariantCulture)));
            } else
            {
                envSteps.Add(new EnvStep(lineSplitted[0], Int32.Parse(lineSplitted[1]), Int32.Parse(lineSplitted[2]), Int32.Parse(lineSplitted[3]), Double.Parse(lineSplitted[4], CultureInfo.InvariantCulture), Int32.Parse(lineSplitted[5])));
            }
            
        }
        simulationTime = 0;
    }

    public GameObject getAgentFromID(int agentID)
    {
        switch(agentID)
        {
            case 0:
                return agent0;
            case 1:
                return agent1;
            case 2:
                return agent2;
            case 3:
                return agent3;
            default:
                return agent0;

        }
    }


    // Update is called once per frame
    void Update()
    {
        simulationTime += Time.deltaTime / Second2HourConversion;
        string hour = string.Format("{0:00}", Math.Floor(simulationTime % 24));
        string minute = string.Format("{0:00}", Math.Floor((simulationTime - Math.Floor(simulationTime)) * 60));
        timeText.GetComponent<Text>().text = hour + ":" + minute;

        while(envSteps.Count != 0)
        {
            if(envSteps[0].timeStamp <= simulationTime * 4)
            {
                handleEnvStep(envSteps[0]);
                envSteps.RemoveAt(0);
            } else
            {
                break;
            }
            
        }
    }

    private void handleEnvStep(EnvStep step)
    {
        switch(step.action)
        {
            case "StartChangePositionToHouse":
                Debug.Log("house");
                getAgentFromID(step.agentID).GetComponent<agentManager>().goToHouse();
                break;
            case "ChangePositionToHouse":
                getAgentFromID(step.agentID).GetComponent<agentManager>().updatePos("house");
                break;
            case "StartChangePositionToFarm":
                Debug.Log("farm");
                getAgentFromID(step.agentID).GetComponent<agentManager>().goToFarm(step.plantID);
                break;
            case "ChangePositionToFarm":
                getAgentFromID(step.agentID).GetComponent<agentManager>().updatePos("farm");
                break;
            case "StartChangePositionToWorkshop":
                Debug.Log("woork");
                getAgentFromID(step.agentID).GetComponent<agentManager>().goToWork();
                break;
            case "ChangePositionToWorkshop":
                getAgentFromID(step.agentID).GetComponent<agentManager>().updatePos("work");
                break;
            case "WorkStart":
                getAgentFromID(step.agentID).GetComponent<agentManager>().workStart();
                break;
            case "WorkFinish":
                getAgentFromID(step.agentID).GetComponent<agentManager>().workFinish();
                break;
            case "PlantStart":
                getAgentFromID(step.agentID).GetComponent<agentManager>().plantStart();
                break;
            case "PlantFinish":
                getAgentFromID(step.agentID).GetComponent<agentManager>().plantFinish();
                break;
            case "PlantSeed":
                farm.GetComponent<farmManager>().plantSeed(step.plantID % 4, step.plantID / 4);
                break;
            case "PlantGrow":
                farm.GetComponent<farmManager>().grow(step.plantID % 4, step.plantID / 4);
                break;
            case "PlantHarvest":
                farm.GetComponent<farmManager>().harvest(step.plantID % 4, step.plantID / 4);
                break;
            case "FunStart":
                getAgentFromID(step.agentID).GetComponent<agentManager>().funStart();
                break;
            case "FunFinish":
                getAgentFromID(step.agentID).GetComponent<agentManager>().funFinish();
                break;
            case "SleepStart":
                getAgentFromID(step.agentID).GetComponent<agentManager>().sleepStart();
                break;
            case "SleepFinish":
                getAgentFromID(step.agentID).GetComponent<agentManager>().sleepFinish();
                break;
            case "FoodConsumptionStart":
                getAgentFromID(step.agentID).GetComponent<agentManager>().foodStart();
                break;
            case "FoodConsumptionEnd":
                getAgentFromID(step.agentID).GetComponent<agentManager>().foodEnd();
                break;
            case "HarvestStart":
                getAgentFromID(step.agentID).GetComponent<agentManager>().harvestStart();
                break;
            case "HarvestFinish":
                Debug.Log(step.plantID);
                getAgentFromID(step.agentID).GetComponent<agentManager>().harvestFinish();
                break;

        }
    }

}
