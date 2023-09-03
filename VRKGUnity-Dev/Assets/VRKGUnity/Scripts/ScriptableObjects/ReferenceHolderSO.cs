using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ReferenceHolder", menuName = "ScriptableObjects/ReferenceHolderSO")]
public class ReferenceHolderSO : ScriptableObject
{
    public SingleAssignment<AppManager> AppManagerSA { get; } = new SingleAssignment<AppManager>();
    public SingleAssignment<LifeCycleSceneManager> LifeCycleSceneManagerSA { get; } = new SingleAssignment<LifeCycleSceneManager>();

    public SingleAssignment<Camera> HMDCamSA { get; } = new SingleAssignment<Camera>();
    public SingleAssignment<Transform> WristTf { get; } = new SingleAssignment<Transform>();

    public SingleAssignment<LensRayActivator> LeftLensRayActivator { get; } = new SingleAssignment<LensRayActivator>();
    public SingleAssignment<LensRayActivator> RightLensRayActivator { get; } = new SingleAssignment<LensRayActivator>();

    public GraphDbRepository SelectedGraphDbRepository { get; set; }

    private InputPropagatorManager _inputPropagatorManager;
    public InputPropagatorManager InputPropagatorManager
    {
        get { return _inputPropagatorManager; }
        set
        {
            _inputPropagatorManager = value;
            OnNewInputPropagator?.Invoke(value);
        }
    }


    public NodgeSelectionManager NodgeSelectionManager { get ; set;}

    public float MaxDistanceGraph { get; set; }

    public delegate void NewInputPropagator(InputPropagatorManager inputPropagatorManager);

    public NewInputPropagator OnNewInputPropagator;

}

    



public class SingleAssignment<T>
{
    private T _value;
    private bool _isAssigned;


    public SingleAssignment()
    {
        _isAssigned = false;
    }


    public T Value
    {
        get 
        { 
            if(!_isAssigned)
                Debug.LogWarning($"{typeof(T).Name} hasn't been set yet.");
            return _value; 
        }
        set
        {
            if (!_isAssigned)
            {
                this._value = value;
                _isAssigned = true;
            }
            else
            {
                Debug.LogWarning($"{typeof(T).Name} can only be set once.");
            }
        }
    }
}