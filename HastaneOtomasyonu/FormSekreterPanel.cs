using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;
using System.Linq;
using System.Drawing.Printing;
using System.Text.RegularExpressions;




namespace HastaneOtomasyonu
{


    public partial class FormSekreterPanel : Form
    {
        SqlConnection baglanti = new SqlConnection(ConfigurationManager.ConnectionStrings["HastaneDB"].ConnectionString);

        GroupBox gbTibbi;
        DataGridView dgvHastalar, dgvRandevular;
        TextBox txtAd, txtSoyad, txtTCNo, txtTelefon, txtAdres, txtSaat, txtAciklama, txtEmail, txtSikayet, txtIlaclar, txtTeshis, txtRecete, txtTibbiKayit;
        DateTimePicker dtpDogumTarihi, dtpTarih;
        ComboBox cmbCinsiyet, cmbHasta, cmbDoktor, cmbReceteHasta;
        Button btnHastaEkle, btnRandevuEkle, btnRandevuSil, btnHastaSil, btnReceteYazdir;




        DateTimePicker dtpSaat;


        public FormSekreterPanel()
        {
            InitializeComponent();
            FormuHazirla();
            HastalariYukle();
            HastaVeDoktorYukle();
            RandevulariYukle();
            ReceteHastalariniYukle();
        }

        private void FormuHazirla()
        {
         

            this.Text = "Sekreter Paneli";
            this.Size = new Size(1250, 950);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            Font font = new Font("Segoe UI", 10);

            // Arama Kutusu ve DataGridView
            var lblAra = new Label { Text = "Hasta Ara:", Location = new Point(30, 20), Font = font };
            var txtAra = new TextBox { Location = new Point(130, 18), Width = 200, Font = font };
            txtAra.TextChanged += (s, e) =>
            {
                if (dgvHastalar.DataSource is DataTable dt)
                    dt.DefaultView.RowFilter = $"Ad LIKE '%{txtAra.Text}%' OR Soyad LIKE '%{txtAra.Text}%'";
            };

            TableLayoutPanel tablePanel = new TableLayoutPanel
            {
                Location = new Point(30, 50),
                Size = new Size(1170, 220),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            // dgvHastalar ve dgvRandevular'ı panel içine ekle
            dgvHastalar = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.MintCream,
                BorderStyle = BorderStyle.Fixed3D,
                GridColor = Color.LightGray,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.LightBlue,
                    Font = new Font("Arial", 10, FontStyle.Bold)
                },
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize

            };

            dgvRandevular = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.Honeydew,
                BorderStyle = BorderStyle.Fixed3D,
                GridColor = Color.LightGray,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.LightGreen,
                    Font = new Font("Arial", 10, FontStyle.Bold)
                },
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
            };


            tablePanel.Controls.Add(dgvHastalar, 0, 0);
            tablePanel.Controls.Add(dgvRandevular, 1, 0);

            this.Controls.AddRange(new Control[] { lblAra, txtAra, tablePanel });

            

            // 1. Hasta Bilgileri Grubu
            GroupBox gbHasta = new GroupBox
            {
                Text = "Hasta Bilgileri",
                Font = font,
                Location = new Point(30, 270),
                Size = new Size(570, 330),
                BackColor = Color.MintCream
            };

            string[] labels = { "Ad", "Soyad", "TC No", "Telefon", "Doğum Tarihi", "Cinsiyet", "Adres", "E-posta" };
            Control[] controls = new Control[8];
            for (int i = 0; i < labels.Length; i++)
            {
                gbHasta.Controls.Add(new Label
                {
                    Text = labels[i] + ":",
                    Location = new Point(20, 30 + i * 35),
                    Font = font,
                    AutoSize = true
                });
            }

            txtAd = new TextBox { Location = new Point(130, 28), Width = 200 };
            txtSoyad = new TextBox { Location = new Point(130, 63), Width = 200 };
            txtTCNo = new TextBox { Location = new Point(130, 98), Width = 200, MaxLength = 11 };
            txtTelefon = new TextBox { Location = new Point(130, 133), Width = 200, MaxLength = 11 };
            dtpDogumTarihi = new DateTimePicker { Location = new Point(130, 168), Width = 200 };
            cmbCinsiyet = new ComboBox { Location = new Point(130, 203), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbCinsiyet.Items.AddRange(new[] { "Kadın", "Erkek" });
            txtAdres = new TextBox { Location = new Point(130, 238), Width = 200, Height = 40, Multiline = true };
            txtEmail = new TextBox { Location = new Point(130, 283), Width = 200 };

            controls = new Control[] { txtAd, txtSoyad, txtTCNo, txtTelefon, dtpDogumTarihi, cmbCinsiyet, txtAdres, txtEmail };
            foreach (var ctrl in controls) gbHasta.Controls.Add(ctrl);

            // Hasta Butonları
            btnHastaEkle = new Button { Text = "Hasta Ekle", Location = new Point(360, 100), Width = 160, Height = 40, BackColor = Color.SteelBlue, ForeColor = Color.White };
            btnHastaSil = new Button { Text = "Seçili Hastayı Sil", Location = new Point(360, 160), Width = 160, Height = 40, BackColor = Color.Firebrick, ForeColor = Color.White };
            gbHasta.Controls.AddRange(new Control[] { btnHastaEkle, btnHastaSil });

            // 2. Randevu Grubu
            GroupBox gbRandevu = new GroupBox
            {
                Text = "Randevu Bilgileri",
                Font = font,
                Location = new Point(630, 270),
                Size = new Size(570, 330),
                BackColor = Color.Honeydew
            };

            string[] rLbls = { "Hasta", "Doktor", "Tarih", "Saat", "Açıklama" };
            for (int i = 0; i < rLbls.Length; i++)
            {
                gbRandevu.Controls.Add(new Label
                {
                    Text = rLbls[i] + ":",
                    Location = new Point(20, 30 + i * 45),
                    Font = font,
                    AutoSize = true
                });
            }

            cmbHasta = new ComboBox { Location = new Point(130, 28), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbDoktor = new ComboBox { Location = new Point(130, 73), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            dtpTarih = new DateTimePicker { Location = new Point(130, 118), Width = 200 };
            dtpSaat = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "HH:mm",
                ShowUpDown = true,
                Location = new Point(130, 163),
                Width = 200
            };

            txtAciklama = new TextBox { Location = new Point(130, 208), Width = 400, Height = 60, Multiline = true };

            gbRandevu.Controls.AddRange(new Control[] { cmbHasta, cmbDoktor, dtpTarih, dtpSaat, txtAciklama });

            btnRandevuEkle = new Button { Text = "Randevu Ekle", Location = new Point(130, 280), Width = 160, Height = 35, BackColor = Color.ForestGreen, ForeColor = Color.White };
            btnRandevuSil = new Button { Text = "Randevu Sil", Location = new Point(310, 280), Width = 160, Height = 35, BackColor = Color.Firebrick, ForeColor = Color.White };
            gbRandevu.Controls.AddRange(new Control[] { btnRandevuEkle, btnRandevuSil });

            // 3. Tıbbi Bilgi ve Reçete Grubu
            gbTibbi = new GroupBox
            {
                Text = "Tıbbi Bilgi ve Reçete",
                Font = font,
                Location = new Point(30, 620),
                Size = new Size(1170, 260),
                BackColor = Color.AntiqueWhite
            };

            // Konumlar güncellendi: Etiketler 20px yukarıda, kutular altında
            cmbReceteHasta = new ComboBox
            {
                Location = new Point(130, 40),
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            txtSikayet = new TextBox
            {
                Location = new Point(130, 100),
                Width = 250,
                Height = 60,
                Multiline = true
            };

            txtTeshis = new TextBox
            {
                Location = new Point(420, 100),
                Width = 250,
                Height = 60,
                Multiline = true
            };

            txtRecete = new TextBox
            {
                Location = new Point(710, 100),
                Width = 250,
                Height = 60,
                Multiline = true
            };

            btnReceteYazdir = new Button
            {
                Text = "Reçete Yazdır",
                Location = new Point(980, 180),
                Width = 150,
                Height = 40,
                BackColor = Color.DarkBlue,
                ForeColor = Color.White
            };

            // Etiketlerin Y konumu kutulara göre 20 piksel yukarıya alındı
            gbTibbi.Controls.AddRange(new Control[]
            {
new Label
{
    Text = "Reçete Hastası:",
    Location = new Point(20, 20),
    Font = font,
    BackColor = Color.Transparent,
    AutoSize = true
},
cmbReceteHasta,

new Label
{
    Text = "Şikayet:",
    Location = new Point(20, 80),
    Font = font,
    BackColor = Color.Transparent,
    AutoSize = true
},
txtSikayet,

new Label
{
    Text = "Teşhis:",
    Location = new Point(400, 80),
    Font = font,
    BackColor = Color.Transparent,
    AutoSize = true
},
txtTeshis,

new Label
{
    Text = "İlaç:",
    Location = new Point(690, 80),
    Font = font,
    BackColor = Color.Transparent,
    AutoSize = true
},
txtRecete,

btnReceteYazdir
            });

            this.Controls.AddRange(new Control[] { gbHasta, gbRandevu, gbTibbi });

            // Event atamaları
            btnHastaEkle.Click += BtnHastaEkle_Click;
            btnHastaSil.Click += BtnHastaSil_Click;
            btnRandevuEkle.Click += BtnRandevuEkle_Click;
            btnRandevuSil.Click += BtnRandevuSil_Click;
            btnReceteYazdir.Click += BtnReceteYazdir_Click;
        }


        private void RemovePlaceholder(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.ForeColor == Color.Gray)
            {
                tb.Text = "";
                tb.ForeColor = Color.Black;
            }
        }

        private void AddPlaceholder(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                if (tb == txtSikayet) tb.Text = "Şikayet";
                else if (tb == txtTeshis) tb.Text = "Teşhis";
                else if (tb == txtRecete) tb.Text = "Reçete";

                tb.ForeColor = Color.Gray;
            }
        }


        private void BtnHastaEkle_Click(object sender, EventArgs e)
        {
            try
            {
                string tcNo = txtTCNo.Text.Trim();

                // SÖZDİZİMİ KONTROLÜ (11 rakam ve sadece sayı)
                if (!Regex.IsMatch(tcNo, @"^\d{11}$"))
                {
                    MessageBox.Show("TC Kimlik Numarası 11 haneli ve yalnızca rakamlardan oluşmalıdır.");
                    return;
                }

                //ALGORİTMA KONTROLÜ
                if (!TcKimlikNoValidator.GecerliMi(tcNo))
                {
                    MessageBox.Show("Geçersiz TC Kimlik Numarası! Lütfen geçerli bir numara giriniz.");
                    return;
                }

                if (baglanti.State == ConnectionState.Closed)
                    baglanti.Open();

                // Daha önce kayıtlı mı kontrolü
                SqlCommand kontrolCmd = new SqlCommand("SELECT COUNT(*) FROM Hastalar WHERE TCNo = @TC", baglanti);
                kontrolCmd.Parameters.AddWithValue("@TC", SifrelemeHelper.Sifrele(tcNo));
                int sayi = (int)kontrolCmd.ExecuteScalar();

                if (sayi > 0)
                {
                    MessageBox.Show("Bu TC numarasına sahip bir hasta zaten kayıtlı.");
                    return;
                }

                // Hasta ekleme işlemi
                SqlCommand cmd = new SqlCommand("INSERT INTO Hastalar (Ad, Soyad, TCNo, Telefon, DogumTarihi, Cinsiyet, Adres, Email) " +
                "VALUES (@Ad, @Soyad, @TC, @Tel, @Dogum, @Cinsiyet, @Adres, @Email)", baglanti);

                cmd.Parameters.AddWithValue("@Ad", SifrelemeHelper.Sifrele(txtAd.Text));
                cmd.Parameters.AddWithValue("@Soyad", SifrelemeHelper.Sifrele(txtSoyad.Text));
                cmd.Parameters.AddWithValue("@TC", SifrelemeHelper.Sifrele(tcNo));
                cmd.Parameters.AddWithValue("@Tel", SifrelemeHelper.Sifrele(txtTelefon.Text));
                cmd.Parameters.AddWithValue("@Dogum", dtpDogumTarihi.Value);
                cmd.Parameters.AddWithValue("@Cinsiyet", SifrelemeHelper.Sifrele(cmbCinsiyet.Text));
                cmd.Parameters.AddWithValue("@Adres", SifrelemeHelper.Sifrele(txtAdres.Text));
                cmd.Parameters.AddWithValue("@Email", SifrelemeHelper.Sifrele(txtEmail.Text));

                cmd.ExecuteNonQuery();

                MessageBox.Show("Hasta eklendi!");
                HastalariYukle();
                HastaVeDoktorYukle();
                ReceteHastalariniYukle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                baglanti.Close();
            }
        }


        private void HastaVeDoktorYukle()
        {

            if (baglanti.State == ConnectionState.Closed)
                baglanti.Open();

            // Hasta
            // Hasta
            SqlDataAdapter da1 = new SqlDataAdapter("SELECT HastaID, Ad, Soyad FROM Hastalar", baglanti);
            DataTable dt1 = new DataTable();
            da1.Fill(dt1);

            // Yeni sütun oluştur
            dt1.Columns.Add("AdSoyad", typeof(string));

            // Şifre çöz ve yeni kolona yaz
            foreach (DataRow row in dt1.Rows)
            {
                string ad = SifrelemeHelper.SifreCoz(row["Ad"].ToString());
                string soyad = SifrelemeHelper.SifreCoz(row["Soyad"].ToString());
                row["AdSoyad"] = ad + " " + soyad;
            }

            cmbHasta.DataSource = dt1;
            cmbHasta.DisplayMember = "AdSoyad";
            cmbHasta.ValueMember = "HastaID";


            // Doktor
            SqlDataAdapter da2 = new SqlDataAdapter("SELECT DoktorID, Ad, Soyad, Branş FROM Doktorlar", baglanti);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);

            // Yeni sütun oluştur
            dt2.Columns.Add("AdSoyad", typeof(string));

            // Şifre çöz ve yeni kolona yaz
            foreach (DataRow row in dt2.Rows)
            {
                string ad = SifrelemeHelper.SifreCoz(row["Ad"].ToString());
                string soyad = SifrelemeHelper.SifreCoz(row["Soyad"].ToString());
                string brans = row["Branş"].ToString(); 

                row["AdSoyad"] = ad + " " + soyad + " - " + brans;
            }

            cmbDoktor.DataSource = dt2;
            cmbDoktor.DisplayMember = "AdSoyad";
            cmbDoktor.ValueMember = "DoktorID";

            baglanti.Close();
        }

        //recete hasta
        private void ReceteHastalariniYukle()
        {
            if (baglanti.State == ConnectionState.Closed)
                baglanti.Open();

            SqlDataAdapter da = new SqlDataAdapter("SELECT HastaID, Ad, Soyad FROM Hastalar", baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                row["Ad"] = SifrelemeHelper.SifreCoz(row["Ad"].ToString());
                row["Soyad"] = SifrelemeHelper.SifreCoz(row["Soyad"].ToString());
            }
            dt.Columns.Add("AdSoyad", typeof(string), "Ad + ' ' + Soyad");
            
            da.Fill(dt);
            cmbReceteHasta.DataSource = dt;
            cmbReceteHasta.DisplayMember = "AdSoyad";
            cmbReceteHasta.ValueMember = "HastaID";

            if (dt.Rows.Count > 0)
                cmbReceteHasta.SelectedIndex = 0;

            baglanti.Close();
        }


        private void RandevulariYukle()
        {
            dgvRandevular.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            dgvRandevular.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            if (baglanti.State == ConnectionState.Closed)
                baglanti.Open();

            SqlDataAdapter da = new SqlDataAdapter(@"
        SELECT r.RandevuID, h.Ad AS HastaAd, h.Soyad AS HastaSoyad, 
               d.Ad AS DoktorAd, d.Soyad AS DoktorSoyad, 
               d.Branş AS Brans, r.Tarih, r.Saat, r.Aciklama
        FROM Randevular r
        JOIN Hastalar h ON r.HastaID = h.HastaID
        JOIN Doktorlar d ON r.DoktorID = d.DoktorID
        ORDER BY r.Tarih, r.Saat", baglanti);

            DataTable dt = new DataTable();
            da.Fill(dt);

            // Yeni sütunlar ekle
            dt.Columns.Add("Hasta", typeof(string));
            dt.Columns.Add("Doktor", typeof(string));

            foreach (DataRow row in dt.Rows)
            {
                string hastaAd = row["HastaAd"] != DBNull.Value ? SifrelemeHelper.SifreCoz(row["HastaAd"].ToString()) : "";
                string hastaSoyad = row["HastaSoyad"] != DBNull.Value ? SifrelemeHelper.SifreCoz(row["HastaSoyad"].ToString()) : "";
                row["Hasta"] = hastaAd + " " + hastaSoyad;

                string doktorAd = row["DoktorAd"] != DBNull.Value ? SifrelemeHelper.SifreCoz(row["DoktorAd"].ToString()) : "";
                string doktorSoyad = row["DoktorSoyad"] != DBNull.Value ? SifrelemeHelper.SifreCoz(row["DoktorSoyad"].ToString()) : "";
                row["Doktor"] = doktorAd + " " + doktorSoyad;

                

            }


            DataTable gosterilecek = new DataTable();
            gosterilecek.Columns.Add("RandevuID");
            gosterilecek.Columns.Add("Hasta");
            gosterilecek.Columns.Add("Doktor");
            gosterilecek.Columns.Add("Brans");
            gosterilecek.Columns.Add("Tarih");
            gosterilecek.Columns.Add("Saat");
            gosterilecek.Columns.Add("Aciklama");

            foreach (DataRow row in dt.Rows)
            {
                string saat = row["Saat"] != DBNull.Value
                    ? TimeSpan.Parse(row["Saat"].ToString()).ToString(@"hh\:mm")
                    : "";

                string tarih = row["Tarih"] != DBNull.Value
                    ? Convert.ToDateTime(row["Tarih"]).ToString("dd.MM.yyyy")
                    : "";

                gosterilecek.Rows.Add(
                    row["RandevuID"],
                    row["Hasta"],
                    row["Doktor"],
                    row["Brans"],
                    tarih,     // Saniyesiz tarih
                    saat,      // Saniyesiz saat
                    row["Aciklama"]
                );
            }


            dgvRandevular.DataSource = gosterilecek;

            baglanti.Close();
        }


        private void BtnRandevuEkle_Click(object sender, EventArgs e)
        {
            try
            {
                if (baglanti.State == ConnectionState.Closed)
                    baglanti.Open();

                SqlCommand cmd = new SqlCommand("INSERT INTO Randevular (HastaID, DoktorID, Tarih, Saat, Aciklama) VALUES (@Hasta, @Doktor, @Tarih, @Saat, @Aciklama)", baglanti);
                cmd.Parameters.AddWithValue("@Hasta", cmbHasta.SelectedValue);
                cmd.Parameters.AddWithValue("@Doktor", cmbDoktor.SelectedValue);
                cmd.Parameters.AddWithValue("@Tarih", dtpTarih.Value.Date);
               
                TimeSpan saat = dtpSaat.Value.TimeOfDay;
                cmd.Parameters.AddWithValue("@Saat", dtpSaat.Value.TimeOfDay);

                cmd.Parameters.AddWithValue("@Aciklama", txtAciklama.Text);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Randevu eklendi!");

                // E-POSTA GÖNDERME
                SqlCommand mailCmd = new SqlCommand("SELECT Ad, Soyad, Email FROM Hastalar WHERE HastaID = @id", baglanti);
                mailCmd.Parameters.AddWithValue("@id", cmbHasta.SelectedValue);
                SqlDataReader reader = mailCmd.ExecuteReader();

                string hastaAd = "", hastaSoyad = "", hastaMail = "";
                if (reader.Read())
                {
                    hastaAd = SifrelemeHelper.SifreCoz(reader["Ad"].ToString());
                    hastaSoyad = SifrelemeHelper.SifreCoz(reader["Soyad"].ToString());
                    hastaMail = SifrelemeHelper.SifreCoz(reader["Email"].ToString());

                }
                reader.Close();

                if (!string.IsNullOrWhiteSpace(hastaMail))
                {
                    string icerik = $"Sayın {hastaAd} {hastaSoyad},\n\n" +
                                    $"Randevunuz başarıyla oluşturulmuştur.\n\n" +
                                    $"Doktor: {cmbDoktor.Text}\n" +
                                    $"Tarih: {dtpTarih.Value.ToShortDateString()}\n" +
                                    $"Saat: {dtpSaat.Value.ToShortTimeString()}\n\n" +
                                    $"Geçmiş olsun.\n\n" +
                                    $"------------------------------\n" +
                                    $"Hastane Otomasyon Sistemi\n" +
                                    $"Bu e-posta otomatik olarak gönderilmiştir. Lütfen yanıtlamayınız.";

                    MailGonder(hastaMail, "Randevu Bilgilendirme", icerik);
                }

                RandevulariYukle();
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



        private void BtnRandevuSil_Click(object sender, EventArgs e)
        {
            if (dgvRandevular.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dgvRandevular.SelectedRows[0].Cells["RandevuID"].Value);

                try
                {
                    if (baglanti.State == ConnectionState.Closed)
                        baglanti.Open();

                    SqlCommand cmd = new SqlCommand("DELETE FROM Randevular WHERE RandevuID = @ID", baglanti);
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Randevu silindi!");
                    RandevulariYukle();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
                finally
                {
                    baglanti.Close();
                }
            }
        }
        private void HastalariYukle()
        {
            dgvHastalar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvHastalar.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            try
            {
                if (baglanti.State == ConnectionState.Closed)
                    baglanti.Open();

                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT HastaID, Ad, Soyad, TCNo, Telefon, DogumTarihi, Cinsiyet, Adres,Email, Teshis, Ilaclar FROM Hastalar", baglanti);
                DataTable dt = new DataTable();
                da.Fill(dt);

                //Şifre çözümü gereken alanlar
                string[] sifrelenecekAlanlar = { "Ad", "Soyad", "TCNo", "Telefon", "Cinsiyet", "Adres", "Email", "Teshis", "Ilaclar" };

                foreach (DataRow row in dt.Rows)
                {
                    foreach (string alan in sifrelenecekAlanlar)
                    {
                        if (dt.Columns.Contains(alan) && row[alan] != DBNull.Value)
                        {
                            try
                            {
                                row[alan] = SifrelemeHelper.SifreCoz(row[alan].ToString());
                            }
                            catch
                            {
                                row[alan] = ""; // Hatalı veri varsa boş geç
                            }
                        }
                    }
                }

                dgvHastalar.DataSource = dt;

                dgvHastalar.Columns["Teshis"].HeaderText = "Teşhis";
                dgvHastalar.Columns["Ilaclar"].HeaderText = "İlaçlar";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hasta listeleme hatası: " + ex.Message);
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                    baglanti.Close();
            }
        }


        private void BtnHastaSil_Click(object sender, EventArgs e)
        {
            if (dgvHastalar.SelectedRows.Count > 0)
            {
                int hastaID = Convert.ToInt32(dgvHastalar.SelectedRows[0].Cells["HastaID"].Value);

                DialogResult onay = MessageBox.Show("Seçili hastayı silmek istediğinizden emin misiniz?", "Hasta Sil", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (onay == DialogResult.Yes)
                {
                    try
                    {
                        if (baglanti.State == ConnectionState.Closed)
                            baglanti.Open();

                        // 1. İlişkili Tibbi Kayıtları sil
                        SqlCommand tibbiSil = new SqlCommand("DELETE FROM TibbiKayitlar WHERE HastaID = @id", baglanti);
                        tibbiSil.Parameters.AddWithValue("@id", hastaID);
                        tibbiSil.ExecuteNonQuery();

                        // 2. İlişkili Randevuları sil
                        SqlCommand randevuSil = new SqlCommand("DELETE FROM Randevular WHERE HastaID = @id", baglanti);
                        randevuSil.Parameters.AddWithValue("@id", hastaID);
                        randevuSil.ExecuteNonQuery();

                        // 3. Son olarak hastayı sil
                        SqlCommand hastaSil = new SqlCommand("DELETE FROM Hastalar WHERE HastaID = @id", baglanti);
                        hastaSil.Parameters.AddWithValue("@id", hastaID);
                        hastaSil.ExecuteNonQuery();

                        MessageBox.Show("Hasta ve tüm ilişkili kayıtlar başarıyla silindi.");
                        HastalariYukle();
                        HastaVeDoktorYukle();
                        RandevulariYukle();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Silme hatası: " + ex.Message);
                    }
                    finally
                    {
                        baglanti.Close();
                    }

                }
            }
            else
            {
                MessageBox.Show("Lütfen silmek için bir hasta seçin.");
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
        private void BtnKayitEkle_Click(object sender, EventArgs e)
        {
            try
            {
                baglanti.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO TibbiKayitlar (HastaID, SIKayet, Teshis, Tarih) OUTPUT INSERTED.KayitID VALUES (@HastaID, @Sikayet, @Teshis, @Tarih)", baglanti);
                cmd.Parameters.AddWithValue("@HastaID", cmbHasta.SelectedValue);
                cmd.Parameters.AddWithValue("@Sikayet", SifrelemeHelper.Sifrele(txtSikayet.Text));
                cmd.Parameters.AddWithValue("@Teshis", SifrelemeHelper.Sifrele(txtTeshis.Text));
                

                cmd.Parameters.AddWithValue("@Tarih", DateTime.Today);

                int kayitID = (int)cmd.ExecuteScalar();

                SqlCommand receteCmd = new SqlCommand("INSERT INTO Receteler (KayitID, Ilaclar, Tarih) VALUES (@KayitID, @Ilaclar, @Tarih)", baglanti);
                receteCmd.Parameters.AddWithValue("@KayitID", kayitID);
                receteCmd.Parameters.AddWithValue("@Ilaclar", SifrelemeHelper.Sifrele(txtIlaclar.Text));
                receteCmd.Parameters.AddWithValue("@Tarih", DateTime.Today);
                receteCmd.ExecuteNonQuery();

                MessageBox.Show("Tıbbi kayıt ve reçete başarıyla kaydedildi.");
               

                HastalariYukle();
                HastaVeDoktorYukle();
                ReceteHastalariniYukle();
               

            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                baglanti.Close();
            }
        }
        private void BtnReceteYazdir_Click(object sender, EventArgs e)
        {
            string hastaAdi = (cmbReceteHasta.SelectedItem as DataRowView)?["AdSoyad"]?.ToString() ?? "Hasta Adı Bulunamadı";
            string recete =
                $"*** REÇETE BELGESİ ***\n\n" +
                $"HASTA: {hastaAdi}\n" +
                $"Tarih: {DateTime.Today:dd.MM.yyyy}\n\n" +
                $"Şikayet:\n{txtSikayet.Text}\n\n" +
                $"Teşhis:\n{txtTeshis.Text}\n\n" +
                $"Reçete:\n{txtRecete.Text}";

            PrintPreviewDialog preview = new PrintPreviewDialog();
            PrintDocument doc = new PrintDocument();

            doc.PrintPage += (s, ev) =>
            {
                ev.Graphics.DrawString(recete, new Font("Segoe UI", 12), Brushes.Black, new RectangleF(20, 20, 750, 1000));
            };
            try
            {
                if (baglanti.State == ConnectionState.Closed)
                    baglanti.Open();

                SqlCommand guncelle = new SqlCommand("UPDATE Hastalar SET Teshis = @Teshis, Ilaclar = @Ilaclar WHERE HastaID = @HastaID", baglanti);
                guncelle.Parameters.AddWithValue("@Teshis", SifrelemeHelper.Sifrele(txtTeshis.Text));
                guncelle.Parameters.AddWithValue("@Ilaclar", SifrelemeHelper.Sifrele(txtRecete.Text));

                guncelle.Parameters.AddWithValue("@HastaID", cmbReceteHasta.SelectedValue);
                guncelle.ExecuteNonQuery();

                HastalariYukle();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Teşhis/İlaç bilgisi hasta kaydına eklenemedi: " + ex.Message);
            }
            finally
            {
                baglanti.Close();
            }

            preview.Document = doc;
            preview.ShowDialog();
        }


        private void SadeceRakamGirisi_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }
        private void SadeceHarfGirisi_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
                e.Handled = true;
        }


    }
}
