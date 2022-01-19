using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("LevelManager")]
public class BusSimulation : MonoBehaviour
{

    [Space(13f)]
    [Header("UI Assets")]
    public Gradient gradient; // we are using this in Uimanager as well
    [SerializeField] GameObject emotionImage;
    [SerializeField] Text timerText;
    [SerializeField] Image infoImage;
    [SerializeField] GameObject miniCamUi;
    public GameObject fpsRearCam;
    Text infoText;
    internal Image emotionBar;
    [SerializeField] Button openDoorBtn, closeDoorBtn;
    [Space(10)]
    [Header("Fuel Terms")]
    internal Image fuelBar, fuelLowIndicator;
    [SerializeField] GameObject fuelImage;
    [SerializeField] Image fuelBarExtra;
    [SerializeField] Text fuelText;
    [SerializeField] public GameObject fuelToggleBtn;
    [SerializeField] Button fuel25Button, fuelFullBuyBtn, yesBtn, noBtn;
    [SerializeField] GameObject fuelPanel;
    [SerializeField] GameObject insufficientBalancePanel;
    [SerializeField] int fuelCost = 80;
    [SerializeField] float refillSpeed = 7f;
    [SerializeField] float fuelConsumptionRate = 2f;
    [SerializeField] float fallTime = 2f;
    [SerializeField] Button serveFood;
    [SerializeField] float serveLimitTime = 3f;
    [SerializeField] float serveAfterTime = 5f;
    Image serveImage;

    [SerializeField] GameObject passengersParent;


    [SerializeField] public bool isFilling;

    [Space(15f)]
    [Header("for checking Variables")]
    //public List<Transform> allLevelParents;
    public RCC_CarSelectionExample carSelectionExample;
    public bool isGameOver = false, isGameStart = false;
    public bool isLevelDone;
    [HideInInspector]
    public RCC_CarControllerV3 activeBus;
    [HideInInspector]
    public ExtraTagObjectContainer extraTag;
    public bool isInArea;
    [HideInInspector]
    public RCC_Camera.CameraMode prevCamMode;

    [Space(13f)]
    [Header("Private Checking Variables")]

    #region Private Variables for Checking Purpose


    RCC_UIDashboardButton gearUi;


    BusStopMode modeofStop;

    int currentLevelNum;

    bool isPickUp, isDropOff;

    float elapsedTime;
    TimeSpan timeSpan;


    GameObject frontDoor;
    Vector3 busOpenDoorPos = new Vector3(0.15f, 0, -1.31f);
    float fallElapsed = 0;
    int comfortScore = 0;
    int timeBonus = 0, cashEarned = 0, totalEarned = 0;
    int servingBonus = 0;
    bool isServable = false;
    bool stuck;
    #endregion


    #region public Properties
    public bool IsGameOver
    {
        get
        {
            return isGameOver;
        }
        set
        {
            isGameOver = value;
            //if (value)
            //{
            //    isGameStart = false;
            //}
        }
    }
    public bool IsGameStart
    {
        get
        {
            return isGameStart;
        }
        set
        {
            isGameStart = value;
            //if (value)
            //{
            //    isGameOver = false;
            //}
        }
    }
    internal LevelInfo CurrentLevel
    {
        get
        {
            return GameManager.LevelManager.CurrentLevel;
        }
        set
        {
            GameManager.LevelManager.currentLevel = value;
        }
    }
    public int MaxTimeBonus
    {
        get
        {
            return CurrentLevel.maxTimeBonus;
        }

    }
    public int MaxTime
    {
        get
        {
            return CurrentLevel.maxTime;
        }

    }

    public int CurrentLevelNum
    {
        get
        {
            return currentLevelNum /*= PlayerPrefs.GetInt(StringManager.LEVELINDEX, 0)*/;
        }
        set
        {
            currentLevelNum = value;
            GameManager.UiManager.UnlockLevel(value);
        }
    }
    public float FuelAmount
    {
        get
        {
            return fuelBar.fillAmount;
        }
        set
        {
            fuelBarExtra.fillAmount = fuelBar.fillAmount = value;
            fuelBarExtra.color = fuelBar.color = gradient.Evaluate(fuelBar.fillAmount);
            fuelLowIndicator.color = fuelBar.fillAmount < 0.15f ? Color.red : new Color(.1f, 0f, 0f);
            fuelText.text = String.Format("{0} %", (int)(fuelBar.fillAmount * 100));

            fuel25Button.interactable = !(fuelBar.fillAmount >= 0.7f);
            fuelFullBuyBtn.interactable = !(fuelBarExtra.fillAmount > 0.99f);
        }
    }
    public bool IsFilling
    {
        get
        {
            return isFilling;
        }
        set
        {
            isFilling = value;
            fuel25Button.gameObject.SetActive(!value);
            fuelFullBuyBtn.gameObject.SetActive(!value);
        }
    }

    public float EmotionAmount
    {
        get
        {
            return emotionBar.fillAmount;
        }
        set
        {
            emotionBar.fillAmount = value;
            emotionBar.color = gradient.Evaluate(emotionBar.fillAmount);
        }
    }
    public bool IsPickUp
    {
        get { return isPickUp; }
        set
        {
            isPickUp = value;
        }
    }
    public bool IsDropOff
    {
        get
        {
            return isDropOff;
        }
        set
        {
            isDropOff = value;
            if (isDropOff && isPickUp)
                LevelComplete();
        }
    }
    public bool ActiveServing
    {
        //get
        //{
        //    return serveFood.gameObject.activeInHierarchy;
        //}
        set
        {
            serveFood.gameObject.SetActive(value);
        }
    }
    public float ServeAmount
    {
        get
        {
            return serveImage.fillAmount;
        }
        set
        {
            serveImage.fillAmount = Mathf.Clamp01(value);
            serveImage.color = gradient.Evaluate(serveImage.fillAmount);
            serveFood.image.color = gradient.Evaluate(serveImage.fillAmount);
        }
    }
    IEnumerator StartServing()
    {
        yield return new WaitUntil(() =>
        {
            if (elapsedTime > serveAfterTime) //control waits here till the serving time
            {
                return true;
            }
            return false;
        });
        ActiveServing = true;
        yield return null;
        float servingTime = Time.time + serveLimitTime;
        ServeAmount = 0;
        isServable = false;
        while (Time.time < servingTime && !isServable)
        {
            if (!IsGameStart || IsGameOver)
            {
                yield break;
            }
            ServeAmount = Mathf.PingPong(Time.time * 0.5f, 1f);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        if (isServable) //when we click serve then this becomes true..
        {
            GameManager.Notification.DisplayNotification("SERVING DONE SUCCESSFUL!");
        }
        else
        {
            GameManager.Notification.DisplayNotification("YOU DINT SERVE FOOD TO PASSENGERS!");
            ServeAmount = 0;
        }
        isServable = false;
        ActiveServing = false;
    }

    #endregion

    #region unity Methods
    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        carSelectionExample = GameManager.CarSelectionExample;
        openDoorBtn.onClick.AddListener(() => { OpenDoor(); });
        closeDoorBtn.onClick.AddListener(() => { CloseDoor(); });

        fuel25Button.onClick.AddListener(() => { BuyFuel((int)(activeBus.fuelTankCapacity / 4)); });
        fuelFullBuyBtn.onClick.AddListener(() => { BuyFuel(-1); });

        fuelToggleBtn.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
        {
            fuelPanel.SetActive(value);
            ShowMainHUD(!value, !value);
            PauseResume(value);
            RefreshFuelRates();

        }
        );
        yesBtn.onClick.AddListener(() => { OnYesBtn(); });
        noBtn.onClick.AddListener(() => { OnNoBtn(); });
        serveImage = serveFood.transform.GetChild(1).GetComponent<Image>();
        serveFood.onClick.AddListener(() => isServable = true);
        //StartCoroutine(StartServing());
        infoText = infoImage.GetComponentInChildren<Text>(true);
        emotionBar = emotionImage.transform.GetChild(0).GetComponentInChildren<Image>(true);
        fuelBar = fuelImage.transform.GetChild(0).GetComponentInChildren<Image>(true);
        fuelLowIndicator = fuelImage.transform.GetChild(1).GetComponentInChildren<Image>(true);

        EvaluateComponentValues();



    }

    private void Start()
    {
        if (SingleTon<DebugPanel>.Exists)
        {
            // DebugPanel.RegisterSection("Finish Level", 90, new DebugPanel.RenderDebugGUI(this.RenderDebugPanel));
        }
        gearUi = RCC_MobileButtons.Instance.gearButton.GetComponent<RCC_UIDashboardButton>();
    }
    public void RenderDebugPanel()
    {
        //GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        //if (GUILayout.Button("Next Checkpoint", new GUILayoutOption[0]))
        //{
        //    this.MovePlayer(1);
        //}
        //if (GUILayout.Button("Previous Checkpoint", new GUILayoutOption[0]))
        //{
        //    this.MovePlayer(-1);
        //}
        //GUILayout.EndHorizontal();
        // GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        if (GUILayout.Button("Finish Level", new GUILayoutOption[0]))
        {
            LevelComplete();
        }
        //   GUILayout.EndHorizontal();
        //GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        //if (GUILayout.Button("Quit", new GUILayoutOption[0]))
        //{
        //    this.OnQuit();
        //}
        //GUILayout.EndHorizontal();
    }
    public float totalDistance, distance;
    public int prevDis;
    private void LateUpdate()
    {
        if (!IsGameStart)
        {
            return;
        }
        if (IsGameOver || isLevelDone)
        {
            return;
        }
        if (activeBus)
        {
            distance = activeBus.rigid.velocity.magnitude * Time.deltaTime;
            totalDistance += distance;
            if ((int)totalDistance > prevDis)
            {
                prevDis = (int)totalDistance;
                GameManager.AchievementManager.AddAchievementProgress("SUPER_DRIVE", 1);
            }

            if (IsStuckOrFell(out stuck))
            {
                fallElapsed += Time.deltaTime;
                if (fallElapsed >= fallTime)
                {
                    fallElapsed = 0;
                    GameOver(StringManager.YOUFELL);
                }
            }
            else
            {
                fallElapsed = 0;
            }

            if (FuelAmount <= 0)
            {
                GameOver(StringManager.NOFUEL);
            }

        }


    }
    #endregion



    #region private Methods
    void PauseResume(bool val)
    {
        if (val)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;

        }
    }
    void EvaluateComponentValues()
    {
        EmotionAmount = 1f;
        ActiveServing = false;
        fuelToggleBtn.SetActive(false);

        DisableExtraUI();
        GameManager.UiManager.currencyManager.UpdateCurrenncyTexts();

    }
    bool IsStuckOrFell(out bool stuck)
    {
        int g = 0;
        if (!activeBus.FrontLeftWheelCollider.isGrounded)
        {
            g++;
        }
        if (!activeBus.FrontRightWheelCollider.isGrounded)
        {
            g++;
        }
        if (!activeBus.RearRightWheelCollider.isGrounded)
        {
            g++;
        }
        if (!activeBus.RearLeftWheelCollider.isGrounded)
        {
            g++;
        }
        if (g == 2)
        {
            stuck = true;
        }
        if (g > 2)
        {
            stuck = false;
            return true;
        }
        stuck = false;
        return false;
    }

    public void RefreshFuelRates()
    {
        int costOf25 = (int)(activeBus.fuelTankCapacity / 4) * fuelCost;
        int fullCost = (int)(activeBus.fuelTankCapacity - activeBus.FuelTank) * fuelCost;
        fuel25Button.GetComponentInChildren<Text>().text = String.Format("FILL 25%" + "\n{0}?", costOf25);
        fuelFullBuyBtn.GetComponentInChildren<Text>().text = String.Format("FILL FULL" + "\n{0}?", fullCost);
    }
    public void BuyFuel(int _defaulFill)
    {
        int fueltoAdd;
        if (_defaulFill < 0)
        {
            _defaulFill = (int)(activeBus.fuelTankCapacity - activeBus.FuelTank);
        }
        int totalcost = _defaulFill * fuelCost;

        if (totalcost > GameManager.UiManager.currencyManager.GetCoinsCount())
        {
            GameManager.Notification.DisplayNotification("Insufficient Balance");
            insufficientBalancePanel.SetActive(true);
            return;
        }
        Debug.Log("fueel to add == " + _defaulFill);
        fueltoAdd = (int)activeBus.FuelTank + _defaulFill + 1;
        GameManager.AchievementManager.AddAchievementProgress("FUEL_SPENDER", totalcost);
        GameManager.UiManager.currencyManager.RemoveCoins(totalcost);
        Debug.Log("Fueel to be there now ===" + fueltoAdd);
        StartCoroutine(FuelTransition(fueltoAdd));
        RefreshFuelRates();
    }
    void OnYesBtn()
    {
        fuelToggleBtn.GetComponent<Toggle>().isOn = false;
        GameManager.UiManager.OpenStoreMenu();
    }
    void OnNoBtn()
    {
        insufficientBalancePanel.SetActive(false);
    }
    IEnumerator FuelTransition(int value)
    {
        yield return null;

        IsFilling = true;
        while (activeBus.FuelTank < value)
        {

            activeBus.FuelTank += refillSpeed * Time.unscaledDeltaTime;
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }
        GameManager.Notification.DisplayNotification("FUEL FILLED SUCCESSFUL!");
        IsFilling = false;
    }
    public void ShowMainHUD(bool value, bool fullDisplay = true)
    {
        timerText.gameObject.SetActive(value);
        emotionImage.SetActive(value);
        fuelImage.SetActive(value);
        if (activeBus)
        {
            activeBus.useFuelConsumption = value;
        }

        miniCamUi.SetActive(value);
        //fpsRearCam.SetActive(value);
        if (fullDisplay)
        {
            RCC_SceneManager.Instance.activePlayerCanvas.displayType = RCC_UIDashboardDisplay.DisplayType.Full;

        }
        else
        {
            RCC_SceneManager.Instance.activePlayerCanvas.displayType = RCC_UIDashboardDisplay.DisplayType.Off;
        }
    }
    public void EnableExtraUI()
    {
        // SetInfoText(active: true);
        timerText.gameObject.SetActive(true);
        emotionImage.SetActive(true);
        fuelImage.SetActive(true);
        miniCamUi.SetActive(true);
        //fpsRearCam.SetActive(true);
    }
    public void DisableExtraUI()
    {
        Debug.Log("Disabled Extra UI");
        ShowButton(closeDoorBtn, false);
        ShowButton(openDoorBtn, false);
        SetInfoText(active: false);
        timerText.gameObject.SetActive(false);
        emotionImage.SetActive(false);
        fuelImage.SetActive(false);
        miniCamUi.SetActive(false);
        fpsRearCam.SetActive(false);
        insufficientBalancePanel.SetActive(false);
        fuelPanel.SetActive(false);
        fuelToggleBtn.SetActive(false);
    }
    void ResetTimer()
    {
        elapsedTime = 0;
        if (timerText != null)
            timerText.text = "0:00";
    }
    void ChangeToForwardGear()
    {
        gearUi.GearSlider = 0f;
        activeBus.rigid.isKinematic = true;
    }


    void CalculateScore()
    {
        comfortScore = (int)(emotionBar.fillAmount * 100);
        if (comfortScore > 98)
        {
            GameManager.AchievementManager.AddAchievementProgress("NO_CRASH_1", 1);
            GameManager.AchievementManager.AddAchievementProgress("NO_CRASH_10", 1);
        }

        cashEarned += 500 + (CurrentLevelNum * 150);
        timeBonus = (int)(Mathf.InverseLerp(MaxTime, 0, elapsedTime) * MaxTimeBonus);
        servingBonus = (int)(ServeAmount * 100);

        totalEarned = timeBonus + cashEarned + comfortScore + servingBonus;
        GameManager.UiManager.DisplayScore(GameManager.PassengerManager.noOfPassengerToCreate, elapsedTime, cashEarned, comfortScore, timeBonus, servingBonus, totalEarned);

    }

    void SetInfoText(bool active = true, string val = "")
    {
        infoImage.gameObject.SetActive(active);
        infoText.text = val;
    }

    void OpenDoor()
    {
        if (extraTag != null)
            extraTag.OpenDoor();
        var passList = GameManager.PassengerManager.passengersList;
        switch (modeofStop)
        {
            case BusStopMode.PICKUP:

                foreach (var item in passList)
                {
                    item.thisTransform.SetParent(passengersParent.transform);
                }
                if (CurrentLevel != null)
                {
                    CurrentLevel.pickUp.DisableArea();
                    CurrentLevel.dropOff.EnableArea();
                }

                break;
            case BusStopMode.DROPOFF:
                // currentLevel.dropOff.DisableArea();
                break;
            case BusStopMode.NONE:
                break;
            default:
                break;
        }

        ShowButton(openDoorBtn, false);
        //ShowPlayerControls(false);
        StartCoroutine(IwaitForPassengers()); // waits till Passengers Gets in the bus
    }


    void CloseDoor()
    {
        Debug.Log("close Clicked");
        ShowButton(closeDoorBtn, false);
        if (extraTag)
            extraTag.CloseDoor();
        StartCoroutine(IDelayCloseDoor());

        IEnumerator IDelayCloseDoor()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            if (modeofStop == BusStopMode.PICKUP)
            {
                if (timerText != null)
                    StartCoroutine(IStartTimer());
            }
            ShowPlayerControls(true);
            isInArea = false;
        }

    }

    void ShowButton(Button button, bool state = true)
    {
        //Debug.Log(button.name + "=>" + state);
        button.gameObject.SetActive(state);

        if (button == openDoorBtn)
        {
            SetInfoText(active: state, val: StringManager.OPENDOOR);
            //if (state)
            //    SetUserInterFace(false);
        }
        else
        {
            SetInfoText(active: state, val: StringManager.CLOSEDOOR);
        }

    }

    public void ShowPlayerControls(bool flag)
    {
        if (flag)
        {
            miniCamUi.SetActive(true);
            Debug.Log("full");
            RCC_SceneManager.Instance.activePlayerCanvas.displayType = RCC_UIDashboardDisplay.DisplayType.Full;
            switch (prevCamMode)
            {
                case RCC_Camera.CameraMode.TPS:
                    RCC_SceneManager.Instance.activePlayerCamera.cameraMode = RCC_Camera.CameraMode.TPS;
                    break;
                case RCC_Camera.CameraMode.FPS:

                    RCC_SceneManager.Instance.activePlayerCamera.cameraMode = RCC_Camera.CameraMode.FPS;
                    break;
                case RCC_Camera.CameraMode.TOP:
                    RCC_SceneManager.Instance.activePlayerCamera.cameraMode = RCC_Camera.CameraMode.TOP;
                    break;
                default:
                    break;
            }
        }
        else
        {
            miniCamUi.SetActive(false);
            Debug.Log("off");
            RCC_SceneManager.Instance.activePlayerCanvas.displayType = RCC_UIDashboardDisplay.DisplayType.Off;
            if (RCC_SceneManager.Instance.activePlayerCamera.cameraMode != RCC_Camera.CameraMode.WHEEL)
                prevCamMode = RCC_SceneManager.Instance.activePlayerCamera.cameraMode;
            RCC_SceneManager.Instance.activePlayerCamera.cameraMode = RCC_Camera.CameraMode.WHEEL;
        }

    }

    void LevelComplete()
    {
        GameManager.UiManager.OpenMenuWithEnum(EUI_Menus.levelComplete);
        GameManager.UiManager.UnlockLevel(CurrentLevelNum + 1);
        CalculateScore();
        IsPickUp = false;
        IsDropOff = false;

        IsGameOver = false;
        isLevelDone = true;
        IsGameStart = false;
        DisableExtraUI();
        StopGameFlow();
    }
    void GetLevelAreas()
    {

        if (GameManager.UiManager.freeMode)
        {
            GameManager.LevelManager.SetToFreeMode(activeBus.transform);
            GameManager.PassengerManager.DestroyPassengers();
            return;
        }
        if (activeBus)
            GameManager.LevelManager.SetCurrentLevel(activeBus.transform);

        GameManager.LevelManager.DisableOtherLevels();
        if (!GameManager.UiManager.freeMode)
            CurrentLevel.pickUp.EnableArea();

    }

    #endregion privateMethods


    #region Public Methods

    public void SelectedVehicle(RCC_CarControllerV3 bus)
    {

        activeBus = bus;
        activeBus.semiAutomaticGear = true;
        activeBus.rigid.isKinematic = false;
        activeBus.fuelConsumptionRate = fuelConsumptionRate;
        activeBus.FuelTank = activeBus.fuelTankCapacity;
        extraTag = activeBus.GetComponent<ExtraTagObjectContainer>();
        if (!frontDoor)
        {
            frontDoor = extraTag.frontDoor; // GameObject.FindGameObjectWithTag(StringManager.FRONTDOOR);
            extraTag.CloseDoor();
        }

        if (!passengersParent)
            passengersParent = GameObject.FindGameObjectWithTag(StringManager.PASSENGERPARENT);

        RCC_SceneManager.Instance.activePlayerVehicle.semiAutomaticGear = true;
        RCC_SceneManager.Instance.activePlayerVehicle.useDamage = false;
        RCC_SceneManager.Instance.activePlayerVehicle.useWheelDamage = false;
        GetLevelAreas();

        if (GameManager.UiManager.freeMode)
        {
            return;
        }
        if (activeBus)
        {
            Invoke(nameof(ChangeToForwardGear), 1f);
            StartCoroutine(GameManager.PassengerManager.InitiateBusPoints());
        }
        ShowMainHUD(true, true);
        // EnableExtraUI();
    }

    public void RestartGame()
    {
        StartGame();
    }


    public void StopGameFlow()
    {
        activeBus = null;
        ActiveServing = false;
        StopCoroutine(StartServing());
        fuelToggleBtn.SetActive(false);
        if (extraTag)
            extraTag.CloseDoor();
        StartCoroutine(DelayStopGame());

    }
    public IEnumerator DelayStopGame()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        ShowMainHUD(false);
        DisableExtraUI();
        ShowPlayerControls(false);
        carSelectionExample.DeSelectVehicle();
    }
    public void StartGame(int i = -1)
    {
        IsGameStart = true;
        ActiveServing = false;
        EvaluateComponentValues();
        RCC_SceneManager.Instance.activePlayerCamera.cameraMode = RCC_Camera.CameraMode.TPS;
        GameManager.UiManager.StartLoading = true;

        isInArea = false;
        ShowPlayerControls(true);


        GameManager.UiManager.HideAllMenus();

        IsGameOver = false;
        fallElapsed = 0;
        carSelectionExample.SelectVehicle(PlayerPrefs.GetInt(StringManager.PLAYERINDEX));

        if (i != -1)
        {
            CurrentLevelNum = i;
        }

        IsPickUp = false;
        IsDropOff = false;
        isLevelDone = false;
        if (GameManager.UiManager.freeMode)
        {
            ShowMainHUD(false, true);
            //  DisableExtraUI();
            fuelImage.SetActive(true);
            fuelBar.fillAmount = 1f;
            fuelBar.color = gradient.Evaluate(fuelBar.fillAmount);
            return;
        }


        ResetTimer();
        ShowMainHUD(true, true);

        // EnableExtraUI();
        EmotionAmount = 1f;
    }

    public void WaitTillPickUp(BusStopMode _mode)
    {
        modeofStop = _mode;
        StartCoroutine(IWaitTillBusStops());
    }

    public void ApplyDamage(float damageValue)
    {
        Debug.Log("Applied Damage => " + damageValue);
        EmotionAmount -= damageValue;
        //emotionBar.fillAmount -= damageValue;
        //emotionBar.color = gradient.Evaluate(emotionBar.fillAmount);
        if (EmotionAmount == 0)
        {
            GameOver(StringManager.YOUCRASHED);
        }
    }

    public void GameOver(string _gameOverReason)
    {
        if (activeBus)
        {
            activeBus.KillEngine();
        }
        StopGameFlow();
        IsGameOver = true;
        isLevelDone = false;
        IsGameStart = false;
        ActiveServing = false;
        IsPickUp = false;
        IsDropOff = false;
        Time.timeScale = 0;
        GameManager.UiManager.GameOver(_gameOverReason);


    }
    public void NextLevel()
    {
        CurrentLevelNum += 1;
        if (CurrentLevelNum > GameManager.LevelManager.TotalLevels - 1)
        {
            CurrentLevelNum = 0;
        }
        StartGame();
    }
    #endregion



    #region Coroutines
    IEnumerator IStartTimer()
    {
        StartCoroutine(StartServing());
        while (IsPickUp)
        {
            elapsedTime += Time.deltaTime;
            timeSpan = TimeSpan.FromSeconds(elapsedTime);
            timerText.text = string.Format("{0}:{1}", timeSpan.Minutes, timeSpan.Seconds.ToString("00"));
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public IEnumerator IWaitTillBusStops()
    {

        if (isLevelDone || IsGameOver)
        {
            yield break;
        }


        bool busstopped = false;

        while (!busstopped)
        {
            yield return new WaitForSeconds(0.2f);

            if (!isInArea)
            {
                yield break;
            }
            if ((activeBus.speed <= 0.1f))
            {
                if (!IsPickUp)
                    GameManager.PassengerManager.SetParentsAttributes(activeBus.transform/*, reposition: true*/);
                if (IsPickUp)
                    //if (!IsDropOff)
                    GameManager.PassengerManager.SetParentsAttributes(CurrentLevel.dropOff.transform);
                busstopped = true;
            }
        }
        if (activeBus)
            activeBus.rigid.isKinematic = true;
        ShowPlayerControls(false);
        ShowButton(openDoorBtn, true);

        yield break;
    }

    IEnumerator IwaitForPassengers()
    {
        SetInfoText(val: StringManager.WAITFORPASSENGER);
        switch (modeofStop)
        {
            case BusStopMode.PICKUP:

                Debug.Log("wait picking");
                GameManager.PassengerManager.DisableColliders();

                yield return StartCoroutine(GameManager.PassengerManager.ICallGetInPassengers());

                // yield return StartCoroutine(GameManager.PassengerManager.CheckPassengerStatus(checkPick: true));
                IsPickUp = true;
                //currentLevel.pickUp.DisableArea();
                Debug.Log("Done picking");
                GameManager.AchievementManager.AddAchievementProgress("ONE_PICKUP", 1);

                break;

            case BusStopMode.DROPOFF:
                Debug.Log("wait droppiing");

                yield return StartCoroutine(GameManager.PassengerManager.ICallGetOffPassengers());

                //  yield return StartCoroutine(GameManager.PassengerManager.CheckPassengerStatus(checkPick: false));
                // GameManager.PassengerManager.EnableColliders();
                IsDropOff = true;

                GameManager.AchievementManager.AddAchievementProgress("ONE_DROPOFF", 1);

                Debug.Log("done droppiing");
                break;
            case BusStopMode.NONE:
                break;
            default:
                break;
        }


        ShowButton(closeDoorBtn, true);

        if (activeBus)
            activeBus.rigid.isKinematic = false;

        yield return null;
    }
    #endregion



    #region




    #endregion


}//mainClass


