# ⚡ AIDE-MÉMOIRE RAPIDE

## 🎯 EN 3 ÉTAPES

### 1️⃣ DANS UNITY (10 min)
```
File > New Scene > Save as "MainMenu"

Créer :
- Canvas
  - MainMenuPanel (3 boutons)
  - SettingsPanel (sliders + dropdowns)
```

### 2️⃣ ATTACHER SCRIPTS (5 min)
```
Canvas → MainMenuManager (assigner les panels)
GameObject → SettingsManager (assigner les UI)
```

### 3️⃣ CONNECTER BOUTONS (15 min)
```
PlayButton → MainMenuManager.PlayGame()
SettingsButton → MainMenuManager.OpenSettings()
QuitButton → MainMenuManager.QuitGame()

Sliders → SettingsManager.SetXxxVolume()
```

## ✅ C'EST PRÊT !

---

## 📍 SCRIPTS CRÉÉS

| Script | Fonction |
|--------|----------|
| MainMenuManager | Navigation menu |
| SettingsManager | Paramètres complets |
| PauseMenuManager | Menu pause in-game |
| ButtonHoverEffect | Effets visuels |
| MenuMusicManager | Musique fond |
| SceneFadeTransition | Transitions |
| LoadingScreen | Écran chargement |
| StyledButton | Stylisation |
| MenuTitleAnimator | Animation titre |

**📂 Emplacement :** `Assets/Scripts/UI/`

---

## 📚 GUIDES

| Besoin | Fichier |
|--------|---------|
| **Démarrer** | [README.md](README.md) |
| **Navigation** | [INDEX.md](INDEX.md) |
| **Config rapide** | [START_HERE.md](START_HERE.md) |
| **Guide complet** | [MENU_GUIDE.md](MENU_GUIDE.md) |
| **Menu pause** | [PAUSE_MENU_GUIDE.md](PAUSE_MENU_GUIDE.md) |

---

## 🐛 DÉPANNAGE EXPRESS

| Problème | Solution |
|----------|----------|
| **Bouton ne fonctionne pas** | Vérifier OnClick() connecté |
| **Scène ne charge pas** | Ajouter dans Build Settings |
| **Settings non sauvegardés** | Normal en éditeur, tester en build |
| **Pas de son** | Vérifier AudioSource assigné |

---

## 🎨 STRUCTURE UI MINIMALE

```
Canvas
├─ MainMenuPanel
│  ├─ PlayButton
│  ├─ SettingsButton
│  └─ QuitButton
└─ SettingsPanel [DÉSACTIVÉ]
   ├─ MasterVolumeSlider + Text
   ├─ QualityDropdown
   └─ BackButton
```

---

## ⚡ COMMANDES UNITY RAPIDES

- **Créer Canvas :** `Right Click > UI > Canvas`
- **Créer Bouton :** `Right Click > UI > Button - TextMeshPro`
- **Créer Panel :** `Right Click > UI > Panel`
- **Créer Slider :** `Right Click > UI > Slider`
- **Add Component :** `Inspector > Add Component`

---

## 🔥 CHECKLIST EXPRESS

- [ ] Scène MainMenu créée
- [ ] Canvas + 2 Panels créés
- [ ] Scripts attachés
- [ ] Boutons connectés
- [ ] Build Settings configuré
- [ ] Testé en Play Mode

---

## 🚀 LANCER LE JEU

1. **Play dans Unity** : Tester rapidement
2. **Build Settings** : File > Build Settings
3. **Add Scenes** : MainMenu (0) + Game (1)
4. **Build** : Créer l'exécutable

---

## 📞 BESOIN D'AIDE ?

**Lisez :** [START_HERE.md](START_HERE.md) section "Dépannage rapide"

---

**✨ Tout est prêt ! Il ne reste qu'à configurer dans Unity.**

**Temps total : 30 minutes**

🎮 Bon développement !

