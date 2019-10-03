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
#pragma warning restore 0649
    public Vector3 targetRot;

    private void Update()
    {
        Debug.DrawRay(focusObject.transform.position, new Vector3(1, 1.35f, -1) * distance, Color.red);
        if (Input.GetKeyDown(KeyCode.A))
        {
            SceneManager.LoadScene("Ben");
        }
    }

    public Vector3 GetTargetPos()
    {
        print(focusObject.transform.position + new Vector3(1, 1.35f, -1) * distance);
        return (focusObject.transform.position + new Vector3(1, 1.35f, -1) * distance);
    }
}
