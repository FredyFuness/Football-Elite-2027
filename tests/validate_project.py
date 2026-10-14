#!/usr/bin/env python3
"""Unity project structure and gameplay wiring checks.
These checks do not compile C# or substitute for running the Unity Editor.
"""
from pathlib import Path
import json
import re

ROOT = Path(__file__).resolve().parents[1]

def read(path):
    p = ROOT / path
    assert p.is_file(), f"Missing {path}"
    return p.read_text(encoding="utf-8")

manifest = json.loads(read("Packages/manifest.json"))
assert "com.unity.render-pipelines.universal" in manifest["dependencies"]
assert "com.unity.inputsystem" in manifest["dependencies"]
assert "com.unity.ugui" in manifest["dependencies"]
version = read("ProjectSettings/ProjectVersion.txt")
assert "6000.0" in version

scripts = [
    "FootballInput", "TouchJoystick", "TouchButton", "BallController",
    "PlayerController", "GoalkeeperAI", "GoalTrigger", "MatchManager",
    "BroadcastCamera", "GraphicsBootstrap", "MatchHUD",
    "FootballerVisualAnimator", "TouchControlVisibility",
]
for script in scripts:
    contents = read(f"Assets/FootballElite/Scripts/Runtime/{script}.cs")
    assert f"class {script}" in contents, f"Class missing in {script}.cs"
    assert "namespace FootballElite" in contents, f"Namespace missing in {script}.cs"
    assert contents.count("{") == contents.count("}"), f"Unbalanced braces in {script}.cs"

editor = read("Assets/FootballElite/Scripts/Editor/TrainingSceneBuilder.cs")
assert editor.startswith("#if UNITY_EDITOR")
assert editor.rstrip().endswith("#endif")
assert "[MenuItem(" in editor
assert "Training.unity" in editor
assert "InputSystemUIInputModule" in editor
assert "AssignDefaultActions()" in editor
assert editor.count("{") == editor.count("}")

for method in [
    "CreatePitch()", "CreateStadium()", "CreateGoals(match)", "CreateLighting()",
    "CreateBall()", "CreateFootballer(input, ball)", "CreateGoalkeeper(ball)",
    "CreateCamera(player.transform, ball.transform)", "CreateUI(input, match)",
]:
    assert method in editor, f"Generator call missing: {method}"

source = {
    name: read(f"Assets/FootballElite/Scripts/Runtime/{name}.cs")
    for name in scripts
}
assert "Physics" in read("Docs/ARQUITECTURA.md") or "PhysX" in read("Docs/ARQUITECTURA.md")
assert "body.AddForce" in source["BallController"]
assert "ForceMode.Impulse" in source["BallController"]
assert "TryDribble" in source["BallController"]
assert "ball.Kick" in source["PlayerController"]
assert "controller.Move" in source["PlayerController"]
assert "OnTriggerEnter" in source["GoalTrigger"]
assert "RegisterGoal" in source["MatchManager"]
assert "ResetMatch()" in source["MatchManager"]
assert "Gamepad.current" in source["FootballInput"]
assert "Keyboard.current" in source["FootballInput"]
assert "IPointerDownHandler" in source["TouchJoystick"]
assert "IPointerDownHandler" in source["TouchButton"]
assert "Application.isMobilePlatform" in source["TouchControlVisibility"]
assert "Screen.orientation" in source["GraphicsBootstrap"]
assert "BroadcastCamera" in editor

gitignore = read(".gitignore")
for junk in ["/[Ll]ibrary/", "/[Tt]emp/", "/[Bb]uilds/"]:
    assert junk in gitignore

print(f"PASS: {len(scripts)} Unity runtime scripts, Editor generator, input, physics, scene wiring and package configuration.")
print("NOTE: full Unity compile, physics and iPhone device testing are still required.")
