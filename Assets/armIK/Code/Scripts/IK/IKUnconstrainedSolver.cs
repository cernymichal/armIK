using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class IKUnconstrainedSolver : MonoBehaviour {
    [SerializeField] public IKJoint root = null;
    [SerializeField] private int maxIterations = 16;
    [SerializeField] private float targetDistanceTolerance = 0.001f;
    [SerializeField] private float minIterationImprovement = 0.005f;
    [SerializeField] private bool solveFixedUpdate = true;

    private List<IKJoint> joints = new List<IKJoint>();
    private List<float> distances = new List<float>();
    private float distanceSum = 0f;

    Transform target = null;

    private void Awake() {
        InitializeSystem();
    }

    private void FixedUpdate() {
        if (solveFixedUpdate)
            Solve();
    }

    public void Solve() {
        if (joints.Count == 0)
            return;

        target = (joints[joints.Count - 1] as IKEffector)?.target;
        if (!target)
            return;

        if (Vector3.Distance(joints[0].position, target.position) >= distanceSum) {
            FABRIKPass();
            return;
        }

        var targetDistance = DistanceToTarget();
        for (int i = 0; i < maxIterations && targetDistance > targetDistanceTolerance; i++) {
            FABRIKPass();

            var newTargetDistance = DistanceToTarget();
            if (targetDistance - newTargetDistance < minIterationImprovement)
                break;

            targetDistance = newTargetDistance;
        }
    }

    public void InitializeSystem() {
        if (!root)
            return;

        IKJoint joint = root;
        while (joint) {
            joints.Add(joint);
            var child = joint.Child();
            if (!child)
                break;

            var distance = Vector3.Distance(joint.transform.position, child.transform.position);
            distances.Add(distance);
            distanceSum += distance;

            joint = child;
        }
    }

    private void FABRIKPass() {
        // Forward reaching
        joints[joints.Count - 1].position = target.position;
        // Root joint is skipped so we dont have to reset its position
        for (int i = joints.Count - 2; i > 0; i--) {
            var PPrev = joints[i + 1];
            var PI = joints[i];

            var lamda = distances[i] / Vector3.Distance(PI.position, PPrev.position);
            PI.position = (1 - lamda) * PPrev.position + lamda * PI.position;
        }

        // Backward reaching
        for (int i = 1; i < joints.Count; i++) {
            var PI = joints[i];
            var PPrev = joints[i - 1];

            var lamda = distances[i - 1] / Vector3.Distance(PI.position, PPrev.position);
            PI.position = (1 - lamda) * PPrev.position + lamda * PI.position;
        }
    }

    private float DistanceToTarget() {
        return Vector3.Distance(joints[joints.Count - 1].transform.position, target.position);
    }
}
