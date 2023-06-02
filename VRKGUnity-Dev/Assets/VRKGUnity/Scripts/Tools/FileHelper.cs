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
}
