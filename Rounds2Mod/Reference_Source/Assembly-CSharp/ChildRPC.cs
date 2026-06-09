using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200011D RID: 285
public class ChildRPC : MonoBehaviour
{
	// Token: 0x06000597 RID: 1431 RVA: 0x000203BF File Offset: 0x0001E5BF
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000598 RID: 1432 RVA: 0x000203CD File Offset: 0x0001E5CD
	public void CallFunction(string key)
	{
		this.view.RPC("RPCA_RecieveFunction", 0, new object[]
		{
			key
		});
	}

	// Token: 0x06000599 RID: 1433 RVA: 0x000203EA File Offset: 0x0001E5EA
	[PunRPC]
	public void RPCA_RecieveFunction(string key)
	{
		if (this.childRPCs.ContainsKey(key))
		{
			this.childRPCs[key].Invoke();
		}
	}

	// Token: 0x0600059A RID: 1434 RVA: 0x0002040B File Offset: 0x0001E60B
	public void CallFunction(string key, int intData)
	{
		this.view.RPC("RPCA_RecieveFunction", 0, new object[]
		{
			key,
			intData
		});
	}

	// Token: 0x0600059B RID: 1435 RVA: 0x00020431 File Offset: 0x0001E631
	[PunRPC]
	public void RPCA_RecieveFunction(string key, int intData)
	{
		if (this.childRPCsInt.ContainsKey(key))
		{
			this.childRPCsInt[key].Invoke(intData);
		}
	}

	// Token: 0x0600059C RID: 1436 RVA: 0x00020453 File Offset: 0x0001E653
	public void CallFunction(string key, Vector2 vectorData)
	{
		this.view.RPC("RPCA_RecieveFunction", 0, new object[]
		{
			key,
			vectorData
		});
	}

	// Token: 0x0600059D RID: 1437 RVA: 0x00020479 File Offset: 0x0001E679
	[PunRPC]
	public void RPCA_RecieveFunction(string key, Vector2 vectorData)
	{
		if (this.childRPCsVector2.ContainsKey(key))
		{
			this.childRPCsVector2[key].Invoke(vectorData);
		}
	}

	// Token: 0x0600059E RID: 1438 RVA: 0x0002049B File Offset: 0x0001E69B
	public void CallFunction(string key, Vector2 vectorData, Vector2 vectorData2, int intData)
	{
		this.view.RPC("RPCA_RecieveFunction", 0, new object[]
		{
			key,
			vectorData,
			vectorData2,
			intData
		});
	}

	// Token: 0x0600059F RID: 1439 RVA: 0x000204D4 File Offset: 0x0001E6D4
	[PunRPC]
	public void RPCA_RecieveFunction(string key, Vector2 vectorData, Vector2 vectorData2, int intData)
	{
		if (this.childRPCsVector2Vector2Int.ContainsKey(key))
		{
			this.childRPCsVector2Vector2Int[key].Invoke(vectorData, vectorData2, intData);
		}
	}

	// Token: 0x060005A0 RID: 1440 RVA: 0x000204FC File Offset: 0x0001E6FC
	public void CallFunction(string key, Vector2 vectorData, Vector2 vectorData2, int intData, int intData2)
	{
		this.view.RPC("RPCA_RecieveFunction", 0, new object[]
		{
			key,
			vectorData,
			vectorData2,
			intData,
			intData2
		});
	}

	// Token: 0x060005A1 RID: 1441 RVA: 0x0002054A File Offset: 0x0001E74A
	[PunRPC]
	public void RPCA_RecieveFunction(string key, Vector2 vectorData, Vector2 vectorData2, int intData, int intData2)
	{
		if (this.childRPCsVector2Vector2IntInt.ContainsKey(key))
		{
			this.childRPCsVector2Vector2IntInt[key].Invoke(vectorData, vectorData2, intData, intData2);
		}
	}

	// Token: 0x060005A2 RID: 1442 RVA: 0x00020571 File Offset: 0x0001E771
	public void CallFunction(string key, Vector3 vectorData, Quaternion quaterion)
	{
		this.view.RPC("RPCA_RecieveFunction", 0, new object[]
		{
			key,
			vectorData,
			quaterion
		});
	}

	// Token: 0x060005A3 RID: 1443 RVA: 0x000205A0 File Offset: 0x0001E7A0
	[PunRPC]
	public void RPCA_RecieveFunction(string key, Vector3 vectorData, Quaternion quaterion)
	{
		if (this.childRPCsVector3Quaternion.ContainsKey(key))
		{
			this.childRPCsVector3Quaternion[key].Invoke(vectorData, quaterion);
		}
	}

	// Token: 0x0400073D RID: 1853
	public Dictionary<string, Action<Vector2, Vector2, int>> childRPCsVector2Vector2Int = new Dictionary<string, Action<Vector2, Vector2, int>>();

	// Token: 0x0400073E RID: 1854
	public Dictionary<string, Action<Vector2, Vector2, int, int>> childRPCsVector2Vector2IntInt = new Dictionary<string, Action<Vector2, Vector2, int, int>>();

	// Token: 0x0400073F RID: 1855
	public Dictionary<string, Action<Vector2>> childRPCsVector2 = new Dictionary<string, Action<Vector2>>();

	// Token: 0x04000740 RID: 1856
	public Dictionary<string, Action<Vector3, Quaternion>> childRPCsVector3Quaternion = new Dictionary<string, Action<Vector3, Quaternion>>();

	// Token: 0x04000741 RID: 1857
	public Dictionary<string, Action<int>> childRPCsInt = new Dictionary<string, Action<int>>();

	// Token: 0x04000742 RID: 1858
	public Dictionary<string, Action> childRPCs = new Dictionary<string, Action>();

	// Token: 0x04000743 RID: 1859
	private PhotonView view;
}
