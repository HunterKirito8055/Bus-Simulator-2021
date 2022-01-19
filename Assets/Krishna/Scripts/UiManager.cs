using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;

public class UiManager : MonoBehaviour
{

    public BusItem[] busProperties;

    [Space(20)]
    [Header("UI Menus")]
    public UIMenu[] menus;
    public EUI_Menus currentMenu;
    public EUI_Menus previousMenu;

    public Button levelsButton;
    public bool freeMode;
    public GameObject PopUp;
    public bool backFromOtherMenus;




    //changes
    public LevelSelectionMenu levelSelectionMenu;
    public Store store;
    public SettingsMenu settingsMenu;
    public VehicleSelectionMenu vehicleSelectionUi;
    public LevelCompleteMenu levelCompleteMenu;
    public LoadingScreenUI loadingScreenUI;
    public GameOverMenu gameOverMenu;

    void Start()
    {
        HideAllMenus();
        OpenMenuWithEnum(EUI_Menus.mainMenu);

        settingsMenu.Awake();
        if (SingleTon<DebugPanel>.Exists)
        {
            DebugPanel.RegisterSection("Unlock All Level", 90, new DebugPanel.RenderDebugGUI(this.RenderDebugPanel));
        }
        levelSelectionMenu.Start();
    }
    public void OpenMenu(UIMenu _menu)
    {
        previousMenu = currentMenu;
        HideAllMenus();
        if (_menu)
        {
            currentMenu = _menu.menuName;
            switch (_menu.menuName)
            {
                case EUI_Menus.none:
                    break;
                case EUI_Menus.mainMenu:
                    break;
                case EUI_Menus.vehicleSelectionMenu:
                    vehicleSelectionUi.OnEnable();
                    break;
                case EUI_Menus.storeMenu:
                    break;
                case EUI_Menus.settingsMenu:
                    break;
                case EUI_Menus.careerModeMenu:
                    break;
                case EUI_Menus.levelSelectionMenu:
                    levelSelectionMenu.UpdateUnlockStatus();
                    break;
                case EUI_Menus.gameOver:
                    break;
                case EUI_Menus.levelComplete:
                    break;
                case EUI_Menus.pauseMenu:
                    break;
                case EUI_Menus.gameTaskMenu:
                    break;
                case EUI_Menus.areYouSure:
                    break;
                default:
                    break;
            }
            _menu.Show();
        }
    }

    public void Back_Btn()
    {
        switch (currentMenu)
        {
            case EUI_Menus.none:
                break;
            case EUI_Menus.mainMenu:
                break;
            case EUI_Menus.vehicleSelectionMenu:
                vehicleSelectionUi.OnDisable();
                OpenMenuWithEnum(EUI_Menus.mainMenu);
                break;
            case EUI_Menus.storeMenu:
                OpenMenuWithEnum(EUI_Menus.mainMenu);
                break;
            case EUI_Menus.settingsMenu:
                OpenMenuWithEnum(EUI_Menus.mainMenu);
                break;
            case EUI_Menus.careerModeMenu:
                OpenMenuWithEnum(EUI_Menus.vehicleSelectionMenu);
                break;
            case EUI_Menus.levelSelectionMenu:
                if (backFromOtherMenus)
                {
                    MainMenu();
                }
                else
                {
                    OpenMenuWithEnum(EUI_Menus.careerModeMenu);
                }
                break;
            case EUI_Menus.gameTaskMenu:
                if (previousMenu == EUI_Menus.pauseMenu)
                {
                    OpenMenuWithEnum(previousMenu);
                }
                else
                {
                    OpenMenuWithEnum(EUI_Menus.levelSelectionMenu);
                }
                break;
            default:
                break;
        }
    }
    public void HideAllMenus()
    {
        foreach (var item in menus)
        {
            if (item != null)
                item.Hide();
            else
            {
                Debug.Log("Script Reference not given / Created ==> " + item.name);
            }
        }
    }
    public void OpenMenuWithEnum(EUI_Menus _menu)
    {
        UIMenu m = GetUiMenu(_menu);
        OpenMenu(m);
    }


    public UIMenu GetUiMenu(EUI_Menus _menu)
    {
        return System.Array.Find(menus, x => x.menuName == _menu);
    }
    public void OnFreeModeBtn()
    {
        freeMode = true;
        GameManager.BusSimulation.StartGame();
    }


    public void PopUpDisplay(string _message)
    {
        PopUp.GetComponent<PopUpMenu>().PopUpMessage(_message);
    }
    public void UnlockLevel(int _levelNumber)
    {
        PlayerPrefs.SetInt(StringManager.LevelUnlocked + _levelNumber, 1);
        levelSelectionMenu.UpdateUnlockStatus();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            for (int i = 0; i < LevelManager.GetTotalLevels(); i++)
            {
                PlayerPrefs.SetInt(StringManager.LevelUnlocked + i, 1);
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            PlayerPrefs.DeleteKey(StringManager.totalCoins);
        }
    }

    /// <summary>
    /// vishal ================================================
    /// </summary>
    public void PauseGame()
    {
        levelsButton.interactable = !freeMode;
        Time.timeScale = 0;
        OpenMenuWithEnum(EUI_Menus.pauseMenu);
        backFromOtherMenus = true;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
        HideAllMenus();
    }

    public void MainMenu()
    {
        ResumeGame();
        OpenMenuWithEnum(EUI_Menus.mainMenu);
        StopGameFlow();
        backFromOtherMenus = false;
    }
    public void OpenStoreMenu()
    {
        ResumeGame();
        OpenMenuWithEnum(EUI_Menus.storeMenu);
        StopGameFlow();
        backFromOtherMenus = false;
    }
    public void LevelSelection()
    {
        if (freeMode)
        {
            return;
        }
        backFromOtherMenus = true;
        OpenMenuWithEnum(EUI_Menus.levelSelectionMenu);
        StopGameFlow();
    }
    public void DisplayScore(int _passangersCount, float _timeTakenCount, int _cashEarnedCount, int _passengersComfortCount, int _timeBonusCount, int _servingBonus, int _totalEarned)
    {
        levelCompleteMenu.DisplayScore(_passangersCount, _timeTakenCount, _cashEarnedCount, _passengersComfortCount, _timeBonusCount, _servingBonus, _totalEarned);
    }

    public void NextLevel()
    {
        ResumeGame();

        GameManager.BusSimulation.NextLevel();
    }
   
    public void GameOver(string _gameOverReason)
    {

        PopUpDisplay(_gameOverReason);
        OpenMenuWithEnum(EUI_Menus.gameOver);
        gameOverMenu.DisplayGameOverReason(_gameOverReason);
    }

    public void RetryGame()
    {
        if (GameManager.BusSimulation.activeBus)
            GameManager.BusSimulation.activeBus.KillEngine();
        GameManager.BusSimulation.RestartGame();
        ResumeGame();
    }
    void StopGameFlow()
    {
        GameManager.BusSimulation.StopGameFlow();
    }

    public void SoundOff()
    {
        if (settingsMenu.Sound)
        {
            settingsMenu.Sound = false;
        }
        if (settingsMenu.Music)
        {
            settingsMenu.Music = false;
        }

    }
    public void SoundOn()
    {
        if (!settingsMenu.Sound)
        {
            settingsMenu.Sound = true;
        }
        if (!settingsMenu.Music)
        {
            settingsMenu.Music = false;
        }
    }

    public void YesExit()
    {
        Application.Quit();
    }

    public void RenderDebugPanel()
    {

        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        if (GUILayout.Button("Unlock All Levels", new GUILayoutOption[0]))
        {
            foreach (var item in levelSelectionMenu.levelBtnScripts)
            {
                item.BtnLock(true);
            }
        }
        GUILayout.EndHorizontal();

    }
    #region Loading Panel
    /// for Loadding Panel

    public bool StartLoading
    {

        set
        {
            if (value)
            {
                StartCoroutine(loadingScreenUI.StartLoading());
            }
        }
    }
    [System.Serializable]
    public class LoadingScreenUI
    {
        public GameObject loadingPanelObject;

        public Image loadingFillBar;

        public float amount;
        public IEnumerator StartLoading()
        {
            Time.timeScale = 0;
            if (GameManager.BusSimulation.activeBus)
                GameManager.BusSimulation.activeBus.fuelInput = 0;
            GameManager.UiManager.SoundOff();
            //
            loadingFillBar.fillAmount = 0;
            amount = 0;
            loadingPanelObject.SetActive(true);
            while (loadingFillBar.fillAmount != 1)
            {
                yield return new WaitForSecondsRealtime(Random.Range(0.3f, 1f));
                amount += Random.Range(0.1f, 0.5f);
                loadingFillBar.fillAmount = Mathf.Clamp01(amount);
            }



            yield return new WaitForSecondsRealtime(0.3f);
            GameManager.UiManager.StartLoading = false;
            if (GameManager.BusSimulation.activeBus)
                GameManager.BusSimulation.activeBus.FuelTank = GameManager.BusSimulation.activeBus.fuelTankCapacity;
            loadingPanelObject.SetActive(false);
            Time.timeScale = 1;

            GameManager.UiManager.SoundOn();
            yield return null;
        }
    }//class
    #endregion

    #region LevelcompleteMenu

    [System.Serializable]
    public class LevelCompleteMenu
    {
        public TextMeshProUGUI passangersCount, timeTakenCount, cashEarnedCount, passengersComfortCount, timeBonusCount, servingBonus, totalEarned;
        public void DisplayScore(int _passangersCount, float _timeTakenCount, int _cashEarnedCount, int _passengersComfortCount, int _timeBonusCount, int _servingBonus, int _totalEarned)
        {
            passangersCount.text = _passangersCount.ToString();

            System.TimeSpan ts = new System.TimeSpan();
            ts = System.TimeSpan.FromSeconds(_timeTakenCount);
            timeTakenCount.text = string.Format("{0}:{1}", ts.Minutes, ts.Seconds.ToString("00"));

            cashEarnedCount.text = _cashEarnedCount.ToString();
            passengersComfortCount.text = _passengersComfortCount.ToString();
            timeBonusCount.text = _timeBonusCount.ToString();
            servingBonus.text = _servingBonus.ToString();
            totalEarned.text = _totalEarned.ToString();

            GameManager.UiManager.currencyManager.AddCoins(_totalEarned);
        }
    }

    #endregion

    #region Currency Manager
    public CurrencyManager currencyManager;
    [System.Serializable]
    public class CurrencyManager
    {

        public TextMeshProUGUI[] coinsTexts;

        public void AddCoins(int _addingCoins)
        {
            PlayerPrefs.SetInt(StringManager.totalCoins, PlayerPrefs.GetInt(StringManager.totalCoins, 0) + _addingCoins);
            UpdateCurrenncyTexts();
        }
        public void RemoveCoins(int _removingCoins)
        {

            GameManager.AchievementManager.AddAchievementProgress("BASIC_SPENDER", _removingCoins);

            PlayerPrefs.SetInt(StringManager.totalCoins, PlayerPrefs.GetInt(StringManager.totalCoins, 0) - _removingCoins);
            UpdateCurrenncyTexts();
        }
        public int GetCoinsCount()
        {
            return PlayerPrefs.GetInt(StringManager.totalCoins, 0);
        }

        public void UpdateCurrenncyTexts()
        {
            foreach (var item in coinsTexts)
            {
                item.text = GetCoinsCount().ToString();
            }
        }
    }

    #endregion

    #region Vehicle Selection Menu

    public void OnNextBus()
    {
        vehicleSelectionUi.OnNext();
    }
    public void OnPreviousBus()
    {
        vehicleSelectionUi.OnPrevious();
    }
    public void OnSelectBus()
    {
        vehicleSelectionUi.OnSelect();
    }
    [System.Serializable]
    public class VehicleSelectionMenu
    {
        public int playerIndex;
        public TextMeshProUGUI selectButtonText;


        public Image powerBar, suspensionBar, gripBar, torqueBar;
        public void OnEnable()
        {
            playerIndex = 0;
            LimitToBounds();
            PlayerPrefs.SetInt(StringManager.playerPurchaseState + playerIndex, 1);
            updateSelectBtnTxt();
        }
        public void OnDisable()
        {
            foreach (var item in GameManager.UiManager.busProperties)
            {
                if (item.busObject)
                {
                    item.busObject.SetActive(false);
                }
            }
        }
        internal void OnNext()
        {
            playerIndex++;
            LimitToBounds();
            updateSelectBtnTxt();
        }
        internal void OnPrevious()
        {
            playerIndex--;
            LimitToBounds();
            updateSelectBtnTxt();
        }
        void updateSelectBtnTxt()
        {
            if (PlayerPrefs.GetInt(StringManager.playerPurchaseState + playerIndex) == 1)
            {
                selectButtonText.text = "Select";
            }
            else
            {
                selectButtonText.text = "Buy for " + GameManager.UiManager.busProperties[playerIndex].price + " $";
            }
        }
        internal void OnSelect()
        {
            if (selectButtonText.text.Contains("Select"))
            {
                Select();
            }
            if (selectButtonText.text.Contains("Buy for"))
            {
                if (GameManager.UiManager.busProperties[playerIndex].price <= GameManager.UiManager.currencyManager.GetCoinsCount())
                {
                    GameManager.UiManager.currencyManager.RemoveCoins(GameManager.UiManager.busProperties[playerIndex].price);
                    PlayerPrefs.SetInt(StringManager.playerPurchaseState + playerIndex, 1);
                    PlayerPrefs.SetInt(StringManager.PLAYERINDEX, playerIndex);
                    GameManager.Notification.DisplayNotification("Purchased Successfully");
                    updateSelectBtnTxt();
                    if (playerIndex == 1)
                        GameManager.AchievementManager.AddAchievementProgress("UNLOCK_1_BUS", 1);
                }
                else
                {
                    GameManager.Notification.DisplayNotification("Insufficient Coins");
                }
            }
        }
        void Select()
        {
            PlayerPrefs.SetInt(StringManager.PLAYERINDEX, playerIndex);
            GameManager.UiManager.OpenMenuWithEnum(EUI_Menus.careerModeMenu);
            foreach (var item in GameManager.UiManager.busProperties)
            {
                item.busObject.SetActive(false);
            }
        }
        private void LimitToBounds()
        {
            if (playerIndex > GameManager.UiManager.busProperties.Length - 1)
            {
                playerIndex = 0;
            }
            if (playerIndex < 0)
            {
                playerIndex = GameManager.UiManager.busProperties.Length - 1;
            }
            UpdateBusItemStats();
        }


        void UpdateBusItemStats()
        {
            foreach (var item in GameManager.UiManager.busProperties)
            {
                item.busObject.SetActive(false);
            }
            GameManager.UiManager.busProperties[playerIndex].busObject.SetActive(true);
            suspensionBar.fillAmount = GameManager.UiManager.busProperties[playerIndex].suspension;
            powerBar.fillAmount = GameManager.UiManager.busProperties[playerIndex].power;
            gripBar.fillAmount = GameManager.UiManager.busProperties[playerIndex].grip;
            torqueBar.fillAmount = GameManager.UiManager.busProperties[playerIndex].torque;
        }
    }
    #endregion


    #region StoreMenu
    public void AddCoins(int count)
    {
        store.AddCoins(count);
        PopUpDisplay(count + " Added Successful");
    }

    [System.Serializable]
    public class Store
    {
        internal void AddCoins(int _coins)
        {
            GameManager.UiManager.currencyManager.AddCoins(_coins);
            GameManager.Notification.DisplayNotification(_coins + " Added Successfully");
        }

    }
    #endregion

    #region Settings Menu


    [System.Serializable]
    public class SettingsMenu
    {
        public GameObject soundCheckMark,soundCheckMarkp, musicCheckMark;
        public Button touchBtn, steeringBtn, soundToggle, musicTogggle;
        public Button touchBtnP, steeringBtnP, soundToggleP, musicTogggleP;
        public TMP_Dropdown dropdown, dropDownP;

        public RenderPipelineAsset[] qualityLevels;
        public void Awake()
        {
            Controls = PlayerPrefs.GetInt(StringManager.controlsKey, 0) == 0 ? Controls.touch : Controls.steering;
            Sound = PlayerPrefs.GetInt(StringManager.soundKey, 1) == 1 ? true : false;
            Music = PlayerPrefs.GetInt(StringManager.musicKey, 1) == 1 ? true : false;
            touchBtn.onClick.AddListener(OnTouch_btn);
            steeringBtn.onClick.AddListener(OnSteering_btn);
            soundToggle.onClick.AddListener(OnSoundToggle);
            musicTogggle.onClick.AddListener(OnMusicToggle);
            touchBtnP.onClick.AddListener(OnTouch_btn);
            steeringBtnP.onClick.AddListener(OnSteering_btn);
            soundToggleP.onClick.AddListener(OnSoundToggle);
            if (musicTogggleP)
                musicTogggleP.onClick.AddListener(OnMusicToggle);

            dropdown.onValueChanged.AddListener(ChangeQuality);
            if (dropDownP)
                dropDownP.onValueChanged.AddListener(ChangeQuality);
        }

        #region ControlSettingsFunctions

        Controls controls;
        public Controls Controls
        {
            get
            {
                return controls;
            }
            set
            {
                controls = value;
                var mobile = RCC_Settings.MobileController.TouchScreen;
                switch (controls)
                {
                    case Controls.touch:
                        mobile = RCC_Settings.MobileController.TouchScreen;
                        touchBtn.interactable = false;
                        touchBtnP.interactable = false;
                        steeringBtn.interactable = true;
                        steeringBtnP.interactable = true;
                        break;
                    case Controls.steering:
                        mobile = RCC_Settings.MobileController.SteeringWheel;
                        steeringBtn.interactable = false;
                        steeringBtnP.interactable = false;
                        touchBtn.interactable = true;
                        touchBtnP.interactable = true;
                        break;
                    default:
                        break;
                }
                RCC.SetMobileController(mobile);
                PlayerPrefs.SetInt(StringManager.controlsKey, (int)mobile);

            }
        }



        internal void OnTouch_btn()
        {
            Controls = Controls.touch;
        }
        internal void OnSteering_btn()
        {
            Controls = Controls.steering;
        }
        #endregion
        #region SoundSettingsFunctions

        bool sound;
        public bool Sound
        {
            get
            {
                return sound;
            }
            set
            {
                sound = value;
                if (sound)
                {
                    RCC_Settings.Instance.audioMixer.audioMixer.SetFloat("volume", 0f);
                }
                else
                {
                    RCC_Settings.Instance.audioMixer.audioMixer.SetFloat("volume", -80f);
                }
                soundCheckMark.SetActive(sound);
                soundCheckMarkp.SetActive(sound);
                PlayerPrefs.SetInt(StringManager.soundKey, sound ? 1 : 0);
            }
        }
        internal void OnSoundToggle()
        {
            Sound = !Sound;
        }

        #endregion
        #region MusicSettingFunctions

        bool music;
        public bool Music
        {
            get
            {
                return music;
            }
            set
            {
                music = value;
                if (music)
                {
                    if (!musicCheckMark)
                    {
                        return;
                    }
                    //music is not yet present
                }
                else
                {
                    if (!musicCheckMark)
                    {
                        return;
                    }
                    //music is not yet present
                }
                PlayerPrefs.SetInt(StringManager.musicKey, music ? 1 : 0);
                musicCheckMark.SetActive(music);
            }
        }
        internal void OnMusicToggle()
        {
            Music = !Music;
        }

        #endregion


        #region 
        public void ChangeQuality(int level)
        {
            QualitySettings.SetQualityLevel(level);
            QualitySettings.renderPipeline = qualityLevels[level];
            dropDownP.value = dropdown.value = level;
            PlayerPrefs.SetInt("QualityState", level);
        }
        #endregion
    }
    public enum Controls
    {
        touch, steering
    }
    #endregion

    #region LevelSelectionMenu
    [System.Serializable]
    public class LevelSelectionMenu
    {
        public GameObject btnPrefab;
        public Transform content;
        int numberOfLevels;
        public LevelBtnScript[] levelBtnScripts;
        public void Start()
        {
            numberOfLevels = GameManager.LevelManager.TotalLevels;
            PlayerPrefs.SetInt(StringManager.LevelUnlocked + 0, 1);
            CreateButtons();

        }
        void CreateButtons()
        {
            levelBtnScripts = new LevelBtnScript[numberOfLevels];
            for (int i = 0; i < numberOfLevels; i++)
            {
                LevelBtnScript levelBtnScript = new LevelBtnScript();
                GameObject newBtn = Instantiate(btnPrefab, content);
                newBtn.SetActive(true);
                levelBtnScript.Init(i, newBtn);
                levelBtnScripts[i] = levelBtnScript;
            }
            UpdateUnlockStatus();
        }
        public void UpdateUnlockStatus()
        {
            for (int i = 0; i < numberOfLevels; i++)
            {
                if (!(PlayerPrefs.GetInt(StringManager.LevelUnlocked + i) == 1))
                {
                    levelBtnScripts[i].BtnLock(false);
                }
                else
                {
                    levelBtnScripts[i].BtnLock(true);
                }
            }
        }

        public bool IsLevelUnlocked(int val)
        {
            return (PlayerPrefs.GetInt(StringManager.LevelUnlocked + val) == 1);
        }
    }
    #endregion


    #region GameOverMenu
    [System.Serializable]
    public class GameOverMenu
    {
        public TextMeshProUGUI gameOverReasonTxt;
        public TextMeshProUGUI reasonTxt;
        string passengerKilledReason = "Oh no! you killed passengers Upgrade your bus! for stronger brakes";
        string fellOffTheHillReason = "Oh no! you fell off the hill Upgrade your bus! for more power and better controls";
        public void DisplayGameOverReason(string _gameOverReason)
        {
            GameManager.BusSimulation.isLevelDone = false;
            GameManager.BusSimulation.IsGameOver = false;
            gameOverReasonTxt.text = _gameOverReason;
            switch (_gameOverReason)
            {
                case StringManager.YOUFELL:
                    reasonTxt.text = fellOffTheHillReason;
                    break;
                case StringManager.YOUKILLEDPASSENGER:
                    reasonTxt.text = passengerKilledReason;
                    break;
                default:
                    break;
            }
        }
    }


    #endregion
}//class
[System.Serializable]
public enum EUI_Menus
{
    none,
    mainMenu,
    vehicleSelectionMenu,
    storeMenu,
    settingsMenu,
    careerModeMenu,
    levelSelectionMenu,
    gameOver,
    levelComplete,
    pauseMenu,
    gameTaskMenu,
    areYouSure
}
[System.Serializable]
public class BusItem
{
    public GameObject busObject;
    public int price;

    [Range(0f, 1f)]
    public float power, suspension, grip, torque;
}

[System.Serializable]
public class LevelBtnScript
{
    int btnIndex;
    GameObject gameObj;
    Button levelSelectionBtn;
    TextMeshProUGUI areasToTravel;
    TextMeshProUGUI distanceToTravel;
    TextMeshProUGUI timeToTravel;
    public void Init(int _btnIndex, GameObject _gameObject)
    {
        btnIndex = _btnIndex;
        gameObj = _gameObject;

        GetComponents();
        AssignFunctionalities();
    }
    private void GetComponents()
    {
        areasToTravel = gameObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        distanceToTravel = gameObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        timeToTravel = gameObj.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        levelSelectionBtn = gameObj.transform.GetChild(3).GetComponent<Button>();
    }
    private void AssignFunctionalities()
    {
        int maxTime = GameManager.LevelManager.GetLevelInfo(btnIndex).maxTime;
        int distance = (int)Vector3.Distance(GameManager.LevelManager.GetLevelInfo(btnIndex).pickUp.transform.position, GameManager.LevelManager.GetLevelInfo(btnIndex).dropOff.transform.position);

        distanceToTravel.text = distance + "M".ToString();
        areasToTravel.text = (btnIndex + 1) + "-" + (btnIndex + 2);
        System.TimeSpan ts = new System.TimeSpan();
        ts = System.TimeSpan.FromSeconds(maxTime);
        timeToTravel.text = string.Format("{0}M:{1}S", ts.Minutes, ts.Seconds.ToString("00"));


        levelSelectionBtn.onClick.AddListener(() => LoadLevel(btnIndex));
    }
    void LoadLevel(int i)
    {
        GameManager.BusSimulation.CurrentLevelNum = i;
        Debug.Log("level " + (i + 1) + " enabled");
        GameManager.UiManager.freeMode = false;
        GameManager.BusSimulation.StartGame(i);
    }
    public void BtnLock(bool _locked)
    {
        levelSelectionBtn.interactable = _locked;
    }
}
