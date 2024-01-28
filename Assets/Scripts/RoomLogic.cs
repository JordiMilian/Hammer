using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Generic_OnTriggerEnterEvents;

public class RoomLogic : MonoBehaviour
{
    [SerializeField] DoorController exitDoorController;
    [SerializeField] Generic_OnTriggerEnterEvents LoadTrigger;

    List<GameObject> EnemiesGO = new List<GameObject>();
    int EnemiesAlive;
    public bool AreCorrectlySpawned = true;

    [Serializable]
    public class RespawnPoint 
    {
        public GameObject CurrentlySpawnedEnemy;
        public GameObject EnemyPrefab;
        Vector3 SpawnVector;
        public void setSpawnVector()
        {
            if (CurrentlySpawnedEnemy == null) 
            { 
                SpawnVector = Vector2.zero; 
                Debug.Log("Missing original enemy");
            }
            else SpawnVector = CurrentlySpawnedEnemy.transform.position; 
        }
        public GameObject Spawn()
        {
            GameObject Spawned = Instantiate(EnemyPrefab, SpawnVector, Quaternion.identity);
            return Spawned;
        }
        public void DestroyCurrentEnemy()
        {
            if(CurrentlySpawnedEnemy != null) { Destroy(CurrentlySpawnedEnemy); }
        }
    }
    [SerializeField] List<RespawnPoint> respawnPoints;
    private void OnEnable()
    {
        LoadTrigger.ActivatorTags.Add("Player_SinglePointCollider");
        LoadTrigger.OnTriggerEntered += RespawnEnemies;
    }
    private void OnDisable()
    {
        LoadTrigger.OnTriggerEntered -= RespawnEnemies;
    }
    private void Start()
    {
        foreach (RespawnPoint point in respawnPoints)
        {
            point.setSpawnVector();
            AssignEnemyInfo(point, point.CurrentlySpawnedEnemy);
        }  
    }
    void EnemyDied(object sender, EventArgs args)
    {
        EnemiesAlive--;
        if (EnemiesAlive <= 0)
        {
            OpenDoor();
            AreCorrectlySpawned = false;
        }
    }
     void OpenDoor()
    {
        exitDoorController.OpenDoor();
    }
    public void RespawnEnemies(object sender, EventArgsTriggererInfo triggereInfo)
    {
        if (AreCorrectlySpawned) return;
        
        EnemiesGO.Clear();
        foreach (RespawnPoint point in respawnPoints)
        {
            if (point.EnemyPrefab == null) { Debug.Log("Missing Prefab"); continue; }
                
            point.DestroyCurrentEnemy();
            GameObject spawnedEnemy = point.Spawn();
            AssignEnemyInfo(point, spawnedEnemy);
        }
        EnemiesAlive = EnemiesGO.Count;
        StartCoroutine(RespawnCooldown());
        
    }
    void AssignEnemyInfo(RespawnPoint point, GameObject spawnedEnemy)
    {
        point.CurrentlySpawnedEnemy = spawnedEnemy;
        EnemiesGO.Add(spawnedEnemy);
        Generic_HealthSystem thisHealth = spawnedEnemy.GetComponent<Generic_HealthSystem>();
        thisHealth.OnDeath += EnemyDied;
    }
    IEnumerator RespawnCooldown()
    {
        AreCorrectlySpawned = true;
        yield return new WaitForSeconds(4);
        AreCorrectlySpawned = false;
    }
}
