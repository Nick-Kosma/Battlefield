using System;
using TaleWorlds.Engine;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.CustomBattle;

namespace Battlefield
{
    public class BattlefieldSubModule : CustomBattleSubModule
    {
        protected override void OnSubModuleLoad()
        {
            Module.CurrentModule.AddInitialStateOption(new InitialStateOption("Battlefield", new TextObject("{=4gOGGbeQ}Battlefield"), 5000, () =>
            {
                MBGameManager.StartNewGame(new BattlefieldGameManager());
            }, () => new ValueTuple<bool, TextObject>(false, null), null));

            // Apply Harmony patches
            HarmonyPatcher.ApplyPatches();
        }

        public CustomGameManager GameManager { get; set; }
    }
}
