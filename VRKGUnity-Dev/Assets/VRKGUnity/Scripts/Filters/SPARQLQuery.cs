public class SPARQLQuery
{
    public SPARQLType Type { get { return _type; } }

    public string Query { get { return _query; } }

    SPARQLType _type;
    string _query;

    public SPARQLQuery(Node node)
    {
        _type = SPARQLType.HideNode;
        string uri = node.Value;
        _query = "FILTER(!(?s = <" + uri + "> || ?o = <" + uri + ">))";
    }

    public SPARQLQuery(Edge edge)
    {
        _type = SPARQLType.HideEdge;
        string uri = edge.Value;
        _query = "FILTER(?p != < " + uri + ">)";
    }
}


public enum SPARQLType
{
    HideNode,
    HideEdge
}
