using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RagdollBehaviour : MonoBehaviour {

    public Transform[] _bones;

    public float _ExpRadious = 0.3f;

    public void Awake()
    {
        SetKinematic(true);
        gameObject.SetActive(false);
    }


    //Залочить / разлочить все кости
    public void SetKinematic(bool _setTo)
    {
        foreach (var _bone in _bones)
        {
            if (_bone.GetComponent<Rigidbody>() != null)
            {
                _bone.GetComponent<Rigidbody>().isKinematic = _setTo;
            }
        }
    }


    public void InitDeath(Vector3 _bulletPos, float _force)
    {
        gameObject.SetActive(true);
        SetKinematic(false);


        //Добавляем разлет
        foreach (var _bone in _bones)
        {
            if (_bone.GetComponent<Rigidbody>() != null)
            {
                _bone.GetComponent<Rigidbody>().AddExplosionForce(_force, _bulletPos, _ExpRadious);
            }
        }
    }
    

    //Грабим повороты и положения костей
    public void SetBonesState(Transform[] _inputBones)
    {
        for (int i = 0; i < _inputBones.Length; i++)
        {
            _bones[i].position = _inputBones[i].position;
            _bones[i].rotation = _inputBones[i].rotation;
        }
    }
}
