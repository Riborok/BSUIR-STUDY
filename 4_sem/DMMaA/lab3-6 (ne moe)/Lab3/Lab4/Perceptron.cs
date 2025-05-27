using System;
using System.Collections.Generic;

namespace Lab4
{
    public class Perceptron
    {
        public class Vector
        {
            public List<int> Values= new List<int>();
        }

        public class TestClass
        {
            public List<Vector> Values = new List<Vector>();
        }
        
        private int _classCount;
        private const int IterationCap = 5000;
        private const int ParametrRange = 40;
        private const int Attributes = 2;
        private const int ObjectCount = 1;
        public List<Vector> _functions = new List<Vector>();
        public List<TestClass> _classes = new List<TestClass>();
        
        public Perceptron(int classCount)
        {
            _classCount = classCount;
        }

        public void Solve()
        {
            GetRandomClasses();
            SolveFuncs();
        }

        private void GetRandomClasses()
        {
            var rand = new Random();
            for (int i = 0; i < _classCount; i++)
            {
                var currentClass = new TestClass();
                
                for (int j = 0; j < ObjectCount; j++)
                {
                    var currentObject = new Vector();
                 
                    for (int k = 0; k < Attributes; k++)
                    {
                        currentObject.Values.Add(rand.Next(ParametrRange) - ParametrRange / 2 // + ParametrRange * i
                        );   
                    }

                    currentObject.Values.Add(1);
                    currentClass.Values.Add(currentObject);
                }

                _classes.Add(currentClass);
            }

            foreach (var _ in _classes)
            {
                var currentVector = new Vector();
                for (int i = 0; i < Attributes; i++)
                {
                    currentVector.Values.Add(0);
                }

                currentVector.Values.Add(0);
                _functions.Add(currentVector);
            }
        }

        private void SolveFuncs()
        {
            int n = 0;
            bool isNotClassifed = true;
            while (isNotClassifed && n < IterationCap)
            {
                bool iterationState = true;
                for (int i = 0; i < _classCount; i++)
                {
                    TestClass currentClass = _classes[i];
                    for (int j = 0; j < ObjectCount; j++)
                    {
                        Vector currentObject = currentClass.Values[j];
                        iterationState &= EditFuncs(currentObject, i);
                    }
                }

                isNotClassifed &= iterationState;
                n++;
            }
        }

        private bool EditFuncs(Vector currentObject, int currentClassTest)
        {
            bool result = true;
            int currentFuncVal = Val(currentObject, _functions[currentClassTest]);
            for (int i = 0; i < _functions.Count; i++) {
                if (i == currentClassTest){ continue;}

                int temp = Val(currentObject, _functions[i]);
                if (temp >= currentFuncVal)
                {
                    result = false;
                    Punish(currentObject, _functions[i], -1);
                }
            }
            if (!result)
            {
                Punish(currentObject,_functions[currentClassTest], 1);
            }
            return result;
        }

        private void Punish(Vector currentObject, Vector func, int sign)
        {
            for (int i = 0; i < currentObject.Values.Count; i++)
            {
                func.Values[i] += currentObject.Values[i] * sign;
            }
        }
        private int Val(Vector currentObject, Vector func)
        {
            int result = 0;
            for (int i = 0; i < currentObject.Values.Count; i++)
            {
                result += currentObject.Values[i] * func.Values[i];
            }
            return result;
        }
    }
}