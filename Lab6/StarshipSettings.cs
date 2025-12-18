using Lab6.UI;
using Lab6.Util;

namespace Lab6;

public static class StarshipSettings
{
    [BooleanOption("Delta Decay", "Physics")] public static bool DeltaDecay = true;
    [BooleanOption("Delta Movement", "Physics")] public static bool PhysicsMovement = true;

    [ChoiceOption("Collision", "Physics", ["Normal", "Cheap", "Cheaper", "Cheapest"])] public static string CollisionType = "Normal";
    
    [BooleanOption("Debug Camera", "Render")] public static bool DebugCam = false;
    
    [BooleanOption("Debug Objects", "Render")] public static bool DebugObjects = false;
    
}
