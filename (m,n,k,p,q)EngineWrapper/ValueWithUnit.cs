namespace _m_n_k_p_q_EngineWrapper
{
    public struct ValueWithUnit
    {
        public ValueWithUnit(double value, string unit)
        {
            Value = value;
            Unit = unit;
        }

        public double Value { get; }
        public string Unit { get; }

        public override string ToString()
        {
            return $"{Value} {Unit}";
        }
    }
}