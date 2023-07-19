using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VDS.RDF;

public class OntologyPlaygroundTest : MonoBehaviour
{

    public string OntologyToLoadFileName = "";
    public string OntologyToLoadUri = "";

    [ContextMenu("Load Ontology")]
    private async void LoadOntology()
    {
        var xmlContent = await FileHelper.LoadAsync(Application.persistentDataPath, "Data", "cap44_1455283593", OntologyToLoadFileName);


        IGraph graph = new VDS.RDF.Graph();

        if (!graph.TryLoadFromRdf(xmlContent))
            return;

        graph.CleanFromLabelAndComment();

        TestOntology(graph);

    }


    private void TestOntology(IGraph graph)
    {
        Dictionary<int, OntoNode> ontoNodes = new();
        Dictionary<int, OntoEdge> ontoEdges = new();

        List<Triple> noUsableTriple = new();

        foreach (Triple triple in graph.Triples)
        {
            string sValue = triple.Subject.ToString();
            string pValue = triple.Predicate.ToString();
            string oValue = triple.Object.ToString();


            if (pValue == "http://www.w3.org/2000/01/rdf-schema#subClassOf")
            {
                AddSubclassOf(sValue, oValue);
                continue;
            }

            noUsableTriple.Add(triple);

        }

        Debug.Log(noUsableTriple);


        if (ontoNodes.Count == 0)
            Debug.Log("cc");

        //return new OntologyTree(ontoNodes, ontoEdges, uri);


        void AddSubclassOf(string sValue, string oValue)
        {

            OntoNode ontoNodeSubject = GetOntoNode(sValue);
            OntoNode ontoNodeObject = GetOntoNode(oValue);

            ontoNodeObject.OntoNodeTarget.Add(ontoNodeSubject);
            ontoNodeSubject.OntoNodeSource.Add(ontoNodeObject);
        }

        OntoNode GetOntoNode(string ontoNodeValue)
        {
            var idOntoNode = ontoNodeValue.GetHashCode();

            if (ontoNodes.TryGetValue(idOntoNode, out OntoNode ontoNodeB))
            {
                return ontoNodeB;
            }
            else
            {
                var ontoNode = new OntoNode(ontoNodeValue);
                ontoNodes.Add(idOntoNode, ontoNode);
                return ontoNode;
            }
        }
    }


    [ContextMenu("Test Create Tree")]
    private void TestCreateTree()
    {
        //var repo = new GraphDbRepository()




    }

}
