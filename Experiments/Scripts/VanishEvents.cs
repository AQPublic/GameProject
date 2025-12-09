using System;
public static class VanishEvents
{
    // type examples: "Blue Cube", "Red Sphere"
    public static event Action<string> OnElementVanished;

    public static void Report(string type)
    {
        OnElementVanished?.Invoke(type);
    }
}
