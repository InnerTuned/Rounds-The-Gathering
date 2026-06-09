using System;
using UnityEngine;

namespace Landfall.AI
{
	// Token: 0x0200031F RID: 799
	public class Genome
	{
		// Token: 0x1700013C RID: 316
		// (get) Token: 0x06001125 RID: 4389 RVA: 0x0005261D File Offset: 0x0005081D
		// (set) Token: 0x06001126 RID: 4390 RVA: 0x00052625 File Offset: 0x00050825
		public double[] Genes
		{
			get
			{
				return this.m_genes;
			}
			set
			{
				this.m_genes = value;
			}
		}

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x06001127 RID: 4391 RVA: 0x0005262E File Offset: 0x0005082E
		// (set) Token: 0x06001128 RID: 4392 RVA: 0x00052636 File Offset: 0x00050836
		public float Fitness
		{
			get
			{
				return this.m_fitness;
			}
			set
			{
				this.m_fitness = value;
			}
		}

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x06001129 RID: 4393 RVA: 0x0005263F File Offset: 0x0005083F
		public int Size
		{
			get
			{
				return this.m_genes.Length;
			}
		}

		// Token: 0x0600112A RID: 4394 RVA: 0x00003CCC File Offset: 0x00001ECC
		public Genome()
		{
		}

		// Token: 0x0600112B RID: 4395 RVA: 0x00052649 File Offset: 0x00050849
		public Genome(double[] genes)
		{
			this.SetGenes(genes);
		}

		// Token: 0x0600112C RID: 4396 RVA: 0x00052658 File Offset: 0x00050858
		public Genome(Genome other)
		{
			this.m_genes = other.m_genes;
			this.m_fitness = 0f;
		}

		// Token: 0x0600112D RID: 4397 RVA: 0x00052678 File Offset: 0x00050878
		public void GenerateIndividual()
		{
			NeuralNet neuralNet = new NeuralNet(PlayerAIDavid.topology);
			this.m_genes = neuralNet.GetWeights().ToArray();
		}

		// Token: 0x0600112E RID: 4398 RVA: 0x000526A4 File Offset: 0x000508A4
		public void Mutate(float mutationRate)
		{
			for (int i = 0; i < this.m_genes.Length; i++)
			{
				if (Random.value < mutationRate)
				{
					Random r = new Random();
					this.m_genes[i] += r.NextGaussianDouble();
				}
			}
		}

		// Token: 0x0600112F RID: 4399 RVA: 0x000526E8 File Offset: 0x000508E8
		public double GetGene(int index)
		{
			return this.m_genes[index];
		}

		// Token: 0x06001130 RID: 4400 RVA: 0x000526F2 File Offset: 0x000508F2
		public void SetGene(int index, double value)
		{
			this.m_genes[index] = value;
			this.m_fitness = 0f;
		}

		// Token: 0x06001131 RID: 4401 RVA: 0x00052625 File Offset: 0x00050825
		public void SetGenes(double[] genes)
		{
			this.m_genes = genes;
		}

		// Token: 0x04000FEE RID: 4078
		private double[] m_genes;

		// Token: 0x04000FEF RID: 4079
		private float m_fitness;
	}
}
