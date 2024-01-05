using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMuzzle
{
    void Fire();

    public void SetPrefab(GameObject prefab);

    public void SetShotSpeed(float speed);
}