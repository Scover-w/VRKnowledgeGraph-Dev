using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static System.Net.WebRequestMethods;

public class Ontology
{
    public string QueryPrefixs { get { return _queryPrefixs; } }
    public string QueryFilter { get { return _queryFilter; } }


    List<Prefix> _prefixs;

    string _queryPrefixs;
    string _queryFilter;

    public Ontology()
    {

        _prefixs = new List<Prefix>();

        AddBaseOntology();
        BuildQueries();
    }


    private void AddBaseOntology()
    {
        _prefixs.Add(new Prefix("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
        _prefixs.Add(new Prefix("rdfs", "http://www.w3.org/2000/01/rdf-schema#"));
        _prefixs.Add(new Prefix("owl", "http://www.w3.org/2002/07/owl#"));
        _prefixs.Add(new Prefix("skos", "http://www.w3.org/2004/02/skos/core#"));
        _prefixs.Add(new Prefix("xsd", "http://www.w3.org/2001/XMLSchema#"));
        _prefixs.Add(new Prefix("proton", "http://proton.semanticweb.org/protonsys#"));
        _prefixs.Add(new Prefix("crm", "http://www.cidoc-crm.org/cidoc-crm/"));
        _prefixs.Add(new Prefix("dcterms", "http://purl.org/dc/terms/"));
    }


    public bool AddPrefix(Prefix prefix)
    {
        if(IsPrefixNameAlreadyUsed(prefix))
        {
            // TODO : Notification
            return false;
        }

        _prefixs.Add(prefix);
        BuildQueries();

        return true;
    }


    private void BuildQueries()
    {
        BuildQueryPrefixs();
        BuildQueryFilter();
    }

    private void BuildQueryPrefixs()
    {
        int nbPrefixs = _prefixs.Count;

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < nbPrefixs; i++)
        {
            var prefix = _prefixs[i];
            sb.Append("PREFIX " + prefix.Name + ": <" + prefix.Namespce + ">\n");
        }

        _queryPrefixs = sb.ToString();
    }

    private void BuildQueryFilter()
    {
        StringBuilder sb = new();
        sb.Append("FILTER NOT EXISTS " +
            "    {" +
            "        VALUES ?na { PRFX_NAME }" +
            "        VALUES ?nb { PRFX_NAME }" +
            "        FILTER (" +
            "            (strStarts(str(?s), str(?na)) && strStarts(str(?p), str(?nb)))" +
            "             ||" +
            "            (strStarts(str(?p), str(?na)) && strStarts(str(?o), str(?nb)))" +
            "               )" +
            "       }");

        var prefixNames = GetPrefixNames();


        sb.Replace("PRFX_NAME", prefixNames);

        _queryFilter = sb.ToString();


        string GetPrefixNames()
        {
            int nbPrefix = _prefixs.Count;

            StringBuilder sb = new();

            for (int i = 0; i < nbPrefix; i++)
            {
                var prefix = _prefixs[i];
                sb.Append(prefix.Name + ": ");
            }

            return sb.ToString();
        }
    }

    

    public bool IsOntology(string values)
    {
        (string namespce, string localName) = ExtractParts(values);


        int nbPrefix = _prefixs.Count;


        for (int i = 0; i < nbPrefix; i++)
        {
            if (namespce != _prefixs[i].Namespce)
                continue;

            return true;
        }

        return false;
    }


    private bool IsPrefixNameAlreadyUsed(Prefix prefix)
    {

        int nbPrefixs = _prefixs.Count;


        for (int i = 0; i < nbPrefixs; i++)
        {
            var prefixB = _prefixs[i];
            if(prefixB.Name == prefix.Name)
                return true;
        }

        return false;
    }


    public (string namespce, string localName) ExtractParts(string uri)
    {
        string separator = "/";
        if (uri.Contains("#"))
        {
            separator = "#";
        }

        int lastSeparatorIndex = uri.LastIndexOf(separator);
        if (lastSeparatorIndex != -1)
        {
            string namespce = uri.Substring(0, lastSeparatorIndex + 1).Trim();
            string localName = uri.Substring(lastSeparatorIndex + 1).Trim();
            return (namespce, localName);
        }

        return (uri.Trim(), string.Empty);
    }
}
