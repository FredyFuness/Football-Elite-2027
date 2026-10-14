#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FootballElite.Editor
{
    /// <summary>
    /// Repeatable training pitch generator. All dependencies are assigned in the scene;
    /// no imported third-party meshes are required to run the first milestone.
    /// </summary>
    public static class TrainingSceneBuilder
    {
        private const string Root = "Assets/FootballElite";
        private const string Scenes = Root + "/Scenes";
        private const string Materials = Root + "/Materials";
        private static Material pitch, stripe, white, blue, red, navy, yellow, black, metal, seats, sand;

        [MenuItem("Football Elite/Crear escenario de entrenamiento (Fase 1)")]
        public static void BuildTrainingScene()
        {
            if (!EditorUtility.DisplayDialog("Football Elite — Fase 1",
                    "Se generará Training.unity y se reemplazará la escena abierta. ¿Continuar?", "Generar", "Cancelar"))
                return;

            EnsureFolder(Scenes);
            EnsureFolder(Materials);
            MakeMaterials();

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            SceneManager.SetActiveScene(scene);

            new GameObject("Football Elite — Systems").AddComponent<GraphicsBootstrap>();
            MatchManager match = new GameObject("Match Manager").AddComponent<MatchManager>();
            FootballInput input = new GameObject("Unified Input").AddComponent<FootballInput>();

            CreatePitch();
            CreateStadium();
            CreateGoals(match);
            CreateLighting();

            BallController ball = CreateBall();
            PlayerController player = CreateFootballer(input, ball);
            GoalkeeperAI keeper = CreateGoalkeeper(ball);
            match.Configure(player, ball, keeper);
            CreateCamera(player.transform, ball.transform);
            CreateUI(input, match);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, Scenes + "/Training.unity");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Football Elite",
                "La Fase 1 se generó correctamente. Abre Training.unity y pulsa Play.", "Aceptar");
        }

        private static void EnsureFolder(string path)
        {
            string[] parts = path.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }

        private static Material CreateMaterial(string name, Color color, float smoothness = .1f, float metallic = 0f)
        {
            string path = Materials + "/" + name + ".mat";
            Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null) shader = Shader.Find("Standard");
                existing = new Material(shader) { name = name };
                AssetDatabase.CreateAsset(existing, path);
            }

            existing.color = color;
            if (existing.HasProperty("_BaseColor")) existing.SetColor("_BaseColor", color);
            if (existing.HasProperty("_Smoothness")) existing.SetFloat("_Smoothness", smoothness);
            if (existing.HasProperty("_Metallic")) existing.SetFloat("_Metallic", metallic);
            EditorUtility.SetDirty(existing);
            return existing;
        }

        private static void MakeMaterials()
        {
            pitch = CreateMaterial("GrassDark", new Color(.045f, .27f, .13f));
            stripe = CreateMaterial("GrassLight", new Color(.08f, .33f, .16f));
            white = CreateMaterial("PitchLines", new Color(.91f, .97f, .94f), .22f);
            blue = CreateMaterial("TeamHome", new Color(.09f, .39f, .89f), .45f);
            red = CreateMaterial("TeamAway", new Color(.85f, .16f, .16f), .42f);
            yellow = CreateMaterial("Goalkeeper", new Color(.93f, .75f, .11f), .43f);
            navy = CreateMaterial("Stadium", new Color(.07f, .12f, .19f));
            black = CreateMaterial("DarkParts", new Color(.035f, .055f, .083f));
            metal = CreateMaterial("Metal", new Color(.53f, .62f, .69f), .7f, .7f);
            seats = CreateMaterial("Seats", new Color(.08f, .18f, .37f));
            sand = CreateMaterial("Surroundings", new Color(.21f, .28f, .29f));
        }

        private static GameObject Primitive(PrimitiveType primitive, string name, Transform parent,
            Vector3 position, Vector3 scale, Material material, bool keepCollider = false)
        {
            GameObject obj = GameObject.CreatePrimitive(primitive);
            obj.name = name;
            if (parent != null) obj.transform.SetParent(parent, false);
            obj.transform.localPosition = position;
            obj.transform.localScale = scale;
            Renderer meshRenderer = obj.GetComponent<Renderer>();
            if (material != null && meshRenderer != null) meshRenderer.sharedMaterial = material;
            if (!keepCollider)
            {
                Collider collider = obj.GetComponent<Collider>();
                if (collider != null) Object.DestroyImmediate(collider);
            }
            return obj;
        }

        private static void Cube(string name, Transform parent, Vector3 pos, Vector3 size, Material material, bool collision = false)
        {
            Primitive(PrimitiveType.Cube, name, parent, pos, size, material, collision);
        }

        private static void Mark(Vector3 a, Vector3 b, float thickness = .12f)
        {
            Vector3 direction = b - a;
            GameObject mark = Primitive(PrimitiveType.Cube, "Painted line", null,
                (a + b) * .5f + Vector3.up * .015f,
                new Vector3(thickness, .025f, direction.magnitude), white);
            mark.transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        }

        private static void BoxMark(float left, float right, float near, float far, bool closed = true)
        {
            if (closed) Mark(new Vector3(left, 0, near), new Vector3(right, 0, near));
            Mark(new Vector3(left, 0, near), new Vector3(left, 0, far));
            Mark(new Vector3(right, 0, near), new Vector3(right, 0, far));
            Mark(new Vector3(left, 0, far), new Vector3(right, 0, far));
        }

        private static void Circle(float radius, float centerZ = 0f)
        {
            for (int i = 0; i < 64; i++)
            {
                float a = i * Mathf.PI * 2 / 64;
                float b = (i + 1) * Mathf.PI * 2 / 64;
                Mark(new Vector3(Mathf.Cos(a) * radius, 0, centerZ + Mathf.Sin(a) * radius),
                     new Vector3(Mathf.Cos(b) * radius, 0, centerZ + Mathf.Sin(b) * radius), .09f);
            }
        }

        private static void CreatePitch()
        {
            Cube("Pitch physical collider", null, new Vector3(0, -.17f, 0), new Vector3(68, .30f, 105), pitch, true);
            for (int i = 0; i < 10; i++)
            {
                float z = -45f + i * 10f;
                Cube("Mowed grass band", null, new Vector3(0, -.012f, z),
                    new Vector3(64, .025f, 10f), i % 2 == 0 ? stripe : pitch);
            }
            BoxMark(-32f, 32f, -50f, 50f);
            Mark(new Vector3(-32, 0, 0), new Vector3(32, 0, 0));
            Circle(9.15f);
            Primitive(PrimitiveType.Cylinder, "Kickoff spot", null,
                new Vector3(0, .02f, 0), new Vector3(.20f, .02f, .20f), white);

            BoxMark(-20.16f, 20.16f, 33.5f, 50f, false);
            BoxMark(-20.16f, 20.16f, -50f, -33.5f, false);
            BoxMark(-9.16f, 9.16f, 44.5f, 50f, false);
            BoxMark(-9.16f, 9.16f, -50f, -44.5f, false);

            foreach (int side in new[] { -1, 1 })
                Primitive(PrimitiveType.Cylinder, "Penalty spot", null,
                    new Vector3(0, .02f, side * 39f), new Vector3(.16f, .02f, .16f), white);
        }

        private static void CreateStadium()
        {
            Cube("Track north", null, new Vector3(0, -.35f, 59), new Vector3(90, .25f, 12), sand);
            Cube("Track south", null, new Vector3(0, -.35f, -59), new Vector3(90, .25f, 12), sand);
            Cube("Track east", null, new Vector3(39, -.35f, 0), new Vector3(10, .25f, 114), sand);
            Cube("Track west", null, new Vector3(-39, -.35f, 0), new Vector3(10, .25f, 114), sand);
            for (int row = 0; row < 4; row++)
            {
                float height = 1.1f + row * 1.2f;
                float lateral = 37.5f + row * 2.4f;
                float end = 56.4f + row * 2.3f;
                Cube("West grandstand tier", null, new Vector3(-lateral, height * .5f, 0),
                    new Vector3(2.3f, height, 112), row % 2 == 0 ? seats : navy);
                Cube("East grandstand tier", null, new Vector3(lateral, height * .5f, 0),
                    new Vector3(2.3f, height, 112), row % 2 == 0 ? seats : navy);
                Cube("North grandstand tier", null, new Vector3(0, height * .5f, end),
                    new Vector3(79, height, 2.2f), row % 2 == 0 ? seats : navy);
                Cube("South grandstand tier", null, new Vector3(0, height * .5f, -end),
                    new Vector3(79, height, 2.2f), row % 2 == 0 ? seats : navy);
            }
            for (int z = -45; z < 50; z += 12)
            {
                Cube("LED panel left", null, new Vector3(-33.7f, .57f, z),
                    new Vector3(.2f, .95f, 11), blue);
                Cube("LED panel right", null, new Vector3(33.7f, .57f, z),
                    new Vector3(.2f, .95f, 11), blue);
            }
        }

        private static void Pole(Vector3 bottom, Vector3 top, float radius, Material material)
        {
            Vector3 delta = top - bottom;
            GameObject pole = Primitive(PrimitiveType.Cylinder, "Goal frame / net wire", null,
                (bottom + top) * .5f,
                new Vector3(radius * 2, delta.magnitude * .5f, radius * 2),
                material);
            pole.transform.rotation = Quaternion.FromToRotation(Vector3.up, delta.normalized);
        }

        private static void CreateGoals(MatchManager match)
        {
            foreach (int side in new[] { -1, 1 })
            {
                float z = side * 50.12f;
                float backZ = z + side * 2.2f;
                Pole(new Vector3(-3.66f, 0, z), new Vector3(-3.66f, 2.44f, z), .075f, white);
                Pole(new Vector3(3.66f, 0, z), new Vector3(3.66f, 2.44f, z), .075f, white);
                Pole(new Vector3(-3.66f, 2.44f, z), new Vector3(3.66f, 2.44f, z), .075f, white);
                for (float x = -3.65f; x <= 3.65f; x += .73f)
                {
                    Pole(new Vector3(x, .07f, backZ), new Vector3(x, 2.40f, backZ), .009f, metal);
                }
                for (float y = .15f; y <= 2.44f; y += .45f)
                {
                    Pole(new Vector3(-3.66f, y, backZ), new Vector3(3.66f, y, backZ), .008f, metal);
                }
                for (int x = -1; x <= 1; x += 2)
                {
                    float postX = x * 3.66f;
                    Pole(new Vector3(postX, 2.44f, z), new Vector3(postX, 2.44f, backZ), .017f, metal);
                    Pole(new Vector3(postX, 0f, z), new Vector3(postX, 0f, backZ), .012f, metal);
                }

                GameObject sensor = new GameObject(side > 0 ? "Goal trigger — Home scores" : "Goal trigger — Away scores");
                sensor.transform.position = new Vector3(0, 1.24f, z + side * .56f);
                BoxCollider box = sensor.AddComponent<BoxCollider>();
                box.size = new Vector3(7.05f, 2.4f, .65f);
                box.isTrigger = true;
                GoalTrigger goal = sensor.AddComponent<GoalTrigger>();
                goal.Configure(match, side > 0);
            }
        }

        private static void CreateLighting()
        {
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(.57f, .64f, .76f);
            GameObject sun = new GameObject("Stadium sunlight");
            sun.transform.rotation = Quaternion.Euler(48, -36, 0);
            Light light = sun.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.4f;
            light.shadows = LightShadows.Soft;
            light.shadowStrength = .58f;

            foreach (int x in new[] { -43, 43 })
                foreach (int z in new[] { -58, 58 })
                {
                    Cube("Floodlight mast", null, new Vector3(x, 12, z), new Vector3(.30f, 24, .30f), metal);
                    Cube("Floodlight array", null, new Vector3(x, 24, z), new Vector3(6, 2.2f, .6f), white);
                }
        }

        private static BallController CreateBall()
        {
            GameObject football = Primitive(PrimitiveType.Sphere, "Match ball", null,
                new Vector3(0, .26f, -21.55f), Vector3.one * .44f, white, true);
            BallController controller = football.AddComponent<BallController>();
            Rigidbody rigidbody = football.GetComponent<Rigidbody>();
            if (rigidbody == null) rigidbody = football.AddComponent<Rigidbody>();
            rigidbody.mass = .43f;

            foreach (Vector3 normal in new[]
                     {
                         Vector3.right, Vector3.left, Vector3.forward, Vector3.back, Vector3.up
                     })
            {
                GameObject panel = Primitive(PrimitiveType.Sphere, "Ball pattern", football.transform,
                    normal * .478f, new Vector3(.18f, .025f, .18f), black);
                panel.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);
            }
            return controller;
        }

        private static void AddBody(Transform root, Material kit, Material shorts, Material shoes,
            out Transform leftLeg, out Transform rightLeg, out Transform leftArm, out Transform rightArm)
        {
            Primitive(PrimitiveType.Capsule, "Football shirt", root,
                new Vector3(0, 1.18f, 0), new Vector3(.59f, .55f, .35f), kit);
            Primitive(PrimitiveType.Sphere, "Head", root,
                new Vector3(0, 1.90f, 0), new Vector3(.31f, .34f, .31f), sand);
            Primitive(PrimitiveType.Cube, "Shorts", root,
                new Vector3(0, .78f, 0), new Vector3(.53f, .33f, .34f), shorts);

            leftLeg = new GameObject("LeftLeg").transform;
            rightLeg = new GameObject("RightLeg").transform;
            leftArm = new GameObject("LeftArm").transform;
            rightArm = new GameObject("RightArm").transform;
            leftLeg.SetParent(root, false); rightLeg.SetParent(root, false);
            leftArm.SetParent(root, false); rightArm.SetParent(root, false);
            leftLeg.localPosition = new Vector3(-.16f, .7f, 0);
            rightLeg.localPosition = new Vector3(.16f, .7f, 0);
            leftArm.localPosition = new Vector3(-.35f, 1.48f, 0);
            rightArm.localPosition = new Vector3(.35f, 1.48f, 0);

            foreach (Transform leg in new[] { leftLeg, rightLeg })
            {
                Primitive(PrimitiveType.Capsule, "Sock", leg,
                    new Vector3(0, -.32f, 0), new Vector3(.23f, .35f, .23f), white);
                Primitive(PrimitiveType.Cube, "Boot", leg,
                    new Vector3(0, -.61f, .09f), new Vector3(.26f, .14f, .40f), shoes);
            }
            foreach (Transform arm in new[] { leftArm, rightArm })
                Primitive(PrimitiveType.Capsule, "Arm", arm,
                    new Vector3(0, -.27f, 0), new Vector3(.16f, .31f, .16f), kit);
        }

        private static PlayerController CreateFootballer(FootballInput input, BallController ball)
        {
            GameObject obj = new GameObject("Footballer — Home");
            obj.transform.position = new Vector3(0, 0, -23);
            CharacterController cc = obj.AddComponent<CharacterController>();
            cc.height = 1.90f; cc.radius = .35f; cc.center = new Vector3(0, .95f, 0);
            cc.stepOffset = .25f;

            AddBody(obj.transform, blue, navy, black,
                out Transform ll, out Transform rl, out Transform la, out Transform ra);
            PlayerController controller = obj.AddComponent<PlayerController>();
            controller.Configure(input, ball, cc);
            FootballerVisualAnimator animator = obj.AddComponent<FootballerVisualAnimator>();
            animator.Configure(controller, ll, rl, la, ra);
            return controller;
        }

        private static GoalkeeperAI CreateGoalkeeper(BallController ball)
        {
            GameObject obj = new GameObject("Training goalkeeper");
            obj.transform.position = new Vector3(0, 0, 47.9f);
            obj.transform.rotation = Quaternion.LookRotation(Vector3.back);
            AddBody(obj.transform, yellow, black, black,
                out _, out _, out _, out _);
            CapsuleCollider collider = obj.AddComponent<CapsuleCollider>();
            collider.center = new Vector3(0, 1, 0);
            collider.height = 2f; collider.radius = .42f;
            Rigidbody body = obj.AddComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;
            GoalkeeperAI keeper = obj.AddComponent<GoalkeeperAI>();
            keeper.Configure(ball, 47.9f);
            return keeper;
        }

        private static void CreateCamera(Transform player, Transform ball)
        {
            GameObject go = new GameObject("Broadcast Camera");
            go.tag = "MainCamera";
            Camera cam = go.AddComponent<Camera>();
            cam.fieldOfView = 53f;
            cam.nearClipPlane = .15f;
            cam.farClipPlane = 300f;
            go.transform.position = new Vector3(0, 24, -49);
            go.transform.LookAt(new Vector3(0, .5f, -20));
            go.AddComponent<AudioListener>();
            BroadcastCamera cameraRig = go.AddComponent<BroadcastCamera>();
            cameraRig.Configure(player, ball);
        }

        private static RectTransform Rect(GameObject go, Transform parent, Vector2 anchor,
            Vector2 offset, Vector2 size)
        {
            go.transform.SetParent(parent, false);
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = anchor;
            rect.pivot = new Vector2(.5f, .5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = offset;
            return rect;
        }

        private static GameObject Panel(string name, Transform parent, Vector2 anchor,
            Vector2 offset, Vector2 size, Color color, bool clickable)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            Rect(obj, parent, anchor, offset, size);
            Image image = obj.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = clickable;
            return obj;
        }

        private static Font Font()
        {
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return font != null ? font : Font.CreateDynamicFontFromOSFont("Arial", 18);
        }

        private static Text Label(string name, Transform parent, Vector2 anchor,
            Vector2 offset, Vector2 size, string initialText, int sizePx, Color color)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            Rect(go, parent, anchor, offset, size);
            Text label = go.GetComponent<Text>();
            label.font = Font();
            label.text = initialText;
            label.alignment = TextAnchor.MiddleCenter;
            label.fontSize = sizePx;
            label.fontStyle = FontStyle.Bold;
            label.color = color;
            label.raycastTarget = false;
            return label;
        }

        private static TouchButton CreateTouchButton(Transform parent, string name, string caption,
            Vector2 offset, float width, Color color)
        {
            GameObject baseGo = Panel(name, parent, new Vector2(1, 0),
                offset, Vector2.one * width, color, true);
            Label("Label", baseGo.transform, new Vector2(.5f, .5f),
                Vector2.zero, new Vector2(width, width), caption, 27, Color.white);
            return baseGo.AddComponent<TouchButton>();
        }

        private static void CreateUI(FootballInput input, MatchManager match)
        {
            GameObject canvasGo = new GameObject("Football Elite HUD",
                typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 20;
            CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = .5f;

            GameObject header = Panel("Scoreboard", canvasGo.transform, new Vector2(.5f, 1),
                new Vector2(0, -56), new Vector2(560, 112),
                new Color(.035f, .10f, .18f, .92f), false);
            Text score = Label("Score", header.transform, new Vector2(.5f, .5f),
                new Vector2(0, 21), new Vector2(540, 58), "ELITE FC   0 : 0   RIVALES", 33, Color.white);
            Text clock = Label("Clock", header.transform, new Vector2(.5f, .5f),
                new Vector2(0, -26), new Vector2(240, 37), "03:00", 27, new Color(.56f, .90f, 1f));

            GameObject resetGo = Panel("Restart", canvasGo.transform, new Vector2(1, 1),
                new Vector2(-115, -50), new Vector2(215, 72),
                new Color(.055f, .18f, .31f, .90f), true);
            Button restart = resetGo.AddComponent<Button>();
            Label("Restart Label", resetGo.transform, new Vector2(.5f, .5f),
                Vector2.zero, new Vector2(210, 70), "REINICIAR", 23, Color.white);

            GameObject joystickBase = Panel("Move joystick", canvasGo.transform, new Vector2(0, 0),
                new Vector2(177, 160), Vector2.one * 252,
                new Color(.1f, .2f, .29f, .58f), true);
            GameObject knob = Panel("Knob", joystickBase.transform, new Vector2(.5f, .5f),
                Vector2.zero, Vector2.one * 95, new Color(.47f, .87f, 1f, .77f), false);
            TouchJoystick joystick = joystickBase.AddComponent<TouchJoystick>();
            joystick.Configure(joystickBase.GetComponent<RectTransform>(), knob.GetComponent<RectTransform>());

            TouchButton pass = CreateTouchButton(canvasGo.transform, "Pass", "PASE",
                new Vector2(-340, 137), 169, new Color(.02f, .47f, .74f, .85f));
            TouchButton shoot = CreateTouchButton(canvasGo.transform, "Shoot", "TIRO",
                new Vector2(-146, 154), 180, new Color(.90f, .24f, .14f, .9f));
            TouchButton sprint = CreateTouchButton(canvasGo.transform, "Sprint", "SPRINT",
                new Vector2(-190, 344), 154, new Color(.16f, .25f, .43f, .87f));
            input.Configure(joystick, pass, shoot, sprint);

            GameObject controls = new GameObject("Control visibility", typeof(RectTransform));
            Rect(controls, canvasGo.transform, new Vector2(.5f, 0),
                Vector2.zero, new Vector2(1, 1));
            TouchControlVisibility display = controls.AddComponent<TouchControlVisibility>();
            display.Configure(joystickBase, pass.gameObject, shoot.gameObject, sprint.gameObject);

            MatchHUD hud = canvasGo.AddComponent<MatchHUD>();
            hud.Configure(match, score, clock, restart);

            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            eventSystem.transform.position = Vector3.zero;
        }
    }
}
#endif
