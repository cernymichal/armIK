using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKUnconstrainedJoint : IIKJoint {
    [SerializeField] public IIKJoint child = null;

    public override IIKJoint Child() {
        return child;
    }
}
