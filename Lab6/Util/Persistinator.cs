using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace Lab6.Util;

public static class Persistinator
{
    private static bool _setup = false;
    public static string DataPath;

    public static void Setup()
    {
        DataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/Starship";
        Directory.CreateDirectory(DataPath);
        _setup = true;
    }

    private static void _Check()
    {
        if (!_setup) throw new Exception("Persistinator did not get set up yet! Call .Setup() first!");
    }

    public static void WriteToFile(String filename, string contents)
    {
        _Check();
        File.WriteAllText(DataPath + "/" + filename, contents);
    }

    public static String ReadFile(String filename)
    {
        _Check();
        try
        {
            return File.ReadAllText(DataPath + "/" + filename);
        }
        catch (FileNotFoundException _)
        {
            return "";
        }
    }

    public static int ReadInt(String filename)
    {
        if (int.TryParse(ReadFile(filename), out var res))
            return res;
        return 0;
    }
    
    
}