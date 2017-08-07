using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
//using System.Windows.Media.Imaging;
using System.Collections.Generic;

namespace LibUtility
{
	public class Utility
	{
		public static Image BytesToImage(byte[] imageBytes)
		{
			if (imageBytes == null || imageBytes.Length == 0) {
				return (Image)(new Bitmap(100, 100)).Clone();
			}

			try {
				//Read bytes into a MemoryStream
				/*using (MemoryStream ms = new MemoryStream(imageBytes))
				{
					//Recreate the frame from the MemoryStream
                    using (Image bmp = Image.FromStream(ms))
					{
						return (Image)bmp.Clone();
					}
				}*/
				ImageConverter _imageConverter = new ImageConverter();
				Image XImage = (Image)_imageConverter.ConvertFrom(imageBytes);
				return (Image)XImage.Clone();
			} catch (Exception ex) {
				Debug.WriteLine(
					"Error type: " + ex.GetType().ToString() +
					"\nMessage: " + ex.Message +
					"\nError in " + MethodBase.GetCurrentMethod().Name
				);
			}

			return (Image)(new Bitmap(100, 100)).Clone();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="imagen"></param>
		/// <returns></returns>
		public static byte[] ImageToBytes(Image imagen)
		{
			if (imagen == null) {
				return BitConverter.GetBytes('f');
			}

			try {
				/*
                using (MemoryStream ms =new MemoryStream())
                {
                    imagen.Save(ms,ImageFormat.MemoryBmp);
                    return ms.ToArray();
                } */
				ImageConverter _imageConverter = new ImageConverter();
				byte[] xByte = (byte[])_imageConverter.ConvertTo(imagen, typeof(byte[]));
				return xByte;
			} catch (Exception ex) {
				Debug.WriteLine(
					"Error type: " + ex.GetType().ToString() + "\n" +
					"\nMessage: " + ex.Message,
					"\nError in " + MethodBase.GetCurrentMethod().Name + "\n"
				);
			}

			return BitConverter.GetBytes('f');
			;
		}

		/*
        public static BitmapSource CreateBitmapSourceFromBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            using (MemoryStream memoryStream = new MemoryStream())
            {
                try
                {
                    // You need to specify the image format to fill the stream. 
                    // I'm assuming it is PNG
                    bitmap.Save(memoryStream, ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    BitmapDecoder bitmapDecoder = BitmapDecoder.Create(
                        memoryStream,
                        BitmapCreateOptions.PreservePixelFormat,
                        BitmapCacheOption.OnLoad);

                    // This will disconnect the stream from the image completely...
                    WriteableBitmap writable = new WriteableBitmap(bitmapDecoder.Frames.Single());
                    writable.Freeze();

                    return writable;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
*/
		/// <summary>
		/// Redimensionar imagen de forma proporcional.
		/// </summary>
		/// <param name="imgToResize"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		public static Image ResizeImage(Image imgToResize, System.Drawing.Size size)
		{
			try {
				int sourceWidth = imgToResize.Width;
				int sourceHeight = imgToResize.Height;

				float nPercent = 0;
				float nPercentW = 0;
				float nPercentH = 0;

				nPercentW = ((float)size.Width / (float)sourceWidth);
				nPercentH = ((float)size.Height / (float)sourceHeight);

				if (nPercentH < nPercentW)
					nPercent = nPercentH;
				else
					nPercent = nPercentW;

				int destWidth = (int)(sourceWidth * nPercent);
				int destHeight = (int)(sourceHeight * nPercent);

				Bitmap b = new Bitmap(destWidth, destHeight);
				Graphics g = Graphics.FromImage((Image)b);
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;

				g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
				g.Dispose();
				return (Image)b.Clone();
			} catch (Exception ex) {
				Debug.WriteLine(
					"Error type: " + ex.GetType().ToString() + "\n" +
					"\nMessage: " + ex.Message,
					"\nError in " + MethodBase.GetCurrentMethod().Name + "\n"
				);
			}

			return (Image)(new Bitmap(100, 100)).Clone();
		}

		/// <summary>
		/// hacer una copia de una imagen.
		/// </summary>
		/// <param name="img"></param>
		/// <param name="cropArea"></param>
		/// <returns></returns>
		public static Image CopyImage(Image img, Rectangle cropArea)
		{
			try {
				Bitmap bmpImage = new Bitmap(img);
				Bitmap bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
				return (Image)(bmpCrop.Clone());
			} catch (Exception ex) {
				Debug.WriteLine(
					"Error type: " + ex.GetType().ToString() + "\n" +
					"\nMessage: " + ex.Message,
					"\nError in " + MethodBase.GetCurrentMethod().Name + "\n"
				);
			}
			return (Image)(new Bitmap(100, 100)).Clone();
		}

		/// <summary>
		/// Salvar a fichero una imagen en formato jpg
		/// </summary>
		/// <param name="path"></param>
		/// <param name="img"></param>
		/// <param name="quality"></param>
		public void SaveJPG(string path, Bitmap img, long quality)
		{
			try {
				// Encoder parameter for image quality
				EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

				// Jpeg image codec
				ImageCodecInfo jpegCodec = getEncoderInfo("image/jpeg");

				if (jpegCodec == null)
					return;

				EncoderParameters encoderParams = new EncoderParameters(1);
				encoderParams.Param[0] = qualityParam;

				img.Save(path, jpegCodec, encoderParams);
				qualityParam.Dispose();
				encoderParams.Dispose();
			} catch (Exception ex) {
				Debug.WriteLine(
					"Error type: " + ex.GetType().ToString() + "\n" +
					"\nMessage: " + ex.Message,
					"\nError in " + MethodBase.GetCurrentMethod().Name + "\n"
				);
			}
		}

		/// <summary>
		/// Obtener el codigo del tipo de la Imagen.
		/// si no existe devuelve parametro null.
		/// </summary>
		/// <param name="mimeType"></param>
		/// <returns></returns>
		private static ImageCodecInfo getEncoderInfo(string mimeType)
		{
			// Get image codecs for all image formats
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

			// Find the correct image codec
			for (int i = 0; i < codecs.Length; i++)
				if (codecs[i].MimeType == mimeType)
					return codecs[i];
			return null;
		}

		/// <summary>
		/// Redimensionar imagen con deformacion
		/// Ajusta una imagen a las dimensiones especificadas.
		/// </summary>
		/// <param name="srcImage"></param>
		/// <param name="newWidth"></param>
		/// <param name="newHeight"></param>
		/// <returns></returns>
		public static Image ResizeImage(Image srcImage, int newWidth, int newHeight)
		{
			using (Bitmap imagenBitmap =
				       new Bitmap(newWidth, newHeight, PixelFormat.Format32bppRgb)) {
				imagenBitmap.SetResolution(
					Convert.ToInt64(srcImage.HorizontalResolution),
					Convert.ToInt64(srcImage.HorizontalResolution));

				using (Graphics imagenGraphics =
					       Graphics.FromImage(imagenBitmap)) {
					imagenGraphics.SmoothingMode =
                       SmoothingMode.AntiAlias;
					imagenGraphics.InterpolationMode =
                       InterpolationMode.HighQualityBicubic;
					imagenGraphics.PixelOffsetMode =
                       PixelOffsetMode.HighQuality;
					imagenGraphics.DrawImage(srcImage,
						new Rectangle(0, 0, newWidth, newHeight),
						new Rectangle(0, 0, srcImage.Width, srcImage.Height),
						GraphicsUnit.Pixel);
					MemoryStream imagenMemoryStream = new MemoryStream();
					imagenBitmap.Save(imagenMemoryStream, ImageFormat.Jpeg);
					srcImage = Image.FromStream(imagenMemoryStream);
					imagenMemoryStream.Dispose();
				}
			}

			return srcImage;
		}

		/// <summary>
		/// Redimensionar una imagen sin deformacion, en el tamaño de cuadros predefinidos
		/// entero pix, representa la base sobre la dimension 4:3 o 16:9.
		/// corto, refiere al formato 4:3 true, false ->16:9
		/// En caso de error devuelve la misma imagen
		/// </summary>
		/// <param name="imgToResize"></param>
		/// <param name="pix"></param>
		/// <param name="corto"></param>
		/// <returns></returns>
		public static Image ResizeImage(Image imgToResize, int pix, bool corto)
		{
			System.Drawing.Size size;
			int _width;
			int _height;
			try {
				//dimensiones de imagenes 4:3 y 16:9 ¿? cual aplicamos
				if (corto) {
					_width = pix * 4;
					_height = pix * 3;
				} else {
					_width = pix * 16;
					_height = pix * 9;
				}

				size = new System.Drawing.Size(_width, _height);
				using (Image imgRedim = ResizeImage(imgToResize, size)) {
					Bitmap marco = new Bitmap(_width, _height);
					Graphics gc = Graphics.FromImage((Image)marco);
					gc.InterpolationMode = InterpolationMode.HighQualityBicubic;

					if (imgRedim.Height == _height) {
						if (imgRedim.Width == _width) {
							gc.DrawImage(imgRedim, 0, 0);
						} else {
							gc.DrawImage(imgRedim, (_width - imgRedim.Width) / 2, 0);
						}
					} else if (imgRedim.Width == _width) {
						gc.DrawImage(imgRedim, 0, (_height - imgRedim.Height) / 2);
					} else {
						gc.DrawImage(imgRedim, (_width - imgRedim.Width) / 2, (_height - imgRedim.Height) / 2);
					}
				
					gc.Dispose();
					return (Image)marco;
				}
			} catch (Exception ex) {
				Debug.WriteLine(
                    "Error type: " + ex.GetType().ToString() + "\n" +
                    "\nMessage: " + ex.Message,
                    "\nError in " + MethodBase.GetCurrentMethod().Name +"\n"
                   );
			}
			//devolvemos la misma imagen si hay un error
			return imgToResize;
		}

		/// <summary>
		/// redimensionar una imagen sin deformacion.
		/// center true or falce.
		/// </summary>
		/// <param name="imgToResize"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="center"></param>
		/// <returns></returns>
		public static Image ResizeImage(Image imgToResize, int width, int height, bool center)
		{
			System.Drawing.Size size;
			int _width = width;
			int _height = height;
			try {
				size = new System.Drawing.Size(_width, _height);
				using (Image imgRedim = ResizeImage(imgToResize, size)) {
					Bitmap marco = new Bitmap(_width, _height);
					Graphics gc = Graphics.FromImage((Image)marco);
					gc.InterpolationMode = InterpolationMode.HighQualityBicubic;
					gc.Clear(Color.Transparent);
					if (center) {
						if (imgRedim.Height == _height) {
							if (imgRedim.Width == _width) {
								gc.DrawImage(imgRedim, 0, 0);
							} else {
								gc.DrawImage(imgRedim, (_width - imgRedim.Width) / 2, 0);
							}
						} else if (imgRedim.Width == _width) {
							gc.DrawImage(imgRedim, 0, (_height - imgRedim.Height) / 2);
						} else {
							gc.DrawImage(imgRedim, (_width - imgRedim.Width) / 2, (_height - imgRedim.Height) / 2);
						}
					} else {
						gc.DrawImage(imgRedim, 0, 0);
					}

					gc.Dispose();
					return (Image)marco.Clone();
				}
			} catch (Exception ex) {
				Debug.WriteLine(
                    "Error type: " + ex.GetType().ToString() + "\n" +
                    "\nMessage: " + ex.Message,
                    "\nError in " + MethodBase.GetCurrentMethod().Name +"\n"
                   );
			}

			return imgToResize;
		}

		/// <summary>
		///  +++++ en desarrollo +++
		/// </summary>
		/// <param name="path"></param>
		/// <param name="img"></param>
		public static void saveGif(string path, Bitmap[] img)
		{
			/*
            try
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    System.Drawing.Size size = new System.Drawing.Size(180, 180);
                    Image[] img = new Image[frames.Count];
                    for (int i = 0; i < frames.Count; i++)
                    {
                        // ms.Write(frames[item],0,frames[item].Length);
                        img[i] = Utility.BytesToImage(frames[i]);
                        img[i] = Utility.resizeImage(img[i], size);
                    }
                    GifBitmapEncoder gEnc = new GifBitmapEncoder();
                    myTextBlock.Text = gEnc.CodecInfo.Author.ToString();

                    for (int i = 0; i < img.Length; i++)
                    {
                        //BitmapSource src = Utility.CreateBitmapSourceFromBitmap((Bitmap)img[i].Clone());
                        //Image uno = Image.FromHbitmap(((Bitmap)img[i].Clone()).GetHbitmap());

                        BitmapSource src = Imaging.CreateBitmapSourceFromHBitmap(
                            ((Bitmap)img[i].Clone()).GetHbitmap(),
                            IntPtr.Zero,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());

                        gEnc.Frames.Add(BitmapFrame.Create(src) as BitmapFrame);
                        //gEnc.Frames.Insert(i, BitmapFrame.Create(src) as BitmapFrame);
                    }

                    //myTextBlock.Text = gEnc.Frames.Count.ToString();
                    //string path = saveFileDialog1.FileName;

                    using (FileStream fs = new FileStream(path, FileMode.Create))
                    {
                        gEnc.Save(fs);
                        fs.Position = 0;
                        //Image paso = Image.FromStream(fs);
                        //paso.Save(path, ImageFormat.Gif);
                        fs.Close();
                    }
                    //GifImage imgif = 
                    //Image img = Image.FromStream(ms);
                    //img.FrameDimensionsList = frames.Count;
                    //string nameFile = saveFileDialog1.FileName;
                    //img.Save(nameFile, ImageFormat.Gif);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            //Bitmap myBitmap = new Bitmap(@"test.bmp");
            ImageCodecInfo myImageCodecInfo = getEncoderInfo("image/gif");
            EncoderParameter encCompressionrParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression, (long)EncoderValue.CompressionLZW);
            EncoderParameter encQualityParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 0L);
            EncoderParameters myEncoderParameters = new EncoderParameters(2);
            myEncoderParameters.Param[0] = encCompressionrParameter;
            myEncoderParameters.Param[1] = encQualityParameter;
            //myBitmap.Save("Output.gif", myImageCodecInfo, myEncoderParameters);
            */
		}

		/// <summary>
		/// Devuelve una imagen que es la diferencia entre las imagenes
		/// pasadas
		/// </summary>
		/// <param name="imgleft"></param>
		/// <param name="imgrignt"></param>
		/// <returns></returns>
		public static Image DifferenceTwoImages(Image imgleft, Image imgrignt)
		{
			int width = imgleft.Width;
			int height = imgleft.Height;
			if (width != imgrignt.Width && height != imgrignt.Height)
				imgrignt = ResizeImage(imgrignt, width, height, true);

			//obtenemos la dimension de la imagen imgleft y la comparamos con la
			//imgright.
			//si son iguales segimos, si no lo son igualamos la dimensiones de imgright
			//por deformacion o sin deformacion.
			Bitmap marco = new Bitmap(width, height);
			marco.MakeTransparent();
			// Loop through the images pixels to reset color.
			for (int x = 0; x < imgleft.Width; x++) {
				for (int y = 0; y < imgleft.Height; y++) {
					Color pixelColorL = ((Bitmap)imgleft).GetPixel(x, y);
					Color pixelColorR = ((Bitmap)imgrignt).GetPixel(x, y);
					if (!pixelColorL.Equals(pixelColorR))
						marco.SetPixel(x, y, pixelColorR);
				}
			}

			//crea una bitmap vacio que va almacenar la diferencia entre las imagenes
			//recorre los pixels de imgleft e imgright y si son diferentes guarda en el marco.
			//si son iguales continua.
			return (Image)marco.Clone();
		}

		/// <summary>
		/// Superpone una imagen sobre otra y devuelve el resultado.
		/// </summary>
		/// <param name="below"></param>
		/// <param name="above"></param>
		/// <returns></returns>
		public static Image PuttingUpImage(Image below, Image above)
		{
			//Bitmap marco = new Bitmap(below.Width, below.Height);
			Graphics gc = Graphics.FromImage(below);
			gc.InterpolationMode = InterpolationMode.HighQualityBicubic;
			gc.DrawImage(above, 0, 0);
			gc.Dispose();
			return (Image)below.Clone();
		}

		/// <summary>
		/// Salva un conjunto de imagenes como un fichero Tiff.
		/// </summary>
		/// <param name="outfilename"></param>
		/// <param name="listOriginals"></param>
		public static void SaveAsMultiPageTiff(string outfilename, List<Image> listOriginals)
		{
			Encoder encoder = Encoder.SaveFlag;
			ImageCodecInfo encoderInfo = ImageCodecInfo.GetImageEncoders().First(i => i.MimeType == "image/tiff");
			EncoderParameters encoderParameters = new EncoderParameters(1);
			encoderParameters.Param[0] = new EncoderParameter(encoder, (long)EncoderValue.MultiFrame);

			// Save the first frame of the multi page tiff
			Bitmap firstImage = (Bitmap)listOriginals[0];
			firstImage.Save(outfilename, encoderInfo, encoderParameters);

			encoderParameters.Param[0] = new EncoderParameter(encoder, (long)EncoderValue.FrameDimensionPage);

			// Add the remining images to the tiff
			for (int i = 1; i < listOriginals.Count; i++) {
				Bitmap img = (Bitmap)listOriginals[i];
				firstImage.SaveAdd(img, encoderParameters);
				img.Dispose();
			}

			// Close out the file
			encoderParameters.Param[0] = new EncoderParameter(encoder, (long)EncoderValue.Flush);
			firstImage.SaveAdd(encoderParameters);
			firstImage.Dispose();
		}

		/// <summary>
		/// Carga las distintas imagenes de un fichero tiff.
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public static List<Image> OpenMultiPageTiff(string file)
		{
			List<Image> images = new List<Image>();
			Bitmap bitmap = (Bitmap)Image.FromFile(file);
			int count = bitmap.GetFrameCount(FrameDimension.Page);
			for (int idx = 0; idx < count; idx++) {
				// save each frame to a bytestream
				bitmap.SelectActiveFrame(FrameDimension.Page, idx);
				MemoryStream byteStream = new MemoryStream();
				bitmap.Save(byteStream, ImageFormat.Tiff);

				// and then create a new Image from it
				images.Add(Image.FromStream(byteStream));
				byteStream.Dispose();
			}
			bitmap.Dispose();
			return images;
		}
	}
}