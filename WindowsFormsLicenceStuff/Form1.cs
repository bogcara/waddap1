using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsLicenceStuff
{
    public partial class Form1 : Form
    {
        String conString = "Data Source=EN614211;Initial Catalog=LicenceDB;Integrated Security=True";
        Bitmap image = new Bitmap(@"C:\Users\bcaramihai\Desktop\asdasdasd.jpg");
        List<Bitmap> images = new List<Bitmap>();


        public Form1()
        {
            InitializeComponent();
            pictureBox1.Image = new Bitmap(@"D:\C#\bcaramihai\source\repos\WindowsFormsLicenceStuff\images\no.png");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            String imageLocation = "";
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = " Jpg files(*.jpg)|*.jpg| PNG files(*.png)|*.png| All Files(*.*)|*.*";

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    imageLocation = dialog.FileName;
                    Bitmap firstPic = new Bitmap(imageLocation);

                    pictureBox1.Image = new Bitmap(firstPic, new Size(128, 128));
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Error Occured", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static List<bool> GetHash(Bitmap bmpSource)
        {
            List<bool> lResult = new List<bool>();
            //create new image with 128x128 pixel
            Bitmap bmpMin = new Bitmap(bmpSource, new Size(128, 128));
            for (int j = 0; j < bmpMin.Height; j++)
            {
                for (int i = 0; i < bmpMin.Width; i++)
                {
                    //reduce colors to true / false                
                    lResult.Add(bmpMin.GetPixel(i, j).GetBrightness() < 0.5f);
                }
            }
            return lResult;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            { 
                SqlConnection con = new SqlConnection(conString);
                con.Open();
                if (con.State == System.Data.ConnectionState.Open)
                {
                    String userQuery = "SELECT [ImageName] FROM [LicenceDB].[dbo].[TestImages]";
                    SqlCommand userC = new SqlCommand(userQuery, con);

                    using (SqlDataReader objReader = userC.ExecuteReader())
                    {
                        int i = 0;
                        if (objReader.HasRows)
                        {
                            while (objReader.Read())
                            {
                                if (!objReader.IsDBNull(0))
                                {
                                    byte[] photo = (byte[])objReader[0];
                                    MemoryStream ms = new MemoryStream(photo);
                                    Bitmap bm = new Bitmap(ms);
                                    images.Add(bm);

                                    ms.Close();
                                    i++;
                                }
                            }
                        }
                    }
                }

                List<bool> firstHash = GetHash(new Bitmap(pictureBox1.Image));
                int bestHash = 0;
                
                for (int x = 0; x < images.Count(); x++) {
                    Bitmap verifyImage = images[x];
                    List<bool> testHash = GetHash(verifyImage);

                    int verifyEqualElements = firstHash.Zip(testHash, (i, j) => i == j).Count(eq => eq);
                    int verifyEqualPercentage = (verifyEqualElements * 100) / 16384;
                    if (verifyEqualPercentage >= bestHash) {
                        bestHash = verifyEqualPercentage;
                        image = images[x];
                    }
                }

                /*List<bool> secondHash = GetHash(image);

                int equalElements = firstHash.Zip(secondHash, (i, j) => i == j).Count(eq => eq);
                int equalPercentage = (equalElements * 100) / 16384;*/
                String textResult = /*equalElements.ToString() + " pixels (" +*/ bestHash.ToString() + "% similarity";

                Bitmap imgResult = new Bitmap(image, new Size(128, 128));

                Form myForm = new Form2(imgResult, textResult);
                myForm.ShowDialog();
            }
            else {
                MessageBox.Show("No selected image!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }
    }
}