using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestExplo : MonoBehaviour
{

    public float Force = 100f;

	void Update () {
	    if (Input.GetMouseButtonDown(0))
	    {
	        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	        var radious = 3f;

	        var hits = Physics.SphereCastAll(ray, radious);
	        foreach (var hit in hits)
	        {
	            var position = hit.point;
	            var rigidbody = hit.rigidbody;

                rigidbody.AddExplosionForce(Force, position, radious, 0, ForceMode.Impulse);
	        }
	    }
	}

    void OnDrawGizmos()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var radious = 3f;
        RaycastHit hit;

        if (Physics.SphereCast(ray, radious, out hit))
        {
            Gizmos.DrawWireSphere(hit.point, radious);
        }
    }
}
