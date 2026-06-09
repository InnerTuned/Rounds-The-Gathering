using System;
using UnityEngine.SceneManagement;

// Token: 0x0200007F RID: 127
public class MapWrapper
{
	// Token: 0x17000008 RID: 8
	// (get) Token: 0x060002A3 RID: 675 RVA: 0x000112F1 File Offset: 0x0000F4F1
	// (set) Token: 0x060002A4 RID: 676 RVA: 0x000112F9 File Offset: 0x0000F4F9
	public Map Map { get; private set; }

	// Token: 0x17000009 RID: 9
	// (get) Token: 0x060002A5 RID: 677 RVA: 0x00011302 File Offset: 0x0000F502
	// (set) Token: 0x060002A6 RID: 678 RVA: 0x0001130A File Offset: 0x0000F50A
	public Scene Scene { get; private set; }

	// Token: 0x060002A7 RID: 679 RVA: 0x00011313 File Offset: 0x0000F513
	public MapWrapper(Map map, Scene scene)
	{
		this.Map = map;
		this.Scene = scene;
	}
}
