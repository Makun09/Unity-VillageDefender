# Guide de Configuration UI - Village Defender

## 🔧 Problèmes à Corriger

### 1. **Écran de Chargement trop petit**
### 2. **Panneau Settings avec éléments superposés**

---

## 📋 Solution 1 : Écran de Chargement Plein Écran

### Configuration du Canvas LoadingScreen

1. **Vérifier le Canvas**
   - Dans la hiérarchie, sélectionnez le GameObject `LoadingScreenCanvas` (ou similaire)
   - Dans l'Inspector, vérifiez les paramètres du **Canvas** :
     - ✅ **Render Mode** : `Screen Space - Overlay`
     - ✅ **Canvas Scaler** : 
       - UI Scale Mode : `Scale With Screen Size`
       - Reference Resolution : `1920 x 1080`
       - Screen Match Mode : `Match Width Or Height`
       - Match : `0.5` (milieu entre Width et Height)

2. **Configuration du Panel de Chargement**
   - Sélectionnez le GameObject `loadingScreenPanel` (celui référencé dans LoadingScreen.cs)
   - Dans l'Inspector, configurez le **Rect Transform** :
     - Cliquez sur l'icône d'ancrage (en haut à gauche du Rect Transform)
     - Maintenez **Alt + Shift** et cliquez sur **Stretch-Stretch** (en bas à droite)
     - Cela va étirer le panel pour couvrir tout le Canvas
     - Vérifiez que les valeurs sont :
       - Left : `0`
       - Top : `0`
       - Right : `0`
       - Bottom : `0`

3. **Vérifier l'Image de Fond**
   - Le panel doit avoir un composant **Image** :
     - Color : Noir ou couleur de votre choix (exemple : #000000FF pour noir opaque)
     - Image Type : Simple ou Sliced

4. **Ordre de Rendu**
   - Assurez-vous que le Canvas a un **Canvas** avec :
     - Sort Order : `100` (pour être au-dessus de tout)

### Structure Hiérarchique Recommandée

```
LoadingScreenCanvas (Canvas)
├── LoadingScreenPanel (Image - plein écran)
    ├── LoadingContent (VerticalLayoutGroup centré)
        ├── LoadingText (TextMeshProUGUI)
        ├── ProgressBarBackground (Image)
        │   └── ProgressBarFill (Image)
        └── PercentageText (TextMeshProUGUI)
```

---

## 📋 Solution 2 : Panneau Settings Bien Organisé

### Configuration du Settings Panel

1. **Panel Principal**
   - Sélectionnez le GameObject `SettingsPanel` dans le Pause Menu
   - **Rect Transform** :
     - Ancrage : Stretch-Stretch (Alt + Shift + clic en bas à droite)
     - Left : `0`, Top : `0`, Right : `0`, Bottom : `0`
   - **Image** :
     - Color : Semi-transparent (exemple : #000000CC pour noir à 80%)

2. **Conteneur de Settings**
   - Créez un GameObject enfant nommé `SettingsContainer`
   - **Rect Transform** :
     - Ancrage : Middle-Center
     - Width : `800`
     - Height : `600`
     - Pos X : `0`
     - Pos Y : `0`
   - Ajoutez un composant **Vertical Layout Group** :
     - Child Alignment : `Middle Center`
     - Child Force Expand Width : ✅
     - Child Force Expand Height : ❌
     - Spacing : `20`
     - Padding : Left=50, Right=50, Top=50, Bottom=50

3. **Organisation des Sections**

#### Section Volume (Audio)
```
VolumeSection (GameObject avec VerticalLayoutGroup)
├── SectionTitle (TextMeshProUGUI) - "VOLUME"
├── MasterVolumeGroup (HorizontalLayoutGroup)
│   ├── MasterLabel (TextMeshProUGUI) - "Master"
│   ├── MasterSlider (Slider)
│   └── MasterValueText (TextMeshProUGUI) - "100%"
├── MusicVolumeGroup (HorizontalLayoutGroup)
│   ├── MusicLabel (TextMeshProUGUI) - "Musique"
│   ├── MusicSlider (Slider)
│   └── MusicValueText (TextMeshProUGUI) - "100%"
└── SFXVolumeGroup (HorizontalLayoutGroup)
    ├── SFXLabel (TextMeshProUGUI) - "Effets Sonores"
    ├── SFXSlider (Slider)
    └── SFXValueText (TextMeshProUGUI) - "100%"
```

**Configuration de chaque VolumeGroup** (HorizontalLayoutGroup) :
- Child Alignment : `Middle Left`
- Child Control Size Width : ❌
- Child Control Size Height : ✅
- Child Force Expand Width : ❌
- Child Force Expand Height : ❌
- Spacing : `10`

**Tailles recommandées** :
- Label : Width=200, Height=30
- Slider : Width=400, Height=30
- ValueText : Width=80, Height=30

#### Section Graphiques
```
GraphicsSection (GameObject avec VerticalLayoutGroup)
├── SectionTitle (TextMeshProUGUI) - "GRAPHIQUES"
├── QualityGroup (HorizontalLayoutGroup)
│   ├── QualityLabel (TextMeshProUGUI) - "Qualité"
│   └── QualityDropdown (TMP_Dropdown)
├── ResolutionGroup (HorizontalLayoutGroup)
│   ├── ResolutionLabel (TextMeshProUGUI) - "Résolution"
│   └── ResolutionDropdown (TMP_Dropdown)
└── FullscreenGroup (HorizontalLayoutGroup)
    ├── FullscreenLabel (TextMeshProUGUI) - "Plein Écran"
    └── FullscreenToggle (Toggle)
```

4. **Boutons de Contrôle**
```
ButtonsGroup (HorizontalLayoutGroup en bas)
├── ApplyButton (Button)
├── ResetButton (Button)
└── BackButton (Button)
```

**Configuration du ButtonsGroup** :
- **Rect Transform** :
  - Ancrage : Bottom-Center
  - Pos Y : `50` (50 pixels du bas)
  - Width : `600`
  - Height : `60`
- **HorizontalLayoutGroup** :
  - Child Alignment : `Middle Center`
  - Spacing : `20`
  - Child Force Expand : Width=✅, Height=✅

---

## 🎨 Configuration Détaillée des Layout Groups

### Vertical Layout Group (pour les sections)
- **Spacing** : `15-20` pixels entre les éléments
- **Child Force Expand** : Width=✅, Height=❌
- **Child Control Size** : Width=✅, Height=✅
- **Padding** : Top/Bottom=10, Left/Right=20

### Horizontal Layout Group (pour les lignes)
- **Spacing** : `10-15` pixels entre les éléments
- **Child Force Expand** : Width=❌, Height=❌
- **Child Control Size** : Width=❌, Height=✅

### Content Size Fitter (si nécessaire)
Si des éléments ne s'affichent pas correctement, ajoutez un **Content Size Fitter** :
- Horizontal Fit : `Unconstrained` ou `Preferred Size`
- Vertical Fit : `Preferred Size`

---

## 🔍 Checklist de Vérification

### Pour l'Écran de Chargement
- [ ] Canvas en mode Screen Space - Overlay
- [ ] Canvas Scaler configuré avec Scale With Screen Size
- [ ] Panel principal en Stretch-Stretch (0,0,0,0)
- [ ] Couleur de fond opaque (Alpha = 255)
- [ ] Sort Order du Canvas suffisamment élevé (100+)

### Pour le Panneau Settings
- [ ] Panel principal en plein écran
- [ ] Container central avec taille fixe (800x600)
- [ ] Vertical Layout Group sur le container
- [ ] Chaque section a son propre Layout Group
- [ ] Les labels ont une largeur fixe (Layout Element)
- [ ] Les sliders ont une largeur fixe
- [ ] Spacing approprié entre les éléments (15-20px)
- [ ] Padding sur les containers (20-50px)

---

## 🛠️ Étapes de Création Rapide dans Unity

### Créer un Slider Audio Correctement
1. Clic droit dans la hiérarchie > UI > Slider
2. Renommez en `MasterVolumeSlider`
3. Configurez dans l'Inspector :
   - Min Value : `0`
   - Max Value : `1`
   - Value : `1`
4. Ajoutez un **Layout Element** :
   - Preferred Width : `400`
   - Preferred Height : `30`

### Créer un Label
1. Clic droit > UI > Text - TextMeshPro
2. Configurez :
   - Font Size : `20-24`
   - Alignment : Middle Left
   - Ajoutez un **Layout Element** :
     - Preferred Width : `200`
     - Preferred Height : `30`

### Créer un Dropdown
1. Clic droit > UI > Dropdown - TextMeshPro
2. Ajoutez un **Layout Element** :
   - Preferred Width : `400`
   - Preferred Height : `40`

---

## 🎯 Ordre des Opérations pour Tout Réparer

### Étape 1 : Fixer l'Écran de Chargement (5 min)
1. Ouvrez la scène qui contient le LoadingScreen
2. Sélectionnez le Canvas
3. Configurez Canvas Scaler
4. Sélectionnez le Panel
5. Ancrez-le en Stretch-Stretch
6. Testez en Play Mode

### Étape 2 : Fixer le Settings Panel (15-20 min)
1. Ouvrez la scène du Menu ou Game (selon où est le Settings Panel)
2. Sélectionnez le SettingsPanel
3. Assurez-vous qu'il couvre tout l'écran
4. Créez le SettingsContainer avec VerticalLayoutGroup
5. Réorganisez tous les éléments UI dans ce container
6. Ajoutez les Layout Elements sur chaque élément
7. Configurez les spacing et padding
8. Testez en Play Mode

### Étape 3 : Désactiver les Panels par Défaut
1. Sélectionnez `SettingsPanel`
2. Dans l'Inspector, **décochez la case en haut à gauche** pour désactiver le GameObject
3. Faites de même pour `PauseMenuPanel` si nécessaire
4. Le script les activera au besoin

---

## 📱 Conseils Supplémentaires

### Pour un UI Responsive
- Utilisez toujours **Canvas Scaler** avec Scale With Screen Size
- Ancrez correctement les éléments (Anchors = Pivots pour le scaling)
- Utilisez des Layout Groups pour les listes d'éléments
- Testez dans différentes résolutions (Game View)

### Pour Éviter les Superpositions
- Un seul élément par "couche" de Layout Group
- Spacing suffisant (minimum 10px)
- Layout Element sur les éléments qui ont une taille fixe
- Content Size Fitter sur les éléments à taille variable

### Pour le Z-Order (Ordre d'Affichage)
- Les éléments en bas de la hiérarchie s'affichent au-dessus
- Utilisez `Canvas.sortingOrder` pour les Canvas multiples
- Les panels de popup doivent avoir un sortingOrder plus élevé

---

## ❓ Questions Fréquentes

**Q : Mon panel ne couvre pas tout l'écran malgré le Stretch-Stretch ?**
R : Vérifiez que le Canvas parent est bien en Screen Space - Overlay et que le Canvas Scaler est configuré.

**Q : Les éléments se chevauchent toujours ?**
R : Ajoutez des Layout Elements avec Preferred Width/Height sur chaque élément et augmentez le Spacing.

**Q : Le layout ne se met pas à jour ?**
R : Parfois Unity ne recalcule pas. Solution : désactivez/réactivez le GameObject ou sortez/rentrez en Play Mode.

**Q : Comment centrer verticalement les éléments ?**
R : Sur le VerticalLayoutGroup, mettez Child Alignment à `Upper Center`, `Middle Center` ou `Lower Center`.

---

## 🔗 Liens avec les Scripts

### LoadingScreen.cs
- `loadingScreenPanel` doit référencer le Panel plein écran
- `progressBar` doit être le Slider de progression
- `loadingText` et `percentageText` sont les TextMeshProUGUI

### SettingsManager.cs
- Tous les Sliders doivent avoir leur événement `OnValueChanged` lié aux méthodes du script
- Les Dropdowns doivent avoir leur événement `OnValueChanged` lié
- Le Toggle Fullscreen aussi

### PauseMenuManager.cs
- `pauseMenuPanel` doit référencer le panel de pause principal
- `settingsPanel` doit référencer le panel de settings

---

**Bon courage ! Si vous suivez ce guide, vos menus devraient être parfaitement configurés.** 🎮

