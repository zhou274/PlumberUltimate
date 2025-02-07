

using MS;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TTSDK.UNBridgeLib.LitJson;
using TTSDK;
using StarkSDKSpace;
using System.Collections.Generic;
using UnityEngine.Analytics;

public class GameScene : MonoBehaviour
{
	[Header("Menu")]
	public Text coinLbl;

	public TextMeshProUGUI titleLevelGroupLbl;

	public Text titleLevelNoLbl;

	public Image titleBG;

	public Popup gameOverPopup;

	public Text rewardValueText;

	public Text coinForHintText;

	public GameObject skipCoinObj;

	public GameObject skipTextObj;

	public Text skipCoinValueText;

	public static GameScene instance;

    public string clickid;
    private StarkAdManager starkAdManager;
    private void Start()
	{
		instance = this;
		rewardValueText.text = "+" + GameConfig.instance.rewardedVideoAmount;
		coinForHintText.text = GameConfig.instance.numCoinForHint.ToString();
		skipCoinValueText.text = GameConfig.instance.numCoinForSkipGame.ToString();
		Music.instance.Play(Music.Type.MainMusic);
	}

	public void UpdateUI()
	{
		coinLbl.text = GameManager.Coin + string.Empty;
		titleLevelGroupLbl.text = GameManager.currentLevelGroup.LevelGroupName;
		titleLevelNoLbl.text = GameManager.CurrentLevelNo + string.Empty;
		titleLevelNoLbl.color = GameManager.currentLevelGroup.bgColor;
		titleBG.sprite = GameManager.currentLevelGroup.LevelHeaderBG;
	}

	public void OnBackBtn()
	{
		if (!GamePlayManager.instance.isGameOver || GamePlayManager.instance.closeGameOver)
		{
			Sound.instance.PlayButton(Sound.Button.Back);
			GameManager.openLevelSelection = true;
			SceneManager.LoadScene("HomeScene");
		}
	}

	public void ShowMenuPopup()
	{
		bool flag = GameManager.CurrentLevelNo > GameManager.currentLevelGroup.CompletedLevel;
		skipCoinObj.SetActive(flag);
		Transform transform = skipTextObj.transform;
		float x = (!flag) ? 12 : (-19);
		Vector3 localPosition = skipTextObj.transform.localPosition;
		transform.localPosition = new Vector3(x, localPosition.y);
		Sound.instance.PlayButton();
	}

	public void OnGameOverCloseBtn()
	{
		Sound.instance.PlayButton(Sound.Button.Back);
		gameOverPopup.Close();
		GamePlayManager.instance.closeGameOver = true;
	}

	public void OnHomeBtn()
	{
		if (!GamePlayManager.instance.isGameOver || GamePlayManager.instance.closeGameOver)
		{
			Sound.instance.PlayButton();
			SceneManager.LoadScene("HomeScene");
		}
	}

	public void OnVideoRewarded()
	{
		int rewardedVideoAmount = GameConfig.instance.rewardedVideoAmount;
		GameManager.Coin += rewardedVideoAmount;
		UpdateUI();
		Toast.instance.ShowMessage($"You got {rewardedVideoAmount} free coins");
	}

	public void OnHintBtn()
	{
        ShowVideoAd("192if3b93qo6991ed0",
            (bol) => {
                if (bol)
                {

                    if (!GamePlayManager.instance.isGameOver)
                    {
                        Sound.instance.PlayButton();

                        UpdateUI();
                        GamePlayManager.instance.GiveHint();
                    }


                    clickid = "";
                    getClickid();
                    apiSend("game_addiction", clickid);
                    apiSend("lt_roi", clickid);


                }
                else
                {
                    StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
                }
            },
            (it, str) => {
                Debug.LogError("Error->" + str);
                //AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
            });
        
	}

	public void OnUndoBtn()
	{
		if (!GamePlayManager.instance.isGameOver)
		{
			Sound.instance.PlayButton();
			GamePlayManager.instance.Undo();
		}
	}

	public void OnRestart()
	{
		if (!GamePlayManager.instance.isGameOver || GamePlayManager.instance.closeGameOver)
		{
			Sound.instance.PlayButton();
			SceneManager.LoadScene("GameScene");
		}
	}

	public void PlayButton()
	{
		Sound.instance.PlayButton();
	}

	public void PlayBackButton()
	{
		Sound.instance.PlayButton(Sound.Button.Back);
	}
    public void getClickid()
    {
        var launchOpt = StarkSDK.API.GetLaunchOptionsSync();
        if (launchOpt.Query != null)
        {
            foreach (KeyValuePair<string, string> kv in launchOpt.Query)
                if (kv.Value != null)
                {
                    Debug.Log(kv.Key + "<-参数-> " + kv.Value);
                    if (kv.Key.ToString() == "clickid")
                    {
                        clickid = kv.Value.ToString();
                    }
                }
                else
                {
                    Debug.Log(kv.Key + "<-参数-> " + "null ");
                }
        }
    }

    public void apiSend(string eventname, string clickid)
    {
        TTRequest.InnerOptions options = new TTRequest.InnerOptions();
        options.Header["content-type"] = "application/json";
        options.Method = "POST";

        JsonData data1 = new JsonData();

        data1["event_type"] = eventname;
        data1["context"] = new JsonData();
        data1["context"]["ad"] = new JsonData();
        data1["context"]["ad"]["callback"] = clickid;

        Debug.Log("<-data1-> " + data1.ToJson());

        options.Data = data1.ToJson();

        TT.Request("https://analytics.oceanengine.com/api/v2/conversion", options,
           response => { Debug.Log(response); },
           response => { Debug.Log(response); });
    }


    /// <summary>
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="closeCallBack"></param>
    /// <param name="errorCallBack"></param>
    public void ShowVideoAd(string adId, System.Action<bool> closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            starkAdManager.ShowVideoAdWithId(adId, closeCallBack, errorCallBack);
        }
    }
}
