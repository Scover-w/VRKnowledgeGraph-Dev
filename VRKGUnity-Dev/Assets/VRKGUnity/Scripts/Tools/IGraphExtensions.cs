using System.IO;
using System.Text;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;
using VDS.RDF.Writing.Formatting;

public static class IGraphExtensions
{
    public static string ToTurtle(this IGraph graph)
    {
        var writer = new CompressingTurtleWriter();
        var serializedTurtle = new System.IO.StringWriter();

        writer.Save(graph, serializedTurtle);

        string turtleContent = serializedTurtle.ToString();

        return turtleContent;
    }


    /// <summary>
    /// Return true if load succeed
    /// </summary>
    /// <param name="graph"></param>
    /// <param name="rdfContent"></param>
    /// <returns></returns>
    public static bool TryLoadFromRdf(this IGraph graph, string rdfContent)
    {
        RdfXmlParser parser = new RdfXmlParser();

        try
        {
            using (StringReader reader = new StringReader(rdfContent))
            {
                parser.Load(graph, reader);
            }
        }
        catch (RdfParseException e)
        {
            return false;
        }

        return true;
    }


    public static string ToSparql(this IGraph graph) 
    {
        SparqlFormatter formatter = new SparqlFormatter();
        StringBuilder queryBuilder = new StringBuilder();

        queryBuilder.AppendLine("INSERT DATA {");

        foreach (Triple triple in graph.Triples)
        {
            string subject = formatter.Format(triple.Subject);
            string predicate = formatter.Format(triple.Predicate);
            string @object = formatter.Format(triple.Object);

            queryBuilder.AppendLine($"  {subject} {predicate} {@object} .");
        }

        queryBuilder.AppendLine("}");

        return queryBuilder.ToString();

    }

}
