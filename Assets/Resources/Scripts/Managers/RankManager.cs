using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RankManager : MonoBehaviour
{
    public Button backButton = null;

    public void LoadMain()
    {
        SceneManager.LoadScene("Main");
    }
}
