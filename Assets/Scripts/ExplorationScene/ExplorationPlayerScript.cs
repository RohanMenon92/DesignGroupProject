using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExplorationPlayerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            CheckInteract();
        }
    }

    void CheckInteract()
    {
        // Check Interaction if can interact
        SceneManager.LoadScene(1);
    }
}
