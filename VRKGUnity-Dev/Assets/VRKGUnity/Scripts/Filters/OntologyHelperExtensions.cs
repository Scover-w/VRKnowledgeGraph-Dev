using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
}
