namespace FuzzyLib.Sets
{
    public class SingletonFuzzySet : FuzzySet
    {
        //the values that define the shape of this FLV
        private readonly double _midPoint;
        private readonly double _leftOffset;
        private readonly double _rightOffset;
        private readonly double _min;
        private readonly double _max;

        public override double GetMinBound()
        {
            return _min;
        }

        public override double GetMaxBound()
        {
            return _max;
        }

        public override double CalculateDegreeOfMembership(double value)
        {
            if ((value >= _midPoint - _leftOffset) &&
                 (value <= _midPoint + _rightOffset))
            {
                return 1.0;
            }

            //out of range of this FLV, return zero
            return 0.0;
        }


        public SingletonFuzzySet(double min,
                             double peak,
                             double max)
            : base(peak)
        {
            _min = min;
            _max = max;
            _midPoint = peak;
            _leftOffset = peak - min;
            _rightOffset = max - peak;
        }
    }
}
