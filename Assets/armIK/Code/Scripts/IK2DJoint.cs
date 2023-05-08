using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK2DJoint : IIKJoint {
    [SerializeField] public IIKJoint child = null;

    public override IIKJoint Child() {
        return child;
    }

    private void OnDrawGizmos() {
        if (!child)
            return;

        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, child.transform.position);
    }
}
