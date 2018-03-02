namespace FuzzyLib.Sets
{
    public class SingletonFuzzySet : IFuzzySet
    {
        //the values that define the shape of this FLV
        private readonly double _midPoint;
        private readonly double _leftOffset;
        private readonly double _rightOffset;
        private double _degreeOfMembership;

        public double MinBound
        {
            get;
            private set;
        }

        public double MaxBound
        {
            get;
            private set;
        }

        public double DegreeOfMembership
        {
            get => _degreeOfMembership;
            set => _degreeOfMembership = value.Range(0, 1.0d);
        }

        public double RepresentativeValue
        {
            get;
            private set;
        }

        public double CalculateDegreeOfMembership(double value)
        {
            if ((value >= _midPoint - _leftOffset) &&
                 (value <= _midPoint + _rightOffset))
            {
                return 1.0;
            }

            //out of range of this FLV, return zero
            return 0.0;
        }

        public void Clear()
        {
            _degreeOfMembership = 0.0d;
        }

        public SingletonFuzzySet(double min,
                                double max,
                                double peak)
        {
            RepresentativeValue = peak;
            MinBound = min;
            MaxBound = max;
            _midPoint = peak;
            _leftOffset = peak - min;
            _rightOffset = max - peak;
        }

        public void MergeWithDegreeOfMembership(double value)
        {
            if (value > _degreeOfMembership)
                _degreeOfMembership = value;
        }
    }
}
