using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IKJoint : MonoBehaviour {
    public Vector3 position { get { return transform.position; } set { transform.position = value; } }
    public Quaternion rotation { get { return transform.rotation; } set { transform.rotation = value; } }
    public abstract IKJoint Child();

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

public abstract class IK1DOFJoint : IKJoint {
    [SerializeField] public IKJoint child = null;

    public override IKJoint Child() {
        return child;
    }
}
