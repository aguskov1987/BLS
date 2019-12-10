namespace BLS
{
    public class BlBinaryExpression
    {
        public BlBinaryExpression Right { get; set; }
        public BlOperator Operator { get; set; }
        public BlBinaryExpression Left { get; set; }

        public bool IsLeaf { get; set; }
        public string PropName { get; set; }
        public object Value { get; set; }
    }
}