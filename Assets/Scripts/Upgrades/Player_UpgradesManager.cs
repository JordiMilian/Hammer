using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_UpgradesManager : MonoBehaviour
{
    [SerializeField] bool trigger_DeleteRandomUpgrade;
    [SerializeField] GameState gameState;
    [SerializeField] Player_EventSystem playerEvents;
    [SerializeField] Generic_OnTriggerEnterEvents singlePointCollider;
    private void OnEnable()
    {
        singlePointCollider.AddActivatorTag(TagsCollection.UpgradeContainer);
        singlePointCollider.OnTriggerEntered += onSingleTriggerEnter;

        foreach (Upgrade upgrade in gameState.playerUpgrades)
        {
            upgrade.onAdded(gameObject);
        }
    }
    private void OnDisable()
    {
        singlePointCollider.OnTriggerEntered -= onSingleTriggerEnter;

        foreach (Upgrade upgrade in gameState.playerUpgrades)
        {
            upgrade.onRemoved(gameObject);
        }
    }
    private void onSingleTriggerEnter(Collider2D collision)
    {
        if(collision.CompareTag("UpgradeContainer"))
        {
            Debug.Log("upgrade:" +  collision.name);
            UpgradeContainer upgradeContainer = collision.GetComponent<UpgradeContainer>(); //CREA UN TAG O ALGUNA COSA PERFA
            AddNewUpgrade(upgradeContainer.upgradeEffect);
            playerEvents.OnPickedNewUpgrade?.Invoke(upgradeContainer);
        }
    }
    void AddNewUpgrade(Upgrade upgrade)
    {
        upgrade.onAdded(gameObject);
        gameState.playerUpgrades.Add(upgrade);
        
    }
    void deleteRandomUpgrade()
    {
        if(gameState.playerUpgrades.Count == 0) { Debug.Log("No upgrades to delete"); return;}
        int randomIndex = Random.Range(0, gameState.playerUpgrades.Count-1);
        deleteUpgrade(randomIndex);

    }
    void deleteUpgrade(int i)
    {
        gameState.playerUpgrades[i].onRemoved(gameObject);
        gameState.playerUpgrades.RemoveAt(i);
    }
    private void Update()
    {
       if(trigger_DeleteRandomUpgrade)
        {
            deleteRandomUpgrade();
            trigger_DeleteRandomUpgrade = false;
        }
    }
}
