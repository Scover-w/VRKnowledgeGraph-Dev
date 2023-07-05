using System.Collections.Generic;
using System.Text;

public class SPARQLAdditiveBuilder
{
    
    string _startBaseQuery;
    string _endBaseQuery;

    string _startAdditiveQuery;
    string _endAdditiveQuery;

    string _firstAdditiveQuery;

    List<string> _additiveQueries;

    Ontology _ontology;

    public SPARQLAdditiveBuilder(GraphDbRepositoryNamespaces repoNamespaces)
    {
        _startBaseQuery = "SELECT ?s ?p ?o WHERE {";
        _endBaseQuery = "}";

        _startAdditiveQuery = "{ SELECT ?s ?p ?o WHERE { ?s ?p ?o . ";
        _endAdditiveQuery = "} }";

        _firstAdditiveQuery = "{ SELECT ?s ?p ?o " +
                              "WHERE { " +
                              "?s ?p ?o. " +
                              "FILTER NOT EXISTS { {" +
                              "?s ?p ?o . " +
                              "FILTER (?p = <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> && (?o = <http://www.w3.org/2000/01/rdf-schema#Class> || ?o = <http://www.w3.org/2002/07/owl#Class>)) } " +
                              "UNION { " +
                              "?s ?p ?o." +
                              " FILTER(?p = <http://www.w3.org/2000/01/rdf-schema#subClassOf> || ?p = <http://www.w3.org/2000/01/rdf-schema#subPropertyOf> || ?p = <http://www.w3.org/2000/01/rdf-schema#domain> " +
                              "|| ?p = <http://www.w3.org/2000/01/rdf-schema#range> || ?p = <http://www.w3.org/2002/07/owl#inverseOf> || " +
                              "?o = <http://www.w3.org/1999/02/22-rdf-syntax-ns#Property> || ?p = <http://www.w3.org/2000/01/rdf-schema#isDefinedBy>) } " +
                              "UNION { " +
                              "BIND (\"http://www.w3.org/\" AS ?w3) " +
                              "BIND (\"https://www.w3.org/\" AS ?w3s) " +
                              "?s ?p ?o. " +
                              "FILTER ( ( " +
                              "(strStarts(str(?s), ?w3) || strStarts(str(?s), ?w3s)) && " +
                              "(strStarts(str(?o), ?w3) || strStarts(str(?o), ?w3s))) || " +
                              "((strStarts(str(?s), ?w3) || strStarts(str(?s), ?w3s)) && " +
                              "(strStarts(str(?p), ?w3) || strStarts(str(?p), ?w3s))) || " +
                              "((strStarts(str(?p), ?w3) || strStarts(str(?p), ?w3s)) && " +
                              "(strStarts(str(?o), ?w3) || strStarts(str(?o), ?w3s))) ) " +
                              "}}}} ";

        _additiveQueries = new List<string>();

        //_ontology = new(ontology);
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
        StringBuilder sb = new StringBuilder();

        //sb.Append(_ontology.QueryPrefixs);

        sb.Append(_startBaseQuery);

        sb.Append(_firstAdditiveQuery);

        int nbAdditiveQuery = _additiveQueries.Count;

        for (int i = 0; i < nbAdditiveQuery; i++)
        {
            sb.Append(_startAdditiveQuery);
            sb.Append(_additiveQueries[i]);
            sb.Append(_endAdditiveQuery);
        }

        //sb.Append(_startAdditiveQuery);
        //sb.Append(_ontology.QueryFilter);
        //sb.Append(_endAdditiveQuery);

        sb.Append(_endBaseQuery);

        return sb.ToString();
    }
}
