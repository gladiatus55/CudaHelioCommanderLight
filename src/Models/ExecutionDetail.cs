using CudaHelioCommanderLight.Enums;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CudaHelioCommanderLight.Models
{
    public class ExecutionDetail
    {
        public bool IsSelected { get; set; }
        public string FolderName { get; set; }
        public bool IsGrid { get; set; }

        public MethodType MethodType { get; set; }

        public string K0Params
        {
            get
            {
                return ConvertListToString(paramK0);
            }
        }

        public string VParams
        {
            get
            {
                return ConvertListToString(paramV);
            }
        }

        public string DtParams
        {
            get
            {
                return ConvertListToString(paramDt);
            }
        }

        public string NParams
        {
            get
            {
                return ConvertListToString(paramN);
            }
        }

        public List<Execution> Executions { get; set; }

        // TODO: change visibility
        public List<double> paramK0;
        public List<double> paramV;
        public List<double> paramDt;
        public List<double> paramN;

        public int VSize { get; set; }
        public int DtSize { get; set; }
        public int K0Size { get; set; }
        public int NSize { get; set; }

        public int TotalNumberOfComputations
        {
            get
            {
                return VSize * DtSize * K0Size;
            }
        }

        public Execution GetExecutionByParam(double V, double k0)
        {
            return Executions.Where(x => x.V == V && x.K0 == k0).FirstOrDefault();
        }

        public ExecutionDetail()
        {
            FolderName = "dt=5.0,k0=5e22";
            IsGrid = true;
            Executions = new List<Execution>();
            paramK0 = new List<double>();
            paramV = new List<double>();
            paramDt = new List<double>();
            paramN = new List<double>();
            MethodType = MethodType.UNDEFINED;
            
        }

        public void AddK0(double K0Value)
        {
            paramK0.Add(K0Value);
        }

        public void AddV(double VValue)
        {
            paramV.Add(VValue);
        }

        public void AddDt(double DtValue)
        {
            paramDt.Add(DtValue);
        }

        public void AddN(double NValue)
        {
            paramN.Add(NValue);
        }

        private string ConvertListToString(List<double> list)
        {
            StringBuilder sb = new StringBuilder();
            bool firstRun = true;
            foreach (double val in list)
            {
                if (!firstRun)
                {
                    sb.Append(", ");
                }

                sb.Append(val);

                firstRun = false;
            }

            return sb.ToString();
        }

        // TODO: consider refactor
        public bool AddExecutionFilePathsToExecution(string localSubDirPath, string onlineSubDirPath, string localDirPath, string onlineDirPath)
        {
            foreach (Execution execution in Executions)
            {
                if (IsDirectoryRelatedToParams(localSubDirPath, execution))
                {
                    execution.OnlineParentDirPath = onlineDirPath;
                    execution.LocalParentDirPath = localDirPath;
                    execution.OnlineDirPath = onlineSubDirPath;
                    execution.LocalDirPath = localSubDirPath;
                    return true;
                }
            }
            return false;
        }


        private bool IsDirectoryRelatedToParams(string dirNamePath, Execution execution)
        {
            string n = execution.N.ToString().ToLower();
            string v = "v=" + execution.V.ToString().ToLower();
            string dt = "dt=" + execution.dt.ToString().ToLower();
            string k = "k0=" + execution.K0.ToString().ToLower();

            string dirName = Path.GetFileName(dirNamePath).ToLower();

            return (dirName.Contains(v) && dirName.Contains(dt) && dirName.Contains(k));
        }


        public List<Execution> GetLowestExecutions(int count)
        {
            List<Execution> firstNLowest = Executions.Where(x => x.ErrorValue != double.NaN).OrderBy(x => x.ErrorValue).Take(10).ToList();

            return firstNLowest;
        }
    }
}
