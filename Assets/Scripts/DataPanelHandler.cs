using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;



public class DataPanelHandler : MonoBehaviour
{
    private SpaceXAPIHandler spaceXAPIHandler;
    private SpaceXAPIHandler.LaunchStructuredData[] dataSource;
    [SerializeField] private TextMeshProUGUI missionNameText;
    [SerializeField] private Image allreadyLaunched;
    [SerializeField] private TextMeshProUGUI numberOfPayloads;
    [SerializeField] private TextMeshProUGUI rocketName;
    [SerializeField] private TextMeshProUGUI country;
    private int index;
    [SerializeField] private int offset;
    
    [SerializeField] private Sprite[] launchedOrNot;
    [SerializeField] private GameObject shipsPanel;
    [SerializeField] private Transform shipPanelTransform;
    [SerializeField] private GameObject shipsListPanel;
    

    private void Awake() 
    {
        //Initializing variables

        spaceXAPIHandler = GameObject.FindObjectOfType<SpaceXAPIHandler>();
        shipsPanel.SetActive(false);
        
        StartCoroutine(FristUpdate());
    }
    
    private IEnumerator FristUpdate()
    {
        while(!spaceXAPIHandler.isCalculated) yield return null;
        dataSource = spaceXAPIHandler.allLaunches;
    }

//updataing data in this record
    public void UpdateData(int newIndex)
    {
        index = newIndex + offset;
        missionNameText.text = dataSource[index].missionName;
        allreadyLaunched.sprite = launchedOrNot[dataSource[index].allreadyHappend? 0:1];
        if(spaceXAPIHandler.showPayloadFromMissions)
        {
            numberOfPayloads.text = dataSource[index].noOfPayloadInMission.ToString();
        }
        else
        {
            numberOfPayloads.text = dataSource[index].noOfPayload.ToString();
        }
        rocketName.text = dataSource[index].rockeName;
        country.text = dataSource[index].rocketCountry;

    }
    public void OnClick()
    {
        shipsPanel.SetActive(true);
        //there are some ships from prevoius launch
        if(UIHandler.Instance.activeShips.Count >0)
        {
            //destroy them all...
            foreach (var item in UIHandler.Instance.activeShips)
            {
                item.SetActive(false);
            }
            //and claea list for future population
            UIHandler.Instance.activeShips.Clear();
        }
        shipsListPanel.SetActive(false);
        //populate panel with pooled panels
        if(dataSource[index].shipsUsed.Length > 0)
        {
            shipsListPanel.SetActive(true);
            foreach (var item in dataSource[index].shipsUsed)
            {
                GameObject tempShip = ObjectsPooler.Instance.SpawnFromDictionary("Ship",shipPanelTransform);
                ShipInfoHandler tempShipInfoHandler = tempShip.GetComponent<ShipInfoHandler>();
                tempShipInfoHandler.fotoURL = item.imageURL;
                tempShipInfoHandler.shipNameText.text = item.shipName;
                tempShipInfoHandler.shipTypeText.text = item.shipType;
                tempShipInfoHandler.numberOfMisionsText.text = item.noOfMissionsUsed.ToString();
                tempShipInfoHandler.homePortText.text = item.homePort;
                tempShipInfoHandler.isLoaded = true;

                UIHandler.Instance.activeShips.Add(tempShip);
            }
        }


    }
    public int GetCurrentIndex()
    {
      return index;
    }
}
