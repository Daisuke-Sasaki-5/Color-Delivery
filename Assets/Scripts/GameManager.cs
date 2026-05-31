using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [Header("スコア管理")]
    [SerializeField] private TMP_Text scoreText;

    [Header("制限時間")]
    [SerializeField] private float timeLimit = 30f; // 制限時間
    [SerializeField] private TMP_Text timetext;
    private float timer;
    private bool isGameOver = false;
    private bool isGameStarted;

    [Header("荷物配列")]
    [SerializeField] private PickupObject[] pickupItems;

    [Header("配達先配列")]
    [SerializeField] private DeliveryPoint[] deliveryPoints;

    private int currentIndex; // 現在運んでいるもの

    [Header("荷物が配置される場所")]
    [SerializeField] private Transform[] pickupSpawnPoints;

    [Header("配達先が配置される場所")]
    [SerializeField] private Transform[] deliverySpawnPoints;

    [SerializeField] private Renderer goalRenderer;

    [Header("スタートUI")]
    [SerializeField] private TextMeshProUGUI readytext;
    [SerializeField] private TextMeshProUGUI gotext;

    // 外部参照用
    public bool IsGameOver => isGameOver;
    public bool IsGameStarted => isGameStarted;

    public ItemType CurrentTargetType
    {
        get { return pickupItems[currentIndex].itemType; }
    }

    private Coroutine startRoutine;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        InitGame();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void InitGame()
    {
        // 現在のシーンをチェック
        if (SceneManager.GetActiveScene().name != "GameScene") return; 

        timer = timeLimit;
        isGameOver = false;
        isGameStarted = false;

        UpdateScoreUI();
        UpdateTimeUI();

        // UI初期化
        if (scoreText != null) scoreText.gameObject.SetActive(false);
        if (timetext != null) timetext.gameObject.SetActive(false);
        if (readytext != null) readytext.gameObject.SetActive(false);
        if (gotext != null) gotext.gameObject.SetActive(false);

        // 既存コルーチンがあれば停止
        if(startRoutine != null)StopCoroutine(startRoutine);
        startRoutine = StartCoroutine(StartGameRoutine());

        StartRound();
    }

    private void Update()
    {
        if(isGameOver)return;

        timer -= Time.deltaTime;
        if(timer <= 0 )
        {
            timer = 0;
            GameOver();
        }
        UpdateTimeUI() ;
    }

    // ==== READY/GO 演出 ====
    // 順番に出現させる
    // Ready→Go→Score、Timer
    private IEnumerator StartGameRoutine()
    {
        // 時間停止
        Time.timeScale = 0f;

        if (FadeManager.instance != null)
        {
            while (!FadeManager.instance.IsFadeComplete)
            {
                yield return null;
            }
        }
        
        // ReadyText出現
        if(readytext != null)
        {
            readytext.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(1f);
            readytext.gameObject.SetActive(false);
        }

        // GoText出現
        if (gotext != null)
        {
            gotext.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(1f);
            gotext.gameObject.SetActive(false);
        }

        // ScoreTextとTimeText出現
        if (scoreText != null) scoreText.gameObject.SetActive(true);
        if (timetext != null) timetext.gameObject.SetActive(true);

        // ゲーム開始
        isGameStarted = true;
        Time.timeScale = 1f;
    }

    /// <summary>
    /// ランダムにお題を設定
    /// </summary>
    private void StartRound()
    {
        // ==== 始めに荷物とゴールを全て非表示にする ====
        foreach(var Item in pickupItems)
        {
            Item.gameObject.SetActive(false);
        }

        foreach(var Goal in deliveryPoints)
        {
            Goal.gameObject.SetActive(false);
        }

        currentIndex = UnityEngine.Random.Range(0,pickupItems.Length);

        // ==== 荷物の場所とゴールの位置をランダムに取得 ====
        int pickupPos = UnityEngine.Random.Range(0,pickupSpawnPoints.Length);
        int goalPos = UnityEngine.Random.Range(0,deliverySpawnPoints.Length);

        PickupObject item = pickupItems[currentIndex];
        DeliveryPoint goal = deliveryPoints[currentIndex];

        // ==== 荷物とゴールの位置を設定 ====
        item.transform.position = pickupSpawnPoints[pickupPos].position;
        goal.transform.position = deliverySpawnPoints[goalPos].position;

        // ==== 選ばれた荷物とゴールの位置を表示 ====
        item.gameObject.SetActive(true);
        goal.gameObject.SetActive(true);

        Debug.Log(pickupItems[currentIndex].name + "を運べ");
    }

    public void CheckDelivery(Player player, DeliveryPoint point)
    {
        if(player.CurrentItem == null) return;

        if(player.CurrentItem == point.itemType)
        {
            player.ClearItem();
            DeliverySuccess();
        }
    }

    public void DeliverySuccess()
    {
        pickupItems[currentIndex].gameObject.SetActive(true);

        Debug.Log("リセット");
        AddScore(100);
        StartRound();
    }

    // ==== スコア関連 ====
    public void AddScore(int amont)
    {
       // ScoreManagerにスコアを送る
        ScoreManager.instance.AddScore(amont);
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score:" + ScoreManager.instance.Score;
        }
    }

    public void ResetScore()
    {
        UpdateScoreUI() ;

        if (ScoreManager.instance != null) ScoreManager.instance.ResetScore();
    }

    // ==== 時間表示 ====
    private void UpdateTimeUI()
    {
        if(timetext != null)
        {
            timetext.text = "Time:" + timer.ToString("F1");
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        Debug.Log("ゲームオーバー");

        // ==== カーソル表示 ====
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        FadeManager.instance.FadeToScene("EndScene");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            //InitGame();
        }
        else
        {
            if (scoreText != null) scoreText.gameObject.SetActive(false);
            if (timetext != null) timetext.gameObject.SetActive(false);
            if (readytext != null) readytext.gameObject.SetActive(false);
            if (gotext != null) gotext.gameObject.SetActive(false);
            Time.timeScale = 1;
        }
    }

    private IEnumerator DelayStart()
    {
        yield return null;
        StartCoroutine(StartGameRoutine());
    }
}
