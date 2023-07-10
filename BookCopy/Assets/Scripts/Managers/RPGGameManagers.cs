using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGGameManagers : MonoBehaviour
{
    public RPGCameraManager cameraManager;
    public SpawnPoint playerSpawnPoint;
    public static RPGGameManagers sharedInstance = null;

    private void Awake() 
    {
        if ((sharedInstance != null) && (sharedInstance != this)){
            Destroy(gameObject);
        }
        else{
            sharedInstance = this;
        }
    }

    private void Start() 
    {
        SetupScene();
    }
    private void Update() {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }

    public void SetupScene()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        if(playerSpawnPoint != null)
        {
            GameObject player = playerSpawnPoint.SpawnObject();
            cameraManager.virtualCamera.Follow = player.transform;
        }
    }
}
