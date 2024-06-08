﻿using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.CustomBattle;
using TaleWorlds.MountAndBlade.Multiplayer;
using TaleWorlds.MountAndBlade;

namespace Battlefield
{
    public class BattlefieldGameManager : CustomGameManager
    {
        protected override void DoLoadingForGameManager(GameManagerLoadingSteps gameManagerLoadingStep, out GameManagerLoadingSteps nextStep)
        {
            nextStep = GameManagerLoadingSteps.None;
            switch (gameManagerLoadingStep)
            {
                case GameManagerLoadingSteps.PreInitializeZerothStep:
                    MBGameManager.LoadModuleData(isLoadGame: false);
                    MBGlobals.InitializeReferences();
                    Game.CreateGame(new BattlefieldCustomGame(), this).DoLoading();
                    nextStep = GameManagerLoadingSteps.FirstInitializeFirstStep;
                    break;
                case GameManagerLoadingSteps.FirstInitializeFirstStep:
                    {
                        bool flag = true;
                        foreach (MBSubModuleBase subModule in Module.CurrentModule.SubModules)
                        {
                            flag = flag && subModule.DoLoading(Game.Current);
                        }

                        nextStep = ((!flag) ? GameManagerLoadingSteps.FirstInitializeFirstStep : GameManagerLoadingSteps.WaitSecondStep);
                        break;
                    }
                case GameManagerLoadingSteps.WaitSecondStep:
                    MBGameManager.StartNewGame();
                    nextStep = GameManagerLoadingSteps.SecondInitializeThirdState;
                    break;
                case GameManagerLoadingSteps.SecondInitializeThirdState:
                    nextStep = (Game.Current.DoLoading() ? GameManagerLoadingSteps.PostInitializeFourthState : GameManagerLoadingSteps.SecondInitializeThirdState);
                    break;
                case GameManagerLoadingSteps.PostInitializeFourthState:
                    nextStep = GameManagerLoadingSteps.FinishLoadingFifthStep;
                    break;
                case GameManagerLoadingSteps.FinishLoadingFifthStep:
                    nextStep = GameManagerLoadingSteps.None;
                    break;
            }
        }

        public override void OnAfterCampaignStart(Game game)
        {
            MultiplayerMain.Initialize(new GameNetworkHandler());
        }

        public override void OnLoadFinished()
        {
            base.OnLoadFinished();
            Game.Current.GameStateManager.CleanAndPushState(Game.Current.GameStateManager.CreateState<CustomBattleState>());
        }
    }
}