using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;

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

        var json = await _api.SelectQuery("select * where { \r\n\t?s ?p ?o .\r\n}");

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


            int sId = sValue.GetHashCode();
            int oId = oValue.GetHashCode();


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

}
