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
            _title.text = "I request your assistance. \nMy tower has fallen down,";
        else if(line ==1)
            _title.text = "and right before the Annual Wizarding Convention, \nno less!";
        else if(line == 2)
            _title.text = "Will you help me rebuild it in time for the event?";
        else if(line ==3)
            SceneManager.LoadScene("SampleScene");
        else
            line = 0;
        
        line++;
    }

    public GameObject pages;
    public int page = 0;

    private void PagePicker()
    {
        for (int i = 0; i < pages.transform.childCount; i++)
        {
            GameObject child = pages.transform.GetChild(i).gameObject;
            child.SetActive(false);

            if (page == i)
                child.SetActive(true);
        }

        if (page < 0)
            SceneManager.LoadScene("MainMenu");
        if (page > 1)
            SceneManager.LoadScene("MainMenu");
    }


    public void NextPage()
    {
        page++;
        PagePicker();
    }

    public void PreviousPage()
    {
        page--;
        PagePicker();
    }

}