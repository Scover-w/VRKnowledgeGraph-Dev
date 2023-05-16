using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GraphDBAPI;
using System.Net.Http;
using System.Net;
using System;
using UnityEditor.Search;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor.Graphs;
using System.Xml;
using static System.Net.WebRequestMethods;
using System.Xml.Linq;
using System.Data.SqlTypes;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;

public class NodeUriRetrievalTest : MonoBehaviour
{
    [Test]
    public async void TestQueryUri()
    {
        string[] url = new string[] { "https://sws.geonames.org/2990969",
                                       "http://data.culture.fr/thesaurus/resource/ark:/67717/T96-834",
                                        "http://viaf.org/viaf/21377236"};

        try
        {
            using (HttpClient client = new HttpClient())
            {
                string encodedUrl = System.Net.WebUtility.UrlEncode(url[0]);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url[0]);
                request.Headers.Add("Accept", "application/rdf+xml");

                HttpResponseMessage response;

                response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                Debug.Log(responseBody);

                TestXMLUri(responseBody);


            }

        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
    }

    public static string TestXML(string xmlContent)
    {
        XElement root = XElement.Parse(xmlContent);
        XAttribute nameAttribute = null;

        // Find the "name" attribute in the "property" element
        foreach (XElement element in root.Descendants("property"))
        {
            nameAttribute = element.Attribute("name");
            if (nameAttribute != null && nameAttribute.Value == "name")
            {
                break;
            }
        }

        if (nameAttribute != null)
        {
            // Get the value of the "property" element with the "name" attribute
            string nameValue = nameAttribute.Parent.Value;
            Console.WriteLine($"Name value: {nameValue}");
            return nameValue;
        }
        else
        {
            Console.WriteLine("No \"name\" attribute found.");
        }

        return null;
    }

    public static string ExtractNameFromXml(string xmlContent)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlContent);

        XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
        namespaceManager.AddNamespace("rdfs", "http://www.w3.org/2000/01/rdf-schema#");
        namespaceManager.AddNamespace("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
        namespaceManager.AddNamespace("skos", "http://www.w3.org/2004/02/skos/core#");
        namespaceManager.AddNamespace("skos1", "http://www.w3.org/2004/02/skos/core#");
        namespaceManager.AddNamespace("dcterms", "http://purl.org/dc/terms/");
    
        XmlNode nameNode = null;

        // Try to get name from 'gn:name' element
        nameNode = xmlDoc.SelectSingleNode("//rdfs:label", namespaceManager);

        if (nameNode != null)
            return nameNode.InnerText;

        nameNode = xmlDoc.SelectSingleNode("//skos:prefLabel", namespaceManager);

        if (nameNode != null)
            return nameNode.InnerText;

        nameNode = xmlDoc.SelectSingleNode("//skos1:altLabel", namespaceManager);

        if (nameNode != null)
            return nameNode.InnerText;

        nameNode = xmlDoc.SelectSingleNode("//dcterms:title", namespaceManager);

        if (nameNode != null)
            return nameNode.InnerText;

        return null;

    }


    public void TestXMLUri(string xmlContent)
    {



        List<XmlLine> _xmlLines = new List<XmlLine>();

        using (StringReader stringReader = new StringReader(xmlContent))
        using (XmlReader xmlReader = XmlReader.Create(stringReader))
        {
            while (xmlReader.Read())
            {
                if ((xmlReader.NodeType != XmlNodeType.Element))
                    continue;

                var name = xmlReader.Name.ToLower();

                if (!(name.Contains("label") || name.Contains("title") || name.Contains("name")))
                    continue;

                _xmlLines.Add(new XmlLine(xmlReader));
                
            }
        }
    }

    public class XmlLine
    {
        public string Name;
        public string Content;

        public Dictionary<string, string> Attributes;

        public XmlLine(XmlReader xmlReader)
        {
            Name = xmlReader.Name;
            Content = xmlReader.ReadElementContentAsString();

            Attributes = new();

            string lang = xmlReader.GetAttribute("xml:lang");


            if (!xmlReader.HasAttributes)
                return;


            xmlReader.MoveToFirstAttribute();
            do
            {
                Attributes.Add(xmlReader.Name, xmlReader.Value);
            }
            while (xmlReader.MoveToNextAttribute());

            // xml:lang="en"
            xmlReader.MoveToElement();
        }
    }


    [Test]
    public void TestUrisSplit()
    {
        string[] uris = new string[] { "http://www.productontology.org/id/qgrdg", "http://purl.org/dc/terms/jului", "http://www.w3.org/1999/02/22-rdf-syntax-ns#azertyh" };
        string[] prefixUris = new string[] { "http://www.productontology.org/id/", "http://purl.org/dc/terms/", "http://www.w3.org/1999/02/22-rdf-syntax-ns#" };
        string[] suffixUris = new string[] { "qgrdg", "jului", "azertyh" };

        int nburis = uris.Length;

        for (int i = 0; i < nburis; i++)
        {
            UriHelper.Split(uris[i], out string prefix, out string suffix);

            Assert.AreEqual(prefix, prefixUris[i]);
            Assert.AreEqual(suffix, suffixUris[i]);
        }   
    }
}