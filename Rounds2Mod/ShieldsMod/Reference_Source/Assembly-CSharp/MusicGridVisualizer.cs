using System;

// Token: 0x020000F2 RID: 242
public class MusicGridVisualizer : GridVisualizer
{
	// Token: 0x060004DF RID: 1247 RVA: 0x0001BF20 File Offset: 0x0001A120
	private void Update()
	{
		for (int i = 0; i < this.numberOfObjects.x; i++)
		{
			for (int j = 0; j < this.numberOfObjects.y; j++)
			{
				this.spawnedObjects[i, j].OnSetSize(MusicVisualizerData.Samples[i + j] * 200f);
			}
		}
	}
}
