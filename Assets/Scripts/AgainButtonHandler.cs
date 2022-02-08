using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AgainButtonHandler : MonoBehaviour
{
    public void Click()
    {
        SceneManager.LoadScene("2PFight");
    }
}
