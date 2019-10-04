using System.Collections;
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

    [Header("SPEED PARAMETERS")]
    [SerializeField] float speed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float speedToIsland;

    [Header("HEADING OBJECTS")]
    [SerializeField] GameObject leftHeading;
    [SerializeField] GameObject middleHeading;
    [SerializeField] GameObject rightHeading;
#pragma warning restore 0649

    //Heading variables
    [HideInInspector] public int headingAngle = 0;
    [HideInInspector] bool headingChoosen;
    bool pathSpawn;
    GameObject selectedHeading;
    SpriteRenderer selectedHeadingColor;

    //Encounter variables
    IslandEncounter islandEncounterScript;
    Vector3 dockingPos;
    Vector3 exitPos;
    Vector3 startPosLerp;
    float lerpTimer;
    [HideInInspector] public bool encounterDone;
    [HideInInspector] public float distanceLerpDock;
    [HideInInspector] public float distanceLerpExit;
    bool spawnIsland;

    Rigidbody rigid;
    CameraController camController;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        camController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
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
                            if (Input.GetButtonDown("Left"))
                            {
                                selectedHeadingColor.color = Color.white;
                                switch (selectedHeading.name)
                                {
                                    case "LeftHeading":
                                        selectedHeading = rightHeading;
                                        break;
                                    case "MiddleHeading":
                                        selectedHeading = leftHeading;
                                        break;
                                    case "RightHeading":
                                        selectedHeading = middleHeading;
                                        break;
                                    default:
                                        break;
                                }
                                selectedHeadingColor = selectedHeading.GetComponent<SpriteRenderer>();
                                selectedHeadingColor.color = Color.cyan;
                            }
                            if (Input.GetButtonDown("Right"))
                            {
                                selectedHeadingColor.color = Color.white;
                                switch (selectedHeading.name)
                                {
                                    case "LeftHeading":
                                        selectedHeading = middleHeading;
                                        break;
                                    case "MiddleHeading":
                                        selectedHeading = rightHeading;
                                        break;
                                    case "RightHeading":
                                        selectedHeading = leftHeading;
                                        break;
                                    default:
                                        break;
                                }
                                selectedHeadingColor = selectedHeading.GetComponent<SpriteRenderer>();
                                selectedHeadingColor.color = Color.cyan;
                            }
                            if (Input.GetButtonDown("Validate"))
                            {
                                switch (selectedHeading.name)
                                {
                                    case "LeftHeading":
                                        headingAngle = -30;
                                        break;
                                    case "MiddleHeading":
                                        headingAngle = 0;
                                        break;
                                    case "RightHeading":
                                        headingAngle = 30;
                                        break;
                                    default:
                                        break;
                                }
                                headingChoosen = true;
                                leftHeading.SetActive(false);
                                middleHeading.SetActive(false);
                                rightHeading.SetActive(false);
                            }
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
                        if (!encounterDone)
                        {
                            lerpTimer += Time.deltaTime * speedToIsland / distanceLerpDock;
                            transform.position = Vector3.Lerp(startPosLerp, dockingPos, lerpTimer);
                        }
                        else
                        {
                            lerpTimer += Time.deltaTime * speedToIsland / distanceLerpDock;
                            transform.position = Vector3.Lerp(startPosLerp, exitPos, lerpTimer);
                        }
                        
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
            encounterDone = false;
            switch (other.GetComponent<Encounter>().encounterType)
            {
                case Encounter.EncounterType.none:
                    Debug.LogError("Please configure encounter type !");
                    break;
                case Encounter.EncounterType.heading:
                    currentEncounter = Encounter.EncounterType.heading;
                    selectedHeading = middleHeading;
                    selectedHeadingColor = selectedHeading.GetComponent<SpriteRenderer>();
                    leftHeading.SetActive(true);
                    middleHeading.SetActive(true);
                    rightHeading.SetActive(true);
                    selectedHeadingColor.color = Color.cyan;
                    break;
                case Encounter.EncounterType.islandLoad:
                    if (!spawnIsland)
                    {
                        currentEncounter = Encounter.EncounterType.islandLoad;
                        other.GetComponent<IslandLoadEncounter>().LoadIsland(headingAngle, transform);
                        spawnIsland = true;
                    }
                    break;
                case Encounter.EncounterType.island:
                    spawnIsland = false;
                    currentEncounter = Encounter.EncounterType.island;
                    islandEncounterScript = other.GetComponent<IslandEncounter>();
                    camController.FocusIsland(islandEncounterScript.GetTargetPos(), islandEncounterScript.targetRot);
                    dockingPos = islandEncounterScript.dockingPos.transform.position;
                    exitPos = islandEncounterScript.exitPos.transform.position;
                    distanceLerpDock = Vector3.Distance(transform.position, dockingPos);
                    distanceLerpExit = Vector3.Distance(transform.position, exitPos);
                    startPosLerp = transform.position;
                    lerpTimer = 0;
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
        switch (other.tag)
        {
            case "StartPath":
                currentState = BoatState.gameplay;
                currentEncounter = Encounter.EncounterType.none;
                headingChoosen = false;
                pathSpawn = false;
                break;
            case "Exit":
                SceneManager.LoadScene("Ben");
                break;
            default:
                break;
        }
    }

    public void EndIslandEncounter()
    {
        startPosLerp = transform.position;
        lerpTimer = 0;
    }

    IEnumerator SpawnIslandPath()
    {
        pathSpawn = true;
        yield return new WaitForSeconds(2);
        Instantiate<GameObject>(pathPrefab, transform.position + transform.forward * 10, Quaternion.Euler(new Vector3(0, headingAngle, 0)));
    }

    //ENUM

    public enum BoatState
    {
        loading, gameplay, encounter
    }
}
