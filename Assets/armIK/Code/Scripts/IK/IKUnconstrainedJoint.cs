using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gizmos = Popcron.Gizmos;

public abstract class IKJoint : MonoBehaviour {
    public Vector3 position { get { return transform.position; } set { transform.position = value; } }
    public Quaternion rotation { get { return transform.rotation; } set { transform.rotation = value; } }
    public abstract IKJoint Child();

    private void OnDrawGizmos() {
        DrawGizmos();
    }

    private void LateUpdate() {
        DrawGizmos();
    }

    protected virtual void DrawGizmos() {
        DrawLineToChild();
    }

    protected void DrawLineToChild() {
        if (!Child())
            return;

        Gizmos.Line(transform.position, Child().transform.position, Color.white);
    }
}

public class IKUnconstrainedJoint : IKJoint {
    [SerializeField] public IKJoint child = null;

    public override IKJoint Child() {
        return child;
    }
}
