﻿namespace NonVisuals.StreamDeck
{
    public enum EnumComparator
    {
        Equals = 0,
        NotEquals = 1,
        LessThan = 2,
        LessThanEqual = 3,
        BiggerThan = 4,
        BiggerThanEqual = 5

    }

    public class DCSBIOSNumberToText
    {
        private EnumComparator _comparator;
        private uint _referenceValue;
        private string _outputText;

        public string GetText(uint comparisonValue)
        {
            string result = null;

            switch (_comparator)
            {
                case EnumComparator.Equals:
                    {
                        if (comparisonValue == _referenceValue)
                        {
                            result = _outputText;
                        }
                        break;
                    }
                case EnumComparator.NotEquals:
                    {
                        if (comparisonValue != _referenceValue)
                        {
                            result = _outputText;
                        }
                        break;
                    }
                case EnumComparator.LessThan:
                    {
                        if (comparisonValue < _referenceValue)
                        {
                            result = _outputText;
                        }
                        break;
                    }
                case EnumComparator.LessThanEqual:
                    {
                        if (comparisonValue <= _referenceValue)
                        {
                            result = _outputText;
                        }
                        break;
                    }
                case EnumComparator.BiggerThan:
                    {
                        if (comparisonValue > _referenceValue)
                        {
                            result = _outputText;
                        }
                        break;
                    }
                case EnumComparator.BiggerThanEqual:
                    {
                        if (comparisonValue >= _referenceValue)
                        {
                            result = _outputText;
                        }
                        break;
                    }
            }

            return result;
        }

        public string GetFriendlyInfo
        {
            get
            {
                var comparator = "";

                switch (_comparator)
                {
                    case EnumComparator.Equals:
                    {
                        comparator = "==";
                        break;
                    }
                    case EnumComparator.NotEquals:
                    {
                        comparator = "!=";
                        break;
                    }
                    case EnumComparator.LessThan:
                    {
                        comparator = "<";
                        break;
                    }
                    case EnumComparator.LessThanEqual:
                    {
                        comparator = "<=";
                        break;
                    }
                    case EnumComparator.BiggerThan:
                    {
                        comparator = ">";
                        break;
                    }
                    case EnumComparator.BiggerThanEqual:
                    {
                        comparator = ">=";
                        break;
                    }
                }
                return " {dcsbios} " + " " + comparator + _referenceValue + " ==> " + OutputText;
            }
        }

        public EnumComparator Comparator
        {
            get => _comparator;
            set => _comparator = value;
        }

        public uint ReferenceValue
        {
            get => _referenceValue;
            set => _referenceValue = value;
        }

        public string OutputText
        {
            get => _outputText;
            set => _outputText = value;
        }
    }
}

