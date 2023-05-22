using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class NodeDictHelperExtension
{
    public static Dictionary<int, Node> GetNoLabeledNodes(this Dictionary<int, Node> idAndNodes)
    {
        Dictionary<int, Node> nolabeled = new();

        foreach (var idAndNode in idAndNodes)
        {
            var node = idAndNode.Value;

            if (node.Type != "uri")
                continue;

            if (node.DoesPropertiesContainName())
                continue;

            nolabeled.Add(idAndNode.Key, idAndNode.Value);
        }

        return nolabeled;
    }
}
