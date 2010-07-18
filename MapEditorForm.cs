// Copyright (C) 2008 Daniel Maturana
// This file is part of KamikazeMazeEdit.
// 
// KamikazeMazeEdit is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// KamikazeMazeEdit is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with KamikazeMapEdit. If not, see <http://www.gnu.org/licenses/>.
// 
// MapEditorForm.cs 
// User: acantostega at 9:33 PM 7/14/2008
//

using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace kamikazeMazeEdit
{
    
    public class MapEditorForm : Form
    {
        const int MIN_SIZE_H = 400;
        const int MIN_SIZE_W = 400;
        const int PADDING = 10;
        const int TOOLBAR_BITMAP_SIDE_LEN = 16;
        MazeGuiView mazeGuiView = new MazeGuiView();
        bool unsaved = false;
        
        Dictionary<string, Heading> buttonNameToHeading = new Dictionary<string,Heading>();
        bool goalButtonPushed = false;
        Heading robotButtonPushed = Heading.NONE;
        
        public MapEditorForm()
        {
            
            this.Text = "Float";
            this.MinimumSize = new Size(MIN_SIZE_W,MIN_SIZE_H);
            
            mazeGuiView.MazeEditorHandle = this;
            mazeGuiView.AutoScroll = true;
            //mazeGuiView.BackColor = System.Drawing.Color.White;
            mazeGuiView.Padding = new Padding(PADDING,PADDING,PADDING,PADDING);
            mazeGuiView.Dock = DockStyle.Fill;
            this.DockPadding.All = PADDING;
            this.Controls.Add(mazeGuiView);
            
            MainMenu mainMenu = new MainMenu();
            MenuItem quitMenuItem = new MenuItem("&Quit", new EventHandler(quitMenuItem_OnClick));
            quitMenuItem.Shortcut = Shortcut.CtrlQ;
            MenuItem newMenuItem = new MenuItem("&New...", new EventHandler(newMenuItem_OnClick));
            newMenuItem.Shortcut = Shortcut.CtrlN;
            MenuItem openMenuItem = new MenuItem("&Open...", new EventHandler(openMenuItem_OnClick));
            openMenuItem.Shortcut = Shortcut.CtrlO;
            MenuItem saveMenuItem = new MenuItem("&Save As...", new EventHandler(saveMenuItem_OnClick));
            saveMenuItem.Shortcut = Shortcut.CtrlS;
            MenuItem fileMenuItem = new MenuItem("&File");
            
            ToolBar tb = new ToolBar();
            
            // Draw picture of little robot
            Bitmap bmp = new Bitmap(TOOLBAR_BITMAP_SIDE_LEN,TOOLBAR_BITMAP_SIDE_LEN);
            Graphics bmpg = Graphics.FromImage(bmp);
            bmpg.DrawEllipse(Pens.Red, bmp.Height/4, bmp.Height/4, bmp.Height/2, bmp.Height/2);
            bmpg.DrawLine(Pens.Black, bmp.Height/2, bmp.Height/2, bmp.Height/2, bmp.Height/4);
            
            ImageList imglist = new ImageList();
            // up
            imglist.Images.Add((Image)bmp.Clone());
            // right            
            bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
            imglist.Images.Add((Image)bmp.Clone());

            // down
            bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
            imglist.Images.Add((Image)bmp.Clone());
            
            // left            
            bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
            imglist.Images.Add((Image)bmp.Clone());
            
            // Draw a "G" TODO: on some computers it looks wrong. 
            // I'm probably doing it wrong.
            bmp = new Bitmap(TOOLBAR_BITMAP_SIDE_LEN,TOOLBAR_BITMAP_SIDE_LEN);
            bmpg = Graphics.FromImage(bmp);
            bmpg.DrawString("G", this.Font, Brushes.DarkBlue, 1, 1);						
			//bmpg.DrawString("G", this.Font, Brushes.DarkBlue, new RectangleF(1,1,bmp.Width, bmp.Height));
            // G
            imglist.Images.Add(bmp);
            
            tb.ImageList = imglist;
            ToolBarButton robotUp = new ToolBarButton();
            robotUp.ImageIndex = 0;
            robotUp.Name = "RobotUp";
            ToolBarButton robotRight = new ToolBarButton();
            robotRight.ImageIndex = 1;
            robotRight.Name = "RobotRight";
            ToolBarButton robotDown = new ToolBarButton();
            robotDown.ImageIndex = 2;      
            robotDown.Name = "RobotDown";
            ToolBarButton robotLeft = new ToolBarButton();
            robotLeft.ImageIndex = 3;
            robotLeft.Name = "RobotLeft";
            ToolBarButton goal = new ToolBarButton();
            goal.ImageIndex = 4;
            goal.Name = "Goal";
            
            // fill dictionary thing with corresponding tokens
            buttonNameToHeading[robotUp.Name] = Heading.UP;
            buttonNameToHeading[robotDown.Name] = Heading.DOWN;
            buttonNameToHeading[robotLeft.Name] = Heading.LEFT;
            buttonNameToHeading[robotRight.Name] = Heading.RIGHT;
            buttonNameToHeading["None"] = Heading.NONE;
            
            robotUp.Style 
                = robotDown.Style
                = robotLeft.Style 
                = robotRight.Style 
                = goal.Style 
                = ToolBarButtonStyle.ToggleButton;
            
            tb.ButtonClick += new ToolBarButtonClickEventHandler(robotButton_Toggle);
            
            tb.Buttons.AddRange(new ToolBarButton[] {robotUp, robotDown, robotLeft, robotRight, goal});
            
            this.Controls.Add(tb);
            
            fileMenuItem.MenuItems.Add(newMenuItem);
            fileMenuItem.MenuItems.Add(openMenuItem);
            fileMenuItem.MenuItems.Add(saveMenuItem);
            fileMenuItem.MenuItems.Add(quitMenuItem);
            mainMenu.MenuItems.Add(fileMenuItem);
            this.Menu = mainMenu;
            
        }
        
        void robotButton_Toggle(object sender, ToolBarButtonClickEventArgs args) {
            ToolBar tb = (ToolBar) sender;
            foreach (ToolBarButton btn in tb.Buttons) {
                if (btn != args.Button)
                    btn.Pushed = false;
            }
            if (args.Button.Pushed) {
                if (args.Button.Name.Equals("Goal")) {
                    goalButtonPushed = true;
                    robotButtonPushed = Heading.NONE;
                } else {
                    robotButtonPushed = buttonNameToHeading[args.Button.Name];
                    goalButtonPushed = false;
                }
            } else {
                goalButtonPushed = false;
                robotButtonPushed = Heading.NONE;
            }            
        }
        
        void quitMenuItem_OnClick(Object sender, EventArgs e) {
            if (checkSaved())
                Application.Exit();
        }
        
        void openMenuItem_OnClick(Object sender, EventArgs e) {
            if (!checkSaved()) 
                return;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Float Open Map";
            dlg.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            dlg.InitialDirectory = Environment.CurrentDirectory;
            if (dlg.ShowDialog() == DialogResult.OK) {
                string file = dlg.FileName;
                System.Console.WriteLine("file:" + file);
                try { 
                    Maze mz = new Maze(file);
                    mazeGuiView.Maze = mz;             
                }
                catch (FileLoadException ex) {
                    MessageBox.Show(this, "There was a FileLoadException, whatever that is. (" + ex.Message + ")", 
					                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //System.Console.WriteLine("File load exception.");
                }
                catch (FileNotFoundException ex) {
                    MessageBox.Show(this, "File not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    System.Console.Error.WriteLine("ERROR: " + ex.Message);
                    
                    //System.Console.WriteLine("File not found.");
                } catch (MazeException ex) {
                    MessageBox.Show(this, "Couldn't read maze file " + file + ", error: " + ex.Message, "Error",
					                MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                }
            }
        }
        
        bool checkSaved() {
            if (!unsaved) return true;
            // ask. 
            string msg = "Maze not saved! Save?";
            DialogResult result = MessageBox.Show(this,msg,"Save?",MessageBoxButtons.YesNoCancel,
                                                  MessageBoxIcon.Question);
            if (result == DialogResult.Cancel)
                return false; // cancel whatever we were doing
            
            if (result == DialogResult.No) 
                return true; //go on doing what we were doing
            
            if (result == DialogResult.Yes) {
                saveMaze();
            }
            return true;
        }
        
        void newMenuItem_OnClick(Object sender, EventArgs e) {
            if (!checkSaved()) return;
            // ask for #rows, #cols
            using (AskRowCols dlg = new AskRowCols()) {
                if (dlg.ShowDialog() == DialogResult.OK) {
                    int rows = dlg.Rows;
                    int cols = dlg.Cols;
                    // System.Console.WriteLine("R: " + rows + ", C: " + cols);
                    this.mazeGuiView.Maze = new Maze(rows, cols);
                }                
            }
           
        }
        
        void saveMenuItem_OnClick(Object sender, EventArgs e) {
            saveMaze();
        }
        
        void saveMaze() {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.OverwritePrompt = true;
            dlg.Title = "Float Open Map";
            dlg.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            dlg.InitialDirectory = Environment.CurrentDirectory;
            if (dlg.ShowDialog() == DialogResult.OK) {
                string file = dlg.FileName;
                System.Console.WriteLine("Save to " + file);
				try {
                	mazeGuiView.Maze.SaveMaze(file);
				} catch (FileLoadException ex) {
                    MessageBox.Show(this, "There was an error: " + ex.Message, 
					                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //System.Console.WriteLine("File load exception.");
                }
            }
        }
        public bool GoalMode {
            get {
                return goalButtonPushed;
            }
        }    
                    
        public Heading CurrentRobotHeading {
            get {
                return robotButtonPushed;
            }
        }

    }
}
