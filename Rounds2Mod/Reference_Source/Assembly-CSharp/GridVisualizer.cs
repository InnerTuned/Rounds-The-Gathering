using System;
using UnityEngine;

// Token: 0x02000067 RID: 103
public class GridVisualizer : MonoBehaviour
{
	// Token: 0x060001FD RID: 509 RVA: 0x0000C40A File Offset: 0x0000A60A
	private void Start()
	{
		GridVisualizer.instance = this;
		this.SpawnGrid();
	}

	// Token: 0x060001FE RID: 510 RVA: 0x0000C418 File Offset: 0x0000A618
	public void BulletCall(Vector2 worldSpacePosition)
	{
		Vector2Int gridPos = this.WorldToGridSpace(worldSpacePosition);
		float power = Vector2.Distance(worldSpacePosition, this.GridToWorldSpace(gridPos));
		this.spawnedObjects[gridPos.x, gridPos.y].BopCall(power);
	}

	// Token: 0x060001FF RID: 511 RVA: 0x0000C45A File Offset: 0x0000A65A
	public Vector2 GridToWorldSpace(int x, int y)
	{
		return this.GridToWorldSpace(new Vector2Int(x, y));
	}

	// Token: 0x06000200 RID: 512 RVA: 0x0000C46C File Offset: 0x0000A66C
	public Vector2 GridToWorldSpace(Vector2Int gridPos)
	{
		return new Vector2(Mathf.Lerp(this.min.x, this.max.x, (float)gridPos.x / (float)this.numberOfObjects.x), Mathf.Lerp(this.min.y, this.max.y, (float)gridPos.y / (float)this.numberOfObjects.y));
	}

	// Token: 0x06000201 RID: 513 RVA: 0x0000C4E0 File Offset: 0x0000A6E0
	public Vector2Int WorldToGridSpace(Vector2 pos)
	{
		return new Vector2Int((int)(Mathf.InverseLerp(this.min.x, this.max.x, pos.x) * (float)this.numberOfObjects.x), (int)(Mathf.InverseLerp(this.min.y, this.max.y, pos.y) * (float)this.numberOfObjects.y));
	}

	// Token: 0x06000202 RID: 514 RVA: 0x0000C550 File Offset: 0x0000A750
	private void SpawnGrid()
	{
		this.spawnedObjects = new GridObject[this.numberOfObjects.x, this.numberOfObjects.y];
		for (int i = 0; i < this.numberOfObjects.x; i++)
		{
			for (int j = 0; j < this.numberOfObjects.y; j++)
			{
				this.spawnedObjects[i, j] = Object.Instantiate<GameObject>(this.prefab, this.GridToWorldSpace(i, j), Quaternion.identity).GetComponent<GridObject>();
			}
		}
	}

	// Token: 0x040002A5 RID: 677
	public Vector2 min;

	// Token: 0x040002A6 RID: 678
	public Vector2 max;

	// Token: 0x040002A7 RID: 679
	public GameObject prefab;

	// Token: 0x040002A8 RID: 680
	public Vector2Int numberOfObjects;

	// Token: 0x040002A9 RID: 681
	internal GridObject[,] spawnedObjects;

	// Token: 0x040002AA RID: 682
	public static GridVisualizer instance;
}
