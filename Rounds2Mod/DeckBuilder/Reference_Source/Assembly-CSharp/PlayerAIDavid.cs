using System;
using System.Collections.Generic;
using Landfall.AI;
using UnityEngine;

// Token: 0x02000040 RID: 64
public class PlayerAIDavid : MonoBehaviour
{
	// Token: 0x06000137 RID: 311 RVA: 0x00008740 File Offset: 0x00006940
	private void Awake()
	{
		this.m_api = base.GetComponentInParent<PlayerAPI>();
		this.m_cachedInput = new double[2];
		this.m_neuralNet = new NeuralNet(PlayerAIDavid.topology);
		if (this.m_weightData != null && this.m_useWeights)
		{
			this.m_neuralNet.SetWeights(this.m_weightData.m_weightDatas[this.m_weightData.m_weightDatas.Count - 1].m_weights);
		}
		this.m_startTime = Time.time + 0.8f;
	}

	// Token: 0x06000138 RID: 312 RVA: 0x000087CE File Offset: 0x000069CE
	public void SetWeights(double[] weights)
	{
		this.m_neuralNet.SetWeights(new List<double>(weights));
	}

	// Token: 0x06000139 RID: 313 RVA: 0x000087E1 File Offset: 0x000069E1
	public void SetWeights(List<double> weights)
	{
		this.m_neuralNet.SetWeights(weights);
	}

	// Token: 0x0600013A RID: 314 RVA: 0x000087F0 File Offset: 0x000069F0
	private void Update()
	{
		if (Time.time < this.m_startTime)
		{
			return;
		}
		if (this.m_aiTimer >= 0.1f)
		{
			this.m_aiTimer -= 0.1f;
			Vector3 vector = this.m_api.OtherPlayerPosition() - this.m_api.PlayerPosition();
			this.m_cachedInput[0] = (double)vector.x;
			this.m_cachedInput[1] = (double)vector.y;
			this.m_neuralNet.FeedForward(this.m_cachedInput, 1.0);
			double[] results = this.m_neuralNet.GetResults();
			this.m_api.SetAimDirection(new Vector2((float)results[3], (float)results[4]));
			if (results[0] > 0.5)
			{
				this.m_api.Jump();
			}
			if (results[1] > 0.5)
			{
				this.m_api.Attack();
			}
			Vector2 zero = Vector2.zero;
			if (results[2] > 0.5)
			{
				zero.x = 1f;
			}
			else if (results[2] < -0.5)
			{
				zero.x = -1f;
			}
			this.m_movement = zero;
		}
		this.m_api.Move(this.m_movement);
		this.m_aiTimer += TimeHandler.deltaTime;
	}

	// Token: 0x040001A2 RID: 418
	public static int[] topology = new int[]
	{
		2,
		10,
		16,
		5
	};

	// Token: 0x040001A3 RID: 419
	[SerializeField]
	private bool m_useWeights;

	// Token: 0x040001A4 RID: 420
	[SerializeField]
	private WeightDataAsset m_weightData;

	// Token: 0x040001A5 RID: 421
	private PlayerAPI m_api;

	// Token: 0x040001A6 RID: 422
	private NeuralNet m_neuralNet;

	// Token: 0x040001A7 RID: 423
	private double[] m_cachedInput;

	// Token: 0x040001A8 RID: 424
	private float m_startTime;

	// Token: 0x040001A9 RID: 425
	private Vector2 m_movement = Vector2.zero;

	// Token: 0x040001AA RID: 426
	private float m_aiTimer;
}
