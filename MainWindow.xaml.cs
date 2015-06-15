using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace THCardDealer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int currentStage = 0; //0=pre flop, 1=flop, 2=turn, 3=river
        private List<string> fullDeck = new List<string>() { "1H", "1C", "1S", "1D", "2H", "2C", "2S", "2D", "3H", "3C", "3S", "3D", "4H", "4C", "4S", "4D", "5H", "5C", "5S", "5D", "6H", "6C", "6S", "6D", "7H", "7C", "7S", "7D", "8H", "8C", "8S", "8D", "9H", "9C", "9S", "9D", "10H", "10C", "10S", "10D", "11H", "11C", "11S", "11D", "12H", "12C", "12S", "12D", "13H", "13C", "13S", "13D", };
        private List<string> currentDeck = null;
        private List<string> CommunityCards = null;
        Random rand = new Random();


        public MainWindow()
        {
            InitializeComponent();
         
        }


        private void Window_ContentRendered(object sender, EventArgs e)
        {
            ReadFromFile();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveToFile();
        }

        private void ReadFromFile()
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            //string webData = wc.DownloadString("http://www.yoursite.com/resource/file.htm");

            if (File.Exists("params.txt"))
            {
                string txtParams = File.ReadAllText("params.txt");
                List<string> str = txtParams.Split('~').ToList<string>();

                currentDeck = new List<string>();
                foreach (var item in str[0].Split(','))
                    if (item != "")
                        currentDeck.Add(item);

                CommunityCards = new List<string>();
                foreach (var item in str[1].Split(','))
                {
                    if (item != "")
                        CommunityCards.Add(item);
                }

                currentStage = Convert.ToInt32(str[2]);
                handNumber.Text = str[3];
                lblCardsLeft.Content = "Cards In Deck : " + (currentDeck == null || currentDeck.Count == 0 ? "52" : currentDeck.Count.ToString());

                try
                {
                    SmallBlind.Text = str[4];
                    BigBlind.Text = str[5];
                    Email.Text = str[6];
                    Password.Password = str[7];
                    Email1.Text = str[8];
                    Email2.Text = str[9];
                    Email3.Text = str[10];
                    Email4.Text = str[11];
                    Email5.Text = str[12];
                    Email6.Text = str[13];
                    Email7.Text = str[14];
                    Email8.Text = str[15];
                    Email9.Text = str[16];
                }
                catch (Exception)
                {


                }

                btnDeal.Content = SetButtonText();
            }
        }

        private void SaveToFile()
        {
            StringBuilder sb = new StringBuilder();
            if (currentDeck != null && currentDeck.Count > 0)
            {
                foreach (var item in currentDeck)
                    sb.Append(item + ",");

                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append('~');

            if (CommunityCards != null)
            {
                foreach (var item in CommunityCards)
                    sb.Append(item + ",");
                if (CommunityCards.Count > 0)
                    sb.Remove(sb.Length - 1, 1);
            }
            sb.Append('~');

            sb.Append(currentStage);
            sb.Append('~');
            sb.Append(handNumber.Text);
            sb.Append('~');
            sb.Append(SmallBlind.Text);
            sb.Append('~');
            sb.Append(BigBlind.Text);
            sb.Append('~');
            sb.Append(Email.Text);
            sb.Append('~');
            sb.Append(Password.Password);
            sb.Append('~');
            sb.Append(Email1.Text);
            sb.Append('~');
            sb.Append(Email2.Text);
            sb.Append('~');
            sb.Append(Email3.Text);
            sb.Append('~');
            sb.Append(Email4.Text);
            sb.Append('~');
            sb.Append(Email5.Text);
            sb.Append('~');
            sb.Append(Email6.Text);
            sb.Append('~');
            sb.Append(Email7.Text);
            sb.Append('~');
            sb.Append(Email8.Text);
            sb.Append('~');
            sb.Append(Email9.Text);
            sb.Append('~');
            File.WriteAllText("params.txt", sb.ToString());
        }

        private void btnDeal_Click(object sender, RoutedEventArgs e)
        {
            if (handNumber.Text == "")
            {
                MessageBox.Show("Please enter hand number");
                return;
            }
            if (Email.Text == "" || Email.Text == "Email address")
            {
                MessageBox.Show("Please enter Email");
                return;
            }
            if (Password.Password == "")
            {
                MessageBox.Show("Please enter Password");
                return;
            }
            if (SmallBlind.Text == "")
            {
                MessageBox.Show("Please enter Small blind");
                return;
            }
            if (BigBlind.Text == "")
            {
                MessageBox.Show("Please enter Big blind");
                return;
            }


            lblSending.Visibility = System.Windows.Visibility.Visible;
            System.Windows.Forms.Application.DoEvents();
            switch (currentStage)
            {
                case 0:
                    //Testing :
                    //for (int i = 0; i < 26; i++)
                    //{
                    if (DealCards())
                        currentStage++;

                    //}
                    break;

                case 1:
                    if (Flop())
                        currentStage++;
                    break;

                case 2:
                    if (Turn())
                        currentStage++;
                    break;

                case 3:
                    if (River())
                    {
                        Reset();
                        handNumber.Text = ((Convert.ToInt32(handNumber.Text)) + 1).ToString();
                    }
                    break;

            }
            btnDeal.Content = SetButtonText();
            lblSending.Visibility = System.Windows.Visibility.Hidden;
            lblCardsLeft.Content = "Cards In Deck : " + (currentDeck == null || currentDeck.Count == 0 ? "52" : currentDeck.Count.ToString());
            SaveToFile();
        }

        private string SetButtonText()
        {
            switch (currentStage)
            {
                case 1:
                    return "Flop!";

                case 2:
                    return "Turn!";

                case 3:
                    return "River!";

                case 0:
                    return "Deal!";

            }
            return "";
        }

        private bool DealCards()
        {
            currentDeck = new List<string>();
            foreach (string card in fullDeck)
                currentDeck.Add(card);

            CommunityCards = new List<string>();
            string err = "";

            try { DealPlayer(Email1.Text); }
            catch { err += Email1.Text + ", "; }
            try { DealPlayer(Email2.Text); }
            catch { err += Email2.Text + ", "; }
            try { DealPlayer(Email3.Text); }
            catch { err += Email3.Text + ", "; }
            try { DealPlayer(Email4.Text); }
            catch { err += Email4.Text + ", "; }
            try { DealPlayer(Email5.Text); }
            catch { err += Email5.Text + ", "; }
            try { DealPlayer(Email6.Text); }
            catch { err += Email6.Text + ", "; }
            try { DealPlayer(Email7.Text); }
            catch { err += Email7.Text + ", "; }
            try { DealPlayer(Email8.Text); }
            catch { err += Email8.Text + ", "; }
            try { DealPlayer(Email9.Text); }
            catch { err += Email9.Text + ", "; }


            if (err != "")
            {
                err = "Emails weren't sent to - " + err;
                MessageBox.Show(err);
            }

            return true;
        }

        private void DealPlayer(string to)
        {
            if (to == "") return;

            string subject = "Invitational Tournament Hand #" + handNumber.Text;
            int rnd = rand.Next(currentDeck.Count);
            string card1 = currentDeck[rnd];
            currentDeck.RemoveAt(rnd);
            rnd = rand.Next(currentDeck.Count);
            string card2 = currentDeck[rnd];
            currentDeck.RemoveAt(rnd);

            SendEmail(to, subject, card1, card2);
        }

        private bool Flop()
        {
            int rnd1 = rand.Next(currentDeck.Count);
            CommunityCards.Add(currentDeck[rnd1]);
            currentDeck.RemoveAt(rnd1);
            int rnd2 = rand.Next(currentDeck.Count);
            CommunityCards.Add(currentDeck[rnd2]);
            currentDeck.RemoveAt(rnd2);
            int rnd3 = rand.Next(currentDeck.Count);
            CommunityCards.Add(currentDeck[rnd3]);
            currentDeck.RemoveAt(rnd3);

            string subject = "Flop for hand #" + handNumber.Text;

            string err = "";

            try { SendCommunityCard(Email1.Text, subject); }
            catch { err += Email1.Text + ", "; }
            try { SendCommunityCard(Email2.Text, subject); }
            catch { err += Email2.Text + ", "; }
            try { SendCommunityCard(Email3.Text, subject); }
            catch { err += Email3.Text + ", "; }
            try { SendCommunityCard(Email4.Text, subject); }
            catch { err += Email4.Text + ", "; }
            try { SendCommunityCard(Email5.Text, subject); }
            catch { err += Email5.Text + ", "; }
            try { SendCommunityCard(Email6.Text, subject); }
            catch { err += Email6.Text + ", "; }
            try { SendCommunityCard(Email7.Text, subject); }
            catch { err += Email7.Text + ", "; }
            try { SendCommunityCard(Email8.Text, subject); }
            catch { err += Email8.Text + ", "; }
            try { SendCommunityCard(Email9.Text, subject); }
            catch { err += Email9.Text + ", "; }


            if (err != "")
            {
                err = "Emails weren't sent to - " + err;
                MessageBox.Show(err);
            }
            return true;
        }

        private bool Turn()
        {
            int rnd = rand.Next(currentDeck.Count);
            CommunityCards.Add(currentDeck[rnd]);
            currentDeck.RemoveAt(rnd);

            string subject = "Turn for hand #" + handNumber.Text;
            string err = "";

            try { SendCommunityCard(Email1.Text, subject); }
            catch { err += Email1.Text + ", "; }
            try { SendCommunityCard(Email2.Text, subject); }
            catch { err += Email2.Text + ", "; }
            try { SendCommunityCard(Email3.Text, subject); }
            catch { err += Email3.Text + ", "; }
            try { SendCommunityCard(Email4.Text, subject); }
            catch { err += Email4.Text + ", "; }
            try { SendCommunityCard(Email5.Text, subject); }
            catch { err += Email5.Text + ", "; }
            try { SendCommunityCard(Email6.Text, subject); }
            catch { err += Email6.Text + ", "; }
            try { SendCommunityCard(Email7.Text, subject); }
            catch { err += Email7.Text + ", "; }
            try { SendCommunityCard(Email8.Text, subject); }
            catch { err += Email8.Text + ", "; }
            try { SendCommunityCard(Email9.Text, subject); }
            catch { err += Email9.Text + ", "; }


            if (err != "")
            {
                err = "Emails weren't sent to - " + err;
                MessageBox.Show(err);
            }
            return true;
        }

        private bool River()
        {

            int rnd = rand.Next(currentDeck.Count);
            CommunityCards.Add(currentDeck[rnd]);
            currentDeck.RemoveAt(rnd);

            string subject = "River for hand #" + handNumber.Text;
            string err = "";

            try { SendCommunityCard(Email1.Text, subject); }
            catch { err += Email1.Text + ", "; }
            try { SendCommunityCard(Email2.Text, subject); }
            catch { err += Email2.Text + ", "; }
            try { SendCommunityCard(Email3.Text, subject); }
            catch { err += Email3.Text + ", "; }
            try { SendCommunityCard(Email4.Text, subject); }
            catch { err += Email4.Text + ", "; }
            try { SendCommunityCard(Email5.Text, subject); }
            catch { err += Email5.Text + ", "; }
            try { SendCommunityCard(Email6.Text, subject); }
            catch { err += Email6.Text + ", "; }
            try { SendCommunityCard(Email7.Text, subject); }
            catch { err += Email7.Text + ", "; }
            try { SendCommunityCard(Email8.Text, subject); }
            catch { err += Email8.Text + ", "; }
            try { SendCommunityCard(Email9.Text, subject); }
            catch { err += Email9.Text + ", "; }


            if (err != "")
            {
                err = "Emails weren't sent to - " + err;
                MessageBox.Show(err);
            }
            return true;
        }

        private void SendCommunityCard(string to, string subject)
        {
            if (to == "") return;

            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.Credentials = new System.Net.NetworkCredential(Email.Text, Password.Password);
                client.EnableSsl = true;
                MailMessage newMail = new MailMessage();
                newMail.To.Add(new MailAddress(to));
                newMail.Subject = subject;
                newMail.IsBodyHtml = true;
                newMail.From = new MailAddress(Email.Text);

                var inlineImg1 = new LinkedResource("Cards\\" + CommunityCards[0] + ".jpg");
                inlineImg1.ContentId = Guid.NewGuid().ToString();
                var inlineImg2 = new LinkedResource("Cards\\" + CommunityCards[1] + ".jpg");
                inlineImg2.ContentId = Guid.NewGuid().ToString();
                var inlineImg3 = new LinkedResource("Cards\\" + CommunityCards[2] + ".jpg");
                inlineImg3.ContentId = Guid.NewGuid().ToString();

                string body = string.Format(@"
                        <img height=""100"" src=""cid:{0}"" />
                        <img height=""100""  src=""cid:{1}"" />
                        <img height=""100""  src=""cid:{2}"" />
                            ", inlineImg1.ContentId, inlineImg2.ContentId, inlineImg3.ContentId);

                LinkedResource inlineImg4 = null;
                if (currentStage == 2 || currentStage == 3)
                {
                    inlineImg4 = new LinkedResource("Cards\\" + CommunityCards[3] + ".jpg");
                    inlineImg4.ContentId = Guid.NewGuid().ToString();

                    body += string.Format(@"
                        <img height=""100"" src=""cid:{0}"" />
                            ", inlineImg4.ContentId);
                }

                LinkedResource inlineImg5 = null;
                if (currentStage == 3)
                {
                    inlineImg5 = new LinkedResource("Cards\\" + CommunityCards[4] + ".jpg");
                    inlineImg5.ContentId = Guid.NewGuid().ToString();

                    body += string.Format(@"
                        <img height=""100"" src=""cid:{0}"" />
                            ", inlineImg5.ContentId);
                }

                AlternateView view = AlternateView.CreateAlternateViewFromString(body, null, "text/html");

                view.LinkedResources.Add(inlineImg1);
                view.LinkedResources.Add(inlineImg2);
                view.LinkedResources.Add(inlineImg3);

                if (currentStage == 2 || currentStage == 3)
                    view.LinkedResources.Add(inlineImg4);
                if (currentStage == 3)
                    view.LinkedResources.Add(inlineImg5);

                newMail.AlternateViews.Add(view);
                client.Send(newMail);
            }
        }

        private void SendEmail(string to, string subject, string card1, string card2)
        {
            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.Credentials = new System.Net.NetworkCredential(Email.Text, Password.Password);
                client.EnableSsl = true;
                MailMessage newMail = new MailMessage();
                newMail.To.Add(new MailAddress(to));
                newMail.Subject = subject;
                newMail.IsBodyHtml = true;
                newMail.From = new MailAddress(Email.Text);
                var inlineImg1 = new LinkedResource("Cards\\" + card1 + ".jpg");
                inlineImg1.ContentId = Guid.NewGuid().ToString();
                var inlineImg2 = new LinkedResource("Cards\\" + card2 + ".jpg");
                inlineImg2.ContentId = Guid.NewGuid().ToString();

                string body = string.Format(@"
            <p>Blinds: " + SmallBlind.Text + " / " + BigBlind.Text + @"</p>
            <p>Your cards for hand #" + handNumber.Text + @":</p>
            <img height=""100"" src=""cid:{0}"" />
            <img height=""100""  src=""cid:{1}"" />
                ", inlineImg1.ContentId, inlineImg2.ContentId);

                var view = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                view.LinkedResources.Add(inlineImg1);
                view.LinkedResources.Add(inlineImg2);
                newMail.AlternateViews.Add(view);

                client.Send(newMail);
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            Reset();

        }

        private void Reset()
        {
            currentDeck = null;
            currentStage = 0;
            CommunityCards = null;
            lblCardsLeft.Content = "Cards In Deck : " + (currentDeck == null || currentDeck.Count == 0 ? "52" : currentDeck.Count.ToString());
            btnDeal.Content = SetButtonText();
            SaveToFile();
        }

        private void Email_GotFocus(object sender, RoutedEventArgs e)
        {

            if (Email.Text == "Email address")
                Email.Text = "";
        }
    }
}
