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
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace kamikazeMazeEdit {

    class MazeGuiConsts {
        public const int sqrSidePx = 100;
        public const double wallWidthMargin = 0.08;
        public const double avoidCornerMargin = 0.1;
    }

    public class MazeGuiView : UserControl {
        Maze maze;
        List<State> pathToFollow = new List<State>(); // a list of states representing a path plan
        int delta; // width and height of cells
        MapEditorForm mazeEditorHandle = null;
        
        public MazeGuiView() {
            Init(null);
        }

        public MazeGuiView(Maze maze) {
            Init(maze);
        }
        
        void Init(Maze maze) {
            this.maze = maze;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MouseClick += new MouseEventHandler(MazeGuiPanel_MouseClick);
            this.Paint += new PaintEventHandler(MazeGuiPanel_paint);
            //this.MouseMove += new MouseEventHandler(MazeGuiPanel_Move);
            
            this.SetStyle(ControlStyles.ResizeRedraw, true); // redraw everything when resizing
        }
        
        void MazeGuiPanel_MouseClick(object sender, MouseEventArgs args) {
            if (maze==null) return;
            double ddelta = (double) delta;
            double xquot = args.X/ddelta;
            double yquot = args.Y/ddelta;
            double xdiff = (Math.Round(xquot) - xquot);
            double ydiff = (Math.Round(yquot) - yquot);
            int row = args.Y/delta;
            int col = args.X/delta;
            if (Math.Abs(xdiff) < MazeGuiConsts.wallWidthMargin 
                && Math.Abs(ydiff) > MazeGuiConsts.avoidCornerMargin) {
                // toggle vertical wall
                int toggle_col = (int) Math.Round(xquot);
                if (toggle_col <= 0 || toggle_col > maze.Cols-1) 
                    return;
                if (toggle_col == col) {
                    //System.Console.WriteLine("toggle left");
                    // the left side
                    maze.ToggleWall(row, col, Heading.LEFT);
                } else {
                    //System.Console.WriteLine("toggle right");
                    // right
                    maze.ToggleWall(row, col, Heading.RIGHT);
                }
                this.Refresh();
            } else if (Math.Abs(ydiff) < MazeGuiConsts.wallWidthMargin 
                       && Math.Abs(xdiff) > MazeGuiConsts.avoidCornerMargin) {
                // toggle vertical wall
                int toggle_row = (int) Math.Round(yquot);
                if (toggle_row <= 0 || toggle_row > maze.Rows-1) 
                    return;
                if (toggle_row == row) {
                    //System.Console.WriteLine("toggle up");
                    // the left side
                    maze.ToggleWall(row, col, Heading.UP);
                } else {
                    //System.Console.WriteLine("toggle down");
                    // right
                    maze.ToggleWall(row, col, Heading.DOWN);
                }
                this.Refresh();
            } else if (mazeEditorHandle.GoalMode) {
                // System.Console.WriteLine("Toggle goal mode");
                maze.ToggleGoal(row, col);
                this.Invalidate();
            } else if (mazeEditorHandle.CurrentRobotHeading != Heading.NONE) {
                // System.Console.WriteLine("Toggle robot w/ heading " + mazeEditorHandle.CurrentRobotHeading );
                maze.ToggleStart(row,col,mazeEditorHandle.CurrentRobotHeading);
                this.Invalidate();
            }
            
            //Console.WriteLine(xdiff + ", " + ydiff + ", r:" + row + ", c:" + col);
        }
        

        void MazeGuiPanel_paint(object sender, PaintEventArgs e) {
            if (maze == null) return;
            Graphics g = e.Graphics;
            int dx = this.Width / maze.Cols - 2;
            int dy = this.Height / maze.Rows - 2;
            delta = (dx < dy) ? dx : dy; // keep min of deltas
            int x = 0;
            int y = 0;
            //string drawstr = "";
            // draw horiz. grid lines
            int length = delta * maze.Cols;
            for (int i = 0; i < maze.Rows; ++i) {
                g.DrawLine(Pens.Gray, 0, y, length, y);
                y += delta;
            }
            // draw vertical grid lines
            length = delta*maze.Rows;
            for (int j = 0; j < maze.Cols; ++j) {
                g.DrawLine(Pens.Gray, x, 0, x, length);
                x += delta;
            }

            // draw path
            if (pathToFollow.Count > 0)
            {
                Pen pathpen = new Pen(Color.BlueViolet, 3);
                pathpen.DashCap = System.Drawing.Drawing2D.DashCap.Triangle;
                int lastx = (pathToFollow[0].Col * delta) + delta/2;
                int lasty = (pathToFollow[0].Row * delta) + delta/2;
                foreach (State fst in pathToFollow)
                {
                    x = (fst.Col * delta) + delta/2;
                    y = (fst.Row * delta) + delta/2;
                    g.DrawLine(pathpen, lastx, lasty, x, y);
                    lastx = x;
                    lasty = y;
                }
                pathpen.Dispose();
            }
            
            // draw walls
            Pen wallpen = new Pen(Color.Black, 8);
            wallpen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
            y = 0;
            for (int i = 0; i < maze.Rows; ++i) {
                x = 0;
                for (int j = 0; j < maze.Cols; ++j) {
                    //drawstr = string.Format("{0},{1} ", i, j);
                    if ((maze[i, j] & MazeCell.UP) != 0) {
                        g.DrawLine(wallpen, x, y, x + delta, y);
                    }
                    if ((maze[i, j] & MazeCell.DOWN) != 0) {
                        g.DrawLine(wallpen, x, y + delta, x + delta, y + delta);
                    }
                    if ((maze[i, j] & MazeCell.LEFT) != 0) {
                        g.DrawLine(wallpen, x, y, x, y + delta);
                    }
                    if ((maze[i, j] & MazeCell.RIGHT) != 0) {
                        g.DrawLine(wallpen, x + delta, y, x + delta, y + delta);
                    }
                    x += delta;
                }
                y += delta;
            }

            foreach (State goalstate in maze.GoalStates) {
                x = delta*goalstate.Col;
                y = delta*goalstate.Row;
                g.DrawString("G", this.Font, Brushes.DarkBlue, x + 10, y + 10);
            }
            
//            foreach (State elstate in maze.Electrolitos) {
//
//            }
//            foreach (State keystate in maze.KeyStates) {
//
//            }

            foreach (State robotstate in maze.StartStates) {
                if (robotstate.Row == -1)
                {
                    continue;
                }
                // valid robot
                int robotDiameter = delta / 2;
                // starting pt for robot is not x,y
                x = (robotstate.Col * delta) + delta / 4;
                y = (robotstate.Row * delta) + delta / 4;
                // draw robot circle
                g.DrawEllipse(Pens.Red, x, y, robotDiameter, robotDiameter);
                // center of square
                x = (robotstate.Col * delta) + delta / 2;
                y = (robotstate.Row * delta) + delta / 2;
                int x1 = 0, y1 = 0;
                // TODO: it seems GDI supports affine transformations
                // we could use rotation here.
                // OTOH it may not be worthwhile.                
                switch (robotstate.Heading) {
                    case Heading.UP:
                        x1 = (robotstate.Col * delta) + delta / 2;
                        y1 = (robotstate.Row * delta) + delta / 4;
                        break;
                    case Heading.DOWN:
                        x1 = (robotstate.Col * delta) + delta / 2;
                        y1 = (robotstate.Row * delta) + 3 * delta / 4;
                        break;
                    case Heading.LEFT:
                        x1 = (robotstate.Col * delta) + delta / 4;
                        y1 = (robotstate.Row * delta) + delta / 2;
                        break;
                    case Heading.RIGHT:
                        x1 = (robotstate.Col * delta) + 3 * delta / 4;
                        y1 = (robotstate.Row * delta) + delta / 2;
                        break;
                }
                // draw robot stripe
                g.DrawLine(Pens.Black, x, y, x1, y1);
            }
            wallpen.Dispose();
        }
        
        public Maze Maze {
            set { 
                this.maze = value;
                this.Refresh();
            } 
			get {
				return this.maze;
			}
        }
		
        
        public MapEditorForm MazeEditorHandle {
            set {
                this.mazeEditorHandle = value;
            }
        }

        public List<State> PathToFollow
        {
            set
            {
                this.pathToFollow = value;
                this.Refresh();
            }
        }
        
    } // mazeguipanel
} // namespace
