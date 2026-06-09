using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Landfall.AI
{
	// Token: 0x02000320 RID: 800
	public class Population
	{
		// Token: 0x1700013F RID: 319
		// (get) Token: 0x06001132 RID: 4402 RVA: 0x00052708 File Offset: 0x00050908
		public int Size
		{
			get
			{
				return this.m_genomes.Count;
			}
		}

		// Token: 0x06001133 RID: 4403 RVA: 0x00052715 File Offset: 0x00050915
		public Population(int size) : this(size, true)
		{
		}

		// Token: 0x06001134 RID: 4404 RVA: 0x00052720 File Offset: 0x00050920
		public Population(int size, bool init)
		{
			for (int i = 0; i < size; i++)
			{
				this.m_genomes.Add(null);
			}
			if (init)
			{
				for (int j = 0; j < size; j++)
				{
					Genome genome = new Genome();
					genome.GenerateIndividual();
					this.m_genomes[j] = genome;
				}
			}
		}

		// Token: 0x06001135 RID: 4405 RVA: 0x0005277E File Offset: 0x0005097E
		public Genome GetGenome(int index)
		{
			return this.m_genomes[index];
		}

		// Token: 0x06001136 RID: 4406 RVA: 0x0005278C File Offset: 0x0005098C
		public Genome GetRandomGenome()
		{
			return this.m_genomes[Random.Range(0, this.m_genomes.Count)];
		}

		// Token: 0x06001137 RID: 4407 RVA: 0x000527AA File Offset: 0x000509AA
		public void SetGenome(int index, Genome g)
		{
			this.m_genomes[index] = g;
		}

		// Token: 0x06001138 RID: 4408 RVA: 0x000527BC File Offset: 0x000509BC
		public Genome GetFittest()
		{
			Genome genome = this.m_genomes[0];
			for (int i = 1; i < this.m_genomes.Count; i++)
			{
				Genome genome2 = this.m_genomes[i];
				if (genome2.Fitness > genome.Fitness)
				{
					genome = genome2;
				}
			}
			return genome;
		}

		// Token: 0x06001139 RID: 4409 RVA: 0x0005280A File Offset: 0x00050A0A
		public void Sort()
		{
			Enumerable.OrderBy<Genome, float>(this.m_genomes, (Genome g) => g.Fitness);
		}

		// Token: 0x04000FF0 RID: 4080
		private List<Genome> m_genomes = new List<Genome>();
	}
}
