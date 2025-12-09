using System.Collections.Generic;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    // ---- Tunables ----
    [Header("World")]
    public int worldSize = 40;           // plane is worldSize x worldSize
    public float cell = 2f;              // grid spacing for spawn placement
    public int cubeCount = 18;           // total cubes (evenly split R/G/B if possible)
    public int sphereCount = 18;         // total spheres (evenly split R/G/B if possible)

    [Header("Player")]
    public Vector3 playerStart = new Vector3(0f, 1.2f, 0f);
    public float playerHeight = 1.8f;

    [Header("Rules")]
    public int powerNeededToClearRedSpheres = 3;

    // Materials cached by color name
    private readonly Dictionary<string, Material> _mat = new();

    private void Awake()
    {
        // Ensure a stable random seed per session (optional)
        Random.InitState(System.Environment.TickCount);

        // 1) Materials
        CreateMaterials();

        // 2) Ground
        CreateGround();

        // 3) Player + Camera + Logic
        var player = CreatePlayer();
        var power = player.GetComponent<PlayerPower>();
        HUDController.Create(power); // <-- add this line


        // 4) Spawns
        SpawnWorld(player);
    }

    private void CreateMaterials()
    {
        _mat["red"]   = MakeLitMaterial(new Color(0.85f, 0.15f, 0.2f), "Mat_Red");
        _mat["green"] = MakeLitMaterial(new Color(0.15f, 0.7f, 0.2f),  "Mat_Green");
        _mat["blue"]  = MakeLitMaterial(new Color(0.2f, 0.45f, 0.95f), "Mat_Blue");
        _mat["gray"]  = MakeLitMaterial(new Color(0.5f, 0.5f, 0.5f),   "Mat_Gray");
    }

    private Material MakeLitMaterial(Color c, string name)
    {
        var m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        m.name = name;
        m.color = c;
        return m;
    }

    private void CreateGround()
    {
        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = Vector3.one * (worldSize / 10f); // Unity plane is 10x10 units
        var mr = ground.GetComponent<MeshRenderer>();
        mr.sharedMaterial = _mat["gray"];
        // Plane already has a MeshCollider; keep it non-trigger for walking
    }

    private GameObject CreatePlayer()
    {
        // Root
        var player = new GameObject("Player");
        player.transform.position = playerStart;

        // CharacterController
        var cc = player.AddComponent<CharacterController>();
        cc.height = playerHeight;
        cc.radius = 0.35f;
        cc.center = new Vector3(0f, playerHeight / 2f, 0f);

        // Power system
        var power = player.AddComponent<PlayerPower>();
        power.powerNeeded = powerNeededToClearRedSpheres;

        // Movement
        //var controller = player.AddComponent<PlayerController>();
        //controller.MoveSpeed = 6.0f;
        //controller.TurnSpeed = 150f;
        //controller.Gravity = 20f;

        GetComponent("MainCamera").transform.SetParent(player.transform);

        // Camera child
        //var camGo = new GameObject("PlayerCamera");
        //camGo.transform.SetParent(player.transform);
        //camGo.transform.localPosition = new Vector3(0f, playerHeight - 0.2f, 0f);
        //var cam = camGo.AddComponent<Camera>();
        //cam.nearClipPlane = 0.05f;

        // Light so the scene looks reasonable
        var lightGo = new GameObject("Directional Light");
        var light = lightGo.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.0f;
        lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        return player;
    }

    private void SpawnWorld(GameObject player)
    {
        var occupied = new HashSet<Vector2Int>();
        int half = worldSize / 2 - 1;

        // Helper: pick a free grid slot away from player start
        Vector3 NextPosition()
        {
            for (int tries = 0; tries < 500; tries++)
            {
                int gx = Random.Range(-half, half);
                int gz = Random.Range(-half, half);
                var key = new Vector2Int(gx, gz);
                if (occupied.Contains(key)) continue;

                // keep some radius clear around player start
                var worldPos = new Vector3(gx * cell, 0f, gz * cell);
                if (Vector3.Distance(worldPos, playerStart) < 3f) continue;

                occupied.Add(key);
                return worldPos;
            }
            // Fallback
            return Vector3.zero;
        }

        // Even distribution helper
        (int r, int g, int b) SplitRGB(int total)
        {
            int each = total / 3;
            int remainder = total % 3;
            return (each + (remainder > 0 ? 1 : 0),
                    each + (remainder > 1 ? 1 : 0),
                    each);
        }

        // Cubes
        var (rC, gC, bC) = SplitRGB(cubeCount);
        SpawnManyCubes("red", rC, NextPosition, player);
        SpawnManyCubes("green", gC, NextPosition, player);
        SpawnManyCubes("blue", bC, NextPosition, player);   // these are pickups

        // Spheres
        var (rS, gS, bS) = SplitRGB(sphereCount);
        SpawnManySpheres("red", rS, NextPosition, player);  // can disappear after power
        SpawnManySpheres("green", gS, NextPosition, player);
        SpawnManySpheres("blue", bS, NextPosition, player);
    }

    private void SpawnManyCubes(string colorKey, int count, System.Func<Vector3> posFn, GameObject player)
    {
        for (int i = 0; i < count; i++)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = $"{colorKey.ToUpperInvariant()}_Cube_{i}";
            go.transform.position = posFn() + Vector3.up * 0.5f;
            go.GetComponent<MeshRenderer>().sharedMaterial = _mat[colorKey];

            var col = go.GetComponent<Collider>();
            col.isTrigger = colorKey == "blue"; // blue cubes are pickups
            if (colorKey == "blue")
            {
                go.AddComponent<PickupBlueCube>();
            }
            else
            {
                // leave as static obstacle (non-trigger) except we made trigger false above
                col.isTrigger = false;
            }
        }
    }

    private void SpawnManySpheres(string colorKey, int count, System.Func<Vector3> posFn, GameObject player)
    {
        for (int i = 0; i < count; i++)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = $"{colorKey.ToUpperInvariant()}_Sphere_{i}";
            go.transform.position = posFn() + Vector3.up * 0.5f;
            go.GetComponent<MeshRenderer>().sharedMaterial = _mat[colorKey];

            var col = go.GetComponent<Collider>();
            col.isTrigger = true; // use trigger for interaction

            if (colorKey == "red")
            {
                go.AddComponent<RedSphereTarget>();
            }
            // blue/green spheres are inert triggers (no effect)
        }
    }
}
