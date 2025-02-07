
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class LevelGroupState : MonoBehaviour
{
	public LevelGroup levelGroup;

	[Header("Show Detail")]
	[SerializeField]
	private TextMeshProUGUI TitleLbl;

	[SerializeField]
	private Text LevelCompletdLbl;

	[SerializeField]
	private Text AvarageTimeLbl;

	[SerializeField]
	private GameObject AwardGoldImage;

	private void Start()
	{
		UpdateUI();
	}

	private void Update()
	{
	}

	[ContextMenu("UpdateUI")]
	public void UpdateUI()
	{
		base.name = levelGroup.LevelGroupName;
		TitleLbl.text = base.name.ToUpper();
		LevelCompletdLbl.text = levelGroup.CompletedLevel + " / " + levelGroup.TotalLevel;
		AvarageTimeLbl.text = levelGroup.AverageCompletedTimeString;
		AwardGoldImage.SetActive(levelGroup.CompletedLevel >= levelGroup.TotalLevel);
	}
}
