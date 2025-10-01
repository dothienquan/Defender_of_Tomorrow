using UnityEngine;

public class Goal : MonoBehaviour
{
    [Header("Assign the panel GameObject in Inspector")]
    public GameObject goalPanel;   

    public void Activate()
    {
        Debug.Log("Goal Activated!");

        if (goalPanel != null)
        {
            goalPanel.SetActive(true); 
        }
        else
        {
            Debug.LogWarning("Goal panel not assigned!");
        }
    }
}
