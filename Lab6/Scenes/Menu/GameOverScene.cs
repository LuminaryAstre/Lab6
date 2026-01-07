using System.Collections.Generic;
using Lab6.Scenes.Game;
using Lab6.UI;
using Microsoft.Xna.Framework;

namespace Lab6.Scenes.Menu;

public class GameOverScene(int score) : MenuScene
{
    public int Score = score;
    public override string Title { get; set; } = "Game Over";

    public override List<StarshipButton> Buttons { get; set; } = new List<StarshipButton>()
    {
        new StarshipButton()
        {
            Name = "{Score} points",
            Outline = Color.DarkGray,
            Active = false
        },
        new StarshipButton()
        {
            Name = "Retry",
            Outline = Color.Gold,
            Callback = _ => Reactor._instance.SetActive(new GameScene())
        },
        new StarshipButton()
        {
            Name = "Exit",
            Outline = Color.Crimson,
            Callback = _ => MenuWrapper.Instance.SetActive(new MenuScene())
        }
    };

    public override void Initialize()
    {
        Buttons[0].Name = Buttons[0].Name.Replace("{Score}", Score.ToString());
        base.Initialize();
    }
}