//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

/// <summary>
/// Localization manager is able to parse localization information from text assets.
/// Using it is simple: text = Localization.Get(key), or just add a UILocalize script to your labels.
/// You can switch the language by using Localization.language = "French", for example.
/// This will attempt to load the file called "French.txt" in the Resources folder,
/// or a column "French" from the Localization.csv file in the Resources folder.
/// If going down the TXT language file route, it's expected that the file is full of key = value pairs, like so:
/// 
/// LABEL1 = Hello
/// LABEL2 = Music
/// Info = Localization Example
/// 
/// In the case of the CSV file, the first column should be the "KEY". Other columns
/// should be your localized text values, such as "French" for the first row:
/// 
/// KEY,English,French
/// LABEL1,Hello,Bonjour
/// LABEL2,Music,Musique
/// Info,"Localization Example","Par exemple la localisation"
/// </summary>


/////////////   Tiggly  ////////////////// 
/// Tiggly customisation has been made to enable the localization to read all the files 
/// containing "Localization" instead of only one.
/// Main change is in "LoadDictionary".
/// Also in "LoadCSV" the line "mDictionary.Clear();" has been commented out.
public static class Localization
{
	/// <summary>
	/// Whether the localization dictionary has been loaded.
	/// </summary>
 
	static public bool localizationHasBeenSet = false;

	// Loaded languages, if any
	static string[] mLanguages = null;

	// Key = Value dictionary (single language)
	static Dictionary<string, string> mOldDictionary = new Dictionary<string, string>();

	// Key = Values dictionary (multiple languages)
	static Dictionary<string, string[]> mDictionary = new Dictionary<string, string[]>();

	// Index of the selected language within the multi-language dictionary
	static int mLanguageIndex = -1;
	
	// Index of the global version of selected language (Example: "en" if the selected language is "en-UK"
	static int mGlobalLanguageIndex = -1;

	// Currently selected language
	static string mLanguage;

	public delegate void OnLanguageChangedHandler();
	public static OnLanguageChangedHandler OnLanguageChanged;

	/// <summary>
	/// Localization dictionary. Dictionary key is the localization key. Dictionary value is the list of localized values (columns in the CSV file).
	/// Be very careful editing this via code, and be sure to set the "KEY" to the list of languages.
	/// </summary>

	static public Dictionary<string, string[]> dictionary
	{
		get
		{
			if (!localizationHasBeenSet) language = PlayerPrefs.GetString("Language", "English");
			return mDictionary;
		}
		set
		{
			localizationHasBeenSet = (value != null);
			mDictionary = value;
		}
	}

	/// <summary>
	/// List of loaded languages. Available if a single Localization.csv file was used.
	/// </summary>

	static public string[] knownLanguages
	{
		get
		{
			if (!localizationHasBeenSet) LoadDictionary(PlayerPrefs.GetString("Language", "English"));
			return mLanguages;
		}
	}

	/// <summary>
	/// Name of the currently active language.
	/// </summary>

	static public string language
	{
		get
		{
			if (string.IsNullOrEmpty(mLanguage))
			{
				string[] lan = knownLanguages;
				mLanguage = PlayerPrefs.GetString("Language", lan != null ? lan[0] : "English");
				LoadAndSelect(mLanguage);
			}
			return mLanguage;
		}
		set
		{
			if (mLanguage != value)
			{
				mLanguage = value;
				LoadAndSelect(value);
			}
		}
	}

	static int GetOrderIndex(TextAsset textAsset)
	{

		int orderIndex = 0;

		ByteReader reader = new ByteReader(textAsset);

		string firstLine = reader.ReadLine(true);

		Regex regex = new Regex(@"order-index\s*[=:]\s*([0-9]+)");

		Match match = regex.Match(firstLine);

		if (match.Success)
		{
			string orderIndexString = match.Groups[1].Captures[0].Value;
			int.TryParse(orderIndexString, out orderIndex);
		}

		return orderIndex;
	}

	/// <summary>
	/// Load the specified localization dictionary.
	/// </summary>

	static bool LoadDictionary (string value)
	{
		// Try to load the localization file
		if (!localizationHasBeenSet)
		{
			localizationHasBeenSet = true;
			mDictionary.Clear();
			bool hasLoaded = false;
			TextAsset[] localizationFiles = Resources.LoadAll<TextAsset>("Localization");

			localizationFiles = localizationFiles.OrderBy(ta => GetOrderIndex(ta)).ToArray();

			for(int i = 0 ; i < localizationFiles.Length ; i++)
			{
				TextAsset locFile = localizationFiles[i];
				if (locFile != null && LoadCSV(locFile))
					hasLoaded = true;
			}
			if (hasLoaded)
				return true;
		}

		// If this point was reached, the localization file was not present
		if (string.IsNullOrEmpty(value)) return false;

		// Not a referenced asset -- try to load it dynamically
		TextAsset txt = Resources.Load(value, typeof(TextAsset)) as TextAsset;
		
		if (txt != null)
		{
			Load(txt);
			return true;
		}
		return false;

		/// Old version

//		bool loadCSVSucceed = false;
//		// Try to load the Localization CSV
//		if (!localizationHasBeenSet)
//		{
//			mDictionary.Clear();
//			foreach (TextAsset locFile in Resources.LoadAll<TextAsset>("Localization")) {
//				
//				// Try to load the localization file
//				if (locFile != null && LoadCSV (locFile))
//					loadCSVSucceed =  true;
//				
//			}
//		}
//		
//		localizationHasBeenSet = true;
//		
//		if (loadCSVSucceed)
//			return true;
//		
//		// If this point was reached, the localization file was not present
//		if (string.IsNullOrEmpty (value))
//			return false;
//		
//		// Not a referenced asset -- try to load it dynamically
//		TextAsset txt = Resources.Load (value, typeof(TextAsset)) as TextAsset;
//		
//		if (txt != null) {
//			Load (txt);
//			return true;
//		}
//		return false;
	}

	/// <summary>
	/// Load the specified language.
	/// </summary>

	static bool LoadAndSelect (string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			if (mDictionary.Count == 0 && !LoadDictionary(value)) return false;
			if (SelectLanguage(value)) return true;
		}

		// Old style dictionary
		if (mOldDictionary.Count > 0) return true;

		// Either the language is null, or it wasn't found
		mOldDictionary.Clear();
		mDictionary.Clear();
		if (string.IsNullOrEmpty(value)) PlayerPrefs.DeleteKey("Language");
		return false;
	}

	/// <summary>
	/// Load the specified asset and activate the localization.
	/// </summary>

	static public void Load (TextAsset asset)
	{
		ByteReader reader = new ByteReader(asset);
		Set(asset.name, reader.ReadDictionary());
	}

	/// <summary>
	/// Load the specified CSV file.
	/// </summary>

	static public bool LoadCSV (TextAsset asset)
	{
		ByteReader reader = new ByteReader(asset);

		// The first line should contain the order index.
		BetterList<string> temp = reader.ReadCSV();

		// The second line should contain "KEY", followed by languages.
		temp = reader.ReadCSV();

		// There must be at least two columns in a valid CSV file
		if (temp.size < 2) return false;

		// The first entry must be 'KEY', capitalized
		temp[0] = "KEY";

#if !UNITY_3_5
		// Ensure that the first value is what we expect
		if (!string.Equals(temp[0], "KEY"))
		{
			Debug.LogError("Invalid localization CSV file. The first value is expected to be 'KEY', followed by language columns.\n" +
				"Instead found '" + temp[0] + "'", asset);
			return false;
		}
		else
#endif
		{
			mLanguages = new string[temp.size - 1];
			for (int i = 0; i < mLanguages.Length; ++i)
				mLanguages[i] = temp[i + 1];
		}
		
		//		mDictionary.Clear();
		
		// Read the entire CSV file into memory
		while (temp != null)
		{
			AddCSV(temp);
			temp = reader.ReadCSV();
		}
		return true;
	}

	/// <summary>
	/// Select the specified language from the previously loaded CSV file.
	/// </summary>

	static bool SelectLanguage (string language)
	{
		Debug.Log("SelectLanguage : " + language);
		mLanguageIndex = -1;
		mGlobalLanguageIndex = -1;

		if (mDictionary.Count == 0) return false;

		string[] keys;
		// look for a global version of the language ("English" for "English-UK")
		string globalLanguage = "";
		int dashIndex = language.IndexOf("-");
		if (dashIndex > 0)
			globalLanguage = language.Remove(dashIndex);
		if (mDictionary.TryGetValue("KEY", out keys))
		{
			for (int i = 0; i < keys.Length; ++i)
			{
				if (keys[i] == language)
				{
					mLanguageIndex = i;
				}
			}
			Debug.Log("mLanguageIndex = " + mLanguageIndex);
			Debug.Log("globalLanguage = " + globalLanguage);
			// if there is a global version of the language, look for it too
			if (globalLanguage != language)
			{
				for (int i = 0; i < keys.Length; ++i)
				{
					if (keys[i] == globalLanguage)
					{
						// if language has been found, set the global language as global language
						if (mLanguageIndex >= 0)
							mGlobalLanguageIndex = i;
						// if language hasn't been found, set the global language as main language
						else
							mLanguageIndex = i;
					}
				}
			}
			Debug.Log("mGlobalLanguageIndex = " + mGlobalLanguageIndex);
			if (mLanguageIndex >= 0)
			{
				mOldDictionary.Clear();
				mLanguage = language;
				PlayerPrefs.SetString("Language", mLanguage);
				UIRoot.Broadcast("OnLocalize");
				if (OnLanguageChanged != null) OnLanguageChanged();
				return true;

			}
		}
		return false;
	}

	/// <summary>
	/// Helper function that adds a single line from a CSV file to the localization list.
	/// </summary>

	static void AddCSV (BetterList<string> values)
	{
		if (values.size < 2) return;
		string[] temp = new string[values.size - 1];
		for (int i = 1; i < values.size; ++i) temp[i - 1] = values[i];
		
		try
		{
			if(mDictionary.ContainsKey(values[0]))
			{
				Debug.Log("Override localization entry: " + values[0]);
				mDictionary[values[0]] = temp;
			}
			else
				mDictionary.Add(values[0], temp);
		}
		catch (System.Exception ex)
		{
			Debug.LogWarning("Unable to add '" + values[0] + "' to the Localization dictionary.\n" + ex.Message);
		}
	}

	/// <summary>
	/// Load the specified asset and activate the localization.
	/// </summary>

	static public void Set (string languageName, Dictionary<string, string> dictionary)
	{
		Debug.Log("Set");
		mLanguage = languageName;
		PlayerPrefs.SetString("Language", mLanguage);
		mOldDictionary = dictionary;
		localizationHasBeenSet = false;
		mLanguageIndex = -1;
		mLanguages = new string[] { languageName };
		UIRoot.Broadcast("OnLocalize");
	}

	static bool isArabic
	{
		get
		{
			return (language == "Arabic");
		}
	}

	static string FixArabic(string text, bool fixArabic = true)
	{
//		return ((isArabic && fixArabic) ? ArabicSupport.ArabicFixer.Fix(text) : text);
		return text;
	}

	/// <summary>
	/// Localize the specified value.
	/// </summary>

	static public string Get (string key, bool fixArabic = true)
	{
		// Ensure we have a language to work with
		if (!localizationHasBeenSet) language = PlayerPrefs.GetString("Language", "English");

		string val;
		string[] vals;
#if UNITY_IPHONE || UNITY_ANDROID
		string mobKey = key;

		if (mLanguageIndex != -1 && mDictionary.TryGetValue(mobKey, out vals))
		{
			if (mLanguageIndex < vals.Length)
				return FixArabic(vals[mLanguageIndex], fixArabic);
		}
		else if (mOldDictionary.TryGetValue(mobKey, out val)) return FixArabic(val, fixArabic);
#endif
		if (mLanguageIndex != -1 && mDictionary.TryGetValue(key, out vals))
		{
			if (mLanguageIndex < vals.Length)
				return FixArabic(vals[mLanguageIndex], fixArabic);
		}
		else if (mOldDictionary.TryGetValue(key, out val)) return FixArabic(val, fixArabic);

		//Look for global version of this language

#if UNITY_IPHONE || UNITY_ANDROID
		
		if (mGlobalLanguageIndex != -1 && mDictionary.TryGetValue(mobKey, out vals))
		{
			if (mGlobalLanguageIndex < vals.Length)
				return FixArabic(vals[mGlobalLanguageIndex], fixArabic);
		}
		else if (mOldDictionary.TryGetValue(mobKey, out val)) return FixArabic(val, fixArabic);
#endif
		if (mGlobalLanguageIndex != -1 && mDictionary.TryGetValue(key, out vals))
		{
			if (mGlobalLanguageIndex < vals.Length)
				return FixArabic(vals[mGlobalLanguageIndex], fixArabic);
		}
		else if (mOldDictionary.TryGetValue(key, out val)) return FixArabic(val, fixArabic);
		
#if UNITY_EDITOR
		Debug.LogWarning("Localization key not found: '" + key + "'");
#endif
		return "";
	}

	static public string Get (string key, int number)
	{
		string res = string.Empty;
		// Try to find an entry for the key with an appended "_i+" where i <= number, searching from biggest to smallest
		for (int i = number ; i > 1 ; i--)
		{
			res = Get(key + "_" + i + "+");
			if (!string.IsNullOrEmpty(res))
				return res;
		}
		res = Get(key + "_Plural");
		if (!string.IsNullOrEmpty(res))
			return res;
		res = Get(key + "_P");
		return res;
	}

	[System.Obsolete("Localization is now always active. You no longer need to check this property.")]
	static public bool isActive { get { return true; } }

	[System.Obsolete("Use Localization.Get instead")]
	static public string Localize (string key) { return Get(key); }

	/// <summary>
	/// Returns whether the specified key is present in the localization dictionary.
	/// </summary>

	static public bool Exists (string key)
	{
		// Ensure we have a language to work with
		if (!localizationHasBeenSet) language = PlayerPrefs.GetString("Language", "English");

#if UNITY_IPHONE || UNITY_ANDROID
		string mobKey = key + " Mobile";
		if (mDictionary.ContainsKey(mobKey)) return true;
		else if (mOldDictionary.ContainsKey(mobKey)) return true;
#endif
		return mDictionary.ContainsKey(key) || mOldDictionary.ContainsKey(key);
	}
}
