using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public static class FileHelper
{
    public static async Task SaveAsync(string content, params string[] paths)
    {
        var path = Path.Combine(paths);

        string directory = Path.GetDirectoryName(path);

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        await File.WriteAllTextAsync(path, content);
    }

    public static void Save(string content, params string[] paths)
    {
        var path = Path.Combine(paths);

        string directory = Path.GetDirectoryName(path);

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(path, content);
    }

    public static async Task<string> LoadAsync(params string[] paths)
    {
        var path = Path.Combine(paths);

        if (File.Exists(path))
        {
            return await File.ReadAllTextAsync(path);
        }

        Debug.LogWarning("FileHelper : " + path + " doesn't exist.");
        return "";

    }

    public static string Load(params string[] paths)
    {
        var path = Path.Combine(paths);

        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }

        Debug.LogWarning("FileHelper : " + path + " doesn't exist.");
        return "";
    }

}
