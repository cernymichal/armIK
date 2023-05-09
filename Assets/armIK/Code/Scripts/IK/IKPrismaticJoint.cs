using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKPrismaticJoint : IKUnconstrainedJoint {
    [SerializeField] public float minLength = 0.0f;
    [SerializeField] public float maxLength = 1.0f;

    // TODO Position constraint
}
