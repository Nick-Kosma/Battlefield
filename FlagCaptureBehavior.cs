using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade.Objects;

namespace Battlefield
{
    public class SinglePlayerFlagCaptureMode : CampaignBehaviorBase
    {
        private const int TotalFlags = 5;
        private const float GameDuration = 1200f; // 20 minutes in seconds

        private float _gameStartTime;
        private int _capturedFlags;
        private List<FlagCapturePoint> _capturePoints;

        public override void RegisterEvents()
        {
            CampaignEvents.OnMissionStartedEvent.AddNonSerializedListener(this, OnMissionStarted);
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("_capturedFlags", ref _capturedFlags);
        }

        private void OnMissionStarted(IMission mission)
        {
            if (Mission.Current.SceneName == "CustomBattleScene")
            {
                InitializeGameMode();
                Mission.Current.AddMissionBehavior(new FlagMissionLogic(this));
            }
        }

        private void InitializeGameMode()
        {
            _gameStartTime = Mission.Current.CurrentTime;
            _capturePoints = Mission.Current.MissionObjects.FindAllWithType<FlagCapturePoint>().ToList();
            foreach (var flagCapturePoint in _capturePoints)
            {
                flagCapturePoint.SetTeamColorsWithAllSynched(0xFFFFFFFF, 0xFFFFFFFF); // Set to neutral colors initially
            }
        }

        public void CaptureFlag(FlagCapturePoint flag)
        {
            _capturedFlags++;
            flag.GameEntity.SetVisibilityExcludeParents(false); // Hide the flag
            InformationManager.DisplayMessage(new InformationMessage($"Flag {_capturedFlags} captured by player!"));

            if (_capturedFlags == TotalFlags)
            {
                EndGame(true);
            }
        }

        public void CheckGameOver()
        {
            if (Mission.Current.CurrentTime - _gameStartTime > GameDuration)
            {
                EndGame(false);
            }
        }

        private void EndGame(bool playerWon)
        {
            if (playerWon)
            {
                InformationManager.DisplayMessage(new InformationMessage("Player wins!"));
            }
            else
            {
                InformationManager.DisplayMessage(new InformationMessage("Time's up! Player loses!"));
            }
            // End the mission or transition to another state
        }

        public class FlagMissionLogic : MissionLogic
        {
            private readonly SinglePlayerFlagCaptureMode _flagCaptureMode;

            public FlagMissionLogic(SinglePlayerFlagCaptureMode flagCaptureMode)
            {
                _flagCaptureMode = flagCaptureMode;
            }

            public override void OnMissionTick(float dt)
            {
                base.OnMissionTick(dt);
                _flagCaptureMode.CheckGameOver();

                foreach (var entity in Mission.Current.Scene.FindEntitiesWithTag("flag"))
                {
                    var flagHolder = entity.CollectChildrenEntitiesWithTag("score_stand").SingleOrDefault()?.GetFirstScriptOfType<SynchedMissionObject>();
                    if (flagHolder != null)
                    {
                        var flag = flagHolder.GameEntity.GetChildren().SingleOrDefault(e => e.HasTag("flag"));
                        if (flag != null && IsFlagCapturedByPlayer(flag))
                        {
                            _flagCaptureMode.CaptureFlag(flagHolder as FlagCapturePoint);
                        }
                    }
                }
            }

            private bool IsFlagCapturedByPlayer(GameEntity flag)
            {
                // Check if the flag's color matches the player's team color
                var flagCapturePoint = flag.GetFirstScriptOfType<FlagCapturePoint>();
                return flagCapturePoint != null && IsFlagOwnedByPlayer(flagCapturePoint);
            }

            private bool IsFlagOwnedByPlayer(FlagCapturePoint flagCapturePoint)
            {
                // Implement logic to determine if the flag is owned by the player
                // For example, check if the flag's team color matches the player's team color
                // Placeholder for actual check:
                uint playerColor = Mission.Current.PlayerTeam.Color;
                uint flagColor = flagCapturePoint.GameEntity.GetFirstScriptOfType<SynchedMissionObject>().Color;
                return playerColor == flagColor;
            }
        }
    }
}
