using System.Collections.Generic;
using System.Reflection;
using Lab6.UI;
using Microsoft.Xna.Framework;

namespace Lab6.Scenes.Menu;

public class OptionsScene : MenuScene
{
    public override string Title { get; set; } = "Options";
    public override List<StarshipButton> Buttons { get; set; } = [];
    public Dictionary<string, List<StarshipButton>> Categories = [];
    public List<StarshipButton> Main = [];

    public void SetActiveCategory(string name = null)
    {
        if (name == null)
        {
            Buttons = Main;
            Title = "Options";
            return;
        }

        if (Categories.TryGetValue(name, out var cat))
        {
            Buttons = cat;
            Title = name;
        }
        else
            SetActiveCategory();
    }
    
    public override void Initialize()
    {
        foreach (var fieldInfo in typeof(StarshipSettings).GetFields())
        {
            BaseStarshipAttribute opt = fieldInfo.GetCustomAttribute<BaseStarshipAttribute>();
            if (opt == null) continue;
            if (!Categories.TryGetValue(opt.Category, out _))
                Categories.Add(opt.Category, []);
            var cat = Categories[opt.Category];

            if (opt is BooleanOptionAttribute)
            {
                bool value = (bool)fieldInfo.GetValue(typeof(StarshipSettings))!;
                cat.Add(new StarshipButton
                {
                    Name = $"{opt.Name}: {value}",
                    Callback = button =>
                    {
                        bool current = (bool)fieldInfo.GetValue(typeof(StarshipSettings))!;
                        fieldInfo.SetValue(typeof(StarshipSettings), !current);
                        button.Outline = !current ? Color.Green : Color.Red;
                        button.Name = $"{opt.Name}: {!current}";
                    },
                    Outline = value ? Color.Green : Color.Red
                });
            } else if (opt is ChoiceOptionAttribute ch)
            {
                string value = (string)fieldInfo.GetValue(typeof(StarshipSettings))!;
                cat.Add(new StarshipButton
                {
                    Name = $"{ch.Name}: {value}",
                    Callback = button =>
                    {
                        string current = (string)fieldInfo.GetValue(typeof(StarshipSettings))!;
                        var index = ch.Choices.IndexOf(current);
                        var newIndex = (index + 1) % ch.Choices.Count;
                        var newCurrent = ch.Choices[newIndex];
                        fieldInfo.SetValue(typeof(StarshipSettings), newCurrent);
                        button.Name = $"{ch.Name}: {newCurrent}";
                    }
                });
            }
        }

        foreach (var catName in Categories.Keys)
        {
            List<StarshipButton> cat = Categories[catName];
            cat.Add(new StarshipButton
            {
                Name = "Back",
                Callback = button => SetActiveCategory()
            });
            Main.Add(new StarshipButton
            {
                Name = catName,
                Callback = button => SetActiveCategory(catName)
            });
        }

        Main.Add(new StarshipButton
        {
            Name = "Back",
            Callback = button => MenuWrapper.Instance.SetActive(new MenuScene())
        });

        Buttons = Main;
        base.Initialize();
    }
}