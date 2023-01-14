using UnityEngine;

public class ToolSwitching : MonoBehaviour
{

    public int selectedTool = 0;

    // Start is called before the first frame update
    void Start ()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    public void ScrollUp()
    {
        if (selectedTool >= transform.childCount - 1)
            selectedTool = 0;
        else
            selectedTool++;
    }
    
}
