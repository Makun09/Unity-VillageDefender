# 🎮 Guide de Configuration du Menu Principal

Ce guide vous explique comment créer et configurer un menu principal pour votre jeu Unity Village Defender.

## 📋 Scripts créés

Les scripts suivants ont été créés dans `Assets/Scripts/UI/` :

1. **MainMenuManager.cs** - Gère la navigation entre les panneaux du menu
2. **SettingsManager.cs** - Gère tous les paramètres du jeu (audio, graphismes, etc.)
3. **ButtonHoverEffect.cs** - Ajoute des effets visuels aux boutons (optionnel)

## 🎯 Configuration dans Unity

### Étape 1 : Créer la scène du menu

1. Dans Unity, créez une nouvelle scène : `File > New Scene`
2. Sauvegardez-la sous le nom **"MainMenu"** dans `Assets/Scenes/`

### Étape 2 : Créer l'interface utilisateur

#### A. Créer le Canvas principal

1. `Right Click` dans la Hierarchy > `UI > Canvas`
2. Sur le Canvas, configurez :
   - Canvas Scaler > UI Scale Mode : **Scale With Screen Size**
   - Reference Resolution : **1920 x 1080**

#### B. Créer le panneau du menu principal

1. `Right Click` sur Canvas > `UI > Panel`
2. Renommez-le en **"MainMenuPanel"**
3. Ajoutez une image de fond si vous le souhaitez

#### C. Créer les 3 boutons principaux

Pour chaque bouton :

1. `Right Click` sur MainMenuPanel > `UI > Button - TextMeshPro`
2. Renommez les boutons :
   - **"PlayButton"**
   - **"SettingsButton"**
   - **"QuitButton"**

3. Pour chaque bouton, modifiez le texte (TextMeshPro child) :
   - PlayButton : "JOUER" ou "LANCER PARTIE"
   - SettingsButton : "PARAMÈTRES"
   - QuitButton : "QUITTER"

4. Positionnez les boutons verticalement au centre de l'écran

**Optionnel** : Ajoutez le composant `ButtonHoverEffect` à chaque bouton pour un effet visuel

#### D. Créer le panneau des paramètres

1. `Right Click` sur Canvas > `UI > Panel`
2. Renommez-le en **"SettingsPanel"**
3. **Décochez** le GameObject dans l'Inspector pour le rendre inactif par défaut

##### Contenu du panneau Settings :

**Audio Settings (Sliders pour le volume) :**
1. `Right Click` sur SettingsPanel > `UI > Slider`
2. Créez 3 sliders : "MasterVolumeSlider", "MusicVolumeSlider", "SFXVolumeSlider"
3. Pour chaque slider :
   - Min Value : 0
   - Max Value : 1
   - Value : 1
4. Ajoutez un TextMeshPro à côté de chaque slider pour afficher le pourcentage

**Graphics Settings :**
1. `Right Click` sur SettingsPanel > `UI > Dropdown - TextMeshPro`
2. Créez : "QualityDropdown" pour les niveaux de qualité
3. Créez : "ResolutionDropdown" pour les résolutions
4. `Right Click` sur SettingsPanel > `UI > Toggle` : "FullscreenToggle"

**Bouton Retour :**
1. `Right Click` sur SettingsPanel > `UI > Button - TextMeshPro`
2. Renommez en "BackButton"
3. Texte : "RETOUR"

### Étape 3 : Connecter les scripts

#### A. MainMenuManager

1. Sélectionnez le Canvas (ou créez un GameObject vide nommé "MenuManager")
2. `Add Component` > **MainMenuManager**
3. Dans l'Inspector, assignez :
   - Main Menu Panel : glissez **MainMenuPanel**
   - Settings Panel : glissez **SettingsPanel**

#### B. Configurer les boutons du menu principal

Pour chaque bouton, dans le composant Button > On Click() :

**PlayButton :**
- Cliquez sur le `+`
- Glissez l'objet avec **MainMenuManager**
- Fonction : `MainMenuManager > PlayGame()`

**SettingsButton :**
- Cliquez sur le `+`
- Glissez l'objet avec **MainMenuManager**
- Fonction : `MainMenuManager > OpenSettings()`

**QuitButton :**
- Cliquez sur le `+`
- Glissez l'objet avec **MainMenuManager**
- Fonction : `MainMenuManager > QuitGame()`

#### C. SettingsManager

1. Créez un GameObject vide sur le Canvas : "SettingsManager"
2. `Add Component` > **SettingsManager**
3. Dans l'Inspector, assignez tous les éléments UI :
   - Master Volume Slider : glissez votre slider
   - Master Volume Text : glissez le TextMeshPro correspondant
   - (Répétez pour Music et SFX)
   - Quality Dropdown : glissez votre dropdown
   - Fullscreen Toggle : glissez votre toggle
   - Resolution Dropdown : glissez votre dropdown

#### D. Connecter les éléments Settings

Pour chaque slider, dans le composant Slider > On Value Changed() :
- **MasterVolumeSlider** → `SettingsManager > SetMasterVolume(float)`
- **MusicVolumeSlider** → `SettingsManager > SetMusicVolume(float)`
- **SFXVolumeSlider** → `SettingsManager > SetSFXVolume(float)`

Pour les autres éléments :
- **QualityDropdown** → On Value Changed : `SettingsManager > SetQuality(int)`
- **FullscreenToggle** → On Value Changed : `SettingsManager > SetFullscreen(bool)`
- **ResolutionDropdown** → On Value Changed : `SettingsManager > SetResolution(int)`

Pour le bouton Retour :
- **BackButton** → On Click : `MainMenuManager > ShowMainMenu()`

### Étape 4 : Configuration du Build

1. Allez dans `File > Build Settings`
2. Cliquez sur `Add Open Scenes` avec votre scène MainMenu ouverte
3. Glissez **MainMenu** en **position 0** (première scène)
4. Assurez-vous que la scène **"Game"** est également dans la liste

**Important :** Dans le script `MainMenuManager.cs`, vérifiez que le nom de la scène dans `PlayGame()` correspond au nom exact de votre scène de jeu.

## 🎨 Personnalisation

### Améliorer le design

- Ajoutez un titre du jeu en haut (TextMeshPro avec une grande police)
- Ajoutez une image de fond personnalisée
- Personnalisez les couleurs des boutons dans `Button > Colors`
- Ajoutez des effets de particules pour le fond
- Ajoutez de la musique d'ambiance (AudioSource sur le Canvas)

### Ajouter des sons

1. Importez vos clips audio dans Unity
2. Sur chaque bouton, ajoutez un AudioSource ou utilisez le `ButtonHoverEffect`
3. Dans Button > On Click, ajoutez `AudioSource > PlayOneShot(AudioClip)`

## 🔧 Fonctionnalités incluses

✅ Navigation entre menu principal et paramètres
✅ Lancement du jeu
✅ Quitter l'application (fonctionne en build et en éditeur)
✅ Contrôle du volume (Master, Musique, SFX)
✅ Sélection de la qualité graphique
✅ Basculer en plein écran
✅ Sélection de la résolution
✅ Sauvegarde automatique des paramètres (PlayerPrefs)
✅ Effets visuels sur les boutons (hover)

## 📝 Notes importantes

- Les paramètres sont sauvegardés automatiquement dans PlayerPrefs
- Le bouton Quitter fonctionne différemment en éditeur et en build
- Assurez-vous que le nom de votre scène de jeu correspond dans le code
- Vous pouvez personnaliser tous les éléments visuels selon votre style

## 🐛 Dépannage

**Le bouton Play ne charge pas la scène :**
- Vérifiez que la scène "Game" est bien dans Build Settings
- Vérifiez le nom exact de la scène dans le code

**Les paramètres ne se sauvegardent pas :**
- Les PlayerPrefs fonctionnent uniquement dans un build, pas toujours en éditeur

**Les sliders ne changent rien :**
- Vérifiez que les événements On Value Changed sont bien connectés
- Vérifiez que le SettingsManager est assigné

Bon développement ! 🚀

