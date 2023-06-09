using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class Ontology
{
    public string QueryPrefixs { get { return _queryPrefixs; } }
    public string QueryFilter { get { return _queryFilter; } }


    List<Prefix> _prefixs;

    string _queryPrefixs;
    string _queryFilter;

    public Ontology(GraphDbRepositoryNamespaces repoNamespaces)
    {
        _prefixs = new List<Prefix>();

        AddBaseOntology(repoNamespaces);
        BuildQueries();
    }

    private void AddBaseOntology(GraphDbRepositoryNamespaces repoNamespaces)
    {

        _prefixs = new List<Prefix>();
        HashSet<string> prefixNames = new();


        var ontoTreeDict = repoNamespaces.OntoTreeDict;

        foreach(var namespaceAndOntoTree in ontoTreeDict)
        {
            string namespce = namespaceAndOntoTree.Key;

            var alias = CreatePrefixName(namespce);

            int i = 1;

            var originalAlias = alias;

            while (prefixNames.Contains(alias))
            {
                alias = originalAlias + i.ToString();
                i++;
            }

            _prefixs.Add(new Prefix(alias, namespce));
        }
    }

    public void RecreateBaseOntology(GraphDbRepositoryNamespaces repoNamespaces)
    {
        AddBaseOntology(repoNamespaces);
    }

    private string CreatePrefixName(string nameSpce)
    {
        nameSpce = nameSpce.ToLower();
        nameSpce = nameSpce.Replace("http://","");
        nameSpce = nameSpce.Replace("https://","");
        nameSpce = nameSpce.Replace("www.","");

        if(nameSpce.EndsWith("#") || nameSpce.EndsWith("/"))
            nameSpce = nameSpce.Substring(0, nameSpce.Length - 1);

        string patternStart = "^[a-zA-Z0-9]+"; // Get every letter andnumber before a special char
        var start = Regex.Match(nameSpce, patternStart).Value;

        string patternEnd = "[a-zA-Z0-9]+$";
        var end = Regex.Match(nameSpce, patternEnd).Value;

        return start + "_" + end;
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
            "        VALUES ?nc { PRFX_NAME }" +
            "        FILTER (" +
            "            (strStarts(str(?s), str(?na)) && strStarts(str(?p), str(?nb)) && strStarts(str(?o), str(?nc))) " +
            "               )" +
            "     }");

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

    public bool IsOntology(string uri)
    {
        (string namespce, string localName) = uri.ExtractUri();


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
}


public static class OntologyHelperExtension
{
    public static (string namespce, string localName) ExtractUri(this string uri)
    {

        if (uri == null)
        {
            Debug.Log("fse");
            return ("", "");
        }

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

    public static string CleanUriFromUrlPart(this string uri)
    {
        return uri.Replace("http://", "").Replace("/", "").Replace(".", "").Replace("\\", "").Replace("#", "");
    }
}
