using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using UnityEngine;

public class MisceTests : MonoBehaviour
{
    [Test]
    public void TestNoDoubleInList()
    {

        List<int> list = new List<int>();
        HashSet<int> set = new HashSet<int>();

        for (int i = 0; i < 1000; i++)
        {
            list.Add(i);
        }

        int nbInList = list.Count;

        for (int i = 0; i < nbInList; i++)
        {
            for (int j = i + 1; j < nbInList; j++)
            {
                var num = list[i] + list[j] * 10000000;

                Assert.False(set.Contains(num));

                set.Add(num);

            }
        }
    }

    [Test]
    public void TestColorConversion()
    {
        var systemColor = System.Drawing.Color.AliceBlue;

        var unityColor = systemColor.ToUnityColor();

        var systemColorB = unityColor.ToSystemColor();

        Assert.True(systemColorB.Equals(systemColor));

    }

    [Test]
    public void TestHashSetDuplicate()
    {
        HashSet<int> set = new HashSet<int>();


        set.Add(1);
        set.Add(1);

    }

    [Test]
    public void GetIps()
    {
        string hostName = Dns.GetHostName(); // Retrieve the Name of HOST
                                             // Get the IP
        var ipEntry = Dns.GetHostEntry(hostName);
        var addr = ipEntry.AddressList;
        foreach (var a in addr)
        {
            if (a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) // Ensure we have an IPv4 address
            {
                Debug.Log(a.ToString());
            }
        }
    }

    [Test]
    public void TestGateway()
    {
        foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            foreach (GatewayIPAddressInformation gatewayAddress in networkInterface.GetIPProperties().GatewayAddresses)
            {
                Debug.Log(gatewayAddress.Address.ToString());
            }
        }
    }
}
