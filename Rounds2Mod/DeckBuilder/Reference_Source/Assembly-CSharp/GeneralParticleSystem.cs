using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000061 RID: 97
public class GeneralParticleSystem : MonoBehaviour
{
	// Token: 0x060001C3 RID: 451 RVA: 0x0000B42F File Offset: 0x0000962F
	private void OnEnable()
	{
		this.isPlaying = false;
		if (this.playOnEnablee)
		{
			this.Play();
		}
	}

	// Token: 0x060001C4 RID: 452 RVA: 0x0000B448 File Offset: 0x00009648
	private void Start()
	{
		if (this.playOnAwake)
		{
			this.Play();
		}
		Mask component = base.transform.parent.GetComponent<Mask>();
		if (component)
		{
			component.showMaskGraphic = false;
		}
	}

	// Token: 0x060001C5 RID: 453 RVA: 0x0000B484 File Offset: 0x00009684
	private void Init()
	{
		if (this.inited)
		{
			return;
		}
		this.inited = true;
		if (this.particleSettings.sizeOverTime.keys.Length > 1)
		{
			this.sizeOverTimeAnimationCurveLength = this.particleSettings.sizeOverTime.keys[this.particleSettings.sizeOverTime.keys.Length - 1].time - this.particleSettings.sizeOverTime.keys[0].time;
		}
		if (this.particleSettings.alphaOverTime.keys.Length > 1)
		{
			this.alphaOverTimeAnimationCurveLength = this.particleSettings.alphaOverTime.keys[this.particleSettings.alphaOverTime.keys.Length - 1].time - this.particleSettings.alphaOverTime.keys[0].time;
		}
		if (this.sizeMultiplierOverTime.keys.Length > 1)
		{
			this.sizeMultiplierOverTimeAnimationCurveLength = this.sizeMultiplierOverTime.keys[this.sizeMultiplierOverTime.keys.Length - 1].time - this.sizeMultiplierOverTime.keys[0].time;
		}
		if (this.emissionMultiplierOverTime.keys.Length > 1)
		{
			this.emissionOverTimeAnimationCurveLength = this.emissionMultiplierOverTime.keys[this.emissionMultiplierOverTime.keys.Length - 1].time - this.emissionMultiplierOverTime.keys[0].time;
		}
		this.particleObject.SetActive(false);
		this.particlePool = new ObjectPool(this.particleObject, 100, base.transform);
	}

	// Token: 0x060001C6 RID: 454 RVA: 0x0000B632 File Offset: 0x00009832
	public void StartLooping()
	{
		this.loop = true;
		this.Play();
	}

	// Token: 0x060001C7 RID: 455 RVA: 0x0000B641 File Offset: 0x00009841
	public void EndLooping()
	{
		this.loop = false;
		if (this.emissionLoop != null)
		{
			base.StopCoroutine(this.emissionLoop);
		}
	}

	// Token: 0x060001C8 RID: 456 RVA: 0x0000B65E File Offset: 0x0000985E
	[Button]
	public void Play()
	{
		this.Init();
		if (!this.isPlaying)
		{
			this.emissionLoop = base.StartCoroutine(this.DoPlay());
		}
	}

	// Token: 0x060001C9 RID: 457 RVA: 0x0000B680 File Offset: 0x00009880
	private void OnDisable()
	{
		this.Stop();
	}

	// Token: 0x060001CA RID: 458 RVA: 0x0000B688 File Offset: 0x00009888
	public void Stop()
	{
		this.isPlaying = false;
		this.DisableAllParticles();
		if (this.emissionLoop != null)
		{
			base.StopCoroutine(this.emissionLoop);
		}
	}

	// Token: 0x060001CB RID: 459 RVA: 0x0000B6AC File Offset: 0x000098AC
	private void DisableAllParticles()
	{
		if (base.transform.childCount > 0)
		{
			for (int i = 0; i < base.transform.GetChild(0).childCount; i++)
			{
				base.transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x060001CC RID: 460 RVA: 0x0000B700 File Offset: 0x00009900
	private IEnumerator DoPlay()
	{
		this.isPlaying = true;
		if (this.startEvent != null)
		{
			if (this.startEventDelay != 0f)
			{
				base.StartCoroutine(this.DelayEvent(this.startEvent, this.startEventDelay));
			}
			else
			{
				this.startEvent.Invoke();
			}
		}
		float counter = 0f;
		while (counter < this.duration)
		{
			this.CheckIfShouldEmit(counter / this.duration);
			counter += (this.useTimeScale ? TimeHandler.deltaTime : Time.unscaledDeltaTime) * (this.simulationSpeed * this.simulationSpeedMultiplier);
			yield return null;
		}
		this.isPlaying = false;
		if (this.loop)
		{
			this.Play();
		}
		else if (this.endEvent != null)
		{
			if (this.endEventDelay != 0f)
			{
				base.StartCoroutine(this.DelayEvent(this.endEvent, this.endEventDelay));
			}
			else
			{
				this.endEvent.Invoke();
			}
		}
		yield break;
	}

	// Token: 0x060001CD RID: 461 RVA: 0x0000B710 File Offset: 0x00009910
	private void CheckIfShouldEmit(float currentAnimationTime)
	{
		if ((this.useTimeScale ? Time.time : Time.unscaledTime) > this.lastEmissionTime + 1f / this.rate / (this.simulationSpeed * this.simulationSpeedMultiplier) / this.emissionMultiplierOverTime.Evaluate(currentAnimationTime * this.emissionOverTimeAnimationCurveLength) * Time.timeScale)
		{
			base.StartCoroutine(this.DoPlarticleLife(currentAnimationTime));
			this.lastEmissionTime = Time.time;
		}
	}

	// Token: 0x060001CE RID: 462 RVA: 0x0000B787 File Offset: 0x00009987
	private IEnumerator DoPlarticleLife(float currentAnimationTime)
	{
		GameObject spawned = this.particlePool.GetObject();
		float counter = 0f;
		float t = this.particleSettings.lifetime;
		Vector3 startSize = spawned.transform.localScale;
		Vector3 modifiedStartSize = spawned.transform.localScale * this.particleSettings.size * this.sizeMultiplierOverTime.Evaluate(currentAnimationTime * this.sizeMultiplierOverTimeAnimationCurveLength);
		Image img = spawned.GetComponent<Image>();
		Color startColor = Color.magenta;
		if (img)
		{
			startColor = img.color;
		}
		if (img)
		{
			float value = Random.value;
			if (this.particleSettings.color != Color.magenta)
			{
				img.color = this.particleSettings.color;
			}
			if (this.particleSettings.randomColor != Color.magenta)
			{
				img.color = Color.Lerp(img.color, this.particleSettings.randomColor, value);
			}
			if (!this.particleSettings.singleRandomValueColor)
			{
				value = Random.value;
			}
			if (this.particleSettings.randomAddedColor != Color.black)
			{
				img.color += Color.Lerp(Color.black, this.particleSettings.randomAddedColor, value);
			}
			if (!this.particleSettings.singleRandomValueColor)
			{
				value = Random.value;
			}
			if (this.particleSettings.randomAddedSaturation != 0f || this.saturationMultiplier != 1f)
			{
				float h;
				float num;
				float v;
				Color.RGBToHSV(img.color, out h, out num, out v);
				num += value * this.particleSettings.randomAddedSaturation;
				num *= this.saturationMultiplier;
				img.color = Color.HSVToRGB(h, num, v);
			}
		}
		spawned.transform.Rotate(base.transform.forward * this.particleSettings.rotation);
		spawned.transform.Rotate(base.transform.forward * Random.Range(-this.particleSettings.randomRotation, this.particleSettings.randomRotation));
		spawned.transform.localPosition = Vector3.zero;
		spawned.transform.position += base.transform.up * Random.Range(-this.randomYPos, this.randomYPos);
		spawned.transform.position += base.transform.right * Random.Range(-this.randomXPos, this.randomXPos);
		spawned.transform.position += base.transform.forward * Random.Range(-0.1f, 0.1f);
		while (counter < t)
		{
			if (this.particleSettings.sizeOverTime.keys.Length > 1)
			{
				spawned.transform.localScale = modifiedStartSize * this.particleSettings.sizeOverTime.Evaluate(counter / t * this.sizeOverTimeAnimationCurveLength);
			}
			float num2 = this.particleSettings.alphaOverTime.Evaluate(counter / t * this.alphaOverTimeAnimationCurveLength);
			if (img && img.color.a != num2)
			{
				img.color = new Color(img.color.r, img.color.g, img.color.b, num2);
			}
			counter += (this.useTimeScale ? TimeHandler.deltaTime : Time.unscaledDeltaTime) * (this.simulationSpeed * this.simulationSpeedMultiplier);
			yield return null;
		}
		if (img)
		{
			img.color = startColor;
		}
		spawned.transform.localScale = startSize;
		this.particlePool.ReleaseObject(spawned);
		yield break;
	}

	// Token: 0x060001CF RID: 463 RVA: 0x0000B79D File Offset: 0x0000999D
	private IEnumerator DelayEvent(UnityEvent e, float t)
	{
		yield return new WaitForSeconds(t);
		if (e != null)
		{
			e.Invoke();
		}
		yield break;
	}

	// Token: 0x0400026A RID: 618
	public GameObject particleObject;

	// Token: 0x0400026B RID: 619
	public bool useTimeScale;

	// Token: 0x0400026C RID: 620
	private ObjectPool particlePool;

	// Token: 0x0400026D RID: 621
	[FoldoutGroup("Spawn settings", 0)]
	public float randomXPos;

	// Token: 0x0400026E RID: 622
	[FoldoutGroup("Spawn settings", 0)]
	public float randomYPos;

	// Token: 0x0400026F RID: 623
	[FoldoutGroup("Spawn settings", 0)]
	public float rate = 10f;

	// Token: 0x04000270 RID: 624
	[FoldoutGroup("Spawn settings", 0)]
	public bool playOnAwake = true;

	// Token: 0x04000271 RID: 625
	[FoldoutGroup("Spawn settings", 0)]
	public bool playOnEnablee;

	// Token: 0x04000272 RID: 626
	[FoldoutGroup("Spawn settings", 0)]
	public bool loop;

	// Token: 0x04000273 RID: 627
	[FoldoutGroup("Spawn settings", 0)]
	public float duration = 2f;

	// Token: 0x04000274 RID: 628
	[FoldoutGroup("Spawn settings", 0)]
	public AnimationCurve sizeMultiplierOverTime = AnimationCurve.Linear(0f, 1f, 1f, 1f);

	// Token: 0x04000275 RID: 629
	public AnimationCurve emissionMultiplierOverTime = AnimationCurve.Linear(0f, 1f, 1f, 1f);

	// Token: 0x04000276 RID: 630
	public ObjectParticle particleSettings;

	// Token: 0x04000277 RID: 631
	[FoldoutGroup("Global settings", 0)]
	public float simulationSpeed = 1f;

	// Token: 0x04000278 RID: 632
	[HideInInspector]
	public float simulationSpeedMultiplier = 1f;

	// Token: 0x04000279 RID: 633
	[FoldoutGroup("Global settings", 0)]
	public float saturationMultiplier = 1f;

	// Token: 0x0400027A RID: 634
	[FoldoutGroup("Events", 0)]
	public UnityEvent startEvent;

	// Token: 0x0400027B RID: 635
	[FoldoutGroup("Events", 0)]
	public float startEventDelay;

	// Token: 0x0400027C RID: 636
	[FoldoutGroup("Events", 0)]
	public UnityEvent endEvent;

	// Token: 0x0400027D RID: 637
	[FoldoutGroup("Events", 0)]
	public float endEventDelay;

	// Token: 0x0400027E RID: 638
	private float sizeOverTimeAnimationCurveLength;

	// Token: 0x0400027F RID: 639
	private float sizeMultiplierOverTimeAnimationCurveLength;

	// Token: 0x04000280 RID: 640
	private float alphaOverTimeAnimationCurveLength;

	// Token: 0x04000281 RID: 641
	private float emissionOverTimeAnimationCurveLength;

	// Token: 0x04000282 RID: 642
	private bool inited;

	// Token: 0x04000283 RID: 643
	private Coroutine emissionLoop;

	// Token: 0x04000284 RID: 644
	private float lastEmissionTime;

	// Token: 0x04000285 RID: 645
	private bool isPlaying;
}
