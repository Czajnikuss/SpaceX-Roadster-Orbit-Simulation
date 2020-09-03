using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System;
using TMPro;




public class RoadsterSim : MonoBehaviour
{
    public float distanceFactor = 1000f;
    public List<DatePositionRaw> AllRawData;
    public List<Vector3Double> AllPositions;
    public Queue<GameObject> tailQueue;
    public Material tailMaterial;
    public int currentLocation;
    public GameObject roadster;
    public GameObject sol;
    [Space]
    [Header("UI Texts")]
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI semimajorAxisText;
    public TextMeshProUGUI eccentricityText;
    public TextMeshProUGUI inclinationText;
    public TextMeshProUGUI longitudeOfAscendingNodeText;
    public TextMeshProUGUI periapsisArgumentText;
    public TextMeshProUGUI trueAnomalyText;
    
    
    private void Start()
    {
        //Initializing variables
        AllRawData = new List<DatePositionRaw>();
        AllPositions = new List<Vector3Double>();
        tailQueue = new Queue<GameObject>(21);
        
        ReadRoadsterData();

        CreatePositionsList();
        
        TimeTickSystem.OnTick += MoveRoadster;

    }
    public void SpeedChange(int speed)
    {
       TimeTickSystem.OnTick -= MoveRoadster;
       TimeTickSystem.OnTick_2 -= MoveRoadster;
       TimeTickSystem.OnTick_10 -= MoveRoadster;
       if(speed == 1) TimeTickSystem.OnTick_10 += MoveRoadster;
       else if(speed == 5) TimeTickSystem.OnTick_2 += MoveRoadster;
       else if(speed == 10) TimeTickSystem.OnTick += MoveRoadster;

    }
    private void ReadRoadsterData()
    {

        //Loading Source Data
        TextAsset loadedAsset = (TextAsset)Resources.Load("RoadsterData", typeof(TextAsset));
        string roadsterData = loadedAsset.text;
        
        //Loaded correctly
        if(roadsterData != "")
        {
        //Lines - data sepatarion to sets
        string[] loadedData = roadsterData.Split( '\n' );
        //got data sets, now to separate them to various paramenters
        //and save structs with date for future calculations
        for (int i = 1; i < loadedData.Length; i++)
            {
                if(loadedData[i] != "")
                {
                    string[] singleEntryString = loadedData[i].Split(',');
                    DatePositionRaw singleEntryStruct = new DatePositionRaw();
                
                    singleEntryStruct.date = DateTime.Parse(singleEntryString[1]);
                    //pecify that given time is UTC, for future Local conversion
                    DateTime.SpecifyKind(singleEntryStruct.date, DateTimeKind.Utc);
                    
                    singleEntryStruct.semimajorAxis = Double.Parse(singleEntryString[2], System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                    singleEntryStruct.eccentricity = Double.Parse(singleEntryString[3], System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                    singleEntryStruct.inclination = Double.Parse(singleEntryString[4], System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                    singleEntryStruct.longitudeOfAscendingNode = Double.Parse(singleEntryString[5], System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                    singleEntryStruct.periapsisArgument = Double.Parse(singleEntryString[6], System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                    singleEntryStruct.trueAnomaly = Double.Parse(singleEntryString[8], System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));

                    AllRawData.Add(singleEntryStruct);
                }
            }
        }
        else 
        {
          Debug.Log("Empty");
         
        }


    }

    private void CreatePositionsList()
    {
        for (int i = 0; i < AllRawData.Count; i++)
        {
            Vector3Double tempVector = Calc.CalculateOrbitalPosition(AllRawData[i].semimajorAxis, AllRawData[i].eccentricity, AllRawData[i].inclination, AllRawData[i].longitudeOfAscendingNode, AllRawData[i].periapsisArgument, AllRawData[i].trueAnomaly);
            AllPositions.Add(tempVector);
        }
    }
//for debug 
    private void CreatePoints()
    {
        for (int i = 0; i < AllPositions.Count; i++)
        {

            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = new Vector3((float)AllPositions[i].x/distanceFactor, (float)AllPositions[i].y/distanceFactor, (float)AllPositions[i].z/distanceFactor);
        }
    }

    public void MoveRoadster(System.Object sender, EventArgs e)
    {
        if(roadster != null)
        {
            roadster.transform.position = new Vector3((float)AllPositions[currentLocation].x/distanceFactor, (float)AllPositions[currentLocation].y/distanceFactor, (float)AllPositions[currentLocation].z/distanceFactor);
            roadster.transform.root.LookAt(sol.transform);
            UpdateText(currentLocation);
            DrawTail(currentLocation);
            if(AllRawData[currentLocation].date >= DateTime.Parse("2019-10-08 00:00:00"))
            {
                currentLocation = 0;
            }
            else
            {
                currentLocation++;
                
                if(currentLocation >= AllPositions.Count) currentLocation=0;
            }
        }
    }
    public void UpdateText(int index)
    {
        DateTime tempTime = AllRawData[index].date.ToLocalTime();
        dateText.text = "Date: " + tempTime.ToString();
        semimajorAxisText.text = "Semi Major Axies: " + AllRawData[index].semimajorAxis.ToString(); 
        eccentricityText.text = "Eccentricity: " + AllRawData[index].eccentricity.ToString();
        inclinationText.text = "Inclination: " + AllRawData[index].inclination.ToString();
        longitudeOfAscendingNodeText.text = "Longitude of node: " + AllRawData[index].longitudeOfAscendingNode.ToString();
        periapsisArgumentText.text = "Pariapsis Argument: " + AllRawData[index].periapsisArgument.ToString();
        trueAnomalyText.text = "Anomaly: " + AllRawData[index].trueAnomaly.ToString();
    
    }
   
    private void DrawTail(int index)
    {
        if(index >0)
        {
            
            GameObject tempTailElement = DrawLine(new Vector3((float)AllPositions[index-1].x/distanceFactor, (float)AllPositions[index-1].y/distanceFactor, (float)AllPositions[index-1].z/distanceFactor), roadster.transform.position, 10f);
            tailQueue.Enqueue(tempTailElement);
            if(tailQueue.Count >= 20)
            {
                Destroy(tailQueue.Dequeue());
            }
        }
    }
    private GameObject DrawLine(Vector3 start, Vector3 end, float width)
         {
             GameObject myLine = new GameObject();
             myLine.transform.position = start;
             myLine.AddComponent<LineRenderer>();
             LineRenderer lr = myLine.GetComponent<LineRenderer>();
             lr.material = tailMaterial;
             lr.startColor = Color.white;
             lr.endColor = Color.white;
             lr.startWidth = width;

             lr.SetPosition(0, start);
             lr.SetPosition(1, end);
             return myLine;
         }
}
