
using System;
using UnityEngine;

[Serializable]
public class Admob
{
	[Header("Banner")]
	public string androidBanner;

	public string iosBanner;

	[Header("Interstitial")]
	public string androidInterstitial;

	public string iosInterstitial;

	[Header("RewardedVideo")]
	public string androidRewarded;

	public string iosRewarded;
}
