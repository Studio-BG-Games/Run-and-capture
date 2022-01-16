using HexFiled;
using TMPro;
using UnityEngine;

#if UNITY_EDITOR


public class Score : MonoBehaviour
{
    private TMP_Text _text;
    void Start()
    {
        _text = GetComponent<TMP_Text>();
    }

    
    void Update()
    {
        _text.text = "";
        var sum = 0;
        foreach (var color in HexManager.CellByColor)
        {
            _text.text = _text.text + color.Key + " " + color.Value.Count + '\n';
            sum += color.Value.Count;
        }

        _text.text += sum;
    }
}
#endif