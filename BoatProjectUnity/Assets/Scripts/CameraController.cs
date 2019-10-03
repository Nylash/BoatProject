using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("STATE (no manual change)")]
    public CameraFocus currentFocus;

    [Header("CAMERA OFFSET")]
    [SerializeField] float xOffset = 5; 
    [SerializeField] float zOffset = 5;

    /*Camera preset for boat
     * 
     * 1)
     * pos x = 8 y = 10 z = -10
     * rota x = 38 y = -20 z = 0
     * 
     * 2)
     * pos x = 5 y = 10 z = -5
     * rota x = 45 y = -20 z = 0
     * 
     */

    /*Camera preset for island
     * 
     * 
     * 
     */

    Transform target;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    void Update()
    {
        switch (currentFocus)
        {
            case CameraFocus.boat:
                if(transform.position.y != 10)
                    transform.position = new Vector3(target.position.x + xOffset, 10, target.position.z - zOffset);
                if (transform.rotation.eulerAngles != new Vector3(45, -20, 0))
                    transform.eulerAngles = new Vector3(45, -20, 0);
                transform.position = new Vector3(target.position.x + xOffset, transform.position.y, target.position.z - zOffset);
                break;
            case CameraFocus.island:
                break;
            default:
                break;
        }
    }

    public enum CameraFocus
    {
        boat, island
    }
}
