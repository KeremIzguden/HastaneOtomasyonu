using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net;

namespace HastaneOtomasyonu
{
    public partial class FormRandevu : Form
    {
        SqlConnection baglanti = new SqlConnection(ConfigurationManager.ConnectionStrings["HastaneDB"].ConnectionString);

        ComboBox cmbHasta, cmbBrans, cmbDoktor, cmbSaat;
        DateTimePicker dtpTarih;
        TextBox txtAciklama;
        Button btnRandevuEkle, btnGeri;
        DataGridView dgvRandevular;

        public FormRandevu()
        {
            InitializeComponent();
            FormuHazirla();
            HastalariGetir();
            BranslariGetir();
            RandevulariListele();
        }
        private void FormuHazirla()
        {
            this.Text = "Randevu İşlemleri";
            this.Size = new Size(1000, 600);
            this.MinimumSize = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            Font genelFont = new Font("Segoe UI", 11, FontStyle.Regular);
            Font labelFont = new Font("Segoe UI", 10, FontStyle.Bold);
            Color butonRenk = Color.SteelBlue;
            Color butonYaziRenk = Color.White;

            int labelX = 30;
            int inputX = 160;
            int startY = 30;
            int dikeyBosluk = 50;

            // Hasta
            Label lblHasta = new Label { Text = "Hasta:", Location = new Point(labelX, startY), AutoSize = true, Font = labelFont };
            this.Controls.Add(lblHasta);
            cmbHasta = new ComboBox { Location = new Point(inputX, startY - 5), Width = 250, Font = genelFont, DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(cmbHasta);

            // Branş
            Label lblBrans = new Label { Text = "Branş:", Location = new Point(labelX, startY + dikeyBosluk), AutoSize = true, Font = labelFont };
            this.Controls.Add(lblBrans);
            cmbBrans = new ComboBox { Location = new Point(inputX, startY + dikeyBosluk - 5), Width = 250, Font = genelFont, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbBrans.SelectedIndexChanged += CmbBrans_SelectedIndexChanged;
            this.Controls.Add(cmbBrans);

            // Doktor
            Label lblDoktor = new Label { Text = "Doktor:", Location = new Point(labelX, startY + dikeyBosluk * 2), AutoSize = true, Font = labelFont };
            this.Controls.Add(lblDoktor);
            cmbDoktor = new ComboBox { Location = new Point(inputX, startY + dikeyBosluk * 2 - 5), Width = 250, Font = genelFont, DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(cmbDoktor);

            // Tarih
            Label lblTarih = new Label { Text = "Tarih:", Location = new Point(labelX, startY + dikeyBosluk * 3), AutoSize = true, Font = labelFont };
            this.Controls.Add(lblTarih);
            dtpTarih = new DateTimePicker { Location = new Point(inputX, startY + dikeyBosluk * 3 - 5), Width = 250, Font = genelFont };
            this.Controls.Add(dtpTarih);

            // Saat
            Label lblSaat = new Label { Text = "Saat:", Location = new Point(labelX, startY + dikeyBosluk * 4), AutoSize = true, Font = labelFont };
            this.Controls.Add(lblSaat);
            cmbSaat = new ComboBox
            {
                Location = new Point(inputX, startY + dikeyBosluk * 4 - 5),
                Width = 250,
                Font = genelFont,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbSaat.Items.AddRange(new string[]
            {
                "09:00", "09:30", "10:00", "10:30", "11:00",
                "11:30", "13:00", "13:30", "14:00", "14:30",
                "15:00", "15:30", "16:00", "16:30"
            });
            this.Controls.Add(cmbSaat);

            // Açıklama
            Label lblAciklama = new Label { Text = "Açıklama:", Location = new Point(labelX, startY + dikeyBosluk * 5), AutoSize = true, Font = labelFont };
            this.Controls.Add(lblAciklama);
            txtAciklama = new TextBox { Location = new Point(inputX, startY + dikeyBosluk * 5 - 5), Width = 250, Height = 80, Multiline = true, Font = genelFont };
            this.Controls.Add(txtAciklama);

            // Randevu Ekle Butonu
            btnRandevuEkle = new Button
            {
                Text = "Randevu Ekle",
                Location = new Point(inputX, startY + dikeyBosluk * 7),
                Width = 250,
                Height = 45,
                BackColor = butonRenk,
                ForeColor = butonYaziRenk,
                Font = genelFont,
                FlatStyle = FlatStyle.Flat
            };
            btnRandevuEkle.FlatAppearance.BorderSize = 0;
            btnRandevuEkle.Click += BtnRandevuEkle_Click;
            this.Controls.Add(btnRandevuEkle);


            // DataGridView
            dgvRandevular = new DataGridView
            {
                Location = new Point(450, 30),
                Width = 500,
                Height = 450,
                ReadOnly = true,
                AllowUserToAddRows = false,
                Font = new Font("Segoe UI", 10),
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.Gainsboro },
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells, // veya Fill
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    BackColor = Color.LightSteelBlue
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.Black
                }
            };
            this.Controls.Add(dgvRandevular);

            // Otomatik başlık ve sıralama düzenleme için DataBindingComplete
            dgvRandevular.DataBindingComplete += (s, e) =>
            {
                if (dgvRandevular.Columns.Contains("DoktorAdSoyad"))
                    dgvRandevular.Columns["DoktorAdSoyad"].HeaderText = "Doktor";

                if (dgvRandevular.Columns.Contains("HastaAdSoyad"))
                    dgvRandevular.Columns["HastaAdSoyad"].HeaderText = "Hasta";

                if (dgvRandevular.Columns.Contains("Brans"))
                    dgvRandevular.Columns["Brans"].HeaderText = "Branş";

                if (dgvRandevular.Columns.Contains("Aciklama"))
                    dgvRandevular.Columns["Aciklama"].HeaderText = "Açıklama";

                // Sıralama devre dışı
                foreach (DataGridViewColumn col in dgvRandevular.Columns)
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;

                // Kolon sıraları (önem sırasına göre sola kaydır)
                if (dgvRandevular.Columns.Contains("HastaAdSoyad"))
                    dgvRandevular.Columns["HastaAdSoyad"].DisplayIndex = 0;
                if (dgvRandevular.Columns.Contains("DoktorAdSoyad"))
                    dgvRandevular.Columns["DoktorAdSoyad"].DisplayIndex = 1;

                // Gizlenecek kolon
                if (dgvRandevular.Columns.Contains("RandevuID"))
                    dgvRandevular.Columns["RandevuID"].Visible = false;
            };


            // Geri Butonu
            btnGeri = new Button
            {
                Text = "Geri Dön",
                Location = new Point(850, 500),
                Width = 100,
                Height = 40,
                BackColor = Color.Gray,
                ForeColor = Color.White,
                Font = genelFont,
                FlatStyle = FlatStyle.Flat
            };
            btnGeri.FlatAppearance.BorderSize = 0;
            btnGeri.Click += BtnGeri_Click;
            this.Controls.Add(btnGeri);

            Button btnRandevuSil = new Button
            {
                Text = "Seçili Randevuyu Sil",
                Location = new Point(inputX, startY + dikeyBosluk * 8),
                Size = new Size(250, 45),
                BackColor = Color.Firebrick,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRandevuSil.FlatAppearance.BorderSize = 0;
            btnRandevuSil.Click += BtnRandevuSil_Click;
            this.Controls.Add(btnRandevuSil);
            

        }

        private void HastalariGetir()
        {
            try
            {
                if (baglanti.State == ConnectionState.Closed)
                    baglanti.Open();

                SqlCommand komut = new SqlCommand("SELECT HastaID, Ad, Soyad FROM Hastalar", baglanti);
                SqlDataAdapter da = new SqlDataAdapter(komut);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Yeni kolon ekle
                dt.Columns.Add("AdSoyad", typeof(string));

                foreach (DataRow row in dt.Rows)
                {
                    string ad = row["Ad"] != DBNull.Value ? SifrelemeHelper.SifreCoz(row["Ad"].ToString()) : "";
                    string soyad = row["Soyad"] != DBNull.Value ? SifrelemeHelper.SifreCoz(row["Soyad"].ToString()) : "";
                    row["AdSoyad"] = ad + " " + soyad;
                }

                cmbHasta.DataSource = dt;
                cmbHasta.DisplayMember = "AdSoyad";
                cmbHasta.ValueMember = "HastaID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hastalar yüklenirken hata: " + ex.Message);
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                    baglanti.Close();
            }
        }


        private void BranslariGetir()
        {
            try
            {
                if (baglanti.State == ConnectionState.Closed)
                    baglanti.Open();

                SqlCommand komut = new SqlCommand("SELECT DISTINCT Branş FROM Doktorlar", baglanti);
                SqlDataAdapter da = new SqlDataAdapter(komut);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbBrans.DataSource = dt;
                cmbBrans.DisplayMember = "Branş";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Branşlar yüklenirken hata: " + ex.Message);
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                    baglanti.Close();
            }
        }

        private void CmbBrans_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBrans.SelectedIndex != -1)
            {
                try
                {
                    if (baglanti.State == ConnectionState.Closed)
                        baglanti.Open();

                    SqlCommand komut = new SqlCommand("SELECT DoktorID, Ad, Soyad, Branş FROM Doktorlar", baglanti);
                    SqlDataAdapter da = new SqlDataAdapter(komut);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    string seciliBrans = cmbBrans.Text;

                    // Yeni tablo oluştur
                    DataTable filtrelenmis = new DataTable();
                    filtrelenmis.Columns.Add("DoktorID", typeof(int));
                    filtrelenmis.Columns.Add("AdSoyad", typeof(string));

                    foreach (DataRow row in dt.Rows)
                    {
                        string brans = row["Branş"].ToString();

                        if (brans == seciliBrans)
                        {
                            string ad = SifrelemeHelper.SifreCoz(row["Ad"].ToString());
                            string soyad = SifrelemeHelper.SifreCoz(row["Soyad"].ToString());

                            DataRow yeni = filtrelenmis.NewRow();
                            yeni["DoktorID"] = Convert.ToInt32(row["DoktorID"]);
                            yeni["AdSoyad"] = ad + " " + soyad;
                            filtrelenmis.Rows.Add(yeni);
                        }
                    }

                    filtrelenmis.DefaultView.Sort = "AdSoyad ASC";
                    cmbDoktor.DataSource = filtrelenmis.DefaultView.ToTable();
                    cmbDoktor.DisplayMember = "AdSoyad";
                    cmbDoktor.ValueMember = "DoktorID";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Doktorlar yüklenirken hata: " + ex.Message);
                }
                finally
                {
                    if (baglanti.State == ConnectionState.Open)
                        baglanti.Close();
                }
            }
        }




        private void BtnRandevuEkle_Click(object sender, EventArgs e)
        {
            if (cmbHasta.SelectedIndex == -1 || cmbBrans.SelectedIndex == -1 || cmbDoktor.SelectedIndex == -1 || cmbSaat.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen tüm alanları eksiksiz doldurunuz.");
                return;
            }

            try
            {
                if (baglanti.State == ConnectionState.Closed)
                    baglanti.Open();

                string sifreliAciklama = SifrelemeHelper.Sifrele(txtAciklama.Text);

                SqlCommand komut = new SqlCommand("INSERT INTO Randevular (HastaID, DoktorID, Tarih, Saat, Aciklama) VALUES (@HastaID, @DoktorID, @Tarih, @Saat, @Aciklama)", baglanti);
                komut.Parameters.AddWithValue("@HastaID", cmbHasta.SelectedValue);
                komut.Parameters.AddWithValue("@DoktorID", cmbDoktor.SelectedValue);
                komut.Parameters.AddWithValue("@Tarih", dtpTarih.Value.Date);
                komut.Parameters.AddWithValue("@Saat", cmbSaat.Text);
                komut.Parameters.AddWithValue("@Aciklama", sifreliAciklama);
                komut.ExecuteNonQuery();

                // 📩 E-Posta için hasta bilgilerini alalım
                string hastaAd = "", hastaSoyad = "", hastaMail = "";

                SqlCommand mailCmd = new SqlCommand("SELECT Ad, Soyad, Email FROM Hastalar WHERE HastaID = @id", baglanti);
                mailCmd.Parameters.AddWithValue("@id", cmbHasta.SelectedValue);
                SqlDataReader reader = mailCmd.ExecuteReader();

                if (reader.Read())
                {
                    hastaAd = SifrelemeHelper.SifreCoz(reader["Ad"].ToString());
                    hastaSoyad = SifrelemeHelper.SifreCoz(reader["Soyad"].ToString());
                    hastaMail = SifrelemeHelper.SifreCoz(reader["Email"].ToString());
                }
                reader.Close();

                // Eğer mail adresi varsa, gönder
                if (!string.IsNullOrWhiteSpace(hastaMail))
                {
                    string icerik = $"Sayın {hastaAd} {hastaSoyad},\n\n" +
                                    $"Randevunuz başarıyla oluşturulmuştur.\n\n" +
                                    $"Doktor: {cmbDoktor.Text}\n" +
                                    $"Tarih: {dtpTarih.Value.ToShortDateString()}\n" +
                                    $"Saat: {cmbSaat.Text}\n\n" +
                                    $"Geçmiş olsun.\n\n" +
                                    $"------------------------------\n" +
                                    $"Hastane Otomasyon Sistemi\n" +
                                    $"Bu e-posta otomatik olarak gönderilmiştir. Lütfen yanıtlamayınız.";

                    MailGonder(hastaMail, "Randevu Bilgilendirme", icerik);
                }

                MessageBox.Show("Randevu başarıyla eklendi.");
                RandevulariListele();
                Temizle();
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

        private void RandevulariListele()
        {
            try
            {
                if (baglanti.State == ConnectionState.Closed)
                    baglanti.Open();

                SqlDataAdapter da = new SqlDataAdapter(@"
            SELECT 
                r.RandevuID,
                h.Ad AS HastaAd,
                h.Soyad AS HastaSoyad,
                d.Ad AS DoktorAd,
                d.Soyad AS DoktorSoyad,
                d.Branş AS Brans,
                r.Tarih,
                r.Saat,
                r.Aciklama
            FROM Randevular r
            JOIN Hastalar h ON r.HastaID = h.HastaID
            JOIN Doktorlar d ON r.DoktorID = d.DoktorID", baglanti);

                DataTable dt = new DataTable();
                da.Fill(dt);

                // Yeni çözülmüş sütunlar ekleniyor
                dt.Columns.Add("HastaAdSoyad", typeof(string));
                dt.Columns.Add("DoktorAdSoyad", typeof(string));

                foreach (DataRow row in dt.Rows)
                {
                    string hastaAd = row["HastaAd"] != DBNull.Value ? SifrelemeHelper.SifreCoz(row["HastaAd"].ToString()) : "";
                    string hastaSoyad = row["HastaSoyad"] != DBNull.Value ? SifrelemeHelper.SifreCoz(row["HastaSoyad"].ToString()) : "";
                    row["HastaAdSoyad"] = hastaAd + " " + hastaSoyad;

                    string doktorAd = row["DoktorAd"] != DBNull.Value ? SifrelemeHelper.SifreCoz(row["DoktorAd"].ToString()) : "";
                    string doktorSoyad = row["DoktorSoyad"] != DBNull.Value ? SifrelemeHelper.SifreCoz(row["DoktorSoyad"].ToString()) : "";
                    row["DoktorAdSoyad"] = doktorAd + " " + doktorSoyad;

                    if (row["Aciklama"] != DBNull.Value)
                        row["Aciklama"] = SifrelemeHelper.SifreCoz(row["Aciklama"].ToString());
                    if (row["Saat"] != DBNull.Value)
                    {
                        DateTime saat;
                        if (DateTime.TryParse(row["Saat"].ToString(), out saat))
                        {
                            row["Saat"] = saat.ToString("HH:mm"); // sadece saat ve dakika
                        }
                    }

                }
                dt.Columns.Remove("HastaAd");
                dt.Columns.Remove("HastaSoyad");
                dt.Columns.Remove("DoktorAd");
                dt.Columns.Remove("DoktorSoyad");

              

                dgvRandevular.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Listeleme hatası: " + ex.Message);
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                    baglanti.Close();
            }
        }




        private void BtnGeri_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Temizle()
        {
            cmbHasta.SelectedIndex = -1;
            cmbBrans.SelectedIndex = -1;
            cmbDoktor.DataSource = null;
            cmbSaat.SelectedIndex = -1;
            dtpTarih.Value = DateTime.Today;
            txtAciklama.Clear();
        }

        private void BtnRandevuSil_Click(object sender, EventArgs e)
        {
            if (dgvRandevular.SelectedRows.Count > 0)
            {
                int randevuID = Convert.ToInt32(dgvRandevular.SelectedRows[0].Cells["RandevuID"].Value);

                DialogResult result = MessageBox.Show("Seçili randevuyu silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        if (baglanti.State == ConnectionState.Closed)
                            baglanti.Open();

                        SqlCommand cmd = new SqlCommand("DELETE FROM Randevular WHERE RandevuID = @id", baglanti);
                        cmd.Parameters.AddWithValue("@id", randevuID);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Randevu silindi.");
                        RandevulariListele();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Silme hatası: " + ex.Message);
                    }
                    finally
                    {
                        if (baglanti.State == ConnectionState.Open)
                            baglanti.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen silmek için bir randevu seçin.");
            }
        }
        private void MailGonder(string alici, string konu, string mesaj)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("seninmailin@gmail.com", "Hastane Otomasyon");
                mail.To.Add(alici);
                mail.Subject = konu;
                mail.Body = mesaj;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("otomasyon.proje.hastane@gmail.com", "yiil syed axdd vfmh");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mail gönderilemedi: " + ex.Message);
            }
        }

    }
}
