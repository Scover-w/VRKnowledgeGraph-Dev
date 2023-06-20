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

        Dictionary<int, OntoNodeGroup> ontoGroups = CreateOntoGroupsFromOntoNodes(ontoTreeDict);
        OntoNodeGroup groupTreeRoot = CreateRootOfOntoNodeTree(ontoTreeDict);

        if(!PruneTreeToColorLimit(groupTreeRoot, ontoGroups, wantSpreadOut))
        {
            // TODO : Handle this case
            Debug.Log("Can't reduce to nb color because too many ontology");
        }

        return new OntoNodeTree(ontoTreeDict, ontoGroups, groupTreeRoot);
    }

    private static Dictionary<int, OntoNodeGroup> CreateOntoGroupsFromOntoNodes(IReadOnlyDictionary<string, OntologyTree> ontoTreeDict)
    {
        Dictionary<int, OntoNodeGroup> ontoGroups = new();

        foreach (OntologyTree ontoTree in ontoTreeDict.Values)
        {
            var ontoNodes = ontoTree.OntoNodes;

            var groupOntoRoot = new OntoNodeGroup(ontoTree.RootOntoNode);

            foreach (var ontoNode in ontoNodes.Values)
            {
                if (ontoNode.CreateGroupIfOwnAttachedNode(out OntoNodeGroup ontoNodeGroup))
                    ontoGroups.Add(ontoNodeGroup.Id, ontoNodeGroup);
            }
        }

        return ontoGroups;
    }

    private static OntoNodeGroup CreateRootOfOntoNodeTree(IReadOnlyDictionary<string, OntologyTree> ontoTreeDict)
    {
        var groupTreeRoot = new OntoNodeGroup(new OntoNode("rootOntoNodeTree", true));


        foreach (OntologyTree ontoTree in ontoTreeDict.Values)
        {
            var ontoNodeRoot = ontoTree.RootOntoNode;
            var ontoNodeGroupRoot = new OntoNodeGroup(ontoNodeRoot);

            groupTreeRoot.LinkTo(ontoNodeGroupRoot);

            ontoNodeRoot.CreateOntoGroupTree(null);
        }

        groupTreeRoot.ComputeHeight();
        groupTreeRoot.ComputeDepthDownward(0);

        return groupTreeRoot;
    }

    private static bool PruneTreeToColorLimit(OntoNodeGroup groupTreeRoot, Dictionary<int, OntoNodeGroup> ontoGroups, bool wantSpreadOut)
    {
        // Filter to reduce to nbColor
        int nbColors = 4;
        int maxDeltaHeight = 1;


        HashSet<OntoNodeGroup> ontoGroupToForget = new();

        while (ontoGroups.Count > nbColors && ontoGroupToForget.Count != ontoGroups.Count)
        {
            TryPruneOntoGroup();
        }

        return (ontoGroupToForget.Count != ontoGroups.Count);



        void TryPruneOntoGroup()
        {
            var ontoGroupToDelete = GetDeeperOntoGroup();

            var upperOntoNode = ontoGroupToDelete.GetUpperOntoNode(wantSpreadOut);

            if (upperOntoNode == null)
            {
                if (ontoGroupToForget.Contains(ontoGroupToDelete))
                    Debug.LogError("OntoNodeTree : ontoGroupToDelete already in ontoGroupToForget");

                ontoGroupToForget.Add(ontoGroupToDelete);
                return;
            }

            if (ontoGroups.TryGetValue(upperOntoNode.Id, out OntoNodeGroup upperOntoGroup))
            {
                ontoGroupToDelete.SendNodesTo(upperOntoGroup);
                ontoGroups.Remove(ontoGroupToDelete.Id);

                ontoGroupToDelete.RemoveFromParent();
                ontoGroupToDelete.RemoveFromOntoNode();
                return;
            }

            var newOntoGroup = new OntoNodeGroup(upperOntoNode);
            ontoGroupToDelete.SendNodesTo(newOntoGroup);

            ontoGroups.Remove(ontoGroupToDelete.Id);
            ontoGroups.Add(newOntoGroup.Id, newOntoGroup);

            ontoGroupToDelete.ReplaceFromParent(newOntoGroup);
            ontoGroupToDelete.RemoveFromOntoNode();
            newOntoGroup.ComputeDepthUpward();
        }

        OntoNodeGroup GetDeeperOntoGroup()
        {
            OntoNodeGroup selectedOntoGroup = groupTreeRoot;

            List<OntoNodeGroup> ontoGroupLeafs = new();

            int minMaxNbNode = wantSpreadOut ? int.MinValue : int.MaxValue;

            int maxDepth = 0;

            foreach (OntoNodeGroup ontoGroup in ontoGroups.Values)
            {
                if (ontoGroup.Height != 0)
                    continue;

                if (ontoGroupToForget.Contains(ontoGroup))
                    continue;

                ontoGroupLeafs.Add(ontoGroup);

                var depth = ontoGroup.Depth;
                if (depth > maxDepth)
                    maxDepth = depth;
            }


            int nbLeaf = ontoGroupLeafs.Count;

            for (int i = nbLeaf - 1; i > -1; i--)
            {
                var ontoGroup = ontoGroupLeafs[i];
                var depth = ontoGroup.Depth;
                var delta = Mathf.Abs(depth - maxDepth);

                if (delta > maxDeltaHeight)
                    continue;

                int nbNode = ontoGroup.NodeCount;

                if ((wantSpreadOut && nbNode > minMaxNbNode) || (!wantSpreadOut && nbNode < minMaxNbNode))
                {
                    selectedOntoGroup = ontoGroup;
                    minMaxNbNode = nbNode;
                }
            }

            return selectedOntoGroup;
        }
    }

    public static OntoNodeTree CreateOntoNodeTreeB(IReadOnlyDictionary<string, OntologyTree> ontoTreeDict, bool wantSpreadOut = true)
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

                if (ontoNode.NodesAttached.Count == 0)
                    continue;

                OntoNodeGroup ontoGroup;

                if (!ontoGroups.TryGetValue(ontoNode.Id, out ontoGroup))
                {
                    ontoGroup = new OntoNodeGroup(ontoNode);
                    ontoGroups.Add(ontoGroup.Id, ontoGroup);
                }

                foreach (var node in ontoNode.NodesAttached)
                {
                    ontoGroup.Nodes.Add(node);
                }
            }
        }


        int nbColors = 4;

        while (ontoGroups.Count > nbColors)
        {
            var ontoGroup = GetDeeperOntoGroup();

            var upperOntoNode = ontoGroup.GetUpperOntoNode(wantSpreadOut);

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
