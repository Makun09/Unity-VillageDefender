# ⏸️ Guide Menu de Pause

## Vue d'ensemble

Le script **PauseMenuManager.cs** a été créé pour gérer un menu de pause pendant le jeu. Il s'intègre parfaitement avec les autres scripts du système de menu.

## 🎮 Fonctionnalités

- ✅ Pause/reprise du jeu avec la touche **Escape** (nouveau Input System)
- ✅ Arrêt du temps pendant la pause (Time.timeScale = 0)
- ✅ Panneau de pause avec boutons
- ✅ Accès aux paramètres depuis la pause
- ✅ Redémarrage du niveau
- ✅ Retour au menu principal
- ✅ Support des transitions fade
- ✅ Gestion du curseur
- ✅ Compatible avec Unity Input System Package

## 📐 Configuration dans Unity

### 1. Créer l'UI du menu de pause

Dans votre scène de jeu (Game.unity), ajoutez :

```
Canvas
└─ PauseMenuUI (Panel)
   ├─ PausePanel (Panel)
   │  ├─ Title (TextMeshPro) → "PAUSE"
   │  ├─ ResumeButton → "REPRENDRE"
   │  ├─ SettingsButton → "PARAMÈTRES"
   │  ├─ RestartButton → "REDÉMARRER"
   │  ├─ MainMenuButton → "MENU PRINCIPAL"
   │  └─ QuitButton → "QUITTER"
   │
   └─ SettingsPanelInGame (Panel - DÉSACTIVÉ)
      └─ [Même structure que SettingsPanel du menu principal]
         ├─ Volume sliders
         ├─ Dropdowns
         └─ BackButton → "RETOUR"
```

### 2. Attacher le script

1. Créez un GameObject vide dans la scène : "PauseMenuManager"
2. Add Component > **PauseMenuManager**
3. Assignez dans l'Inspector :
   - **Pause Menu Panel** : PausePanel
   - **Settings Panel** : SettingsPanelInGame
   - **Pause Time Scale** : ✓ (coché)

**Note :** Le menu de pause s'ouvre automatiquement avec la touche **Escape** grâce au nouveau Input System de Unity.

### 3. Connecter les boutons

**ResumeButton > On Click() :**
- `PauseMenuManager > Resume()`

**SettingsButton > On Click() :**
- `PauseMenuManager > OpenSettings()`

**RestartButton > On Click() :**
- `PauseMenuManager > RestartLevel()`

**MainMenuButton > On Click() :**
- `PauseMenuManager > ReturnToMainMenu()`

**QuitButton > On Click() :**
- `PauseMenuManager > QuitGame()`

**BackButton (dans Settings) > On Click() :**
- `PauseMenuManager > BackToPauseMenu()`

### 4. Configuration du Canvas

Le Canvas doit être configuré ainsi :
- **Render Mode** : Screen Space - Overlay
- **Canvas Scaler** : Scale With Screen Size
- **Reference Resolution** : 1920 x 1080

Le PausePanel doit être désactivé par défaut (décoché dans Inspector).

## 🎨 Style recommandé pour le menu de pause

### Overlay semi-transparent

Pour l'effet "flou" derrière le menu :

1. Sur PauseMenuUI, ajoutez une Image :
   - Color : Noir
   - Alpha : 0.7 (70% opacité)
   - Raycast Target : ✓ (pour bloquer les clics)

2. Le PausePanel peut avoir un fond plus clair :
   - Color : Gris foncé (#2A2A2A)
   - Alpha : 1

### Animation d'apparition (optionnel)

Créez un Animator pour animer l'apparition du menu :

```csharp
// Exemple d'animation simple dans le code
void Pause()
{
    // ... code existant ...
    pauseMenuPanel.transform.localScale = Vector3.zero;
    LeanTween.scale(pauseMenuPanel, Vector3.one, 0.3f)
        .setEaseOutBack()
        .setIgnoreTimeScale(true); // Important pour fonctionner en pause
}
```

## 🔧 Intégration avec SettingsManager

Si vous avez déjà un SettingsManager sur le menu principal, vous pouvez :

### Option 1 : SettingsManager séparé pour le jeu

Créez un second SettingsManager dans votre scène de jeu, attaché à SettingsPanelInGame.

### Option 2 : SettingsManager persistant

Transformez votre SettingsManager en Singleton qui persiste :

```csharp
// Dans SettingsManager.cs, ajoutez dans Awake()
private static SettingsManager instance;

void Awake()
{
    if (instance == null)
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    else if (instance != this)
    {
        Destroy(gameObject);
    }
}
```

## 🎯 Utilisation avancée

### Vérifier si le jeu est en pause

Depuis un autre script :

```csharp
PauseMenuManager pauseManager = FindObjectOfType<PauseMenuManager>();
if (pauseManager != null && pauseManager.IsPaused())
{
    // Le jeu est en pause
}
```

### Personnaliser la touche de pause

Le menu de pause utilise la touche **Escape** par défaut avec le nouveau Input System. Si vous souhaitez utiliser une autre touche, vous pouvez modifier le script `PauseMenuManager.cs` :

```csharp
// Dans la méthode Update(), remplacez :
if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)

// Par exemple, pour utiliser la touche P :
if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
```

**Touches disponibles :** `spaceKey`, `pKey`, `tabKey`, `enterKey`, etc.

### Empêcher certains scripts de fonctionner en pause

Si vous ne voulez pas que Time.timeScale affecte certains éléments :

```csharp
// Utilisez Time.unscaledDeltaTime au lieu de Time.deltaTime
transform.Rotate(Vector3.up * rotationSpeed * Time.unscaledDeltaTime);
```

## 📱 Gestion du curseur

Le script gère automatiquement le curseur :
- **En pause** : Curseur visible et déverrouillé
- **En jeu** : (optionnel) Curseur verrouillé

Si votre jeu nécessite un curseur verrouillé, décommentez ces lignes dans `Resume()` :

```csharp
Cursor.lockState = CursorLockMode.Locked;
Cursor.visible = false;
```

## ⚠️ Points importants

### Time.timeScale

Quand le jeu est en pause, `Time.timeScale = 0`, ce qui signifie :
- Les animations s'arrêtent
- Les mouvements s'arrêtent
- Les timers s'arrêtent
- L'audio peut s'arrêter

**Solutions :**
- Utilisez `Time.unscaledDeltaTime` pour ce qui doit continuer
- Pour l'audio, utilisez `AudioSource.ignoreListenerPause = true`

### Audio pendant la pause

Pour que la musique continue pendant la pause :

```csharp
AudioSource musicSource = GetComponent<AudioSource>();
musicSource.ignoreListenerPause = true;
```

### Animations UI pendant la pause

Pour les animations UI, utilisez :

```csharp
// Dans un Animator
animator.updateMode = AnimatorUpdateMode.UnscaledTime;
```

## 🎮 Exemple de hiérarchie complète

```
Game Scene
├─ Canvas (Main UI)
│  ├─ MoneyUI
│  ├─ HealthUI
│  └─ PauseMenuUI
│     ├─ DarkOverlay (Image semi-transparente)
│     ├─ PausePanel [DÉSACTIVÉ]
│     │  ├─ Title
│     │  └─ Buttons (Vertical Layout Group)
│     │     ├─ ResumeButton
│     │     ├─ SettingsButton
│     │     ├─ RestartButton
│     │     ├─ MainMenuButton
│     │     └─ QuitButton
│     └─ SettingsPanelInGame [DÉSACTIVÉ]
│        ├─ BackButton
│        └─ Settings Content
│
├─ PauseMenuManager (Empty GameObject)
│  └─ PauseMenuManager (Script)
│
└─ SettingsManager (Empty GameObject)
   └─ SettingsManager (Script)
```

## 🚀 Test de fonctionnement

### Checklist :

- [ ] Appuyer sur **Escape** met en pause
- [ ] Le temps s'arrête (les objets ne bougent plus)
- [ ] Le menu de pause s'affiche
- [ ] Le bouton Reprendre fonctionne
- [ ] Le bouton Settings ouvre les paramètres
- [ ] Le bouton Retour revient au menu pause
- [ ] Le bouton Redémarrer recharge la scène
- [ ] Le bouton Menu Principal retourne au menu
- [ ] Le bouton Quitter fonctionne
- [ ] Le curseur est visible pendant la pause
- [ ] On peut cliquer sur les boutons
- [ ] Le nouveau Input System est actif dans Player Settings

## 💡 Conseils de design

### Visual Feedback

Ajoutez un effet de flou sur le gameplay :
- Utilisez un Post-Processing Volume
- Activez le Depth of Field pendant la pause

### Sons

Ajoutez des sons pour :
- L'ouverture du menu de pause
- La fermeture du menu
- Les clics sur les boutons

### Confirmation

Pour des actions critiques (Quitter, Redémarrer), ajoutez une confirmation :

```csharp
public void ShowConfirmQuit()
{
    // Afficher un panneau de confirmation
    confirmPanel.SetActive(true);
}

public void ConfirmQuit()
{
    QuitGame();
}

public void CancelQuit()
{
    confirmPanel.SetActive(false);
}
```

## 🎓 Intégration avec les autres systèmes

Le PauseMenuManager s'intègre parfaitement avec :
- **MainMenuManager** : Même structure, même logique
- **SceneFadeTransition** : Support automatique des transitions
- **SettingsManager** : Paramètres accessibles en pause
- **LoadingScreen** : Peut être utilisé pour le redémarrage

## 📝 Résumé

✅ **1 script créé** : PauseMenuManager.cs
✅ **Fonctionnalités complètes** : Pause, Settings, Restart, Quit
✅ **Intégration facile** : Compatible avec le système de menu principal
✅ **Personnalisable** : Touche de pause, gestion du temps, etc.

**Temps de configuration : 15-20 minutes**

Bon jeu ! 🎮⏸️

