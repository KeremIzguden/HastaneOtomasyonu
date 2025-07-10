using System;
using System.Drawing;
using System.Windows.Forms;

namespace HastaneOtomasyonu
{
    public partial class FormAna : Form
    {
        Button btnHastaIslemleri, btnDoktorIslemleri, btnRandevuIslemleri;

        public FormAna()
        {
            InitializeComponent();
            FormuHazirla();
        }

        private void FormuHazirla()
        {
            // Form Özellikleri
            this.Text = "🏥 Hastane Otomasyon Sistemi - Ana Sayfa";
            this.Size = new Size(600, 500);
            this.MinimumSize = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            Font genelFont = new Font("Segoe UI", 12, FontStyle.Regular);
            Color butonArkaPlan = Color.SteelBlue;
            Color butonYaziRenk = Color.White;

            // Hasta İşlemleri Butonu
            btnHastaIslemleri = new Button
            {
                Text = "Hasta İşlemleri",
                Size = new Size(250, 50),
                BackColor = butonArkaPlan,
                ForeColor = butonYaziRenk,
                Font = genelFont,
                FlatStyle = FlatStyle.Flat
            };
            btnHastaIslemleri.FlatAppearance.BorderSize = 0;
            btnHastaIslemleri.Click += BtnHastaIslemleri_Click;
            this.Controls.Add(btnHastaIslemleri);

            // Doktor İşlemleri Butonu
            btnDoktorIslemleri = new Button
            {
                Text = "Doktor İşlemleri",
                Size = new Size(250, 50),
                BackColor = butonArkaPlan,
                ForeColor = butonYaziRenk,
                Font = genelFont,
                FlatStyle = FlatStyle.Flat
            };
            btnDoktorIslemleri.FlatAppearance.BorderSize = 0;
            btnDoktorIslemleri.Click += BtnDoktorIslemleri_Click;
            this.Controls.Add(btnDoktorIslemleri);

            // Randevu İşlemleri Butonu
            btnRandevuIslemleri = new Button
            {
                Text = "Randevu İşlemleri",
                Size = new Size(250, 50),
                BackColor = butonArkaPlan,
                ForeColor = butonYaziRenk,
                Font = genelFont,
                FlatStyle = FlatStyle.Flat
            };
            btnRandevuIslemleri.FlatAppearance.BorderSize = 0;
            btnRandevuIslemleri.Click += BtnRandevuIslemleri_Click;
            this.Controls.Add(btnRandevuIslemleri);

            // Sayfa ilk açılırken ortala
            this.Load += FormAna_Load;
            this.Resize += FormAna_Resize;
        }

        private void FormAna_Load(object sender, EventArgs e)
        {
            ButonlariOrtala();
        }

        private void FormAna_Resize(object sender, EventArgs e)
        {
            ButonlariOrtala();
        }

        private void ButonlariOrtala()
        {
            int merkezX = (this.ClientSize.Width - btnHastaIslemleri.Width) / 2;
            int merkezY = (this.ClientSize.Height - (3 * btnHastaIslemleri.Height + 40)) / 2; // 3 buton + aralarındaki boşluklar

            btnHastaIslemleri.Location = new Point(merkezX, merkezY);
            btnDoktorIslemleri.Location = new Point(merkezX, merkezY + btnHastaIslemleri.Height + 20);
            btnRandevuIslemleri.Location = new Point(merkezX, merkezY + 2 * (btnHastaIslemleri.Height + 20));
        }

        private void BtnHastaIslemleri_Click(object sender, EventArgs e)
        {
            FormHasta formHasta = new FormHasta();
            formHasta.Show();
            this.Hide();
            formHasta.FormClosed += (s, args) => this.Show();
        }

        private void BtnDoktorIslemleri_Click(object sender, EventArgs e)
        {
            FormDoktor formDoktor = new FormDoktor();
            formDoktor.Show();
            this.Hide();
            formDoktor.FormClosed += (s, args) => this.Show();
        }

        private void BtnRandevuIslemleri_Click(object sender, EventArgs e)
        {
            FormRandevu formRandevu = new FormRandevu();
            formRandevu.Show();
            this.Hide();
            formRandevu.FormClosed += (s, args) => this.Show();
        }
    }
}
