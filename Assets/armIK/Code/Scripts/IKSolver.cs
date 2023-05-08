using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKSolver : MonoBehaviour {
    [SerializeField] public IIKJoint root = null;
    [SerializeField] private int maxSteps = 16;
    [SerializeField] private float targetTolerance = 0.001f;

    private List<IIKJoint> joints = new List<IIKJoint>();
    private List<float> distances = new List<float>();
    private float distanceSum = 0f;

    Vector3 origin = Vector3.zero;
    Transform target = null;

    private void Awake() {
        LoadSystem();
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

        for (int i = 0; i < maxSteps && Vector3.Distance(joints[joints.Count - 1].transform.position, target.position) >= targetTolerance; i++)
            FABRIKPass();
    }

    public void LoadSystem() {
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
        Backwards();
        Forwards();
    }

    private void Backwards() {
        var targetPosition = target.position;
        for (int i = joints.Count - 1; i > 0; i--) {
            var jointA = joints[i];
            var jointB = joints[i - 1];

            var distance = distances[i - 1];
            jointA.transform.position = targetPosition;
            var direction = (jointB.transform.position - jointA.transform.position).normalized;
            targetPosition = jointA.transform.position + direction * distance;
        }
        joints[0].transform.position = targetPosition;
    }

    private void Forwards() {
        var targetPosition = origin;
        for (int i = 0; i < joints.Count - 1; i++) {
            var jointA = joints[i];
            var jointB = joints[i + 1];

            var distance = distances[i];
            jointA.transform.position = targetPosition;
            var direction = (jointB.transform.position - jointA.transform.position).normalized;
            targetPosition = jointA.transform.position + direction * distance;
        }
        joints[joints.Count - 1].transform.position = targetPosition;
    }
}
