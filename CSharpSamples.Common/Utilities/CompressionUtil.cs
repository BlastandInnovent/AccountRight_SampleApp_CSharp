//  File:        CompressionUtil.cs
//  Copyright:   Copyright 2012 MYOB Technology Pty Ltd. All rights reserved.
//  Website:     http://www.myob.com
//  Author:      MYOB
//  E-mail:      info@myob.com
//
//Documentation, code and sample applications provided by MYOB Australia are for 
//information purposes only. MYOB Technology Pty Ltd and its suppliers make no 
//warranties, either express or implied, in this document. 
//
//Information in this document or code, including website references, is subject
//to change without notice. Unless otherwise noted, the example companies, 
//organisations, products, domain names, email addresses, people, places, and 
//events are fictitious. 
//
//The entire risk of the use of this documentation or code remains with the user. 
//Complying with all applicable copyright laws is the responsibility of the user. 
//
//Copyright 2012 MYOB Technology Pty Ltd. All rights reserved.

using System.Globalization;
using System.IO;
using System.IO.Packaging;

namespace CSharpSamples.Common.Utilities
{
    public class CompressionUtil
    {
        public static void ExtractZipFile(Stream archiveFilestream, string password, string outFolder)
        {
            using (var zipFilePackage = Package.Open(archiveFilestream))
            {
                foreach (var contentFile in zipFilePackage.GetParts())
                {
                    ExtractContentFile(outFolder, contentFile);
                }

                zipFilePackage.Close();
            }
        }

        private static void ExtractContentFile(string rootFolder, PackagePart contentFile)
        {
            // Initially create file under the folder specified
            string contentFilePath = contentFile.Uri.OriginalString.Replace('/', Path.DirectorySeparatorChar);

            if (contentFilePath.StartsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture)))
            {
                contentFilePath = contentFilePath.TrimStart(Path.DirectorySeparatorChar);
            }

            contentFilePath = Path.Combine(rootFolder, contentFilePath);
            
            //Check for the folder already exists. If not then create that folder
            if (!Directory.Exists(Path.GetDirectoryName(contentFilePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(contentFilePath));
            }

            using (var newFileStream = File.Create(contentFilePath))
            {
                newFileStream.Close();
                var content = new byte[contentFile.GetStream().Length];
                contentFile.GetStream().Read(content, 0, content.Length);
                File.WriteAllBytes(contentFilePath, content);
            }
        } 
    }
}
