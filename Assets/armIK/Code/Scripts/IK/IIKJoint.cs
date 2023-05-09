using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IIKJoint : MonoBehaviour {
    public abstract IIKJoint Child();

    private void OnDrawGizmos() {
        DrawLineToChild();
    }

    protected void DrawLineToChild() {
        if (!Child())
            return;

        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, Child().transform.position);
    }
}
