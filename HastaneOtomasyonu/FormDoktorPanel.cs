using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace HastaneOtomasyonu
{
    public partial class FormDoktorPanel : Form
    {
        private int _doktorID;
        SqlConnection baglanti = new SqlConnection(ConfigurationManager.ConnectionStrings["HastaneDB"].ConnectionString);

        DataGridView dgvRandevular;

        public FormDoktorPanel(int doktorID)
        {
            _doktorID = doktorID;
            InitializeComponent();
            FormuHazirla();
            RandevulariYukle();
        }

        private void FormuHazirla()
        {
            this.Text = "Doktor Paneli - Randevularım";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            Font genelFont = new Font("Segoe UI", 10);
            Font baslikFont = new Font("Segoe UI", 16, FontStyle.Bold);

            Label lblBaslik = new Label
            {
                Text = "RANDEVULARIM",
                Font = baslikFont,
                ForeColor = Color.MidnightBlue,
                Location = new Point((this.Width / 2) - 120, 20),
                AutoSize = true
            };
            this.Controls.Add(lblBaslik);

            MonthCalendar takvim = new MonthCalendar
            {
                Location = new Point(750, 80),
                MaxSelectionCount = 1
            };
            this.Controls.Add(takvim);

            takvim.DateSelected += (s, e) =>
            {
                string tarih = takvim.SelectionStart.ToString("yyyy-MM-dd");
                SqlDataAdapter da = new SqlDataAdapter(
                    @"SELECT h.Ad, h.Soyad, d.Branş, r.Tarih, r.Saat, r.Aciklama
          FROM Randevular r
          JOIN Hastalar h ON r.HastaID = h.HastaID
          JOIN Doktorlar d ON r.DoktorID = d.DoktorID
          WHERE CONVERT(date, r.Tarih) = @Tarih AND r.DoktorID = @DoktorID", baglanti);

                da.SelectCommand.Parameters.AddWithValue("@Tarih", tarih);
                da.SelectCommand.Parameters.AddWithValue("@DoktorID", _doktorID);

                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    try
                    {
                        row["Ad"] = SifrelemeHelper.SifreCoz(row["Ad"].ToString());
                        row["Soyad"] = SifrelemeHelper.SifreCoz(row["Soyad"].ToString());
                    }
                    catch
                    {
                        row["Ad"] = "";
                        row["Soyad"] = "";
                    }

                    row["Ad"] = $"{row["Ad"]} {row["Soyad"]}";

                    // Saat formatı
                    try
                    {
                        row["Saat"] = TimeSpan.Parse(row["Saat"].ToString()).ToString(@"hh\:mm");

                    }
                    catch
                    {
                        row["Saat"] = "";
                    }
                }

                if (dt.Columns.Contains("Soyad")) dt.Columns.Remove("Soyad");
                dt.Columns["Ad"].ColumnName = "Hasta";
                dgvRandevular.DataSource = dt;
            };


            dgvRandevular = new DataGridView
            {
                Location = new Point(50, 80),
                Size = new Size(680, 450),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = genelFont,
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.Gainsboro }
            };
            this.Controls.Add(dgvRandevular);

            Button btnParolaDegistir = new Button
            {
                Text = "Parolamı Değiştir",
                Location = new Point(750, 400),
                Size = new Size(200, 40),
                BackColor = Color.DarkOrange,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat
            };
            btnParolaDegistir.Click += BtnParolaDegistir_Click;
            this.Controls.Add(btnParolaDegistir);

            dgvRandevular.CellFormatting += (s, e) =>
            {
                if (dgvRandevular.Columns[e.ColumnIndex].Name == "Saat" && e.Value is TimeSpan time)
                {
                    e.Value = time.ToString(@"hh\:mm");
                    e.FormattingApplied = true;
                }
            };


        }

        private void RandevulariYukle()
        {
            try
            {
                if (baglanti.State == ConnectionState.Closed)
                    baglanti.Open();

                SqlDataAdapter da = new SqlDataAdapter(@"
            SELECT r.RandevuID, h.Ad, h.Soyad, r.Tarih, r.Saat, r.Aciklama
            FROM Randevular r
            INNER JOIN Hastalar h ON r.HastaID = h.HastaID
            WHERE r.DoktorID = @DoktorID", baglanti);

                da.SelectCommand.Parameters.AddWithValue("@DoktorID", _doktorID);

                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    try
                    {
                        row["Ad"] = SifrelemeHelper.SifreCoz(row["Ad"].ToString());
                        row["Soyad"] = SifrelemeHelper.SifreCoz(row["Soyad"].ToString());
                    }
                    catch
                    {
                        row["Ad"] = "";
                        row["Soyad"] = "";
                    }

                    row["Ad"] = $"{row["Ad"]} {row["Soyad"]}";

                    // Saat formatını HH:mm yap
                    try
                    {
                        row["Saat"] = TimeSpan.Parse(row["Saat"].ToString()).ToString(@"hh\:mm");

                    }
                    catch
                    {
                        row["Saat"] = "";
                    }
                }

                if (dt.Columns.Contains("Soyad")) dt.Columns.Remove("Soyad");
                dt.Columns["Ad"].ColumnName = "Hasta";
                dgvRandevular.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                    baglanti.Close();
            }
        }


        private void BtnParolaDegistir_Click(object sender, EventArgs e)
        {
            Form sifreForm = new Form
            {
                Text = "Parola Değiştir",
                Size = new Size(400, 300),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.WhiteSmoke
            };

            Label lblEski = new Label { Text = "Eski Şifre:", Location = new Point(30, 30), AutoSize = true };
            TextBox txtEski = new TextBox { Location = new Point(150, 25), Width = 200, PasswordChar = '●' };

            Label lblYeni = new Label { Text = "Yeni Şifre:", Location = new Point(30, 80), AutoSize = true };
            TextBox txtYeni = new TextBox { Location = new Point(150, 75), Width = 200, PasswordChar = '●' };

            Label lblTekrar = new Label { Text = "Yeni Şifre (Tekrar):", Location = new Point(30, 130), AutoSize = true };
            TextBox txtTekrar = new TextBox { Location = new Point(150, 125), Width = 200, PasswordChar = '●' };

            Button btnOnayla = new Button { Text = "Parolayı Güncelle", Location = new Point(150, 180), Width = 200, BackColor = Color.SteelBlue, ForeColor = Color.White };

            btnOnayla.Click += (s2, e2) =>
            {
                if (txtYeni.Text != txtTekrar.Text)
                {
                    MessageBox.Show("Yeni şifreler uyuşmuyor!");
                    return;
                }

                SqlConnection baglanti = new SqlConnection(ConfigurationManager.ConnectionStrings["HastaneDB"].ConnectionString);
                try
                {
                    baglanti.Open();
                    SqlCommand kontrolCmd = new SqlCommand("SELECT COUNT(*) FROM Kullanicilar WHERE DoktorID = @id AND Sifre = @sifre", baglanti);
                    kontrolCmd.Parameters.AddWithValue("@id", _doktorID);
                    kontrolCmd.Parameters.AddWithValue("@sifre", SifrelemeHelper.Sifrele(txtEski.Text));
                    int sayi = (int)kontrolCmd.ExecuteScalar();

                    if (sayi == 0)
                    {
                        MessageBox.Show("Eski şifreniz hatalı.");
                        return;
                    }

                    SqlCommand guncelleCmd = new SqlCommand("UPDATE Kullanicilar SET Sifre = @sifre WHERE DoktorID = @id", baglanti);
                    guncelleCmd.Parameters.AddWithValue("@sifre", SifrelemeHelper.Sifrele(txtYeni.Text));
                    guncelleCmd.Parameters.AddWithValue("@id", _doktorID);
                    guncelleCmd.ExecuteNonQuery();

                    MessageBox.Show("Parola başarıyla güncellendi.");
                    sifreForm.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
                finally
                {
                    baglanti.Close();
                }
            };

            sifreForm.Controls.AddRange(new Control[] { lblEski, txtEski, lblYeni, txtYeni, lblTekrar, txtTekrar, btnOnayla });
            sifreForm.ShowDialog();
        }

    }
}
