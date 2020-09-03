using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class SpaceXAPIHandler : MonoBehaviour
{
    public bool isCalculated = false;
    public bool showPayloadFromMissions = false;
    private const string URLLaunches = "https://api.spacexdata.com/v3/launches?pretty=true&filter=flight_number,mission_name,mission_id,upcoming,ships,rocket/(rocket_name,second_stage/payloads/payload_id)";
    private const string URLMissions = "https://api.spacexdata.com/v3/missions?pretty=true&filter=mission_id,payload_ids";
    private const string URLRockets = "https://api.spacexdata.com/v3/rockets?pretty=true&filter=rocket_name,country";
    private const string URLShips = "https://api.spacexdata.com/v3/ships?pretty=true&filter=missions,ship_id,ship_type,home_port,image,ship_name";
[Space]
[Header ("Data containers")]
    public LaunchStructuredData[] allLaunches;
    public LaunchesData launchesData;
    public MissionsData missionsData;
    public RocketsData rocketsData;
    public ShipsData shipsData;
    private string loadedData;



    void Start()
    {

        GameObject duplicate = GameObject.FindObjectOfType<SpaceXAPIHandler>().gameObject;
        if(duplicate != this.gameObject) Destroy(duplicate);
        DontDestroyOnLoad(this.gameObject);
        //setting info for further calculations
        isCalculated = false;
        StartCoroutine(WebResponce(URLLaunches));
        
    }
    //show payload from missions_id or from payload_id 
    public void SetMissionOrPayload()
    {
        showPayloadFromMissions = !showPayloadFromMissions;
    }


    //Except for ship fotos all web comunication 
    //is done here, I've chose sequential patern 
    //becouse it is less prone to error then sending 
    //all request in the same update
    private IEnumerator WebResponce ( string URL)
    {
        UnityWebRequest req =  UnityWebRequest.Get(URL);
        
        yield return req.SendWebRequest();
        
        if(req.isNetworkError || req.isHttpError)
        {
            Debug.Log(req.error);
        }
        else 
        {
            loadedData = req.downloadHandler.text;
            Debug.Log(loadedData);
            if(URL == URLLaunches) 
            {
                DeJasonLaunches(loadedData);
                yield return StartCoroutine(WebResponce(URLMissions));
            }
            else if(URL == URLMissions) 
            {
                DeJasonMissions(loadedData);
                yield return StartCoroutine(WebResponce(URLRockets));
            }
            else if(URL == URLRockets) 
            {
                DeJasonRockets(loadedData);
                yield return StartCoroutine(WebResponce(URLShips));
            }
            else if(URL == URLShips) 
            {
                DeJasonShips(loadedData);
                yield return StartCoroutine(StructureData());
            }
        }

    }
#region "Classes to de-JSON"    
    [System.Serializable] public class LaunchesData
    {
        public LaunchData[] launcheArray;
    }
    
    [System.Serializable] public class LaunchData
    {
        public string flight_number;
        public string mission_name;
        public string[] mission_id;
        public Rocket rocket;
        public string upcoming;
        public string[] ships;
        
    }
    [System.Serializable] public class SecondStage
    {
        public PayloadLaunch[] payloads;
        
    }
    
    [System.Serializable] public class Rocket
    {
        public string rocket_name;
        public  SecondStage second_stage;
    }
    [System.Serializable] public class PayloadLaunch
    {
        public string payload_id;
    }
    [System.Serializable] public class MissionsData
    {
        public MissionData[] missionsArray;
    }
    [System.Serializable] public class MissionData
    {
        public string mission_id;
        public string[] payload_ids;
    }
    
    [System.Serializable] public class RocketsData
    {
        public RocketData[] rocketsArray;
    }
    [System.Serializable] public class RocketData
    {
        public string rocket_name;
        public string country;
    }
    [System.Serializable] public class ShipsData
    {
        public ShipData[] shipsArray;
    }
    [System.Serializable] public class ShipData
    {
        public ShipMissions[] missions;
        public string ship_id;
        public string ship_type;
        public string home_port;
        public string image;
        public string ship_name;
    }
    [System.Serializable] public class ShipMissions
    {
        public string name;
        public string flight;
    }
#endregion   
    //last class of data mangment from it I'll be able to 
    //acces all needed data fast.
    [System.Serializable] public class LaunchStructuredData
    {
        public string missionName;
        public int noOfPayload;
        public int noOfPayloadInMission;
        public string rockeName;
        public string rocketCountry;
        public bool allreadyHappend;
        public ShipsInLaunch[] shipsUsed;

    }
    [System.Serializable] public class ShipsInLaunch
    {
        public string shipId;
        public int noOfMissionsUsed;
        public string homePort;
        public string shipType;
        public string imageURL;
        public string shipName;
    }
    //becouse of structure of given data I had to wrap anwser from server in my own variable name
    private void DeJasonLaunches(string loadedData)
    {
        launchesData = JsonUtility.FromJson<LaunchesData>("{\"launcheArray\":" + loadedData + "}");;
    }

    private void DeJasonMissions(string loadedData)
    {
        missionsData = JsonUtility.FromJson<MissionsData>("{\"missionsArray\":" + loadedData + "}");;
    }
    private void DeJasonRockets(string loadedData)
    {
        rocketsData = JsonUtility.FromJson<RocketsData>("{\"rocketsArray\":" + loadedData + "}");;
    }
    private void DeJasonShips(string loadedData)
    {
        shipsData = JsonUtility.FromJson<ShipsData>("{\"shipsArray\":" + loadedData + "}");;
    }
    //Changing data structure to suit my needs may be 
    //a burden for CPU so I went with coorutine
    public IEnumerator StructureData()
    {
        bool tempUpcoming;
        int tempNoOfPayload;
        string tempRocketCountry = "";
        string[] tempMissions;
        int tempPayloadInMission;
        
        allLaunches = new LaunchStructuredData[launchesData.launcheArray.Length];
        for (int i = 0; i < allLaunches.Length; i++)
        {
            allLaunches[i] = new LaunchStructuredData();
            tempUpcoming = bool.Parse(launchesData.launcheArray[i].upcoming);
            tempNoOfPayload = launchesData.launcheArray[i].rocket.second_stage.payloads.Length;
            tempMissions = launchesData.launcheArray[i].mission_id;
            tempPayloadInMission = 0;
            if(tempMissions !=null && tempMissions.Length >0)
            {
                for (int m = 0; m < tempMissions.Length; m++)
                {
                    for (int n = 0; n < missionsData.missionsArray.Length; n++)
                    {
                        if(tempMissions[m] == missionsData.missionsArray[n].mission_id)
                        {
                            tempPayloadInMission += missionsData.missionsArray[n].payload_ids.Length;
                        }
                    }
                }
            }
            else tempPayloadInMission = 0;

            allLaunches[i].missionName = launchesData.launcheArray[i].mission_name;
            allLaunches[i].noOfPayload = tempNoOfPayload;
            allLaunches[i].noOfPayloadInMission = tempPayloadInMission;
            allLaunches[i].allreadyHappend = !tempUpcoming;
            allLaunches[i].rockeName = launchesData.launcheArray[i].rocket.rocket_name;
            foreach (var item in rocketsData.rocketsArray)
            {
                if(allLaunches[i].rockeName == item.rocket_name)
                {
                    tempRocketCountry = item.country;
                    goto gotRocketCountry;
                }
            }
            gotRocketCountry:;
            allLaunches[i].rocketCountry = tempRocketCountry;
            allLaunches[i].shipsUsed = new ShipsInLaunch[launchesData.launcheArray[i].ships.Length];
            for (int j = 0; j < allLaunches[i].shipsUsed.Length; j++)
            {
                allLaunches[i].shipsUsed[j] = new ShipsInLaunch();
                allLaunches[i].shipsUsed[j].shipId = launchesData.launcheArray[i].ships[j];
                foreach (var item in shipsData.shipsArray)
                {
                    if(item.ship_id == allLaunches[i].shipsUsed[j].shipId)
                    {
                        allLaunches[i].shipsUsed[j].shipName = item.ship_name;
                        allLaunches[i].shipsUsed[j].noOfMissionsUsed = item.missions.Length;
                        allLaunches[i].shipsUsed[j].homePort = item.home_port;
                        allLaunches[i].shipsUsed[j].shipType = item.ship_type;
                        allLaunches[i].shipsUsed[j].imageURL = item.image;
                        goto gotThisShip;
                    }
                }
                gotThisShip:;
            }

        }
        isCalculated = true;
        yield return null;
    }

}


