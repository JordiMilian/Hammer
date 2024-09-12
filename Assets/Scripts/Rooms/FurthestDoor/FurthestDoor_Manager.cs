using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurthestDoor_Manager : MonoBehaviour
{
    public List<EnterExitScene_withDistance> enterExitScenesList;
    [SerializeField] GameState gameState;
    [SerializeField] RoomGenerator_Manager roomGenerator;

    public static FurthestDoor_Manager Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        gameState.LastEnteredDoor = roomGenerator.AreaIndex - 1;
    }
    private void Start()
    {
        SubscribeToDoors();
        sortDoorsByDistance();
        ActivateGameStatesDoor();
    }
    void ActivateGameStatesDoor()
    {
        if (gameState.FurthestDoorsArray[roomGenerator.AreaIndex] < 0) { return; }
        foreach (EnterExitScene_withDistance enterExitScene in enterExitScenesList)
        {
            enterExitScene.playEnteringCutsceneOnLoad = false;
        }
        enterExitScenesList[gameState.FurthestDoorsArray[roomGenerator.AreaIndex]].playEnteringCutsceneOnLoad = true;
        Player_RespawnerManager.Instance.Respawners[roomGenerator.AreaIndex].ExternallyActivateRespawner();


    }
    void sortDoorsByDistance()
    {
        SetDistancesToDoors();

        int doorsLenght = enterExitScenesList.Count;
        for (int i = 0; i < doorsLenght; i++)
        {
            int closestIndex = i;
            for (int j = i + 1; j < doorsLenght; j++)
            {
                if (enterExitScenesList[j].DistanceToManager < enterExitScenesList[closestIndex].DistanceToManager)
                {
                    closestIndex = j;
                }
            }

            EnterExitScene_withDistance tempEnterExit = enterExitScenesList[closestIndex];
            enterExitScenesList[closestIndex] = enterExitScenesList[i];
            enterExitScenesList[i] = tempEnterExit;
        }
    }
    void SetDistancesToDoors()
    {
        foreach (EnterExitScene_withDistance enterExit in enterExitScenesList)
        {
            enterExit.DistanceToManager = (enterExit.transform.position - transform.position).sqrMagnitude;
        }
    }

    void SubscribeToDoors()
    {
        foreach(EnterExitScene_withDistance enterExit in enterExitScenesList)
        {
            enterExit.onDoorActivated += UpdateFurthestDoorInState;
        }
    }
    void UpdateFurthestDoorInState()
    {
        gameState.FurthestDoorsArray[roomGenerator.AreaIndex] = GetFurthestActiveDoor();
    }
    int GetFurthestActiveDoor()
    {
        int furthestIndex = 0;
        for (int i = 0; i < enterExitScenesList.Count; i++)
        {
            if (enterExitScenesList[i].DistanceToManager > enterExitScenesList[furthestIndex].DistanceToManager && enterExitScenesList[i].isDoorActive)
            {
                furthestIndex = i;
            }
        }
        return furthestIndex;
    }
}
