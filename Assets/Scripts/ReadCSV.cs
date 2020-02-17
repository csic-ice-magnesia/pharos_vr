using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulsarDatabase
{
    public static void Read(
        string filename)
    {
        var filedata = System.IO.File.ReadAllText(filename);
        string[] lines = filedata.Split('\n');

        for (int i = 0; i < lines.Length; ++i)
        {
            string[] lineData = (lines[i].Trim()).Split(',');

        }
    }
}
