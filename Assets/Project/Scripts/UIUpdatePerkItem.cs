using System;
using Project;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUpdatePerkItem : MonoBehaviour
{
    [SerializeField]
    private Button _upgradeButton = null;

    [SerializeField]
    private Image _perkIcon = null;

    [SerializeField]
    private TextMeshProUGUI _perkDescription = null;

    [SerializeField]
    private TextMeshProUGUI _cost = null;

    [SerializeField]
    private GameObject _lockLable = null;

    [SerializeField, Space]
    private Image[] _levelIndicatorImages = null;

    [SerializeField]
    private Color _upLevelColor = Color.white;

    [SerializeField]
    private Color _defaultColor = Color.white;

    [SerializeField]
    private Color _cantUpgradeColor = Color.white;

    [SerializeField, Space]
    private Color _defaultBackColor = Color.white;

    [SerializeField]
    private Color _defaultStrokeColor = Color.white;

    [SerializeField]
    private Color _maxLevelBackColor = Color.white;

    [SerializeField]
    private Color _maxLevelStrokeColor = Color.white;

    [SerializeField]
    private Image[] _backs = null;

    [SerializeField]
    private Image[] _backStrokes = null;

    private Action _onClickAction;


    [field: SerializeField]
    public UpgradeLinePerkType PerkLineType
    {
        get;
        private set;
    }

    public void Prepare(Action onClickAction)
    {
        _onClickAction = onClickAction;

        _upgradeButton.onClick.AddListener(OnUpgradeButtonClick);
    }

    private void OnUpgradeButtonClick()
    {
        _onClickAction.Invoke();
    }

    public void Setup(TowerUpgradeSettings.UpdatePreset updatePreset, bool isMaxUpgradeLvl, bool isLock,
        int upgradeLevel)
    {
        _lockLable.SetActive(isLock);

        if (!isLock)
        {
            _perkIcon.sprite = updatePreset.UIIcon;
            _perkDescription.text = updatePreset.UpdateDescription;
            _cost.text = $"{updatePreset.Cost}$";

            if (upgradeLevel >= 0)
            {
                _levelIndicatorImages[upgradeLevel].color = _upLevelColor;
            }
        }
        
        ChangeBackColor(isMaxUpgradeLvl);
    }

    public void RefreshUpgradeLevelIndicator(int currentUpgradeLevel, int maxLvl)
    {
        for (var i = 0; i < _levelIndicatorImages.Length; i++)
        {
            var isMoreThanMaxLvl = i >= maxLvl;
            var isUpgradedLevel = i >= currentUpgradeLevel;

            _levelIndicatorImages[i].color =
                isMoreThanMaxLvl ? _cantUpgradeColor : isUpgradedLevel ? _defaultColor : _upLevelColor;
        }
    }

    public void ChangeBackColor(bool isNeedMaxLevelBack)
    {
        _backs.Do(x => x.color = isNeedMaxLevelBack ? _maxLevelBackColor : _defaultBackColor);
        _backStrokes.Do(x => x.color = isNeedMaxLevelBack ? _maxLevelStrokeColor : _defaultStrokeColor);
    }
}