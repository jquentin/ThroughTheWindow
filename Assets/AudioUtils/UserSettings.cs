
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//next time call it UserData
public static class UserSettings : object {

	public const string VOICE_OVER_ENABLED_KEY = "_voiceOverEnabled";
	public const string EXTENDED_STORYTELLING_ENABLED_KEY = "_extendedStoryTellingVoiceOverEnabled";
	public const string SFX_ENABLED_KEY = "_sfxEnabled";
	public const string MUSIC_ENABLED_KEY = "_musicEnabled";
	public const string SPEECH_AND_TEXT_ENABLED_KEY = "_speechAndTextEnabled";

	public static bool ApplicationIsInDemoMode = false;//when set to true it will skip intros and not show menus


	private static bool _voiceOverEnabled;
	private static bool _sfxEnabled;
	private static bool _extendedStoryTellingVoiceOverEnabled;
	private static bool _musicEnabled;
	private static bool _speechAndTextEnabled;
	private static bool _rodModeUnlockingCompleted;//has the user already gone through unlocking the full game by going through the rods unlocking UI?
	private static bool _noRodsModeFullyUnlocked;
	private static bool _userSawSignUpForNewsletter;
	private static bool _userSignedUpForNewsletter;

	private static string _firstInstallDate;//the date the app was first installed, right now not used for anything, but saving it locally anyway, in case later we want to lock features for people who install an update but don't want to lock it for people who installed it before the update


	public static bool alreadyPlayedTitlescreenIntroAnim = false; //this is not saved to playerprefs, just set to true after playing the intro once so it does not play again next time the user presses the home button..
	

	private static bool alreadyLoadedSavedSettingVoiceOverEnabled;
	private static bool alreadyLoadedSavedSettingSFXEnabled;
	private static bool alreadyLoadedSavedSettingextendedStoryTellingVoiceOverEnabled;
	private static bool alreadyLoadedSavedSettingMusicEnabled;
	private static bool alreadyLoadedSavedSettingSpeechAndTextEnabled;
	private static bool alreadyLoadedSavedSettingGameUnlocked;
	public static bool alreadySolvedParentAccessMathProblem = false;//gets set to true when solving the math problem so then on later external links calls it can skip showing the math problem first


	private static int prefsValue = 0;
	
	private static readonly string FIRST_INSTALL_DATE_KEY;

	public delegate void OnAudioStateChangedHandler();
	
	public static OnAudioStateChangedHandler OnVoiceOverStateChanged;
	public static OnAudioStateChangedHandler OnMusicStateChanged;
	public static OnAudioStateChangedHandler OnSoundsStateChanged;
	public static OnAudioStateChangedHandler OnExtendedStoryTellingStateChanged;
	public static OnAudioStateChangedHandler OnSpeechAndTextStateChanged;

	public static DateTime firstInstallDate{
		get {
			string _firstInstallDate;
			if(PlayerPrefs.HasKey(FIRST_INSTALL_DATE_KEY)){
				_firstInstallDate = PlayerPrefs.GetString(FIRST_INSTALL_DATE_KEY);
				
			}else{
				DateTime today = System.DateTime.Today;
				_firstInstallDate = today.ToString();
				PlayerPrefs.SetString(FIRST_INSTALL_DATE_KEY, _firstInstallDate);
				//date: 09/28/2014 00:00:00,_firstInstallDate:09/28/2014 00:00:00
			}
			DateTime dt = Convert.ToDateTime(_firstInstallDate);
			return dt;
		}
	}


	static bool result;
	static bool GetAudioTypeStatus(ref bool cacheBool, ref bool alreadyLoaded, string playerPrefsKey, OnAudioStateChangedHandler callbackEvent)
	{
		if(!alreadyLoaded){
//			if (TeachleyFacade.IsTeachleyAvailable())
//			{
//				result = true;
//				if (TeachleyFacade.isSettingsCached)
//				{
//					TeachleyFacade.GetSettingBool(playerPrefsKey, ((settingValue) => {Debug.Log(settingValue); result = settingValue;}), true);
//					cacheBool = result;
//				}
//				else
//				{
//					//TODO work here. lambda functions don't accept ref parameters so we can't set the cached value in the callback.
//					bool cacheBoolCopy = cacheBool;
//					TeachleyFacade.GetSettingBool(playerPrefsKey, ((bool settingValue) => SetAudioTypeStatus(settingValue, ref cacheBoolCopy, playerPrefsKey, null, null, callbackEvent, false)), true);
//				}
//				return result;
//			}
//			else
			{
				int enabledInt = PlayerPrefs.GetInt(playerPrefsKey,1);
				if(enabledInt==1){
					cacheBool = true;
				}else{
					cacheBool = false;
				}
			}
			alreadyLoaded = true;
		}
		return cacheBool;
	}

	static void SetAudioTypeStatus(bool value, ref bool cacheBool, string playerPrefsKey, string GAActionEnable, string GAActionDisable, OnAudioStateChangedHandler callbackEvent, bool isUserInput = true)
	{

		if (cacheBool == value)
			return;
		cacheBool = value;

		Debug.Log(playerPrefsKey + " = " + value);

#if !DISABLE_DEFAULT_ANALYTICS
		if (isUserInput)
			GASingleton.LogMenuEvent(value ? GAActionEnable : GAActionDisable, "", 0L);
#endif
//		if (TeachleyFacade.IsTeachleyAvailable ()) {
//			TeachleyFacade.SetSettingBool(playerPrefsKey, playerPrefsKey, value);
//		}
		PlayerPrefs.SetInt(playerPrefsKey, value ? 1 : 0);

		if (callbackEvent != null)
			callbackEvent();
	}

	public static bool voiceOverAndSpeechEnabled{
		get
		{
			return voiceOverEnabled && speechAndTextEnabled;
		}
	}

	public static bool voiceOverEnabled{
		get {
			return GetAudioTypeStatus(ref _voiceOverEnabled, ref alreadyLoadedSavedSettingVoiceOverEnabled, VOICE_OVER_ENABLED_KEY, OnVoiceOverStateChanged);
		}
		
		set {
			SetAudioTypeStatus(value, ref _voiceOverEnabled, VOICE_OVER_ENABLED_KEY, GAConstants.ACT_ENABLE_VO, GAConstants.ACT_DISABLE_VO, OnVoiceOverStateChanged);
		}
	}


	public static bool extendedStoryTellingVoiceOverEnabled{
		get {
			return GetAudioTypeStatus(ref _extendedStoryTellingVoiceOverEnabled, ref alreadyLoadedSavedSettingextendedStoryTellingVoiceOverEnabled, EXTENDED_STORYTELLING_ENABLED_KEY, OnExtendedStoryTellingStateChanged);
		}
		
		set {
			SetAudioTypeStatus(value, ref _extendedStoryTellingVoiceOverEnabled, EXTENDED_STORYTELLING_ENABLED_KEY, GAConstants.ACT_ENABLE_STORYTELLING, GAConstants.ACT_DISABLE_STORYTELLING, OnExtendedStoryTellingStateChanged);
		}
	}

	public static bool sfxEnabled{
		get {
			return GetAudioTypeStatus(ref _sfxEnabled, ref alreadyLoadedSavedSettingSFXEnabled, SFX_ENABLED_KEY, OnSoundsStateChanged);
		}
		
		set {
			SetAudioTypeStatus(value, ref _sfxEnabled, SFX_ENABLED_KEY, GAConstants.ACT_ENABLE_SOUNDS, GAConstants.ACT_DISABLE_SOUNDS, OnSoundsStateChanged);
		}
	}
	
	
	
	public static bool musicEnabled{
		get {
			return GetAudioTypeStatus(ref _musicEnabled, ref alreadyLoadedSavedSettingMusicEnabled, MUSIC_ENABLED_KEY, OnMusicStateChanged);
		}
		
		set {
			SetAudioTypeStatus(value, ref _musicEnabled, MUSIC_ENABLED_KEY, GAConstants.ACT_ENABLE_MUSIC, GAConstants.ACT_DISABLE_MUSIC, OnMusicStateChanged);
		}
	}

	public static bool speechAndTextEnabled{
		get {
			return GetAudioTypeStatus(ref _speechAndTextEnabled, ref alreadyLoadedSavedSettingSpeechAndTextEnabled, SPEECH_AND_TEXT_ENABLED_KEY, OnSpeechAndTextStateChanged);
		}

		set {
			SetAudioTypeStatus(value, ref _speechAndTextEnabled, SPEECH_AND_TEXT_ENABLED_KEY, GAConstants.ACT_ENABLE_SPEECH_AND_TEXT, GAConstants.ACT_DISABLE_SPEECH_AND_TEXT, OnSpeechAndTextStateChanged);
		}
	}

	public static string firstinstallDate{

		get {
			if(PlayerPrefs.HasKey("firstInstallDate")){
				_firstInstallDate = PlayerPrefs.GetString("firstInstallDate");

				Debug.Log("dateStored: "+_firstInstallDate);

			}else{
				DateTime today = System.DateTime.Today;

				_firstInstallDate = today.ToString();
				PlayerPrefs.SetString("firstInstallDate",_firstInstallDate);

				Debug.Log("date: "+today+",_firstInstallDate:"+_firstInstallDate);//date: 09/28/2014 00:00:00,_firstInstallDate:09/28/2014 00:00:00

			}
			return _firstInstallDate;

		}



	}

	public static void GetStoredSettings(){

		//just have this here to load the saved settings:
		bool getValueForApp = sfxEnabled;
		bool getValueForApp2 = voiceOverEnabled;
		bool getValueForApp3 = extendedStoryTellingVoiceOverEnabled;
		string installDate = firstinstallDate;

		//CheckDate(installDate);
	}

}
