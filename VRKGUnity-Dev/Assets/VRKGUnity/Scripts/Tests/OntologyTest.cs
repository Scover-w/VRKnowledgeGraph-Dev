using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using VDS.RDF;
using VDS.RDF.Parsing;

public class OntologyTest : MonoBehaviour
{
    public string OntologyUri;
    public string OntologyName;
    public List<string> Ontologies;

    GraphDBAPI _api;
    Ontology _ontology;

    // Start is called before the first frame update
    void Start()
    {
        //RetrieveAllAndSaveUris();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Retrieve")]
    public async void Retrieve()
    {

        var json = await _api.Query("select * where { \r\n\t?s ?p ?o .\r\n}");

        var data = JsonConvert.DeserializeObject<JObject>(json);

        var nodges = new Nodges();

        var nodesDicId = nodges.NodesDicId;
        var edgesDicId = nodges.EdgesDicId;


        Dictionary<string, int> fullOnto = new();
        Dictionary<string, int> partialOnto = new();
        Dictionary<string, int> noOnto = new();

        foreach (JToken binding in data["results"]["bindings"])
        {
            string sType = binding["s"]["type"].Value<string>();
            string sValue = binding["s"]["value"].Value<string>();

            string pType = binding["p"]["type"].Value<string>();
            string pValue = binding["p"]["value"].Value<string>();

            string oType = binding["o"]["type"].Value<string>();
            string oValue = binding["o"]["value"].Value<string>();


            int sId = (sType + sValue).GetHashCode();
            int oId = (oType + oValue).GetHashCode();


            Node s;
            Node o;

            bool isSExist = false;
            bool isPExist = false;
            bool isOExist = false;


            if (nodesDicId.TryGetValue(sId, out Node sNodeExisting))
            {
                s = sNodeExisting;
                isSExist = true;
            }
            else
            {
                s = new Node(sId, sType, sValue);
                nodesDicId.Add(sId, s);
            }

            if (nodesDicId.TryGetValue(oId, out Node oNodeExisting))
            {
                isOExist = true;
                o = oNodeExisting;
            }
            else
            {
                o = new Node(oId, oType, oValue);
                nodesDicId.Add(oId, o);
            }

            var edge = new Edge(pType, pValue, s, o);

            if (edgesDicId.TryGetValue(edge.Id, out Edge edgeExisting))
            {
                isPExist = true;
            }
            else
            {
                edgesDicId.Add(edge.Id, edge);  
                s.EdgeSource.Add(edge);
                o.EdgeTarget.Add(edge);

            }

            bool isSOntology = _ontology.IsOntology(sValue);
            bool isPOntology = _ontology.IsOntology(pValue);
            bool isOOntology = _ontology.IsOntology(oValue);

            if(isSOntology && isPOntology && isOOntology)
            {
                // Full Ontology Triplet
                var sNamespce = sValue.ExtractUri().namespce;
                var pNamespce = pValue.ExtractUri().namespce;
                var oNamespce = oValue.ExtractUri().namespce;

                TryAdd(sNamespce, isSExist, sType, fullOnto);
                TryAdd(pNamespce, isPExist, pType, fullOnto);
                TryAdd(oNamespce, isOExist, oType, fullOnto);

            }
            else if(isSOntology || isPOntology || isOOntology)
            {
                // Partial Ontology
                
                // Full Ontology Triplet
                var sNamespce = sValue.ExtractUri().namespce;
                var pNamespce = pValue.ExtractUri().namespce;
                var oNamespce = oValue.ExtractUri().namespce;


                TryAdd(sNamespce, isSExist, sType, partialOnto);
                TryAdd(pNamespce, isPExist, pType, partialOnto);
                TryAdd(oNamespce, isOExist, oType, partialOnto);
            }
            else
            {

                Debug.Log(sValue + "  " + pValue + "  " + sValue);

                var sNamespce = sValue.ExtractUri().namespce;
                var pNamespce = pValue.ExtractUri().namespce;
                var oNamespce = oValue.ExtractUri().namespce;

                TryAdd(sNamespce, isSExist, sType, noOnto);
                TryAdd(pNamespce, isPExist, pType, noOnto);
                TryAdd(oNamespce, isOExist, oType, noOnto);
            }

        }


        Debug.Log(fullOnto);
        Debug.Log(partialOnto);
        Debug.Log(noOnto);

        void TryAdd(string namespce, bool nodeAlreadyExist, string type, Dictionary<string, int> onto)
        {
            if (type != "uri" || nodeAlreadyExist)
                return;

            if (onto.ContainsKey(namespce))
            {
                onto[namespce]++;
            }
            else
            {
                onto.Add(namespce, 1);
            }
        }

    }

    [ContextMenu("RetrieveAllAndSaveUris")]
    public async void RetrieveAllAndSaveUris()
    {

        GraphDbRepository graphDbRepository = new GraphDbRepository("http://localhost:7200/", "cap44");

        _api = new GraphDBAPI(graphDbRepository);

        var childs = await graphDbRepository.LoadChilds();

        GraphDbRepositoryOntology graphOntology = childs.ontology;

        var json = await _api.Query("select * where { \r\n\t?s ?p ?o .\r\n}");

        var data = JsonConvert.DeserializeObject<JObject>(json);


        foreach (JToken binding in data["results"]["bindings"])
        {
            string sType = binding["s"]["type"].Value<string>();
            string sValue = binding["s"]["value"].Value<string>();

            string pType = binding["p"]["type"].Value<string>();
            string pValue = binding["p"]["value"].Value<string>();

            string oType = binding["o"]["type"].Value<string>();
            string oValue = binding["o"]["value"].Value<string>();


            if (sType == "uri" && sValue.StartsWith("http"))
            {
                var uri = sValue.ExtractUri();
                graphOntology.AddWithoutSave(uri.namespce);
            }

            if (pType == "uri" && pValue.StartsWith("http"))
            {
                var uri = pValue.ExtractUri();
                graphOntology.AddWithoutSave(uri.namespce);
            }

            if (oType == "uri" && oValue.StartsWith("http"))
            {
                var uri = oValue.ExtractUri();
                graphOntology.AddWithoutSave(uri.namespce);
            }
        }

        await graphOntology.Save();

        var ontology = new Ontology(graphOntology);
    }

    [ContextMenu("OntologyTreee")]
    public async void OntologyTreee()
    {
        //var ontologyUri = "http://www.cidoc-crm.org/cidoc-crm/";

        try
        {
            RdfXmlParser parser = new RdfXmlParser();
            IGraph graph = new VDS.RDF.Graph();

            string xmlContent = await HttpHelper.RetrieveRdf(OntologyUri);

            using (StringReader reader = new StringReader(xmlContent))
            {
                parser.Load(graph, reader);
            }


            var ontologyTree = await OntologyTree.CreateAsync(graph);

            var firstOntoNode = ontologyTree.GetSource();

            ontologyTree.SaveToFile();
            return;
            var def = graph.AllNodes;

            List<Triple> triplesToRemove = new();

            foreach (Triple triple in graph.Triples)
            {
                // Access the subject, predicate, and object of each triple
                string subject = triple.Subject.ToString();
                string predicate = triple.Predicate.ToString();
                string obj = triple.Object.ToString();

                if(!(subject.Contains(OntologyUri) || predicate.Contains(OntologyUri) || obj.Contains(OntologyUri)))
                {
                    triplesToRemove.Add(triple);
                    continue;
                }


                if (predicate == "http://www.w3.org/2000/01/rdf-schema#comment")
                {
                    triplesToRemove.Add(triple);
                    continue;
                }

                bool isLabel = (predicate == "http://www.w3.org/2000/01/rdf-schema#label");

                if (isLabel)
                {
                    triplesToRemove.Add(triple);
                    continue;
                }
                else if(obj.Contains("@en"))
                {
                    obj.Replace("@en", "");// TODO: useless here soneedapplyit somewhere else
                }

                // Process the triple as needed
                // For example, you can print the triple components
                Console.WriteLine($"Subject: {subject}");
                Console.WriteLine($"Predicate: {predicate}");
                Console.WriteLine($"Object: {obj}");
                Console.WriteLine();
            }


            graph.Retract(triplesToRemove);

            List<Triple> goood = graph.Triples.ToList();


            var node = def.First();

        }
        catch(Exception ex) 
        {
            Debug.Log(ex);
        }

        return;
        string xmlContentb = await HttpHelper.RetrieveRdf(OntologyUri);

        try
        {
            using (StringReader stringReader = new StringReader(xmlContentb))

            using (XmlReader xmlReader = XmlReader.Create(stringReader))
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType != XmlNodeType.Element)
                        continue;

                    var name = xmlReader.Name.ToLower();


                    var property = xmlReader.Name;

                    if(!xmlReader.IsEmptyElement)
                    {
                        var value = xmlReader.ReadElementContentAsString();
                    }

                }
            }
        }
        catch (Exception e)
        {
            
        }

    }

    [ContextMenu("RetrieveOntology")]
    public async void RetrieveOntology()
    {
        var ontologyUri = "http://www.cidoc-crm.org/cidoc-crm/";

        string content = await HttpHelper.RetrieveRdf(OntologyUri);

        await FileHelper.SaveAsync(content, Application.dataPath, "VRKGUnity", "Data", "Tests", OntologyName + ".rdf");

        Debug.Log("Done");
    }

    

    // TODO : Get only ontologies
    // Extract tree ontology
    // Can change from none to ontology, but can't change deepontology (would need to re-request the db)
}
