using System;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x0200011B RID: 283
public class ChatFilter : MonoBehaviour
{
	// Token: 0x06000592 RID: 1426 RVA: 0x0002023D File Offset: 0x0001E43D
	private void Awake()
	{
		ChatFilter.instance = this;
	}

	// Token: 0x06000593 RID: 1427 RVA: 0x00020248 File Offset: 0x0001E448
	[Button]
	private void EnterData()
	{
		string[] array = this.enterData.Replace('"', ' ').Trim().Split(new char[]
		{
			','
		});
		ChatFilterInstance chatFilterInstance = null;
		for (int i = 0; i < this.filters.Length; i++)
		{
			if (this.filters[i].category == this.category)
			{
				chatFilterInstance = this.filters[i];
				break;
			}
		}
		if (chatFilterInstance == null)
		{
			UnityEngine.Debug.LogError("No valid category target!");
			return;
		}
		for (int j = 0; j < array.Length; j++)
		{
			array[j] = array[j].Trim();
			array[j] = array[j].ToUpper();
			if (!(array[j] == ""))
			{
				bool flag = false;
				for (int k = 0; k < chatFilterInstance.words.Count; k++)
				{
					if (chatFilterInstance.words[k] == array[j])
					{
						flag = true;
					}
				}
				if (!flag)
				{
					chatFilterInstance.words.Add(array[j]);
				}
			}
		}
	}

	// Token: 0x06000594 RID: 1428 RVA: 0x00020340 File Offset: 0x0001E540
	public string FilterMessage(string message)
	{
		for (int i = 0; i < this.filters.Length; i++)
		{
			for (int j = 0; j < this.filters[i].words.Count; j++)
			{
				if (message.ToUpper().Contains(this.filters[i].words[j]))
				{
					return this.filters[i].replacement;
				}
			}
		}
		return message;
	}

	// Token: 0x04000736 RID: 1846
	[TextArea]
	public string enterData;

	// Token: 0x04000737 RID: 1847
	public string category;

	// Token: 0x04000738 RID: 1848
	public static ChatFilter instance;

	// Token: 0x04000739 RID: 1849
	public ChatFilterInstance[] filters;
}
