using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GraphActionKey
{
    SelectionMode,

    UndoFilter,
    RedoFilter,

    FilterSelected,
    FilterUnselected,
    FilterPropagated,
    FilterUnpropagated,

    Simulate,

    ResetFilters,
    RecalculateMetrics
}
