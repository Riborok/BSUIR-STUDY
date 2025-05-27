using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace LabN6
{
    public class Pair
    {
        public float R;
        public int X;
        public int Y;
        public int Mark;

        public Pair(float range, int first, int second, int mark)
        {
            R = range;
            X = first;
            Y = second;
            Mark = mark;
        }
    }
    public class Hierarchy
    {
        private Random _random = new Random();
        private int _randomRange = 250;
        private int _size = 0;
        private List<List<int>> _inputRanges = new List<List<int>>();

        public List<List<int>> GetRanges()
        {
            return _inputRanges;
        }
        public Hierarchy(int n)
        {
            SetN(n);
        }

        public void SetN(int n)
        {
            _size = n;
            GetRandomRanges();
        }

        private void GetRangesForNewClaster(int[][] ranges, bool[] used, int x, int y, int cur)
        {
            for (int i = 0; i <= cur; i++)
            {
                if (used[i] || i == cur)
                {
                    ranges[cur][i] = 0;
                    ranges[i][cur] = 0;
                    continue;
                }
                ranges[cur][i] = Math.Min(ranges[x][i], ranges[y][i]);
                ranges[i][cur] = ranges[cur][i];
            }
        }
        
        public List<Pair> Solve(bool maxVersion)
        {    
            int next = _size;
            int actualSize = _size * 2 - 1;
            int[][] ranges = PrepareRanges(actualSize);
            bool[] used = new bool[actualSize];
            List<Pair> res = new List<Pair>();
            while (next < actualSize)
            {
                int best = maxVersion ? int.MinValue : int.MaxValue;
                int bestX = 0;
                int bestY = 0;
                for (int j = 0; j < next; j++)
                {
                    if (used[j])
                    {
                        continue;
                    }
                    for (int i = j; i < next ; i++)
                    {
                        if (used[i] || i == j)
                        {
                            continue;
                        }
                        if (maxVersion ? ranges[i][j] > best : ranges[i][j] < best)
                        {
                            best = ranges[i][j];
                            bestX = i;
                            bestY = j;
                        }
                    }
                }

                used[bestX] = true;
                used[bestY] = true;
                
                res.Add(new Pair(maxVersion?1 /(float)best:best,bestX,bestY, next));
                GetRangesForNewClaster(ranges, used, bestX, bestY, next);

                next++;
            }
            
            return res;
        }

        private int[][] PrepareRanges(int size)
        {
            int[][] res = new int[size][];
            for (int i = 0; i < size; i++)
            {
                res[i] = new int[size];
            }

            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    res[i][j] = _inputRanges[i][j];
                }
            }
            return res;
        }
        private void GetRandomRanges()
        {
            bool[] used = new bool[_randomRange];
            int next = 0;
            _inputRanges.Clear();
            for (int j = 0; j < _size; j++)
            {
                List<int> temp = new List<int>();
                for (int i = 0; i < j; i++)
                {
                    temp.Add(_inputRanges[i][j]);
                }
                for (int i = j; i < _size ; i++)
                {
                    next = _random.Next(_randomRange - 1) + 1;
                    while (used[next])
                    {
                        next = _random.Next(_randomRange - 1) + 1;
                    }
                    used[next] = true;
                    temp.Add( i == j ? 0 : next);
                }
                _inputRanges.Add(temp);
            }
        }
        
    }
}