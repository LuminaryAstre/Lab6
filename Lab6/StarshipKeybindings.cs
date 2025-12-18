using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;

namespace Lab6;

public static class StarshipKeybindings
{
    public static readonly Keys? MoveForward = Keys.Up;
    public static readonly Keys? MoveBackward = Keys.Down;
    public static readonly Keys? RotateCounterClockwise = Keys.Left;
    public static readonly Keys? RotateClockwise = Keys.Right;
    public static readonly Keys? Shoot = null;//Keys.Z;

    public static bool WasKeyPressed(this Keys? key, KeyboardStateExtended kb)
    {
        return key != null && kb.WasKeyPressed((Keys)key);
    }

    public static bool IsKeyDown(this Keys? key, KeyboardState kb)
    {
        return key != null && kb.IsKeyDown((Keys)key);
    }

    public static bool IsKeyDown(this Keys? key, KeyboardStateExtended kb)
    {
        return key != null && kb.IsKeyDown((Keys)key);
    }
}