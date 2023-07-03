using CudaHelioCommanderLight.Enums;
using System;
using System.Collections.Generic;

namespace CudaHelioCommanderLight.Models
{
    public class OutputFileContent
    {
        private Dictionary<OutputFileColumnType, List<double>> listDictionary;

        public List<double> TKinList
        {
            get
            {
                return listDictionary[OutputFileColumnType.TKin];
            }
            set
            {
                listDictionary[OutputFileColumnType.TKin] = value;
            }
        }
        public List<double> Spe1e3NList
        {
            get
            {
                return listDictionary[OutputFileColumnType.Spe1e3N];
            }
            set
            {
                listDictionary[OutputFileColumnType.Spe1e3N] = value;
            }
        }
        public List<double> Spe1e3List
        {
            get
            {
                return listDictionary[OutputFileColumnType.Spe1e3];
            }
            set
            {
                listDictionary[OutputFileColumnType.Spe1e3] = value;
            }
        }
        public List<double> StdDevList
        {
            get
            {
                return listDictionary[OutputFileColumnType.StdDeviation];
            }
            set
            {
                listDictionary[OutputFileColumnType.StdDeviation] = value;
            }
        }
        public List<double> WHLISList
        {
            get
            {
                return listDictionary[OutputFileColumnType.WHLIS];
            }
            set
            {
                listDictionary[OutputFileColumnType.WHLIS] = value;
            }
        }
        public List<double> OtherList
        {
            get
            {
                return listDictionary[OutputFileColumnType.Other];
            }
            set
            {
                listDictionary[OutputFileColumnType.Other] = value;
            }
        }

        public string FirstLine { get; set; }
        public int NumberOfColumns { get; set; }
        public string FilePath { get; set; }
        public bool IsValid { get; set; }

        public OutputFileContent()
        {
            listDictionary = new Dictionary<OutputFileColumnType, List<double>>();

            this.TKinList = new List<double>();
            this.Spe1e3List = new List<double>();
            this.Spe1e3NList = new List<double>();
            this.StdDevList = new List<double>();
            this.WHLISList = new List<double>();
            this.OtherList = new List<double>();
        }

        public void SwapLists(OutputFileColumnType oldType, OutputFileColumnType newType)
        {
            List<double> oldTypeList = listDictionary[oldType];

            listDictionary[oldType] = listDictionary[newType];
            listDictionary[newType] = oldTypeList;
        }

        public OutputFileColumnType GetColumnTypeByFirstValue(double firstValue)
        {
            foreach (KeyValuePair<OutputFileColumnType, List<double>> keyPair in listDictionary)
            {
                var list = keyPair.Value;
                if (list != null && list.Count > 0 && IsDoubleValueEqual(list[0], firstValue))
                {
                    return keyPair.Key;
                }
            }

            return OutputFileColumnType.Other;
        }

        private bool IsDoubleValueEqual(double val1, double val2)
        {
            return Math.Abs(val1 - val2) < 0.000001;
        }
    }
}
