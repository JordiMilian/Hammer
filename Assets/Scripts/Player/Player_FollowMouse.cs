using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
//using System.Diagnostics;
using UnityEngine;

public class Player_FollowMouse : MonoBehaviour
{

    public float FollowMouse_Speed = 0.08f;
    Player_Controller Player;
    GameObject EnemyFocus;
    Transform CameraFocusTransform;
    public GameObject FocusIcon;
    bool IsFocusingEnemy;
    [SerializeField] float FocusMaxDistance;

    GameObject PlayerGO;
    [SerializeField] GameObject BodySprite;
    bool FlipOnce;



    List<GameObject> CurrentEnemies = new List<GameObject>();


    CinemachineTargetGroup cinemachineTarget;
    void Start()
    {

        cinemachineTarget = GameObject.Find("TargetGroup").GetComponent<CinemachineTargetGroup>();
        Player = GetComponentInParent<Player_Controller>();
        PlayerGO = Player.gameObject;
        CameraFocusTransform = GameObject.Find("CameraController").transform;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            CurrentEnemies.Clear();
            CurrentEnemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));

            EnemyFocus = ClosestEnemyToMouseInRange(FocusMaxDistance);

            if (EnemyFocus == null)
            {
                OnLookAtMouse();
            }
            else
            {
                OnLookAtEnemy();
            }
        }
        if (EnemyFocus == null)
        {
            OnLookAtMouse();
        }

        if (IsFocusingEnemy == true) { LookingAtEnemy(); }
        else { LookingAtMouse(); }

        
    }
    void flipSprite(GameObject objecto)
    {
        objecto.transform.localScale = new Vector2(objecto.transform.localScale.x * -1, objecto.transform.localScale.y);
        FlipOnce = !FlipOnce;
    }
    GameObject ClosestEnemyToMouseInRange(float range)
    {
        Vector2 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int u = 0;

        List<GameObject> InrangeEnemies = new List<GameObject>();
        List<float> InrangeDistances = new List<float>();

        int minIndex = 0;

        for (int i = 0; i < CurrentEnemies.Count; i++)
        {
            if (Vector2.Distance(mousepos, CurrentEnemies[i].transform.position) < range)
            {
                InrangeEnemies.Add(CurrentEnemies[i]);
                InrangeDistances.Add(Vector2.Distance(mousepos, CurrentEnemies[i].transform.position));
                u++;
            }
        }
        if (InrangeEnemies.Count == 0)
        {
            return null;
        }
        for (int o = 0; o < InrangeDistances.Count; o++)
        {
            if (InrangeDistances[o] < InrangeDistances[minIndex])
            {
                minIndex = o;
            }
        }
        return InrangeEnemies[minIndex];

    }
    void OnLookAtMouse()
    {
        FocusIcon.GetComponent<SpriteRenderer>().enabled = false;
        IsFocusingEnemy = false;
        cinemachineTarget.m_Targets[1].target = CameraFocusTransform;
        cinemachineTarget.m_Targets[1].weight = 1;
    }
    void OnLookAtEnemy()
    {
        FocusIcon.GetComponent<SpriteRenderer>().enabled = true;

        IsFocusingEnemy = true;
        cinemachineTarget.m_Targets[1].target = EnemyFocus.transform;
        cinemachineTarget.m_Targets[1].weight = 2;
    }
    void LookingAtMouse()
    {
        Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.up = (Vector3.RotateTowards(transform.up, mousePos - new Vector2(transform.position.x, transform.position.y), FollowMouse_Speed, 10f));

        if (mousePos.x > PlayerGO.transform.position.x)
        {
            if (FlipOnce == false) flipSprite(BodySprite);

        }
        if (mousePos.x < PlayerGO.transform.position.x)
        {
            if (FlipOnce == true) flipSprite(BodySprite);
        }
    }
    void LookingAtEnemy()
    {
        FocusIcon.transform.position = EnemyFocus.transform.position;
        transform.up = (Vector3.RotateTowards(transform.up, new Vector2(EnemyFocus.transform.position.x, EnemyFocus.transform.position.y) - new Vector2(transform.position.x, transform.position.y), FollowMouse_Speed, 10f));
        
        if (EnemyFocus.transform.position.x > PlayerGO.transform.position.x)
        {
            if (FlipOnce == false) flipSprite(BodySprite);

        }
        if (EnemyFocus.transform.position.x < PlayerGO.transform.position.x)
        {
            if (FlipOnce == true) flipSprite(BodySprite);
        }
    }

}
