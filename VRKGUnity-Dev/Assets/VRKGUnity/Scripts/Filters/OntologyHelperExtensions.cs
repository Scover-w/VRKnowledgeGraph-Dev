using System.Collections.Generic;
using VDS.RDF;

public static class OntologyHelperExtensions
{
    public static void CleanFromLabelAndComment(this IGraph graph)
    {
        List<Triple> triplesToRemove = new();

        foreach (Triple triple in graph.Triples)
        {
            string subject = triple.Subject.ToString();
            string predicate = triple.Predicate.ToString();
            string obj = triple.Object.ToString();



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
        }

        graph.Retract(triplesToRemove);
    }

    public static (string namespce, string localName) ExtractUri(this string uri)
    {

        if (uri == null)
        {
            DebugDev.Log("fse");
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
