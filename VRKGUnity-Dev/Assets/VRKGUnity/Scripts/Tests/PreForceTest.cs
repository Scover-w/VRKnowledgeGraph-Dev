using UnityEditor.SceneManagement;
using UnityEngine;

public class PreForceTest : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSO;


    private async void Start()
    {
        var graphDbRespositories = await GraphDbRepositories.Load();
        var repo = graphDbRespositories.AutoSelect();
        _referenceHolderSO.SelectedGraphDbRepository = repo;

        await repo.LoadChilds();

        EditorSceneManager.LoadSceneAsync("ForceTest", UnityEngine.SceneManagement.LoadSceneMode.Additive);

    }
}
