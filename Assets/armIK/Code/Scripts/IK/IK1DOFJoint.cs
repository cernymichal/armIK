using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IK1DOFJoint : IKJoint {
    [SerializeField] public IKJoint child = null;

    public override IKJoint Child() {
        return child;
    }
}
