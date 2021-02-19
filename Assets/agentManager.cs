using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathSection
{
    public double timeStart;
    public double timeFinish;
    public Vector2 positionStart;
    public Vector2 positionFinish;
    public string direction;

    public PathSection(double timeStart, double timeFinish, Vector2 positionStart, Vector2 positionFinish, string direction)
    {
        this.timeStart = timeStart;
        this.timeFinish = timeFinish;
        this.positionStart = positionStart;
        this.positionFinish = positionFinish;
        this.direction = direction;
    }

    public Vector2 getPosition(double currentTime)
    {
        double timePercentage = (currentTime - timeStart) / (timeFinish - timeStart);
        return ((positionFinish - positionStart) * (float)timePercentage) + positionStart;
    }

}
public class agentManager : MonoBehaviour
{
    private string currentPosition;
    private double timeToTransfer = 1;
    private double simulationTime;
    private double Second2HourConversion = 4;

    private Vector2 housePosition;
    private Vector2 workPosition;
    private Vector2 centerPosition;
    private Vector2 farmPivotPosition;

    private int farmSlot;
    public string direction;


    public GameObject sleepMesh;
    public GameObject lightMesh;
    public GameObject funMesh;

    public GameObject foodMesh;

    public Sprite plantSprite;
    private Sprite idleSprite;

    private List<PathSection> pathsToTake;

    // Start is called before the first frame update
    void Start()
    {
        idleSprite = GetComponent<SpriteRenderer>().sprite;
        simulationTime = 0;
        housePosition = new Vector2(-6.5f, 1);
        centerPosition = new Vector2(-6.5f, -3);
        farmPivotPosition = new Vector2(-14.5f, -3);
        switch (gameObject.name)
        {
            case "Agent0":
                workPosition = new Vector2(-0.6f, -3);
                break;
            case "Agent1":
                workPosition = new Vector2((float)(-0.6 + 1 * 3.05), -3);
                break;
            case "Agent2":
                workPosition = new Vector2((float)(-0.6 + 2 * 3.05), -3);
                break;
            case "Agent3":
                workPosition = new Vector2((float)(-0.6 + 3 * 3.05), -3);
                break;
        }
        gameObject.transform.position = centerPosition;
        farmSlot = 0;
        direction = "up";
        pathsToTake = new List<PathSection>();
        currentPosition = "center";
        sleepMesh.GetComponent<Renderer>().enabled = false;
        lightMesh.GetComponent<Renderer>().enabled = false;
        funMesh.GetComponent<Renderer>().enabled = false;
        foodMesh.GetComponent<Renderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        simulationTime += Time.deltaTime / Second2HourConversion;
        if(pathsToTake.Count != 0)
        {
            if(pathsToTake[0].timeStart < simulationTime && simulationTime < pathsToTake[0].timeFinish)
            {
                gameObject.transform.position = pathsToTake[0].getPosition(simulationTime);
                if(direction != pathsToTake[0].direction)
                {
                    direction = pathsToTake[0].direction;
                    playAnimation(direction);
                }
            } else
            {
                if(simulationTime >= pathsToTake[0].timeFinish)
                {
                    pathsToTake.RemoveAt(0);
                }
            }
        }
    }

    public void goToHouse()
    {
        if (currentPosition == "house")
        {
            return;
        }

        if(currentPosition == "farm")
        {
            int posX = farmSlot % 4;
            int posY = (int)Math.Floor(farmSlot / 4.0);
            Vector2 newFarmPivot = new Vector2(farmPivotPosition.x + posX, farmPivotPosition.y);
            Vector2 position = new Vector2(farmPivotPosition.x + posX, farmPivotPosition.y - (posY - 2));
            float distancePos2Pivot = (newFarmPivot - position).magnitude;
            float distancePivot2Center = (centerPosition - newFarmPivot).magnitude;
            float distanceCenter2House = (housePosition - centerPosition).magnitude;
            float sumDistances = distancePos2Pivot + distancePivot2Center + distanceCenter2House;
            float per1 = distancePos2Pivot / sumDistances;
            float per2 = distancePivot2Center / sumDistances;
            float per3 = distanceCenter2House / sumDistances;
            if (per1 > 0.001)
            {
                pathsToTake.Add(new PathSection(simulationTime, simulationTime + per1 * timeToTransfer, position, newFarmPivot, (newFarmPivot - position).y > 0 ? "up" : "down"));
            }
            pathsToTake.Add(new PathSection(simulationTime + per1 * timeToTransfer, simulationTime + ((per1 + per2) * timeToTransfer), newFarmPivot, centerPosition, "right"));
            pathsToTake.Add(new PathSection(simulationTime + ((per1 + per2) * timeToTransfer), simulationTime + ((per1 + per2 + per3) * timeToTransfer), centerPosition, housePosition, "up"));
        }
        if(currentPosition == "work")
        {
            float distanceWork2C = (centerPosition - workPosition).magnitude;
            float distanceC2House = (housePosition - centerPosition).magnitude;
            float sumDistances = distanceWork2C + distanceC2House;
            float per1 = distanceWork2C / sumDistances;
            float per2 = distanceC2House / sumDistances;
            pathsToTake.Add(new PathSection(simulationTime , simulationTime + (per1 * timeToTransfer), workPosition, centerPosition, "left"));
            pathsToTake.Add(new PathSection(simulationTime + (per1 * timeToTransfer), simulationTime + ((per1 + per2) * timeToTransfer), centerPosition, housePosition, "up"));

        }
        if (currentPosition == "center")
        {
            pathsToTake.Add(new PathSection(simulationTime, simulationTime + timeToTransfer, centerPosition, housePosition, "up"));
        }

    }

    public void goToFarm(int plantPos)
    {
        Debug.Log("agFarm");
        if (currentPosition == "house")
        {
            int posX = plantPos % 4;
            int posY = (int)Math.Floor(plantPos / 4.0);
            Vector2 newFarmPivot = new Vector2(farmPivotPosition.x + posX, farmPivotPosition.y);
            Vector2 position = new Vector2(farmPivotPosition.x + posX, farmPivotPosition.y - (posY - 2));
            float distancePos2Pivot = (newFarmPivot - position).magnitude;
            float distancePivot2Center = (centerPosition - newFarmPivot).magnitude;
            float distanceCenter2House = (housePosition - centerPosition).magnitude;
            float sumDistances = distancePos2Pivot + distancePivot2Center + distanceCenter2House;
            float per1 = distanceCenter2House / sumDistances;
            float per2 = distancePivot2Center / sumDistances;
            float per3 = distancePos2Pivot / sumDistances;


            pathsToTake.Add(new PathSection(simulationTime, simulationTime + per1 * timeToTransfer, housePosition, centerPosition, "down"));
            pathsToTake.Add(new PathSection(simulationTime + per1 * timeToTransfer, simulationTime + ((per1 + per2) * timeToTransfer), centerPosition, newFarmPivot, "left"));
            if (per3 > 0.001)
            {
                pathsToTake.Add(new PathSection(simulationTime + ((per1 + per2) * timeToTransfer), simulationTime + ((per1 + per2 + per3) * timeToTransfer), newFarmPivot, position, (position - newFarmPivot).y > 0 ? "up" : "down"));
            }
            
        }

        if (currentPosition == "farm")
        {
            int posXnew = plantPos % 4;
            int posYnew = (int)Math.Floor(plantPos / 4.0);
            gameObject.transform.position = new Vector2(farmPivotPosition.x + posXnew, farmPivotPosition.y - (posYnew - 2));

        }
        if (currentPosition == "work")
        {
            int posX = plantPos % 4;
            int posY = (int)Math.Floor(plantPos / 4.0);
            Vector2 newFarmPivot = new Vector2(farmPivotPosition.x + posX, farmPivotPosition.y);
            Vector2 position = new Vector2(farmPivotPosition.x + posX, farmPivotPosition.y - (posY - 2));
            float distancePos2Pivot = (newFarmPivot - position).magnitude;
            float distancePivot2Center = (centerPosition - newFarmPivot).magnitude;
            float distanceCenter2House = (workPosition - centerPosition).magnitude;
            float sumDistances = distancePos2Pivot + distancePivot2Center + distanceCenter2House;
            float per1 = distanceCenter2House / sumDistances;
            float per2 = distancePivot2Center / sumDistances;
            float per3 = distancePos2Pivot / sumDistances;


            pathsToTake.Add(new PathSection(simulationTime, simulationTime + per1 * timeToTransfer, workPosition, centerPosition, "left"));
            pathsToTake.Add(new PathSection(simulationTime + per1 * timeToTransfer, simulationTime + ((per1 + per2) * timeToTransfer), centerPosition, newFarmPivot, "left"));
            if (per3 > 0.001)
            {
                pathsToTake.Add(new PathSection(simulationTime + ((per1 + per2) * timeToTransfer), simulationTime + ((per1 + per2 + per3) * timeToTransfer), newFarmPivot, position, (position - newFarmPivot).y > 0 ? "up" : "down"));
            }

        }
        if (currentPosition == "center")
        {
            int posX = plantPos % 4;
            int posY = (int)Math.Floor(plantPos / 4.0);
            Vector2 newFarmPivot = new Vector2(farmPivotPosition.x + posX, farmPivotPosition.y);
            Vector2 position = new Vector2(farmPivotPosition.x + posX, farmPivotPosition.y - (posY - 2));
            float distancePos2Pivot = (newFarmPivot - position).magnitude;
            float distancePivot2Center = (centerPosition - newFarmPivot).magnitude;
            float sumDistances = distancePos2Pivot + distancePivot2Center;
            float per1 = distancePivot2Center / sumDistances;
            pathsToTake.Add(new PathSection(simulationTime, simulationTime + per1 * timeToTransfer, centerPosition, newFarmPivot, "left"));
            pathsToTake.Add(new PathSection(simulationTime + per1, simulationTime + timeToTransfer, newFarmPivot, position, (position - newFarmPivot).y > 0 ? "up" : "down"));
        }
    }

    public void goToWork()
    {
        Debug.Log("agWork");
        if (currentPosition == "house")
        {
            float distanceWork2C = (centerPosition - workPosition).magnitude;
            float distanceC2House = (housePosition - centerPosition).magnitude;
            float sumDistances = distanceWork2C + distanceC2House;
            float per1 = distanceC2House / sumDistances;
            float per2 = distanceWork2C / sumDistances;
            pathsToTake.Add(new PathSection(simulationTime, simulationTime + (per1 * timeToTransfer), housePosition, centerPosition, "down"));
            pathsToTake.Add(new PathSection(simulationTime + (per1 * timeToTransfer), simulationTime + ((per1 + per2) * timeToTransfer), centerPosition, workPosition, "right"));
        }

        if (currentPosition == "farm")
        {
            int posX = farmSlot % 4;
            int posY = (int)Math.Floor(farmSlot / 4.0);
            Vector2 newFarmPivot = new Vector2(farmPivotPosition.x + posX, farmPivotPosition.y);
            Vector2 position = new Vector2(farmPivotPosition.x + posX, farmPivotPosition.y - (posY - 2));
            float distancePos2Pivot = (newFarmPivot - position).magnitude;
            float distancePivot2Center = (centerPosition - newFarmPivot).magnitude;
            float distanceCenter2House = (workPosition - centerPosition).magnitude;
            float sumDistances = distancePos2Pivot + distancePivot2Center + distanceCenter2House;
            float per1 = distancePos2Pivot / sumDistances;
            float per2 = distancePivot2Center / sumDistances;
            float per3 = distanceCenter2House / sumDistances;
            if (per1 > 0.001)
            {
                pathsToTake.Add(new PathSection(simulationTime, simulationTime + per1 * timeToTransfer, position, newFarmPivot, (newFarmPivot - position).y > 0 ? "up" : "down"));
            }
            pathsToTake.Add(new PathSection(simulationTime + per1 * timeToTransfer, simulationTime + ((per1 + per2) * timeToTransfer), newFarmPivot, centerPosition, "right"));
            pathsToTake.Add(new PathSection(simulationTime + ((per1 + per2) * timeToTransfer), simulationTime + ((per1 + per2 + per3) * timeToTransfer), centerPosition, workPosition, "right"));

        }
        if (currentPosition == "work")
        {
            return;
        }
        if (currentPosition == "center")
        {
            pathsToTake.Add(new PathSection(simulationTime, simulationTime + timeToTransfer, centerPosition, workPosition, "right"));
        }
    }

    private double distanceHouseCenter()
    {
        return 4;
    }

    private double distanceWorkCenter()
    {
        Vector2 diffs = centerPosition - workPosition;
        return diffs.magnitude;
    }

    private double distanceFarmCenter(int plantID)
    {
        int posX = plantID % 4;
        Vector2 newFarmPivot = new Vector2(farmPivotPosition.x + posX, farmPivotPosition.y);
        return (newFarmPivot - centerPosition).magnitude;
    }

    private double distancePlantFarm(int plantID)
    {
        int posX = plantID % 4;
        int posY = (int)Math.Floor(plantID / 4.0);
        Vector2 newFarmPivot = new Vector2(farmPivotPosition.x + posX, farmPivotPosition.y);
        Vector2 position = new Vector2(farmPivotPosition.x + posX, farmPivotPosition.y - (posY - 2));
        return (position - newFarmPivot).magnitude;
    }

    private void playAnimation(string direction)
    {
        return;
        //switch (direction)
        //{
        //    case "up":
        //        switch (gameObject.name)
        //        {
        //            case "Agent0":
        //                gameObject.transform.localScale = new Vector3(1, 1, 1);
        //                gameObject.GetComponent<Animation>().Play("hero0up");
        //                break;
        //            case "Agent1":
        //                gameObject.transform.localScale = new Vector3(1, 1, 1);
        //                gameObject.GetComponent<Animation>().Play("hero1Up");
        //                break;
        //            case "Agent2":
        //                gameObject.transform.localScale = new Vector3(1, 1, 1);
        //                gameObject.GetComponent<Animation>().Play("hero2Up");
        //                break;
        //            case "Agent3":
        //                gameObject.transform.localScale = new Vector3(1, 1, 1);
        //                gameObject.GetComponent<Animation>().Play("hero3Up");
        //                break;
        //        }
        //        break;
        //    case "down":
        //        switch (gameObject.name)
        //        {
        //            case "Agent0":
        //                gameObject.transform.localScale = new Vector3(1, 1, 1);
        //                gameObject.GetComponent<Animation>().Play("hero0Down");
        //                break;
        //            case "Agent1":
        //                gameObject.transform.localScale = new Vector3(1, 1, 1);
        //                gameObject.GetComponent<Animation>().Play("hero1Down");
        //                break;
        //            case "Agent2":
        //                gameObject.transform.localScale = new Vector3(1, 1, 1);
        //                gameObject.GetComponent<Animation>().Play("hero2Down");
        //                break;
        //            case "Agent3":
        //                gameObject.transform.localScale = new Vector3(1, 1, 1);
        //                gameObject.GetComponent<Animation>().Play("hero3Down");
        //                break;
        //        }
        //        break;
        //    case "left":
        //        switch (gameObject.name)
        //        {
        //            case "Agent0":
        //                gameObject.transform.localScale = new Vector3(1, 1, 1);
        //                gameObject.GetComponent<Animation>().Play("hero0Side");
        //                break;
        //            case "Agent1":
        //                gameObject.transform.localScale = new Vector3(1, 1, 1);
        //                gameObject.GetComponent<Animation>().Play("hero1Side");
        //                break;
        //            case "Agent2":
        //                gameObject.transform.localScale = new Vector3(1, 1, 1);
        //                gameObject.GetComponent<Animation>().Play("hero2Side");
        //                break;
        //            case "Agent3":
        //                gameObject.transform.localScale = new Vector3(1, 1, 1);
        //                gameObject.GetComponent<Animation>().Play("hero3Side");
        //                break;
        //        }
        //        break;
        //    case "right":
        //        switch (gameObject.name)
        //        {
        //            case "Agent0":
        //                gameObject.transform.localScale = new Vector3(1, 1, -1);
        //                gameObject.GetComponent<Animation>().Play("hero0Side");
        //                break;
        //            case "Agent1":
        //                gameObject.transform.localScale = new Vector3(1, 1, -1);
        //                gameObject.GetComponent<Animation>().Play("hero1Side");
        //                break;
        //            case "Agent2":
        //                gameObject.transform.localScale = new Vector3(1, 1, -1);
        //                gameObject.GetComponent<Animation>().Play("hero2Side");
        //                break;
        //            case "Agent3":
        //                gameObject.transform.localScale = new Vector3(1, 1, -1);
        //                gameObject.GetComponent<Animation>().Play("hero3Side");
        //                break;
        //        }
        //        break;
        //}
    }

    public void updatePos(string pos)
    {
        currentPosition = pos;
    }

    public void workStart()
    {
        gameObject.GetComponent<Renderer>().enabled = false;
        lightMesh.GetComponent<Renderer>().enabled = true;
    }

    public void workFinish()
    {
        gameObject.GetComponent<Renderer>().enabled = true;
        lightMesh.GetComponent<Renderer>().enabled = false;
    }

    public void sleepStart()
    {
        gameObject.GetComponent<Renderer>().enabled = false;
        sleepMesh.GetComponent<Renderer>().enabled = true;
    }

    public void sleepFinish()
    {
        gameObject.GetComponent<Renderer>().enabled = true;
        sleepMesh.GetComponent<Renderer>().enabled = false;
    }

    public void funStart()
    {
        funMesh.GetComponent<Renderer>().enabled = true;
        gameObject.GetComponent<Renderer>().enabled = false;
    }

    public void funFinish()
    {
        funMesh.GetComponent<Renderer>().enabled = false;
        gameObject.GetComponent<Renderer>().enabled = true;
    }

    public void plantStart()
    {
        GetComponent<SpriteRenderer>().sprite = plantSprite;
    }

    public void plantFinish()
    {
        GetComponent<SpriteRenderer>().sprite = idleSprite;
    }

    public void harvestStart()
    {
        GetComponent<SpriteRenderer>().sprite = plantSprite;
    }

    public void harvestFinish()
    {
        GetComponent<SpriteRenderer>().sprite = idleSprite;
    }

    public void foodStart()
    {
        foodMesh.transform.position = (Vector2)transform.position /*+ new Vector2(0, 0.1f)*/;
        foodMesh.GetComponent<Renderer>().enabled = true;
    }

    public void foodEnd()
    {
        foodMesh.GetComponent<Renderer>().enabled = false;
    }



}
