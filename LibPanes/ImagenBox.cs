using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using LibUtility;
using Gif.Components;

namespace LibPanes
{
    public partial class ImagenBox : UserControl
    {
        #region propiedades
        /// <summary>
        /// Imagen actual.
        /// </summary>
        [Category("ImagenBox")]
        [Description("imagen represent a imagen has the ImagenBox.")]
        public Image Imagen { get; set; }

        private FrameDimension Dimension { get; set; }

        /// <summary>
        /// Total de imagens that gif can has
        /// </summary>
        private int _count;
        [Category("ImagenBox")]
        [Description("int represent how mach imagen has the ImagenBox.")]
        public int Count { get { return _count; } }

        [Category("ImagenBox")]
        [Description("int represent how show the next imagen in ImagenBox.")]
        public int CurrentFrame { get; set; }
        [Category("ImagenBox")]
        [Description("set Imagen to show")]
        public int SetCurrentFrame
        {
            get
            {
                return CurrentFrame;
            }
            set
            {
                if(value < Count)
                {
                    CurrentFrame = value;
                    GetFrame(CurrentFrame);
                }

            }
        }
        /// <summary>
        /// reverse at end
        /// </summary>
        [Category("ImagenBox")]
        [Description("bool represent how show the next imagen in ImagenBox.")]
        public bool Reverse { get; set; }

        [Category("ImagenBox")]
        [Description("int represent jump to next imagen in ImagenBox.")]
        private int Step { get; set; }
        private string _namepathfile;
        [Category("ImagenBox")]
        [Description("string represent path complete to imagen in ImagenBox. Allow to load Image FromFile")]
        public string NamePathFile
        {
            get
            {
                return String.IsNullOrEmpty(_namepathfile) ?"Ninguno" : _namepathfile;
            }
            set
            {
                _namepathfile = value;
                if (!String.IsNullOrEmpty(_namepathfile))
                    this.FromFile(value);
            }
        }

        [Category("ImagenBox")]
        [Description("milisegons represent time to next imagen in ImagenBox.")]
        public int Time { get; set; }

        /// <summary>
        /// variable privada almacena el modo de visualizacion
        /// </summary>
        private PictureBoxSizeMode _sizemode;
        [Category("ImagenBox")]
        [Description("Mode of visualizate the imagen into ImagenBox.")]
        public PictureBoxSizeMode SizeMode
        {
            get { return _sizemode; }
            set
            {
                _sizemode = value;
                this.Invalidate(this.ClientRectangle);
            }
        }

        List<ImageU> ImagenList = new List<ImageU>();

        [Category("ImagenBox")]
        [Description("presentation with action mouse.")]
        private bool Accion { get; set; } = false;

        [Category("ImagenBox")]
        [Description("Automatic presentation imagens.")]
        public bool Automatic { get; set; } = false;

        /// <summary>
		/// variable thread
		/// </summary>
		private Thread t;

        #endregion

        public ImagenBox()
        {
            InitializeComponent();
            this.SetStyle(System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
                System.Windows.Forms.ControlStyles.ResizeRedraw |
                System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer |
                System.Windows.Forms.ControlStyles.UserPaint, true);
            this.CausesValidation = false;
            CurrentFrame = -1;
            _count = 0;
            Reverse = false;
            Step = 1;
            Time = 800;
            NamePathFile = String.Empty;
            SizeMode = PictureBoxSizeMode.Zoom;
            Accion = false;
        }
        /// <summary>
        /// Reset ImagenList
        /// </summary>
        public void Reset()
        {
            ImagenList = new List<ImageU>();
            CurrentFrame = -1;
            _count = 0;
            Reverse = false;
            Step = 1;
            Time = 800;
            NamePathFile = String.Empty;
            SizeMode = PictureBoxSizeMode.Zoom;
            Accion = false;
        }

        #region presentacion

        private void GetNextFrame()
        {
            Debug.WriteLine("ImageBox_GetNextFrame()");
            CurrentFrame += Step;
            //if the animation reaches a boundary...
            if (CurrentFrame >= _count || CurrentFrame < 1)
            {
                if (Reverse)
                {
                    Step *= -1;
                    //...reverse the count
                    //apply it
                    CurrentFrame += Step;
                }
                else
                {
                    CurrentFrame = 0;
                    //...or start over
                }
            }

            GetFrame(CurrentFrame);
        }

        private void GetFrame(int index)
        {
            Debug.WriteLine("ImageBox_GetFrame()");
            //actualiza el indice.
            CurrentFrame = index;
            ImagenFromFile(index);
        }

        public void FromFile(string pathImage)
        {
            //se supone que existe.
            Debug.WriteLine("ImageBox_FromFile()");
            using (var stream = File.Open(pathImage, FileMode.Open))
            {
                Image img = Image.FromStream(stream);

                //ImageFormat formato = GetImagenFormat(img);
                //string ext = Path.GetExtension(pathImage).ToUpper();
                //if (ext == ".GIF") {
                //cargar valores del gif
                var dimension = new FrameDimension(img.FrameDimensionsList[0]);
                int iCount = img.GetFrameCount(dimension);
                for (int i = 0; i < iCount; i++)
                {
                    ImageU imgU = new ImageU(pathImage, Format.GIF, i);
                    ImagenList.Add(imgU);
                }

                _count = ImagenList.Count;
                img.Dispose();
            }

            //
            /*if (ext == ".JPG") {
				ImageU imgU = new ImageU(pathImage, Format.JPG, 0);
				ImagenList.Add(imgU);
				_count = ImagenList.Count;
			}*/

            //img.Dispose();
            if (Count <= 0) //no hay imagen que mostrar
                CurrentFrame = -1;
            else
                GetNextFrame();//se muestra la primera imagen cargada
            this.Invalidate(this.ClientRectangle);

            //todo: se extrae la conclusion que solo necesitmaos
            //el nombre del fichero y la frameCount de la imagen
            //que pretendemos extraer.
        }

        private void ImagenFromFile(int index)
        {
            //se supone que el indice está dentro de los parametros
            Debug.WriteLine("ImageBox_ImagenFromFile()");
            if (ImagenList == null || Count == 0)
                return;
            ImageU imgU = ImagenList[index];

            if (imgU.Format == Format.GIF)
                ImagenGifFromFile(index);
            if (imgU.Format == Format.JPG)
                Imagen = Image.FromFile(imgU.Name, true);

            this.Invalidate(this.ClientRectangle);
        }

        private void ImagenGifFromFile(int index)
        {
            Debug.WriteLine("ImageBox_ImagenGifFromFile()");
            ImageU imgU = ImagenList[index];
            try
            {
                Dictionary<Guid, ImageFormat> guidToImageFormatMap = new Dictionary<Guid, ImageFormat>() {
                    { ImageFormat.Bmp.Guid, ImageFormat.Bmp },
                    { ImageFormat.Gif.Guid, ImageFormat.Png },
                    { ImageFormat.Icon.Guid, ImageFormat.Png },
                    { ImageFormat.Jpeg.Guid, ImageFormat.Jpeg },
                    { ImageFormat.Png.Guid, ImageFormat.Png }
                };

                using (Image img = Image.FromFile(imgU.Name, true))
                {
                    //Check the image format to determine what
                    //format the image will be saved to the 
                    //memory stream in
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
                    var dimension = new FrameDimension(img.FrameDimensionsList[0]);
                    //int iCount = img.GetFrameCount(Dimension);
                    img.SelectActiveFrame(dimension, imgU.Value);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        img.Save(ms, imageFormat);
                        Imagen = Image.FromStream(ms);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    "Error type: " + ex.GetType().ToString() +
                    "\nMessage: " + ex.Message +
                    "\nError in " + MethodBase.GetCurrentMethod().Name + "\n"
                );
            }
        }

        #endregion
        #region Paint

        protected override void OnPaint(PaintEventArgs e)
        {
            Debug.WriteLine("ImageBox_OnPaint()");
            DrawCurrentFrame(e);
            base.OnPaint(e);
        }

        private void DrawCurrentFrame(PaintEventArgs e)
        {
            Debug.WriteLine("ImageBox_DrawCurrentFrame(): currentframe: " + CurrentFrame.ToString());
            if (Imagen == null || CurrentFrame == -1 || Count <= 0)
                return;
            Image newImage = new Bitmap(this.Width, this.Height, PixelFormat.Format64bppPArgb);

            try
            {
                switch (SizeMode)
                {
                    case PictureBoxSizeMode.Normal:
                        e.Graphics.DrawImage(Imagen, 0, 0,
                            new RectangleF(0, 0, this.Width, this.Height),
                            GraphicsUnit.Pixel);
                        break;
                    case PictureBoxSizeMode.Zoom:
                        e.Graphics.DrawImage(Utility.ResizeImage(Imagen, this.Width, this.Height, true), 0, 0,
                            new RectangleF(0, 0, this.Width, this.Height),
                            GraphicsUnit.Pixel);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    "Error type: " + ex.GetType().ToString() +
                    "\nMessage: " + ex.Message +
                    "\nError in " + MethodBase.GetCurrentMethod().Name + "\nCurrentFrame: " +
                    CurrentFrame.ToString() + "\ncount: " + Count.ToString() + "\n"
                );
            }

            newImage.Dispose();
        }

        #endregion
        #region Automatismo

        private void ActionImagen()
        {
            Debug.WriteLine("ImagenBox_ActionImagen()");
            try
            {
                while (Accion || Automatic)
                {
                    GetNextFrame();
                    //using (PaintEventArgs e = new PaintEventArgs(this.CreateGraphics(), ClientRectangle)) {
                    //DrawCurrentFrame(e);
                    //}
                    Thread.Sleep(Time);
                }

                ;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    "Error type: " + ex.GetType().ToString() +
                    "\nMessage: " + ex.Message +
                    "\nError in " + MethodBase.GetCurrentMethod().Name + "\n"
                );
            }
        }

        void ImagenBox_MouseHover(object sender, EventArgs e)
        {
            Debug.WriteLine("ImagenBox_MouseHover()");
            if (t != null)
                if (t.IsAlive)
                    return;
            //if (CurrentFrame == -1)
            //	return;
            if (Count <= 1)
                return;
            //System.Action action = ActionImagen;
            t = new Thread(ActionImagen);
            t.Start();
        }

        void ImagenBox_MouseEnter(object sender, EventArgs e)
        {
            Debug.WriteLine("ImagenBox_MouseEnter()");
            Accion = true;
        }

        void ImagenBox_MouseLeave(object sender, EventArgs e)
        {
            Debug.WriteLine("ImagenBox_MuseLeave()");
            //no mata la tarea si está en automatico
            if (Automatic) return;
            Accion = false;
            if (t != null)
                t.Abort();
        }

        #endregion

        #region save_colection_imagens

        Task tar;

        public void SaveGif(string pathfile)
        {
            tar = Task.Factory.StartNew(new Action(() => TasKSaveImageGif(pathfile)));
        }

        private void TasKSaveImageGif(string pathfile)
        {
            if (ImagenList.Count == 0 || ImagenList == null)
                return;
            Debug.WriteLine("Iniciando ... taskSaveImageGif()");
            try
            {
                ImageU imgu;
                List<ImageU> ImagenRedim = new List<ImageU>();
                string dir = Path.GetTempPath();
                string nametemp = Path.GetTempFileName();
                int wd = Imagen.Width;
                int hd = Imagen.Height;

                for (int i = 0; i < ImagenList.Count; i++)
                {
                    nametemp = Path.Combine(dir, Path.GetTempFileName());
                    imgu = ImagenList[i];

                    if (imgu.Format == Format.GIF)
                    {
                        using (Image img = Image.FromFile(imgu.Name, true))
                        {
                            var dimension = new FrameDimension(img.FrameDimensionsList[0]);
                            img.SelectActiveFrame(dimension, imgu.Value);
                            using (MemoryStream ms = new MemoryStream())
                            {
                                img.Save(ms, ImageFormat.Jpeg);
                                Image imgp = Image.FromStream(ms);
                                Image imgpaso = Utility.ResizeImage(imgp, wd, hd, true);
                                imgpaso.Save(nametemp + ".jpg", ImageFormat.Jpeg);
                                ImageU paso = new ImageU(nametemp + ".jpg", Format.GIF, 0);
                                ImagenRedim.Add(paso);
                                imgpaso.Dispose();
                                imgp.Dispose();
                            }
                        }
                    }

                    if (imgu.Format == Format.JPG)
                    {
                        using (Image img = Image.FromFile(imgu.Name, true))
                        {
                            Image imgpaso = Utility.ResizeImage(img, wd, hd, true);
                            imgpaso.Save(nametemp + ".jpg", ImageFormat.Jpeg);
                            ImageU paso = new ImageU(nametemp + ".jpg", Format.JPG, 0);
                            ImagenRedim.Add(paso);
                            imgpaso.Dispose();
                        }
                    }
                }

                AnimatedGifEncoder egif = new AnimatedGifEncoder();

                egif.Start(pathfile);
                egif.SetDelay(Time);
                egif.SetRepeat(0);

                for (int i = 0; i < ImagenRedim.Count; i++)
                {
                    using (var stream = File.Open(ImagenRedim[i].Name, FileMode.Open))
                    {
                        egif.AddFrame(Image.FromStream(stream));
                    }

                    //añadirla.
                }

                egif.Finish();

                for (int i = 0; i < ImagenRedim.Count; i++)
                {
                    File.Delete(ImagenRedim[i].Name);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    "Error type: " + ex.GetType().ToString() +
                    "\nMessage: " + ex.Message +
                    "\nError in " + MethodBase.GetCurrentMethod().Name + "\n"
                );
            }

            Debug.WriteLine("Finalizada la construccion del Gif: " + pathfile);
        }

        /// <summary>
        /// save current imagen to file sin comprobaciones
        /// de ningun tipo.
        /// </summary>
        /// <param name="path"></param>
        public void SaveCurrentImagen(string path)
        {
            Imagen.Save(path);
        }

        #endregion


    }

    #region enum_Format

    /// <summary>
	/// Enum Format.
	/// </summary>
	public enum Format
    {
        GIF,
        JPG,
        PNG,
        NAN
    }

    #endregion

    #region struct_ImageU

    /// <summary>
    /// clase estructura imageU
    /// almacena la direccion de la imagen
    /// </summary>
    public struct ImageU
    {
        public static ImageU Empty;
        //el nombre completo del fichero de imagen
        string _name;
        //tipod e fichero de imagen
        Format _format;
        //el indice dentro del fichero gif.
        int _value;
        public string Name { get { return _name; } set { _name = value; } }
        public int Value { get { return _value; } set { _value = value; } }
        public Format Format { get { return _format; } set { _format = value; } }

        static ImageU()
        {
            Empty = new ImageU();
            Empty.Name = String.Empty;
            Empty.Format = Format.NAN;
            Empty.Value = unchecked((int)double.NaN);
        }

        public ImageU(string name, Format format, int valor)
        {
            _name = name;
            _format = format;
            _value = valor;
        }

        // disable once ConvertToAutoProperty
    }

    #endregion  


}
