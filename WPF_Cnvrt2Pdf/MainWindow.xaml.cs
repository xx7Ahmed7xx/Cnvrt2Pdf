using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.IO;
using Syncfusion.Drawing;
using System.Diagnostics;
using System.Threading;

namespace WPF_Cnvrt2Pdf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Ookii.Dialogs.Wpf.VistaFolderBrowserDialog oVFBD = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
        
        Ookii.Dialogs.Wpf.VistaOpenFileDialog oVOFDImg = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();
        List<string> ImgPaths = new List<string>();
        Ookii.Dialogs.Wpf.VistaOpenFileDialog oVOFDPdf = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();
        List<string> PdfPaths = new List<string>();
        Ookii.Dialogs.Wpf.VistaOpenFileDialog oVOFDTxt = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();
        List<string> TxtPaths = new List<string>();

        List<BitmapImage> myLoadedImgs = new List<BitmapImage>();

        string OutDir = "";


        public MainWindow()
        {
            InitializeComponent();
            oVOFDImg.Multiselect = true;
            oVOFDPdf.Multiselect = true;
            oVOFDTxt.Multiselect = true;
            oVOFDImg.Filter = "Images Files|*.jpg;*.jpeg;*.png";
            oVOFDPdf.Filter = "Pdf Files|*.pdf";
            oVOFDTxt.Filter = "Text Files|*.txt;*.docx";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            oVFBD.ShowDialog();
            OutDir = oVFBD.SelectedPath;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if ((bool)rdioImg.IsChecked)
            {
                if ((bool)oVOFDImg.ShowDialog())
                {
                    ImgPaths.AddRange(oVOFDImg.FileNames);
                    foreach (var file in oVOFDImg.FileNames)
                    {
                        string selectedFileName = file;
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.None;
                        bitmap.UriSource = new Uri(selectedFileName);
                        bitmap.EndInit();
                        myLoadedImgs.Add(bitmap);
                        //ImageViewer.Source = bitmap;
                    }
                }
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (myLoadedImgs.Count() >= 1)
            {
                int currentImage = myLoadedImgs.FindIndex(x => x == ImageViewer.Source);
                if (currentImage == -1)
                {
                    ImageViewer.Source = myLoadedImgs[0];
                }
                else if (myLoadedImgs.Count - 1 == currentImage)
                {
                    ImageViewer.Source = myLoadedImgs[currentImage];
                }
                else if (myLoadedImgs.Count > currentImage)
                {
                    ImageViewer.Source = myLoadedImgs[currentImage+1];
                }
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (myLoadedImgs.Count() >= 1)
            {
                int currentImage = myLoadedImgs.FindIndex(x => x == ImageViewer.Source);
                if (currentImage == -1)
                {
                    ImageViewer.Source = myLoadedImgs[0];
                }
                else if (myLoadedImgs.Count + 1 == currentImage)
                {
                    ImageViewer.Source = myLoadedImgs[currentImage];
                }
                else if (0 < currentImage)
                {
                    ImageViewer.Source = myLoadedImgs[currentImage - 1];
                }
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            //Create a new PDF document
            PdfDocument doc = new PdfDocument();

            //Add the first section to the PDF document
            PdfSection section = doc.Sections.Add();

            for (int i = 0; i < myLoadedImgs.Count(); i++)
            {
                // Old code for scaling PDF to be as image size.
                /*
                //Set margin for section
                section.PageSettings.Margins.All = 0;

                //Load the image from the disk
                PdfBitmap image = new PdfBitmap(new MemoryStream(File.ReadAllBytes(myLoadedImgs[i].UriSource.OriginalString)));

                //Initialize unit converter
                PdfUnitConverter converter = new PdfUnitConverter();
                //Convert image size from pixel to points
                SizeF size = converter.ConvertFromPixels(image.PhysicalDimension, PdfGraphicsUnit.Pixel);
                //Set section size based on the image size
                section.PageSettings.Size = size;
                // Set section orientation based on image size (by default Portrait) 
                if (image.Width > image.Height)
                    section.PageSettings.Orientation = PdfPageOrientation.Landscape;
                //Set margin for section
                section.PageSettings.Margins.All = 0;
                //Add a page to the section
                PdfPage page = section.Pages.Add();
                //Draw image
                page.Graphics.DrawImage(image, 0, 0);
                */

                // Now fixing PDF size to A4, and then scaling image accordingly..
                // A4 size in pixels : 	3508 x 2480 px.
                // Set margin for section
                section.PageSettings.Margins.All = 0;

                //Add a page to the document   
                PdfPage page = section.Pages.Add();

                // Load the image from the disk
                PdfBitmap image = new PdfBitmap(new MemoryStream(File.ReadAllBytes(myLoadedImgs[i].UriSource.OriginalString)));

                // Set section size to A4 size.
                section.PageSettings.Size = new SizeF(2480, 3508);

                //Set margin for section
                section.PageSettings.Margins.All = 20;

                // Create PDF Graphics.
                PdfGraphics graphics = page.Graphics;

                var tempImageBounds = new System.Drawing.Size((int)image.GetBounds().Width, (int)image.GetBounds().Height);
                var ScaledImageBounds = ResizeKeepAspect(tempImageBounds, 2480, 3508, true);

                //Draw the image   
                /*graphics.DrawImage(image, new PointF(
                    (2480 / 2) - ScaledImageBounds.Width,
                    (3508 / 2) - ScaledImageBounds.Height), ScaledImageBounds);*/
                graphics.DrawImage(image, new PointF(
                    0,
                    0), ScaledImageBounds);
            }

            string FileLocation = OutDir + "\\" + fileNameTxt.Text + ".pdf";

            //Save the document
            FileStream file = new FileStream(FileLocation, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            doc.Save(file);

            //Close the document
            doc.Close(true);

            doc.Dispose();

            Thread.Sleep(1000);
        }

        public static SizeF ResizeKeepAspect(System.Drawing.Size src, int maxWidth, int maxHeight, bool enlarge = false)
        {
            maxWidth = enlarge ? maxWidth : Math.Min(maxWidth, src.Width);
            maxHeight = enlarge ? maxHeight : Math.Min(maxHeight, src.Height);

            decimal rnd = Math.Min(maxWidth / (decimal)src.Width, maxHeight / (decimal)src.Height);
            return new SizeF((int)Math.Round(src.Width * rnd), (int)Math.Round(src.Height * rnd));
        }


    }
}
