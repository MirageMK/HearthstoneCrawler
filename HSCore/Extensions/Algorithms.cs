using System;

namespace HSCore.Extensions
{
    public class Algorithms
    {
        public static int LevenshteinDistance(string source, string target)
        {
            if(string.IsNullOrEmpty(source))
            {
                if(string.IsNullOrEmpty(target)) return 0;
                return target.Length;
            }
            if(string.IsNullOrEmpty(target)) return source.Length;

            if(source.Length > target.Length)
            {
                string temp = target;
                target = source;
                source = temp;
            }

            int m = target.Length;
            int n = source.Length;
            int[ , ] distance = new int[ 2, m + 1 ];
            // Initialize the distance 'matrix'
            for(int j = 1; j <= m; j++) distance[0, j] = j;

            int currentRow = 0;
            for(int i = 1; i <= n; ++i)
            {
                currentRow = i & 1;
                distance[currentRow, 0] = i;
                int previousRow = currentRow ^ 1;
                for(int j = 1; j <= m; j++)
                {
                    int cost = target[j - 1] == source[i - 1] ? 0 : 1;
                    distance[currentRow, j] = Math.Min(Math.Min(
                                                                distance[previousRow, j] + 1,
                                                                distance[currentRow, j - 1] + 1),
                                                       distance[previousRow, j - 1] + cost);
                }
            }
            return distance[currentRow, m];
        }
    }
}