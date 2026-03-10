# 📋 VALEURS EXACTES POUR LA CONFIGURATION UI

## 🎯 Canvas Principal - Valeurs à Copier

### Canvas Component
```
Render Mode: Screen Space - Overlay
Pixel Perfect: OFF
Sort Order: (voir tableau ci-dessous)
```

### Canvas Scaler
```
UI Scale Mode: Scale With Screen Size
Reference Resolution X: 1920
Reference Resolution Y: 1080
Screen Match Mode: Match Width Or Height
Match: 0.5
Reference Pixels Per Unit: 100
```

### Graphic Raycaster
```
Ignore Reversed Graphics: ON
Blocking Objects: None
Blocking Mask: Everything
```

---

## 🔢 Sort Order par Canvas

| Canvas | Sort Order | Priorité |
|--------|-----------|----------|
| MainMenuCanvas | 0 | Normal |
| GameUICanvas | 0 | Normal |
| PauseMenuCanvas | 50 | Par-dessus le jeu |
| LoadingScreenCanvas | 100 | Toujours visible |

---

## 📐 LoadingScreen - Configuration Exacte

### LoadingScreenPanel (Image)
```
RectTransform:
  Anchor Preset: Stretch-Stretch
  Anchor Min: (0, 0)
  Anchor Max: (1, 1)
  Pivot: (0.5, 0.5)
  Left: 0
  Top: 0
  Right: 0
  Bottom: 0
  
Image:
  Color: (0, 0, 0, 255) - Noir opaque
  Material: None
  Raycast Target: OFF
```

### LoadingContent (Container)
```
RectTransform:
  Anchor: Middle-Center
  Pos X: 0
  Pos Y: 0
  Width: 600
  Height: 400

VerticalLayoutGroup:
  Spacing: 30
  Child Alignment: Middle Center
  Child Control Size: Width=ON, Height=ON
  Child Force Expand: Width=OFF, Height=OFF
  Padding: Left=50, Right=50, Top=50, Bottom=50
```

### LoadingText (TextMeshProUGUI)
```
Text: "Chargement en cours..."
Font Size: 36
Color: (255, 255, 255, 255) - Blanc
Alignment: Center
Auto Size: OFF

LayoutElement:
  Preferred Width: 500
  Preferred Height: 50
```

### ProgressBar (Slider)
```
Slider:
  Min Value: 0
  Max Value: 1
  Value: 0
  Whole Numbers: OFF

RectTransform (Background):
  Width: 500
  Height: 30

LayoutElement:
  Preferred Width: 500
  Preferred Height: 30
```

### PercentageText (TextMeshProUGUI)
```
Text: "0%"
Font Size: 28
Color: (255, 255, 255, 255)
Alignment: Center

LayoutElement:
  Preferred Width: 100
  Preferred Height: 40
```

---

## ⚙️ SettingsPanel - Configuration Exacte

### SettingsPanel (fond noir semi-transparent)
```
RectTransform:
  Anchor Preset: Stretch-Stretch
  Left: 0, Top: 0, Right: 0, Bottom: 0

Image:
  Color: (0, 0, 0, 204) - Noir 80% opaque
  Raycast Target: ON (pour bloquer les clics)
```

### SettingsContainer
```
RectTransform:
  Anchor: Middle-Center
  Pos X: 0
  Pos Y: 0
  Width: 900
  Height: 700

VerticalLayoutGroup:
  Spacing: 25
  Child Alignment: Upper Center
  Child Control Size: Width=ON, Height=ON
  Child Force Expand: Width=ON, Height=OFF
  Padding: 40 (tous côtés)
```

---

## 🔊 Section Volume - Valeurs Exactes

### VolumeSection (Container)
```
VerticalLayoutGroup:
  Spacing: 15
  Child Alignment: Upper Center
  Child Control Size: Width=ON, Height=ON
  Child Force Expand: Width=ON, Height=OFF
  Padding: 20 (tous côtés)

RectTransform:
  Height: auto (contrôlé par le layout)
```

### SectionTitle (TextMeshProUGUI)
```
Text: "── VOLUME ──"
Font Size: 28
Font Style: Bold
Color: (255, 215, 0, 255) - Doré
Alignment: Center
Margin: Bottom=10

LayoutElement:
  Preferred Height: 40
```

### MasterVolumeRow (HorizontalLayoutGroup)
```
HorizontalLayoutGroup:
  Spacing: 15
  Child Alignment: Middle Left
  Child Control Size: Width=OFF, Height=ON
  Child Force Expand: tous OFF
  Padding: 0
```

#### Label "Master" (TextMeshProUGUI)
```
Text: "Volume Principal"
Font Size: 20
Color: (255, 255, 255, 255)
Alignment: Middle-Left
Wrapping: OFF

LayoutElement:
  Preferred Width: 200
  Preferred Height: 35
```

#### MasterSlider (Slider)
```
Slider:
  Min Value: 0
  Max Value: 1
  Value: 1
  Whole Numbers: OFF

RectTransform (Background):
  Height: 20

Fill Area > Fill (Image):
  Color: (255, 215, 0, 255) - Doré

Handle Slide Area > Handle (Image):
  Width: 30
  Height: 30

LayoutElement:
  Preferred Width: 400
  Preferred Height: 35
```

#### ValueText "100%" (TextMeshProUGUI)
```
Text: "100%"
Font Size: 20
Color: (255, 255, 255, 255)
Alignment: Center

LayoutElement:
  Preferred Width: 80
  Preferred Height: 35
```

### Répétez pour MusicVolumeRow et SFXVolumeRow
Changez juste :
- Label : "Volume Musique" et "Volume Effets"
- Fill Color du Slider (optionnel) : bleu et vert

---

## 🎨 Section Graphiques - Valeurs Exactes

### GraphicsSection (Container)
```
VerticalLayoutGroup:
  Spacing: 15
  Child Alignment: Upper Center
  Child Control Size: Width=ON, Height=ON
  Child Force Expand: Width=ON, Height=OFF
  Padding: 20 (tous côtés)
```

### SectionTitle "GRAPHIQUES"
```
Text: "── GRAPHIQUES ──"
Font Size: 28
Font Style: Bold
Color: (100, 200, 255, 255) - Bleu clair
Alignment: Center

LayoutElement:
  Preferred Height: 40
```

### QualityRow (HorizontalLayoutGroup)
```
HorizontalLayoutGroup:
  Spacing: 15
  Child Alignment: Middle Left
  Child Control Size: Width=OFF, Height=ON
  Child Force Expand: tous OFF
```

#### Label "Qualité"
```
Text: "Qualité Graphique"
Font Size: 20
Alignment: Middle-Left

LayoutElement:
  Preferred Width: 200
  Preferred Height: 40
```

#### QualityDropdown (TMP_Dropdown)
```
Template:
  Height: 150

Item Background (Highlighted):
  Color: (50, 150, 255, 255)

LayoutElement:
  Preferred Width: 480
  Preferred Height: 40
```

### ResolutionRow (même structure)
```
Label: "Résolution"
Dropdown: même config que Quality
```

### FullscreenRow (HorizontalLayoutGroup)
```
Label: "Plein Écran"
  Width: 200, Height: 40

Toggle:
  Background: Width=50, Height=30
  Checkmark: Color=(0, 255, 0, 255) - Vert
  
  LayoutElement:
    Preferred Width: 80
    Preferred Height: 40
```

---

## 🎮 Boutons de Contrôle - Valeurs Exactes

### ButtonsRow (HorizontalLayoutGroup)
```
RectTransform:
  Anchor: Bottom-Center
  Pos Y: 50
  Width: 700
  Height: 70

HorizontalLayoutGroup:
  Spacing: 30
  Child Alignment: Middle Center
  Child Control Size: Width=OFF, Height=OFF
  Child Force Expand: Width=OFF, Height=ON
  Padding: 0
```

### Chaque Button (StyledButton ou Button basique)
```
RectTransform:
  (contrôlé par LayoutElement)

Button:
  Target Graphic: Background Image
  Transition: Color Tint
  Normal Color: (100, 100, 100, 255)
  Highlighted Color: (150, 150, 150, 255)
  Pressed Color: (80, 80, 80, 255)
  Selected Color: (120, 120, 120, 255)
  Disabled Color: (50, 50, 50, 128)
  Color Multiplier: 1
  Fade Duration: 0.1

Image (Background):
  Color: (60, 60, 60, 255)
  Material: None

TextMeshProUGUI (Label):
  Font Size: 24
  Font Style: Bold
  Color: (255, 255, 255, 255)
  Alignment: Center
  Auto Size: OFF

LayoutElement:
  Preferred Width: 180
  Preferred Height: 60
```

#### BackButton
```
Text: "< RETOUR"
Color: (200, 100, 100, 255) - Rouge clair
```

#### ResetButton
```
Text: "RÉINITIALISER"
Color: (255, 200, 100, 255) - Orange
```

#### ApplyButton
```
Text: "APPLIQUER"
Color: (100, 255, 100, 255) - Vert clair
```

---

## 🎯 PauseMenuPanel - Valeurs Exactes

### PauseMenuPanel (fond)
```
RectTransform:
  Anchor: Stretch-Stretch
  Left: 0, Top: 0, Right: 0, Bottom: 0

Image:
  Color: (0, 0, 0, 230) - Noir presque opaque
```

### PauseMenuContainer
```
RectTransform:
  Anchor: Middle-Center
  Width: 500
  Height: 600
  Pos X: 0, Pos Y: 0

VerticalLayoutGroup:
  Spacing: 25
  Child Alignment: Upper Center
  Child Force Expand: Width=ON, Height=OFF
  Padding: 50 (tous côtés)

Image (fond du container - optionnel):
  Color: (30, 30, 30, 255)
```

### Title "PAUSE" (TextMeshProUGUI)
```
Text: "PAUSE"
Font Size: 48
Font Style: Bold
Color: (255, 255, 255, 255)
Alignment: Center

LayoutElement:
  Preferred Height: 80
```

### Boutons du Menu Pause
```
ResumeButton:
  Text: "► REPRENDRE"
  Width: 350, Height: 70
  
SettingsButton:
  Text: "⚙ PARAMÈTRES"
  Width: 350, Height: 70

RestartButton:
  Text: "↻ RECOMMENCER"
  Width: 350, Height: 70

MainMenuButton:
  Text: "◄ MENU PRINCIPAL"
  Width: 350, Height: 70

QuitButton:
  Text: "✖ QUITTER"
  Width: 350, Height: 70
  Color: Rouge (200, 100, 100)
```

---

## 📏 Tableau Récapitulatif des Tailles

| Élément | Width | Height | Notes |
|---------|-------|--------|-------|
| Label | 200 | 35-40 | Texte aligné à gauche |
| Slider | 400 | 35 | Volume controls |
| Value Text | 80 | 35 | "100%" etc. |
| Dropdown | 480 | 40 | Quality, Resolution |
| Toggle | 80 | 40 | Checkboxes |
| Petit Bouton | 180 | 60 | Settings, Apply, etc. |
| Moyen Bouton | 350 | 70 | Pause menu buttons |
| Grand Bouton | 400 | 80 | Main menu buttons |

---

## 🎨 Palette de Couleurs Recommandée

### Textes
```
Titre Principal: (255, 255, 255, 255) - Blanc
Sous-titre: (200, 200, 200, 255) - Gris clair
Texte Normal: (180, 180, 180, 255) - Gris
Section Title: (255, 215, 0, 255) - Doré
```

### Boutons
```
Normal: (60, 60, 60, 255) - Gris foncé
Hover: (90, 90, 90, 255) - Gris moyen
Pressed: (40, 40, 40, 255) - Gris très foncé
Danger (Quit): (200, 50, 50, 255) - Rouge
Success (Apply): (50, 200, 50, 255) - Vert
```

### Fonds
```
Panel Principal: (0, 0, 0, 204) - Noir 80%
Container: (30, 30, 30, 255) - Gris très foncé
Highlight: (100, 150, 255, 128) - Bleu semi-transparent
```

### Sliders
```
Background: (100, 100, 100, 255) - Gris
Fill (Master): (255, 215, 0, 255) - Doré
Fill (Music): (100, 150, 255, 255) - Bleu
Fill (SFX): (100, 255, 100, 255) - Vert
Handle: (255, 255, 255, 255) - Blanc
```

---

## ⚡ Commandes Unity Rapides

### Pour aligner rapidement :
```
Stretch-Stretch: Alt + Shift + Clic (bas-droite de l'anchor preset)
Middle-Center: Clic direct (milieu de l'anchor preset)
```

### Pour copier des valeurs :
```
Clic droit sur le component > Copy Component
Clic droit sur l'autre GameObject > Paste Component Values
```

### Pour réinitialiser :
```
Clic droit sur le component > Reset
```

---

## 🔍 Debug - Valeurs à Vérifier

Si votre UI ne fonctionne pas, vérifiez ces valeurs :

### Canvas Scaler
- [ ] UI Scale Mode = Scale With Screen Size
- [ ] Reference Resolution = 1920 x 1080
- [ ] Match = 0.5

### Tous les Panels
- [ ] Anchor = Stretch-Stretch
- [ ] Left = 0, Right = 0, Top = 0, Bottom = 0

### Tous les LayoutGroups
- [ ] Spacing > 0 (minimum 10)
- [ ] Child Force Expand Height = OFF (sauf exceptions)

### Tous les éléments UI avec taille fixe
- [ ] LayoutElement component présent
- [ ] Preferred Width défini
- [ ] Preferred Height défini

---

## 💾 Sauvegarder comme Template

Une fois configuré, sauvegardez vos panels en prefabs :

1. Glissez le panel dans `Assets/Prefab/UI/`
2. Utilisez-les comme templates pour d'autres menus
3. Créez des variants pour différents styles

---

**Avec ces valeurs exactes, vous pouvez recréer l'UI pixel-perfect ! 🎯**

