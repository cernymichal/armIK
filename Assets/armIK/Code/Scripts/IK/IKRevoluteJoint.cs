using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gizmos = Popcron.Gizmos;

public class IKRevoluteJoint : IK1DOFJoint {
    // [SerializeField] public float maxAngle = 360.0f;

    protected override void DrawGizmos() {
        DrawLineToChild();

        Utils.GizmosDrawArc(transform.localToWorldMatrix, Vector3.up, Vector3.back, 360, Color.red);
        Gizmos.Line(transform.position,  transform.position+ transform.forward * 0.1f, Color.red);
    }
}
