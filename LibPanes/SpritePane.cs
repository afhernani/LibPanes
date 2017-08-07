/*
 * Creado por SharpDevelop.
 * Usuario: hernani
 * Fecha: 22/06/2017
 * Hora: 14:20
 * 
 * 
 */
using System;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.Versioning;
//using System.Xml.Serialization;

namespace LibPanes
{
	/// <summary>
	/// Description of SpritePane.
	/// </summary>
	//[Serializable, XmlRoot("SpritePane", Namespace = "", IsNullable = false)]
	public partial class SpritePane : UserControl
	{
		public SpritePane()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();

			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//

            this.SetStyle(System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
            System.Windows.Forms.ControlStyles.ResizeRedraw |
            System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer |
			System.Windows.Forms.ControlStyles.UserPaint, true);
			Time = 1000;
			Accion = false;
			CurrentFrame = -1;
			SizeMode = PictureBoxSizeMode.Zoom;
			//this.Invalidate(this.ClientRectangle);
			//ResizeRedraw = true;
		}

		/// <summary>
		/// variable privada almacena el modo de visualizacion
		/// </summary>
		private PictureBoxSizeMode _sizemode;
		[Category("Action")]
		[Description("Mode of visualizate the imagen into sprite-pane.")]
		public PictureBoxSizeMode SizeMode {
			get { return _sizemode; }
			set {
				_sizemode = value;
				//this.Invalidate(this.ClientRectangle);
			}
		}

		/// <summary>
		/// variable thread
		/// </summary>
		private Thread t;
		/// <summary>
		/// Contenedor ImageGif.
		/// </summary>
		private ImageGif _imagegif = null;
		[Category("Action")]
		[Description("int represent to active imagen in sprite-pane.")]
		public int CurrentFrame { get; set; }
		[Category("Action")]
		[Description("Total de imagenes en el contenedor imageGif.")]
		public int Count {
			get {
				if (_imagegif != null)
					return _imagegif.GetCount();
				else
					return 0;
			}
		}

		[Category("Action")]
		[Description("return active imagen in sprite-pane.")]
		public Image GetImage {
			get {
				if (_imagegif != null) {
					return (Image)_imagegif.GetFrame(_imagegif.CurrentFrame).Clone();
				}				else {
					return (Image)(new Bitmap(this.Width, this.Height)).Clone();
				}
			}
		}

		//string _filepath = String.Empty;
		[Category("Action")]
		[Description("load file to pas the string path file. route and name with extension")]
		public String FilePath {
			get {
				if (_imagegif != null)
					return _imagegif.Namefilegif;
				else
					return "Not";
			}

			set {
				if (System.IO.File.Exists(value)) {
					CurrentFrame = -1;
					this.FileOpen(value);
					//Invalidate(this.ClientRectangle);
				}
			}
		}

		private void PaintGif(object e)
		{
			Debug.WriteLine("SpritePane_PaintGif()");
			//Invalidate(this.ClientRectangle);
			//OnSizeChanged(new PaintEventArgs(this.CreateGraphics(), ClientRectangle));
			Invoke(new Action(() => Refresh()));
		}

		/// <summary>
		/// from file
		/// </summary>
		/// <param name="path"></param>
		public void FileOpen(string path)
		{
			Debug.WriteLine("SpritePane_FileOpen()");
			if (Path.GetExtension(path).ToUpper() == ".gif".ToUpper()) {
				this.SetImageGif = new ImageGif();
				this.SetImageGif.Paint += PaintGif;
				this.SetImageGif.Namefilegif = path;
			}

			if (Path.GetExtension(path).ToUpper() == ".jpg".ToUpper()) {
				this.SetImageGif = new ImageGif();
				this.SetImageGif.AddImage((Image)Image.FromFile(path).Clone());
				CurrentFrame++;
			}
		}

		[Category("Action")]
		[Description("set objet imagegif to spritepane component.")]
		public ImageGif SetImageGif {
			set {
				_imagegif = value;
				if (_imagegif != null)
					_imagegif.Paint += PaintGif;
				CurrentFrame = -1;
				//Invalidate(this.ClientRectangle);
			}

			get {
				return _imagegif;
			}
		}

		private void ActionImagen()
		{
			Debug.WriteLine("SpritePane_ActionImagen()");
			try {
				while (Accion) {
					using (PaintEventArgs e = new PaintEventArgs(this.CreateGraphics(), ClientRectangle)) {
						//OnPaint(e);
						//OnPaint(e);
						DrawNextFrame(e);
					}

					Debug.WriteLine("dibujando image ...{" + _imagegif.CurrentFrame + "}");
					Thread.Sleep(Time);
				}
			}
			catch (Exception ex) {
				Debug.WriteLine(
					"Error type: " + ex.GetType().ToString() +
					"\nMessage: " + ex.Message +
					"\nError in " + MethodBase.GetCurrentMethod().Name +"\n"
				);
			}
		}

		private bool Accion { get; set; }

		[Category("Action")]
		[Description("time in milisecons to renove imge in component.")]
		public int Time { get; set; }

		private void SpritePane_MouseHover(object sender, EventArgs e)
		{
			Debug.WriteLine("SpritePane_SpritePane_MouseHover()");
			if (t != null)
				if (t.IsAlive)
					return;
			if (_imagegif == null && CurremtFrame ==-1)
				return;
			if (Count == 1)
				return;
			t = new Thread(ActionImagen);
			t.Start();
		}

		#region onlayaut
		/// <summary>
		/// call base handler first, otherwise we get old
		/// dimenions, not the new ones.
		/// </summary>
		/// <param name="levent"></param>
		//protected override void OnLayout(LayoutEventArgs levent)
		//{
		//    Debug.WriteLine("SpritePane_OnLayout");
		//    // call the base handler first, otherwise we get old 
		//    //dimensions, not the new ones
		//    base.OnLayout(levent);
		//    OnSizeChanged(new PaintEventArgs(this.CreateGraphics(), ClientRectangle));
		//}
		//private void OnSizeChanged(PaintEventArgs e)
		//{
		//    Debug.WriteLine("SpritePane_OnSizeChanged()");
		//    //var g = this.CreateGraphics();
		//    //g.Clear(Color.White);
		//    //aqui no hay nada que dibujar solo actualizar la imagen
		//    using (PaintEventArgs g = new PaintEventArgs(e.Graphics, ClientRectangle))
		//    {
		//        DrawCurrentFrame(g);
		//    }
		//    //g.Dispose();
		//    Refresh();
		//}
		#endregion
        private void SpritePane_MouseEnter(object sender, EventArgs e)
		{
			Debug.WriteLine("SpritePane_SpritePane_MouseEnter()");
			Accion = true;
		}

		private void SpritePane_MouseLeave(object sender, EventArgs e)
		{
			Debug.WriteLine("SpritePane_SpritePaneMuseLeave()");;
			Accion = false;
			if (t != null)
				t.Abort();
		}

		public void SaveGif(string pathfile)
		{
			Debug.WriteLine("SpritePane_SaveGif()");
			if (_imagegif != null && Path.GetExtension(pathfile).ToUpper().Equals(".GIF")) {
				_imagegif.Time = Time;
				_imagegif.SaveImageGif(pathfile);
			}
		}

		//protected override void OnResize(EventArgs e)
		//{
		//    Debug.WriteLine("SpritePane_OnResize()");
		//    base.OnResize(e);
		//    OnSizeChanged();
		//}

		protected override void OnPaint(PaintEventArgs e)
		{
			Debug.WriteLine("SpritePane_OnPaint()");
			DrawCurrentFrame(e);
			base.OnPaint(e);
		}

		/// <summary>
		/// complementa OnPaint(PaintEventArgs e)
		/// </summary>
		/// <param name="e"></param>
        private void DrawNextFrame(PaintEventArgs e)
		{
			Debug.WriteLine("SpritePane_DrawNexFrame()");
			if (_imagegif == null) return;
			if (CurrentFrame == -1) return;
			Image newImage = new Bitmap(this.Width, this.Height, PixelFormat.Format64bppPArgb);

			try {
				switch (SizeMode) {
					case PictureBoxSizeMode.Normal:
						e.Graphics.DrawImage(_imagegif.GetNextFrame(), 0, 0,
							new RectangleF(0, 0, this.Width, this.Height),
							GraphicsUnit.Pixel);
					break;
					case PictureBoxSizeMode.Zoom:
						e.Graphics.DrawImage(LibUtility.Utility.ResizeImage(_imagegif.GetNextFrame(), this.Width, this.Height, true), 0, 0,
							new RectangleF(0, 0, this.Width, this.Height),
							GraphicsUnit.Pixel);
					break;
				}

				CurrentFrame = _imagegif.CurrentFrame;
			}
			catch (Exception ex) {
				newImage.Dispose();
				Debug.WriteLine(
					"Error type: " + ex.GetType().ToString() +
					"\nMessage: " + ex.Message+
					"\nError in " + MethodBase.GetCurrentMethod().Name + "\nCurrentFrame: " +
					CurrentFrame.ToString() + "\ncount: "+ Count.ToString() +"\n File: " + this.FilePath + "\n"
				);
			}

			newImage.Dispose();
		}

		private void DrawCurrentFrame(PaintEventArgs e)
		{
			Debug.WriteLine("SpritePane_DrawCurrentFrame()");
			if (_imagegif == null) return;
			if (CurrentFrame == -1)
				return;
			Image newImage = new Bitmap(this.Width, this.Height, PixelFormat.Format64bppPArgb);

			try {
				switch (SizeMode) {
					case PictureBoxSizeMode.Normal:
						e.Graphics.DrawImage(_imagegif.GetFrame(CurrentFrame), 0, 0,
							new RectangleF(0, 0, this.Width, this.Height),
							GraphicsUnit.Pixel);
					break;
					case PictureBoxSizeMode.Zoom:
						e.Graphics.DrawImage(LibUtility.Utility.ResizeImage(_imagegif.GetFrame(CurrentFrame), this.Width, this.Height, true), 0, 0,
							new RectangleF(0, 0, this.Width, this.Height),
							GraphicsUnit.Pixel);
					break;
				}
			}
			catch (Exception ex) {
				newImage.Dispose();
				Debug.WriteLine(
					"Error type: " + ex.GetType().ToString() +
					"\nMessage: " + ex.Message +
					"\nError in " + MethodBase.GetCurrentMethod().Name + "\nCurrentFrame: " +
					CurrentFrame.ToString() + "\ncount: "+ Count.ToString()+ "\n File: " + this.FilePath+"\n"
				);
			}

			newImage.Dispose();
		}

		/// <summary>
		/// save all imagens contens in SpritePane
		/// </summary>
		/// <param name="path"></param>
		public void SaveAllImagens(string path)
		{
			Debug.WriteLine("SpritePane_SaveAllImagens()");
			if (_imagegif != null)
				_imagegif.SaveAllImagenToFile(path);
		}

		public void SaveCurrentImage()
		{
			Debug.WriteLine("SpritePane_SaveCurrentImage()");
			if (!String.IsNullOrEmpty(FilePath)) {
				string rutayNombre = Path.Combine(Path.GetDirectoryName(FilePath), Path.GetFileNameWithoutExtension(FilePath));
				string ext = ".jpg";
				int _n = _imagegif.CurrentFrame;
				rutayNombre += _n + ext;
				_imagegif.SaveCurrentImagen(rutayNombre);
			}
		}
	}
}