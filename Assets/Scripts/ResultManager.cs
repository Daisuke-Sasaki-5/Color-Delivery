using UnityEngine;
using TMPro;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    [Header("表示UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject[] showObjects; // 順番に出すオブジェクト
    [SerializeField] private float interval = 0.5f;    // 表示間隔
     void Start()
    {
        Debug.Log($"[Start] Time.timeScale = {Time.timeScale}");
        // 最初は全部表示
        foreach (var obj in showObjects)obj.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // スコア表示更新
        if(scoreText != null) scoreText.text = "SCORE :" + ScoreManager.instance.Score;

        // 順番表示
        StartCoroutine(ShowUI());
    }

    private IEnumerator ShowUI()
    {
        Time.timeScale = 1f;
        foreach (var obj in showObjects)
        {
            obj.SetActive(true);
            yield return new WaitForSeconds(interval);
        }
    }

    // ==== ボタン処理 ====
    public void OnRetryButton()
    {
        // スコアリセット
        GameManager.instance.ResetScore();

        Time.timeScale = 1;

        // ゲームシーンをロード
        SceneManager.LoadScene("GameScene");
    }

    public void OnTitleButton()
    {
        // スコアリセット
        GameManager.instance.ResetScore();

        Time.timeScale = 1;

        // タイトルシーンをロード
        SceneManager.LoadScene("TitleScene");
    }
}
