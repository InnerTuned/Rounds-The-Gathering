using System;
using UnityEngine;

// Token: 0x020000A5 RID: 165
public class PlayLineAnimation : MonoBehaviour
{
	// Token: 0x060003A8 RID: 936 RVA: 0x000164E4 File Offset: 0x000146E4
	private void Start()
	{
		this.lineEffect = base.GetComponent<LineEffect>();
	}

	// Token: 0x060003A9 RID: 937 RVA: 0x000164F2 File Offset: 0x000146F2
	public void PlayOffset()
	{
		this.lineEffect.PlayAnim(LineEffect.AnimType.Offset, this.offsetCurve, this.offsetSpeed);
	}

	// Token: 0x060003AA RID: 938 RVA: 0x0001650C File Offset: 0x0001470C
	public void PlayWidth()
	{
		this.lineEffect.PlayAnim(LineEffect.AnimType.Width, this.widthCurve, this.widthSpeed);
	}

	// Token: 0x040004CC RID: 1228
	private LineEffect lineEffect;

	// Token: 0x040004CD RID: 1229
	public AnimationCurve offsetCurve;

	// Token: 0x040004CE RID: 1230
	public float offsetSpeed = 1f;

	// Token: 0x040004CF RID: 1231
	public AnimationCurve widthCurve;

	// Token: 0x040004D0 RID: 1232
	public float widthSpeed = 1f;
}
