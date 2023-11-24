using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_WeaponDetection : MonoBehaviour
{
    CameraShake CameraShake;
    public Player_Controller Player;
    public GameObject VFX_HitEnemy;
    [SerializeField] GameObject VFX_Blood;
    void Start()
    {
        Player = GameObject.Find("MainCharacter").GetComponent<Player_Controller>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Hit Enemy
        if (collision.tag == "Enemy_Hitbox")
        {
            Player.OnHitEnemy();
            
            Instantiate(VFX_HitEnemy, collision.ClosestPoint(transform.position), Quaternion.identity);
        }
        //Parry
        if (collision.tag == "Enemy_Attack_hitbox")
        {
            Player.OnParry();
            Instantiate(VFX_HitEnemy, collision.ClosestPoint(transform.position), Quaternion.identity);
            
        }

        if(collision.CompareTag("Static_Attack_hitbox"))
        {
            Instantiate(VFX_HitEnemy, collision.ClosestPoint(transform.position), Quaternion.identity);
        }
    }
}
