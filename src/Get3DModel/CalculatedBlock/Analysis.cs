﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;

namespace CalculatedBlock
{
    public class Analysis : IAnalysis
    {
        List<Data.Point> pointAnalysis;
        Dictionary<Point, List<double>> gradients3x3;
        Dictionary<Point, List<double>> gradients5x5;
        Dictionary<Point, List<double>> gradients7x7;
        public Analysis(List<Data.Point> points)
        {
            pointAnalysis = points;
            gradients3x3 = new Dictionary<Point, List<double>>();
            gradients5x5 = new Dictionary<Point, List<double>>();
            gradients7x7 = new Dictionary<Point, List<double>>();
            foreach (var point in pointAnalysis)
            {
                gradients3x3.Add(point, new List<double>());
                gradients5x5.Add(point, new List<double>());
                gradients7x7.Add(point, new List<double>());
            }
        }

        public void addImageAnalysis(Image image)
        {
            IMathematical matematical3x3 = new MathematicialSearchPoint1();
            IMathematical matematical5x5 = new MathematicialSearchPoint8();
            IMathematical matematical7x7 = new MathematicialSearchPoint9();
            matematical3x3.setImage(image.image);
            matematical5x5.setImage(image.image);
            matematical7x7.setImage(image.image);
            foreach (var point in pointAnalysis)
            {
                gradients3x3[point].Add(matematical3x3.gradientAtPoint(point.x, point.y));
                gradients5x5[point].Add(matematical5x5.gradientAtPoint(point.x, point.y));
                gradients7x7[point].Add(matematical7x7.gradientAtPoint(point.x, point.y));
            }
        }

        public List<IMathematical> getCore()
        {
            List<IMathematical> cores = new List<IMathematical>();
            foreach (var point in pointAnalysis)
            {
                double dispersion3x3 = getDispersion(gradients3x3[point]);
                double dispersion5x5 = getDispersion(gradients5x5[point]);
                double dispersion7x7 = getDispersion(gradients7x7[point]);
                if (dispersion3x3 < dispersion5x5)
                {
                    if (dispersion3x3 < dispersion7x7)
                    {
                        cores.Add(new MathematicialSearchPoint1());
                    }
                    else
                    {
                        cores.Add(new MathematicialSearchPoint9());
                    }
                }
                else
                {
                    if (dispersion5x5 < dispersion7x7)
                    {
                        cores.Add(new MathematicialSearchPoint8());
                    }
                    else
                    {
                        cores.Add(new MathematicialSearchPoint9());
                    }
                }
            }
            return cores;
        }

        private double getDispersion(List<double> list)
        {
            list.Sort();
            int count = list.Count / 3;
            while (list.Count > count)
            {
                list.RemoveAt(0);
            }
            double probability = (double)1 / list.Count;
            double dispersion = 0;
            double expectedValue = 0;
            foreach (double element in list)
            {
                dispersion += Math.Pow(element, 2) * probability;
                expectedValue += element * probability;
            }
            return dispersion - Math.Pow(expectedValue, 2);
        }
    }
}