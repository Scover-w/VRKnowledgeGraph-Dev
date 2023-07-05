using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SPARQLQuery
{
    public string Query { get { return _query; } }

    string _query;

    public SPARQLQuery(Node node, DynamicFilterType type)
    {
        string uri = node.Value;

        _query = "FILTER( + " + (type == DynamicFilterType.ExcludeAll? "!" : "") + "(?s = <" + uri + "> || ?o = <" + uri + ">) )";
    }

    public SPARQLQuery(HashSet<Node> nodes, DynamicFilterType type)
    {
        if (nodes == null || nodes.Count == 0)
            Debug.LogError("nodes is null or length equal 0.");

        StringBuilder sb = new StringBuilder();


        if(type == DynamicFilterType.ExcludeAll)
        {
            sb.Append("FILTER( !(");

            int nbNodes = nodes.Count;
            int i = 0;

            foreach (Node node in nodes)
            {
                i++;
                string uri = node.Value;
                sb.Append("(?s = <" + uri + "> || ?o = <" + uri + ">)");

                if(i != nbNodes)
                    sb.Append(" || ");

            }
        }
        else
        {
            sb.Append("FILTER( (");

            int nbNodes = nodes.Count;
            int i = 0;

            foreach (Node node in nodes)
            {
                i++;
                string uri = node.Value;
                sb.Append("(?s = <" + uri + "> || ?o = <" + uri + ">)");

                if (i != nbNodes)
                    sb.Append(" || ");

            }
        }

        sb.Append("))");
        _query = sb.ToString();
    }
}