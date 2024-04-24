using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_LoadGutwhale : UI_BaseAction
{
    public override void Action(UI_Button button)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
