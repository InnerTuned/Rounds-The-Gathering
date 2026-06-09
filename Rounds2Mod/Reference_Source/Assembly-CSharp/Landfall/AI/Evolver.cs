using System;
using System.Collections.Generic;
using UnityEngine;

namespace Landfall.AI
{
	// Token: 0x0200031E RID: 798
	public class Evolver
	{
		// Token: 0x06001121 RID: 4385 RVA: 0x00052414 File Offset: 0x00050614
		public Population Evolve(Population population)
		{
			Population population2 = new Population(population.Size, false);
			int num = 0;
			if (this.m_elitism)
			{
				num = population2.Size / 2;
				population.Sort();
				for (int i = 0; i < num; i++)
				{
					Genome genome = population.GetGenome(i);
					population2.SetGenome(i, genome);
					genome.Fitness = 0f;
				}
				for (int j = 0; j < population2.Size / 2; j++)
				{
					population2.SetGenome(j + num, new Genome(population.GetGenome(j)));
				}
			}
			else
			{
				for (int k = num; k < population2.Size; k++)
				{
					Genome parentA = this.Selection(population);
					Genome parentB = this.Selection(population);
					Genome g = this.Crossover(parentA, parentB);
					population2.SetGenome(k, g);
				}
			}
			for (int l = num; l < population2.Size; l++)
			{
				population2.GetGenome(l).Mutate(this.m_mutationRate);
			}
			return population2;
		}

		// Token: 0x06001122 RID: 4386 RVA: 0x00052504 File Offset: 0x00050704
		public Genome Crossover(Genome parentA, Genome parentB)
		{
			Genome genome = new Genome();
			int num = Random.Range(0, parentA.Size - 1);
			int num2 = Random.Range(0, parentA.Size - 1);
			for (int i = 0; i < genome.Size; i++)
			{
				if (num < num2 && i > num && i < num2)
				{
					genome.SetGene(i, parentA.GetGene(i));
				}
				else if (num > num2 && (i >= num || i <= num2))
				{
					genome.SetGene(i, parentA.GetGene(i));
				}
				else
				{
					genome.SetGene(i, parentB.GetGene(i));
				}
			}
			return genome;
		}

		// Token: 0x06001123 RID: 4387 RVA: 0x00052590 File Offset: 0x00050790
		public Genome Selection(Population population)
		{
			List<Genome> list = new List<Genome>(this.m_selectionSize);
			for (int i = 0; i < this.m_selectionSize; i++)
			{
				list.Add(population.GetGenome(i));
			}
			Genome genome = list[0];
			for (int j = 0; j < this.m_selectionSize; j++)
			{
				Genome genome2 = population.GetGenome(j);
				if (genome2.Fitness > genome.Fitness)
				{
					genome = genome2;
				}
			}
			return genome;
		}

		// Token: 0x04000FEB RID: 4075
		private float m_mutationRate = 0.1f;

		// Token: 0x04000FEC RID: 4076
		private int m_selectionSize = 5;

		// Token: 0x04000FED RID: 4077
		private bool m_elitism = true;
	}
}
