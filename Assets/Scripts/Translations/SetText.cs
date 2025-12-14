using UnityEngine;
using TMPro;

public class SetText : MonoBehaviour
{
    [SerializeField] ToTranslate key;

    private void Start()
    {
        GetComponent<TMP_Text>().text = AutoTranslate.DoEnum(key);
    }
}
