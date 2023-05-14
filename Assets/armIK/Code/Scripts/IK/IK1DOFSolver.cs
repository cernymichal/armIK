using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class IK1DOFSolver : MonoBehaviour {
    [SerializeField] public IKJoint root = null;
    [SerializeField] private int maxIterations = 16;
    [SerializeField] private float targetDistanceTolerance = 0.001f;
    [SerializeField] private float minIterationImprovement = 0.005f;
    [SerializeField] private bool solveFixedUpdate = true;

    private List<IKJoint> joints = new List<IKJoint>();
    private List<float> distances = new List<float>();
    private List<Quaternion> relativeRotations = new List<Quaternion>();
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

        joints = new List<IKJoint>();
        distances = new List<float>();
        relativeRotations = new List<Quaternion>();
        distanceSum = 0f;

        IKJoint joint = root;
        while (joint) {
            joints.Add(joint);
            var child = joint.Child();
            if (!child)
                break;

            var distance = Vector3.Distance(joint.transform.position, child.transform.position);
            distances.Add(distance);
            distanceSum += distance;

            var relativeRotation = Quaternion.Inverse(child.transform.rotation) * joint.transform.rotation;
            relativeRotations.Add(relativeRotation);

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
            var PNext = joints[i - 1];

            var PIHat = findPIHat(PPrev, PI, distances[i], relativeRotations[i]);
            PI.position = PIHat.Item1;
            PI.rotation = PIHat.Item2;

            var theta = findTheta(PPrev, PI, PNext);
            PI.transform.RotateAround(PPrev.position, PPrev.transform.up, theta);
        }

        // Backward reaching
        for (int i = 1; i < joints.Count; i++) {
            var PI = joints[i];
            var PPrev = joints[i - 1];

            var PIHat = findPIHat(PPrev, PI, distances[i - 1], relativeRotations[i - 1]);
            PI.position = PIHat.Item1;
            PI.rotation = PIHat.Item2;

            if (i == joints.Count - 1)
                continue;

            var PNext = joints[i + 1];
            var theta = findTheta(PPrev, PI, PNext);
            PI.transform.RotateAround(PPrev.position, PPrev.transform.up, theta);
        }
    }

    /// <summary>
    ///     Find a new P_i that satisfies the P_prev constraint
    /// </summary>
    private Tuple<Vector3, Quaternion> findPIHat(IKJoint PPrev, IKJoint PI, float distance, Quaternion relativeRotation) {
        var projectedPosition = Vector3.ProjectOnPlane(PI.position - PPrev.position, PPrev.transform.up);
        var position = projectedPosition.normalized * distance + PPrev.position;
        var rotation = Quaternion.LookRotation((PPrev.position - position).normalized, PPrev.transform.up) * relativeRotation;

        return new Tuple<Vector3, Quaternion>(position, rotation);
    }

    /// <summary>
    ///     Calculate the angle to rotate P_i around the actuation vector of P_prev to satisfy the P_next constraint,
    ///     while not breaking the P_prev constraint
    /// </summary>
    private float findTheta(IKJoint PPrev, IKJoint PI, IKJoint PNext) {
        var phiIN = Vector3.Cross(PNext.transform.up, (PNext.position - PPrev.position).normalized).normalized;
        // Debug.DrawRay(PPrev.position + (PNext.position - PPrev.position) / 2, phiIN, Color.red, 5f);

        var phiINProjectedToPhiPrev = Vector3.ProjectOnPlane(phiIN, PPrev.transform.up);
        if (Vector3.Dot(phiINProjectedToPhiPrev, PI.transform.up) < 0)
            phiINProjectedToPhiPrev *= -1f;

        // Debug.DrawRay(PI.position, phiINProjectedToPhiPrev, Color.magenta, 5f);

        Quaternion.FromToRotation(PI.transform.up, phiINProjectedToPhiPrev).ToAngleAxis(out var angle, out var axis);
        return angle;
    }

    private float DistanceToTarget() {
        return Vector3.Distance(joints[joints.Count - 1].position, target.position);
    }
}
