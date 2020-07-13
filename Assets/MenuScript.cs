﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    public Vector2 normalisedMousePosition;
    public float currentAngle;
    public int selection;
    private int previousSelection;

    public GameObject[] menuItems;

    private MenuItem menuItemSc;
    private MenuItem previousMenuItemSc;

    [HideInInspector] public bool showing;

    // Start is called before the first frame update
    void Awake()
    {
        selection = 0;
        ChooseSpell(selection);
        HideMenu();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            ShowMenu();
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            ChooseSpell(selection);
            HideMenu();
        }

        if (!showing)
            return;

        normalisedMousePosition = new Vector2(Input.mousePosition.x - Screen.width / 2, Input.mousePosition.y - Screen.height / 2);
        currentAngle = Mathf.Atan2(normalisedMousePosition.y, normalisedMousePosition.x) * Mathf.Rad2Deg;

        currentAngle = (currentAngle + 360) % 360;

        selection = (int) currentAngle / 60;

        if(selection != previousSelection)
        {
            previousMenuItemSc = menuItems[previousSelection].GetComponent<MenuItem>();
            previousMenuItemSc.Deselect();
            previousSelection = selection;

            menuItemSc = menuItems[selection].GetComponent<MenuItem>();
            menuItemSc.Select();
        }
    }

    private void ShowMenu()
    {
        foreach (GameObject item in menuItems)
        {
            item.SetActive(true);
        }
        Cursor.lockState = CursorLockMode.None;
        showing = true;
    }

    private void HideMenu()
    {
        foreach (GameObject item in menuItems)
        {
            item.SetActive(false);
        }
        Cursor.lockState = CursorLockMode.Locked;
        showing = false;
    }

    private void ChooseSpell(int selectedSpell)
    {
        switch (selectedSpell)
        {
            case 0:
                Debug.Log("Has elegido Geyser");
                break;
            case 1:
                Debug.Log("Has elegido Push");
                break;
            case 2:
                Debug.Log("Has elegido Pull Rocks");
                break;
            case 3:
                Debug.Log("Has elegido Grow Wall");
                break;
            case 4:
                Debug.Log("Has elegido Proyectiles");
                break;
            default:
                Debug.Log("Has elegido Roots");
                break;

        }
    }
}