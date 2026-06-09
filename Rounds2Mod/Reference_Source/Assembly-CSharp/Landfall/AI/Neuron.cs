using System;
using System.Collections.Generic;
using UnityEngine;

namespace Landfall.AI
{
	// Token: 0x02000322 RID: 802
	public class Neuron
	{
		// Token: 0x17000140 RID: 320
		// (get) Token: 0x06001140 RID: 4416 RVA: 0x00052B25 File Offset: 0x00050D25
		// (set) Token: 0x06001141 RID: 4417 RVA: 0x00052B2D File Offset: 0x00050D2D
		public double OutputValue
		{
			get
			{
				return this.m_outputValue;
			}
			set
			{
				this.m_outputValue = value;
			}
		}

		// Token: 0x06001142 RID: 4418 RVA: 0x00052B36 File Offset: 0x00050D36
		public Neuron(int numOutputs, int index, Neuron[] prevLayer) : this(numOutputs, index)
		{
			this.m_prevLayer = prevLayer;
		}

		// Token: 0x06001143 RID: 4419 RVA: 0x00052B48 File Offset: 0x00050D48
		public Neuron(int numOutputs, int index)
		{
			this.m_index = index;
			for (int i = 0; i < numOutputs; i++)
			{
				Neuron.Connection connection = new Neuron.Connection();
				connection.m_weight = (double)Random.Range(-0.5f, 0.5f);
				this.m_outputWeights.Add(i, connection);
			}
		}

		// Token: 0x06001144 RID: 4420 RVA: 0x00052BA4 File Offset: 0x00050DA4
		public void FeedForward()
		{
			double prevLayerSum = this.GetPrevLayerSum();
			this.m_outputValue = Neuron.TransferFunction(prevLayerSum);
		}

		// Token: 0x06001145 RID: 4421 RVA: 0x00052BC4 File Offset: 0x00050DC4
		public void FeedForward(double bonusInput)
		{
			double num = this.GetPrevLayerSum();
			num += bonusInput;
			this.m_outputValue = Neuron.TransferFunction(num);
		}

		// Token: 0x06001146 RID: 4422 RVA: 0x00052BE8 File Offset: 0x00050DE8
		private double GetPrevLayerSum()
		{
			double num = 0.0;
			if (this.m_prevLayer != null)
			{
				for (int i = 0; i < this.m_prevLayer.Length; i++)
				{
					Neuron neuron = this.m_prevLayer[i];
					num += neuron.OutputValue * neuron.m_outputWeights[this.m_index].m_weight;
				}
			}
			return num;
		}

		// Token: 0x06001147 RID: 4423 RVA: 0x00052C44 File Offset: 0x00050E44
		public List<double> GetWeights()
		{
			List<double> list = new List<double>();
			List<int> list2 = new List<int>(this.m_outputWeights.Keys);
			list2.Sort();
			for (int i = 0; i < list2.Count; i++)
			{
				list.Add(this.m_outputWeights[list2[i]].m_weight);
			}
			return list;
		}

		// Token: 0x06001148 RID: 4424 RVA: 0x00052CA0 File Offset: 0x00050EA0
		public int SetWeights(List<double> weights, int offset)
		{
			List<int> list = new List<int>(this.m_outputWeights.Keys);
			list.Sort();
			for (int i = 0; i < list.Count; i++)
			{
				this.m_outputWeights[list[i]].m_weight = weights[offset + i];
			}
			return list.Count;
		}

		// Token: 0x06001149 RID: 4425 RVA: 0x00052CFB File Offset: 0x00050EFB
		private static double TransferFunction(double x)
		{
			return Math.Tanh(x);
		}

		// Token: 0x0600114A RID: 4426 RVA: 0x00052D03 File Offset: 0x00050F03
		private static double TransferFunctionDerivative(double x)
		{
			return 1.0 - x * x;
		}

		// Token: 0x04000FF3 RID: 4083
		private static double _ETA = 0.15;

		// Token: 0x04000FF4 RID: 4084
		private static double _ALPHA = 0.5;

		// Token: 0x04000FF5 RID: 4085
		private double m_outputValue;

		// Token: 0x04000FF6 RID: 4086
		private double m_gradient;

		// Token: 0x04000FF7 RID: 4087
		private int m_index;

		// Token: 0x04000FF8 RID: 4088
		private Dictionary<int, Neuron.Connection> m_outputWeights = new Dictionary<int, Neuron.Connection>();

		// Token: 0x04000FF9 RID: 4089
		private Neuron[] m_prevLayer;

		// Token: 0x020003ED RID: 1005
		private class Connection
		{
			// Token: 0x0400134A RID: 4938
			public double m_weight;

			// Token: 0x0400134B RID: 4939
			public double m_deltaWeight;
		}
	}
}
