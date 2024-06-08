using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.CustomBattle.CustomBattle;
using TaleWorlds.MountAndBlade;



namespace Battlefield
{
    // Token: 0x02000007 RID: 7
    public class BattlefieldArmyCompositionGroupVM : ViewModel
    {
        // Token: 0x0600001E RID: 30 RVA: 0x00004CA4 File Offset: 0x00002EA4
        public BattlefieldArmyCompositionGroupVM(bool isPlayerSide, TroopTypeSelectionPopUpVM troopTypeSelectionPopUp)
        {
            this._isPlayerSide = isPlayerSide;
            this.MinArmySize = 1;
            this.MaxArmySize = BannerlordConfig.MaxBattleSize;
            foreach (BasicCharacterObject item in from c in Game.Current.ObjectManager.GetObjectTypeList<BasicCharacterObject>()
                                                  where c.IsSoldier && !c.IsObsolete
                                                  select c)
            {
                this._allCharacterObjects.Add(item);
            }
            this.CompositionValues = new int[4];
            this.CompositionValues[0] = 25;
            this.CompositionValues[1] = 25;
            this.CompositionValues[2] = 25;
            this.CompositionValues[3] = 25;
            this.MeleeInfantryComposition = new BattlefieldArmyCompositionItemVM(BattlefieldArmyCompositionItemVM.CompositionType.MeleeInfantry, this._allCharacterObjects, this._allSkills, new Action<int, int>(this.UpdateSliders), troopTypeSelectionPopUp, this.CompositionValues);
            this.RangedInfantryComposition = new BattlefieldArmyCompositionItemVM(BattlefieldArmyCompositionItemVM.CompositionType.RangedInfantry, this._allCharacterObjects, this._allSkills, new Action<int, int>(this.UpdateSliders), troopTypeSelectionPopUp, this.CompositionValues);
            this.MeleeCavalryComposition = new BattlefieldArmyCompositionItemVM(BattlefieldArmyCompositionItemVM.CompositionType.MeleeCavalry, this._allCharacterObjects, this._allSkills, new Action<int, int>(this.UpdateSliders), troopTypeSelectionPopUp, this.CompositionValues);
            this.RangedCavalryComposition = new BattlefieldArmyCompositionItemVM(BattlefieldArmyCompositionItemVM.CompositionType.RangedCavalry, this._allCharacterObjects, this._allSkills, new Action<int, int>(this.UpdateSliders), troopTypeSelectionPopUp, this.CompositionValues);
            this.ArmySize = BannerlordConfig.GetRealBattleSize() / 5;
            this.RefreshValues();
        }

        // Token: 0x0600001F RID: 31 RVA: 0x00004E58 File Offset: 0x00003058
        public override void RefreshValues()
        {
            base.RefreshValues();
            this.ArmySizeTitle = GameTexts.FindText("str_army_size", null).ToString();
            this.MeleeInfantryComposition.RefreshValues();
            this.RangedInfantryComposition.RefreshValues();
            this.MeleeCavalryComposition.RefreshValues();
            this.RangedCavalryComposition.RefreshValues();
        }

        // Token: 0x06000020 RID: 32 RVA: 0x00004EB0 File Offset: 0x000030B0
        private static int SumOfValues(int[] array, bool[] enabledArray, int excludedIndex = -1)
        {
            int num = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (enabledArray[i] && excludedIndex != i)
                {
                    num += array[i];
                }
            }
            return num;
        }

        // Token: 0x06000021 RID: 33 RVA: 0x00004EE0 File Offset: 0x000030E0
        public void SetCurrentSelectedCulture(BasicCultureObject selectedCulture)
        {
            if (this._selectedCulture != selectedCulture)
            {
                this.MeleeInfantryComposition.SetCurrentSelectedCulture(selectedCulture);
                this.RangedInfantryComposition.SetCurrentSelectedCulture(selectedCulture);
                this.MeleeCavalryComposition.SetCurrentSelectedCulture(selectedCulture);
                this.RangedCavalryComposition.SetCurrentSelectedCulture(selectedCulture);
                this._selectedCulture = selectedCulture;
            }
        }

        // Token: 0x06000022 RID: 34 RVA: 0x00004F30 File Offset: 0x00003130
        private void UpdateSliders(int value, int changedSliderIndex)
        {
            if (this._updatingSliders)
            {
                return;
            }
            this._updatingSliders = true;
            bool[] array = new bool[]
            {
                !this.MeleeInfantryComposition.IsLocked,
                !this.RangedInfantryComposition.IsLocked,
                !this.MeleeCavalryComposition.IsLocked,
                !this.RangedCavalryComposition.IsLocked
            };
            int[] array2 = new int[]
            {
                this.CompositionValues[0],
                this.CompositionValues[1],
                this.CompositionValues[2],
                this.CompositionValues[3]
            };
            int[] array3 = new int[]
            {
                this.CompositionValues[0],
                this.CompositionValues[1],
                this.CompositionValues[2],
                this.CompositionValues[3]
            };
            int num = array.Count((bool s) => s);
            if (array[changedSliderIndex])
            {
                num--;
            }
            if (num > 0)
            {
                int num2 = BattlefieldArmyCompositionGroupVM.SumOfValues(array2, array, -1);
                if (value >= num2)
                {
                    value = num2;
                }
                int num3 = value - array2[changedSliderIndex];
                if (num3 != 0)
                {
                    int num4 = BattlefieldArmyCompositionGroupVM.SumOfValues(array2, array, changedSliderIndex);
                    int num5 = num4 - num3;
                    if (num5 > 0)
                    {
                        int num6 = 0;
                        array3[changedSliderIndex] = value;
                        for (int i = 0; i < array.Length; i++)
                        {
                            if (changedSliderIndex != i && array[i] && array2[i] != 0)
                            {
                                int num7 = MathF.Round((float)array2[i] / (float)num4 * (float)num5);
                                num6 += num7;
                                array3[i] = num7;
                            }
                        }
                        int num8 = num5 - num6;
                        if (num8 != 0)
                        {
                            int num9 = 0;
                            for (int j = 0; j < array.Length; j++)
                            {
                                if (array[j] && j != changedSliderIndex && 0 < array2[j] + num8 && 100 > array2[j] + num8)
                                {
                                    num9++;
                                }
                            }
                            for (int k = 0; k < array.Length; k++)
                            {
                                if (array[k] && k != changedSliderIndex && 0 < array2[k] + num8 && 100 > array2[k] + num8)
                                {
                                    int num10 = MathF.Round((float)num8 / (float)num9);
                                    array3[k] += num10;
                                    num8 -= num10;
                                }
                            }
                            if (num8 != 0)
                            {
                                for (int l = 0; l < array.Length; l++)
                                {
                                    if (array[l] && l != changedSliderIndex && 0 <= array2[l] + num8 && 100 >= array2[l] + num8)
                                    {
                                        array3[l] += num8;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        array3[changedSliderIndex] = value;
                        for (int m = 0; m < array.Length; m++)
                        {
                            if (changedSliderIndex != m && array[m])
                            {
                                array3[m] = 0;
                            }
                        }
                    }
                }
            }
            this.SetArmyCompositionValue(0, array3[0], this.MeleeInfantryComposition);
            this.SetArmyCompositionValue(1, array3[1], this.RangedInfantryComposition);
            this.SetArmyCompositionValue(2, array3[2], this.MeleeCavalryComposition);
            this.SetArmyCompositionValue(3, array3[3], this.RangedCavalryComposition);
            this._updatingSliders = false;
        }

        // Token: 0x06000023 RID: 35 RVA: 0x0000520C File Offset: 0x0000340C
        private void SetArmyCompositionValue(int index, int value, BattlefieldArmyCompositionItemVM composition)
        {
            this.CompositionValues[index] = value;
            composition.RefreshCompositionValue();
        }

        // Token: 0x06000024 RID: 36 RVA: 0x00005220 File Offset: 0x00003420
        public void ExecuteRandomize()
        {
            this.ArmySize = MBRandom.RandomInt(this.MaxArmySize);
            int num = MBRandom.RandomInt(100);
            int num2 = MBRandom.RandomInt(100);
            int num3 = MBRandom.RandomInt(100);
            int num4 = MBRandom.RandomInt(100);
            int num5 = num + num2 + num3 + num4;
            int num6 = MathF.Round(100f * ((float)num / (float)num5));
            int num7 = MathF.Round(100f * ((float)num2 / (float)num5));
            int num8 = MathF.Round(100f * ((float)num3 / (float)num5));
            int compositionValue = 100 - (num6 + num7 + num8);
            this.MeleeInfantryComposition.ExecuteRandomize(num6);
            this.RangedInfantryComposition.ExecuteRandomize(num7);
            this.MeleeCavalryComposition.ExecuteRandomize(num8);
            this.RangedCavalryComposition.ExecuteRandomize(compositionValue);
        }

        // Token: 0x06000025 RID: 37 RVA: 0x000052E4 File Offset: 0x000034E4
        public void OnPlayerTypeChange(CustomBattlePlayerType playerType)
        {
            bool flag = this.ArmySize == this.MinArmySize;
            this.MinArmySize = ((playerType == CustomBattlePlayerType.Commander) ? 1 : 2);
            this.ArmySize = (flag ? this.MinArmySize : this._armySize);
        }

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x06000026 RID: 38 RVA: 0x00005324 File Offset: 0x00003524
        // (set) Token: 0x06000027 RID: 39 RVA: 0x0000532C File Offset: 0x0000352C
        [DataSourceProperty]
        public BattlefieldArmyCompositionItemVM MeleeInfantryComposition
        {
            get
            {
                return this._meleeInfantryComposition;
            }
            set
            {
                if (value != this._meleeInfantryComposition)
                {
                    this._meleeInfantryComposition = value;
                    base.OnPropertyChangedWithValue<BattlefieldArmyCompositionItemVM>(value, "MeleeInfantryComposition");
                }
            }
        }

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x06000028 RID: 40 RVA: 0x0000534A File Offset: 0x0000354A
        // (set) Token: 0x06000029 RID: 41 RVA: 0x00005352 File Offset: 0x00003552
        [DataSourceProperty]
        public BattlefieldArmyCompositionItemVM RangedInfantryComposition
        {
            get
            {
                return this._rangedInfantryComposition;
            }
            set
            {
                if (value != this._rangedInfantryComposition)
                {
                    this._rangedInfantryComposition = value;
                    base.OnPropertyChangedWithValue<BattlefieldArmyCompositionItemVM>(value, "RangedInfantryComposition");
                }
            }
        }

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x0600002A RID: 42 RVA: 0x00005370 File Offset: 0x00003570
        // (set) Token: 0x0600002B RID: 43 RVA: 0x00005378 File Offset: 0x00003578
        [DataSourceProperty]
        public BattlefieldArmyCompositionItemVM MeleeCavalryComposition
        {
            get
            {
                return this._meleeCavalryComposition;
            }
            set
            {
                if (value != this._meleeCavalryComposition)
                {
                    this._meleeCavalryComposition = value;
                    base.OnPropertyChangedWithValue<BattlefieldArmyCompositionItemVM>(value, "MeleeCavalryComposition");
                }
            }
        }

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x0600002C RID: 44 RVA: 0x00005396 File Offset: 0x00003596
        // (set) Token: 0x0600002D RID: 45 RVA: 0x0000539E File Offset: 0x0000359E
        [DataSourceProperty]
        public BattlefieldArmyCompositionItemVM RangedCavalryComposition
        {
            get
            {
                return this._rangedCavalryComposition;
            }
            set
            {
                if (value != this._rangedCavalryComposition)
                {
                    this._rangedCavalryComposition = value;
                    base.OnPropertyChangedWithValue<BattlefieldArmyCompositionItemVM>(value, "RangedCavalryComposition");
                }
            }
        }

        // Token: 0x17000006 RID: 6
        // (get) Token: 0x0600002E RID: 46 RVA: 0x000053BC File Offset: 0x000035BC
        // (set) Token: 0x0600002F RID: 47 RVA: 0x000053C4 File Offset: 0x000035C4
        [DataSourceProperty]
        public string ArmySizeTitle
        {
            get
            {
                return this._armySizeTitle;
            }
            set
            {
                if (value != this._armySizeTitle)
                {
                    this._armySizeTitle = value;
                    base.OnPropertyChangedWithValue<string>(value, "ArmySizeTitle");
                }
            }
        }

        // Token: 0x17000007 RID: 7
        // (get) Token: 0x06000030 RID: 48 RVA: 0x000053E7 File Offset: 0x000035E7
        // (set) Token: 0x06000031 RID: 49 RVA: 0x000053F0 File Offset: 0x000035F0
        public int ArmySize
        {
            get
            {
                return this._armySize;
            }
            set
            {
                if (this._armySize != (int)MathF.Clamp((float)value, (float)this.MinArmySize, (float)this.MaxArmySize))
                {
                    this._armySize = (int)MathF.Clamp((float)value, (float)this.MinArmySize, (float)this.MaxArmySize);
                    base.OnPropertyChangedWithValue(this._armySize, "ArmySize");
                }
            }
        }

        // Token: 0x17000008 RID: 8
        // (get) Token: 0x06000032 RID: 50 RVA: 0x00005448 File Offset: 0x00003648
        // (set) Token: 0x06000033 RID: 51 RVA: 0x00005450 File Offset: 0x00003650
        public int MaxArmySize
        {
            get
            {
                return this._maxArmySize;
            }
            set
            {
                if (this._maxArmySize != value)
                {
                    this._maxArmySize = value;
                    base.OnPropertyChangedWithValue(value, "MaxArmySize");
                }
            }
        }

        // Token: 0x17000009 RID: 9
        // (get) Token: 0x06000034 RID: 52 RVA: 0x0000546E File Offset: 0x0000366E
        // (set) Token: 0x06000035 RID: 53 RVA: 0x00005476 File Offset: 0x00003676
        public int MinArmySize
        {
            get
            {
                return this._minArmySize;
            }
            set
            {
                if (this._minArmySize != value)
                {
                    this._minArmySize = value;
                    base.OnPropertyChangedWithValue(value, "MinArmySize");
                }
            }
        }

        // Token: 0x0400002C RID: 44
        public int[] CompositionValues;

        // Token: 0x0400002D RID: 45
        private readonly bool _isPlayerSide;

        // Token: 0x0400002E RID: 46
        private bool _updatingSliders;

        // Token: 0x0400002F RID: 47
        private BasicCultureObject _selectedCulture;

        // Token: 0x04000030 RID: 48
        private readonly MBReadOnlyList<SkillObject> _allSkills = Game.Current.ObjectManager.GetObjectTypeList<SkillObject>();

        // Token: 0x04000031 RID: 49
        private readonly List<BasicCharacterObject> _allCharacterObjects = new List<BasicCharacterObject>();

        // Token: 0x04000032 RID: 50
        private BattlefieldArmyCompositionItemVM _meleeInfantryComposition;

        // Token: 0x04000033 RID: 51
        private BattlefieldArmyCompositionItemVM _rangedInfantryComposition;

        // Token: 0x04000034 RID: 52
        private BattlefieldArmyCompositionItemVM _meleeCavalryComposition;

        // Token: 0x04000035 RID: 53
        private BattlefieldArmyCompositionItemVM _rangedCavalryComposition;

        // Token: 0x04000036 RID: 54
        private int _armySize;

        // Token: 0x04000037 RID: 55
        private int _maxArmySize;

        // Token: 0x04000038 RID: 56
        private int _minArmySize;

        // Token: 0x04000039 RID: 57
        private string _armySizeTitle;
    }
}
