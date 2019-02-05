using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RankManager : MonoBehaviour
{
    // BACK 버튼
    public Button BackButton = null;

    public void OnClickBack()
    {
        SceneManager.LoadScene("Main");
    }
}
