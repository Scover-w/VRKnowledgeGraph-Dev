using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;

public class NotificationUI : MonoBehaviour
{

    private void Start()
    {
        GraphDBAPI.OnErrorQuery += OnErrorQuery;
    }

    public async void OnErrorQuery(HttpResponseMessage responseMessage)
    {
        Debug.Log(responseMessage.StatusCode);
        Debug.Log(await responseMessage.Content.ReadAsStringAsync());
    }
}
