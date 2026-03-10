# 🎨 Template de Préfabriqués Unity

## Créer des Prefabs réutilisables

Une fois votre menu configuré dans Unity, vous pouvez créer des préfabriqués (prefabs) pour réutiliser facilement les éléments.

## 📦 Prefabs recommandés à créer

### 1. MenuButton Prefab
**Chemin:** `Assets/Prefab/UI/MenuButton.prefab`

**Structure:**
```
MenuButton (Button)
├─ Image (Background)
└─ Text (TMP) (Label)
```

**Scripts attachés:**
- ButtonHoverEffect
- StyledButton

**Utilisation:**
Glissez ce prefab dans votre Canvas, changez le texte, et connectez l'événement OnClick.

---

### 2. VolumeSlider Prefab
**Chemin:** `Assets/Prefab/UI/VolumeSlider.prefab`

**Structure:**
```
VolumeSliderGroup (Empty)
├─ Label (TextMeshPro) → "Volume Master:"
├─ Slider (Slider)
│  ├─ Background
│  ├─ Fill Area
│  │  └─ Fill
│  └─ Handle Slide Area
│     └─ Handle
└─ ValueText (TextMeshPro) → "100%"
```

**Utilisation:**
Parfait pour les 3 sliders de volume (Master, Music, SFX).

---

### 3. SettingsDropdown Prefab
**Chemin:** `Assets/Prefab/UI/SettingsDropdown.prefab`

**Structure:**
```
DropdownGroup (Empty)
├─ Label (TextMeshPro) → "Qualité:"
└─ Dropdown (TMP_Dropdown)
```

**Utilisation:**
Réutilisable pour Qualité, Résolution, etc.

---

### 4. MenuPanel Prefab
**Chemin:** `Assets/Prefab/UI/MenuPanel.prefab`

**Structure:**
```
MenuPanel (Panel)
├─ PanelBackground (Image)
├─ Title (TextMeshPro)
└─ Content (Vertical Layout Group)
```

**Utilisation:**
Base pour créer rapidement de nouveaux panneaux (crédits, options avancées, etc.).

---

### 5. FadeTransition Prefab
**Chemin:** `Assets/Prefab/Systems/FadeTransition.prefab`

**Structure:**
```
FadeTransition [DontDestroyOnLoad]
└─ Canvas
   └─ FadeImage (Image - plein écran noir)
```

**Scripts attachés:**
- SceneFadeTransition

**Configuration:**
- Canvas: Overlay
- Sort Order: 999 (au-dessus de tout)
- FadeImage: Stretch both, Color noir, Alpha 0

---

### 6. MenuMusicManager Prefab
**Chemin:** `Assets/Prefab/Systems/MenuMusicManager.prefab`

**Structure:**
```
MenuMusicManager [DontDestroyOnLoad]
└─ AudioSource
```

**Scripts attachés:**
- MenuMusicManager

**Configuration:**
- AudioSource: Loop activé, Volume 0.5

---

### 7. LoadingScreen Prefab
**Chemin:** `Assets/Prefab/UI/LoadingScreen.prefab`

**Structure:**
```
LoadingScreen [DontDestroyOnLoad]
└─ Canvas (Overlay, Sort Order: 1000)
   └─ LoadingPanel (Panel - plein écran)
      ├─ Background (Image - noir opaque)
      ├─ LoadingText (TextMeshPro) → "Chargement..."
      ├─ ProgressBar (Slider)
      └─ PercentageText (TextMeshPro) → "0%"
```

**Scripts attachés:**
- LoadingScreen

---

## 🎯 Comment créer un Prefab

### Méthode simple :

1. **Créer l'élément UI** dans votre hiérarchie
2. **Configurer** tous les composants
3. **Glisser** depuis la Hierarchy vers le dossier Project
4. Un fichier `.prefab` est créé

### Utilisation d'un Prefab :

1. **Glisser** le prefab depuis Project vers Hierarchy
2. **Modifier** les valeurs si nécessaire
3. Les scripts et configurations sont déjà en place !

---

## 🔧 Variants de Prefabs

Vous pouvez créer des **variantes** pour avoir différents styles :

### Exemple : Boutons stylés

**MenuButton_Default** (bleu)
- Normal: Bleu clair
- Hover: Bleu vif
- Pressed: Bleu foncé

**MenuButton_Success** (vert)
- Normal: Vert clair
- Hover: Vert vif
- Pressed: Vert foncé

**MenuButton_Danger** (rouge)
- Normal: Rouge clair
- Hover: Rouge vif
- Pressed: Rouge foncé

### Créer une variante :
1. Faites un clic droit sur le prefab
2. `Create > Prefab Variant`
3. Modifiez les couleurs/valeurs
4. Le variant hérite du prefab parent

---

## 📐 Layout Groups recommandés

### Vertical Layout Group
Pour organiser les boutons verticalement :
```
ButtonContainer (Empty + Vertical Layout Group)
├─ PlayButton
├─ SettingsButton
└─ QuitButton
```

**Settings:**
- Spacing: 20
- Child Force Expand: Width
- Child Control Size: Width et Height

### Grid Layout Group
Pour organiser des options en grille :
```
OptionsGrid (Empty + Grid Layout Group)
├─ Option1
├─ Option2
├─ Option3
└─ Option4
```

**Settings:**
- Cell Size: 200x50
- Spacing: 10, 10
- Start Axis: Horizontal

---

## 🎨 Thèmes de couleurs suggérés

### Thème Sombre (moderne)
```
Background: #1A1A2E
Primary: #16213E
Secondary: #0F3460
Accent: #E94560
Text: #FFFFFF
```

### Thème Médiéval (fantasy)
```
Background: #2C1810
Primary: #5C3D2E
Secondary: #8B6F47
Accent: #D4AF37
Text: #F5E6D3
```

### Thème Clair (clean)
```
Background: #F0F0F0
Primary: #FFFFFF
Secondary: #E0E0E0
Accent: #2196F3
Text: #212121
```

---

## 🚀 Organisation des dossiers

```
Assets/
├─ Prefab/
│  ├─ UI/
│  │  ├─ Buttons/
│  │  │  ├─ MenuButton.prefab
│  │  │  ├─ MenuButton_Success.prefab
│  │  │  └─ MenuButton_Danger.prefab
│  │  ├─ Panels/
│  │  │  ├─ MenuPanel.prefab
│  │  │  └─ SettingsPanel.prefab
│  │  ├─ Controls/
│  │  │  ├─ VolumeSlider.prefab
│  │  │  └─ SettingsDropdown.prefab
│  │  └─ Screens/
│  │     └─ LoadingScreen.prefab
│  └─ Systems/
│     ├─ FadeTransition.prefab
│     └─ MenuMusicManager.prefab
```

---

## 💡 Conseils de conception

### 1. Cohérence visuelle
- Utilisez la même police partout
- Gardez les mêmes espacements
- Couleurs cohérentes

### 2. Accessibilité
- Taille de texte minimum : 24pt
- Contraste élevé texte/fond
- Boutons assez grands (min 200x50)

### 3. Feedback utilisateur
- Hover effects sur tous les boutons
- Sons de clic
- Animations subtiles
- États visuels clairs

### 4. Performance
- Pas trop d'animations simultanées
- Optimiser les images (compression)
- Atlas de sprites pour les icônes

---

## 🎮 Checklist avant Build

- [ ] Tous les prefabs sont correctement configurés
- [ ] Les scènes sont dans Build Settings
- [ ] Les transitions fonctionnent
- [ ] Les sons sont assignés
- [ ] Le bouton Quitter fonctionne
- [ ] Les settings se sauvegardent
- [ ] L'UI est responsive sur différentes résolutions
- [ ] Pas d'erreurs dans la console
- [ ] Performance acceptable (60 FPS)
- [ ] Tests sur la plateforme cible

---

**Note:** Créer des prefabs vous fera gagner énormément de temps ! Vous pourrez réutiliser vos éléments dans tous vos projets Unity.

Bon design ! 🎨✨

