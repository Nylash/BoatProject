using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IslandLoadEncounter : Encounter
{
#pragma warning disable 0649
    [SerializeField] string sceneName;
#pragma warning restore 0649

    AsyncOperation asyncLoad;
    GameObject rootIsland;

    public void LoadIsland(int angle, Transform boatTransform)
    {
        asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        StartCoroutine(WaitingEndOfLoading(angle, boatTransform));
    }

    IEnumerator WaitingEndOfLoading(int angle, Transform boatTransform)
    {
        while (!asyncLoad.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        rootIsland = GameObject.FindGameObjectWithTag("RootIsland");
        rootIsland.transform.eulerAngles = new Vector3(0, angle, 0);
        rootIsland.transform.position = boatTransform.position + boatTransform.forward * 10;
        rootIsland.transform.GetChild(0).transform.localEulerAngles = new Vector3(0, -angle, 0);
    }
}
