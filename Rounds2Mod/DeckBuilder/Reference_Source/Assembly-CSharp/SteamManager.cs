using System;
using System.Text;
using Steamworks;
using UnityEngine;

// Token: 0x020001AC RID: 428
[DisallowMultipleComponent]
public class SteamManager : MonoBehaviour
{
	// Token: 0x1700003E RID: 62
	// (get) Token: 0x06000889 RID: 2185 RVA: 0x0002D717 File Offset: 0x0002B917
	protected static SteamManager Instance
	{
		get
		{
			if (SteamManager.s_instance == null)
			{
				return new GameObject("SteamManager").AddComponent<SteamManager>();
			}
			return SteamManager.s_instance;
		}
	}

	// Token: 0x1700003F RID: 63
	// (get) Token: 0x0600088A RID: 2186 RVA: 0x0002D73B File Offset: 0x0002B93B
	public static bool Initialized
	{
		get
		{
			return SteamManager.Instance.m_bInitialized;
		}
	}

	// Token: 0x0600088B RID: 2187 RVA: 0x000027C8 File Offset: 0x000009C8
	protected static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
	{
	}

	// Token: 0x0600088C RID: 2188 RVA: 0x0002D748 File Offset: 0x0002B948
	protected virtual void Awake()
	{
		if (SteamManager.s_instance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		SteamManager.s_instance = this;
		if (SteamManager.s_EverInitialized)
		{
			throw new Exception("Tried to Initialize the SteamAPI twice in one session!");
		}
		Object.DontDestroyOnLoad(base.gameObject);
		if (!Packsize.Test())
		{
			global::Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", this);
		}
		if (!DllCheck.Test())
		{
			global::Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", this);
		}
		this.m_bInitialized = SteamAPI.Init();
		if (!this.m_bInitialized)
		{
			global::Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", this);
			return;
		}
		SteamManager.s_EverInitialized = true;
	}

	// Token: 0x0600088D RID: 2189 RVA: 0x0002D7DC File Offset: 0x0002B9DC
	protected virtual void OnEnable()
	{
		if (SteamManager.s_instance == null)
		{
			SteamManager.s_instance = this;
		}
		if (!this.m_bInitialized)
		{
			return;
		}
		if (this.m_SteamAPIWarningMessageHook == null)
		{
			this.m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamManager.SteamAPIDebugTextHook);
			SteamClient.SetWarningMessageHook(this.m_SteamAPIWarningMessageHook);
		}
	}

	// Token: 0x0600088E RID: 2190 RVA: 0x0002D82A File Offset: 0x0002BA2A
	protected virtual void OnDestroy()
	{
		if (SteamManager.s_instance != this)
		{
			return;
		}
		SteamManager.s_instance = null;
		if (!this.m_bInitialized)
		{
			return;
		}
		SteamAPI.Shutdown();
	}

	// Token: 0x0600088F RID: 2191 RVA: 0x0002D84E File Offset: 0x0002BA4E
	protected virtual void Update()
	{
		if (!this.m_bInitialized)
		{
			return;
		}
		SteamAPI.RunCallbacks();
	}

	// Token: 0x040009CE RID: 2510
	protected static bool s_EverInitialized;

	// Token: 0x040009CF RID: 2511
	protected static SteamManager s_instance;

	// Token: 0x040009D0 RID: 2512
	protected bool m_bInitialized;

	// Token: 0x040009D1 RID: 2513
	protected SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;
}
