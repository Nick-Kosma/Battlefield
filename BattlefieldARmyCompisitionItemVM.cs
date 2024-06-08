using System;
using System.Collections.Generic;
using System.Linq; // Ensure this using directive is included
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.CustomBattle;
using TaleWorlds.MountAndBlade.CustomBattle.CustomBattle;

namespace Battlefield
{
    public class BattlefieldArmyCompositionItemVM : ArmyCompositionItemVM
    {
        // Ensure the CompositionType matches with the base class
        public new enum CompositionType
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
        private MBBindingList<CustomBattleTroopTypeVM> _troopTypes; // Changed to CustomBattleTroopTypeVM
        private HintViewModel _invalidHint;
        private HintViewModel _addTroopTypeHint;
        private bool _isLocked;
        private bool _isValid;

        [DataSourceProperty]
        public MBBindingList<CustomBattleTroopTypeVM> TroopTypes // Changed to CustomBattleTroopTypeVM
        {
            get => _troopTypes;
            set
            {
                if (value != _troopTypes)
                {
                    _troopTypes = value;
                    OnPropertyChangedWithValue(value, nameof(TroopTypes));
                }
            }
        }

        [DataSourceProperty]
        public HintViewModel InvalidHint
        {
            get => _invalidHint;
            set
            {
                if (value != _invalidHint)
                {
                    _invalidHint = value;
                    OnPropertyChangedWithValue(value, nameof(InvalidHint));
                }
            }
        }

        [DataSourceProperty]
        public HintViewModel AddTroopTypeHint
        {
            get => _addTroopTypeHint;
            set
            {
                if (value != _addTroopTypeHint)
                {
                    _addTroopTypeHint = value;
                    OnPropertyChangedWithValue(value, nameof(AddTroopTypeHint));
                }
            }
        }

        [DataSourceProperty]
        public bool IsLocked
        {
            get => _isLocked;
            set
            {
                if (value != _isLocked)
                {
                    _isLocked = value;
                    OnPropertyChangedWithValue(value, nameof(IsLocked));
                }
            }
        }

        [DataSourceProperty]
        public bool IsValid
        {
            get => _isValid;
            set
            {
                if (value != _isValid)
                {
                    _isValid = value;
                    OnPropertyChangedWithValue(value, nameof(IsValid));
                }

                OnValidityChanged(value);
            }
        }

        [DataSourceProperty]
        public int CompositionValue
        {
            get => _compositionValues[(int)_type];
            set
            {
                if (value != _compositionValues[(int)_type])
                {
                    _onCompositionValueChanged(value, (int)_type);
                }
            }
        }

        // Corrected constructor
        public BattlefieldArmyCompositionItemVM(CompositionType type, List<BasicCharacterObject> allCharacterObjects, MBReadOnlyList<SkillObject> allSkills, Action<int, int> onCompositionValueChanged, TroopTypeSelectionPopUpVM troopTypeSelectionPopUp, int[] compositionValues)
            : base((ArmyCompositionItemVM.CompositionType)(object)type, allCharacterObjects, allSkills, onCompositionValueChanged, troopTypeSelectionPopUp, compositionValues) // Correct conversion
        {
            _allCharacterObjects = allCharacterObjects;
            _allSkills = allSkills;
            _onCompositionValueChanged = onCompositionValueChanged;
            _troopTypeSelectionPopUp = troopTypeSelectionPopUp;
            _type = type;
            _compositionValues = compositionValues;
            _typeIconData = GetTroopTypeIconData(type);
            TroopTypes = new MBBindingList<CustomBattleTroopTypeVM>(); // Changed to CustomBattleTroopTypeVM
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
            TroopTypes.ApplyActionOnAllItems(delegate (CustomBattleTroopTypeVM x) // Changed to CustomBattleTroopTypeVM
            {
                x.ExecuteRandomize();
            });
            if (!TroopTypes.Any(x => x.IsSelected) && IsValid) // Changed to use Enumerable.Any
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
            OnPropertyChanged(nameof(CompositionValue));
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
                    TroopTypes.Add(new BattlefieldCustomBattleTroopTypeVM(allCharacterObject, _troopTypeSelectionPopUp.OnItemSelectionToggled, _typeIconData, _allSkills, defaultCharacters.Contains(allCharacterObject)));
                }
            }

            IsValid = TroopTypes.Count > 0;
            if (IsValid && !TroopTypes.Any(x => x.IsDefault)) // Changed to use Enumerable.Any
            {
                TroopTypes[0].IsDefault = true;
            }

            TroopTypes.ApplyActionOnAllItems(x =>
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
    }
}
