using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class IPTest : MonoBehaviour
{

    [SerializeField]
    TMP_Text _text;

    // Start is called before the first frame update
    void Start()
    {
        _text.text = "";
        //DisplayIps();
        //StartCoroutine(GetIP());
        GetDefaultGateway();
    }

    private void DisplayIps()
    {
        string hostName = Dns.GetHostName(); // Retrieve the Name of HOST
                                             // Get the IP
        var ipEntry = Dns.GetHostEntry(hostName);
        var addr = ipEntry.AddressList;
        foreach (var a in addr)
        {
            if (a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) // Ensure we have an IPv4 address
            {
                _text.text += a.ToString() + "\n";
            }
        }
    }

    public void GetDefaultGateway()
    {
        foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            _text.text += ($"Name: {networkInterface.Name}, Status: {networkInterface.OperationalStatus}");
            var gatewayAddresses = networkInterface.GetIPProperties().GatewayAddresses;
            foreach (GatewayIPAddressInformation gatewayAddress in gatewayAddresses)
            {
                _text.text += ($"Gateway: {gatewayAddress.Address}");
            }
            var ipAddresses = networkInterface.GetIPProperties().UnicastAddresses;
            foreach (UnicastIPAddressInformation ipAddress in ipAddresses)
            {
                _text.text += ($"IP: {ipAddress.Address}");
            }
        }
    }

    IEnumerator GetIP()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("https://api.ipify.org"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Uh oh, something went wrong with getting the IP: " + www.error);
            }
            else
            {
                _text.text = "My public IP: " + www.downloadHandler.text;
            }
        }
    }
}
