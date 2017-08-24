using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileOperate {

    public static bool IsFileExists(string path)
    {
        return new FileInfo(path).Exists;
    }

    public static ArrayList ReadFileToArray(string path)
    {
        path = "Assets/" + path;
        if (!IsFileExists(path))
        {
            return null;
        }

        StreamReader Reader = File.OpenText(path);
        string t_sLine;
        ArrayList t_aArrayList = new ArrayList();
        while ((t_sLine = Reader.ReadLine()) != null)
        {
            t_aArrayList.Add(t_sLine);
        }
        Reader.Close();
        Reader.Dispose();

        return t_aArrayList;
    }

    public static string ReadFileToString(string path)
    {
        path = "Assets/" + path;
        if (!IsFileExists(path)) return string.Empty;

        StreamReader Reader = File.OpenText(path);
        string all = Reader.ReadToEnd();
        Reader.Close();
        Reader.Dispose();

        return all;
    }
}
