using System.Collections;
using UnityEngine;

public class LockerInteract : Interact
{
    [SerializeField] GameObject boyPrefab;
    [SerializeField] Transform spawnPoint;

    [SerializeField] TalkData talkData;


    void Start()
    {
        interactData.OnInteractAction += () => OnNewEvent();
    }

    void OnNewEvent()
    {
        StartCoroutine(ShowTalkPanel());
        
    }

    IEnumerator ShowTalkPanel()
    {
        Instantiate(boyPrefab, spawnPoint.position, spawnPoint.rotation);
        yield return new WaitForSeconds(1f);
    }
}