using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal.VR;

public class AIDENChainPayload
{
    public int Id;

    public int ManualNbSplitSentences = -1;
    public SplitSentencesPayload ManualSplitSentences;
    public DetectedTypePayload ManualTypePayload;
    public AIDENIntents ManualIntents;

    public int AIDENNbSplitSentences = -1;
    public SplitSentencesPayload AIDENSplitSentences;
    public DetectedTypePayload AIDENTypePayload;
    public AIDENIntents AIDENIntents;

    public Dictionary<FlowStep, bool> AIDENStateAnswer = new();

    public FlowStep CurentManualStep = FlowStep.None;
    public int CurrentFlowId = 0;
    

    public AIDENChainPayload(int id)
    {
        Id = id;
    }


    /// <summary>
    /// Need to stop if a previous flow step was wrong with the manual value
    /// </summary>
    public bool NeedStopThisFlow(int flowId)
    {
        if(flowId < CurrentFlowId)
        {
            DebugDev.Log("Stop this flow.");
            return true;
        }

        return false;
    }

    public bool AreSameMode(List<AIDENPrompts> aidenPromptTypes)
    {
        int id = 0;

        if (ManualTypePayload == null || ManualTypePayload.Count != aidenPromptTypes.Count)
            return false;

        var typeAndSentences = ManualTypePayload.TypeAndSentence;

        foreach (AIDENPrompts type in  aidenPromptTypes) 
        {
            TypeAndSentence typeAndSentence = typeAndSentences[id];

            if (typeAndSentence.Type != type)
                return false;

            id++;
        }

        return true;
    }

    public bool AreSameIntents(AIDENIntents aidenIntentsWrapper)
    {
        var aidenIntents = aidenIntentsWrapper.Intents;
        var manualIntents = ManualIntents.Intents;

        if (manualIntents == null || manualIntents.Count != aidenIntents.Count)
            return false;


        int i = 0;

        foreach(AIDENIntent aidenIntent in aidenIntents)
        {
            var manualIntent = manualIntents[i];

            if (!aidenIntent.AreSame(manualIntent))
                return false;

            i++;
        }

        return true;
    }

    public void ClearAIDENStateAnswerAfter(FlowStep flowStep)
    {
        var keysToRemove = AIDENStateAnswer.Keys.Where(k => k > flowStep).ToList();

        foreach (var key in keysToRemove)
        {
            AIDENStateAnswer.Remove(key);
        }
    }


    /// <summary>
    /// Allow to know what logic to call.
    /// If FalseUnder, need to do nothing because the function will handle it itself
    /// If FalseAbove, need to call the function to restart a new flow branch 
    /// If AllTrue, need to validate the intents
    /// </summary>
    public FlowStepRelativeStatus GetRelativeFlowStatus(FlowStep toCompare, out FlowStep blockedFlowStep)
    {
        var steps = Enum.GetValues(typeof(FlowStep));

        blockedFlowStep = FlowStep.None;

        foreach (FlowStep flowStep in steps)
        {
            if (flowStep == FlowStep.None)
                continue;

            if (flowStep == toCompare)
                continue;

            if (AIDENStateAnswer.ContainsKey(flowStep) && AIDENStateAnswer[flowStep])
                continue;

            blockedFlowStep = flowStep;

            if (flowStep < toCompare)
                return AIDENStateAnswer[flowStep] ? FlowStepRelativeStatus.FalseBefore : FlowStepRelativeStatus.FalseAfter;

            
            return AIDENStateAnswer[flowStep]? FlowStepRelativeStatus.FalseAfter : FlowStepRelativeStatus.NullAfter;
        }

        return FlowStepRelativeStatus.AllTrue;
    }
}