using static Front_end.Themecolor;
using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;

namespace Front_end
{
    public partial class hm : Form
    {
        //Fields
        private Button currentButton;
        private Random random;
        private int tempIndex;
        private Form activeForm;

        //Constructor
        public hm()
        {
            InitializeComponent();
            random = new Random();

        }

        //Methods
        //private Color SelectThemeColor()
       // {
       //     int index = random.Next(ThemeColor.ColorList.Count);
       //     while (tempIndex == index)
        //    {
        //        index = random.Next(ThemeColor.ColorList.Count);
         //   }
         //   tempIndex = index;
        //    string color = ThemeColor.ColorList[index];
        //    return ColorTranslator.FromHtml(color);
      //  }
        private void ActivateButton(object btnSender)
        {
            if (btnSender != null)
            {
                if (currentButton != (Button)btnSender)
                {
                    DisableButton();
                    //Color color = SelectThemeColor();
                    currentButton = (Button)btnSender;
                    //currentButton.BackColor = color;
                   // currentButton.ForeColor = Color.White;
                    currentButton.Font = new Font("Segoe UI", 12.5F); // Optional visual cue
                  //  lblTitle.BackColor = color;

                }
            }
        }

        private void DisableButton()
        {
            foreach (Control previousBtn in panelMenu.Controls)
            {
                if (previousBtn.GetType() == typeof(Button))
                {
                    previousBtn.BackColor = Color.FromArgb(24, 30, 54);
                    previousBtn.ForeColor = Color.Gainsboro;
                    previousBtn.Font = new Font("Segoe UI", 10F);
                }
            }
        }
        private void openChildForm(Form childForm, object btnSender)
        {
            if (activeForm != null)
            {
                activeForm.Close();
            }
            ActivateButton(btnSender);
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            this.panelDesktopPane.Controls.Add(childForm);
            this.panelDesktopPane.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
            label1.Text = childForm.Text;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (activeForm != null)
            {
                activeForm.Close();
            }

            // Ensure 'btnsender' and 'childForm' are properly defined
            object btnsender = button1; // Replace 'button1' with the appropriate button reference
            Form childForm = new forms.joint2(); // Replace 'forms.formjoint()' with the appropriate form instance

            ActivateButton(btnsender);
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            this.panelDesktopPane.Controls.Add(childForm);
            this.panelDesktopPane.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
            label1.Text = childForm.Text;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            openChildForm(new forms.joint2(), sender);
        }

        private void login_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            openChildForm(new forms.history(), sender);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openChildForm(new forms.setting(), sender);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void lblTitle_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void paneldesktoppane_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
