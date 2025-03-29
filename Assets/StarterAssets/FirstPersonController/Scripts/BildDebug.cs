using UnityEngine;
using UnityEngine.UI;
public class BildDebug : MonoBehaviour
{
    [SerializeField] private Text _text;
    public static BildDebug inistate;
    private void Start()
    {
        if (inistate == null)
        {
            inistate = this;
        }
        else
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
    }
    public void Log(string text)
    {
        _text.text += text + "\n";
    }
}