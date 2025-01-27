﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using href.Utils;
using SubtitleTools.Common.EncodingHelper.Model;
using SubtitleTools.Common.Logger;

namespace SubtitleTools.Common.EncodingHelper
{
    //from: http://www.codeproject.com/KB/recipes/DetectEncoding.aspx
    public class DetectEncoding
    {
        #region Methods (2)

        // Public Methods (1)

        public static EncodingsInf DetectProbableFileCodepages(string filePath)
        {
            var result = new EncodingsInf();

            try
            {
                var fileBytes = File.ReadAllBytes(filePath);
                if (fileBytes.Length == 0) return result;

                var encList = EncodingTools.DetectInputCodepages(fileBytes, maxEncodings: 10);
                if (encList == null || encList.Length == 0)
                {
                    addAllEncodings(result);
                    return result;
                }

                foreach (var item in encList.OrderBy(e => e.EncodingName))
                {
                    result.Add(new EncodingInf { Name = item.EncodingName, BodyName = item.BodyName });
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogExceptionToFile(ex);
                addAllEncodings(result);
            }

            addWindows1256IfNotExists(result);

            return result;
        }

        // Private Methods (2)

        private static void addWindows1256IfNotExists(EncodingsInf result)
        {
            var windows1256 = result.FirstOrDefault(r => r.BodyName == "windows-1256");
            if (windows1256 == null)
            {
                result.Add(new EncodingInf { BodyName = "windows-1256", Name = "Arabic (Windows)" });
            }
        }

        private static void addAllEncodings(EncodingsInf result)
        {
            foreach (var item in Encoding.GetEncodings())
            {
                result.Add(new EncodingInf { Name = item.DisplayName, BodyName = item.Name });
            }
        }

        #endregion Methods
    }
}
