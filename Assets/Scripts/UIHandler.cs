using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIHandler : MonoBehaviour
{
    private DataPanelHandler[] allDataPanelHandlers;
    private Scrollbar scrollbar;
    private SpaceXAPIHandler spaceXAPIHandler;
    public GameObject shipFotoPanel;
    public RawImage shipFoto;

    public List<GameObject> activeShips;
#region SINGLETON PATTERN
 public static UIHandler _instance;
 public static UIHandler Instance
 {
     get {
         if (_instance == null)
         {
             _instance = GameObject.FindObjectOfType<UIHandler>();
             
             if (_instance == null)
             {
                 GameObject container = new GameObject("UIHandler");
                 _instance = container.AddComponent<UIHandler>();
             }
         }
     
         return _instance;
     }
 }
 #endregion    
     
    
    void Start()
    {
        //initializing variables
        allDataPanelHandlers = GameObject.FindObjectsOfType<DataPanelHandler>();
        scrollbar = GameObject.FindObjectOfType<Scrollbar>();
        spaceXAPIHandler = GameObject.FindObjectOfType<SpaceXAPIHandler>();
        activeShips = new List<GameObject>();
        
        //ensuring that data will show right after calculation
        StartCoroutine(FristUpdate());
    }
    private IEnumerator FristUpdate()
    {
        while(!spaceXAPIHandler.isCalculated) yield return null;
        UpdateAll(0);
    }
    //updatind data in all panels
    public void UpdateAll(int newIndex)
    {
        foreach (var item in allDataPanelHandlers)
        {
            item.UpdateData(newIndex);
        }
    }
    
    //Scrolling from scrollbar
    public void OnScrollbarChange()
    {       
        UpdateAll((int)(scrollbar.value * (float)(spaceXAPIHandler.allLaunches.Length - 5)) );
    }
    
}
