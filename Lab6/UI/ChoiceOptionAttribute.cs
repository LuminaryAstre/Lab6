using System.Collections.Generic;
using System.Linq;

namespace Lab6.UI;

public class ChoiceOptionAttribute : BaseStarshipAttribute
{
    public List<string> Choices;
    public ChoiceOptionAttribute(string name, string category, string[] choices) : base(name, category)
    {
        Choices = choices.ToList();
    }
}