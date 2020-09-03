using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadeCallback : MonoBehaviour
{
   private bool isFirstUpdate = true;
   public Image loadingBar;
   

    private void Start() {
        isFirstUpdate = true;
    }
   private void Update() 
   {
        loadingBar.fillAmount = SceneLoader.GetLoadingProgress();
        
        if(isFirstUpdate)
        {
            isFirstUpdate = false;
            StartCoroutine(DelaySceneLoad(0.5f));
        }    
   }
   private IEnumerator DelaySceneLoad ( float amount)
   {
       
       yield return new WaitForSeconds(amount);
       SceneLoader.LoaderCallback();
   }
}
