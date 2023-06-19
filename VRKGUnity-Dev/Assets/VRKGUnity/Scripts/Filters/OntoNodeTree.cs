using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class OntoNodeTree
{
    private IReadOnlyDictionary<string, OntologyTree> _ontoTreeDict;

    Dictionary<int, OntoNodeGroup> _ontoGroups;

    OntoNodeGroup _ontoGroupRoot;

    private OntoNodeTree(IReadOnlyDictionary<string, OntologyTree> ontoTreeDict, Dictionary<int, OntoNodeGroup> ontoGroups, OntoNodeGroup ontoGroupRoot)
    {
        _ontoTreeDict = ontoTreeDict;
        _ontoGroups = ontoGroups;
        _ontoGroupRoot = ontoGroupRoot;
    }


    public static OntoNodeTree CreateOntoNodeTree(IReadOnlyDictionary<string, OntologyTree> ontoTreeDict, bool wantSpreadOut = true)
    {
        HashSet<OntoNodeGroup> ontoGroupToForget = new();
        Dictionary<int, OntoNodeGroup> ontoGroups = new();

        var groupTreeRoot = new OntoNodeGroup(new OntoNode("rootOntoNodeTree", true));


        // Create OntoNodeGroups
        foreach (OntologyTree ontoTree in ontoTreeDict.Values)
        {
            var ontoNodes = ontoTree.OntoNodes;

            var groupOntoRoot = new OntoNodeGroup(ontoTree.RootOntoNode);

            foreach (var ontoNode in ontoNodes.Values)
            {
                if (ontoNode.CreateGroupIfOwnDefinedNode(out OntoNodeGroup ontoNodeGroup))
                    ontoGroups.Add(ontoNodeGroup.Id, ontoNodeGroup);
            }
        }

        foreach (OntologyTree ontoTree in ontoTreeDict.Values)
        {
            var ontoNodeRoot = ontoTree.RootOntoNode;
            var ontoNodeGroupRoot = new OntoNodeGroup(ontoNodeRoot);

            ontoNodeRoot.OntoNodeGroup = ontoNodeGroupRoot;

            groupTreeRoot.LinkTo(ontoNodeGroupRoot);

            ontoNodeRoot.CreateOntoGroupTree(null);
        }

        groupTreeRoot.ComputeHeight();



        // Filter to reduce to nbColor
        int nbColors = 4;

        while (ontoGroups.Count > nbColors && ontoGroupToForget.Count != ontoGroups.Count)
        {
            var ontoGroupToDelete = GetDeeperOntoGroup();

            var upperOntoNode = ontoGroupToDelete.GetUpperOntoNode();

            if (upperOntoNode == null)
            {
                ontoGroupToForget.Add(ontoGroupToDelete);
                continue;
            }

            if (ontoGroups.TryGetValue(upperOntoNode.Id, out OntoNodeGroup upperOntoGroup))
            {
                ontoGroupToDelete.SendNodesTo(upperOntoGroup);
                ontoGroups.Remove(ontoGroupToDelete.Id);

                ontoGroupToDelete.RemoveFromParent();
                continue;
            }

            var newOntoGroup = new OntoNodeGroup(upperOntoNode);
            ontoGroupToDelete.SendNodesTo(newOntoGroup);
            ontoGroups.Remove(ontoGroupToDelete.Id);
            ontoGroups.Add(newOntoGroup.Id, newOntoGroup);

            ontoGroupToDelete.ReplaceFromParent(newOntoGroup);

        }


        if(ontoGroupToForget.Count != ontoGroups.Count)
        {
            Debug.Log("Can't reduce to nb color because too many ontology");
        }

        OntoNodeGroup GetDeeperOntoGroup()
        {
            OntoNodeGroup selectedOntoGroup = groupTreeRoot;

            int minMaxNbNode = wantSpreadOut? int.MinValue : int.MaxValue;


            foreach (OntoNodeGroup ontoGroup in ontoGroups.Values)
            {
                if (ontoGroup.Height != 0)
                    continue;

                if (ontoGroupToForget.Contains(ontoGroup))
                    continue;

                int nbNode = ontoGroup.NbNode;

                if( (wantSpreadOut && nbNode > minMaxNbNode) || (!wantSpreadOut && nbNode < minMaxNbNode) )
                {
                    selectedOntoGroup = ontoGroup;
                    minMaxNbNode = nbNode;
                }
            }

            return selectedOntoGroup;
        }


        return new OntoNodeTree(ontoTreeDict, ontoGroups, groupTreeRoot);
    }
    
    public static OntoNodeTree CreateOntoNodeTreeB(IReadOnlyDictionary<string, OntologyTree> ontoTreeDict)
    {
        HashSet<OntoNodeGroup> ontoGroupToForget = new();
        Dictionary<int, OntoNodeGroup> ontoGroups = new();

        var groupTreeRoot = new OntoNodeGroup(new OntoNode("rootOntoNodeTree",true));

        // Nobody can watch this without bleeding, trust me
        foreach (OntologyTree ontoTree in ontoTreeDict.Values)
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

        return new OntoNodeTree(ontoTreeDict, null,null);
    }

}
