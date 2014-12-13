namespace FuzzyLib
{
    public class SingletonFuzzySet : FuzzySet
    {
        //the values that define the shape of this FLV
        private readonly double _midPoint;
        private readonly double _leftOffset;
        private readonly double _rightOffset;

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
            : base(peak, min, max)
        {
            _midPoint = peak;
            _leftOffset = peak - min;
            _rightOffset = max - peak;
        }
    }
}
