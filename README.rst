Kamikaze Maze Edit 0.1
======================

Daniel Maturana 2008

Introduction
------------

This is a GUI editor for robot mazes in the textual format used in PUC's
IIC3684 mobile robotics course. It is named after our robot, Kamikaze,
which in turn was named for its tendency to crash.

Usage
------

Mazes are grid of cells which may be separated by walls. Each goal may be
occupied by two entities:

- A 'goal', in which case the cell is marked with a 'G'. There may be more than
  one goal cell in the maze.
- A 'starting point robot', in which case the cell is marked with a drawing of
  a robot with a line indicating the orientation. Each maze may have more than
  one potential starting point cell, and in each starting point cell the robot
  may be found in one or more of four possible orientations (N, S, W, E).
  Total ignorance of the starting position would be represented as having each
  cell in the maze occupied by four 'starting point robots', one in each
  orientations.  Note that if there is more than one starting point robot in a
  cell, their drawings are superimposed.

To add a wall, simply click on the space between cells where the wall should
be. To remove it click again in the same place.

To add a starting point robot in a certain position, click the robot button in
the toolbar with the desired orientation, and then click a cell to toggle the
presence of the robot. To stop adding robots, toggle the button in the toolbar.

To add a goal, click the button labeled 'G' in the toolbar. The usage is
analogous to that of the starting point robots.

To save, load, and create new mazes use the 'File' menu. You can also use it to
quit.

The textual format is summarized with a diagram in 'maze_format.png'.

Building
--------

This is a C# 2.0 application developed with mono. It should work in all platforms
wit a .net runtime, but it has only been tested in Linux.

You can build it with the monodevelop project file (kamikazeMazeEdit.mds). If
you prefer the command line you simply use make with the supplied Makefile. (It
presupposes the gmcs mono compiler is being used). 

