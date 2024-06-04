using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ScreenSystem;

namespace Battlefield
{
    public class MySubModule : MBSubModuleBase
    {
        private MainMenuHandler _mainMenuHandler;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            // Additional initialization code if needed

            // InformationManager.DisplayMessage(new InformationMessage("TE loaded"));
            Module.CurrentModule.AddInitialStateOption(new InitialStateOption("Message",
                new TextObject("Message", null),
                9990,
                () => { InformationManager.DisplayMessage(new InformationMessage("Hello World!")); },
                () => { return (false, null); }));

        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            _mainMenuHandler = new MainMenuHandler();
            _mainMenuHandler.Initialize(Game.Current.GameStateManager.CreateState<InitialState>());
        }

        protected override void OnApplicationTick(float dt)
        {
            base.OnApplicationTick(dt);
            // Handle any updates per tick if necessary
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            if (game.GameType is Campaign)
            {
                AddCustomGameMode(gameStarterObject as CampaignGameStarter);
            }
        }

        private void AddCustomGameMode(CampaignGameStarter gameStarter)
        {
            gameStarter.AddBehavior(new CustomBattleGameModeBehavior());
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            if (game.GameType is Campaign)
            {
                var campaign = game.GameType as Campaign;
                campaign.CampaignBehaviorManager.AddBehavior(new CustomBattleGameModeBehavior());
            }
        }
    }

    public class CustomBattleGameModeBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, OnNewGameCreated);
        }

        public override void SyncData(IDataStore dataStore)
        {
            // Sync data if needed
        }

        private void OnNewGameCreated(CampaignGameStarter starter)
        {
            // Initialize your custom game mode
            // Custom game mode initialization logic here
        }
    }

    public class CustomBattleMissionBehavior : MissionBehavior
    {
        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Logic;

        public override void OnMissionTick(float dt)
        {
            // Custom behavior logic here
        }
    }

}