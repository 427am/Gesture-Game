using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Hands;

public class ThumbsUpSpawner : MonoBehaviour
{
    [Header("Object")]
    public GameObject objPrefab;

    [Header("Hand to Trigger")]
    public bool useRightHand = true;

    [Header("Projectile Launcher")]
    public ProjectileLauncher ProjectileLauncher;

    [Header("Gesture thresholds")]
    public float thumbExtendedMin = 0.06f;
    public float otherFingerCurledMax = 0.095f;
    public float thumbUpDotMin = 0.2f;

    [Header("Gesture timing")]
    public float requiredHoldSeconds = 0.1f;

    [Header("Cooldown")]
    public float cooldownSeconds = 0.8f;

    private XRHandSubsystem handSubsystem;
    private bool wasThumbsUp = false;
    private float lastSpawnTime = -999f;
    private float thumbsUpStartTime = -1f;

    void Start()
    {
        var subsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(subsystems);

        if (subsystems.Count > 0)
            handSubsystem = subsystems[0];
    }

    void Update()
    {
        if (handSubsystem == null || objPrefab == null) return;

        XRHand hand = useRightHand ? handSubsystem.rightHand : handSubsystem.leftHand;

        if (!hand.isTracked) return;

        bool isThumbsUpNow = CheckThumbsUp(hand);

        if (isThumbsUpNow)
        {
            if (thumbsUpStartTime < 0f)
                thumbsUpStartTime = Time.time;
        }
        else
        {
            thumbsUpStartTime = -1f;
        }

        bool heldLongEnough =
            thumbsUpStartTime >= 0f &&
            (Time.time - thumbsUpStartTime) >= requiredHoldSeconds;

        if (heldLongEnough && !wasThumbsUp && Time.time >= lastSpawnTime + cooldownSeconds)
        {
            SpawnInFrontOfPalm(hand);
            lastSpawnTime = Time.time;
            wasThumbsUp = true;
        }

        if (!isThumbsUpNow)
            wasThumbsUp = false;
    }

    bool CheckThumbsUp(XRHand hand)
    {
        var palm = hand.GetJoint(XRHandJointID.Palm);

        var thumbTip = hand.GetJoint(XRHandJointID.ThumbTip);
        var thumbProx = hand.GetJoint(XRHandJointID.ThumbProximal);

        var indexTip = hand.GetJoint(XRHandJointID.IndexTip);
        var middleTip = hand.GetJoint(XRHandJointID.MiddleTip);
        var ringTip = hand.GetJoint(XRHandJointID.RingTip);
        var littleTip = hand.GetJoint(XRHandJointID.LittleTip);

        if (!palm.TryGetPose(out Pose palmPose)) return false;

        if (!thumbTip.TryGetPose(out Pose thumbTipPose)) return false;
        if (!thumbProx.TryGetPose(out Pose thumbProxPose)) return false;

        if (!indexTip.TryGetPose(out Pose indexPose)) return false;
        if (!middleTip.TryGetPose(out Pose middlePose)) return false;
        if (!ringTip.TryGetPose(out Pose ringPose)) return false;
        if (!littleTip.TryGetPose(out Pose littlePose)) return false;

        float thumbDist = Vector3.Distance(thumbTipPose.position, palmPose.position);
        bool thumbExtended = thumbDist >= thumbExtendedMin;

        float indexDist = Vector3.Distance(indexPose.position, palmPose.position);
        float middleDist = Vector3.Distance(middlePose.position, palmPose.position);
        float ringDist = Vector3.Distance(ringPose.position, palmPose.position);
        float littleDist = Vector3.Distance(littlePose.position, palmPose.position);

        Vector3 indexDir = (indexPose.position - palmPose.position).normalized;
        Vector3 middleDir = (middlePose.position - palmPose.position).normalized;
        Vector3 ringDir = (ringPose.position - palmPose.position).normalized;
        Vector3 littleDir = (littlePose.position - palmPose.position).normalized;

        bool indexCurled = indexDist <= otherFingerCurledMax && Vector3.Dot(indexDir, palmPose.forward) < 0.3f;
        bool middleCurled = middleDist <= otherFingerCurledMax && Vector3.Dot(middleDir, palmPose.forward) < 0.3f;
        bool ringCurled = ringDist <= otherFingerCurledMax && Vector3.Dot(ringDir, palmPose.forward) < 0.3f;
        bool littleCurled = littleDist <= otherFingerCurledMax && Vector3.Dot(littleDir, palmPose.forward) < 0.3f;

        bool otherFingersCurled = indexCurled && middleCurled && ringCurled && littleCurled;

        Vector3 thumbDir = (thumbTipPose.position - thumbProxPose.position).normalized;
        float upDot = Vector3.Dot(thumbDir, Vector3.up);
        bool thumbPointingUp = upDot >= thumbUpDotMin;

        return thumbExtended && otherFingersCurled && thumbPointingUp;
    }

    void SpawnInFrontOfPalm(XRHand hand)
    {
        /* var palm = hand.GetJoint(XRHandJointID.Palm);

        if (!palm.TryGetPose(out Pose palmPose)) return;

        Vector3 spawnPos =
            palmPose.position +
            (palmPose.rotation * Vector3.forward * 0.25f);

        Instantiate(objPrefab, spawnPos, Quaternion.identity); */
        ProjectileLauncher.LaunchObject();
    }
}