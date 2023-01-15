using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Advance : MonoBehaviour
{
    [SerializeField] 
    private TMP_Text _title;
    public int line = 0;

    public void Next()
    {
        if(line == 0)
            _title.text = "My tower has fallen down,";
        else if(line ==1)
            _title.text = "and right before the Annual Wizarding Convention, no less!";
        else if(line == 2)
            _title.text = "Will you help me rebuild it in time for the event?";
        else if(line ==3)
            SceneManager.LoadScene("SampleScene");
        else
            line = 0;
        
        line++;
    }
}