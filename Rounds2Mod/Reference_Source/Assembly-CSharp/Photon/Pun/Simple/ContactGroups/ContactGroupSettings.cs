using System;
using System.Collections.Generic;
using Photon.Utilities;
using UnityEngine;

namespace Photon.Pun.Simple.ContactGroups
{
	// Token: 0x020002FF RID: 767
	public class ContactGroupSettings : SettingsScriptableObject<ContactGroupSettings>
	{
		// Token: 0x06001073 RID: 4211 RVA: 0x0004F333 File Offset: 0x0004D533
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Bootstrap()
		{
			ContactGroupSettings single = SettingsScriptableObject<ContactGroupSettings>.Single;
		}

		// Token: 0x06001074 RID: 4212 RVA: 0x0004F33C File Offset: 0x0004D53C
		public override void Initialize()
		{
			SettingsScriptableObject<ContactGroupSettings>.single = this;
			base.Initialize();
			if (ContactGroupSettings.initialized)
			{
				return;
			}
			ContactGroupSettings.initialized = true;
			ContactGroupSettings.bitsForMask = this.contactGroupTags.Count - 1;
			for (int i = 0; i < this.contactGroupTags.Count; i++)
			{
				if (this.rewindLayerTagToId.ContainsKey(this.contactGroupTags[i]))
				{
					global::Debug.LogError(string.Concat(new string[]
					{
						"The tag '",
						this.contactGroupTags[i],
						"' is used more than once in '",
						base.GetType().Name,
						"'. Repeats will be discarded, which will likely break some parts of rewind until they are removed."
					}));
				}
				else
				{
					this.rewindLayerTagToId.Add(this.contactGroupTags[i], i);
				}
			}
		}

		// Token: 0x06001075 RID: 4213 RVA: 0x0004F408 File Offset: 0x0004D608
		[Obsolete("Left over from NST, likely not useful any more.")]
		public static int FindClosestMatch(string n, int id)
		{
			ContactGroupSettings single = SettingsScriptableObject<ContactGroupSettings>.Single;
			if (single.contactGroupTags.Contains(n))
			{
				return single.contactGroupTags.IndexOf(n);
			}
			if (id < single.contactGroupTags.Count)
			{
				return id;
			}
			return 0;
		}

		// Token: 0x06001076 RID: 4214 RVA: 0x0004F447 File Offset: 0x0004D647
		public ContactGroupSettings()
		{
			List<string> list = new List<string>(2);
			list.Add("Default");
			list.Add("Critical");
			this.contactGroupTags = list;
			this.rewindLayerTagToId = new Dictionary<string, int>();
			base..ctor();
		}

		// Token: 0x04000F64 RID: 3940
		public static bool initialized;

		// Token: 0x04000F65 RID: 3941
		public const string DEF_NAME = "Default";

		// Token: 0x04000F66 RID: 3942
		[HideInInspector]
		public List<string> contactGroupTags;

		// Token: 0x04000F67 RID: 3943
		public Dictionary<string, int> rewindLayerTagToId;

		// Token: 0x04000F68 RID: 3944
		[NonSerialized]
		public static int bitsForMask;
	}
}
