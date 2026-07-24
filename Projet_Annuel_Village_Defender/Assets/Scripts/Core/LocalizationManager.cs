using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public enum Language
    {
        French,
        English
    }

    public static class LocalizationManager
    {
        private const string PrefsKey = "Language";

        public static event Action LanguageChanged;

        private static Language _currentLanguage = Language.French;

        public static Language CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                if (_currentLanguage == value) return;
                _currentLanguage = value;
                PlayerPrefs.SetInt(PrefsKey, (int)value);
                LanguageChanged?.Invoke();
            }
        }

        private static readonly Dictionary<string, (string fr, string en)> Strings = new()
        {
            ["ui.settings_button"] = ("Parametres", "Settings"),
            ["ui.play_button"] = ("Jouer", "Play"),
            ["ui.quit_button"] = ("Quitter", "Quit"),
            ["ui.cancel_button"] = ("Annuler", "Cancel"),
            ["ui.pseudo_label"] = ("Pseudo", "Username"),
            ["ui.enter_text_placeholder"] = ("Entrez du texte...", "Enter text..."),
            ["ui.volume_label"] = ("Volume", "Volume"),
            ["ui.you_lose"] = ("VOUS AVEZ PERDU", "YOU LOSE"),
            ["ui.restart_button"] = ("Recommencer", "Restart"),
            ["ui.menu_button"] = ("Menu", "Menu"),
            ["ui.language_button"] = ("English", "Francais"),

            ["tower.canon"] = ("Canon", "Cannon"),
            ["tower.electrique"] = ("Tour Electrique", "Tesla"),
            ["tower.title_prefix"] = ("Tour - Niveau ", "Tower - Level "),
            ["tower.pv_prefix"] = ("PV : ", "HP : "),
            ["tower.degats_prefix"] = ("Degats : ", "Damage : "),
            ["tower.tirs_prefix"] = ("Tirs/s : ", "Shots/s : "),
            ["tower.cout_amelioration_prefix"] = ("Cout amelioration : ", "Upgrade cost : "),
            ["tower.cout_fusion_prefix"] = ("Cout fusion : ", "Fusion cost : "),
            ["tower.niveau_max"] = ("Niveau max", "Max level"),
            ["tower.ameliorer_button"] = ("Ameliorer", "Upgrade"),
            ["tower.tour_fusionnee"] = ("Tour fusionnee", "Fused tower"),
            ["tower.fusion_instruction"] = ("Cliquez sur une tour niveau 3 d'un autre type pour fusionner", "Click a level 3 tower of a different type to fuse"),
            ["tower.fusion_same_type"] = ("Meme type ! Choisissez une tour d'un type different.", "Same type! Choose a tower of a different type."),
            ["tower.fusion_not_level3"] = ("Cette tour n'est pas niveau 3 !", "This tower isn't level 3!"),

            ["village.hp_prefix"] = ("PV Village : ", "Village HP: "),
            ["score.best_prefix"] = ("Record : ", "Best: "),
            ["player.money_prefix"] = ("Argent : ", "Gold: "),

            ["wave.vague_prefix"] = ("Vague : ", "Wave : "),
            ["wave.next_wave_prefix"] = ("Prochaine vague dans : ", "Next wave in : "),
            ["wave.remaining_prefix"] = ("Gobelins restants : ", "Remaining goblins : "),
            ["wave.waiting_text"] = ("Vague en cours", "Wave in progress"),
        };

        public static string Get(string key)
        {
            if (Strings.TryGetValue(key, out var pair))
                return _currentLanguage == Language.French ? pair.fr : pair.en;

            Debug.LogWarning($"[LocalizationManager] Missing key: {key}");
            return key;
        }

        public static void ToggleLanguage()
        {
            CurrentLanguage = _currentLanguage == Language.French ? Language.English : Language.French;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Load()
        {
            _currentLanguage = (Language)PlayerPrefs.GetInt(PrefsKey, (int)Language.French);
        }
    }
}
