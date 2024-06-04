using System;
using TaleWorlds.Engine;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.CustomBattle;
using TaleWorlds.MountAndBlade.GauntletUI.SceneNotification;

namespace Battlefield
{
    // Token: 0x0200000D RID: 13
    public class BattlefieldSubModule : CustomBattleSubModule
    {
        // Token: 0x060000B5 RID: 181 RVA: 0x000074C4 File Offset: 0x000056C4
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            Module.CurrentModule.AddInitialStateOption(new InitialStateOption("CustomBattle", new TextObject("{=4gOGGbeQ}Battlefield", null), 5000, delegate ()
            {
                MBGameManager.StartNewGame(new CustomGameManager());
            }, () => new ValueTuple<bool, TextObject>(false, null), null));
        }

        // Token: 0x060000B6 RID: 182 RVA: 0x0000753A File Offset: 0x0000573A
        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            if (!this._initialized)
            {
                if (!Utilities.CommandLineArgumentExists("VisualTests"))
                {
                    GauntletSceneNotification.Current.RegisterContextProvider(new CustomBattleSceneNotificationContextProvider());
                }
                this._initialized = true;
            }
        }

        // Token: 0x0400006D RID: 109
        private bool _initialized;
    }
}
