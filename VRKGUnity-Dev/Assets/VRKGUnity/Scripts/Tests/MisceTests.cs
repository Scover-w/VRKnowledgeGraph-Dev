using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net;
using UnityEngine;
using static GraphDBAPI;
using VDS.RDF.Parsing;
using VDS.RDF;
using VDS.RDF.Writing;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Primitives;

public class MisceTests : MonoBehaviour
{
    [ContextMenu("InsertTest")]
    public async void InsertTest()
    {
        string rdfContent = await FileHelper.LoadAsync(Application.dataPath, "VRKGUnity", "Data", "cap44_1455283593", "wwwcidoc-crmorgcidoc-crm.rdf");

        IGraph graph = new VDS.RDF.Graph();


        if(!graph.TryLoadFromRdf(rdfContent)) 
        {
            return;
        }


        string turtleContent = graph.ToTurtle();

        var repo = new GraphDbRepository("http://localhost:7200/", "TestOntology");
        var api = new GraphDBAPI(repo);

        var bipbop = await api.LoadFileContentInDatabase(turtleContent, GraphDBAPIFileType.Turtle);
    }

    [ContextMenu("Select Test")]
    public async void SelectTest()
    {
        var repo = new GraphDbRepository("http://localhost:7200/", "TestOntology");
        var api = new GraphDBAPI(repo);

        Dictionary<char, double> frequencies = new Dictionary<char, double>()
        {
            { 'a', 8.167 },
            { 'b', 1.492 },
            { 'c', 2.782 },
            { 'd', 4.253 },
            { 'e', 12.702 },
            { 'f', 2.228 },
            { 'g', 2.015 },
            { 'h', 6.094 },
            { 'i', 6.966 },
            { 'j', 0.153 },
            { 'k', 0.772 },
            { 'l', 4.025 },
            { 'm', 2.406 },
            { 'n', 6.749 },
            { 'o', 7.507 },
            { 'p', 1.929 },
            { 'q', 0.095 },
            { 'r', 5.987 },
            { 's', 6.327 },
            { 't', 9.056 },
            { 'u', 2.758 },
            { 'v', 0.978 },
            { 'w', 2.360 },
            { 'x', 0.150 },
            { 'y', 1.974 },
            { 'z', 0.074 },
            { ' ', 18.000 } 
        };

        System.Random random = new System.Random();

        StringBuilder sb = new StringBuilder();
        sb.Append("select * where { ?s ?p ?o .}#");

        int lengthbase = sb.Length;

        int nb = 100000;
        int nbAdd = 100;

        for (int i = 0; i < nb; i += nbAdd)
        {
            for(int j = 0; j < nbAdd; j++)
                sb.Append(GetRandomCharacter());

            Debug.Log(i);
            var bipbop = await api.SelectQuery(sb.ToString());


            if (bipbop.Length == 0)
                return;

        }


        Debug.Log("Finish");  

        char GetRandomCharacter()
        {
            // Normalizing the frequencies
            double total = frequencies.Values.Sum();
            Dictionary<char, double> normalizedFrequencies = frequencies.ToDictionary(x => x.Key, x => x.Value / total);

            // Generating a random number
            double randomValue = random.NextDouble();

            // Looping over the characters to find which one our random value landed on
            double cumulative = 0.0;
            foreach (var entry in normalizedFrequencies)
            {
                cumulative += entry.Value;
                if (randomValue <= cumulative)
                {
                    return entry.Key;
                }
            }

            throw new Exception("Should never get here if the probabilities are correctly normalized");
        }
    }
}
