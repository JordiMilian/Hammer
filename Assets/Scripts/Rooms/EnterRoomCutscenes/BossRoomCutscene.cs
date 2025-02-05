using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnterTriggerCutscene;

public class BossRoomCutscene : BaseCutsceneLogic
{

    [SerializeField] RoomWithEnemiesLogic enemyRoomLogic;
    [SerializeField] float zoomToBoss;
    [SerializeField] AnimationClip bossEnterAnimationClip;
    [SerializeField] UI_BossHealthBar healthBar;
    Animator bossAnimator;
    public override void playThisCutscene()
    {
       currentCutscene = StartCoroutine(bossCutscene());
    }
    IEnumerator bossCutscene()
    {
        //Find the references
        CameraZoomController zoomer = GameObject.Find(Tags.CMvcam1).GetComponent<CameraZoomController>();
        Transform bossTf = enemyRoomLogic.CurrentlySpawnedEnemies[0].transform;
        TargetGroupSingleton targetGroup = TargetGroupSingleton.Instance;
        Enemy_References bossRefs = bossTf.GetComponent<Enemy_References>();
        Enemy_StateController_BossEnemy bossState = (Enemy_StateController_BossEnemy)bossRefs.stateController;

        //disable player
        Player_EventSystem playerEvents = GlobalPlayerReferences.Instance.references.events;
        playerEvents.CallDisable();

        //Wait just in case for enemies to spawn
        yield return new WaitForSeconds(0.1f);

        //ACTIVATE ANIMATOR TRIGGER FOR INTENDED ANIMATION

        yield return StartCoroutine(bossState.BossIntroAnimation());

        //Zoom as intended
        zoomer.AddZoomInfoAndUpdate(new CameraZoomController.ZoomInfo(zoomToBoss, 3, "enterCutscene"));

        //Target the camera to boss
        targetGroup.SetOnlyOneTarget(bossTf, 50, 1);


        healthBar.ShowCanvas();

        yield return new WaitForSeconds(1f);


        bossState.ForceAgroo();
        
        yield return new WaitForSeconds(.3f);

        //return to basics
        zoomer.RemoveZoomInfoAndUpdate("enterCutscene");
        targetGroup.ReturnPlayersTarget();
        targetGroup.RemoveTarget(bossTf);


        //enable player again
        playerEvents.CallEnable();

        //Focus the boss
        //Player_FollowMouseWithFocus_V2 followMouse = GlobalPlayerReferences.Instance.references.followMouse;
        //followMouse.AttemptFocus(false, false);

        onCutsceneOver?.Invoke();
    }
}
