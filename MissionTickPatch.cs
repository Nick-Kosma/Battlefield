using HarmonyLib;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Core;
using System.Linq;

namespace Battlefield
{
    [HarmonyPatch(typeof(Mission), "Tick")]
    public class MissionTickPatch
    {
        private static readonly float attackerRespawnInterval = 30f;
        private static readonly float defenderRespawnInterval = 60f;
        private static float attackerRespawnTimer = attackerRespawnInterval;
        private static float defenderRespawnTimer = defenderRespawnInterval;
        private static float previousTime = 0f;

        static void Postfix(Mission __instance)
        {
            if (__instance.Mode == MissionMode.Battle)
            {
                // Calculate delta time manually
                float currentTime = __instance.CurrentTime;
                float deltaTime = currentTime - previousTime;
                previousTime = currentTime;

                // Update the respawn timers
                attackerRespawnTimer -= deltaTime;
                defenderRespawnTimer -= deltaTime;

                // Custom flag objective logic here (to be implemented)

                // Example respawn logic
                if (attackerRespawnTimer <= 0)
                {
                    RespawnUnits(__instance, BattleSideEnum.Attacker);
                    attackerRespawnTimer = attackerRespawnInterval; // Reset the timer
                }

                if (defenderRespawnTimer <= 0)
                {
                    RespawnUnits(__instance, BattleSideEnum.Defender);
                    defenderRespawnTimer = defenderRespawnInterval; // Reset the timer
                }
            }
        }

        private static void RespawnUnits(Mission mission, BattleSideEnum side)
        {
            foreach (Team team in mission.Teams)
            {
                if (team.Side == side)
                {
                    foreach (Formation formation in team.FormationsIncludingEmpty)
                    {
                        var deadAgents = mission.Agents.Where(agent => agent.Team == team && agent.State == AgentState.Killed && agent.Formation == formation).ToList();
                        foreach (Agent agent in deadAgents)
                        {
                            // Respawn logic for dead units
                            AgentBuildData buildData = new AgentBuildData(agent.Character).Team(team);
                            Agent newAgent = mission.SpawnAgent(buildData);
                            formation.AddUnit(newAgent);
                        }
                    }
                }
            }
        }
    }

    public static class HarmonyPatcher
    {
        public static void ApplyPatches()
        {
            var harmony = new Harmony("com.yourname.bannerlord.battlefield");
            harmony.PatchAll();
        }
    }
}
