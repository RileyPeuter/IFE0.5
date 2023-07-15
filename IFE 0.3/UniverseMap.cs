using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFE_0._3
{
    internal class UniverseMap
    {
        //Might be more efficient to just have two int arrays
        public Universe[] map;
        public int[] targetU;
        public int[] voidU;

        public UniverseMap() {
            map = new Universe[10000];
    	    for(int x = 0; x <= 9999; x = x + 1){
                map[x] = new Universe(x); 
            }
            targetU = new int[10];
            voidU = new int[10];
            generateStates();
        }

    public void generateStates()
    {
        Random rng = new Random();

        int[] usedUniverses = new int[20];
        int processedCount = 0;
        while (processedCount < 10)
        {
            int testOutput = rng.Next(9999);
                    //Bro, what the fuck where you thinking here?
            if (Array.IndexOf(usedUniverses, testOutput) == -1){
                map[testOutput].state = UniverseState.target;
                targetU[processedCount] = testOutput;
                    Console.Write(testOutput);
                processedCount = processedCount + 1;
            }
        }
        while (processedCount < 20)
        {
            int testOutput = rng.Next(9999);
            if (Array.IndexOf(usedUniverses, testOutput) == -1)
            {
                map[testOutput].state = UniverseState.abyss;
                voidU[processedCount - 10] = testOutput;
                processedCount = processedCount + 1;
            }
        }
    }
}
}
