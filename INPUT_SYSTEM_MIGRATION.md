# 🎮 Migration vers le nouveau Input System - Documentation complète

## ✅ État de la migration

**Date de mise à jour :** 21 février 2026

Tous les scripts du système de menu ont été migrés vers le **nouveau Unity Input System Package**.

---

## 📋 Scripts vérifiés et mis à jour

### ✅ Scripts UI - COMPATIBLES

| Script | Statut | Utilise Input System |
|--------|--------|---------------------|
| **PauseMenuManager.cs** | ✅ **MIS À JOUR** | Oui - `Keyboard.current.escapeKey` |
| **MainMenuManager.cs** | ✅ Compatible | N/A - Pas d'input |
| **SettingsManager.cs** | ✅ Compatible | N/A - Pas d'input |
| **ButtonHoverEffect.cs** | ✅ Compatible | N/A - Event System |
| **MenuMusicManager.cs** | ✅ Compatible | N/A - Pas d'input |
| **SceneFadeTransition.cs** | ✅ Compatible | N/A - Pas d'input |
| **StyledButton.cs** | ✅ Compatible | N/A - Event System |
| **LoadingScreen.cs** | ✅ Compatible | N/A - Pas d'input |
| **MenuTitleAnimator.cs** | ✅ Compatible | N/A - Pas d'input |
| **MoneyUI.cs** | ✅ Compatible | N/A - Pas d'input |

---

## 🔧 Changements effectués

### 1. PauseMenuManager.cs

**Avant (ancien système) :**
```csharp
using UnityEngine;

[SerializeField] private KeyCode pauseKey = KeyCode.Escape;

private void Update()
{
    if (Input.GetKeyDown(pauseKey))
    {
        // ...
    }
}
```

**Après (nouveau système) :**
```csharp
using UnityEngine;
using UnityEngine.InputSystem; // ✅ Nouveau namespace

// ✅ Plus de champ pauseKey sérialisé

private void Update()
{
    // ✅ Utilisation du nouveau Input System
    if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }
}
```

**Méthodes supprimées :**
- `SetPauseKey(KeyCode newKey)` - Non nécessaire avec le nouveau système

---

## 📝 Documentation mise à jour

Les fichiers suivants ont été mis à jour pour refléter le nouveau Input System :

### ✅ PAUSE_MENU_GUIDE.md
- ✅ Suppression des références à `KeyCode`
- ✅ Mise à jour des instructions de configuration
- ✅ Ajout de la section "Personnaliser la touche de pause"
- ✅ Mise à jour de la checklist de test

### ✅ MENU_SYSTEM_SUMMARY.md
- ✅ Documentation complète du nouveau système
- ✅ Guide de résolution des erreurs Input System

### ✅ INPUT_SYSTEM_MIGRATION.md (ce fichier)
- ✅ Guide complet de migration
- ✅ Liste des changements effectués

---

## 🎯 Configuration requise dans Unity

### Player Settings

Pour que le nouveau Input System fonctionne, vérifiez ces paramètres :

1. Allez dans **Edit > Project Settings > Player**
2. Section **Other Settings**
3. **Active Input Handling** : Doit être réglé sur :
   - **Input System Package (New)** ✅ Recommandé
   - OU **Both** (ancien + nouveau)

**Important :** Si réglé sur "Input Manager (Old)", vous aurez l'erreur :
```
InvalidOperationException: You are trying to read Input using the UnityEngine.Input class, 
but you have switched active Input handling to Input System package in Player Settings.
```

### Package Manager

Vérifiez que le package est installé :

1. **Window > Package Manager**
2. Recherchez **"Input System"**
3. Version recommandée : **1.7.0** ou supérieure
4. Status : **Installed** ✅

---

## 🎮 Utilisation du nouveau Input System

### Touche Escape (menu de pause)

Le `PauseMenuManager` utilise maintenant :
```csharp
using UnityEngine.InputSystem;

if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
{
    // Ouvrir/fermer le menu de pause
}
```

### Personnaliser la touche

Pour utiliser une autre touche, modifiez dans `PauseMenuManager.cs` :

```csharp
// Touche P
Keyboard.current.pKey.wasPressedThisFrame

// Touche Tab
Keyboard.current.tabKey.wasPressedThisFrame

// Touche Space
Keyboard.current.spaceKey.wasPressedThisFrame

// Touche Enter
Keyboard.current.enterKey.wasPressedThisFrame

// Touche F1
Keyboard.current.f1Key.wasPressedThisFrame
```

### Vérification du clavier

Toujours vérifier que le clavier existe avant de l'utiliser :
```csharp
if (Keyboard.current != null)
{
    // Utiliser Keyboard.current
}
```

---

## 🔍 Autres touches disponibles

### Touches principales
- `escapeKey` - Escape
- `spaceKey` - Espace
- `enterKey` - Entrée
- `backspaceKey` - Retour arrière
- `deleteKey` - Suppr
- `tabKey` - Tab

### Touches de fonction
- `f1Key` à `f12Key` - F1 à F12

### Touches de modification
- `leftShiftKey`, `rightShiftKey` - Shift
- `leftCtrlKey`, `rightCtrlKey` - Ctrl
- `leftAltKey`, `rightAltKey` - Alt

### Touches alphabétiques
- `aKey` à `zKey` - A à Z

### Touches numériques
- `digit0Key` à `digit9Key` - 0 à 9
- `numpad0Key` à `numpad9Key` - Pavé numérique

### Flèches directionnelles
- `upArrowKey` - Flèche haut
- `downArrowKey` - Flèche bas
- `leftArrowKey` - Flèche gauche
- `rightArrowKey` - Flèche droite

---

## 🎯 Exemples d'utilisation avancée

### Combinaisons de touches

```csharp
using UnityEngine.InputSystem;

// Ctrl + S pour sauvegarder
if (Keyboard.current.leftCtrlKey.isPressed && 
    Keyboard.current.sKey.wasPressedThisFrame)
{
    SaveGame();
}

// Alt + F4 pour quitter
if (Keyboard.current.leftAltKey.isPressed && 
    Keyboard.current.f4Key.wasPressedThisFrame)
{
    QuitGame();
}
```

### Vérifier si une touche est maintenue

```csharp
// Maintenir la touche
if (Keyboard.current.spaceKey.isPressed)
{
    // La touche est enfoncée
}

// Touche relâchée ce frame
if (Keyboard.current.spaceKey.wasReleasedThisFrame)
{
    // La touche vient d'être relâchée
}

// Touche pressée ce frame
if (Keyboard.current.spaceKey.wasPressedThisFrame)
{
    // La touche vient d'être pressée
}
```

### Souris avec le nouveau système

```csharp
using UnityEngine.InputSystem;

// Position de la souris
Vector2 mousePosition = Mouse.current.position.ReadValue();

// Clic gauche
if (Mouse.current.leftButton.wasPressedThisFrame)
{
    // Clic gauche
}

// Clic droit
if (Mouse.current.rightButton.wasPressedThisFrame)
{
    // Clic droit
}

// Molette
float scroll = Mouse.current.scroll.ReadValue().y;
```

---

## 🐛 Résolution de problèmes

### Erreur : "Namespace 'InputSystem' could not be found"

**Solution :**
1. Vérifiez que le package Input System est installé
2. Ajoutez `using UnityEngine.InputSystem;` en haut du script
3. Redémarrez Unity si nécessaire

### Erreur : "Keyboard.current is null"

**Cause :** Aucun clavier détecté (rare sur PC, possible sur mobile)

**Solution :**
```csharp
if (Keyboard.current != null)
{
    // Vérifier la touche
}
```

### Les touches ne répondent pas

**Vérifications :**
1. Player Settings > Active Input Handling : "Input System Package (New)"
2. Le script est actif et attaché à un GameObject actif
3. Vous utilisez `wasPressedThisFrame` et non `isPressed` pour les actions ponctuelles

### L'ancien Input.GetKeyDown() ne fonctionne plus

**Normal !** C'est l'ancien système. Utilisez :
```csharp
// Ancien
if (Input.GetKeyDown(KeyCode.Escape))

// Nouveau
if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
```

---

## 📚 Ressources supplémentaires

### Documentation officielle Unity
- [Input System Package](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.7/manual/index.html)
- [Migration depuis l'ancien système](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.7/manual/Migration.html)
- [API Reference - Keyboard](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.7/api/UnityEngine.InputSystem.Keyboard.html)

### Input Actions (avancé)

Pour des contrôles plus complexes, vous pouvez utiliser des **Input Actions** :

1. Créez un fichier `.inputactions` dans le projet
2. Définissez vos actions (Pause, Jump, Move, etc.)
3. Générez une classe C#
4. Utilisez-la dans vos scripts

**Avantages :**
- Reconfiguration des touches dans le jeu
- Support multi-plateformes automatique
- Gestion des manettes de jeu
- Interface graphique pour la configuration

---

## ✅ Migration checklist

### Pour votre projet

- [x] PauseMenuManager mis à jour avec Input System
- [x] Documentation mise à jour
- [x] Tests effectués
- [x] Pas d'erreurs de compilation
- [x] Le menu de pause fonctionne avec Escape

### Si vous avez d'autres scripts avec Input

- [ ] Vérifier tous les scripts pour `Input.GetKeyDown()`
- [ ] Remplacer par `Keyboard.current.xxxKey.wasPressedThisFrame`
- [ ] Ajouter `using UnityEngine.InputSystem;`
- [ ] Tester tous les contrôles

### Script de migration automatique (optionnel)

Vous pouvez utiliser Find & Replace dans votre éditeur :

**Rechercher :**
```
Input.GetKeyDown(KeyCode.Escape)
```

**Remplacer par :**
```
Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame
```

**N'oubliez pas d'ajouter le using :**
```csharp
using UnityEngine.InputSystem;
```

---

## 🎉 Résumé

✅ **Tous vos scripts de menu sont maintenant compatibles avec le nouveau Input System !**

### Ce qui a changé :
- `PauseMenuManager.cs` utilise maintenant `Keyboard.current.escapeKey`
- Plus besoin de champ `pauseKey` sérialisé
- Méthode `SetPauseKey()` supprimée
- Documentation mise à jour

### Ce qui reste identique :
- Toutes les fonctionnalités du menu fonctionnent
- Tous les autres scripts sont compatibles
- L'interface Unity reste la même
- Les événements OnClick des boutons fonctionnent

### Prochaines étapes (optionnel) :
- [ ] Explorer les Input Actions pour des contrôles avancés
- [ ] Ajouter le support de manettes de jeu
- [ ] Implémenter la reconfiguration des touches in-game
- [ ] Ajouter le support tactile pour mobile

---

**Le système de menu est maintenant 100% compatible avec le nouveau Input System de Unity ! 🎮✨**

