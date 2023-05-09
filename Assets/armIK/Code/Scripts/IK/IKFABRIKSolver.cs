using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFABRIKSolver : MonoBehaviour {
    [SerializeField] public IIKJoint root = null;
    [SerializeField] private int maxSteps = 16;
    [SerializeField] private float targetDistanceTolerance = 0.001f;
    [SerializeField] private float minIterationImprovement = 0.005f;

    private List<IIKJoint> joints = new List<IIKJoint>();
    private List<float> distances = new List<float>();
    private float distanceSum = 0f;

    Vector3 origin = Vector3.zero;
    Transform target = null;

    private void Awake() {
        InitializeSystem();
    }

    private void Update() {
        if (joints.Count == 0)
            return;

        origin = joints[0].transform.position;
        target = (joints[joints.Count - 1] as IKEffector)?.target;
        if (!target)
            return;

        if (Vector3.Distance(origin, target.position) >= distanceSum) {
            FABRIKPass();
            return;
        }


        var targetDistance = DistanceToTarget();
        for (int i = 0; i < maxSteps && targetDistance > targetDistanceTolerance; i++) {
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

        IIKJoint joint = root;
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
        joints[joints.Count - 1].transform.position = target.position;
        for (int i = joints.Count - 2; i >= 0; i--) {
            var jointA = joints[i + 1];
            var jointB = joints[i];

            var distance = distances[i];
            var difference = (jointA.transform.position - jointB.transform.position).magnitude;
            var lamda = distance / difference;
            jointB.transform.position = (1 - lamda) * jointA.transform.position + lamda * jointB.transform.position;
        }

        // Backward reaching
        joints[0].transform.position = origin;
        for (int i = 0; i < joints.Count - 1; i++) {
            var jointA = joints[i];
            var jointB = joints[i + 1];

            var distance = distances[i];
            var difference = (jointA.transform.position - jointB.transform.position).magnitude;
            var lamda = distance / difference;
            jointB.transform.position = (1 - lamda) * jointA.transform.position + lamda * jointB.transform.position;
        }
    }

    private float DistanceToTarget() {
        return Vector3.Distance(joints[joints.Count - 1].transform.position, target.position);
    }
}
