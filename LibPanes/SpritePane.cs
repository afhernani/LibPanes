using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Xml.Serialization;

namespace LibPanes
{
	/// <summary>
	/// Description of SpritePane.
	/// </summary>
	[Serializable, XmlRoot("SpritePane", Namespace = "", IsNullable = false)]
	public partial class SpritePane: UserControl
	{
		public SpritePane()
		{
			InitializeComponent();
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
			this.ResizeRedraw = true;
			Time = 1000;
			Accion = false;
			CurrentFrame = -1;
			SizeMode = PictureBoxSizeMode.Zoom;
			this.Invalidate(this.ClientRectangle);
		}

		private PictureBoxSizeMode _sizemode;
		[Category("Action")]
		[Description("Mode of visualizate the imagen into sprite-pane.")]
		public PictureBoxSizeMode SizeMode { get { return _sizemode; }
			set { _sizemode = value;
				this.Invalidate(this.ClientRectangle);
			}
		}

		[NonSerialized]
		private Thread t;
		[NonSerialized]
		private ImageGif _imagegif = null;
		[Category("Action")]
		[Description("int represent to active imagen in sprite-pane.")]
		private int CurrentFrame { get; set; }

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
		public Image GetImage
		{
			get
			{
				if (_imagegif != null)
				{
					return (Image)_imagegif.GetFrame(_imagegif.CurrentFrame).Clone();
				}
				else
				{
					return new Bitmap(this.Width, this.Height);
				}
			}
		}

		//string _filepath = String.Empty;
		[Category("Action")]
		[Description("load file to pas the string path file.")]
		public String FilePath {
			get
			{
				if (_imagegif != null)
					return _imagegif.Namefilegif;
				else
					return String.Empty;
			}

			set
			{
				if(System.IO.File.Exists(value))
					this.FileOpen(value);
			}
		}

		/// <summary>
		/// from file
		/// </summary>
		/// <param name="path"></param>
        public void FileOpen(string path)
		{
			if (Path.GetExtension(path).ToUpper() == ".gif".ToUpper())
			{
				this.SetImageGif = new ImageGif(path);
			}

			if (Path.GetExtension(path).ToUpper() == ".jpg".ToUpper()) {
				this.SetImageGif = new ImageGif();
				this.SetImageGif.AddImage((Image)Image.FromFile(path).Clone());
			}
		}

		[Category("Action")]
		[Description("set objet imagegif to spritepane component.")]
		public ImageGif SetImageGif
		{
			set
			{
				_imagegif = value;
				CurrentFrame = 0;
				Invalidate(this.ClientRectangle);
			}

			get
			{
				return _imagegif;
			}
		}

		private void ActionImagen()
		{
			do
			{
				using (PaintEventArgs e = new PaintEventArgs(this.CreateGraphics(), ClientRectangle))
				{
					OnPaint(e);
					//OnPaint(e);
				}

				Debug.WriteLine("dibujando image ...{" + _imagegif.CurrentFrame + "}");
				Thread.Sleep(Time);
			}
			while (Accion);
		}

		private bool Accion { get; set; }

		[Category("Action")]
		[Description("time in milisecons to renove imge in component.")]
		public int Time { get; set; }

		void SpritePane_MouseHover(object sender, EventArgs e)
		{
			if (t != null)
				if (t.IsAlive)
					return;
			if (_imagegif == null)
				return;
			System.Action action = ActionImagen;
			t = new Thread(ActionImagen);
			t.Start();
			Debug.WriteLine("SpritePane.MouseHove()...");
		}

		void SpritePane_MouseEnter(object sender, EventArgs e)
		{
			Debug.WriteLine("MouseEnter ...");
			Accion = true;
		}

		void SpritePane_MouseLeave(object sender, EventArgs e)
		{
			Debug.WriteLine("Mouseleave ...");
			Accion = false;
			if (t != null)
				t.Abort();
		}

		public void SaveGif(string pathfile)
		{
			if (_imagegif != null && Path.GetExtension(pathfile).ToUpper().Equals(".GIF"))
			{
				_imagegif.Time = Time;
				_imagegif.SaveImageGif(pathfile);
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Debug.WriteLine("OnPaint");
			if (_imagegif != null && Accion == true)
				DrawNextFrame(e);
			if (_imagegif != null && Accion == false)
				DrawCurrentFrame(e);
			base.OnPaint(e);
		}

		/// <summary>
		/// complementa OnPaint(PaintEventArgs e)
		/// </summary>
		/// <param name="e"></param>
        private void DrawNextFrame(PaintEventArgs e)
		{
			Image newImage = new Bitmap(this.Width, this.Height, PixelFormat.Format64bppPArgb);
			{
				switch (SizeMode)
				{
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
		}

		private void DrawCurrentFrame(PaintEventArgs e)
		{
			if (CurrentFrame == -1)
				CurrentFrame = 0;
			Image newImage = new Bitmap(this.Width, this.Height, PixelFormat.Format64bppPArgb);
			{
				switch (SizeMode)
				{
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
		}
        /// <summary>
        /// save all imagens contens in SpritePane
        /// </summary>
        /// <param name="path"></param>
        public void SaveAllImagens(string path)
        {
            if(_imagegif!=null)
                _imagegif.SaveAllImagenToFile(path);
        }
	}
}