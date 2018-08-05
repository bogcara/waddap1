using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace WindowsFormsLicenceStuff
{
    public partial class Form3 : Form
    {
        String conString = "Data Source=EN614211;Initial Catalog=LicenceDB;Integrated Security=True";
        List<String> usernames = new List<String>();
        String user = null;

        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            if (con.State == System.Data.ConnectionState.Open) {
                String userQuery = "SELECT [UserName] FROM [LicenceDB].[dbo].[Users] WHERE [UserName] = '" + textBox1.Text + "'";
                SqlCommand userC = new SqlCommand(userQuery, con);
                SqlDataReader objReader = userC.ExecuteReader();
                if (objReader.HasRows)
                {
                    while (objReader.Read())
                    {
                        String user = objReader.GetString(objReader.GetOrdinal("UserName"));
                    }
                }
                /*SqlCommand usernameCmd = new SqlCommand(usernameQuerry, con);
                using (SqlDataReader objReader = usernameCmd.ExecuteReader())
                {
                    if (objReader.HasRows)
                    {
                        while (objReader.Read())
                        {
                            String user = objReader.GetString(objReader.GetOrdinal("UserName"));
                            usernames.Add(user);
                        }
                    }
                } */
            }

            if (isValidLogIn(user))
            {
                Form firstForm = new Form1();
                this.Hide();
                firstForm.ShowDialog();
            }
            else {
                MessageBox.Show("Invalid username or password", "Log in Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public Boolean isValidLogIn(String user)
        {
            String pass = null;
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            if (con.State == System.Data.ConnectionState.Open)
            {
                String passQuery = "SELECT [Password] FROM [LicenceDB].[dbo].[Users] WHERE [UserName] = '" + textBox1.Text + "'";
                SqlCommand passCmd = new SqlCommand(passQuery, con);
                SqlDataReader objReader = passCmd.ExecuteReader();
                if (objReader.HasRows)
                {
                    while (objReader.Read())
                    {
                        pass = objReader.GetString(objReader.GetOrdinal("Password")).Trim();
                    }
                }

                if (Encrypt(textBox2.Text).Equals(pass)) {
                    return true;
                }
            }
            return false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form myForm = new Form4();
            myForm.ShowDialog();
        }

        public String Encrypt(String plainText)
        {
            if (plainText == null) throw new ArgumentNullException("plainText");

            //return as base64 string
            return Convert.ToBase64String(HashPassword(plainText));
        }

        public static byte[] HashPassword(String password)
        {
            var provider = new SHA1CryptoServiceProvider();
            var encoding = new UnicodeEncoding();
            return provider.ComputeHash(encoding.GetBytes(password));
        }
    }
}
