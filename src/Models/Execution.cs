using CudaHelioCommanderLight.Config;
using CudaHelioCommanderLight.Constants;
using CudaHelioCommanderLight.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CudaHelioCommanderLight.Models
{
    public class Execution
    {
        public double K0 { get; set; }
        public double dt { get; set; }
        public double V { get; set; }
        public double N { get; set; }
        public int Percentage { get; set; }
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
        public bool IsFullyDownloaded { get; set; }

        public Execution(double V, double K0, double N, double dt, MethodType methodType)
        {
            this.V = V;
            this.K0 = K0;
            this.N = N;
            this.dt = dt;
            this.MethodType = methodType;
            
            this.Percentage = 0;
            this.Error = double.NaN;
            this.IsFullyDownloaded = false;
        }

        public void SetError(double error)
        {
            this.Error = error;
        }

        public void ComputeError(OutputFileContent fileContent, MetricsConfig metricsConfig)
        {
            List<double> referenceTi = fileContent.Spe1e3List;
            List<double> eta = new List<double>();

            if (referenceTi == null || referenceTi.Count == 0)
            {
                this.Error = double.NaN;
                return;
            }

            if (referenceTi == null || referenceTi.Count == 0)
            {
                this.Error = double.NaN;
                return;
            }

            List<double> computedTi = new List<double>();

            if (MethodType == MethodType.BP_1D || MethodType == MethodType.BP_HELIUM_1D)
            {
                for (int i = 0; i < Spe1e3.Count; i++)
                {
                    computedTi.Add(Spe1e3[i] / Spe1e3N[i]);
                }
            }
            else
            {
                computedTi = Spe1e3.ToList();
            }

            for (int i = 0; i < Spe1e3.Count; i++)
            {
                eta.Add(0);
            }

            double sumUp = 0;
            double sumDown = 0;
            
            int etaIdx = 0;
            for (double T = metricsConfig.ErrorFromGev; T <= metricsConfig.ErrorToGev; T += 0.1)
            {
                double TValue = (int)(T * 10) / 10.0;
                int idx1 = TKin.IndexOf(TValue);
                int idx2 = fileContent.TKinList.IndexOf(TValue);
                eta[etaIdx] = (computedTi[idx1] - referenceTi[idx2]) / referenceTi[idx2];

                sumUp += eta[etaIdx] * eta[etaIdx];
                sumDown++;

                etaIdx++;
            }

            double etaRms = Math.Sqrt(sumUp / sumDown);

            etaRms *= 100.0;

            ErrorValue = etaRms;
            this.Error = etaRms;
        }

        public Execution()
        {

        }
    }
}
