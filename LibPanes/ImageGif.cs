﻿/*
 * Creado por SharpDevelop.
 * Usuario: hernani
 * Fecha: 23/05/2017
 * Hora: 19:25
 * 
 * Para cambiar esta plantilla use Herramientas | Opciones | Codificación | Editar Encabezados Estándar
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Gif.Components;
using LibUtility;

namespace LibPanes
{
	/// <summary>
	/// Description of ImageGif.
	/// </summary>
	public class ImageGif
	{
		private Image GifImage { get; set; }
		//private Image Frame{ get; set; }
		private FrameDimension Dimension { get; set; }
        private int Count { get; set; } = 0;
		public int CurrentFrame { get; set; }
		private bool Reverse { get; set; }
		private int Step { get; set; }

		public int Time { get; set; }
		/// <summary>
		/// variable almacena ruta y nombre del fichero
		/// </summary>
		private string _namefile;
		Task open;
		/// <summary>
		/// load file to pas the string path file. route and name with extension
		/// and return same one.
		/// </summary>
		//[Category("Action")]
        //[Description("load file to pas the string path file. route and name with extension")]
		public string Namefilegif {
			get { return _namefile; }
			set
			{
				if(String.IsNullOrEmpty(value)) {
					_namefile = "newImagegif.gif";
				}
				else {
					if(Path.GetExtension(value).ToUpper() == ".gif".ToUpper()) {
						_namefile = value;
						frames.Clear();
						open=Task.Factory.StartNew( new Action(()=> EnumerateFrames(_namefile)));
					}else {
						//otro formato diferente
						_namefile = Path.Combine(Path.GetDirectoryName(value),"NewImgGif.git");
					}
				}
			}
		}

		/*public ImageGif(string path)
		{
			CurrentFrame = -1;
			Step = 1;
			Reverse = false;
			Time = 800;
			Namefilegif = path;
			/*GifImage = Image.FromFile(path);
			Dimension = new FrameDimension(GifImage.FrameDimensionsList[0]);
			Count = GifImage.GetFrameCount(Dimension);
		}*/

		public ImageGif()
		{
			CurrentFrame = -1;
			Step = 1;
			Reverse = false;
			Time = 800;
			//Count = 0;
			Namefilegif = String.Empty;
		}

		/// <summary>
		/// whether the gif should play backwards when it reaches the end
		/// </summary>
		public bool ReverseAtEnd {
			get { return Reverse; }
			set { Reverse = value; }
		}

		public Image GetNextFrame()
		{
			CurrentFrame += Step;

			//if the animation reaches a boundary...
			if (CurrentFrame >= Count || CurrentFrame < 1) {
				if (Reverse) {
					Step *= -1;
					//...reverse the count
					//apply it
					CurrentFrame += Step;
				}				else {
					CurrentFrame = 0;
					//...or start over
				}
			}

			return GetFrame(CurrentFrame);
		}

		/// <summary>
		/// Retorna la imagen del indice correspondiente.
		/// No existe verificacion de indice por lo que
		/// es conveniente que este este entre los valores 
		/// correctos.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Image GetFrame(int index)
		{
			//GifImage.SelectActiveFrame(Dimension, index);
			//find the frame
			//Frame=(Image)GifImage.Clone();
			//return Frame;
			CurrentFrame = index;
			if (CurrentFrame < Count && CurrentFrame >= 0)
			{
				return Utility.BytesToImage(frames[index]);
			}

			return (Image)(new Bitmap(200, 100){ }).Clone();
			//return a copy of it
		}

		/// <summary>
		/// retorna el numero de fames en imagegif
		/// -1 si no exite ninguna imagen
		/// </summary>
		/// <returns></returns>
		public int GetCount() {
			return Count;
		}

		/// <summary>
		/// lista de imagenes en bytes.
		/// </summary>
		List<byte[]> frames = new List<byte[]>() { };

		/// <summary>
		/// Extrae las imagenes del fichero gif en una lista de bytes
		/// </summary>
		/// <param name="imagePath"></param>
		private void EnumerateFrames(string imagePath)
		{
			try
			{
				//Make sure the image exists
                if (!File.Exists(imagePath))
				{
					throw new FileNotFoundException("Unable to locate " + imagePath);
				}

				Dictionary<Guid, ImageFormat> guidToImageFormatMap = new Dictionary<Guid, ImageFormat>()
				{
					{ ImageFormat.Bmp.Guid, ImageFormat.Bmp },
					{ ImageFormat.Gif.Guid, ImageFormat.Png },
					{ ImageFormat.Icon.Guid, ImageFormat.Png },
					{ ImageFormat.Jpeg.Guid, ImageFormat.Jpeg },
					{ ImageFormat.Png.Guid, ImageFormat.Png }
				};

				using (Image img = Image.FromFile(imagePath, true))
				{
					//Check the image format to determine what
					//format the image will be saved to the 
					//memory stream in
					Count = 0;
                    ImageFormat imageFormat = null;
					Guid imageGuid = img.RawFormat.Guid;

					foreach (KeyValuePair<Guid, ImageFormat> pair in guidToImageFormatMap)
					{
						if (imageGuid == pair.Key)
						{
							imageFormat = pair.Value;
							break;
						}
					}

					if (imageFormat == null)
					{
						throw new NoNullAllowedException("Unable to determine image format");
					}

					//Get the frame count
                    Dimension = new FrameDimension(img.FrameDimensionsList[0]);
                    int iCount = img.GetFrameCount(Dimension);

                    //Step through each frame
                    for (int i = 0; i < iCount; i++)
                    {
                    	//Set the active frame of the image and then
                    	//write the bytes to the tmpFrames array
                    	img.SelectActiveFrame(Dimension, i);
                    	using (MemoryStream ms = new MemoryStream()) {
                    		img.Save(ms, imageFormat);
                    		Action a = new Action(() =>frames.Add(ms.ToArray()));
                    		a.Invoke();
                    		Count = i + 1;
                    	}
                    	if (i == 0) {
                    		Action b = new Action(() => OnPaint(this));
                    		b.Invoke();
                    	}
                    }
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(
                    "Error type: " + ex.GetType().ToString() + 
                    "\nMessage: " + ex.Message +
                    "\nError in " + MethodBase.GetCurrentMethod().Name +"\n"
                  );
			}
		}

		#region addImagen
		/// <summary>
		/// adicionar una imagen a la lista de imagenes.
		/// </summary>
		/// <param name="imagen"></param>
        public void AddImage(Image imagen)
		{
			//Todo: la imagen debe ser redimensionada a dimension
			//de la primera imagen de la lista.
            AddImage(LibUtility.Utility.ImageToBytes(RedimImage(imagen)));
		}

		/// <summary>
		/// en ambito cerrado x falta de redimensionado de las imagenes
		/// </summary>
		/// <param name="imageBytes"></param>
        private void AddImage(byte[] imageBytes)
		{
			frames.Add(imageBytes);
			Count++;
			Debug.WriteLine("Adicionado de la imagen.");
		}

		private Image RedimImage(Image imagen)
		{
			Debug.WriteLine("Redimensionar Imagen ... ");
			if (frames.Count>0 && frames!= null)
			{
				Image size = Utility.BytesToImage(frames[0]);
				return (Image) LibUtility.Utility.ResizeImage(imagen, size.Width, size.Height, true).Clone();
			}
			else
			{
				return (Image) imagen.Clone();
			}
		}

		#endregion
        #region AddAnotherImageGif
		Task t;
        public void SaveImageGif(string pathfile)
		{
			t = Task.Factory.StartNew(new Action(() => TasKSaveImageGif(pathfile)));
		}
        public void TasKSaveImageGif(string pathfile)
        {
        	Image[] img = new Image[frames.Count];
			for (int i = 0; i < frames.Count; i++)
			{
				img[i] = Utility.BytesToImage(frames[i]);
			}

			AnimatedGifEncoder egif = new AnimatedGifEncoder();
			egif.Start(pathfile);
			egif.SetDelay(Time);
			egif.SetRepeat(0);
			for (int i = 0; i < img.Length; i++)
			{
				egif.AddFrame(img[i]);
			}

			egif.Finish();
			Namefilegif = pathfile;
			Debug.WriteLine("Finalizada la construccion del Gif: " + pathfile);
        }
		/// <summary>
		/// save all imagens saparate in files
		/// </summary>
		/// <param name="path"></param>
        public void SaveAllImagenToFile(string path)
		{
			//existe el directorio
            string dir = Path.GetDirectoryName(path);
			if (!Directory.Exists(dir)) return; //salimos no cierto
            string nameb = Path.GetFileNameWithoutExtension(path);
			if (String.IsNullOrEmpty(nameb)) return; //salimos x que no tenemos un nombre.
            string ext = Path.GetExtension(path);
            //todo: comprobar el tipo de extension si da el caso
            //
			//llegados aqui guardamos si existe algo.
            int n = 000;
			foreach (var item in frames)
			{
				Image paso = Utility.BytesToImage(item);
				paso.Save(Path.Combine(dir, nameb + n.ToString())+ext);
				n++;
			}
		}

		/// <summary>
		/// save current imagen to file sin comprobaciones
		/// de ningun tipo.
		/// </summary>
		/// <param name="path"></param>
        public void SaveCurrentImagen(string path)
		{
			Image pas = Utility.BytesToImage(frames[CurrentFrame]);
			pas.Save(path);
		}

		#endregion
		#region eventPaint
		public delegate void PaintHandler(object e);
		public event PaintHandler Paint;
		
		protected virtual void OnPaint(object e)
		{
			
			if (Paint != null)
				Paint(e);
		}

		#endregion
	}
}