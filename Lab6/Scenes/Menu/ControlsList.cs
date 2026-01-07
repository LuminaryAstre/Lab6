using System.Collections.Generic;
using System.Reflection;
using Lab6.UI;
using Microsoft.Xna.Framework.Input;
using SoundFlow.Interfaces;
using Color = Microsoft.Xna.Framework.Color;

namespace Lab6.Scenes.Menu;

public class ControlsList : MenuScene
{
    public override string Title { get; set; } = "Controls";
    public override List<StarshipButton> Buttons { get; set; } = [];

    public override void Initialize()
    {
        foreach (var fieldInfo in typeof(StarshipKeybindings).GetFields())
        {
            Buttons.Add(new StarshipButton()
            {
                Name = $"{fieldInfo.Name}: {fieldInfo.GetValue(fieldInfo.GetValue(typeof(StarshipKeybindings))) ?? "None"}",
                Active = false,
                Outline = Color.DarkGray
            });
        }

        Buttons.Add(new StarshipButton()
        {
            Name = "Back",
            Callback = _ => MenuWrapper.Instance.SetActive(new MenuScene())
        });
        base.Initialize();
    }
}