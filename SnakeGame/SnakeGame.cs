using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class SnakeGame : Form
    {
        //static int xPosition = 120;  //359, 274
        //static int yPosition = 120;
        PictureBox[] snakeParts;
        static int snakeSize = 5;
        Point location = new Point(120,120);
        PictureBox apple = new PictureBox();
        Point appleLocation = new Point();
        string userName = "";
        string direction = "right";
        bool changingDirection = false;
        bool snakeTuchApple = true;
        bool isAppleEaten = false;
        int ScoreCount = 0;
        DataTable playerTable = new DataTable();
        DataTable ScoreTable = new DataTable();

        public SnakeGame()
        {
            InitializeComponent();
        }
        private void startButton_Click(object sender, EventArgs e)
        {
            NewGame();
        }
        public void NewGame()
        {
            landArea.Controls.Clear();
            snakeParts = null;
            snakeSize = 5;
            snakeTuchApple = true;
            ScoreCount = 0;
            pointLabel.Text = ScoreCount.ToString();
            location = new Point(120, 120);
            drowSnake();
            appleDrow();
            timer1.Start();
            Button button = new Button();
            foreach (Control c in Controls)
            {
                button = c as Button;
                if (button != null)
                {
                    button.Enabled = true;
                }
            }
        }
        private void pausetButton_Click(object sender, EventArgs e)
        {
            if(pauseButton.Text == "Pause")
            {
                timer1.Enabled = false;
                pauseButton.Text = "Continue";
            }
            else
            {
                pauseButton.Text = "Pause";
                timer1.Enabled = true;
            }
        }
        //Apple Snake
        public void drowSnake()
        {    
            snakeParts = new PictureBox[snakeSize];
         for(int i = 0; i< snakeSize; i++)
         {
            snakeParts[i] = new PictureBox();
            snakeParts[i].Size = new Size(10,10);
            snakeParts[i].BackColor = Color.Green;
            snakeParts[i].BorderStyle = BorderStyle.FixedSingle;
            snakeParts[i].Location = new Point(location.X - (10 * i), location.Y);
            landArea.Controls.Add(snakeParts[i]);
         }
        }
        //Apple drow
        public void appleDrow()
        {
            Random position = new Random();
            int appleXDim = position.Next(38) * 10;
            int appleYDim = position.Next(30) * 10;
             
                if (snakeParts[0].Location == new Point(appleXDim, appleYDim))
                {
                   appleXDim = position.Next(38) * 10;
                   appleYDim = position.Next(38) * 10;
                }

            if(snakeTuchApple == true)
            {
                appleLocation = new Point(appleXDim, appleYDim);
                apple.Size = new Size(10,10);
                apple.BackColor = Color.Red;
                apple.BorderStyle = BorderStyle.FixedSingle;
                apple.Location = appleLocation;
                landArea.Controls.Add(apple);
                snakeTuchApple = false;
            }
        }
        //Drow wall
        public void hitItSelf()
        {
            for(int i =1; i<snakeSize; i++)
            {
                if (snakeParts[0].Location == snakeParts[i].Location)
                {
                    GameOver();
                }
            }
        }
        public void hitWall()
        {
          if(snakeParts[0].Location.X < 0 || snakeParts[0].Location.X > 402 || snakeParts[0].Location.Y < 0 || snakeParts[0].Location.Y > 341)
          {
                GameOver();
          }
      
        }
        public void GameOver() 
        {
            timer1.Enabled = false;
            Button button = new Button();
            foreach(Control c in Controls)
            {
                button = c as Button;
                if (button != null)
                {
                    button.Enabled = false;
                }
            }

        }
        //Move Snake 
        public void snakeMovement()
        {
            Point move = new Point(0, 0);
            for (int i = 0; i < snakeSize; i++)
            {
                
                if (i == 0)
                {
                    move = snakeParts[i].Location;
                    if (direction == "up")
                    {
                        snakeParts[0].Location = new Point(snakeParts[0].Location.X, snakeParts[0].Location.Y - 10);
                    }
                    if (direction == "down")
                    {
                        snakeParts[0].Location = new Point(snakeParts[0].Location.X, snakeParts[0].Location.Y + 10);
                    }
                    if (direction == "right" )
                    {
                        snakeParts[0].Location = new Point(snakeParts[0].Location.X + 10, snakeParts[0].Location.Y);
                    }
                    if (direction == "left" )
                    {
                        snakeParts[0].Location = new Point(snakeParts[0].Location.X - 10, snakeParts[0].Location.Y);
                    }
                }
                else
                {
                    Point newMove = snakeParts[i].Location;
                    snakeParts[i].Location = move;
                    move = newMove;
                }
                
            }
            changingDirection = false;
        }
        public void EatApple()
        {
    
            if (snakeParts[0].Location == appleLocation)
            {
                isAppleEaten = true;
                snakeSize++;
                PictureBox[] oldSnake = snakeParts;
                landArea.Controls.Clear();
                snakeParts = new PictureBox[snakeSize];
                for(int i = 0; i<snakeSize; i++)
                {
                    snakeParts[i] = new PictureBox();
                    snakeParts[i].Size = new Size(10,10);
                    snakeParts[i].BackColor = Color.Green;
                    snakeParts[i].BorderStyle = BorderStyle.FixedSingle;
                    if (i == 0)
                        snakeParts[i].Location = appleLocation;
                    else
                        snakeParts[i].Location = oldSnake[i - 1].Location;
                    landArea.Controls.Add(snakeParts[i]);
                }
                ScoreCount++;
                if(timer1.Interval !=50)
                    timer1.Interval -= 20;
                pointLabel.Text = ScoreCount.ToString();
                isAppleEaten = false;
                snakeTuchApple = true;
                appleDrow();
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            hitItSelf();
            hitWall();
            EatApple();
            snakeMovement();    
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
           
            if (keyData == Keys.Up && changingDirection != true && direction != "down")
            {
                direction = "up";
                changingDirection = true;
            }
            if (keyData == Keys.Down && changingDirection != true && direction != "up")
            {
                direction = "down";
                changingDirection = true;
            }
            if (keyData == Keys.Left && changingDirection != true && direction != "right")
            {
                direction = "left";
                changingDirection = true;
            }
            if (keyData == Keys.Right && changingDirection != true && direction != "left")
            {
                direction = "right";
                changingDirection = true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Made by Biniyam Yemane");
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void newSnakeGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();
        }
        private void signInButton_Click(object sender, EventArgs e)
        {
           userName = textBox1.Text;    
        }
        public void DataSource()
        {
            SqlConnection myCon = new SqlConnection(@"Data Source = (local)\SQLEXPRESS; initial catalog = GamesDataBase;
                                                                                        integrated security = True;");
            SqlDataAdapter myDA = new SqlDataAdapter("select * from Score", myCon);
            myDA.Fill(ScoreTable);
            dataGridView1.DataSource = ScoreTable;
            myCon.Close();

            SqlCommand myCommand = new SqlCommand();
            myCommand.Connection = myCon;
            myCommand.CommandText = @"insert into Score"
                + " (User_Name, RateChangeDate, shiftID, rate, payfrequency, numberss)"
                + "Values (@employeeID, @RateChangeDate, @shiftID, @rate, @payfrequency, @numberss)";


        }
    }
}
