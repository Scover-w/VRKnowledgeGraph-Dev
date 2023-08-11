using UnityEngine;


[CreateAssetMenu(fileName = "ReferenceHolder", menuName = "ScriptableObjects/ReferenceHolderSO")]
public class ReferenceHolderSO : ScriptableObject
{
    public SingleAssignment<AppManager> AppManagerSA { get; } = new SingleAssignment<AppManager>();
    public SingleAssignment<LifeCycleSceneManager> LifeCycleSceneManagerSA { get; } = new SingleAssignment<LifeCycleSceneManager>();

    public SingleAssignment<Camera> HMDCamSA { get; } = new SingleAssignment<Camera>();
    public GraphDbRepository SelectedGraphDbRepository { get; set; }

    public GraphConfigManager GraphConfigManager { get; set; }

    public User User { get; set; }

    public float MaxDistanceGraph { get; set; }

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