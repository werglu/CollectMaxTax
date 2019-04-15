using System;
namespace ASD
{
    public enum TaxAction
    {
        Empty,
        TakeMoney,
        TakeCarrots
    }

    public class TaxCollectorManager : MarshalByRefObject
    {
      


        public int CollectMaxTax(int[] dist, int[] money, int[] carrots, int maxCarrots, int startingCarrots, out TaxAction[] collectingPlan)
        {
            collectingPlan = new TaxAction[dist.Length];
          

            int max = -2;
            (int, int)[,] tab = new (int, int)[dist.Length+1, maxCarrots + 2];

            for (int i = 0; i <= dist.Length; i++)
            {
                for (int j = 0; j < maxCarrots + 1; j++)
                    tab[i, j] = (-1, -1);
            }

            tab[0, startingCarrots] = (0, -1);

            for (int i=0; i<dist.Length-1; i++)
            {
               

                for (int j = 0; j < maxCarrots + 1; j++)
                {
                    if(tab[i, j].Item1!=-1)
                    {//bierzemy monety
                        if (j - dist[i + 1] >= 0)
                        {   
                            if ((tab[i, j].Item1 + money[i]) > tab[i + 1, j - dist[i + 1]].Item1)
                                tab[i + 1, j - dist[i + 1]] = (tab[i, j].Item1+money[i], j);
                        }

                        int pom = carrots[i];
                        if (j + carrots[i] > maxCarrots) pom = maxCarrots-j;
                        if (j + pom - dist[i + 1] >= 0)
                        {
                            if (tab[i, j].Item1> tab[i + 1, j + pom - dist[i + 1]].Item1)
                                tab[i + 1, j + pom - dist[i + 1]] = (tab[i, j].Item1, j);
                        }
                    }

                }
                
            }

            for (int j = 0; j < maxCarrots + 1; j++)
            {
                if (tab[dist.Length - 1, j].Item1 != -1)
                {
                    tab[dist.Length - 1, j] = (money[dist.Length - 1]+ tab[dist.Length - 1, j].Item1, tab[dist.Length - 1, j].Item2);
                    int pom = carrots[dist.Length - 1];
                    if (j + carrots[dist.Length - 1] > maxCarrots) pom = maxCarrots - j;
                    if (j + pom >= 0)
                    {
                        if (tab[dist.Length, j + pom].Item1 < tab[dist.Length - 1, j].Item1- money[dist.Length - 1])
                            tab[dist.Length, j + pom] = (tab[dist.Length - 1, j].Item1- money[dist.Length - 1], j);
                    }

                }
            }
            int index=0;
            for (int i=startingCarrots; i<maxCarrots+1;i++)
            {
                if (tab[dist.Length - 1, i].Item1 > max)
                {
                    max = tab[dist.Length - 1, i].Item1;
                    index = i;
                    collectingPlan[dist.Length - 1] = TaxAction.TakeMoney;
                }
                if (tab[dist.Length, i].Item1> max)
                {
                    max = tab[dist.Length, i].Item1;
                    index = tab[dist.Length, i].Item2;
                    collectingPlan[dist.Length - 1] = TaxAction.TakeCarrots;
                }
            }

            if (max == -1) { collectingPlan = null; return -1; }

            
                tab[dist.Length - 1, index] = (tab[dist.Length - 1, index].Item1 - money[dist.Length - 1], tab[dist.Length - 1, index].Item2);


                for (int i = dist.Length - 2; i >= 0; i--)
                {
                    int st = index;
                    index = tab[i + 1, index].Item2;
                    if (tab[i, index].Item1 == tab[i + 1, st].Item1) collectingPlan[i] = TaxAction.TakeCarrots;
                    else collectingPlan[i] = TaxAction.TakeMoney;
                }
            
                

                return max;
        }

    }
}
