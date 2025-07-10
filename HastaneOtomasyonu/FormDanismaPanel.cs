using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace HastaneOtomasyonu
{
    public partial class FormDanismaPanel : Form
    {
        SqlConnection baglanti = new SqlConnection(ConfigurationManager.ConnectionStrings["HastaneDB"].ConnectionString);
        DataGridView dgvRandevular;
        Label lblRandevular;

        public FormDanismaPanel()
        {
            InitializeComponent();
            FormuHazirla();
            RandevulariYukle();
        }

        private void FormuHazirla()
        {
            this.Text = "Danışma Paneli";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            Label lblBilgi = new Label
            {
                Text = "Sadece TC Kimlik No ile arama yapabilirsiniz.",
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.DarkRed,
                Location = new Point(70, 55),
                AutoSize = true
            };
            this.Controls.Add(lblBilgi);

            Label lblBaslik = new Label
            {
                Text = "DANIŞMA PANELİ",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.MidnightBlue,
                Location = new Point(370, 20),
                AutoSize = true
            };
            this.Controls.Add(lblBaslik);

            lblRandevular = new Label
            {
                Text = "Hasta Randevu Listesi",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                Location = new Point(20, 70),
                AutoSize = true
            };
            this.Controls.Add(lblRandevular);

            PictureBox pbSearch = new PictureBox
            {
                Image = Image.FromFile("search.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(24, 24),
                Location = new Point(30, 30)
            };
            this.Controls.Add(pbSearch);

            TextBox txtAra = new TextBox
            {
                Location = new Point(70, 28),
                Width = 250,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(txtAra);

            dgvRandevular = new DataGridView
            {
                Location = new Point(20, 100),
                Size = new Size(940, 420),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            this.Controls.Add(dgvRandevular);

            txtAra.TextChanged += (s, e) =>
            {
                if (dgvRandevular.DataSource is DataTable dt)
                {
                    dt.DefaultView.RowFilter = $"[TC Kimlik No] LIKE '%{txtAra.Text}%'";
                }
            };
        }

        private void RandevulariYukle()
        {
            try
            {
                baglanti.Open();
                SqlDataAdapter da = new SqlDataAdapter(@"
            SELECT h.Ad AS HastaAd, h.Soyad AS HastaSoyad, h.TCNo, h.Telefon, h.Cinsiyet,
                   d.Ad AS DoktorAd, d.Soyad AS DoktorSoyad, d.Branş AS Brans,
                   r.Tarih, r.Saat, r.Aciklama
            FROM Randevular r
            JOIN Hastalar h ON r.HastaID = h.HastaID
            JOIN Doktorlar d ON r.DoktorID = d.DoktorID
            ORDER BY r.Tarih, r.Saat", baglanti);

                DataTable dt = new DataTable();
                da.Fill(dt);

                // Şifre çöz
                foreach (DataRow row in dt.Rows)
                {
                    try
                    {
                        // Hasta bilgileri
                        row["HastaAd"] = SifrelemeHelper.SifreCoz(row["HastaAd"].ToString());
                        row["HastaSoyad"] = SifrelemeHelper.SifreCoz(row["HastaSoyad"].ToString());
                        row["TCNo"] = SifrelemeHelper.SifreCoz(row["TCNo"].ToString());
                        row["Telefon"] = SifrelemeHelper.SifreCoz(row["Telefon"].ToString());
                        row["Cinsiyet"] = SifrelemeHelper.SifreCoz(row["Cinsiyet"].ToString());

                        // Doktor bilgileri
                        row["DoktorAd"] = SifrelemeHelper.SifreCoz(row["DoktorAd"].ToString());
                        row["DoktorSoyad"] = SifrelemeHelper.SifreCoz(row["DoktorSoyad"].ToString());
                        
                    }
                    catch
                    {
                        // Hatalı şifrelenmiş veriler varsa temizle
                        row["HastaAd"] = "";
                        row["HastaSoyad"] = "";
                        row["TCNo"] = "";
                        row["Telefon"] = "";
                        row["Cinsiyet"] = "";
                        row["DoktorAd"] = "";
                        row["DoktorSoyad"] = "";
                        row["Brans"] = "";
                    }
                }

                // Yeni tablo oluştur ve kullanıcılara anlamlı başlıklar ver
                DataTable gosterilecek = new DataTable();
                gosterilecek.Columns.Add("Ad");
                gosterilecek.Columns.Add("Soyad");
                gosterilecek.Columns.Add("TC Kimlik No");
                gosterilecek.Columns.Add("Telefon");
                gosterilecek.Columns.Add("Cinsiyet");
                gosterilecek.Columns.Add("Doktor");
                gosterilecek.Columns.Add("Branşı");
                gosterilecek.Columns.Add("Tarih");
                gosterilecek.Columns.Add("Saat");
                gosterilecek.Columns.Add("Açıklama");

                foreach (DataRow row in dt.Rows)
                {
                    string doktorAdSoyad = row["DoktorAd"] + " " + row["DoktorSoyad"];
                    gosterilecek.Rows.Add(
                        row["HastaAd"],
                        row["HastaSoyad"],
                        row["TCNo"],
                        row["Telefon"],
                        row["Cinsiyet"],
                        doktorAdSoyad,
                        row["Brans"],
                        row["Tarih"],
                        row["Saat"],
                        row["Aciklama"]
                    );
                }

                dgvRandevular.DataSource = gosterilecek;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Randevu verileri yüklenirken hata oluştu: " + ex.Message);
            }
            finally
            {
                baglanti.Close();
            }
        }
    }
}
    

