using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum FlowStep
{
    None,
    SplitSentence,
    DetectType, 
    DetectParameters
}

public enum FlowStepRelativeStatus
{
    FalseBefore,
    NullBefore,
    FalseAfter,
    NullAfter,
    AllTrue
}

public static class FlowStepExtensions
{
    public static bool HasReached(this FlowStep step, FlowStep stepToCompare)
    {
        int stepId = (int)step;
        int stepToCompareId = (int)stepToCompare;

        return (stepId >= stepToCompareId);
    }
}

public static class FlowStepRelativeStatusExtensions
{
    public static bool IsBefore(this FlowStepRelativeStatus relativeStatus)
    {
        return (relativeStatus == FlowStepRelativeStatus.FalseBefore || relativeStatus == FlowStepRelativeStatus.NullBefore);
    }

    public static bool IsAfter(this FlowStepRelativeStatus relativeStatus)
    {
        return (relativeStatus == FlowStepRelativeStatus.FalseAfter || relativeStatus == FlowStepRelativeStatus.NullAfter);
    }
}
