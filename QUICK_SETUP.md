# 🎯 Guide Rapide - Configuration Menu Unity

## ✅ Scripts créés

Tous les scripts sont dans `Assets/Scripts/UI/` :

- ✅ **MainMenuManager.cs** - Navigation du menu
- ✅ **SettingsManager.cs** - Gestion des paramètres
- ✅ **ButtonHoverEffect.cs** - Effets visuels sur boutons
- ✅ **MenuMusicManager.cs** - Musique de fond
- ✅ **SceneFadeTransition.cs** - Transitions fluides

## 🚀 Configuration en 5 étapes

### 1️⃣ Créer la scène MainMenu
- `File > New Scene`
- Sauvegarder comme **"MainMenu"** dans `Assets/Scenes/`

### 2️⃣ Créer l'UI de base

**Canvas :**
```
UI > Canvas
  ├─ Canvas Scaler: Scale With Screen Size (1920x1080)
  │
  ├─ MainMenuPanel (Panel)
  │  ├─ PlayButton (Button - TextMeshPro) → "LANCER PARTIE"
  │  ├─ SettingsButton (Button - TextMeshPro) → "PARAMÈTRES"  
  │  └─ QuitButton (Button - TextMeshPro) → "QUITTER"
  │
  └─ SettingsPanel (Panel) [DÉSACTIVÉ]
     ├─ MasterVolumeSlider + Text
     ├─ MusicVolumeSlider + Text
     ├─ SFXVolumeSlider + Text
     ├─ QualityDropdown
     ├─ ResolutionDropdown
     ├─ FullscreenToggle
     └─ BackButton (Button) → "RETOUR"
```

### 3️⃣ Ajouter les scripts

**Sur le Canvas (ou GameObject vide "MenuManager") :**
- Add Component > **MainMenuManager**
  - Assigner Main Menu Panel
  - Assigner Settings Panel

**Sur un GameObject "SettingsManager" :**
- Add Component > **SettingsManager**
  - Assigner tous les sliders, dropdowns, toggles et texts

### 4️⃣ Connecter les boutons

**PlayButton > On Click() :**
- `MainMenuManager > PlayGame()`

**SettingsButton > On Click() :**
- `MainMenuManager > OpenSettings()`

**QuitButton > On Click() :**
- `MainMenuManager > QuitGame()`

**BackButton > On Click() :**
- `MainMenuManager > ShowMainMenu()`

**Sliders > On Value Changed() :**
- MasterVolumeSlider → `SettingsManager > SetMasterVolume(float)`
- MusicVolumeSlider → `SettingsManager > SetMusicVolume(float)`
- SFXVolumeSlider → `SettingsManager > SetSFXVolume(float)`

**Dropdowns/Toggle > On Value Changed() :**
- QualityDropdown → `SettingsManager > SetQuality(int)`
- ResolutionDropdown → `SettingsManager > SetResolution(int)`
- FullscreenToggle → `SettingsManager > SetFullscreen(bool)`

### 5️⃣ Build Settings

- `File > Build Settings`
- Add Open Scenes (MainMenu en position 0)
- Ajouter la scène "Game"

## 🎨 Bonus : Transitions et Musique

### Ajouter des transitions fluides :

1. Créer un GameObject vide : "FadeTransition"
2. Add Component > **SceneFadeTransition**
3. Ajouter un enfant : `UI > Image` (nom: "FadeImage")
4. Configurer FadeImage :
   - Anchor: Stretch both ways (full screen)
   - Color: Noir
   - Alpha: 0
5. Assigner FadeImage dans SceneFadeTransition
6. Cocher "DontDestroyOnLoad" pour l'objet parent

### Ajouter de la musique :

1. Créer GameObject vide : "MenuMusic"
2. Add Component > **MenuMusicManager**
3. Add Component > **AudioSource**
4. Glisser votre clip audio dans Menu Music
5. Play On Awake: ✓
6. Loop: ✓

## ⚠️ Points importants

- Vérifier que le nom de la scène dans MainMenuManager correspond ("Game")
- Désactiver SettingsPanel par défaut
- Les sliders de volume vont de 0 à 1
- Les paramètres se sauvent automatiquement dans PlayerPrefs

## 🎮 Test

1. Play dans Unity
2. Tester chaque bouton
3. Vérifier que les settings fonctionnent
4. Vérifier le bouton Quitter en build

Voilà ! Votre menu est prêt ! 🎉

