using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKSolver : MonoBehaviour {
    [SerializeField] public IKJoint root = null;
    [SerializeField] private int maxSteps = 16;
    [SerializeField] private float targetDistanceTolerance = 0.001f;
    [SerializeField] private float minIterationImprovement = 0.005f;
    [SerializeField] private bool solve = false;

    private List<IKJoint> joints = new List<IKJoint>();
    private List<float> distances = new List<float>();
    private List<Quaternion> relativeRotations = new List<Quaternion>();
    private float distanceSum = 0f;

    Vector3 origin = Vector3.zero;
    Quaternion originRotation = Quaternion.identity;
    Transform target = null;

    private void Awake() {
        InitializeSystem();
    }

    private void Update() {
        //if (!solve)
        //     return;

        //solve = false;

        if (joints.Count == 0)
            return;

        origin = joints[0].transform.position;
        originRotation = joints[0].transform.rotation;
        target = (joints[joints.Count - 1] as IKEffector)?.target;
        if (!target)
            return;

        if (Vector3.Distance(origin, target.position) >= distanceSum) {
            FABRIKPass();
            return;
        }


        var targetDistance = DistanceToTarget();
        for (int i = 0; true || (i < maxSteps && targetDistance > targetDistanceTolerance); i++) {
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

            var relativeRotation = Quaternion.Inverse(child.transform.rotation) * joint.transform.rotation;
            relativeRotations.Add(relativeRotation);

            joint = child;
        }
    }

    private void FABRIKPass() {
        // Forward reaching
        var PPrevOriginalPosition = joints[joints.Count - 1].position;
        joints[joints.Count - 1].position = target.position;
        for (int i = joints.Count - 2; i > 0; i--) {
            var PPrev = joints[i + 1];
            var PI = joints[i];
            var PNext = joints[i - 1];

            // project PI into the constrained plane of PPrev
            var projectedPosition = Vector3.ProjectOnPlane(PI.position - PPrev.position, PPrev.transform.up);
            PPrevOriginalPosition = PI.position;
            PI.position = projectedPosition.normalized * distances[i] + PPrev.position;

            PI.rotation = Quaternion.LookRotation((PPrev.position - PI.position).normalized, PPrev.transform.up) * relativeRotations[i];

            var phiIN = Vector3.Cross(PNext.transform.up, (PNext.position - PPrev.position).normalized).normalized;
            //Debug.DrawRay(PPrev.position + (PNext.position - PPrev.position) / 2, phiIN, Color.red, 5f);

            /*
            var jointA = joints[i + 1];
            var jointB = joints[i];

            var distance = distances[i];
            var difference = (jointA.transform.position - jointB.transform.position).magnitude;
            var lambda = distance / difference;
            jointB.transform.position = (1 - lambda) * jointA.transform.position + lambda * jointB.transform.position;
            */
        }

        // Backward reaching
        //joints[0].transform.position = origin;
        //joints[0].transform.rotation = originRotation;
        for (int i = 1; i < joints.Count; i++) {
            //var PNext = joints[i + 1];
            var PI = joints[i];
            var PPrev = joints[i - 1];

            // project PI into the constrained plane of PPrev
            var projectedPosition = Vector3.ProjectOnPlane(PI.position - PPrev.position, PPrev.transform.up);
            PI.position = projectedPosition.normalized * distances[i - 1] + PPrev.position;

            PI.rotation = Quaternion.LookRotation((PPrev.position - PI.position).normalized, PPrev.transform.up) * relativeRotations[i - 1];

            /*
            var jointA = joints[i];
            var jointB = joints[i + 1];

            var distance = distances[i];
            var difference = (jointA.transform.position - jointB.transform.position).magnitude;
            var lambda = distance / difference;
            jointB.transform.position = (1 - lambda) * jointA.transform.position + lambda * jointB.transform.position;
            */
        }
    }

    private float DistanceToTarget() {
        return Vector3.Distance(joints[joints.Count - 1].transform.position, target.position);
    }
}
