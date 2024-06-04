using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.GauntletUI;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.GauntletUI.PrefabSystem;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Localization;
using TaleWorlds.ScreenSystem;

namespace Battlefield
{
    public class MySubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            // Custom initialization code
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
            // Define your custom behavior here
            gameStarter.AddBehavior(new CustomBattleGameModeBehavior());
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            // Add the custom button to the main menu
            AddMainMenuButton();
        }

        private void AddMainMenuButton()
        {
            // Corrected the way to get the main menu screen
            var mainMenuScreen = UIResourceManager.UIResourceDepot.GetData<ScreenBase>("MainMenuScreen");
            if (mainMenuScreen != null)
            {
                var mainMenuLayer = mainMenuScreen.GetLayer(0);
                if (mainMenuLayer != null)
                {
                    // Corrected the way to create a new button widget
                    var buttonWidget = new ButtonWidget(UIResourceManager.ResourceContext);
                    buttonWidget.TextWidget = new TextWidget(UIResourceManager.ResourceContext)
                    {
                        Brush = UIResourceManager.BrushFactory.GetBrush("DefaultButtonBrush"),
                        Text = new TextObject("Custom Game Mode").ToString()
                    };
                    buttonWidget.Click += OnCustomGameModeButtonClick;

                    var layout = mainMenuLayer.FindChild("MainMenuLayout") as Widget;
                    if (layout != null)
                    {
                        layout.AddChild(buttonWidget);
                    }
                }
            }
        }

        private void OnCustomGameModeButtonClick(Widget widget)
        {
            // Code to execute when the button is clicked
            StartCustomBattle();
        }

        private void StartCustomBattle()
        {
            MissionState.OpenNew("CustomBattleScene", new MissionInitializerRecord(null), mission =>
            {
                mission.AddMissionBehavior(new CustomBattleMissionBehavior());
                return null;  // Ensure the lambda returns null
            });
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
