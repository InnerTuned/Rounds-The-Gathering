using System;
using System.Collections.Generic;
using UnityEngine;

namespace Landfall.AI
{
	// Token: 0x02000321 RID: 801
	public class NeuralNet
	{
		// Token: 0x0600113A RID: 4410 RVA: 0x00052838 File Offset: 0x00050A38
		public NeuralNet(int[] topology)
		{
			int num = topology.Length;
			this.m_layers = new Neuron[num][];
			for (int i = 0; i < num; i++)
			{
				bool flag = i == num - 1;
				int num2 = topology[i];
				if (flag)
				{
					this.m_layers[i] = new Neuron[num2];
				}
				else
				{
					this.m_layers[i] = new Neuron[num2 + 1];
				}
				int numOutputs = flag ? 0 : topology[i + 1];
				if (i < num - 1)
				{
					this.m_layers[i][num2] = new Neuron(topology[i + 1], num2);
					this.m_layers[i][num2].OutputValue = 1.0;
				}
				for (int j = 0; j < num2; j++)
				{
					if (i == 0)
					{
						this.m_layers[i][j] = new Neuron(numOutputs, j);
					}
					else
					{
						this.m_layers[i][j] = new Neuron(numOutputs, j, this.m_layers[i - 1]);
					}
				}
			}
		}

		// Token: 0x0600113B RID: 4411 RVA: 0x00052934 File Offset: 0x00050B34
		public void FeedForward(double[] inputValues, double bonusInput)
		{
			for (int i = 0; i < inputValues.Length; i++)
			{
				this.m_layers[0][i].OutputValue = inputValues[i];
			}
			for (int j = 1; j < this.m_layers.Length; j++)
			{
				int num = (j == this.m_layers.Length - 1) ? this.m_layers[j].Length : (this.m_layers[j].Length - 1);
				for (int k = 0; k < num; k++)
				{
					if (j == this.m_layers.Length - 1)
					{
						this.m_layers[j][k].FeedForward(bonusInput * this.m_bonusWeight);
					}
					else
					{
						this.m_layers[j][k].FeedForward();
					}
				}
			}
		}

		// Token: 0x0600113C RID: 4412 RVA: 0x000529DC File Offset: 0x00050BDC
		public double[] GetResults()
		{
			double[] array = new double[this.m_layers[this.m_layers.Length - 1].Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = this.m_layers[this.m_layers.Length - 1][i].OutputValue;
			}
			return array;
		}

		// Token: 0x0600113D RID: 4413 RVA: 0x00052A2C File Offset: 0x00050C2C
		public List<double> GetWeights()
		{
			List<double> list = new List<double>();
			for (int i = 0; i < this.m_layers.Length; i++)
			{
				bool flag = i == this.m_layers.Length - 1;
				for (int j = 0; j < this.m_layers[i].Length - (flag ? 0 : 1); j++)
				{
					list.AddRange(this.m_layers[i][j].GetWeights());
				}
			}
			list.Add(this.m_bonusWeight);
			return list;
		}

		// Token: 0x0600113E RID: 4414 RVA: 0x00052A9F File Offset: 0x00050C9F
		public void SetWeights(double[] weights)
		{
			this.SetWeights(new List<double>(weights));
		}

		// Token: 0x0600113F RID: 4415 RVA: 0x00052AB0 File Offset: 0x00050CB0
		public void SetWeights(List<double> weights)
		{
			int num = 0;
			for (int i = 0; i < this.m_layers.Length; i++)
			{
				bool flag = i == this.m_layers.Length - 1;
				int num2 = this.m_layers[i].Length - (flag ? 0 : 1);
				for (int j = 0; j < num2; j++)
				{
					num += this.m_layers[i][j].SetWeights(weights, num);
				}
			}
			this.m_bonusWeight = weights[num];
		}

		// Token: 0x04000FF1 RID: 4081
		private Neuron[][] m_layers;

		// Token: 0x04000FF2 RID: 4082
		private double m_bonusWeight = (double)Random.Range(0.2f, 1f);
	}
}
