using System;
using UnityEngine;

namespace Photon.Utilities
{
	// Token: 0x0200024B RID: 587
	public abstract class SettingsScriptableObject<T> : SettingsScriptableObjectBase where T : SettingsScriptableObjectBase
	{
		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000CB9 RID: 3257 RVA: 0x0003FC80 File Offset: 0x0003DE80
		public static T Single
		{
			get
			{
				if (!SettingsScriptableObject<T>.single)
				{
					SettingsScriptableObject<T>.single = Resources.Load<T>(SettingsScriptableObject<T>.AssetName);
					if (SettingsScriptableObject<T>.single)
					{
						SettingsScriptableObject<T>.single.Initialize();
					}
				}
				return SettingsScriptableObject<T>.single;
			}
		}

		// Token: 0x06000CBA RID: 3258 RVA: 0x0003FCD2 File Offset: 0x0003DED2
		protected virtual void Awake()
		{
			T t = SettingsScriptableObject<T>.single;
			this.Initialize();
			if (t == null && SettingsScriptableObject<T>.single != null && SettingsScriptableObject<T>.OnSingletonReady != null)
			{
				SettingsScriptableObject<T>.OnSingletonReady.Invoke();
			}
		}

		// Token: 0x06000CBB RID: 3259 RVA: 0x0003FCD2 File Offset: 0x0003DED2
		protected virtual void OnEnable()
		{
			T t = SettingsScriptableObject<T>.single;
			this.Initialize();
			if (t == null && SettingsScriptableObject<T>.single != null && SettingsScriptableObject<T>.OnSingletonReady != null)
			{
				SettingsScriptableObject<T>.OnSingletonReady.Invoke();
			}
		}

		// Token: 0x06000CBC RID: 3260 RVA: 0x0003FD0F File Offset: 0x0003DF0F
		public override void Initialize()
		{
			SettingsScriptableObject<T>.single = (this as T);
		}

		// Token: 0x04000C65 RID: 3173
		public static string AssetName = typeof(T).Name;

		// Token: 0x04000C66 RID: 3174
		public static Action OnSingletonReady;

		// Token: 0x04000C67 RID: 3175
		public static T single;
	}
}
