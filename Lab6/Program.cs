using System.Reflection;
using HarmonyLib;
using Lab6.Scenes;
using Lab6.Scenes.Menu;

namespace Lab6;

internal class Program
{
    public static void Main(string[] args)
    {
        // Uncomment if I ever need Harmony again
        // var harmony = new Harmony("dev.astre.starship");
        // harmony.PatchAll(Assembly.GetExecutingAssembly());
        
        using var game = new Reactor();
        game.SetActive(new IntroScene());
        game.Run();
    }
}