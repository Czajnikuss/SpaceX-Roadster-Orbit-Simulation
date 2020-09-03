using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ShipInfoHandler : MonoBehaviour, IPooledObject
{
    [Header("Foto related")]
    public string fotoURL;
    public bool isLoaded;
    
    [SerializeField] private RawImage shipFoto;
    [SerializeField] private GameObject shipFotoPanel;
    [Space]
    [Header("Data related")]
    public TextMeshProUGUI shipNameText;
    public TextMeshProUGUI numberOfMisionsText;
    public TextMeshProUGUI homePortText;
    public TextMeshProUGUI shipTypeText;
    


    public void OnObjectPooled()
    {
        //Initailizing variables and ensuring panel off
        shipFotoPanel = UIHandler.Instance.shipFotoPanel;
        shipFoto = UIHandler.Instance.shipFoto;
        shipFotoPanel.SetActive(false);
    }
    //called from button
    public void ShowShipFoto()
    {
        shipFotoPanel.SetActive(true);
            StartCoroutine(DownloadImage(fotoURL));
    }
    IEnumerator DownloadImage(string MediaUrl)
    {   
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if(request.isNetworkError || request.isHttpError) 
            Debug.Log(request.error);
        else
            shipFoto.texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
    } 
}

