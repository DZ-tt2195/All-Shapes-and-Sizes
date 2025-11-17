using UnityEngine;
using TMPro;

public class SetText : MonoBehaviour
{
    [SerializeField] string key;

    private void Start()
    {
        GetComponent<TMP_Text>().text = Translator.inst.Translate(key);
    }
}
