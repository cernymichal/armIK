using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKEffector : IIKJoint {
    [SerializeField] public Transform target;

    public override IIKJoint Child() {
        return null;
    }

    private void OnDrawGizmos() {
        if (!target)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, target.transform.position);
    }
}
