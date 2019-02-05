using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollMaker : MonoBehaviour
{

    public Transform[] bones;
    public RagdollBehaviour ragdoll;
    public float deathForce;
    public Vector3 boomPoint;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MakeDeath();
        }
    }

    //Убиваем наше тело
    public void MakeDeath()
    {
        boomPoint = Vector3.zero;
        if (boomPoint == null)
            boomPoint = new Vector3(0f, 0f, 0f);
        CopyBonesState(ragdoll);
        
        ragdoll.InitDeath(boomPoint, deathForce);
    }

    //Копируем свои кости в рагдол
    public void CopyBonesState(RagdollBehaviour _ragdoll)
    {
        _ragdoll.SetBonesState(bones);
    }
}
