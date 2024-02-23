namespace GXPEngine
{
    public class Eggs
    {
        public static int[] eggs = new int[12];
        private int numOnes;
        private int numZeros;

        public int GetNumOnes()
        {
            return numOnes;
        }

        public int GetResult(int n)
        {
            return eggs[n];
        }
        
        public Eggs()
        {
            numOnes = 0;
            numOnes = 0;
            CreateArray();
        }

        // Next method first puts all 1s into array, then 0s, and then shuffles it, so we get random placement of the
        // specific number of 1s and 0s :)
        void CreateArray()
        {
            numOnes = Utils.Random(5, 10);
            numZeros = eggs.Length - numOnes;

            // Setting random number of 1s to the beginning array untill all 1s are assigned
            for (int i = 0; i < numOnes; i++)
            {
                eggs[i] = 1;
            }

            // Setting (array.Length - 1s amount) amount of 0 to the end of the array
            for (int i = numOnes; i < eggs.Length; i++)
            {
                eggs[i] = 0;
            }

            // We get the value from a random index and swap it with another value of a random index
            // We do it eggs.Length times
            for (int i = 0; i < eggs.Length; i++)
            {
                var randomIndex = Utils.Random(i, eggs.Length);
                var temp = eggs[i];
                eggs[i] = eggs[randomIndex];
                eggs[randomIndex] = temp;
            }
        }
    }
}