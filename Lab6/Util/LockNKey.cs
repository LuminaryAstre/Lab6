using Microsoft.Xna.Framework.Input;

namespace Lab6.Util;

// Utility class to make .IsKeyDown be true for exactly one tick
// because i don't want 700 properties for checking if a key was held down
// last tick.
public static class LockNKey
{
    // private static Dictionary<Keys, bool> _nextState = new();
    // private static Dictionary<Keys, bool> _state = new();
    private static KeyboardState _state;
    private static KeyboardState _lastState;

    public static void Initialize()
    {
        Reactor game = Reactor._instance;
        _lastState = new KeyboardState();
        _state = Keyboard.GetState();
        // game.Window.KeyDown += (sender, args) =>
        // {
        //     _nextState[args.Key] = true;
        // };

        // game.Window.KeyUp += (sender, args) =>
        // {
        //     _state[args.Key] = false;
        //     _nextState[args.Key] = false;
        // };
    }

    public static bool WasPressed(this KeyboardState kb, Keys key)
    {
        return _state.IsKeyDown(key) && _lastState.IsKeyUp(key);
    }

    public static void Tick()
    {
        _lastState = _state;
        _state = Keyboard.GetState();
    }
}
