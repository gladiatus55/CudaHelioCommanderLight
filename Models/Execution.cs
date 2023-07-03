using CudaHelioCommanderLight.Config;
using CudaHelioCommanderLight.Constants;
using CudaHelioCommanderLight.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CudaHelioCommanderLight.Models
{
    public class Execution
    {
        public double K0 { get; set; }
        public double dt { get; set; }
        public double V { get; set; }
        public double N { get; set; }
        public int Percentage { get; set; }
        //public string Error { get; set; }
        public double Error { get; set; }
        public MethodType MethodType { get; set; }

        public string OnlineUrl { get; set; }
        public string LocalUrl { get; set; }
        public string ImageUrl { get; set; }

        public double ErrorValue;
        public List<double> TKin { get; set; }
        public List<double> Spe1e3 { get; set; }
        public List<double> Spe1e3N { get; set; }
        public List<double> StandardDeviatons { get; set; }


        public string OnlineDirPath { get; set; }
        public string LocalDirPath { get; set; }
        public string OnlineParentDirPath { get; set; }
        public string LocalParentDirPath { get; set; }
        public string LocalThumbUrl
        {
            get
            {
                return Path.Combine(LocalDirPath, GlobalFilesToDownload.DetailSpe1e3GraphFile);
            }
        }
        //public string LocalThumbUrl { get; set; }
        public bool IsFullyDownloaded { get; set; }

        public Execution(double V, double K0, double N, double dt, MethodType methodType)
        {
            this.V = V;
            this.K0 = K0;
            this.N = N;
            this.dt = dt;
            this.MethodType = methodType;

            //this.Percentage = new Random().Next(0,100);

            this.Percentage = 0;
            //this.Error = "NaN";
            this.Error = double.NaN;
            this.IsFullyDownloaded = false;
        }

        public void SetError(double error)
        {
            //this.Error = error.ToString();
            this.Error = error;
        }

        //public void ComputeError(List<double> referenceTi)
        public void ComputeError(OutputFileContent fileContent, MetricsConfig metricsConfig)
        {
            List<double> referenceTi = fileContent.Spe1e3List;
            //List<double> referenceTi = fileContent.Spe1e3List.Select(x => x * 1.05).ToList();
            List<double> eta = new List<double>();

            if (referenceTi == null || referenceTi.Count == 0)
            {
                //this.Error = "Error Jref!";
                this.Error = double.NaN;
                return;
            }

            if (referenceTi == null || referenceTi.Count == 0)
            {
                //this.Error = "Error Jex!";
                this.Error = double.NaN;
                return;
            }

            List<double> computedTi = new List<double>();

            if (MethodType == MethodType.BP_1D || MethodType == MethodType.BP_HELIUM_1D)
            {
                for (int i = 0; i < Spe1e3.Count; i++)
                {
                    computedTi.Add(Spe1e3[i] / Spe1e3N[i]);
                    //computedTi.Add(Spe1e3[i] * 1.05 / Spe1e3N[i]);
                }
            }
            else
            {
                computedTi = Spe1e3.ToList();
            }

            //for (int i = 0; i < TKin.Count; i++)
            for (int i = 0; i < Spe1e3.Count; i++)
            {
                eta.Add(0);
            }

            double sumUp = 0;
            double sumDown = 0;

            //for (int i = 0; i < TKin.Count; i++)
            //for (int i = 0; i < 20; i++)
            //for (int i = 4; i < 20; i++)
            //for (int T = 5, i = 0; T <= 20; T++, i++)
            //for (int T = 40, i = 0; T <= 100; T++, i++)
            //for (int T = 200, i = 0; T <= 300; T++, i++)
            int etaIdx = 0;
            for (double T = metricsConfig.ErrorFromGev; T <= metricsConfig.ErrorToGev; T += 0.1)
            {
                //sumUp += Math.Pow(eta[i] / StandardDeviatons[i], 2);
                //sumDown += 1.0 / Math.Pow(StandardDeviatons[i], 2);

                //double TValue = (double)((int)(0.1 * T * 10)/10.0);
                double TValue = (double)((int)(T * 10) / 10.0);
                int idx1 = TKin.IndexOf(TValue);
                int idx2 = fileContent.TKinList.IndexOf(TValue);
                eta[etaIdx] = (computedTi[idx1] - referenceTi[idx2]) / referenceTi[idx2];

                sumUp += eta[etaIdx] * eta[etaIdx];
                sumDown++;

                etaIdx++;

                //eta[i] = (computedTi[i] - referenceTi[i]) / referenceTi[i];
                //sumUp += eta[i] * eta[i];
                //sumDown++;
                //eta[T] = (computedTi[T] - referenceTi[T]) / referenceTi[T];
                //sumUp += eta[T] * eta[T];
                //sumDown++;
            }

            double etaRms = Math.Sqrt(sumUp / sumDown);
            //double etaRms = Math.Sqrt(sumUp) / sumDown;

            etaRms *= 100.0;

            ErrorValue = etaRms;
            //this.Error = etaRms.ToString();
            this.Error = etaRms;
        }

        public Execution()
        {

        }
    }
}
