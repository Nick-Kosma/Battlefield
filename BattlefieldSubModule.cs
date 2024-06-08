using System;
using TaleWorlds.Engine;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.CustomBattle;

namespace Battlefield
{
    // Token: 0x0200000D RID: 13
    public class BattlefieldSubModule : CustomBattleSubModule
    {
        // Token: 0x060000B5 RID: 181 RVA: 0x000074C4 File Offset: 0x000056C4
        protected override void OnSubModuleLoad()
        {
            Module.CurrentModule.AddInitialStateOption(new InitialStateOption("Battlefield", new TextObject("{=4gOGGbeQ}Battlefield", null), 5000, delegate ()
            {
                MBGameManager.StartNewGame(new BattlefieldGameManager());
            }, () => new ValueTuple<bool, TextObject>(false, null), null));
        }

        public CustomGameManager GameManager { get; set; }
    }

    
}
