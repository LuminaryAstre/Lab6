using System;

namespace Lab6.UI;

[AttributeUsage(AttributeTargets.Field)]
public class BaseStarshipAttribute : Attribute
{
    public string Name;
    public string Category;

    public BaseStarshipAttribute(string name, string category)
    {
        Name = name;
        Category = category;
    }
}