using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnterRoomCutscene : MonoBehaviour
{
    [SerializeField] Generic_OnTriggerEnterEvents enterRoomTrigger;
    [SerializeField] EnemyGenerator enemyGenerator;
    [SerializeField] Transform CenterOfRoom;

    private void OnEnable()
    {
        enterRoomTrigger.AddActivatorTag(TagsCollection.Instance.Player_SinglePointCollider);
        enterRoomTrigger.OnTriggerEntered += callEntered;
    }
    private void OnDisable()
    {
        enterRoomTrigger.OnTriggerEntered -= callEntered;
    }
    void callEntered(object sender, Generic_OnTriggerEnterEvents.EventArgsCollisionInfo args)
    {
        StartCoroutine(EnterRoom());
    }

    IEnumerator EnterRoom()
    {
        //Find the references
        CinemachineTargetGroup targetGroup = GameObject.Find(TagsCollection.Instance.TargetGroup).GetComponent<CinemachineTargetGroup>();
        CameraZoomer zoomer = GameObject.Find(TagsCollection.Instance.CMvcam1).GetComponent<CameraZoomer>();

        //Find empty slot
        int emptySlot = EnemyGenerator.FindEmptyTargetgroupSlot(targetGroup);

        //Wait just in case for enemies to spawn
        yield return new WaitForSeconds(0.3f);

        zoomer.AddZoomInfo(new CameraZoomer.ZoomInfo(6.5f, 1, "enter"));
        EnemyGenerator.AddTargetToTargetGroup(targetGroup, emptySlot, CenterOfRoom, 20, 10);

        yield return new WaitForSeconds(0.9f);
        //Activate the Agroo of the enemies
        foreach (GameObject enemy in enemyGenerator.CurrentlySpawnedEnemies)
        {
            enemy.GetComponent<Enemy_EventSystem>().CallAgrooState?.Invoke();
        }
        yield return new WaitForSeconds(0.9f);

        zoomer.RemoveZoomInfo("enter");
        EnemyGenerator.AddTargetToTargetGroup(targetGroup, emptySlot, null, 0, 0);

        
    }
}
