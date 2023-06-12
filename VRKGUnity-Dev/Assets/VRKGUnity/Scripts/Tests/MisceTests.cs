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
}
