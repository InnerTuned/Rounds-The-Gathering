using System;
using System.Collections;
using SoundImplementation;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000081 RID: 129
public class MapTransition : MonoBehaviour
{
	// Token: 0x060002BB RID: 699 RVA: 0x00011691 File Offset: 0x0000F891
	private void Awake()
	{
		MapTransition.instance = this;
	}

	// Token: 0x060002BC RID: 700 RVA: 0x000027C8 File Offset: 0x000009C8
	private void Start()
	{
	}

	// Token: 0x060002BD RID: 701 RVA: 0x00011699 File Offset: 0x0000F899
	public void SetStartPos(Map map)
	{
		map.transform.position = Vector3.right * 90f;
	}

	// Token: 0x060002BE RID: 702 RVA: 0x000116B5 File Offset: 0x0000F8B5
	public void Enter(Map map)
	{
		this.MoveObject(map.gameObject, Vector3.zero);
		base.StartCoroutine(this.DelayEvent(0.1f));
		SoundPlayerStatic.Instance.PlayLevelTransitionIn();
		SoundMusicManager.Instance.PlayIngame(false);
	}

	// Token: 0x060002BF RID: 703 RVA: 0x000116EF File Offset: 0x0000F8EF
	public void Exit(Map map)
	{
		SoundPlayerStatic.Instance.PlayLevelTransitionOut();
		map.MapMoveOut();
		this.MoveObject(map.gameObject, Vector3.right * -90f);
		base.StartCoroutine(this.ClearObjectsAfterSeconds(1f));
	}

	// Token: 0x060002C0 RID: 704 RVA: 0x00011730 File Offset: 0x0000F930
	private void MoveObject(GameObject target, Vector3 targetPos)
	{
		for (int i = 0; i < target.transform.childCount; i++)
		{
			this.Toggle(target.transform.GetChild(i).gameObject, false);
			base.StartCoroutine(this.Move(target.transform.GetChild(i).gameObject, targetPos - target.transform.position, (i == 0) ? target.GetComponent<Map>() : null));
		}
	}

	// Token: 0x060002C1 RID: 705 RVA: 0x000117A8 File Offset: 0x0000F9A8
	private void Toggle(GameObject obj, bool enabled)
	{
		Rigidbody2D component = obj.GetComponent<Rigidbody2D>();
		if (component)
		{
			component.simulated = enabled;
			if (enabled)
			{
				base.StartCoroutine(this.LerpDrag(component));
			}
		}
		Collider2D component2 = obj.GetComponent<Collider2D>();
		if (component2)
		{
			component2.enabled = enabled;
		}
		CodeAnimation component3 = obj.GetComponent<CodeAnimation>();
		if (component3)
		{
			component3.enabled = enabled;
		}
	}

	// Token: 0x060002C2 RID: 706 RVA: 0x00011808 File Offset: 0x0000FA08
	private IEnumerator DelayEvent(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		this.switchMapEvent.Invoke();
		yield break;
	}

	// Token: 0x060002C3 RID: 707 RVA: 0x0001181E File Offset: 0x0000FA1E
	private IEnumerator Move(GameObject target, Vector3 distance, Map targetMap = null)
	{
		MapTransition.isTransitioning = true;
		float maxRandomDelay = 0.25f;
		float randomDelay = Random.Range(0f, maxRandomDelay);
		yield return new WaitForSecondsRealtime(randomDelay);
		Vector3 targetStartPos = target.transform.position;
		float t = this.curve.keys[this.curve.keys.Length - 1].time;
		float c = 0f;
		while (c < t)
		{
			c += Time.unscaledDeltaTime;
			target.transform.position = targetStartPos + distance * this.curve.Evaluate(c);
			yield return null;
		}
		target.transform.position = targetStartPos + distance;
		this.Toggle(target, true);
		yield return new WaitForSecondsRealtime(maxRandomDelay - randomDelay);
		MapTransition.isTransitioning = false;
		if (targetMap)
		{
			targetMap.hasEntered = true;
		}
		yield break;
	}

	// Token: 0x060002C4 RID: 708 RVA: 0x00011842 File Offset: 0x0000FA42
	private IEnumerator LerpDrag(Rigidbody2D target)
	{
		target.gravityScale = 0f;
		float t = this.gravityCurve.keys[this.gravityCurve.keys.Length - 1].time;
		float c = 0f;
		while (c < t && target)
		{
			target.gravityScale = this.gravityCurve.Evaluate(c);
			c += Time.unscaledDeltaTime;
			yield return null;
		}
		if (target)
		{
			target.gravityScale = 1f;
		}
		yield break;
	}

	// Token: 0x060002C5 RID: 709 RVA: 0x00011858 File Offset: 0x0000FA58
	private IEnumerator ClearObjectsAfterSeconds(float time)
	{
		yield return new WaitForSecondsRealtime(time);
		this.ClearObjects();
		yield break;
	}

	// Token: 0x060002C6 RID: 710 RVA: 0x00011870 File Offset: 0x0000FA70
	public void ClearObjects()
	{
		RemoveAfterSeconds[] array = Object.FindObjectsOfType<RemoveAfterSeconds>();
		for (int i = 0; i < array.Length; i++)
		{
			Object.Destroy(array[i].gameObject);
		}
	}

	// Token: 0x040003D7 RID: 983
	[Header("Settings")]
	public UnityEvent switchMapEvent;

	// Token: 0x040003D8 RID: 984
	public AnimationCurve curve;

	// Token: 0x040003D9 RID: 985
	public AnimationCurve gravityCurve;

	// Token: 0x040003DA RID: 986
	public static MapTransition instance;

	// Token: 0x040003DB RID: 987
	private const float mapPadding = 90f;

	// Token: 0x040003DC RID: 988
	public static bool isTransitioning;
}
