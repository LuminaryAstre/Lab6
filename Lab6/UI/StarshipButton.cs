using System;
using Microsoft.Xna.Framework;

namespace Lab6.UI;

public class StarshipButton
{
    public string Name = "[PLACEHOLDER]";
    public Color Outline = Color.White;
    public Action<StarshipButton> Callback = (btn) => { };
    public bool Active = true;
}