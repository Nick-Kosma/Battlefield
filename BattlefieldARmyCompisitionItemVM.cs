#region Assembly TaleWorlds.MountAndBlade.CustomBattle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\Modules\CustomBattle\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.CustomBattle.dll
// Decompiled with ICSharpCode.Decompiler 8.1.1.7464
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.CustomBattle;
using TaleWorlds.MountAndBlade.CustomBattle.CustomBattle;

namespace Battlefield

{ public class BattlefieldArmyCompositionItemVM : ArmyCompositionItemVM
    {
    public enum CompositionType
    {
        MeleeInfantry,
        RangedInfantry,
        MeleeCavalry,
        RangedCavalry
    }

    private readonly MBReadOnlyList<SkillObject> _allSkills;

    private readonly List<BasicCharacterObject> _allCharacterObjects;

    private readonly Action<int, int> _onCompositionValueChanged;

    private readonly TroopTypeSelectionPopUpVM _troopTypeSelectionPopUp;

    private BasicCultureObject _culture;

    private readonly StringItemWithHintVM _typeIconData;

    private readonly CompositionType _type;

    private readonly int[] _compositionValues;

    private MBBindingList<BattlefieldCustomBattleTroopTypeVM> _troopTypes;

    private HintViewModel _invalidHint;

    private HintViewModel _addTroopTypeHint;

    private bool _isLocked;

    private bool _isValid;

    [DataSourceProperty]
    public MBBindingList<BattlefieldCustomBattleTroopTypeVM> TroopTypes
    {
        get
        {
            return _troopTypes;
        }
        set
        {
            if (value != _troopTypes)
            {
                _troopTypes = value;
                OnPropertyChangedWithValue(value, "TroopTypes");
            }
        }
    }

    [DataSourceProperty]
    public HintViewModel InvalidHint
    {
        get
        {
            return _invalidHint;
        }
        set
        {
            if (value != _invalidHint)
            {
                _invalidHint = value;
                OnPropertyChangedWithValue(value, "InvalidHint");
            }
        }
    }

    [DataSourceProperty]
    public HintViewModel AddTroopTypeHint
    {
        get
        {
            return _addTroopTypeHint;
        }
        set
        {
            if (value != _addTroopTypeHint)
            {
                _addTroopTypeHint = value;
                OnPropertyChangedWithValue(value, "AddTroopTypeHint");
            }
        }
    }

    [DataSourceProperty]
    public bool IsLocked
    {
        get
        {
            return _isLocked;
        }
        set
        {
            if (value != _isLocked)
            {
                _isLocked = value;
                OnPropertyChangedWithValue(value, "IsLocked");
            }
        }
    }

    [DataSourceProperty]
    public bool IsValid
    {
        get
        {
            return _isValid;
        }
        set
        {
            if (value != _isValid)
            {
                _isValid = value;
                OnPropertyChangedWithValue(value, "IsValid");
            }

            OnValidityChanged(value);
        }
    }

    [DataSourceProperty]
    public int CompositionValue
    {
        get
        {
            return _compositionValues[(int)_type];
        }
        set
        {
            if (value != _compositionValues[(int)_type])
            {
                _onCompositionValueChanged(value, (int)_type);
            }
        }
    }

    public BattlefieldArmyCompositionItemVM(CompositionType type, List<BasicCharacterObject> allCharacterObjects, MBReadOnlyList<SkillObject> allSkills, Action<int, int> onCompositionValueChanged, TroopTypeSelectionPopUpVM troopTypeSelectionPopUp, int[] compositionValues)
    {
        _allCharacterObjects = allCharacterObjects;
        _allSkills = allSkills;
        _onCompositionValueChanged = onCompositionValueChanged;
        _troopTypeSelectionPopUp = troopTypeSelectionPopUp;
        _type = type;
        _compositionValues = compositionValues;
        _typeIconData = GetTroopTypeIconData(type);
        TroopTypes = new MBBindingList<BattlefieldCustomBattleTroopTypeVM>();
        InvalidHint = new HintViewModel(new TextObject("{=iSQTtNUD}This faction doesn't have this troop type."));
        AddTroopTypeHint = new HintViewModel(new TextObject("{=eMbuGGus}Select troops to spawn in formation."));
    }

    public override void RefreshValues()
    {
        base.RefreshValues();
    }

    public void SetCurrentSelectedCulture(BasicCultureObject culture)
    {
        IsLocked = false;
        _culture = culture;
        PopulateTroopTypes();
    }

    public void ExecuteRandomize(int compositionValue)
    {
        IsValid = true;
        IsLocked = false;
        CompositionValue = compositionValue;
        IsValid = TroopTypes.Count > 0;
        TroopTypes.ApplyActionOnAllItems(delegate (BattlefieldCustomBattleTroopTypeVM x)
        {
            x.ExecuteRandomize();
        });
        if (!TroopTypes.Any((BattlefieldCustomBattleTroopTypeVM x) => x.IsSelected) && IsValid)
        {
            TroopTypes[0].IsSelected = true;
        }
    }

    public void ExecuteAddTroopTypes()
    {
        string title = GameTexts.FindText("str_custom_battle_choose_troop", _type.ToString()).ToString();
        _troopTypeSelectionPopUp?.OpenPopUp(title, TroopTypes);
    }

    public void RefreshCompositionValue()
    {
        OnPropertyChanged("CompositionValue");
    }

    private void OnValidityChanged(bool value)
    {
        IsLocked = false;
        if (!value)
        {
            CompositionValue = 0;
        }

        IsLocked = !value;
    }

    private void PopulateTroopTypes()
    {
        TroopTypes.Clear();
        MBReadOnlyList<BasicCharacterObject> defaultCharacters = GetDefaultCharacters();
        foreach (BasicCharacterObject allCharacterObject in _allCharacterObjects)
        {
            if (IsValidUnitItem(allCharacterObject))
            {
                TroopTypes.Add(new Action<BattlefieldCustomBattleTroopTypeVM>(allCharacterObject, _troopTypeSelectionPopUp.OnItemSelectionToggled, _typeIconData, _allSkills, defaultCharacters.Contains(allCharacterObject)));
            }
        }

        IsValid = TroopTypes.Count > 0;
        if (IsValid && !TroopTypes.Any((BattlefieldCustomBattleTroopTypeVM x) => x.IsDefault))
        {
            TroopTypes[0].IsDefault = true;
        }

        TroopTypes.ApplyActionOnAllItems(delegate (BattlefieldCustomBattleTroopTypeVM x)
        {
            x.IsSelected = x.IsDefault;
        });
    }

    private bool IsValidUnitItem(BasicCharacterObject o)
    {
        if (o != null && _culture == o.Culture)
        {
            switch (_type)
            {
                case CompositionType.MeleeInfantry:
                    if (o.DefaultFormationClass != 0)
                    {
                        return o.DefaultFormationClass == FormationClass.HeavyInfantry;
                    }

                    return true;
                case CompositionType.RangedInfantry:
                    return o.DefaultFormationClass == FormationClass.Ranged;
                case CompositionType.MeleeCavalry:
                    if (o.DefaultFormationClass != FormationClass.Cavalry && o.DefaultFormationClass != FormationClass.HeavyCavalry)
                    {
                        return o.DefaultFormationClass == FormationClass.LightCavalry;
                    }

                    return true;
                case CompositionType.RangedCavalry:
                    return o.DefaultFormationClass == FormationClass.HorseArcher;
                default:
                    return false;
            }
        }

        return false;
    }

    private MBReadOnlyList<BasicCharacterObject> GetDefaultCharacters()
    {
        MBList<BasicCharacterObject> mBList = new MBList<BasicCharacterObject>();
        FormationClass formation = FormationClass.NumberOfAllFormations;
        switch (_type)
        {
            case CompositionType.MeleeInfantry:
                formation = FormationClass.Infantry;
                break;
            case CompositionType.RangedInfantry:
                formation = FormationClass.Ranged;
                break;
            case CompositionType.MeleeCavalry:
                formation = FormationClass.Cavalry;
                break;
            case CompositionType.RangedCavalry:
                formation = FormationClass.HorseArcher;
                break;
        }

        mBList.Add(CustomBattleHelper.GetDefaultTroopOfFormationForFaction(_culture, formation));
        return mBList;
    }

    public static StringItemWithHintVM GetTroopTypeIconData(CompositionType type, bool isBig = false)
    {
        TextObject empty = TextObject.Empty;
        string text;
        switch (type)
        {
            case CompositionType.RangedCavalry:
                text = (isBig ? "horse_archer_big" : "horse_archer");
                empty = GameTexts.FindText("str_troop_type_name", "HorseArcher");
                break;
            case CompositionType.RangedInfantry:
                text = (isBig ? "bow_big" : "bow");
                empty = GameTexts.FindText("str_troop_type_name", "Ranged");
                break;
            case CompositionType.MeleeCavalry:
                text = (isBig ? "cavalry_big" : "cavalry");
                empty = GameTexts.FindText("str_troop_type_name", "Cavalry");
                break;
            case CompositionType.MeleeInfantry:
                text = (isBig ? "infantry_big" : "infantry");
                empty = GameTexts.FindText("str_troop_type_name", "Infantry");
                break;
            default:
                return new StringItemWithHintVM("", TextObject.Empty);
        }

        return new StringItemWithHintVM("General\\TroopTypeIcons\\icon_troop_type_" + text, new TextObject("{=!}" + empty.ToString()));
    }
} }
#if false // Decompilation log
'179' items in cache
------------------
Resolve: 'netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Found single assembly: 'netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Load from: 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\Facades\netstandard.dll'
------------------
Resolve: 'TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll'
------------------
Resolve: 'TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll'
------------------
Resolve: 'TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll'
------------------
Resolve: 'TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll'
------------------
Resolve: 'TaleWorlds.MountAndBlade.View, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Could not find by name: 'TaleWorlds.MountAndBlade.View, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Resolve: 'TaleWorlds.Core.ViewModelCollection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'TaleWorlds.Core.ViewModelCollection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.ViewModelCollection.dll'
------------------
Resolve: 'TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll'
------------------
Resolve: 'TaleWorlds.MountAndBlade.ViewModelCollection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'TaleWorlds.MountAndBlade.ViewModelCollection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.ViewModelCollection.dll'
------------------
Resolve: 'TaleWorlds.InputSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'TaleWorlds.InputSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.InputSystem.dll'
------------------
Resolve: 'TaleWorlds.ScreenSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'TaleWorlds.ScreenSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.ScreenSystem.dll'
------------------
Resolve: 'TaleWorlds.Engine.GauntletUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'TaleWorlds.Engine.GauntletUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.GauntletUI.dll'
------------------
Resolve: 'TaleWorlds.GauntletUI.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'TaleWorlds.GauntletUI.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.GauntletUI.Data.dll'
------------------
Resolve: 'TaleWorlds.ObjectSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'TaleWorlds.ObjectSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.ObjectSystem.dll'
------------------
Resolve: 'TaleWorlds.DotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'TaleWorlds.DotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.DotNet.dll'
------------------
Resolve: 'TaleWorlds.MountAndBlade.GauntletUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Could not find by name: 'TaleWorlds.MountAndBlade.GauntletUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Resolve: 'TaleWorlds.MountAndBlade.Multiplayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'TaleWorlds.MountAndBlade.Multiplayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\Modules\Multiplayer\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.Multiplayer.dll'
------------------
Resolve: 'mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Found single assembly: 'mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Load from: 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\mscorlib.dll'
------------------
Resolve: 'System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Found single assembly: 'System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Load from: 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.Core.dll'
------------------
Resolve: 'System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Found single assembly: 'System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Load from: 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.dll'
------------------
Resolve: 'System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Found single assembly: 'System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Load from: 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.Data.dll'
------------------
Resolve: 'System.Diagnostics.Tracing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Could not find by name: 'System.Diagnostics.Tracing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
------------------
Resolve: 'System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Could not find by name: 'System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
------------------
Resolve: 'System.IO.Compression, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Could not find by name: 'System.IO.Compression, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
------------------
Resolve: 'System.IO.Compression.FileSystem, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Could not find by name: 'System.IO.Compression.FileSystem, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
------------------
Resolve: 'System.ComponentModel.Composition, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Could not find by name: 'System.ComponentModel.Composition, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
------------------
Resolve: 'System.Net.Http, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Found single assembly: 'System.Net.Http, Version=4.2.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
WARN: Version mismatch. Expected: '4.0.0.0', Got: '4.2.0.0'
Load from: 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.Net.Http.dll'
------------------
Resolve: 'System.Numerics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Could not find by name: 'System.Numerics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
------------------
Resolve: 'System.Runtime.Serialization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Could not find by name: 'System.Runtime.Serialization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
------------------
Resolve: 'System.Transactions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Could not find by name: 'System.Transactions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
------------------
Resolve: 'System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Could not find by name: 'System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
------------------
Resolve: 'System.Xml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Found single assembly: 'System.Xml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Load from: 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.Xml.dll'
------------------
Resolve: 'System.Xml.Linq, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Found single assembly: 'System.Xml.Linq, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Load from: 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.Xml.Linq.dll'
------------------
Resolve: 'System.Runtime.InteropServices, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'System.Runtime.InteropServices, Version=4.1.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
WARN: Version mismatch. Expected: '2.0.0.0', Got: '4.1.2.0'
Load from: 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\Facades\System.Runtime.InteropServices.dll'
------------------
Resolve: 'System.Runtime.CompilerServices.Unsafe, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null'
Could not find by name: 'System.Runtime.CompilerServices.Unsafe, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null'
#endif
