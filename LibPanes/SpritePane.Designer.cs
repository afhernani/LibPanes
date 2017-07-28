namespace LibPanes
{
    partial class SpritePane
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SpritePane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "SpritePane";
            this.Size = new System.Drawing.Size(187, 102);
            this.MouseEnter += new System.EventHandler(this.SpritePane_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.SpritePane_MouseLeave);
            this.MouseHover += new System.EventHandler(this.SpritePane_MouseHover);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
