using Unity.Cinemachine;
using UnityEngine;

public class CinemachineCameraSgnalController : MonoBehaviour
{
    [SerializeField] CinemachineCamera playerCam;
    [SerializeField] CinemachineCamera[] otherCam;

    public void SetPlayerCam()
    {
        playerCam.Priority = 10;
    }

    public void SetOtherCam(int index)
    {
        if(otherCam.Length <= 0) return;

        if(index < 0 || index >= otherCam.Length)
        {
            Debug.LogError("index out of range");
            return;
        }
        
        otherCam[index].Priority = 10;
        playerCam.Priority = 5;
    }
}