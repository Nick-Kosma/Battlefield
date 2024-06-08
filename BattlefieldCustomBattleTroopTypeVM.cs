using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Battlefield
{
    // Token: 0x0200000E RID: 14
    public class BattlefieldCustomBattleTroopTypeVM : ViewModel
    {
        // Token: 0x1700002C RID: 44
        // (get) Token: 0x060000B8 RID: 184 RVA: 0x00007574 File Offset: 0x00005774
        // (set) Token: 0x060000B9 RID: 185 RVA: 0x0000757C File Offset: 0x0000577C
        public BasicCharacterObject Character { get; private set; }

        // Token: 0x060000BA RID: 186 RVA: 0x00007588 File Offset: 0x00005788
        public BattlefieldCustomBattleTroopTypeVM(BasicCharacterObject character, Action<BattlefieldCustomBattleTroopTypeVM> onSelectionToggled, StringItemWithHintVM typeIconData, MBReadOnlyList<SkillObject> allSkills, bool isDefault)
        {
            this.Character = character;
            this.IsDefault = isDefault;
            this._onSelectionToggled = onSelectionToggled;
            this._allSkills = allSkills;
            if (character != null)
            {
                this.Visual = new ImageIdentifierVM(CharacterCode.CreateFrom(character));
                this.NameHint = new HintViewModel(character.Name, null);
                this.TroopSkillsHint = new BasicTooltipViewModel(() => this.GetTroopSkillsTooltip(this.Character));
                this.TierIconData = BattlefieldCustomBattleTroopTypeVM.GetCharacterTierData(this.Character, false);
                this.TypeIconData = typeIconData;
            }
            else
            {
                Debug.FailedAssert("Character shouldn't be null", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.CustomBattle\\CustomBattle\\CustomBattleTroopTypeVM.cs", ".ctor", 39);
            }
            this.RefreshValues();
        }

        // Token: 0x060000BB RID: 187 RVA: 0x0000762D File Offset: 0x0000582D
        public override void RefreshValues()
        {
            base.RefreshValues();
            BasicCharacterObject character = this.Character;
            this.Name = ((character != null) ? character.Name.ToString() : null);
        }

        // Token: 0x060000BC RID: 188 RVA: 0x00007652 File Offset: 0x00005852
        public void ExecuteToggleSelection()
        {
            Action<BattlefieldCustomBattleTroopTypeVM> onSelectionToggled = this._onSelectionToggled;
            if (onSelectionToggled == null)
            {
                return;
            }
            onSelectionToggled(this);
        }

        // Token: 0x060000BD RID: 189 RVA: 0x00007665 File Offset: 0x00005865
        public void ExecuteRandomize()
        {
            this.IsSelected = (MBRandom.RandomInt(2) == 1);
        }

        // Token: 0x060000BE RID: 190 RVA: 0x00007678 File Offset: 0x00005878
        private List<TooltipProperty> GetTroopSkillsTooltip(BasicCharacterObject character)
        {
            List<TooltipProperty> list = new List<TooltipProperty>();
            list.Add(new TooltipProperty("", character.Name.ToString(), 1, false, 4096));
            list.Add(new TooltipProperty(GameTexts.FindText("str_skills", null).ToString(), " ", 0, false, 0));
            list.Add(new TooltipProperty("", "", 0, false, 512));
            foreach (SkillObject skillObject in this._allSkills)
            {
                int skillValue = character.GetSkillValue(skillObject);
                if (skillValue > 0)
                {
                    list.Add(new TooltipProperty(skillObject.Name.ToString(), skillValue.ToString(), 0, false, 0));
                }
            }
            return list;
        }

        // Token: 0x060000BF RID: 191 RVA: 0x00007758 File Offset: 0x00005958
        public static StringItemWithHintVM GetCharacterTierData(BasicCharacterObject character, bool isBig = false)
        {
            int characterTier = BattlefieldCustomBattleTroopTypeVM.GetCharacterTier(character);
            if (characterTier <= 0 || characterTier > 7)
            {
                return new StringItemWithHintVM("", TextObject.Empty);
            }
            string str = isBig ? (characterTier.ToString() + "_big") : characterTier.ToString();
            string text = "General\\TroopTierIcons\\icon_tier_" + str;
            GameTexts.SetVariable("TIER_LEVEL", characterTier);
            TextObject textObject = new TextObject("{=!}" + GameTexts.FindText("str_party_troop_tier", null).ToString(), null);
            return new StringItemWithHintVM(text, textObject);
        }

        // Token: 0x060000C0 RID: 192 RVA: 0x000077DF File Offset: 0x000059DF
        public static int GetCharacterTier(BasicCharacterObject character)
        {
            if (character.IsHero)
            {
                return 0;
            }
            return MathF.Min(MathF.Max(MathF.Ceiling(((float)character.Level - 5f) / 5f), 0), 7);
        }

        // Token: 0x1700002D RID: 45
        // (get) Token: 0x060000C1 RID: 193 RVA: 0x0000780F File Offset: 0x00005A0F
        // (set) Token: 0x060000C2 RID: 194 RVA: 0x00007817 File Offset: 0x00005A17
        [DataSourceProperty]
        public ImageIdentifierVM Visual
        {
            get
            {
                return this._visual;
            }
            set
            {
                if (value != this._visual)
                {
                    this._visual = value;
                    base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Visual");
                }
            }
        }

        // Token: 0x1700002E RID: 46
        // (get) Token: 0x060000C3 RID: 195 RVA: 0x00007835 File Offset: 0x00005A35
        // (set) Token: 0x060000C4 RID: 196 RVA: 0x0000783D File Offset: 0x00005A3D
        [DataSourceProperty]
        public BasicTooltipViewModel TroopSkillsHint
        {
            get
            {
                return this._troopSkillsHint;
            }
            set
            {
                if (value != this._troopSkillsHint)
                {
                    this._troopSkillsHint = value;
                    base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "TroopSkillsHint");
                }
            }
        }

        // Token: 0x1700002F RID: 47
        // (get) Token: 0x060000C5 RID: 197 RVA: 0x0000785B File Offset: 0x00005A5B
        // (set) Token: 0x060000C6 RID: 198 RVA: 0x00007863 File Offset: 0x00005A63
        [DataSourceProperty]
        public HintViewModel NameHint
        {
            get
            {
                return this._nameHint;
            }
            set
            {
                if (value != this._nameHint)
                {
                    this._nameHint = value;
                    base.OnPropertyChangedWithValue<HintViewModel>(value, "NameHint");
                }
            }
        }

        // Token: 0x17000030 RID: 48
        // (get) Token: 0x060000C7 RID: 199 RVA: 0x00007881 File Offset: 0x00005A81
        // (set) Token: 0x060000C8 RID: 200 RVA: 0x00007889 File Offset: 0x00005A89
        [DataSourceProperty]
        public StringItemWithHintVM TierIconData
        {
            get
            {
                return this._tierIconData;
            }
            set
            {
                if (value != this._tierIconData)
                {
                    this._tierIconData = value;
                    base.OnPropertyChangedWithValue<StringItemWithHintVM>(value, "TierIconData");
                }
            }
        }

        // Token: 0x17000031 RID: 49
        // (get) Token: 0x060000C9 RID: 201 RVA: 0x000078A7 File Offset: 0x00005AA7
        // (set) Token: 0x060000CA RID: 202 RVA: 0x000078AF File Offset: 0x00005AAF
        [DataSourceProperty]
        public StringItemWithHintVM TypeIconData
        {
            get
            {
                return this._typeIconData;
            }
            set
            {
                if (value != this._typeIconData)
                {
                    this._typeIconData = value;
                    base.OnPropertyChangedWithValue<StringItemWithHintVM>(value, "TypeIconData");
                }
            }
        }

        // Token: 0x17000032 RID: 50
        // (get) Token: 0x060000CB RID: 203 RVA: 0x000078CD File Offset: 0x00005ACD
        // (set) Token: 0x060000CC RID: 204 RVA: 0x000078D5 File Offset: 0x00005AD5
        [DataSourceProperty]
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                if (value != this._name)
                {
                    this._name = value;
                    base.OnPropertyChangedWithValue<string>(value, "Name");
                }
            }
        }

        // Token: 0x17000033 RID: 51
        // (get) Token: 0x060000CD RID: 205 RVA: 0x000078F8 File Offset: 0x00005AF8
        // (set) Token: 0x060000CE RID: 206 RVA: 0x00007900 File Offset: 0x00005B00
        [DataSourceProperty]
        public bool IsSelected
        {
            get
            {
                return this._isSelected;
            }
            set
            {
                if (value != this._isSelected)
                {
                    this._isSelected = value;
                    base.OnPropertyChangedWithValue(value, "IsSelected");
                }
            }
        }

        // Token: 0x0400006F RID: 111
        public bool IsDefault;

        // Token: 0x04000070 RID: 112
        private readonly Action<BattlefieldCustomBattleTroopTypeVM> _onSelectionToggled;

        // Token: 0x04000071 RID: 113
        private readonly MBReadOnlyList<SkillObject> _allSkills;

        // Token: 0x04000072 RID: 114
        private ImageIdentifierVM _visual;

        // Token: 0x04000073 RID: 115
        private BasicTooltipViewModel _troopSkillsHint;

        // Token: 0x04000074 RID: 116
        private HintViewModel _nameHint;

        // Token: 0x04000075 RID: 117
        private StringItemWithHintVM _tierIconData;

        // Token: 0x04000076 RID: 118
        private StringItemWithHintVM _typeIconData;

        // Token: 0x04000077 RID: 119
        private string _name;

        // Token: 0x04000078 RID: 120
        private bool _isSelected;
    }
}
