using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tomato_Spawner : MonoBehaviour
{
    Transform Player;
    [SerializeField] Transform TomatoHandTransform;
    [SerializeField] GameObject TomatoPrefab;
    [SerializeField] GameObject DestinationGO;
    [SerializeField] float MaxDistance;
    [SerializeField] Enemy_EventSystem enemyEvents;
    void Awake()
    {
        Player = GameObject.Find(TagsCollection.MainCharacter).transform;
    }
    public void SpawnTomato()
    {
        //Get direction to Player and move the hand towards it
        Vector2 PlayerPos = Player.position;
        Vector2 HandPosition = TomatoHandTransform.position;
        Vector3 DirectionToPlayer = PlayerPos - HandPosition;
        TomatoHandTransform.up = Vector3.RotateTowards(TomatoHandTransform.up, DirectionToPlayer, 100 * Time.deltaTime, 10);

        //Instantiante the tomato
        var Tomato = Instantiate(TomatoPrefab, TomatoHandTransform.position, TomatoHandTransform.rotation);

        //Call event for Sound effect currently
        enemyEvents.OnThrowTomato?.Invoke();
    }
    public void EV_newSpawnTomato()
    {
        
        Vector2 PlayerPos = Player.position;
        Vector2 destination = PlayerPos;
        Vector2 HandPosition = TomatoHandTransform.position;

        //If the player is too far edit destination
        if( (PlayerPos - HandPosition).magnitude > MaxDistance)
        {
            Vector2 playerDirection = ( PlayerPos - HandPosition).normalized;
            destination = HandPosition + (playerDirection * MaxDistance);
        }

        GameObject newTomato = Instantiate(TomatoPrefab,TomatoHandTransform.position,Quaternion.identity);
        GameObject newDestination = Instantiate(DestinationGO, destination, Quaternion.identity);

        newTomato.GetComponent<Tomato_NewController>().ThrowItself(newDestination, HandPosition, destination);

        //Call event for Sound effect currently
        enemyEvents.OnThrowTomato?.Invoke();
    }

}
