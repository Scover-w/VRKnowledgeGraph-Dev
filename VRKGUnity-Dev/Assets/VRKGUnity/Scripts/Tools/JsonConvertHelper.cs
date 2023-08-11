using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class JsonConvertHelper
{
    public static async Task<string> SerializeObjectAsync(object value)
    {
        return await Task.Run(() =>
        {
            return JsonConvert.SerializeObject(value);
        });
    }

    public static async Task<string> SerializeObjectAsync(object value, Formatting formatting)
    {
        return await Task.Run(() =>
        {
            return JsonConvert.SerializeObject(value, formatting);
        });
    }



    public static async Task<T> DeserializeObjectAsync<T>(string value)
    {
        return await Task.Run(() =>
        {
            return JsonConvert.DeserializeObject<T>(value);
        });
    }

}
