using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoundImplementation
{
	// Token: 0x020001E0 RID: 480
	public class SoundOnePerPlayer
	{
		// Token: 0x06000978 RID: 2424 RVA: 0x00030BCD File Offset: 0x0002EDCD
		private void AddTransformToList(Transform transform)
		{
			this.remoteControlPlayerTransformList.Add(transform);
			this.remoteControlNumberOfList.Add(0);
		}

		// Token: 0x06000979 RID: 2425 RVA: 0x00030BE8 File Offset: 0x0002EDE8
		public int GetNumberOf(Transform transform)
		{
			for (int i = 0; i < this.remoteControlPlayerTransformList.Count; i++)
			{
				if (this.remoteControlPlayerTransformList[i] == transform)
				{
					return this.remoteControlNumberOfList[i];
				}
			}
			this.AddTransformToList(transform);
			return this.GetNumberOf(transform);
		}

		// Token: 0x0600097A RID: 2426 RVA: 0x00030C3C File Offset: 0x0002EE3C
		public void AddNumberOf(Transform transform, int toAdd)
		{
			for (int i = 0; i < this.remoteControlPlayerTransformList.Count; i++)
			{
				if (this.remoteControlPlayerTransformList[i] == transform)
				{
					List<int> list = this.remoteControlNumberOfList;
					int num = i;
					list[num] += toAdd;
					return;
				}
			}
			this.AddTransformToList(transform);
			this.AddNumberOf(transform, toAdd);
		}

		// Token: 0x04000ACD RID: 2765
		private List<Transform> remoteControlPlayerTransformList = new List<Transform>();

		// Token: 0x04000ACE RID: 2766
		private List<int> remoteControlNumberOfList = new List<int>();
	}
}
