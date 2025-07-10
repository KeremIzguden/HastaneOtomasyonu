using System;
using System.Drawing;
using System.Windows.Forms;

namespace HastaneOtomasyonu
{
    public partial class FormAdminPanel : Form
    {
        public FormAdminPanel()
        {
            InitializeComponent();
            PaneliHazirla();
        }

        private void PaneliHazirla()
        {
            this.Text = "Admin Paneli";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            Font genelFont = new Font("Segoe UI", 12, FontStyle.Regular);
            Color butonRenk = Color.SteelBlue;

            Label lblBaslik = new Label
            {
                Text = "Yönetim Paneli",
                Location = new Point(200, 30),
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = true
            };
            this.Controls.Add(lblBaslik);

            int baslangicY = 100;
            int dikeyBosluk = 60;

            Button btnHasta = new Button
            {
                Text = "Hasta İşlemleri",
                Location = new Point(180, baslangicY),
                Size = new Size(220, 40),
                BackColor = butonRenk,
                ForeColor = Color.White,
                Font = genelFont,
                FlatStyle = FlatStyle.Flat
            };
            btnHasta.Click += (s, e) => { FormHasta form = new FormHasta(); form.Show(); };
            this.Controls.Add(btnHasta);

            Button btnDoktor = new Button
            {
                Text = "Doktor İşlemleri",
                Location = new Point(180, baslangicY + dikeyBosluk),
                Size = new Size(220, 40),
                BackColor = butonRenk,
                ForeColor = Color.White,
                Font = genelFont,
                FlatStyle = FlatStyle.Flat
            };
            btnDoktor.Click += (s, e) => { FormDoktor form = new FormDoktor(); form.Show(); };
            this.Controls.Add(btnDoktor);

            Button btnCalisan = new Button
            {
                Text = "Çalışan İşlemleri",
                Location = new Point(180, baslangicY + dikeyBosluk * 2),
                Size = new Size(220, 40),
                BackColor = butonRenk,
                ForeColor = Color.White,
                Font = genelFont,
                FlatStyle = FlatStyle.Flat
            };
            btnCalisan.Click += (s, e) =>
            {
                FormCalisan form = new FormCalisan();
                form.Show();
            };
            this.Controls.Add(btnCalisan);


            Button btnRandevu = new Button
            {
                Text = "Randevu İşlemleri",
                Location = new Point(180, baslangicY + dikeyBosluk * 3),
                Size = new Size(220, 40),
                BackColor = butonRenk,
                ForeColor = Color.White,
                Font = genelFont,
                FlatStyle = FlatStyle.Flat
            };
            btnRandevu.Click += (s, e) => { FormRandevu form = new FormRandevu(); form.Show(); };
            this.Controls.Add(btnRandevu);
            Button btnStok = new Button
            {
                Text = "Stok Yönetimi",
                Location = new Point(180, baslangicY + dikeyBosluk * 4),
                Size = new Size(220, 40),
                BackColor = Color.Teal,
                ForeColor = Color.White,
                Font = genelFont,
                FlatStyle = FlatStyle.Flat
            };
            btnStok.Click += (s, e) => { FormStok form = new FormStok(); form.Show(); };
            this.Controls.Add(btnStok);


            Button btnCikis = new Button
            {
                Text = "Çıkış Yap",
                Location = new Point(180, baslangicY + dikeyBosluk * 5),
                Size = new Size(220, 40),
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                Font = genelFont,
                FlatStyle = FlatStyle.Flat
            };
            btnCikis.Click += (s, e) => { Application.Exit(); };
            this.Controls.Add(btnCikis);

        }
    }
}
