using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoatController : MonoBehaviour
{
    [Header("STATE (no manual change)")]
    public BoatState currentState;
    public Encounter.EncounterType currentEncounter;

    [Header("PREFABS")]
#pragma warning disable 0649
    [SerializeField] GameObject pathPrefab;
#pragma warning restore 0649

    [Header("SPEED PARAMETERS")]
    public float speed;
    public float rotationSpeed;

    //Heading variables
    [HideInInspector]
    public int headingAngle = 0;
    [HideInInspector]
    bool headingChoosen;
    bool pathSpawn;

    Rigidbody rigid;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        switch (currentState)
        {
            //Game is loading, boat is advancing forward but can't be controlled
            case BoatState.loading:
                rigid.AddForce(transform.forward * speed);
                if (transform.rotation.y != 0)
                    transform.rotation = Quaternion.identity;
                break;
            //Boat is advancing forward and can be controlled
            case BoatState.gameplay:
                //Add limit to rotation depending of current heading
                if (Input.GetButton("Right"))
                {
                        transform.Rotate(0, rotationSpeed, 0);
                }   
                if (Input.GetButton("Left"))
                {
                        transform.Rotate(0, -rotationSpeed, 0);
                }
                rigid.AddForce(transform.forward * speed);
                break;
            //Player reach an encounter
            case BoatState.encounter:
                switch (currentEncounter)
                {
                    case Encounter.EncounterType.none:
                        break;
                    case Encounter.EncounterType.heading:
                        rigid.AddForce(transform.forward * speed);
                        if (headingChoosen)
                        {
                            transform.eulerAngles = new Vector3(
                                                    transform.eulerAngles.x,
                                                    Mathf.LerpAngle(transform.eulerAngles.y, headingAngle, Time.deltaTime),
                                                    transform.eulerAngles.z);
                            if (!pathSpawn)
                                StartCoroutine(SpawnIslandPath());
                        }
                        else
                        {
                            transform.eulerAngles = new Vector3(
                                                    transform.eulerAngles.x,
                                                    Mathf.LerpAngle(transform.eulerAngles.y, 0, Time.deltaTime),
                                                    transform.eulerAngles.z);
                        }
                        break;
                    case Encounter.EncounterType.islandLoad:
                        rigid.AddForce(transform.forward * speed);
                        transform.eulerAngles = new Vector3(
                                                    transform.eulerAngles.x,
                                                    Mathf.LerpAngle(transform.eulerAngles.y, headingAngle, Time.deltaTime),
                                                    transform.eulerAngles.z);
                        break;
                    case Encounter.EncounterType.island:
                        break;
                    case Encounter.EncounterType.merchant:
                        break;
                    case Encounter.EncounterType.wreck:
                        break;
                    case Encounter.EncounterType.pirates:
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }

        //DEBUG
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Encounter>())
        {
            currentState = BoatState.encounter;
            switch (other.GetComponent<Encounter>().encounterType)
            {
                case Encounter.EncounterType.none:
                    Debug.LogError("Please configure encounter type !");
                    break;
                case Encounter.EncounterType.heading:
                    currentEncounter = Encounter.EncounterType.heading;
                    other.GetComponent<HeadingEncounter>().ChooseHeading(this);
                    break;
                case Encounter.EncounterType.islandLoad:
                    currentEncounter = Encounter.EncounterType.islandLoad;
                    other.GetComponent<IslandLoadEncounter>().LoadIsland(-30, transform);
                    break;
                case Encounter.EncounterType.island:
                    currentEncounter = Encounter.EncounterType.island;
                    break;
                case Encounter.EncounterType.merchant:
                    break;
                case Encounter.EncounterType.wreck:
                    break;
                case Encounter.EncounterType.pirates:
                    break;
                default:
                    break;
            }
        }
        if (other.CompareTag("StartPath"))
        {
            currentState = BoatState.gameplay;
            currentEncounter = Encounter.EncounterType.none;
            headingChoosen = false;
            pathSpawn = false;
        }
    }

    IEnumerator SpawnIslandPath()
    {
        pathSpawn = true;
        yield return new WaitForSeconds(2);
        Instantiate<GameObject>(pathPrefab, transform.position + transform.forward * 10, Quaternion.Euler(new Vector3(0, headingAngle, 0)));
    }

    public void ChangeBoatHeading(int angle)
    {
        headingChoosen = true;
        headingAngle = angle;
    }

    //ENUM

    public enum BoatState
    {
        loading, gameplay, encounter
    }
}
