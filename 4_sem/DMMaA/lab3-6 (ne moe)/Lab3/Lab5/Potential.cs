using System;
using System.Collections.Generic;
using System.Threading;

namespace Lab5
{
    public class TestingObject
    { 
        public int X, Y, Class;

        public TestingObject(int x, int y, int cl)
        {
            X = x;
            Y = y;
            Class = cl;
        }
    }
    public class Potential
    {
        private int[] _solvingFunc = new[] {0, 0, 0, 0};
        private const int MaxCycleCounts = 1500;

        public int[] OutSolvingFucn()
        {
            return _solvingFunc;
        }
        private int[] GetPartialPotential(TestingObject obj)
        {
            int[] arr = {1, 0, 0, 0};
            arr[1] = 4 * obj.X;
            arr[2] = 4 * obj.Y;
            arr[3] = 16 * obj.X * obj.Y;
            return arr;
        }

        private int GetValue(TestingObject obj, int[] func)
        {
            int res = 0;
            res += func[0];
            res += obj.X * func[1];
            res += obj.Y * func[2];
            res += obj.X * obj.Y * func[3];
            return res;
        }
        
        private void ChangeSolvingFuncs(int[] a, int coef)
        {
            for (int i = 0; i < 4; i++)
            {
                _solvingFunc[i] += coef * a[i];
            }
        }

        private bool LearnOneObject(TestingObject obj)
        {
            bool correct = true;
            int val = GetValue(obj, _solvingFunc);
            if (val <= 0 && obj.Class == 1)
            {
                correct = false;
                ChangeSolvingFuncs(GetPartialPotential(obj),1);
            } else if (val > 0 && obj.Class == 2)
            {
                correct = false;
                ChangeSolvingFuncs(GetPartialPotential(obj), -1);
            }
            return correct;
        }

        private void Reset()
        {
            _solvingFunc = new[] {0, 0, 0, 0};

        }

        public void LearnCollection(List<TestingObject> objs)
        {
            Reset();
            objs.Sort((x, y) => x.Class.CompareTo(y.Class));
            _solvingFunc = GetPartialPotential(objs[0]);
            for (int i = 1; i < objs.Count; i++)
            {
                LearnOneObject(objs[i]);
            }

            bool isCorrect = false;
            int counter = 0;
            while (!isCorrect && counter <= MaxCycleCounts)
            {
                isCorrect = true;
                foreach (var obj in objs)
                {
                    isCorrect &= LearnOneObject(obj);
                }

                counter++;
            }
        }

    }
}