using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CudaHelioCommanderLight.Operations
{
    public class GetAvailableGeliosphereLibRatiosOperation : Operation<string, List<string>>
    {
        public static new List<string> Operate(string libTypeName)
        {
            var libFilesPath = Path.Combine(Environment.CurrentDirectory, "libFiles");

            if (!Directory.Exists(libFilesPath))
            {
                Directory.CreateDirectory(libFilesPath);
            }

            var geliosphereRatios = new List<string>();
            var directories = Directory.GetDirectories(libFilesPath)
                                                .Select(Path.GetFileName)
                                                .ToList();

            foreach (string directoryName in directories)
            {
                if (directoryName.Contains(libTypeName))
                {
                    var numberStr = directoryName.Split('-').Last();
                    geliosphereRatios.Add(numberStr);
                }
            }

            return geliosphereRatios;
        }
    }
}
