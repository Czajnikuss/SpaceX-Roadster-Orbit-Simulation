using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader 
{
    private static AsyncOperation asyncOperation;
    private class LoadingMono : MonoBehaviour {}
   
   
   public enum SceneName
   {
       LoadScene,
       MainMenu,
       RoadsterSimulation,
       SpaceX
   }

    public static IEnumerator LoadCoorutine ( SceneName scene)
    {
        yield return null;
        asyncOperation = SceneManager.LoadSceneAsync(scene.ToString());
        while(!asyncOperation.isDone)
        {
            yield return null;
        }
    }
    public static float GetLoadingProgress()
    {
        if(asyncOperation  != null)
        {
            return asyncOperation.progress;
        }
        else
        {
            return 0f;
        }
    }




    private static Action onLoaderCallback;
   public static void Load(SceneName scene)
   {
       onLoaderCallback = () => {
           GameObject loadingGameObject = new GameObject("loading Game Object");
           loadingGameObject.AddComponent<LoadingMono>().StartCoroutine(LoadCoorutine ( scene));
           
       };

       SceneManager.LoadScene(SceneName.LoadScene.ToString());
       
   }

    public static void LoaderCallback()
    {
        if(onLoaderCallback != null)
        {
            onLoaderCallback();
            onLoaderCallback = null;
        }
    }

}
