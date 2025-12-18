using System;

namespace Lab6.UI;

[AttributeUsage(AttributeTargets.Field)]
public class BooleanOptionAttribute(string name, string category) : BaseStarshipAttribute(name, category);