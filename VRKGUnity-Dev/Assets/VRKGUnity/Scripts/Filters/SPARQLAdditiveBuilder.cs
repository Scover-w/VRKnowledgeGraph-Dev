using System.Collections.Generic;
using System.Text;

public class SPARQLAdditiveBuilder
{
    
    string _startBaseQuery;
    string _endBaseQuery;

    string _startAdditiveQuery;
    string _endAdditiveQuery;

    string _firstQuery;

    List<string> _additiveQueries;

    public SPARQLAdditiveBuilder()
    {
        _startBaseQuery = "SELECT ?s ?p ?o WHERE {";
        _endBaseQuery = "}";

        _startAdditiveQuery = "{ SELECT ?s ?p ?o WHERE { ?s ?p ?o . ";
        _endAdditiveQuery = "} }";


        _firstQuery = "{?s ?p ?o ." +
                      "FILTER EXISTS " +
                      "{ " +
                      "     GRAPH <http://data> " +
                      "     { " +
                      "         ?s ?p1 ?o1 . " +
                      "     }" +
                      "}}";

        _additiveQueries = new List<string>();
    }


    public void Add(SPARQLQuery sparqlQuery)
    {
        _additiveQueries.Add(sparqlQuery.Query);
    }

    public void Add(List<SPARQLQuery> sparqlQueries)
    {

        foreach(SPARQLQuery sparqlQuery in sparqlQueries) 
        { 
            _additiveQueries.Add(sparqlQuery.Query);
        }
    }


    public string Build()
    {
        int nbAdditiveQuery = _additiveQueries.Count;

        StringBuilder sb = new StringBuilder();

        sb.Append(_startBaseQuery);

        sb.Append(_firstQuery);

        for (int i = 0; i < nbAdditiveQuery; i++)
        {
            sb.Append(_startAdditiveQuery);
            sb.Append(_additiveQueries[i]);
            sb.Append(_endAdditiveQuery);
        }

        sb.Append(_endBaseQuery);

        return sb.ToString();
    }
}
