using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using VDS.RDF;
using VDS.RDF.Query.Expressions.Functions.XPath.String;

public class OntologyTree
{
    Dictionary<int, OntoNode> _ontoNodes;


    private OntologyTree(Dictionary<int, OntoNode> ontoNodes)
    {
        _ontoNodes = ontoNodes;
    }

    public static async Task<OntologyTree> CreateAsync(IGraph graph)
    {
        return await Task.Run(() =>
        {
            OntologyTree ontologyTree = CreateOntologyTree(graph);
            return ontologyTree;
        });
    }

    private static OntologyTree CreateOntologyTree(IGraph graph) 
    {
        Dictionary<int, OntoNode> ontoNodes = new();

        foreach(Triple triple in graph.Triples)
        {
            string subject = triple.Subject.ToString();
            string predicate = triple.Predicate.ToString();
            string obj = triple.Object.ToString();

            if(predicate != "http://www.w3.org/2000/01/rdf-schema#subClassOf")
            {
                continue;
            }

            var idSubject = subject.GetHashCode();
            var idObject = obj.GetHashCode();

            OntoNode ontoNodeSubject;
            OntoNode ontoNodeObject;

            if (ontoNodes.TryGetValue(idSubject, out OntoNode ontoNode))
            {
                ontoNodeSubject = ontoNode;

                Debug.Log("Already exist : " + subject + "  /  " + ontoNode.Value);
            }
            else
            {
                ontoNodeSubject = new OntoNode(idSubject, subject);
                ontoNodes.Add(idSubject, ontoNodeSubject);
            }

            if (ontoNodes.TryGetValue(idObject, out OntoNode ontoNodeB))
            {
                ontoNodeObject = ontoNodeB;

                Debug.Log("Already exist : " + obj + "  /  " + ontoNodeB.Value);
            }
            else
            {
                ontoNodeObject = new OntoNode(idObject, obj);
                ontoNodes.Add(idObject, ontoNodeObject);
            }


            ontoNodeObject.NodeTarget.Add(ontoNodeSubject);
            ontoNodeSubject.NodeSource.Add(ontoNodeObject);

        }

        return new OntologyTree(ontoNodes);
    }


    public OntoNode GetSource()
    {
        var ontoNode = _ontoNodes.First().Value;



        while(ontoNode.NodeSource.Count > 0) 
        { 
            ontoNode = ontoNode.NodeSource[0];
        }

        return ontoNode;
    }

    public void SaveToFile()
    {
        List<string> names = new();

        foreach(var s in _ontoNodes)
        {
            names.Add(s.Value.Value.Replace("http://www.cidoc-crm.org/cidoc-crm/", "").Replace("_"," "));
        }

        names.Sort();

        string filePath = Path.Combine(Application.dataPath, "VRKGUnity", "Data", "Tests", "cidocNames.txt");

        // Create a new file or overwrite existing file
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (string str in names)
            {
                // Write each string on a new line
                writer.WriteLine(str);
            }
        }

        Debug.Log("Done");
    }
}



public class OntoNode
{
    public int Id;
    public string Value;

    public List<OntoNode> NodeSource;
    public List<OntoNode> NodeTarget;

    public OntoNode(int id, string value)
    {
        Id = id;
        Value = value;  

        NodeSource = new List<OntoNode>();
        NodeTarget = new List<OntoNode>();
    }

}
