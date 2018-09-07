namespace Mazes
{
	public static class MazeTasks
	{
        static void Move (Robot robot, Direction dir, int count)
        {
            for (int i = 0; i < count; i++)
                robot.MoveTo(dir);
        }
        public static void MoveOutFromEmptyMaze(Robot robot, int width, int height)
		{
            Move(robot, Direction.Right, width - 3);
            Move(robot, Direction.Down, height - 3);
        } 

		public static void MoveOutFromSnakeMaze(Robot robot, int width, int height)
		{
            for (int i = 0; i < (height - 1) / 4; i++)
            {
                Move(robot, Direction.Right, width - 3);
                Move(robot, Direction.Down, 2);
                Move(robot, Direction.Left, width - 3);
                if (robot.Finished == false)
                    Move(robot, Direction.Down, 2);
            }
		}

		public static void MoveOutFromPyramidMaze(Robot robot, int width, int height)
		{
            int variableWidth = width - 3;
            for (int i = 0; i < (height - 1) / 4; i++)
            {
                Move(robot, Direction.Right, variableWidth);
                Move(robot, Direction.Up, 2);
                variableWidth -= 2;
                Move(robot, Direction.Left, variableWidth);
                variableWidth -= 2;
                if (robot.Finished == false)
                    Move(robot, Direction.Up, 2);
            }
        }
        public static void MoveOutFromDiagonalMaze (Robot robot, int width, int height)
        {
            int count;
            if (width > height)
                count = height - 2;
            else count = width - 2;
            for (int i = 0; i < count; i++)
            {
                if (width > height)
                {
                    Move(robot, Direction.Right, (width - 2 + height - 3) / (height - 2) - 1);
                    if (robot.Finished == false)
                        Move(robot, Direction.Down, 1);
                }
                else
                {
                    Move(robot, Direction.Down, (height - 2 + width - 3) / (width - 2) - 1);
                    if (robot.Finished == false)
                        Move(robot, Direction.Right, 1);
                }
            }
        }
    }
}