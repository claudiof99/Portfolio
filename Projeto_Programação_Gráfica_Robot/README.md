# Projeto Programação Gráfica — Robot

Interactive 3D robot built with [p5.js](https://p5js.org/) (WEBGL). Move the robot, pose its limbs, orbit the camera, and trigger sound effects — all from the keyboard.

There is **no build step** and **no `npm install`**. Everything runs in the browser from `index.html`.

---

## What you need

| Requirement | Notes |
|-------------|--------|
| **Modern web browser** | Chrome, Firefox, Edge, or Safari (recent version) |
| **A local web server** | Required — do **not** open `index.html` by double-clicking it |
| **The full project folder** | Must include `textures/` and `audio/` (see below) |

You only need **one** of these to serve the files locally:

- **Live Server** extension in VS Code / Cursor *(recommended)*
- **Python 3** (`python -m http.server`)
- **Node.js** (`npx serve`)

p5.js is already bundled in `libraries/` — no separate install.

---

## Folder structure (must be complete)

When you receive or unzip the project, confirm these paths exist:

```
Projeto_Programação_Gráfica_Robot/
├── index.html              ← entry point
├── sketch.js
├── bodyParts/              ← robot mesh & assembly
│   ├── Robot.js
│   ├── head.js
│   ├── torso.js
│   ├── pelvis.js
│   ├── leftArm.js
│   ├── rightArm.js
│   ├── leftLeg.js
│   └── rightLeg.js
├── libraries/
│   ├── p5.min.js
│   └── p5.sound.min.js
├── textures/               ← required image assets
│   ├── pastel-gray.jpg
│   ├── blackCarbon.jpg
│   ├── redTextureLED.jpg
│   ├── LED_EYE.jpg
│   ├── fire.jpg
│   ├── robobodytex.png
│   └── PlasticSpace.jpg
└── audio/                  ← required sound files
    ├── freesound_community-robot-power-off-97246.mp3
    └── diff_style-robot-talk-344757.mp3
```

If `textures/` or the `.mp3` files are missing, the sketch may fail to load or the robot will render without textures. Make sure the sender included **the entire folder**, not only the `.js` files.

---

## How to run

### Option A — Live Server (easiest)

1. Install the **Live Server** extension in VS Code or Cursor.
2. Open this project folder.
3. Right-click `index.html` → **Open with Live Server**.
4. Your browser opens at a URL like `http://127.0.0.1:5500`.

### Option B — Python

```powershell
cd path\to\Projeto_Programação_Gráfica_Robot
python -m http.server 8000
```

Then open: **http://localhost:8000**

### Option C — Node.js

```powershell
cd path\to\Projeto_Programação_Gráfica_Robot
npx serve .
```

Open the URL printed in the terminal (usually **http://localhost:3000**).

---

## Using the app

1. Wait for the page to load (textures load in `preload()`).
2. **Click the canvas** so keyboard input is focused.
3. Press any key once to unlock the opening sound (browser autoplay policy).

### Controls

| Action | Keys |
|--------|------|
| Move forward / back | **W** / **S** (walk animation) |
| Turn left / right | **A** / **D** |
| Move up / down | **Space** / **Shift** |
| Rotate robot | **Q** / **E** |
| Scale down / up | **Z** / **X** |
| Head tilt / turn | **Arrow keys** |
| Head follows mouse | **M** (toggle) |
| Torso twist | **,** / **.** |
| Torso lean back / forward | **9** / **0** |
| Shoulder | **R** / **F** |
| Elbow | **T** / **G** |
| Hip (both legs) | **Y** / **H** |
| Knee (both legs) | **U** / **N** |
| Left hip (extra) | **5** / **6** |
| Left knee (extra) | **7** / **8** |
| Spotlight cone | **[** / **]** |
| Spotlight edge | **-** / **=** |
| Camera pitch | **I** / **K** |
| Camera yaw | **J** / **L** |
| Camera zoom | **O** / **P** |
| Jump | **V** (hold) |
| Reset pose & camera | **Home** |

---

## Troubleshooting

| Problem | What to try |
|---------|-------------|
| Blank page or errors about images | Run via a **local server**, not `file://`. Check that `textures/` exists. |
| No sound | Click the canvas first; browsers block audio until user interaction. |
| Keys do nothing | Click the canvas to focus it. |
| `404` for `.jpg` or `.mp3` | Re-download or ask for the full zip including `textures/` and `audio/`. |
| Server won't start | Try another option (Python vs Live Server vs `npx serve`). |

Open the browser **Developer Tools** (F12 → **Console**) to see specific load errors.

---

## Sending this project to someone else

Zip the **entire** project directory, including:

- All `.js` files and `index.html`
- `libraries/`
- `textures/` (all images)
- `audio/` (both `.mp3` files)

Then share the zip. The recipient only needs a browser and one way to run a local server (see above).
