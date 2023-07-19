using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using VDS.RDF;
using System.IO;
using System;
using System.Xml.Linq;

public class PrefixListTest
{
    [Test]
    public void LoadPrefixsInDict()
    {
        var path = Path.Combine(Application.streamingAssetsPath, "prefixsList.ttl");


        Dictionary<string, string> namespaceAndPrefixs = new();

        using StreamReader reader = new(path);
        string line;

        while ((line = reader.ReadLine()) != null)
        {

            line = line.Replace("@prefix ", "");

            var elements = line.Split("<");

            string prefix = elements[0];
            string namespce = elements[1];


            prefix = prefix.Replace(" ", "").Replace(":", "");
            namespce = namespce.Replace(">", "");
            namespce = namespce.Substring(0, namespce.Length - 1);

            namespaceAndPrefixs.Add(namespce, prefix);


        }

    }


    [Test]
    public void ExtractPrefixFromRdf()
    {
        string rdf = FileHelper.Load(Application.dataPath, "VRKGUnity", "Data", "Tests", "CidocCrm.json");

        XElement root = XElement.Parse(rdf);

        var elements = root.Elements();

        foreach(XElement element in elements)
        {
            Debug.Log(element);
        }

    }
}
