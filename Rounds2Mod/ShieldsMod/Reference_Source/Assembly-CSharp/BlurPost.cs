using System;
using UnityEngine;

// Token: 0x02000017 RID: 23
[ExecuteInEditMode]
public class BlurPost : MonoBehaviour
{
	// Token: 0x0600007C RID: 124 RVA: 0x00004F44 File Offset: 0x00003144
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height);
		Graphics.Blit(source, temporary, this.postprocessMaterial, 0);
		Graphics.Blit(temporary, destination, this.postprocessMaterial, 1);
		RenderTexture.ReleaseTemporary(temporary);
	}

	// Token: 0x04000081 RID: 129
	[SerializeField]
	private Material postprocessMaterial;
}
