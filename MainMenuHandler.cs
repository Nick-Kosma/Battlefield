using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.InitialMenu;
using TaleWorlds.ScreenSystem;

namespace Battlefield
{
    public class MainMenuHandler
    {
        private GauntletLayer _gauntletLayer;
        private InitialMenuVM _dataSource;

        public void Initialize(InitialState initialState)
        {
            _dataSource = new InitialMenuVM(initialState);
            _gauntletLayer = new GauntletLayer(1, "GauntletLayer");

            _gauntletLayer.LoadMovie("InitialScreen", _dataSource);
            _gauntletLayer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.Mouse);

            ScreenManager.TopScreen.AddLayer(_gauntletLayer);

            AddCustomButton();
        }

        private void AddCustomButton()
        {
            var buttonWidget = new ButtonWidget(_gauntletLayer.UIContext)
            {
                Brush = _gauntletLayer.UIContext.BrushFactory.GetBrush("DefaultButtonBrush")
            };

            var textWidget = new RichTextWidget(_gauntletLayer.UIContext)
            {
                Text = new TextObject("Custom Game Mode").ToString()
            };

            buttonWidget.AddChild(textWidget);
            buttonWidget.ClickEventHandlers.Add(OnCustomGameModeButtonClick);

            var mainMenuLayout = _gauntletLayer.UIContext.Root.FindChild("MainMenuLayout") as Widget;
            mainMenuLayout?.AddChild(buttonWidget);
        }

        private void OnCustomGameModeButtonClick(Widget widget)
        {
            StartCustomBattle();
        }

        private void StartCustomBattle()
        {
            MissionState.OpenNew("CustomBattleScene", new MissionInitializerRecord(null), mission =>
            {
                mission.AddMissionBehavior(new CustomBattleMissionBehavior());
                return null;
            });
        }
    }

}