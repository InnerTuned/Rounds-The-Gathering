using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000D1 RID: 209
public class SetTeamColor : MonoBehaviour
{
	// Token: 0x06000448 RID: 1096 RVA: 0x00019D58 File Offset: 0x00017F58
	private void Awake()
	{
		this.m_spriteRenderer = base.GetComponent<SpriteRenderer>();
		this.meshRend = base.GetComponent<MeshRenderer>();
		this.m_lineRenderer = base.GetComponent<LineRenderer>();
	}

	// Token: 0x06000449 RID: 1097 RVA: 0x00019D80 File Offset: 0x00017F80
	public void Set(PlayerSkin teamColor)
	{
		Color color = teamColor.color;
		if (this.colorType == SetTeamColor.ColorType.Background)
		{
			color = teamColor.backgroundColor;
		}
		if (this.colorType == SetTeamColor.ColorType.Particle)
		{
			color = teamColor.particleEffect;
		}
		if (this.colorType == SetTeamColor.ColorType.WinText)
		{
			color = teamColor.winText;
		}
		if (this.m_lineRenderer)
		{
			this.m_lineRenderer.startColor = color;
			this.m_lineRenderer.endColor = color;
		}
		else if (this.m_spriteRenderer)
		{
			this.m_spriteRenderer.color = color;
		}
		else if (this.meshRend)
		{
			this.meshRend.material.color = color;
		}
		else
		{
			this.m_particleSystem = base.GetComponent<ParticleSystem>();
			if (this.m_particleSystem)
			{
				this.m_particleSystem.main.startColor = color;
			}
		}
		this.SetColorEvent.Invoke();
	}

	// Token: 0x0600044A RID: 1098 RVA: 0x00019E64 File Offset: 0x00018064
	public static void TeamColorThis(GameObject go, PlayerSkin teamColor)
	{
		if (teamColor == null)
		{
			return;
		}
		SetTeamColor[] componentsInChildren = go.GetComponentsInChildren<SetTeamColor>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Set(teamColor);
		}
	}

	// Token: 0x040005CB RID: 1483
	private SpriteRenderer m_spriteRenderer;

	// Token: 0x040005CC RID: 1484
	private ParticleSystem m_particleSystem;

	// Token: 0x040005CD RID: 1485
	private LineRenderer m_lineRenderer;

	// Token: 0x040005CE RID: 1486
	public UnityEvent SetColorEvent;

	// Token: 0x040005CF RID: 1487
	private MeshRenderer meshRend;

	// Token: 0x040005D0 RID: 1488
	public SetTeamColor.ColorType colorType;

	// Token: 0x02000379 RID: 889
	public enum ColorType
	{
		// Token: 0x040011B0 RID: 4528
		Main,
		// Token: 0x040011B1 RID: 4529
		Background,
		// Token: 0x040011B2 RID: 4530
		Particle,
		// Token: 0x040011B3 RID: 4531
		WinText
	}
}
