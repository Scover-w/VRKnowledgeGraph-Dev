using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OntologySwitchTypeUI : MonoBehaviour
{
    [SerializeField]
    GameObject _ontologyCanvasGo;

    [SerializeField]
    RectTransform _parentRect;

    [SerializeField]
    GameObject _namespceUIPf;


    [SerializeField]
    Image _noneOntoImg;

    [SerializeField]
    Image _ontoImg;

    [SerializeField]
    Image _deepOntoImg;

    Ontology _ontology;
    GraphDbRepositoryNamespaces _graphDbRepoOnto;

    //List<Button> _uriTypeBtns;

    public void Display(Ontology ontology, GraphDbRepositoryNamespaces onto)
    {
        _ontology = ontology;
        _graphDbRepoOnto = onto;

        //_uriTypeBtns = new();

        //_noneOntoImg.color = GetColorFromType(UserNamespceType.None);
        //_ontoImg.color = GetColorFromType(UserNamespceType.DomainOntology);
        //_deepOntoImg.color = GetColorFromType(UserNamespceType.DeepOntology);

        //var namespceAndTypes = _graphDbRepoOnto.UserNamepsceTypes;

        //foreach (var namespceAndType in namespceAndTypes)
        //{
        //    int id = _parentRect.childCount;
        //    var namespce = namespceAndType.Key;
        //    var type = namespceAndType.Value;

        //    var cn = Instantiate(_namespceUIPf, _parentRect);

        //    var btn = cn.GetComponent<Button>();
        //    btn.onClick.AddListener(() => UriAndTypeClick(id, namespce));
        //    _uriTypeBtns.Add(btn);

        //    ColorBlock colors = btn.colors;
        //    colors.normalColor = Color.red;
        //    colors.selectedColor = colors.normalColor;
        //    colors.highlightedColor = colors.normalColor.Lighten(.1f);
        //    btn.colors = colors;

        //    cn.transform.GetChild(0).GetComponent<TMP_Text>().text = namespce;

        //}

        _ontologyCanvasGo.SetActive(true);
    }


    //public void UriAndTypeClick(int id, string namespce)
    //{
    //    Debug.Log("UriAndTypeClick " + id);
    //    var btn = _uriTypeBtns[id];


    //    btn.OnDeselect(null);
    //    ColorBlock colors = btn.colors;
    //    colors.normalColor = Color.red;
    //    colors.selectedColor = colors.normalColor;
    //    colors.highlightedColor = colors.normalColor.Lighten(.1f);
    //    btn.colors = colors;

    //    _ = _graphDbRepoOnto.Save();
    //}

    public void ValidateClick()
    {
        _ontology.RecreateBaseOntology(_graphDbRepoOnto);
        _ontologyCanvasGo.SetActive(false);
    }
}
