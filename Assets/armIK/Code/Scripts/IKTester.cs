using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class IKTester : MonoBehaviour {
    [SerializeField] private IK1DOFSolver solver = null;
    [SerializeField] private IKEffector effector = null;
    [SerializeField] private IKJoint root = null;
    [SerializeField] private int maxIterations = 1024;
    [SerializeField] private float targetDistanceTolerance = 0.01f;
    [SerializeField] private float maxTestDistance = 1.5f;
    [SerializeField] private float minTestDistance = 0.5f;
    [SerializeField] private int testAngleResolution = 10;
    [SerializeField] private int testDistanceResolution = 4;
    [SerializeField] private bool test = true;

    private UInt64 testCount = 0;
    private UInt64 successCount = 0;
    private UInt64 successInterationCount = 0;
    private List<Vector3> initialJointPositions;
    private List<Quaternion> initialJointRotations;

    private void FixedUpdate() {
        if (test) {
            test = false;
            Test();
        }
    }

    public void Test() {
        testCount = 0;
        successCount = 0;
        successInterationCount = 0;

        if (!solver || !effector || !root)
            return;

        var target = effector.target;
        if (!target)
            return;

        solver.InitializeSystem();
        SaveJoints();

        for (float yRotation = 0; yRotation < 360; yRotation += 360 / testAngleResolution) {
            for (float xRotation = 360 / testAngleResolution; xRotation < 360 - 360 / testAngleResolution; xRotation += 360 / (testAngleResolution + 2)) {
                for (var distance = minTestDistance; distance <= maxTestDistance; distance += (maxTestDistance - minTestDistance) / (testDistanceResolution - 1)) {
                    testCount++;
                    target.position = root.position + Quaternion.Euler(xRotation, yRotation, 0) * Vector3.forward * distance;
                    //Debug.DrawLine(root.position, target.position, Color.red, 1f);

                    int iterations = 0;
                    for (int i = 0; i < maxIterations && Vector3.Distance(effector.position, target.position) > targetDistanceTolerance; i++) {
                        iterations++;
                        solver.Solve();
                    }

                    if (Vector3.Distance(effector.position, target.position) <= targetDistanceTolerance) {
                        successCount++;
                        successInterationCount += (UInt64)iterations;
                    }

                    ResetJoints();
                }
            }
        }

        Debug.LogFormat("{0} tests", testCount);
        Debug.LogFormat("{0} successes", successCount);
        Debug.LogFormat("{0}% success rate", (((double)successCount / (double)testCount) * 100f));
        Debug.LogFormat("{0} average iterations", ((double)successInterationCount / (double)successCount));
        Debug.LogFormat("{0} m tolerance", targetDistanceTolerance);
        Debug.LogFormat("{0} - {1} m range", minTestDistance, maxTestDistance);
        Debug.LogFormat("{0} maximum iterations", maxIterations);
    }

    private void SaveJoints() {
        if (!root)
            return;

        initialJointPositions = new List<Vector3>();
        initialJointRotations = new List<Quaternion>();

        IKJoint joint = root;
        while (joint) {
            initialJointPositions.Add(joint.position);
            initialJointRotations.Add(joint.rotation);
            joint = joint.Child();
        }
    }

    private void ResetJoints() {
        if (!root)
            return;

        IKJoint joint = root;
        for (int i = 0; i < initialJointPositions.Count; i++) {
            joint.position = initialJointPositions[i];
            joint.rotation = initialJointRotations[i];
            joint = joint.Child();
        }
    }
}
