using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class OntoNodeTree
{
    private IReadOnlyDictionary<string, OntologyTree> _ontoTreeDict;

    Dictionary<int, OntoNodeGroup> _ontoGroups;

    OntoNodeGroup _groupRoot;

    private OntoNodeTree(IReadOnlyDictionary<string, OntologyTree> ontoTreeDict)
    {
        _ontoTreeDict = ontoTreeDict;
    }


    public static OntoNodeTree CreateOntoNodeTree(IReadOnlyDictionary<string, OntologyTree> ontoTreeDict)
    {
        HashSet<OntoNodeGroup> ontoGroupToForget = new();
        Dictionary<int, OntoNodeGroup> ontoGroups = new();

        var groupTreeRoot = new OntoNodeGroup(new OntoNode("rootOntoNodeTree"));

        // Nobody can watch this without bleeding, trust me
        foreach (var ontoTree in ontoTreeDict.Values)
        {
            var ontoNodes = ontoTree.OntoNodes;

            var groupOntoRoot = new OntoNodeGroup(ontoTree.RootOntoNode);

            foreach (var ontoNode in ontoNodes.Values)
            {

                if (ontoNode.NodesDefined.Count == 0)
                    continue;

                OntoNodeGroup ontoGroup;

                if (!ontoGroups.TryGetValue(ontoNode.Id, out ontoGroup))
                {
                    ontoGroup = new OntoNodeGroup(ontoNode);
                    ontoGroups.Add(ontoGroup.Id, ontoGroup);
                }

                foreach (var node in ontoNode.NodesDefined)
                {
                    ontoGroup.Nodes.Add(node);
                }
            }
        }


        int nbColors = 4;

        while (ontoGroups.Count > nbColors)
        {
            var ontoGroup = GetDeeperOntoGroup();

            var upperOntoNode = ontoGroup.GetUpperOntoNode();

            if (upperOntoNode == null)
            {
                ontoGroupToForget.Add(ontoGroup);
                continue;
            }

            if (ontoGroups.TryGetValue(upperOntoNode.Id, out OntoNodeGroup upperOntoGroup))
            {
                ontoGroup.SendNodesTo(upperOntoGroup);
                ontoGroups.Remove(ontoGroup.Id);
                continue;
            }

            var newOntoGroup = new OntoNodeGroup(upperOntoNode);
            ontoGroup.SendNodesTo(newOntoGroup);
            ontoGroups.Remove(ontoGroup.Id);
            ontoGroups.Add(newOntoGroup.Id, newOntoGroup);
        }


        OntoNodeGroup GetDeeperOntoGroup()
        {
            int depth = 0;
            OntoNodeGroup deeperOntoGroup = ontoGroups.First().Value;

            foreach (var ontoGroup in ontoGroups.Values)
            {
                if (ontoGroup.Depth < depth)
                    continue;

                if (ontoGroupToForget.Contains(ontoGroup))
                    continue;

                deeperOntoGroup = ontoGroup;
                depth = ontoGroup.Depth;
            }

            return deeperOntoGroup;

        }

        return new OntoNodeTree(ontoTreeDict);
    }

}
