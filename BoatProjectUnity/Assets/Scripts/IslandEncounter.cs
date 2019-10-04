using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IslandEncounter : Encounter
{
    [Header("CAMERA TARGET POS/ROT")]
#pragma warning disable 0649
    [SerializeField] GameObject focusObject;
    [SerializeField] float distance = 100;
    [SerializeField] public GameObject dockingPos;
    [SerializeField] public GameObject exitPos;
#pragma warning restore 0649
    public Vector3 targetRot;

    BoatController boatControllerScript;

    private void Update()
    {
        Debug.DrawRay(focusObject.transform.position, new Vector3(1, 1.35f, -1) * distance, Color.red);
        if (Input.GetKeyDown(KeyCode.A))
        {
            boatControllerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<BoatController>();
            boatControllerScript.encounterDone = true;
            boatControllerScript.EndIslandEncounter();
        }
    }

    public Vector3 GetTargetPos()
    {
        return (focusObject.transform.position + new Vector3(1, 1.35f, -1) * distance);
    }
}
