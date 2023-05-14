using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gizmos = Popcron.Gizmos;

public class IKEffector : IKJoint {
    [SerializeField] public Transform target;

    public override IKJoint Child() {
        return null;
    }

    protected override void DrawGizmos() {
        if (!target)
            return;

        Gizmos.Line(transform.position, target.transform.position, Color.green);
    }
}
