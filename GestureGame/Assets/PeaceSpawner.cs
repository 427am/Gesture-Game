using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Hands;

public class PeaceSpawner : MonoBehaviour
{
    [Header("Object")]
    public GameObject objPrefab;

    [Header("Hand to Trigger")]
    public bool useRightHand = true;

    [Header("Gesture thresholds")]
    public float fingerExtendedMin = 0.11f;
    public float fingerCurledMax = 0.08f;
    public float indexMiddleSeparationMin = 0.04f;
    public float thumbExtendedMax = 0.11f;

    [Header("Cooldown")]
    public float cooldownSeconds = 0.8f;

    private XRHandSubsystem handSubsystem;
    private bool wasPeace = false;
    private float lastSpawnTime = -999f;

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

        bool isPeace = CheckPeace(hand);

        if (isPeace && !wasPeace && Time.time >= lastSpawnTime + cooldownSeconds)
        {
            SpawnObjInFrontOfPalm(hand);
            lastSpawnTime = Time.time;
        }

        wasPeace = isPeace;
    }

    bool CheckPeace(XRHand hand)
    {
        var palm = hand.GetJoint(XRHandJointID.Palm);

        var thumbTip = hand.GetJoint(XRHandJointID.ThumbTip);
        var indexTip = hand.GetJoint(XRHandJointID.IndexTip);
        var middleTip = hand.GetJoint(XRHandJointID.MiddleTip);
        var ringTip = hand.GetJoint(XRHandJointID.RingTip);
        var littleTip = hand.GetJoint(XRHandJointID.LittleTip);

        if (!palm.TryGetPose(out Pose palmPose)) return false;

        bool hasThumb = thumbTip.TryGetPose(out Pose thumbPose);
        if (!indexTip.TryGetPose(out Pose indexPose)) return false;
        if (!middleTip.TryGetPose(out Pose middlePose)) return false;
        if (!ringTip.TryGetPose(out Pose ringPose)) return false;
        if (!littleTip.TryGetPose(out Pose littlePose)) return false;

        float indexDist = Vector3.Distance(indexPose.position, palmPose.position);
        float middleDist = Vector3.Distance(middlePose.position, palmPose.position);
        float ringDist = Vector3.Distance(ringPose.position, palmPose.position);
        float littleDist = Vector3.Distance(littlePose.position, palmPose.position);

        bool indexExtended = indexDist >= fingerExtendedMin;
        bool middleExtended = middleDist >= fingerExtendedMin;

        bool ringCurled = ringDist <= fingerCurledMax;
        bool littleCurled = littleDist <= fingerCurledMax;

        float indexMiddleSeparation = Vector3.Distance(indexPose.position, middlePose.position);
        bool fingersSeparated = indexMiddleSeparation >= indexMiddleSeparationMin;

        bool thumbOk = true;
        if (hasThumb)
        {
            float thumbDist = Vector3.Distance(thumbPose.position, palmPose.position);
            thumbOk = thumbDist <= thumbExtendedMax;
        }

        return indexExtended && middleExtended && ringCurled && littleCurled && fingersSeparated && thumbOk;
    }

    void SpawnObjInFrontOfPalm(XRHand hand)
    {
        var palm = hand.GetJoint(XRHandJointID.Palm);
        if (!palm.TryGetPose(out Pose palmPose)) return;

        Vector3 spawnPos = palmPose.position + (palmPose.rotation * Vector3.forward * 0.25f);
        Quaternion spawnRot = Quaternion.identity;

        Instantiate(objPrefab, spawnPos, spawnRot);
    }
}