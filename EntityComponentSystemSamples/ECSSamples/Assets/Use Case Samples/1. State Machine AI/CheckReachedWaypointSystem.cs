using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class CheckedReachedWaypointSystem : SystemBase
{
    // Cache a reference to this system in OnCreate() to prevent World.GetExistingSystem being called every frame
    private EndSimulationEntityCommandBufferSystem m_EndSimECBSystem;

    protected override void OnCreate()
    {
        m_EndSimECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        // Create an ECB to record modification to entities that will only be executed at the end of the simulation group. We will use it below to change our state to "Idle" when we reach our waypoint.
        var ecb = m_EndSimECBSystem.CreateCommandBuffer().AsParallelWriter();

        var waypointHandle = Entities
            .WithName("CheckReachedWaypoint") // ForEach name is helpful for debugging
            .WithNone<IsChasingTag, IdleTimer>() // WithNone means "Exclude all entities with IsChasingTag from this ForEach"
                                      // This avoids running the waypoint checking code on guards that are chasing the player
            .ForEach((
                Entity e, // Refers to the current guard entity. Used by the ECB when changing states
                int entityInQueryIndex, // Index of the guard entity in the query. Used for Concurrent ECB writing
                in Translation currentPosition, // "in" keyword makes this parameter ReadOnly
                in TargetPosition targetPosition) =>
                {
                    // Determine if we are within the StopDistance of our target waypoint.
                    var distanceSq = math.lengthsq(targetPosition.Value - currentPosition.Value);
                    if (distanceSq < GuardAIUtility.kStopDistanceSq)
                    {
                        // If we are, transition to idle and remove the target position
                        GuardAIUtility.TransitionFromPatrolling(ecb, e, entityInQueryIndex);
                        GuardAIUtility.TransitionToIdle(ecb, e, entityInQueryIndex);
                    }
                }).ScheduleParallel(Dependency); // Schedule the ForEach with the job system to run

        // EntityCommandBufferSystems need to know about all jobs which write to EntityCommandBuffers it has created
        m_EndSimECBSystem.AddJobHandleForProducer(waypointHandle);

        // Pass the handle generated by the ForEach to the next system
        Dependency = waypointHandle;
    }
}
