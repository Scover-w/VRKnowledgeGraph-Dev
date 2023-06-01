using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SPARQLAdditiveBuilder
{
    
    string _startBaseQuery;
    string _endBaseQuery;

    string _startAdditiveQuery;
    string _endAdditiveQuery;

    List<string> _additiveQueries;

    Ontology _ontology;

    public SPARQLAdditiveBuilder()
    {
        _startBaseQuery = "SELECT ?s ?p ?o WHERE {";
        _endBaseQuery = "}";

        _startAdditiveQuery = "{ SELECT ?s ?p ?o WHERE { ?s ?p ?o . ";
        _endAdditiveQuery = "} }";

        _additiveQueries = new List<string>();

        _ontology = new(null);
    }


    public void Add(SPARQLQuery sparqlQuery)
    {
        _additiveQueries.Add(sparqlQuery.Query);
    }


    public string Build()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(_ontology.QueryPrefixs);

        sb.Append(_startBaseQuery);

        int nbAdditiveQuery = _additiveQueries.Count;

        for (int i = 0; i < nbAdditiveQuery; i++)
        {
            sb.Append(_startAdditiveQuery);
            sb.Append(_additiveQueries[i]);
            sb.Append(_endAdditiveQuery);
        }

        sb.Append(_startAdditiveQuery);
        sb.Append(_ontology.QueryFilter);
        sb.Append(_endAdditiveQuery);

        sb.Append(_endBaseQuery);

        return sb.ToString();
    }
}
