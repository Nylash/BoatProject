using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter : MonoBehaviour
{
    public EncounterType encounterType;

    public enum EncounterType
    {
        none, heading, islandLoad, island, merchant, wreck, pirates
    }
}
