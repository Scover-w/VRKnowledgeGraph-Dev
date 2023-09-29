using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

/// <summary>
/// Payload used to share datas between the differents <see cref="AIDENController"/> functions.
/// </summary>
public class AIDENChainPayload
{
    public int Id { get; private set; }

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

    private int _currentFlowId = 0;

    readonly public Mutex MutexCurrentFlowId = new();
    readonly public Mutex MutexAIDENProperties = new();
    readonly public Mutex MutexManualProperties = new();
    readonly public Mutex MutexBothProperties = new();

    public AIDENChainPayload(int id)
    {
        Id = id;
    }


    /// <summary>
    /// Need to stop if a previous flow step was wrong with the manual value
    /// </summary>
    public bool NeedStopThisFlow(int flowId)
    {
        bool needStopThisFlow = false;

        MutexCurrentFlowId.WaitOne();

        if (flowId < _currentFlowId)
        {
            DebugDev.Log("Stop this flow : " + flowId);
            needStopThisFlow = true;
        }

        MutexCurrentFlowId.ReleaseMutex();

        return needStopThisFlow;
    }

    public bool AreSameType(List<AIDENPrompts> aidenPromptTypes)
    {
        int id = 0;

        MutexManualProperties.WaitOne();

        if (ManualTypePayload == null || ManualTypePayload.Count != aidenPromptTypes.Count)
        {
            MutexManualProperties.ReleaseMutex();
            DebugDev.Log("AreSameType false : " + (ManualTypePayload != null? ManualTypePayload.Count: "-1") + "  ,  " + aidenPromptTypes.Count);
            return false;
        }

        var typeAndSentences = ManualTypePayload.TypeAndSentence;

        foreach (AIDENPrompts type in  aidenPromptTypes) 
        {
            TypeAndSentence typeAndSentence = typeAndSentences[id];
            DebugDev.Log("AreSameType : " + typeAndSentence.Type + "  ,  " + type);


            if (typeAndSentence.Type == type)
            {
                id++;
                continue;
            }

            if(typeAndSentence.Type == AIDENPrompts.Metrique && type == AIDENPrompts.Mode)
            {
                id++;
                continue;
            }

            MutexManualProperties.ReleaseMutex();
            return false;

        }

        MutexManualProperties.ReleaseMutex();
        return true;
    }

    public bool AreSameIntents(AIDENIntents aidenIntentsWrapper)
    {
        MutexManualProperties.WaitOne();

        var aidenIntents = aidenIntentsWrapper.Intents;
        var manualIntents = ManualIntents.Intents;

        if (manualIntents == null || manualIntents.Count != aidenIntents.Count)
        {
            MutexManualProperties.ReleaseMutex();
            return false;
        }


        int i = 0;

        foreach(AIDENIntent aidenIntent in aidenIntents)
        {
            var manualIntent = manualIntents[i];

            if (!aidenIntent.AreSame(manualIntent))
            {
                MutexManualProperties.ReleaseMutex();
                return false;
            }

            i++;
        }

        MutexManualProperties.ReleaseMutex();
        return true;
    }

    public void ClearAIDENStateAnswerAfter(FlowStep flowStep)
    {
        MutexAIDENProperties.WaitOne();

        var keysToRemove = AIDENStateAnswer.Keys.Where(k => k > flowStep).ToList();

        foreach (var key in keysToRemove)
        {
            AIDENStateAnswer.Remove(key);
        }

        MutexAIDENProperties.ReleaseMutex();
    }

    public void SetCurrentFlowId(int flowId)
    {
        MutexCurrentFlowId.WaitOne();
        _currentFlowId = flowId;
        MutexCurrentFlowId.ReleaseMutex();
    }

    /// <summary>
    /// Allow to know what logic to call.
    /// If FalseUnder, need to do nothing because the function will handle it itself
    /// If FalseAbove, need to call the function to restart a new flow branch 
    /// If AllTrue, need to validate the intents
    /// </summary>
    public FlowStepRelativeStatus GetRelativeFlowStatus(FlowStep toCompare, out FlowStep blockedFlowStep)
    {
        DebugDev.LogThread("GetRelativeFlowStatus TOCOMPARE : " + toCompare);
        var steps = Enum.GetValues(typeof(FlowStep));

        blockedFlowStep = FlowStep.None;

        MutexAIDENProperties.WaitOne();

        foreach (FlowStep flowStep in steps)
        {
            if (flowStep == FlowStep.None)
                continue;

            if (flowStep == toCompare)
                continue;

            DebugDev.LogThread("GetRelativeFlowStatus : " + flowStep);

            if (AIDENStateAnswer.ContainsKey(flowStep) && AIDENStateAnswer[flowStep])
                continue;

            blockedFlowStep = flowStep;

            DebugDev.LogThread("GetRelativeFlowStatus : flowStep < toCompare :  " + toCompare + "  ,flowStep " + flowStep);

            if (flowStep < toCompare)
            {
                var returnedFlowStatus = AIDENStateAnswer.ContainsKey(flowStep) ? FlowStepRelativeStatus.FalseBefore : FlowStepRelativeStatus.NullBefore;
                MutexAIDENProperties.ReleaseMutex();
                return returnedFlowStatus;
            }

            var returnedFlowStatusB = AIDENStateAnswer.ContainsKey(flowStep) ? FlowStepRelativeStatus.FalseAfter : FlowStepRelativeStatus.NullAfter;
            MutexAIDENProperties.ReleaseMutex();
            return returnedFlowStatusB;
        }


        MutexAIDENProperties.ReleaseMutex();

        return FlowStepRelativeStatus.AllTrue;

        
    }
}