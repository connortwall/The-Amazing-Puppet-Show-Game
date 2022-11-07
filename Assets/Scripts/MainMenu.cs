using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    //set first scene of the game within Unity
    public string firstScene;

    private GameObject settingsScreen;
    private GameObject controlsScreen;

    //create buttons for controller nagivations
    public GameObject startButton, controlsButton, settingsButton, exitButton, controlsBackButton;


    // Start is called before the first frame update
    void Start()
    {
        //find the settings menu and verify found, then disable
		settingsScreen = GameObject.Find("SettingsScreen");
		if (settingsScreen == null) throw new Exception("Settings screen could not be loaded");
        settingsScreen.SetActive(false);

        //find the controls page and verify found, then disable
		controlsScreen = GameObject.Find("ControlsScreen");
		if (controlsScreen == null) throw new Exception("Controls screen could not be loaded");
        CloseControls();

        //clear controller selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set beginning selection to Start Game
        EventSystem.current.SetSelectedGameObject(startButton);

        //create user preferences if they don't exist
        CreateUserPrefs();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame(){
        SceneManager.LoadScene(firstScene);
    }

    public void OpenControls(){
        controlsScreen.SetActive(true);
        //set selection to controls back button
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(controlsBackButton);
    }

    public void CloseControls(){
        controlsScreen.SetActive(false);
        //set selection to controls button
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(controlsButton);
    }

    public void OpenSettings(){
        settingsScreen.SetActive(true);
    }

    public void CloseSettings(){
        settingsScreen.SetActive(false);

        //set selection to settings
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(settingsButton);
    }

    public void ExitGame(){
        print("Exiting game...");
        //save player preferences before quitting
        PlayerPrefs.Save();
        Application.Quit();
    }

    //checks if the user preferences have been created; else creates them
    private void CreateUserPrefs(){
        if (!PlayerPrefs.HasKey("language")) PlayerPrefs.SetInt("language", 0);
        if (!PlayerPrefs.HasKey("sfxVolume")) PlayerPrefs.SetFloat("masterVolume", 0.75f);
        if (!PlayerPrefs.HasKey("sfxVolume")) PlayerPrefs.SetFloat("sfxVolume", 0.75f);
        if (!PlayerPrefs.HasKey("musicVolume")) PlayerPrefs.SetFloat("musicVolume", 0.75f);
    }
}
