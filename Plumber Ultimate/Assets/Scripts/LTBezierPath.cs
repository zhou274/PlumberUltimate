/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using UnityEngine;

public class LTBezierPath
{
	public Vector3[] pts;

	public float length;

	public bool orientToPath;

	public bool orientToPath2d;

	private LTBezier[] beziers;

	private float[] lengthRatio;

	private int currentBezier;

	private int previousBezier;

	public float distance => length;

	public LTBezierPath()
	{
	}

	public LTBezierPath(Vector3[] pts_)
	{
		setPoints(pts_);
	}

	public void setPoints(Vector3[] pts_)
	{
		if (pts_.Length < 4)
		{
			LeanTween.logError("LeanTween - When passing values for a vector path, you must pass four or more values!");
		}
		if (pts_.Length % 4 != 0)
		{
			LeanTween.logError("LeanTween - When passing values for a vector path, they must be in sets of four: controlPoint1, controlPoint2, endPoint2, controlPoint2, controlPoint2...");
		}
		pts = pts_;
		int num = 0;
		beziers = new LTBezier[pts.Length / 4];
		lengthRatio = new float[beziers.Length];
		length = 0f;
		for (int i = 0; i < pts.Length; i += 4)
		{
			beziers[num] = new LTBezier(pts[i], pts[i + 2], pts[i + 1], pts[i + 3], 0.05f);
			length += beziers[num].length;
			num++;
		}
		for (int i = 0; i < beziers.Length; i++)
		{
			lengthRatio[i] = beziers[i].length / length;
		}
	}

	public Vector3 point(float ratio)
	{
		float num = 0f;
		for (int i = 0; i < lengthRatio.Length; i++)
		{
			num += lengthRatio[i];
			if (num >= ratio)
			{
				return beziers[i].point((ratio - (num - lengthRatio[i])) / lengthRatio[i]);
			}
		}
		return beziers[lengthRatio.Length - 1].point(1f);
	}

	public void place2d(Transform transform, float ratio)
	{
		transform.position = point(ratio);
		ratio += 0.001f;
		if (ratio <= 1f)
		{
			Vector3 vector = point(ratio) - transform.position;
			float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			transform.eulerAngles = new Vector3(0f, 0f, z);
		}
	}

	public void placeLocal2d(Transform transform, float ratio)
	{
		transform.localPosition = point(ratio);
		ratio += 0.001f;
		if (ratio <= 1f)
		{
			Vector3 vector = point(ratio) - transform.localPosition;
			float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			transform.localEulerAngles = new Vector3(0f, 0f, z);
		}
	}

	public void place(Transform transform, float ratio)
	{
		place(transform, ratio, Vector3.up);
	}

	public void place(Transform transform, float ratio, Vector3 worldUp)
	{
		transform.position = point(ratio);
		ratio += 0.001f;
		if (ratio <= 1f)
		{
			transform.LookAt(point(ratio), worldUp);
		}
	}

	public void placeLocal(Transform transform, float ratio)
	{
		placeLocal(transform, ratio, Vector3.up);
	}

	public void placeLocal(Transform transform, float ratio, Vector3 worldUp)
	{
		ratio = Mathf.Clamp01(ratio);
		transform.localPosition = point(ratio);
		ratio = Mathf.Clamp01(ratio + 0.001f);
		if (ratio <= 1f)
		{
			transform.LookAt(transform.parent.TransformPoint(point(ratio)), worldUp);
		}
	}

	public void gizmoDraw(float t = -1f)
	{
		Vector3 to = point(0f);
		for (int i = 1; i <= 120; i++)
		{
			float ratio = (float)i / 120f;
			Vector3 vector = point(ratio);
			Gizmos.color = ((previousBezier != currentBezier) ? Color.grey : Color.magenta);
			Gizmos.DrawLine(vector, to);
			to = vector;
			previousBezier = currentBezier;
		}
	}
}
