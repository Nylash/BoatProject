using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadingEncounter : Encounter
{
    BoatController boatController;

    bool chooseHeading;

    private void Update()
    {
        if (chooseHeading)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                print("You choose path 1");
                chooseHeading = false;
                boatController.ChangeBoatHeading(-30);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                print("You choose path 2");
                chooseHeading = false;
                boatController.ChangeBoatHeading(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                print("You choose path 3");
                chooseHeading = false;
                boatController.ChangeBoatHeading(30);
            }
        }
    }

    public void ChooseHeading(BoatController script)
    {
        boatController = script;
        chooseHeading = true;
        print("Choose your heading, press 1 2 or 3");
    }
}
