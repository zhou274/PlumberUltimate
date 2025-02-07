

using GoogleMobileAds.Api;
using System;
using UnityEngine;

public class RewardedVideoGroup : MonoBehaviour
{
	public GameObject buttonGroup;

	public GameObject textGroup;

	public TimerText timerText;

	private const string ACTION_NAME = "rewarded_video";

	private void Start()
	{
		if (timerText != null)
		{
			TimerText obj = timerText;
			obj.onCountDownComplete = (Action)Delegate.Combine(obj.onCountDownComplete, new Action(OnCountDownComplete));
		}
		Timer.Schedule(this, 0.1f, AddEvents);
		if (!IsAvailableToShow())
		{
			buttonGroup.SetActive(value: false);
			if (IsAdAvailable() && !IsActionAvailable())
			{
				int time = (int)((double)GameConfig.instance.rewardedVideoPeriod - CUtils.GetActionDeltaTime("rewarded_video"));
				ShowTimerText(time);
			}
		}
		InvokeRepeating("IUpdate", 1f, 1f);
	}

	private void AddEvents()
	{
		if (AdmobController.instance.rewardBasedVideo != null)
		{
			AdmobController.instance.rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
		}
	}

	private void IUpdate()
	{
		buttonGroup.SetActive(IsAvailableToShow());
	}

	public void OnClick()
	{
		AdmobController.instance.ShowRewardBasedVideo();
		Sound.instance.PlayButton();
	}

	private void ShowTimerText(int time)
	{
		if (textGroup != null)
		{
			textGroup.SetActive(value: true);
			timerText.SetTime(time);
			timerText.Run();
		}
	}

	public void HandleRewardBasedVideoRewarded(object sender, Reward args)
	{
		buttonGroup.SetActive(value: false);
		ShowTimerText(GameConfig.instance.rewardedVideoPeriod);
	}

	private void OnCountDownComplete()
	{
		textGroup.SetActive(value: false);
		if (IsAdAvailable())
		{
			buttonGroup.SetActive(value: true);
		}
	}

	public bool IsAvailableToShow()
	{
		return IsActionAvailable() && IsAdAvailable();
	}

	private bool IsActionAvailable()
	{
		return CUtils.IsActionAvailable("rewarded_video", GameConfig.instance.rewardedVideoPeriod);
	}

	private bool IsAdAvailable()
	{
		if (AdmobController.instance.rewardBasedVideo == null)
		{
			return false;
		}
		return AdmobController.instance.rewardBasedVideo.IsLoaded();
	}

	private void OnDestroy()
	{
		if (AdmobController.instance.rewardBasedVideo != null)
		{
			AdmobController.instance.rewardBasedVideo.OnAdRewarded -= HandleRewardBasedVideoRewarded;
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause && textGroup != null && textGroup.activeSelf)
		{
			int time = (int)((double)GameConfig.instance.rewardedVideoPeriod - CUtils.GetActionDeltaTime("rewarded_video"));
			ShowTimerText(time);
		}
	}
}
