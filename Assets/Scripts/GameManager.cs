﻿using UnityEngine;
using UnityEngine.EventSystems;

public enum GameState {NullState, MainMenu, Game, PauseMenu}
public delegate void OnStateChangeHandler();

public class GameManager : MonoBehaviour {
	
	private static GameManager _instance;
	
	private static MainMenu _mainmenu   = MainMenu.Instance;
	private static HUD 		_hud 		= HUD.Instance;
	private static Settings _settings 	= Settings.Instance;
	private static Quit 	_quit 		= Quit.Instance;
	
	public event OnStateChangeHandler OnStateChange;
	public GameState gameState {get; private set;}
	
	
	/*
	* Find/Create/Return our one and only Game Manager object
	* for the game
	**/
	public static GameManager Instance {
		get {
			// if we do not have an instance already, lets look to see
			// if one has already been created for us
			if (_instance == null) {
				_instance = Object.FindObjectOfType<GameManager>();
			}
			
			// If we still dont have an instance, one must not be created
			// so lets create our own and prevent it from being deleted
			// when the level changes
			if (_instance == null)
			{
				GameObject go = new GameObject("_GameManager");
				DontDestroyOnLoad(go);
				_instance = go.AddComponent<GameManager>();
				_instance.OnStateChange += OnGameStateChange;

				// Before we start creating the UI lets create the event system
				// This would happen naturally by adding a Canvas through the UI)
				GameObject eventSystem = new GameObject("_EventSystem");
				eventSystem.AddComponent<EventSystem>();
				eventSystem.AddComponent<StandaloneInputModule>();
				eventSystem.AddComponent<TouchInputModule>();
				DontDestroyOnLoad(eventSystem);
				
				// Now lets just create our blank canvas, that all of our GUI will be a part of
				GameObject gui = (GameObject) Instantiate(Resources.Load ("GUI/MainCanvas"), new Vector3(), Quaternion.identity);
				gui.name = "_GUI";
				DontDestroyOnLoad(gui);
			}
			
			return _instance;
		}
	}

	/*
	* Update the Game State and call any Game State Change Handlers that have
	* been added
	**/
	public void SetGameState(GameState aGameState) {
		if (gameState != aGameState) {
			gameState = aGameState;
		
			if(OnStateChange != null) {
				OnStateChange();
			}
		}
	}
	
	private static void OnGameStateChange() {
		
		if (_instance.gameState == GameState.MainMenu) {
			Debug.Log("Loading Main Menu");
			Application.LoadLevel(0);
		}
		else if(_instance.gameState == GameState.Game) {
			Application.LoadLevel(1);
			
			Debug.Log("Changing timeScale from: " + Time.timeScale);
			Time.timeScale = 1;
		}
		else if (_instance.gameState == GameState.PauseMenu)
		{
			Debug.Log("Changing timeScale from: " + Time.timeScale + " to 0");
			Time.timeScale = 0;
		}
	}
}
