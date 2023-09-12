using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MisceTests : MonoBehaviour
{
    [Test]
    public void TestNoDoubleInList()
    {

        List<int> list = new();
        HashSet<int> set = new();

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

    [Test]
    public void GUUID()
    {
        Guid guid = Guid.NewGuid();
        string uniqueId = guid.ToString();
        Debug.Log("http://www.cidoc-crm.org/cidoc-crm/".Length + "  " + uniqueId);

    }

    [Test]
    public void TestVector()
    {
        Vector3 v = new(1f, 2f, 3f);
        Debug.Log(v.ToString());
        Vector2 v2 = v;
        Debug.Log(v2.ToString());

        Vector3 vB = v2;
        Debug.Log(vB.ToString());


    }

    [Test]
    public void TestChangeScript()
    {
        string scriptPath = Application.dataPath + "/VRKGUnity/Scripts/Scenes/MainMenuManager.cs";
        string scriptText = System.IO.File.ReadAllText(scriptPath);

        string[] lines = scriptText.Split("http://");
        string oldIp = lines[1].Split('"')[0]; // 192.168.137.1:7200/

        Debug.Log(oldIp);
    }

    [Test]
    public void TestNameOf()
    {
        Debug.Log(nameof(GraphConfiguration.Instance.SimuParameters));
    }

    [Test]
    public void GetPersistentLink()
    {
        Debug.Log(Application.persistentDataPath);
    }

    [Test]
    public async void IsConnectedToInternet()
    {
        var result = await HttpHelper.IsConnectedToInternet();
        Debug.Log(result);

        await Task.Run(() =>
        {
            Thread.Sleep(5000);
        });

        Debug.Log("5s");
    }

    [Test]
    public async void PingRepo()
    {
        var result = await HttpHelper.Ping("http://localhost:7200/");
        Debug.Log(result);
    }

    [Test]
    public async void RetrieveFrench()
    {
        var xmlContent = await HttpHelper.RetrieveRdf("https://viaf.org/viaf/143903205/");

        if (xmlContent == null || xmlContent.Length == 0)
        {
            return;
        }

        File.WriteAllText(Application.persistentDataPath + "/Test.ttl", xmlContent);
        Debug.Log(Application.persistentDataPath + "Test.ttl");

        if (ExtractName(xmlContent, out string property, out string value))
        {
           
            return;
        }
    }



    private bool ExtractName(string xmlContent, out string property, out string value)
    {
        property = "";
        value = "";

        List<(string, string)> lst = new();


        try
        {
            using StringReader stringReader = new(xmlContent);
            using XmlReader xmlReader = XmlReader.Create(stringReader);

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType != XmlNodeType.Element)
                    continue;

                var name = xmlReader.Name.ToLower();

                if (!(name.Contains("label") || name.Contains("title") || name.Contains("name")))
                    continue;

                property = xmlReader.Name;
                value = xmlReader.ReadElementContentAsString();

                lst.Add((name, value));
                //return true;
            }
        }
        catch (Exception e)
        {
            return false;
        }

        Debug.Log("Bipbop");

        return false;
    }

    [Test]
    public void TestIntToEnum()
    {
        GraphMetricType graphMetricA = GraphMetricType.None;
        int intA = (int)graphMetricA;


        Assert.True(intA.TryParseToEnum(out GraphMetricType graphMetricType));
        Assert.False(156.TryParseToEnum(out GraphMetricType graphMetricTypeB));

    }
}
