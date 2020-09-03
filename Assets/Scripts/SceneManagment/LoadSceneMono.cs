using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneMono : MonoBehaviour
{
   public void LoadRoadster()
   {
       SceneLoader.Load(SceneLoader.SceneName.RoadsterSimulation);
   }
   public void LoadSpaceX()
   {
       
       SceneLoader.Load(SceneLoader.SceneName.SpaceX);
   }
   public void LoadMenu()
   {
       SceneLoader.Load(SceneLoader.SceneName.MainMenu);
   }
   public void QuitApp()
   {
       Application.Quit();
   }
   
}
