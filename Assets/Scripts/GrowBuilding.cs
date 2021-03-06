﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowBuilding : Singleton<GrowBuilding> {
    public List<Vector3> positions;
    Vector3 nextPostion;
    public ShakeTransformS cameraShaker;
    public List<ParticleSystem> dustParticles;
    public bool IsMoving { get; private set; }
    public int CurrentFloor { get; private set; }
    public Action BuildStopMovingAction;
    public Action BuildStartMovingAction;
    private int[] levelPointAmounts = { 600, 2000, 6000 };

    #region TEST
    [ContextMenu("TEST Move To 0")]
    public void TESTMoveTo0() {
        SetInPostion(0);
    }
    [ContextMenu("TEST Move To 1")]
    public void TESTMoveTo1() {
        SetInPostion(1);
    }
    [ContextMenu("TEST Move To 2")]
    public void TESTMoveTo2() {
        SetInPostion(2);
    }
    [ContextMenu("TEST Move To 3")]
    public void TESTMoveTo3() {
        SetInPostion(3);
    }
    #endregion

    private void Awake() {
        dustParticles.ForEach(o => o.Stop());

    }

    private void Update() {
        // Maybe move it to another script
        if (GameController.Instance != null)
            if (!GameController.Instance.isMainMenu)
                if (RoomManager.Instance.totalPoints > levelPointAmounts[0] && CurrentFloor == 0) {
                    SetInPostion(1);
                } else if (RoomManager.Instance.totalPoints > levelPointAmounts[1] && CurrentFloor == 1) {
                    SetInPostion(2);
                } else if (RoomManager.Instance.totalPoints > levelPointAmounts[2] && CurrentFloor == 2) {
                    SetInPostion(3);
                }
    }

    public void MoveUpFloor() {
        if (CurrentFloor <= positions.Count - 2) {
            SetInPostion(CurrentFloor + 1);
        }
    }
    public void MoveDownPosition() {
        if (CurrentFloor > 0) {
            SetInPostion(CurrentFloor - 1);
        }
    }

    public void SetInPostion(int indexPosition) {
        if (AudioManager.Instance != null)
            AudioManager.Instance.Play("GrowBuild");
        nextPostion = positions[indexPosition];
        StartCoroutine(MoveToNextPositon());
        CurrentFloor = indexPosition;
    }

    private IEnumerator MoveToNextPositon() {
        IsMoving = true;
        OnBuildStartMoving();
        cameraShaker.Begin();
        dustParticles.ForEach(o => o.Play());
        while (Vector3.Distance(transform.position, nextPostion) > 0) {
            transform.position = Vector3.MoveTowards(transform.position, nextPostion, Time.deltaTime);
            yield return new WaitForSeconds(.01f);
        }
        dustParticles.ForEach(o => o.Stop());
        cameraShaker.Stop();
        transform.position = nextPostion;
        IsMoving = false;
    }

    public void OnBuildStartMoving() {
        if (BuildStartMovingAction != null) {
            BuildStartMovingAction();
        }
    }
    public void OnBuildStopMoving() {
        if (BuildStopMovingAction != null) {
            BuildStopMovingAction();
        }
    }
}
