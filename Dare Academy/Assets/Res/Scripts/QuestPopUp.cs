using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPopUp : MonoBehaviour
{
    [SerializeField] private Canvas _prefab;
    private bool _destroySelf = false;
    [SerializeField] private TMPro.TMP_Text _body;
    [SerializeField] private TMPro.TMP_Text _title;

    private void OnEnable()
    {
        if (_destroySelf)
        {
            Destroy(_prefab.gameObject);
        }
        else
        {
            _destroySelf = true;
        }
    }

    public void SetBody(string in_body)
    {
        _body.text = in_body;
    }

    public void SetTitle(string in_title)
    {
        _title.text = in_title;
    }
}