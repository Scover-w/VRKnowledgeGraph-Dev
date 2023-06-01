using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ReferenceHolder", menuName = "ScriptableObjects/ReferenceHolderSO")]
public class ReferenceHolderSO : ScriptableObject
{
    public GameManager GameManager { get; set; }
    public GraphDbRepository SelectedGraphDbRepository { get; set; }
}
