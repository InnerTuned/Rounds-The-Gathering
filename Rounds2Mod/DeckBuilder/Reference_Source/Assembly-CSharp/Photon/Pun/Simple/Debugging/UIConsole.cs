using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Pun.Simple.Debugging
{
	// Token: 0x020002FB RID: 763
	public class UIConsole : MonoBehaviour
	{
		// Token: 0x1700012D RID: 301
		// (get) Token: 0x06001055 RID: 4181 RVA: 0x0004EF7B File Offset: 0x0004D17B
		public static UIConsole Single
		{
			get
			{
				if (UIConsole.single == null)
				{
					UIConsole.CreateGUI();
				}
				return UIConsole.single;
			}
		}

		// Token: 0x06001056 RID: 4182 RVA: 0x0004EF95 File Offset: 0x0004D195
		private void Awake()
		{
			UIConsole.single = this;
			UIConsole.uitext = base.GetComponent<Text>();
			UIConsole.uitext.text = UIConsole.strb.ToString();
		}

		// Token: 0x06001057 RID: 4183 RVA: 0x0004EFBC File Offset: 0x0004D1BC
		public static void Log(string str)
		{
			if (!UIConsole.single)
			{
				return;
			}
			if (UIConsole.strb.Length > UIConsole.single.maxSize)
			{
				UIConsole.strb.Length = 0;
			}
			if (UIConsole.uitext != null)
			{
				UIConsole.strb.Append(str).Append("\n");
				UIConsole.uitext.text = UIConsole.strb.ToString();
			}
			if (UIConsole.single.logToDebug)
			{
				global::Debug.Log(str);
			}
		}

		// Token: 0x06001058 RID: 4184 RVA: 0x0004F040 File Offset: 0x0004D240
		public UIConsole _(object str)
		{
			UIConsole.strb.Append(str.ToString());
			return UIConsole.single;
		}

		// Token: 0x06001059 RID: 4185 RVA: 0x0004F058 File Offset: 0x0004D258
		public UIConsole _(string str)
		{
			UIConsole.strb.Append(str);
			return UIConsole.single;
		}

		// Token: 0x0600105A RID: 4186 RVA: 0x0004F06B File Offset: 0x0004D26B
		public UIConsole _(int str)
		{
			UIConsole.strb.Append(str);
			return UIConsole.single;
		}

		// Token: 0x0600105B RID: 4187 RVA: 0x0004F07E File Offset: 0x0004D27E
		public UIConsole _(uint str)
		{
			UIConsole.strb.Append(str);
			return UIConsole.single;
		}

		// Token: 0x0600105C RID: 4188 RVA: 0x0004F091 File Offset: 0x0004D291
		public UIConsole _(byte str)
		{
			UIConsole.strb.Append(str);
			return UIConsole.single;
		}

		// Token: 0x0600105D RID: 4189 RVA: 0x0004F0A4 File Offset: 0x0004D2A4
		public UIConsole _(sbyte str)
		{
			UIConsole.strb.Append(str);
			return UIConsole.single;
		}

		// Token: 0x0600105E RID: 4190 RVA: 0x0004F0B7 File Offset: 0x0004D2B7
		public UIConsole _(short str)
		{
			UIConsole.strb.Append(str);
			return UIConsole.single;
		}

		// Token: 0x0600105F RID: 4191 RVA: 0x0004F0CA File Offset: 0x0004D2CA
		public UIConsole _(ushort str)
		{
			UIConsole.strb.Append(str);
			return UIConsole.single;
		}

		// Token: 0x06001060 RID: 4192 RVA: 0x0004F0DD File Offset: 0x0004D2DD
		public UIConsole _(long str)
		{
			UIConsole.strb.Append(str);
			return UIConsole.single;
		}

		// Token: 0x06001061 RID: 4193 RVA: 0x0004F0F0 File Offset: 0x0004D2F0
		public UIConsole _(ulong str)
		{
			UIConsole.strb.Append(str);
			return UIConsole.single;
		}

		// Token: 0x06001062 RID: 4194 RVA: 0x0004F103 File Offset: 0x0004D303
		public UIConsole _(float str)
		{
			UIConsole.strb.Append(str);
			return UIConsole.single;
		}

		// Token: 0x06001063 RID: 4195 RVA: 0x0004F116 File Offset: 0x0004D316
		public UIConsole _(double str)
		{
			UIConsole.strb.Append(str);
			return UIConsole.single;
		}

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x06001064 RID: 4196 RVA: 0x0004F129 File Offset: 0x0004D329
		public UIConsole __
		{
			get
			{
				UIConsole.strb.Append(" ");
				return UIConsole.single;
			}
		}

		// Token: 0x06001065 RID: 4197 RVA: 0x0004F140 File Offset: 0x0004D340
		public static void Refresh()
		{
			if (!UIConsole.single)
			{
				return;
			}
			if (UIConsole.uitext != null)
			{
				UIConsole.uitext.text = UIConsole.strb.ToString();
			}
		}

		// Token: 0x06001066 RID: 4198 RVA: 0x0004F170 File Offset: 0x0004D370
		public static void Clear()
		{
			UIConsole.strb.Length = 0;
			if (UIConsole.uitext)
			{
				UIConsole.uitext.text = UIConsole.strb.ToString();
			}
		}

		// Token: 0x06001067 RID: 4199 RVA: 0x0004F1A0 File Offset: 0x0004D3A0
		public static UIConsole CreateGUI()
		{
			GameObject gameObject = new GameObject("UI CONSOLE");
			Canvas canvas = gameObject.AddComponent<Canvas>();
			GameObject gameObject2 = new GameObject("CONSOLE TEXT");
			gameObject2.transform.parent = gameObject.transform;
			UIConsole.uitext = gameObject2.AddComponent<Text>();
			canvas.renderMode = 0;
			UIConsole.uitext.font = (Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font);
			UIConsole.uitext.verticalOverflow = 1;
			UIConsole.uitext.horizontalOverflow = 1;
			UIConsole.uitext.alignment = 1;
			UIConsole.uitext.rectTransform.pivot = new Vector2(0f, 0f);
			UIConsole.uitext.rectTransform.anchorMin = new Vector2(0f, 0f);
			UIConsole.uitext.rectTransform.anchorMax = new Vector2(1f, 1f);
			UIConsole.uitext.rectTransform.offsetMax = new Vector2(0f, 0f);
			UIConsole.single = gameObject2.AddComponent<UIConsole>();
			return UIConsole.single;
		}

		// Token: 0x04000F5B RID: 3931
		public int maxSize = 3000;

		// Token: 0x04000F5C RID: 3932
		public bool logToDebug = true;

		// Token: 0x04000F5D RID: 3933
		public static readonly StringBuilder strb = new StringBuilder();

		// Token: 0x04000F5E RID: 3934
		private static UIConsole single;

		// Token: 0x04000F5F RID: 3935
		private static Text uitext;
	}
}
