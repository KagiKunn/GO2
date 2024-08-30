using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LocaleManager : MonoBehaviour
{
    public static LocaleManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(this);
        }
        
        setLocale(PlayerLocalManager.Instance.lLocale);
    }

    public void setLocale(string toLocale)
    {
        var availableLocales = LocalizationSettings.AvailableLocales.Locales;
        Locale selectedLocale = availableLocales.Find(locale => locale.Identifier.Code == toLocale);
        
        if (selectedLocale != null)
        {
            LocalizationSettings.SelectedLocale = selectedLocale;
            PlayerLocalManager.Instance.lLocale = toLocale;
            PlayerLocalManager.Instance.Save();
        }
    }
}
