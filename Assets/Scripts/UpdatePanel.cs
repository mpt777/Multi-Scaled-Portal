using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdatePanel : MonoBehaviour
{
    public static UpdatePanel instance;

    public GameObject inforBox;

    public GameObject trial_text;
    public GameObject technique_text;
    public GameObject task_text;
    public GameObject block_text;

    private void Awake()
    {
        instance = this;  
    }

    public void DoTrialTextUpdate(int numTrial, int maxTrial)
    {
        trial_text.GetComponent<TextMeshProUGUI>().SetText("{0} / {1}", numTrial+1, maxTrial);
    }

    public void DoTechniqueTextUpdate(string techniqueName)
    {
        technique_text.GetComponent<TextMeshProUGUI>().SetText(techniqueName);
    }

    public void DoTaskTextUpdate(string taskName)
    {
        task_text.GetComponent<TextMeshProUGUI>().SetText(taskName);
    }

    public void DoBlockTextUpdate(int numBlock, int maxNumBlock)
    {
        block_text.GetComponent<TextMeshProUGUI>().SetText("{0} / {1}",numBlock, maxNumBlock);
    }
}
