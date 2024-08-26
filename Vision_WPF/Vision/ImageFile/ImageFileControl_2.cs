using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cognex.VisionPro;
using Cognex.VisionPro.Display;
using Cognex.VisionPro.ImageFile;

namespace Vision
{
    public class ImageFileControl_2
    {
        public ImageFileControl_2() { }

        public async Task loadImage(string filePath, CogDisplay display)
        {
            using (var imageFileTool = new CogImageFileTool())
            {
                imageFileTool.Operator.Open(filePath, CogImageFileModeConstants.Read);
                imageFileTool.Run();

                await Task.Run(() => {
                    display.Image = imageFileTool.OutputImage.CopyBase(CogImageCopyModeConstants.CopyPixels);
                });
            }
        }
    }
}