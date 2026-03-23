using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Hands;

public class SpiderSpawner : MonoBehaviour
{
    [Header("Object")]
    public GameObject objPrefab;

    [Header("Hand to Trigger")]
    public bool useRightHand = true;

    [Header("Projectile Launcher")]
    public ProjectileLauncher ProjectileLauncher;

    [Header("Gesture thresholds")]
    public float fingerExtendedMin = 0.095f;
    public float fingerCurledMax = 0.075f;
    public float thumbExtendedMin = 0.09f;
    public float palmUpDotThreshold = 0.5f;

    [Header("Cooldown")]
    public float cooldownSeconds = 0.8f;

    private XRHandSubsystem handSubsystem;
    private bool wasSpider = false;
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

        bool isSpider = CheckSpider(hand);

        if (isSpider && !wasSpider && Time.time >= lastSpawnTime + cooldownSeconds)
        {
            SpawnObjInFrontOfPalm(hand);
            lastSpawnTime = Time.time;
        }

        wasSpider = isSpider;
    }

    public bool CheckSpider(XRHand hand)
    {
        var palm = hand.GetJoint(XRHandJointID.Palm);

        var thumbTip = hand.GetJoint(XRHandJointID.ThumbTip);
        var indexTip = hand.GetJoint(XRHandJointID.IndexTip);
        var middleTip = hand.GetJoint(XRHandJointID.MiddleTip);
        var ringTip = hand.GetJoint(XRHandJointID.RingTip);
        var littleTip = hand.GetJoint(XRHandJointID.LittleTip);

        if (!palm.TryGetPose(out Pose palmPose)) return false;

        if (!thumbTip.TryGetPose(out Pose thumbPose)) return false;
        if (!indexTip.TryGetPose(out Pose indexPose)) return false;
        if (!middleTip.TryGetPose(out Pose middlePose)) return false;
        if (!ringTip.TryGetPose(out Pose ringPose)) return false;
        if (!littleTip.TryGetPose(out Pose littlePose)) return false;


        float thumbDist = Vector3.Distance(thumbPose.position, palmPose.position);
        float indexDist = Vector3.Distance(indexPose.position, palmPose.position);
        float middleDist = Vector3.Distance(middlePose.position, palmPose.position);
        float ringDist = Vector3.Distance(ringPose.position, palmPose.position);
        float littleDist = Vector3.Distance(littlePose.position, palmPose.position);

        bool thumbExtended = thumbDist >= thumbExtendedMin;
        bool indexExtended = indexDist >= fingerExtendedMin;
        bool littleExtended = littleDist >= fingerExtendedMin;

        bool middleCurled = middleDist <= fingerCurledMax;
        bool ringCurled = ringDist <= fingerCurledMax;

        Vector3 palmNormal = -palmPose.up;
        bool palmFacingUp = Vector3.Dot(palmNormal, Vector3.up) > palmUpDotThreshold;

        return palmFacingUp &&
               thumbExtended &&
               indexExtended &&
               littleExtended &&
               middleCurled &&
               ringCurled;
    }

    void SpawnObjInFrontOfPalm(XRHand hand)
    {
        /*var palm = hand.GetJoint(XRHandJointID.Palm);
        if (!palm.TryGetPose(out Pose palmPose)) return;

        Vector3 spawnPos = palmPose.position + (palmPose.rotation * Vector3.forward * 0.25f);
        Quaternion spawnRot = Quaternion.identity;

        Instantiate(objPrefab, spawnPos, spawnRot);*/
        ProjectileLauncher.LaunchObject();
    }
}