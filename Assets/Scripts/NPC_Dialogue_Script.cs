using System;
using UnityEngine;

public class NPC_Dialogue_Script : MonoBehaviour
{
    [SerializeField]
    private Dialogue dialogueBox;

    // void Awake()
    // {
    //     dialogueBox = FindFirstObjectByType<Dialogue>();
    // }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update() { }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering is the player
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered dialogue area");
            dialogueBox.ShowDialogue();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Hide dialogue when the player leaves the area
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited dialogue area");
            dialogueBox.HideDialogue();
        }
    }
}
