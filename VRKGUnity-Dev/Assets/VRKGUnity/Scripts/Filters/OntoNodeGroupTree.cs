using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OntoNodeGroupTree
{
    static GraphConfiguration _graphConfiguration;

    private readonly IReadOnlyDictionary<string, OntologyTree> _ontoTreeDict;

    Dictionary<string, OntoNodeGroup> _ontoGroups;

    OntoNodeGroup _ontoGroupRoot;

    private OntoNodeGroupTree(IReadOnlyDictionary<string, OntologyTree> ontoTreeDict, Dictionary<string, OntoNodeGroup> ontoGroups, OntoNodeGroup ontoGroupRoot)
    {
        _ontoTreeDict = ontoTreeDict;
        _ontoGroups = ontoGroups;
        _ontoGroupRoot = ontoGroupRoot;
    }


    public static OntoNodeGroupTree CreateOntoNodeTree(IReadOnlyDictionary<string, OntologyTree> ontoTreeDict, GraphConfiguration graphConfig, bool wantSpreadOut = true)
    {
        _graphConfiguration = graphConfig;
        Dictionary<string, OntoNodeGroup> ontoGroups = CreateOntoGroupsFromOntoNodes(ontoTreeDict);
        OntoNodeGroup groupTreeRoot = CreateRootOfOntoNodeTree(ontoTreeDict);

        if(!PruneTreeToColorLimit(groupTreeRoot, ontoGroups, wantSpreadOut))
        {
            // TODO : Handle this case
            Debug.Log("Can't reduce to nb color because too many ontology");
        }

        ComputeColorValueToGroups(groupTreeRoot, ontoGroups);

        return new OntoNodeGroupTree(ontoTreeDict, ontoGroups, groupTreeRoot);
    }

    private static Dictionary<string, OntoNodeGroup> CreateOntoGroupsFromOntoNodes(IReadOnlyDictionary<string, OntologyTree> ontoTreeDict)
    {
        Dictionary<string, OntoNodeGroup> ontoGroups = new();

        foreach (OntologyTree ontoTree in ontoTreeDict.Values)
        {
            var ontoNodes = ontoTree.OntoNodes;

            var groupOntoRoot = new OntoNodeGroup(ontoTree.RootOntoNode);

            foreach (var ontoNode in ontoNodes.Values)
            {
                if (ontoNode.CreateGroupIfOwnAttachedNode(out OntoNodeGroup ontoNodeGroup))
                    ontoGroups.Add(ontoNodeGroup.UID, ontoNodeGroup);
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

    private static bool PruneTreeToColorLimit(OntoNodeGroup groupTreeRoot, Dictionary<string, OntoNodeGroup> ontoGroups, bool wantSpreadOut)
    {
        // Filter to reduce to nbColor
        int nbColors = _graphConfiguration.NbOntologyColor;
        int maxDeltaHeight = _graphConfiguration.MaxDeltaOntologyAlgo;


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

            if (ontoGroups.TryGetValue(upperOntoNode.UID, out OntoNodeGroup upperOntoGroup)) // If upper group already exist, merge to it
            {
                ontoGroupToDelete.SendNodesTo(upperOntoGroup);
                ontoGroups.Remove(ontoGroupToDelete.UID);

                ontoGroupToDelete.RemoveFromParent();
                ontoGroupToDelete.RemoveFromOntoNode();
                return;
            }

            // If upper group don't exist, create one and merge to it
            var newOntoGroup = new OntoNodeGroup(upperOntoNode);
            ontoGroupToDelete.SendNodesTo(newOntoGroup);

            ontoGroups.Remove(ontoGroupToDelete.UID);
            ontoGroups.Add(newOntoGroup.UID, newOntoGroup);

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

            foreach (OntoNodeGroup ontoGroup in ontoGroups.Values) // Retrieve all leaf groups
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

                if (delta > maxDeltaHeight) // If group too far from the deepest leafNode, remove it, allows to 
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

    private static void ComputeColorValueToGroups(OntoNodeGroup groupTreeRoot, Dictionary<string, OntoNodeGroup> ontoGroups)
    {
        DebugDev.Log("ontoGroups.Count : " + ontoGroups.Count + "  , _graphConfiguration.NbOntologyColor : " + _graphConfiguration.NbOntologyColor);

        float nbColor = (ontoGroups.Count > _graphConfiguration.NbOntologyColor) ? ontoGroups.Count : _graphConfiguration.NbOntologyColor;


        float delta = 1f / nbColor;
        DebugDev.Log("NbColor : " +  nbColor + "  , delta : " + delta);

        Dictionary<string, float> uriValues = new();

        groupTreeRoot.ComputeColorValueAndSetToNode(0f, delta, uriValues);
    }

    public static OntoNodeGroupTree CreateOntoNodeTreeB(IReadOnlyDictionary<string, OntologyTree> ontoTreeDict, bool wantSpreadOut = true)
    {
        HashSet<OntoNodeGroup> ontoGroupToForget = new();
        Dictionary<string, OntoNodeGroup> ontoGroups = new();

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


                if (!ontoGroups.TryGetValue(ontoNode.UID, out OntoNodeGroup ontoGroup))
                {
                    ontoGroup = new OntoNodeGroup(ontoNode);
                    ontoGroups.Add(ontoGroup.UID, ontoGroup);
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

            if (ontoGroups.TryGetValue(upperOntoNode.UID, out OntoNodeGroup upperOntoGroup))
            {
                ontoGroup.SendNodesTo(upperOntoGroup);
                ontoGroups.Remove(ontoGroup.UID);
                continue;
            }

            var newOntoGroup = new OntoNodeGroup(upperOntoNode);
            ontoGroup.SendNodesTo(newOntoGroup);
            ontoGroups.Remove(ontoGroup.UID);
            ontoGroups.Add(newOntoGroup.UID, newOntoGroup);
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

        return new OntoNodeGroupTree(ontoTreeDict, null,null);
    }

}
