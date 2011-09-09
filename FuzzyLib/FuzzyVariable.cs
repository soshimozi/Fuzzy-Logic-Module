using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyLib
{
    public class FuzzyVariable
    {
        private Dictionary<string, FuzzySet> _memberSets = new Dictionary<string,FuzzySet>();

        private double _minimumRange = 0;
        private double _maximumRange = 0;

        //this method is called with the upper and lower bound of a set each time a
        //new set is added to adjust the upper and lower range values accordingly
        private void AdjustRangeToFit(double min, double max)
        {
            _minimumRange = Math.Min(min, _minimumRange);
            _maximumRange = Math.Max(max, _maximumRange);
        }

        public FuzzySetProxy AddFuzzySet(
            string name, 
            FuzzySet set)
        {
            _memberSets.Add(name, set);
            AdjustRangeToFit(set.MinBound, set.MaxBound);
            return FuzzySetProxy.CreateProxyForSet(set);
        }

        //fuzzify a value by calculating its DOM in each of this variable's subsets
        public void Fuzzify(double value)
        {
            if (value < _minimumRange || value > _maximumRange)
                throw new ArgumentOutOfRangeException("value");


            //for each set in the flv calculate the DOM for the given value
            foreach (FuzzySet set in _memberSets.Values)
            {
                set.DegreeOfMembership = set.CalculateDegreeOfMovement(value);
            }
        }

        //defuzzify the variable using the max average method
        public double DeFuzzifyMaxAv()
        {
            double bottom = 0.0;
            double top = 0.0;

            foreach (FuzzySet set in _memberSets.Values)
            {
                bottom += set.DegreeOfMembership;
                top += set.RepresentativeValue * set.DegreeOfMembership;
            }

            //make sure bottom is not equal to zero
            if (bottom == 0) { return 0; }

            return top / bottom;  
        }

        //defuzzify the variable using the centroid method
        public double DeFuzzifyCentroid(int numSamples)
        {
            //calculate the step size
            double stepSize = (_maximumRange - _minimumRange) / (double)numSamples;

            double totalArea = 0.0;
            double sumOfMoments = 0.0;

            //step through the range of this variable in increments equal to StepSize
            //adding up the contribution (lower of CalculateDOM or the actual DOM of this
            //variable's fuzzified value) for each subset. This gives an approximation of
            //the total area of the fuzzy manifold.(This is similar to how the area under
            //a curve is calculated using calculus... the heights of lots of 'slices' are
            //summed to give the total area.)
            //
            //in addition the moment of each slice is calculated and summed. Dividing
            //the total area by the sum of the moments gives the centroid. (Just like
            //calculating the center of mass of an object)
            for (int samp = 1; samp <= numSamples; ++samp)
            {
                //for each set get the contribution to the area. This is the lower of the 
                //value returned from CalculateDOM or the actual DOM of the fuzzified 
                //value itself   
                foreach (FuzzySet set in _memberSets.Values)
                {
                    double contribution
                        = Math.Min(set.CalculateDegreeOfMovement(_minimumRange + (double)samp * stepSize), set.DegreeOfMembership);

                    totalArea += contribution;
                    sumOfMoments += (_minimumRange + (double)samp * stepSize) * contribution;
                }
            }

            //make sure total area is not equal to zero
            if (totalArea == 0) { return 0; }

            return (sumOfMoments / totalArea);
        }
    }
}
