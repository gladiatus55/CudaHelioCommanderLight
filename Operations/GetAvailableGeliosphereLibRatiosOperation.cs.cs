using CudaHelioCommanderLight.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CudaHelioCommanderLight.Operations
{
    public class GetAvailableGeliosphereLibRatiosOperation : Operation<string, List<decimal>>
    {
        public static new List<decimal> Operate(string libTypeName)
        {
            var libFilesPath = @"libFiles\";
            var geliosphereRatios = new List<decimal>();
            var directories = Directory.GetDirectories(libFilesPath)
                                                .Select(Path.GetFileName)
                                                .ToList();

            foreach (string directoryName in directories)
            {
                if (directoryName.Contains(libTypeName))
                {
                    var numberStr = directoryName.Split('-').Last();
                    if (MainHelper.TryConvertToDecimal(numberStr, out decimal number))
                    {
                        geliosphereRatios.Add(number);
                    }
                }
            }

            return geliosphereRatios;
        }
    }
}
