# 🎮 Système de Menu - Village Defender

## ✅ Problème résolu : Input System Error

**Erreur corrigée :** `InvalidOperationException: You are trying to read Input using the UnityEngine.Input class`

### Ce qui a été fait :
- ✅ Mis à jour `PauseMenuManager.cs` pour utiliser le nouveau Input System (`UnityEngine.InputSystem`)
- ✅ Remplacé `Input.GetKeyDown(KeyCode.Escape)` par `Keyboard.current.escapeKey.wasPressedThisFrame`
- ✅ Supprimé les références à l'ancien système d'Input
- ✅ Le menu de pause fonctionne maintenant avec la touche **Escape**

---

## 🎯 Système de Menu Complet

### Scripts disponibles (Assets/Scripts/UI/)

#### 1. **MainMenuManager.cs** - Menu Principal
Fonctions principales :
- `PlayGame()` - Lance une nouvelle partie
- `OpenSettings()` - Ouvre le panneau des paramètres
- `QuitGame()` - Quitte l'application
- `ShowMainMenu()` - Retour au menu principal

#### 2. **PauseMenuManager.cs** - Menu de Pause
Fonctions principales :
- `Pause()` - Met le jeu en pause (touche Escape)
- `Resume()` - Reprend le jeu
- `OpenSettings()` - Ouvre les paramètres
- `RestartLevel()` - Redémarre le niveau
- `ReturnToMainMenu()` - Retour au menu principal
- `QuitGame()` - Quitte l'application

#### 3. **SettingsManager.cs** - Gestion des Paramètres
Fonctionnalités :
- Contrôle du volume (Master, Musique, SFX)
- Gestion de la qualité graphique
- Changement de résolution
- Mode plein écran
- Sauvegarde automatique dans PlayerPrefs

#### 4. Scripts Utilitaires
- **ButtonHoverEffect.cs** - Effet au survol des boutons
- **MenuMusicManager.cs** - Musique de fond persistante
- **SceneFadeTransition.cs** - Transitions entre scènes
- **StyledButton.cs** - Stylisation des boutons

---

## 🚀 Comment utiliser le système de menu

### Option 1 : Utiliser le menu existant
Si la scène MainMenu est déjà configurée :
1. Ouvrez la scène `Assets/Scenes/MainMenu.unity`
2. Appuyez sur Play
3. Les boutons devraient fonctionner :
   - **Jouer** → Lance la scène "Game"
   - **Paramètres** → Ouvre les options
   - **Quitter** → Ferme l'application

### Option 2 : Créer un nouveau menu depuis zéro

#### Étape 1 : Créer la scène
1. Dans Unity : `File > New Scene`
2. Sauvegarder : `Assets/Scenes/MainMenu.unity`

#### Étape 2 : Créer l'UI
1. Clic droit dans Hierarchy → `UI > Canvas`
2. Créer la structure suivante :

```
Canvas
├─ MainMenuPanel
│  ├─ Title (TextMeshPro)
│  ├─ PlayButton (Button)
│  ├─ SettingsButton (Button)
│  └─ QuitButton (Button)
│
└─ SettingsPanel (décoché dans l'Inspector)
   ├─ Title (TextMeshPro)
   ├─ MasterVolumeSlider (Slider + Text)
   ├─ MusicVolumeSlider (Slider + Text)
   ├─ SFXVolumeSlider (Slider + Text)
   ├─ QualityDropdown (Dropdown)
   ├─ ResolutionDropdown (Dropdown)
   ├─ FullscreenToggle (Toggle)
   └─ BackButton (Button)
```

#### Étape 3 : Attacher les scripts
1. Sélectionnez le **Canvas**
2. Add Component → **MainMenuManager**
3. Dans l'Inspector :
   - Glissez **MainMenuPanel** dans le champ "Main Menu Panel"
   - Glissez **SettingsPanel** dans le champ "Settings Panel"
   - Entrez "Game" dans "Game Scene Name"

4. Créez un GameObject vide nommé "SettingsManager"
5. Add Component → **SettingsManager**
6. Connectez tous les sliders, dropdowns et toggles

#### Étape 4 : Connecter les boutons

**MainMenuPanel - Boutons :**
- **PlayButton** → OnClick() → MainMenuManager.PlayGame
- **SettingsButton** → OnClick() → MainMenuManager.OpenSettings
- **QuitButton** → OnClick() → MainMenuManager.QuitGame

**SettingsPanel - Contrôles :**
- **MasterVolumeSlider** → OnValueChanged() → SettingsManager.SetMasterVolume
- **MusicVolumeSlider** → OnValueChanged() → SettingsManager.SetMusicVolume
- **SFXVolumeSlider** → OnValueChanged() → SettingsManager.SetSFXVolume
- **QualityDropdown** → OnValueChanged() → SettingsManager.SetQuality
- **ResolutionDropdown** → OnValueChanged() → SettingsManager.SetResolution
- **FullscreenToggle** → OnValueChanged() → SettingsManager.SetFullscreen
- **BackButton** → OnClick() → MainMenuManager.ShowMainMenu

#### Étape 5 : Configuration Build Settings
1. `File > Build Settings`
2. Ajouter les scènes dans cet ordre :
   - **0:** MainMenu
   - **1:** Game (ou votre scène de jeu)

---

## 🎮 Menu de Pause - Setup

### Dans votre scène de jeu (Game.unity)

1. Créer l'UI de pause :
```
Canvas
├─ PauseMenuPanel (décoché)
│  ├─ Title (TextMeshPro) - "PAUSE"
│  ├─ ResumeButton
│  ├─ SettingsButton
│  ├─ RestartButton
│  ├─ MainMenuButton
│  └─ QuitButton
│
└─ SettingsPanel (décoché)
   └─ [Même structure que le menu principal]
```

2. Attacher le script :
   - Sélectionnez le **Canvas**
   - Add Component → **PauseMenuManager**
   - Connectez les panneaux

3. Connecter les boutons :
   - **ResumeButton** → PauseMenuManager.Resume
   - **SettingsButton** → PauseMenuManager.OpenSettings
   - **RestartButton** → PauseMenuManager.RestartLevel
   - **MainMenuButton** → PauseMenuManager.ReturnToMainMenu
   - **QuitButton** → PauseMenuManager.QuitGame

4. **Appuyez sur Escape pendant le jeu** pour ouvrir le menu de pause !

**Note :** Le menu utilise le nouveau Input System de Unity (`Keyboard.current.escapeKey`). Assurez-vous que Player Settings > Active Input Handling est réglé sur "Input System Package (New)" ou "Both".

---

## 🎨 Personnalisation

### Ajouter un effet de hover sur les boutons
1. Sélectionnez un bouton
2. Add Component → **ButtonHoverEffect**
3. Configurez :
   - Scale On Hover : 1.1
   - Scale Duration : 0.2
   - Cochez "Play Sound On Hover" (optionnel)

### Ajouter de la musique
1. Créez un GameObject vide nommé "MenuMusicManager"
2. Add Component → **MenuMusicManager**
3. Glissez votre clip audio dans "Background Music"
4. Cochez "Play On Awake"

### Ajouter des transitions fluides
1. Créez un Canvas avec un Panel noir (alpha 0)
2. Ajoutez **SceneFadeTransition** sur ce Canvas
3. Dans MainMenuManager, cochez "Use Fade Transition"

---

## 📋 Checklist de vérification

### Menu Principal
- [ ] Le bouton "Jouer" lance la scène de jeu
- [ ] Le bouton "Paramètres" ouvre le panneau des options
- [ ] Le bouton "Quitter" ferme l'application
- [ ] Le panneau de paramètres se ferme correctement

### Paramètres
- [ ] Les sliders de volume fonctionnent
- [ ] Le dropdown de qualité change les paramètres graphiques
- [ ] Le dropdown de résolution change la résolution
- [ ] Le toggle plein écran fonctionne
- [ ] Les paramètres sont sauvegardés (testez en relançant)

### Menu de Pause
- [ ] La touche **Escape** ouvre/ferme le menu
- [ ] Le temps se met en pause (Time.timeScale = 0)
- [ ] Le bouton "Reprendre" ferme le menu
- [ ] Le bouton "Recommencer" recharge le niveau
- [ ] Le bouton "Menu Principal" retourne au menu
- [ ] Le curseur se déverrouille pendant la pause

---

## 🐛 Problèmes courants

### "Scene 'Game' couldn't be loaded"
**Solution :** Vérifiez que la scène existe dans Build Settings (`File > Build Settings`)

### Le bouton Quitter ne fait rien en Editor
**Normal !** Il fonctionne uniquement dans le build. En Editor, il stoppe le Play Mode.

### Les paramètres ne se sauvent pas
**Solution :** PlayerPrefs fonctionne mieux dans un build. Testez en buildant le projet.

### Le menu de pause ne s'ouvre pas
**Vérifications :**
- Le script PauseMenuManager est bien attaché
- Les panels sont bien assignés dans l'Inspector
- Vous appuyez bien sur **Escape** (pas une autre touche)
- Player Settings > Active Input Handling : "Input System Package (New)" ou "Both"

### Input System Error
**Erreur :** `InvalidOperationException: You are trying to read Input using the UnityEngine.Input class`

**Solution :** Ce problème est maintenant corrigé ! Le PauseMenuManager utilise le nouveau Input System. Assurez-vous que :
1. Le package Input System est installé (Window > Package Manager)
2. Player Settings > Active Input Handling = "Input System Package (New)"
3. Le script contient `using UnityEngine.InputSystem;`

---

## 📚 Documentation complète

Pour plus de détails, consultez :
- **MENU_GUIDE.md** - Guide détaillé complet
- **QUICK_SETUP.md** - Guide de configuration rapide
- **README_MENU.md** - Vue d'ensemble du système

---

## ✨ Résumé

Vous disposez maintenant d'un système de menu complet avec :
- ✅ Menu principal fonctionnel (Jouer, Paramètres, Quitter)
- ✅ Menu de pause avec touche Escape
- ✅ Gestion complète des paramètres (audio, graphismes)
- ✅ Sauvegarde automatique des préférences
- ✅ Scripts optimisés et commentés
- ✅ Compatible avec le nouveau Input System de Unity

**Prêt à l'emploi !** 🎮

---

*Dernière mise à jour : Février 2026*

