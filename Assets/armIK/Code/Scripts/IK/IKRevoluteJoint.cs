using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKRevoluteJoint : IK1DOFJoint {
    // [SerializeField] public float maxAngle = 360.0f;

    private void OnDrawGizmos() {
        DrawLineToChild();
        Gizmos.color = Color.red;
        Utils.GizmosDrawArc(transform.localToWorldMatrix, Vector3.up, Vector3.back, 360);
    }
}
