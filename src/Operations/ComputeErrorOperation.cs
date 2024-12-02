using CudaHelioCommanderLight.Exceptions;
using CudaHelioCommanderLight.Models;
using System.Collections.Generic;
using System;
using System.Linq;

namespace CudaHelioCommanderLight.Operations
{
    public class ComputeErrorOperation : Operation<ErrorComputeModel, ComputedErrorModel>
    {
        public static new ComputedErrorModel Operate(ErrorComputeModel model)
        {
            var amsExecution = model.AmsExecution;
            var metricsConfig = model.MetricsConfig;
            var libraryItem = model.LibraryItem;
            var referenceTi = libraryItem.Spe1e3List;
            var eta = new List<double>();

            var computedErrorModel = new ComputedErrorModel()
            {
                Error = double.NaN,
                MaxError = double.NaN,
            };

            if (referenceTi == null || referenceTi.Count == 0)
            {
                return computedErrorModel;
            }

            if (referenceTi == null || referenceTi.Count == 0)
            {
                return computedErrorModel;
            }

            var computedTi = amsExecution.Spe1e3.ToList();

            for (int i = 0; i < amsExecution.Spe1e3.Count; i++)
            {
                eta.Add(0);
            }

            computedErrorModel.MaxError = 0;

            double sumUp = 0;
            double sumDown = 0;
            int etaIdx = 0;

            var firstRequiredValue = (double)((int)(metricsConfig.ErrorFromGev * 10) / 10.0);
            var lastRequiredValue = (double)((int)(metricsConfig.ErrorFromGev * 10) / 10.0);
            if (amsExecution.TKin.IndexOf(firstRequiredValue) == -1 || libraryItem.TKinList.IndexOf(firstRequiredValue) == -1)
            {
                throw new WrongConfigurationException($"Starting GeV value {firstRequiredValue} from configuration file is not found in library. Adjust configuration file first.");
            }
            else if (amsExecution.TKin.IndexOf(lastRequiredValue) == -1 || libraryItem.TKinList.IndexOf(lastRequiredValue) == -1)
            {
                throw new WrongConfigurationException($"Ending GeV value {lastRequiredValue} from configuration file is not found in library. Adjust configuration file first.");
            }

            for (double T = metricsConfig.ErrorFromGev; T <= metricsConfig.ErrorToGev; T += 0.1)
            {
                double TValue = (double)((int)(T * 10) / 10.0);
                int idx1 = amsExecution.TKin.IndexOf(TValue);
                int idx2 = libraryItem.TKinList.IndexOf(TValue);
                eta[etaIdx] = (computedTi[idx1] - referenceTi[idx2]) / referenceTi[idx2];

                sumUp += eta[etaIdx] * eta[etaIdx];
                sumDown++;
                etaIdx++;

                var ratio = Math.Abs(100 - ((computedTi[idx1] / referenceTi[idx2]) * 100));
                if (ratio > computedErrorModel.MaxError)
                {
                    computedErrorModel.MaxError = ratio;
                }
            }

            computedErrorModel.Error = Math.Sqrt(sumUp / sumDown);
            computedErrorModel.Error *= 100.0;

            return computedErrorModel;
        }
    }
}
