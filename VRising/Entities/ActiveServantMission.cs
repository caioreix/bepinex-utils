using System;
using System.Collections.Generic;

using Unity.Collections;
using Unity.Entities;

using Utils.VRising.Hooks;

namespace Utils.VRising.Entities;

public static class ActiveServantMission {
    // Get the entities of component type ActiveServantMission.
    public static NativeArray<Entity> GetAll(EntityManager em) {
        var servantMissionsQuery = em.CreateEntityQuery(
                ComponentType.ReadWrite<ProjectM.ActiveServantMission>()
            );
        return servantMissionsQuery.ToEntityArray(Allocator.Temp);
    }

    public static DynamicBuffer<ProjectM.ActiveServantMission> GetBuffer(EntityManager em, Entity mission) {
        return em.GetBuffer<ProjectM.ActiveServantMission>(mission);
    }

    public static List<DynamicBuffer<ProjectM.ActiveServantMission>> GetAllBuffers(EntityManager em) {
        var buffers = new List<DynamicBuffer<ProjectM.ActiveServantMission>>();
        var entities = GetAll(em);
        foreach (var entity in entities) {
            var buffer = em.GetBuffer<ProjectM.ActiveServantMission>(entity);
            buffers.Add(buffer);
        }
        return buffers;
    }

    public static List<string> GetAllBuffersMissionUIDs(EntityManager em) {
        var missionUIDs = new List<string>();
        var missions = GetAllBuffers(em);

        foreach (var missionBuffers in missions) {
            foreach (var buffer in missionBuffers) {
                missionUIDs.Add(GetMissionUID(buffer));
            }
        }
        return missionUIDs;
    }

    public static List<string> GetBufferMissionUIDs(EntityManager em, Entity mission) {
        var missionUIDs = new List<string>();
        var missionBuffers = GetBuffer(em, mission);
        foreach (var buffer in missionBuffers) {
            missionUIDs.Add(GetMissionUID(buffer));
        }
        return missionUIDs;
    }

    public static string GetMissionUID(ProjectM.ActiveServantMission mission) {
        return $"{mission.MissiontDataId}-{GetMissionName(mission)}";
    }

    public static string GetMissionName(ProjectM.ActiveServantMission mission) {
        return PrefabCollectionSystem.GetPrefabName(mission.MissionID);
    }

    // MissionLegth
    public static float GetMissionLength(ProjectM.ActiveServantMission mission) {
        return mission.MissionLength;
    }

    public static long GetMissionLengthTimestamp(ProjectM.ActiveServantMission mission) {
        return DateTimeOffset.Now.AddSeconds(GetMissionLength(mission)).ToUnixTimeMilliseconds();
    }

    public static void SetMissionLength(ref ProjectM.ActiveServantMission mission, float seconds) {
        mission.MissionLength = seconds;
    }
}