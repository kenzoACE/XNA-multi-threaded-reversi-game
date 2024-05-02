using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;

namespace Reversi
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        MouseState mouseState = new MouseState();
        KeyboardState keyboardState = new KeyboardState();
        String testingString = "";
        String winnerString = "";
        String messageString = "Press Enter to start a new game";
        String scoreString = "";
        String informationString = "Number of Snapshots: ";
        String advancedString = "  Advanced mode: Off";
        bool GAME_START = true;
        bool COMPUTER_WON = false;
        bool PLAYER_WON = false;
        bool ADVANCED_MODE = false;
        bool ANIMATION_WHITE = false;
        const int NUM_SNAPSHOTS = 999999;
        //bool NO_WINNER = false;
        int computerWins = 0;
        int playerWins = 0;
        int gridCount = 0;
        int minGridCount = int.MaxValue;
        int oldMinGridCount = 0;
        String computerString = "Press a number 0-8: Current compute time (approximate) is 3 seconds";
        SpriteFont spriteFont = null;
        int debugX = 0, debugY = 0;
        int threadX = -1;
        int threadY = -1;
        int[] threadXarray = new int[64];
        int[] threadYarray = new int[64];
        string[,] threadGrid = new string[8, 8];
        string[, ,] threadGrid2 = new string[64, 8, 8];
        int threadGridCount = 0;
        System.Threading.Timer globalTimer = null;
        System.Threading.Timer globalCountTimer = null;
        int[,] computerMoves = new int[32, 2];
        //int[, , ,] playerWinGrid = new int[2, NUM_SNAPSHOTS + 1, 8, 8];
        UInt32[, ,] globalPlayerWinGrid = new UInt32[NUM_SNAPSHOTS + 1, 8, 8]; //[moveNumber, snapshot number, x and y (0 being x, 1 being y), x index, y index]
        int[, , ,] globalPlayerWinGridSimple = new int[32, 2, 8, 8];  //fist index containing the thread number, second being 0 is compute win, 1 is player win, third and forth index is xy coordinate
        int globalSnapShotCounter = 0;
        int[, ,] playerWinGrid = new int[2, 8, 8];

        int[,] threadTable = new int[8, 8];  //contains the thread number of move made by the compute.  Must be initialized to -1 since 0 means first thread
        int[,] globalThreadGridCount = new int[20, 1];

        int globalNumMoves = 0;
        int[,] globalGridNums = new int[32, 1];

        string[,] grid = new string[8, 8] { { "-", "-", "-", "-", "-", "-", "-", "-" }, { "-", "-", "-", "-", "-", "-", "-", "-" }, { "-", "-", "-", "-", "-", "-", "-", "-"}, { "-", "-", "-", "X", "Y", "-", "-", "-"},
                                            { "-", "-", "-", "Y", "X", "-", "-", "-"}, { "-", "-", "-", "-", "-", "-", "-", "-"}, { "-", "-", "-", "-", "-", "-", "-", "-"}, { "-", "-", "-", "-", "-", "-", "-", "-"} };
        string[,] tempGrid = new string[8, 8] { { "-", "-", "-", "-", "-", "-", "-", "-" }, { "-", "-", "-", "-", "-", "-", "-", "-" }, { "-", "-", "-", "-", "-", "-", "-", "-"}, { "-", "-", "-", "-", "-", "-", "-", "-"},
        
                                    { "-", "-", "-", "-", "-", "-", "-", "-"}, { "-", "-", "-", "-", "-", "-", "-", "-"}, { "-", "-", "-", "-", "-", "-", "-", "-"}, { "-", "-", "-", "-", "-", "-", "-", "-"} };
        string[,] oldGrid = new string[8, 8];

        int[,] globalPieceCount = new int[8, 8];

        int frameCounter = 0;

        bool PLAYER_TURN = true;
        bool PLAYER_START = true;

        bool THREAD_IS_ACTIVE = false;
        bool COMPUTER_THREAD_IS_EXITING = false;
        bool THREADS_ARE_ACTIVE = false;
        bool THREADS_ARE_EXITING = false;
        bool SECONDS_REACHED = false;

        bool CURSOR = false;
        bool MAXIMUM_SNAPSHOTS = false;
        bool BREAK_PRESSED = false;

        Random random = new Random();
        int moveCount = 0;

        int advancedSnapshots = 0;

        AutoResetEvent[] stateInfoArray = new AutoResetEvent[64];

        int count = 0;                   //counts the number of milliseconds the compute will compute
        int count2 = 0;
        int count3 = 0;
        int count4 = 0;
        int count5 = 0;
        int count6 = 0;
        int ComputerMilliSeconds = 3000; //number of milliseconds the computer player will compute

        //contains xy grid number for that move (no more than 32 moves); last index 0 being x for that move and 1 for y.
        int[,] threadGridsHashTable = new int[32, 2];
        int globalThreadqueNum = 0;

        System.Threading.Timer[] threads = new System.Threading.Timer[32];

        System.Timers.Timer timer = new System.Timers.Timer();
        System.Timers.Timer timer2 = new System.Timers.Timer();
        System.Timers.Timer timer3 = new System.Timers.Timer();
        System.Timers.Timer timer4 = new System.Timers.Timer();
        System.Timers.Timer timer5= new System.Timers.Timer();
        System.Timers.Timer timer6 = new System.Timers.Timer();



        GameComponent game;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            game = new GameComponent(this);
            Content.RootDirectory = "Content";
            timer.Interval = 50;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer2.Interval = 50;
            timer2.Elapsed += new ElapsedEventHandler(timer_Elapsed2);
            timer3.Interval = 50;
            timer3.Elapsed += new ElapsedEventHandler(timer_Elapsed3);
            timer4.Interval = 50;
            timer4.Elapsed += new ElapsedEventHandler(timer_Elapsed4);
            timer5.Interval = 50;
            timer5.Elapsed += new ElapsedEventHandler(timer_Elapsed5);
            timer6.Interval = 50;
            timer6.Elapsed += new ElapsedEventHandler(timer_Elapsed6);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            count += 50;

            if (count >= ComputerMilliSeconds)
            {
                SECONDS_REACHED = true;
            }
        }

        void timer_Elapsed2(object sender, ElapsedEventArgs e)
        {
            count2 += 50;

            if (count >= ComputerMilliSeconds)
            {
                SECONDS_REACHED = true;
            }
        }

        void timer_Elapsed3(object sender, ElapsedEventArgs e)
        {
            count3 += 50;

            if (count >= ComputerMilliSeconds)
            {
                SECONDS_REACHED = true;
            }
        }

        void timer_Elapsed4(object sender, ElapsedEventArgs e)
        {
            count4 += 50;

             if (count >= ComputerMilliSeconds)
            {
                SECONDS_REACHED = true;
            }
        }

        void timer_Elapsed5(object sender, ElapsedEventArgs e)
        {
            count5 += 50;

            if (count >= ComputerMilliSeconds)
            {
                SECONDS_REACHED = true;
            }
        }

        void timer_Elapsed6(object sender, ElapsedEventArgs e)
        {
            count6 += 50;

            if (count >= ComputerMilliSeconds)
            {
                SECONDS_REACHED = true;
            }
        }
        /*
                                                        void timer_Elapsed2(object sender, ElapsedEventArgs e)
                                                        {
                                                            count2 += 50;

                                                            if (count2 > ComputerMilliSeconds * globalThreadqueNum)
                                                            {
                                                                SECONDS_REACHED = true;
                                                            }
                                                        }

                                                        void timer_Elapsed3(object sender, ElapsedEventArgs e)
                                                        {
                                                            if (count > ComputerMilliSeconds || count2 > ComputerMilliSeconds * globalThreadqueNum)
                                                            {
                                                                SECONDS_REACHED = true;
                                                            }
                                                        }
                                                 */       
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            base.Window.AllowUserResizing = false;

            game.Game.IsMouseVisible = true;

            for (int x = 0; x < 64; x++)
            {
                stateInfoArray[x] = new AutoResetEvent(false);
            }

            timer2.Start();
            timer3.Start();
            timer4.Start();
            timer5.Start();
            timer6.Start();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            if (globalTimer != null)
            {
                globalTimer.Dispose();
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            double lastButtonSeconds = 0;

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            if (game.Game.IsActive)
            {
                if (PLAYER_TURN || !THREAD_IS_ACTIVE)
                {
                    //keyboardState = Keyboard.GetState();
                }

                keyboardState = Keyboard.GetState();

                mouseState = Mouse.GetState();

                if (!MAXIMUM_SNAPSHOTS)
                {

                    if (keyboardState.IsKeyDown(Keys.D0) || keyboardState.IsKeyDown(Keys.NumPad0))
                    {
                        ComputerMilliSeconds = 150;
                        computerString = "Press a number 0-8: Current compute time (approximate) is 0.15 seconds";
                    }
                    else if (keyboardState.IsKeyDown(Keys.D1) || keyboardState.IsKeyDown(Keys.NumPad1))
                    {
                        ComputerMilliSeconds = 500;
                        computerString = "Press a number 0-8: Current compute time (approximate) is 0.5 second";
                    }
                    else if (keyboardState.IsKeyDown(Keys.D2) || keyboardState.IsKeyDown(Keys.NumPad2))
                    {
                        ComputerMilliSeconds = 1000;
                        computerString = "Press a number 0-8: Current compute time (approximate) is 1 seconds";
                    }
                    else if (keyboardState.IsKeyDown(Keys.D3) || keyboardState.IsKeyDown(Keys.NumPad3))
                    {
                        ComputerMilliSeconds = 1500;
                        computerString = "Press a number 0-8: Current compute time (approximate) is 1.5 seconds";
                    }
                    else if (keyboardState.IsKeyDown(Keys.D4) || keyboardState.IsKeyDown(Keys.NumPad4))
                    {
                        ComputerMilliSeconds = 2000;
                        computerString = "Press a number 0-8: Current compute time (approximate) is 2 seconds";
                    }
                    else if (keyboardState.IsKeyDown(Keys.D5) || keyboardState.IsKeyDown(Keys.NumPad5))
                    {
                        ComputerMilliSeconds = 3000;
                        computerString = "Press a number 0-8: Current compute time (approximate) is 3 seconds";
                    }
                    else if (keyboardState.IsKeyDown(Keys.D6) || keyboardState.IsKeyDown(Keys.NumPad6))
                    {
                        ComputerMilliSeconds = 4000;
                        computerString = "Press a number 0-8: Current compute time (approximate) is 4 seconds";
                    }
                    else if (keyboardState.IsKeyDown(Keys.D7) || keyboardState.IsKeyDown(Keys.NumPad7))
                    {
                        ComputerMilliSeconds = 5000;
                        computerString = "Press a number 0-8: Current compute time (approximate) is 5 seconds";
                    }
                    else if (keyboardState.IsKeyDown(Keys.D8) || keyboardState.IsKeyDown(Keys.NumPad8))
                    {
                        ComputerMilliSeconds = 10000;
                        computerString = "Press a number 0-8: Current compute time (approximate) is 10 seconds";
                    }
                    else if (keyboardState.IsKeyDown(Keys.D9) || keyboardState.IsKeyDown(Keys.NumPad9) && PLAYER_TURN == true)
                    {
                        if (ADVANCED_MODE)
                        {
                            //                    ADVANCED_MODE = false;
                            //                    advancedString = "  Advanced mode: Off";
                        }
                        else
                        {
                            ADVANCED_MODE = true;
                            advancedString = "  Advanced mode: On";
                            ComputerMilliSeconds = 8000;
                            computerString = "Press a number 0-8: Current compute time (approximate) is 8 seconds";
                        }
                    }
                }

                if (keyboardState.IsKeyDown(Keys.Enter) && !GAME_START)
                {
                    grid = new string[8, 8] { { "-", "-", "-", "-", "-", "-", "-", "-" }, { "-", "-", "-", "-", "-", "-", "-", "-" }, { "-", "-", "-", "-", "-", "-", "-", "-"}, { "-", "-", "-", "X", "Y", "-", "-", "-"},
                                            { "-", "-", "-", "Y", "X", "-", "-", "-"}, { "-", "-", "-", "-", "-", "-", "-", "-"}, { "-", "-", "-", "-", "-", "-", "-", "-"}, { "-", "-", "-", "-", "-", "-", "-", "-"} };
                    moveCount = 0;
                    winnerString = "";
                    COMPUTER_WON = false;
                    PLAYER_WON = false;
                    GAME_START = true;
                    PLAYER_TURN = true;
                    ANIMATION_WHITE = false;
                    oldMinGridCount = 0;
                    advancedSnapshots = 0;
                }

                int x = 0;
                int y = 0;

                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    x = mouseState.X;
                    y = mouseState.Y;
                }

                if (x > 530 && x < 630 && y > 260 && y < 320)
                {
                    CURSOR = true;
                }

                if (x > 630 && x < 730 && y > 260 && y < 320)
                {
                    CURSOR = false;
                }

                if (ADVANCED_MODE && x > 530 && x < 750 && y > 380 && y < 500)
                {
                    MAXIMUM_SNAPSHOTS = true;
                    computerString = "Press break key to end a computer turn.";
                }

                if (keyboardState.IsKeyDown(Keys.Pause) && MAXIMUM_SNAPSHOTS)
                {
                    THREADS_ARE_EXITING = true;
                }

                //testingString = "Button Pressed.  X:" + x + "Y:" + y;

                if (mouseState.LeftButton == ButtonState.Pressed && x < 512 && x > 10 && y < 512 && y > 10 && GAME_START && PLAYER_TURN)
                {
                    x = 7 - ((512 - x) / (512 / 8));
                    y = 7 - ((512 - y) / (512 / 8));

                    //testingString = "Button Pressed.  Location X:" + x + "Y:" + y;

                    //fill the grid
                    if (PLAYER_TURN && grid[x, y] == "-")
                    {
                        if (globalTimer != null)
                        {
                            globalTimer.Dispose();
                        }

                        if (checkBlackMove(grid, x, y))
                        {
                            if (Array.Equals(oldGrid, grid))
                            {
                                //testingString = "->No move has been made<-";
                            }

                            oldGrid = (string[,])grid.Clone();

                            makeBlackMove(out grid, grid, x, y);

                            if (moveCount == 0)
                            {
                                PLAYER_START = true;
                            }

                            moveCount++;
                            PLAYER_TURN = false;

                            if (!checkWhiteNoMove(grid))  //computer has no move; skip turn
                            {
                                PLAYER_TURN = true;
                                ANIMATION_WHITE = true;
                            }
                            else
                            {
                                count = 0;
                                count2 = 0;
                                count3 = 0;
                                count4 = 0;
                                count5 = 0;
                                count6 = 0;

                                timer.Start();
                                //timer2.Start();

                                if (globalCountTimer != null)
                                {
                                    globalCountTimer.Dispose();
                                }

                                for (int x2 = 0; x2 < 32; x2++)
                                {
                                    if (threads[x2] != null)
                                    {
                                        threads[x2].Dispose();
                                    }
                                }

                                globalSnapShotCounter = 0;

                                /*
                                // Create the delegate that invokes methods for the timer.
                                TimerCallback timerDelegate = new TimerCallback(CheckStatus);

                                // Create a timer that signals the delegate to invoke 
                                // CheckStatus after 0.1 seconds
                                Timer stateTimer = new Timer(timerDelegate, null, 50, 10);
                                globalCountTimer = stateTimer;
                                */

                                THREAD_IS_ACTIVE = true;
                                THREADS_ARE_ACTIVE = false;
                                COMPUTER_THREAD_IS_EXITING = false;
                                THREADS_ARE_EXITING = false;
                                SECONDS_REACHED = false;
                                BREAK_PRESSED = false;
                                 
                                //main thread
                                AutoResetEvent autoResetEvent = new AutoResetEvent(false);
                                // Create the delegate that invokes methods for the timer.
                                TimerCallback timerDelegate2 = new TimerCallback(ComputerMoveThread);
                                // Create a timer that signals the delegate to invoke 
                                System.Threading.Timer stateTimer2 = new System.Threading.Timer(timerDelegate2, autoResetEvent, 0, System.Threading.Timeout.Infinite);
                                globalTimer = stateTimer2;
                                //autoResetEvent.WaitOne(TimeSpan.FromMilliseconds(ComputerMilliSeconds + 1000));
                                //stateTimer.Dispose();

                                if (ADVANCED_MODE)
                                {
                                    //move anticipation threads
                                    threadGridsHashTable = new int[32, 2];
                                    //threadGridsHashTable.Initialize();
                                    //globalPlayerWinGrid = new int[16, NUM_SNAPSHOTS / 10 + 1, 2, 8, 8];
                                    globalPlayerWinGridSimple = new int[32, 2, 8, 8];
                                    //globalPlayerWinGridSimple.Initialize();
                                    globalGridNums = new int[32, 1];
                                    //globalGridNums.Initialize();
                                    globalThreadqueNum = 0; //reset the que number
                                    globalNumMoves = 0;
                                    globalThreadGridCount = new int[20, 1];
                                    //globalThreadGridCount.Initialize();

                                    if(advancedSnapshots != NUM_SNAPSHOTS)
                                    {
                                        oldMinGridCount = advancedSnapshots;
                                    }

                                    advancedSnapshots = NUM_SNAPSHOTS;

                                    //
                                    for (int x5 = 0; x5 < 8; x5++)
                                    {
                                        for (int y5 = 0; y5 < 8; y5++)
                                        {
                                            threadTable[x5, y5] = -1;
                                        }
                                    }
                                    //
                                    findMoveWhite(out computerMoves, out globalNumMoves, grid);
                                    //

                                    int temp = 0;

                                    //create multiple threads here
                                    for (int x2 = 0; x2 < globalNumMoves; x2++)
                                    {
                                        //wait untill the thread has been reached and que number updated unless its the first thread
                                        while (x2 != 0 && temp == globalThreadqueNum)
                                        {
                                            Thread.Sleep(5);
                                        }
                                        temp = globalThreadqueNum;  //record the global variable 

                                        //Create a thread for all possible compute moves for anticipating the player's next move 
                                        TimerCallback timerDelegate3 = new TimerCallback(PlayerMoveThread);
                                        AutoResetEvent autoResetEvent3 = new AutoResetEvent(false);
                                        //int hash = autoResetEvent2.GetHashCode();
                                        //threadGridsHashTable[0, x, 0] = hash;
                                        threadGridsHashTable[x2, 0] = computerMoves[x2, 0];
                                        threadGridsHashTable[x2, 1] = computerMoves[x2, 1];
                                        threadTable[computerMoves[x2, 0], computerMoves[x2, 1]] = x2;  //used to keep track of indivisual threads
                                        System.Threading.Timer stateTimer3 = new System.Threading.Timer(timerDelegate3, autoResetEvent3, 0, System.Threading.Timeout.Infinite);
                                        threads[x2] = stateTimer3;


                                        //autoResetEvent3.WaitOne();
                                    }

                                    THREADS_ARE_ACTIVE = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        /*                    if (!checkBlackNoMove(grid))
                                            {
                                                PLAYER_TURN = false;
                                                // Create the delegate that invokes methods for the timer.
                                                TimerCallback timerDelegate = new TimerCallback(ComputerMoveThread);

                                                // Create a timer that signals the delegate to invoke 
                                                // CheckStatus after 0.5 seconds
                                                Timer stateTimer = new Timer(timerDelegate, null, 0, System.Threading.Timeout.Infinite);
                                            }                    
                         */
                    }

                    int blackCount = 0;
                    int whiteCount = 0;

                    int winner = CheckForWinner(grid, out blackCount, out whiteCount);

                    if (winner != 0 && GAME_START)
                    {
                        if (winner == -1)
                        {
                            winnerString = "Computer is winner. " + blackCount.ToString() + "-" + whiteCount.ToString();
                            computerWins++;
                        }
                        else if (winner == 1)
                        {
                            winnerString = "Player is winner. " + blackCount.ToString() + "-" + whiteCount.ToString();
                            playerWins++;
                        }
                        else if (winner == -2)
                        {
                            winnerString = "Game is a draw. " + blackCount.ToString() + "-" + whiteCount.ToString();
                        }

                        GAME_START = false;
                    }
                }


                /*
                            if (!PLAYER_TURN && GAME_START)
                            {
                                //PLAYER_TURN = true;

                                int randX = 0;
                                int randY = 0;
                                bool computerWin = false;
                                bool playerWin = false;


                                // Create the delegate that invokes methods for the timer.
                                TimerCallback timerDelegate = new TimerCallback(CheckStatus);

                                // Create a timer that signals the delegate to invoke 
                                // CheckStatus after 0.5 seconds
                                Timer stateTimer = new Timer(timerDelegate, null, 0, 100);

                                int tempMoveCount = moveCount;
                                bool COMPUTER_TURN = true;
                                int[, , ,] computerWinGrid = new int[2, NUM_SNAPSHOTS + 1, 8, 8];
                                int computerX = -1;
                                int computerY = -1;
                                int playerX = 0;
                                int playerY = 0;
                                int forceX = -1;
                                int forceY = -1;
                                bool IS_FAULTY_MOVE = false;
                                bool FORCE_MOVE = false;
                

                                gridCount = 0;  //global variable
                */

                if (!THREAD_IS_ACTIVE && !PLAYER_TURN && GAME_START && !checkBlackNoMove(grid))
                {
                    count = 0;
                    count2 = 0;
                    count2 = 0;
                    count3 = 0;
                    count4 = 0;
                    count5 = 0;
                    count6 = 0;

                    timer.Start();
                    //timer2.Start();

                    if (globalCountTimer != null)
                    {
                        globalCountTimer.Dispose();
                    }

                    for (int x2 = 0; x2 < 32; x2++)
                    {
                        if (threads[x2] != null)
                        {
                            threads[x2].Dispose();
                        }
                    }

                    
                    globalSnapShotCounter = 0;

                    /*
                    // Create the delegate that invokes methods for the timer.
                    TimerCallback timerDelegate = new TimerCallback(CheckStatus);
                    // Create a timer that signals the delegate to invoke 
                    // CheckStatus after 0.1 seconds
                    Timer stateTimer = new Timer(timerDelegate, null, 50, 10);
                    
                    globalCountTimer = stateTimer;

                    if (globalTimer != null)
                    {
                        globalTimer.Dispose();
                    }
                    */

                    THREAD_IS_ACTIVE = true;
                    THREADS_ARE_ACTIVE = false;
                    COMPUTER_THREAD_IS_EXITING = false;
                    THREADS_ARE_EXITING = false;
                    SECONDS_REACHED = false;
                    BREAK_PRESSED = false;
                    //game.Game.Services.RemoveService((Type)Mouse.WindowHandle.GetType());  //remove mouse service to improve performance

                    //main thread
                    AutoResetEvent autoResetEvent = new AutoResetEvent(false);
                    // Create the delegate that invokes methods for the timer.
                    TimerCallback timerDelegate2 = new TimerCallback(ComputerMoveThread);
                    // Create a timer that signals the delegate to invoke 
                    // CheckStatus after 0.5 seconds
                    System.Threading.Timer stateTimer2 = new System.Threading.Timer(timerDelegate2, autoResetEvent, 0, System.Threading.Timeout.Infinite);
                    globalTimer = stateTimer2;
                    //autoResetEvent.WaitOne(TimeSpan.FromMilliseconds(ComputerMilliSeconds + 1000));
                    //stateTimer.Dispose();

                    if (ADVANCED_MODE)
                    {
                        //move aniticipation thread
                        threadGridsHashTable = new int[32, 2];
                        //threadGridsHashTable.Initialize();
                        //globalPlayerWinGrid = new int[16, NUM_SNAPSHOTS / 10 + 1, 2, 8, 8];
                        globalPlayerWinGridSimple = new int[32, 2, 8, 8];
                        //globalPlayerWinGridSimple.Initialize();
                        globalGridNums = new int[32, 1];
                        //globalGridNums.Initialize();
                        globalThreadqueNum = 0; //reset the que number
                        globalNumMoves = 0;
                        //globalThreadGridCount.Initialize();
                        globalThreadGridCount = new int[20, 1];
                        
                        
                        if (advancedSnapshots != NUM_SNAPSHOTS)
                        {
                            oldMinGridCount = advancedSnapshots;
                        }

                        advancedSnapshots = NUM_SNAPSHOTS;

                        //
                        for (int x5 = 0; x5 < 8; x5++)
                        {
                            for (int y5 = 0; y5 < 8; y5++)
                            {
                                threadTable[x5, y5] = -1;
                            }
                        }
                        //
                        findMoveWhite(out computerMoves, out globalNumMoves, grid);
                        //

                        int temp = 0;

                        //create multiple threads here
                        for (int x2 = 0; x2 < globalNumMoves; x2++)
                        {
                            //wait untill the thread has been reached and que number updated unless its the first thread
                            while (x2 != 0 && temp == globalThreadqueNum)
                            {
                                Thread.Sleep(5);
                            }
                            temp = globalThreadqueNum;  //record the global variable 

                            //Create a thread for all possible compute moves for anticipating the player's next move 
                            TimerCallback timerDelegate3 = new TimerCallback(PlayerMoveThread);
                            AutoResetEvent autoResetEvent3 = new AutoResetEvent(false);
                            //int hash = autoResetEvent2.GetHashCode();
                            //threadGridsHashTable[0, x, 0] = hash;
                            threadGridsHashTable[x2, 0] = computerMoves[x2, 0];
                            threadGridsHashTable[x2, 1] = computerMoves[x2, 1];
                            threadTable[computerMoves[x2, 0], computerMoves[x2, 1]] = x2;  //used to keep track of indivisual threads
                            System.Threading.Timer stateTimer3 = new System.Threading.Timer(timerDelegate3, autoResetEvent3, 0, System.Threading.Timeout.Infinite);
                            threads[x2] = stateTimer3;

                            //autoResetEvent3.WaitOne();
                        }
                        THREADS_ARE_ACTIVE = true;
                    }
                }
                /*
                            if (Array.Equals(grid, oldGrid))
                            {
                                testingString = "->Debug string<-";
                            }
                */
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin();

            spriteBatch.Draw(Content.Load<Texture2D>("Grid"), new Vector2(0, 0), Color.White);

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (grid[x, y] == "X")
                    {
                        if (false && grid[x, y] != oldGrid[x, y])
                        {
                            spriteBatch.Draw(Content.Load<Texture2D>("White_Effect"), new Vector2(x * (512 / 8) + 1, y * (512 / 8) + 1), Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(Content.Load<Texture2D>("White"), new Vector2(x * (512 / 8) + 1, y * (512 / 8) + 1), Color.White);
                        }
                    }
                    else if (grid[x, y] == "Y")
                    {
                        if (false && grid[x, y] != oldGrid[x, y])
                        {
                            spriteBatch.Draw(Content.Load<Texture2D>("Black_Effect"), new Vector2(x * (512 / 8) + 1, y * (512 / 8) + 1), Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(Content.Load<Texture2D>("Black"), new Vector2(x * (512 / 8) + 1, y * (512 / 8) + 1), Color.White);
                        }
                    }
                }
            }

            if (ANIMATION_WHITE && GAME_START)
            {
                frameCounter++;

                if (frameCounter <= 110)
                {
                    Rectangle source = new Rectangle(frameCounter / 10 * 70, 0, 70, 70);

                    // Draw the current frame.
                    spriteBatch.Draw(Content.Load<Texture2D>("White_Animation"), new Vector2(700, 500), source, Color.White);
                }
                else
                {
                    ANIMATION_WHITE = false;
                    frameCounter = 0;
                }
            }

            if (CURSOR)
            {
                spriteBatch.Draw(Content.Load<Texture2D>("MousePointer"), new Vector2(debugX * (512 / 8) + 32, debugY * (512 / 8) + 32), Color.White);
                spriteBatch.Draw(Content.Load<Texture2D>("On"), new Vector2(530, 260), Color.White);
            }
            else
            {
                spriteBatch.Draw(Content.Load<Texture2D>("Off"), new Vector2(530, 260), Color.White);
            }

            //spriteBatch.Draw(Content.Load<Texture2D>("MousePointer"), new Vector2(mouseState.X, mouseState.Y), Color.SlateBlue);
            spriteFont = Content.Load<SpriteFont>("SpriteFont1");
            spriteBatch.DrawString(spriteFont, testingString, new Vector2(270, 570), Color.Black);
            spriteBatch.DrawString(spriteFont, winnerString, new Vector2(530, 200), Color.Black);
            spriteBatch.DrawString(spriteFont, computerString, new Vector2(100, 530), Color.Black);
            spriteBatch.DrawString(spriteFont, "Press 9 to enter advanced mode." + advancedString, new Vector2(100, 550), Color.Black);
            spriteBatch.DrawString(spriteFont, scoreString + "SCORE: " + "B:" + playerWins.ToString() + " W:" + computerWins.ToString(), new Vector2(530, 100), Color.Black);
            
            //
            if (ADVANCED_MODE)
            {
                spriteBatch.DrawString(spriteFont, "Total number of snapshots: ", new Vector2(530, 150), Color.Black);
                spriteBatch.DrawString(spriteFont, globalSnapShotCounter.ToString(), new Vector2(530, 170), Color.Black);
                //spriteBatch.DrawString(spriteFont, informationString + gridCount.ToString(), new Vector2(530, 50), Color.Black);

                if (MAXIMUM_SNAPSHOTS == false)
                {
                    spriteBatch.Draw(Content.Load<Texture2D>("MaximumButton"), new Vector2(530, 380), Color.White);
                }
                else
                {
                    spriteBatch.Draw(Content.Load<Texture2D>("MaximumButton_pressed"), new Vector2(530, 380), Color.White);
                }

                if (advancedSnapshots == NUM_SNAPSHOTS)
                {
                    spriteBatch.DrawString(spriteFont, informationString + oldMinGridCount.ToString(), new Vector2(530, 50), Color.Black);
                }
                else
                {
                    spriteBatch.DrawString(spriteFont, informationString + advancedSnapshots.ToString(), new Vector2(530, 50), Color.Black);
                }
            }
            else
            {
                spriteBatch.DrawString(spriteFont, informationString + gridCount.ToString(), new Vector2(530, 50), Color.Black);
            }

            //k            spriteBatch.Draw(Content.Load<Texture2D>("MousePointer"), new Vector2(debugX * (512 / 8) + 1, debugY * (512 / 8) + 1), Color.White);


            if (!GAME_START)
            {
                spriteBatch.DrawString(spriteFont, messageString, new Vector2(520, 230), Color.Black);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void CheckStatus(Object stateInfo)
        {
            //count += 100;
        }

        // This method is called by the timer delegate.
        public void PlayerMoveThread(Object stateInfo)
        {
            int temp = globalThreadqueNum;
            globalThreadqueNum++;  //advance the que for the next thread

            AutoResetEvent autoResetEvent3 = (AutoResetEvent)stateInfo;

            int gridCount = 0;

            Random random = new Random();

            string[,] playerGrid = new string[8, 8];

            playerGrid = (string[,])grid.Clone();

            makeWhiteMove(out playerGrid, playerGrid, threadGridsHashTable[temp, 0], threadGridsHashTable[temp, 1]);
            
            while (!THREADS_ARE_ACTIVE)
            {
                Thread.Sleep(2);
            }
            
            /******************************************************************
            * maximum number of times compute will try to win is NUM_SNAPSHOTS
            *******************************************************************/
            while ((MAXIMUM_SNAPSHOTS && BREAK_PRESSED) || (gridCount < NUM_SNAPSHOTS / 10 && !COMPUTER_THREAD_IS_EXITING && !THREADS_ARE_EXITING && (MAXIMUM_SNAPSHOTS || (count < ComputerMilliSeconds && !SECONDS_REACHED))))
            {
                bool computerWin = false;
                bool playerWin = false;
                bool COMPUTER_TURN = false;
                int win = 0;
                bool FIRST_MOVE = true;
                bool PLAYER_FIRST_MOVE = true;
                bool COMPUTER_SECOND_MOVE = true;
                int tempMoveCount = 0;
                bool FORCE_MOVE = false;
                int randX = -1;
                int randY = -1;
                int playerX = -1;
                int playerY = -1;
                int blackCount = 0;
                int whiteCount = 0;
                int[,] localWins = new int[8, 8];
                int[,] localLosses = new int[8, 8];


                string[,] tempGrid = new string[8, 8];

                tempGrid = (string[,])playerGrid.Clone();
                //tempMoveCount = moveCount2;

                while (win == 0 && !FORCE_MOVE)
                {
                    if (COMPUTER_TURN && !FORCE_MOVE)
                    {
                        if (checkWhiteNoMove(tempGrid))
                        {
                            if (false/*ADVANCED_MODE*/)
                            {
                                    do
                                    {
                                        randX = random.Next(8);
                                        randY = random.Next(8);

                                    } while (!checkWhiteMove(tempGrid, randX, randY));

                                    makeWhiteMove(out tempGrid, tempGrid, randX, randY);
                            }
                            else
                            {
                                if (moveCount >= 64)
                                {/*
                                    for (int x = 0; x < 8; x++)
                                    {
                                        for (int y = 0; y < 8; y++)
                                        {
                                            threadGrid2[temp, x, y] = tempGrid[x, y];
                                        }
                                    }

                                    //AutoResetEvent autoResetEvent2 = new AutoResetEvent(false);

                                    // Create the delegate that invokes methods for the timer.
                                    TimerCallback timerDelegate2 = new TimerCallback(PlayerMoveThreadWhite2);

                                    // Create a timer that signals the delegate to invoke 
                                    // CheckStatus after 0.5 seconds
                                    System.Threading.Timer stateTimer2 = new System.Threading.Timer(timerDelegate2, stateInfoArray[temp], 0, System.Threading.Timeout.Infinite);

                                    stateInfoArray[temp].WaitOne(System.Threading.Timeout.Infinite);
                                    stateTimer2.Dispose();*/

                                    Random randThreadWhite2 = new Random();
                                    int computerX2 = -1;
                                    int computerY2 = -1;
                                    int winX2 = -1;
                                    int winY2 = -1;
                                    int threadGridCount = 0;
                                    int win2 = 0;

                                    while (threadGridCount < 20)
                                    {
                                        computerWin = false;
                                        bool COMPUTER_TURN2 = true;
                                        string[,] tempGrid2 = new string[8, 8];
                                        win2 = 0;
                                        bool FIRST_MOVE2 = true;

                                        //replicate the base grid
                                        for (int x3 = 0; x3 < 8; x3++)
                                        {
                                            for (int y3 = 0; y3 < 8; y3++)
                                            {
                                                tempGrid2[x3, y3] = String.Copy(tempGrid[x3, y3]);
                                            }
                                        }
                                        
                                        int tempMoveCount2 = moveCount;

                                        while (win2 == 0)
                                        {
                                            if (COMPUTER_TURN2)
                                            {
                                                if (checkWhiteNoMove(tempGrid2))
                                                {
                                                    do
                                                    {
                                                        randX = randThreadWhite2.Next(8);
                                                        randY = randThreadWhite2.Next(8);

                                                    } while (!checkWhiteMove(tempGrid2, randX, randY));

                                                    makeWhiteMove(out tempGrid2, tempGrid2, randX, randY);

                                                    if (FIRST_MOVE2)
                                                    {
                                                        computerX2 = randX;
                                                        computerY2 = randY;
                                                        FIRST_MOVE2 = false;
                                                    }
                                                }
                                                else
                                                {
                                                    win2 = CheckForWinner(tempGrid2, out blackCount, out whiteCount);
                                                }

                                                COMPUTER_TURN2 = false;
                                                tempMoveCount++;
                                            }
                                            else
                                            {
                                                if (checkBlackNoMove(tempGrid2))
                                                {
                                                    do
                                                    {
                                                        randX = randThreadWhite2.Next(8);
                                                        randY = randThreadWhite2.Next(8);

                                                    } while (!checkBlackMove(tempGrid2, randX, randY));

                                                    makeBlackMove(out tempGrid2, tempGrid2, randX, randY);
                                                }
                                                else
                                                {
                                                    win2 = CheckForWinner(tempGrid2, out blackCount, out whiteCount);
                                                }

                                                tempMoveCount++;
                                                COMPUTER_TURN2 = true;
                                            }

                                            if (win2 == -1 && computerX2 != -1)
                                            {
                                                threadGridCount++;
                                                winX2 = computerX2;
                                                winY2 = computerY2;
                                                threadGridCount = 128;
                                                break;
                                            }
                                            else if (win2 == 1)
                                            {
                                                threadGridCount++;
                                            }
                                            else if (win2 == -2)
                                            {
                                                threadGridCount++;
                                            }
                                        }
                                    }

                                    if (winX2 != -1 && tempGrid[winX2, winY2] == "-")
                                    {
                                        //do nothing
                                    }
                                    else
                                    {
                                        do
                                        {
                                            randX = randThreadWhite2.Next(8);
                                            randY = randThreadWhite2.Next(8);

                                        } while (!checkWhiteMove(tempGrid, randX, randY));

                                        winX2 = randX;
                                        winY2 = randY;
                                    }

                                    makeWhiteMove(out tempGrid, tempGrid, winX2, winY2);
                                    //makeWhiteMove(out tempGrid, tempGrid, threadXarray[temp], threadYarray[temp]);
                                }
                                else
                                {

                                    do
                                    {
                                        randX = random.Next(8);
                                        randY = random.Next(8);

                                    } while (!checkWhiteMove(tempGrid, randX, randY));

                                    makeWhiteMove(out tempGrid, tempGrid, randX, randY);
                                }
                            }
                        }
                        else
                        {
                            win = CheckForWinner(tempGrid, out blackCount, out whiteCount);

                            //player has no move and is last move
                            if (playerX == -1)
                            {
                                playerX = 0;
                                playerY = 0;
                            }

                            if (win == -1)
                            {
                                computerWin = true;
                            }
                            else if (win == 1)
                            {
                                playerWin = true;
                            }

                            if (computerWin)
                            {
                                //localWins[threadGridsHashTable[temp, 0], threadGridsHashTable[temp, 1]]++;
                                globalPlayerWinGrid[gridCount, threadGridsHashTable[temp, 0], threadGridsHashTable[temp, 1]] = 2;
                                //globalPlayerWinGridSimple[temp, 0, playerX, playerY]++;
                                //playerWinGrid[0, threadGridsHashTable[temp, 0], threadGridsHashTable[temp, 1]] --;
                                //playerWinGrid[1, threadGridsHashTable[temp, 0], threadGridsHashTable[temp, 1]] = whiteCount - blackCount;
                                gridCount++;
                                globalThreadGridCount[temp, 0]++;
                                globalGridNums[temp, 0]++;
                                globalSnapShotCounter++;
                            }
                            else if (playerWin)
                            {
                                //localLosses[threadGridsHashTable[temp, 0], threadGridsHashTable[temp, 1]]++;
                                globalPlayerWinGrid[gridCount, threadGridsHashTable[temp, 0], threadGridsHashTable[temp, 1]] = 1;
                                //globalPlayerWinGridSimple[temp, 1, playerX, playerY]++;
                                //playerWinGrid[0, threadGridsHashTable[temp, 0], threadGridsHashTable[temp, 1]] ++;
                                //playerWinGrid[1, threadGridsHashTable[temp, 0], threadGridsHashTable[temp, 1]] = blackCount - whiteCount;
                                gridCount++;
                                globalThreadGridCount[temp, 0]++;
                                globalGridNums[temp, 0]++;
                                globalSnapShotCounter++;
                            }
                            else if (win == -2)  //draw
                            {
                                globalPlayerWinGrid[gridCount, threadGridsHashTable[temp, 0], threadGridsHashTable[temp, 1]] = 3;
                                playerWinGrid[0, threadGridsHashTable[temp, 0], threadGridsHashTable[temp, 1]] = -2;
                                gridCount++;
                                globalThreadGridCount[temp, 0]++;
                                globalGridNums[temp, 0]++;
                                globalSnapShotCounter++;
                            }
                        }

                        COMPUTER_TURN = false;
                        tempMoveCount++;
                    }
                    else
                    {
                        if (checkBlackNoMove(tempGrid))
                        {
                            if (false/*ADVANCED_MODE*/)
                            {

                            }
                            else
                            {
                                if (moveCount >= 64)
                                {/*
                                    for (int x = 0; x < 8; x++)
                                    {
                                        for (int y = 0; y < 8; y++)
                                        {
                                            threadGrid2[temp, x, y] = tempGrid[x, y];
                                        }
                                    }

                                    stateInfoArray[temp] = new AutoResetEvent(false);

                                    // Create the delegate that invokes methods for the timer.
                                    TimerCallback timerDelegate2 = new TimerCallback(PlayerMoveThreadBlack2);

                                    // Create a timer that signals the delegate to invoke 
                                    // CheckStatus after 0.5 seconds
                                    System.Threading.Timer stateTimer2 = new System.Threading.Timer(timerDelegate2, stateInfoArray[temp], 0, System.Threading.Timeout.Infinite);

                                    stateInfoArray[temp].WaitOne(System.Threading.Timeout.Infinite);
                                    stateTimer2.Dispose();
                                    
                                    makeBlackMove(out tempGrid, tempGrid, threadXarray[temp], threadYarray[temp]);

                                    if (FIRST_MOVE)
                                    {
                                        playerX = threadXarray[temp];
                                        playerY = threadYarray[temp];
                                        FIRST_MOVE = false;
                                    }
                                  * */
                                    Random randThreadBlack2 = new Random();
                                    int computerX2 = -1;
                                    int computerY2 = -1;
                                    int winX2 = -1;
                                    int winY2 = -1;
                                    int threadGridCount = 0;
                                    int win2 = 0;
                                    

                                    while (threadGridCount < 20)
                                    {
                                        computerWin = false;
                                        bool COMPUTER_TURN2 = true;
                                        string[,] tempGrid2 = new string[8, 8];
                                        win2 = 0;
                                        bool FIRST_MOVE2 = true;

                                        //replicate the base grid
                                        for (int x3 = 0; x3 < 8; x3++)
                                        {
                                            for (int y3 = 0; y3 < 8; y3++)
                                            {
                                                tempGrid2[x3, y3] = String.Copy(tempGrid[x3, y3]);
                                            }
                                        }

                                        while (win2 == 0)
                                        {
                                            if (COMPUTER_TURN2)
                                            {
                                                if (checkBlackNoMove(tempGrid2))
                                                {
                                                    do
                                                    {
                                                        randX = randThreadBlack2.Next(8);
                                                        randY = randThreadBlack2.Next(8);

                                                    } while (!checkBlackMove(tempGrid2, randX, randY));

                                                    makeBlackMove(out tempGrid2, tempGrid2, randX, randY);

                                                    if (FIRST_MOVE2)
                                                    {
                                                        computerX2 = randX;
                                                        computerY2 = randY;
                                                        FIRST_MOVE2 = false;
                                                    }
                                                }
                                                else
                                                {
                                                    win2 = CheckForWinner(tempGrid2, out blackCount, out whiteCount);
                                                }

                                                COMPUTER_TURN2 = false;
                                                tempMoveCount++;
                                            }
                                            else
                                            {
                                                if (checkWhiteNoMove(tempGrid2))
                                                {
                                                    do
                                                    {
                                                        randX = randThreadBlack2.Next(8);
                                                        randY = randThreadBlack2.Next(8);

                                                    } while (!checkWhiteMove(tempGrid2, randX, randY));

                                                    makeWhiteMove(out tempGrid2, tempGrid2, randX, randY);
                                                }
                                                else
                                                {
                                                    win2 = CheckForWinner(tempGrid2, out blackCount, out whiteCount);
                                                }

                                                tempMoveCount++;
                                                COMPUTER_TURN2 = true;
                                            }

                                            //int noOut = 0;

                                            if (win2 == 1 && computerX2 != -1)
                                            {
                                                threadGridCount++;
                                                winX2 = computerX2;
                                                winY2 = computerY2;
                                                threadGridCount = 128;
                                                break;
                                            }
                                            else if (win2 == -1)
                                            {
                                                threadGridCount++;
                                            }
                                            else if (win2 == -2)
                                            {
                                                threadGridCount++;
                                            }
                                        }
                                    }

                                    if (winX2 != -1 && tempGrid[winX2, winY2] == "-")
                                    {
                                        //do nothing
                                    }
                                    else
                                    {
                                        do
                                        {
                                            randX = randThreadBlack2.Next(8);
                                            randY = randThreadBlack2.Next(8);

                                        } while (!checkBlackMove(tempGrid, randX, randY));

                                        winX2 = randX;
                                        winY2 = randY;
                                    }

                                    makeBlackMove(out tempGrid, tempGrid, winX2, winY2);

                                    if (FIRST_MOVE)
                                    {
                                        playerX = winX2;
                                        playerY = winY2;
                                        FIRST_MOVE = false;
                                    }
                                }
                                else
                                {
                                    do
                                    {
                                        randX = random.Next(8);
                                        randY = random.Next(8);

                                    } while (!checkBlackMove(tempGrid, randX, randY));

                                    makeBlackMove(out tempGrid, tempGrid, randX, randY);

                                    if (FIRST_MOVE)
                                    {
                                        playerX = randX;
                                        playerY = randY;
                                        FIRST_MOVE = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            win = CheckForWinner(tempGrid, out blackCount, out whiteCount);

                            //player has no move and is last move
                            if (playerX == -1)
                            {
                                playerX = 0;
                                playerY = 0;
                            }

                            if (win == -1)
                            {
                                computerWin = true;
                            }
                            else if (win == 1)
                            {
                                playerWin = true;
                            }

                            if (computerWin)
                            {
                                //localWins[threadGridsHashTable[temp, 0], threadGridsHashTable[temp, 1]]++;
                                globalPlayerWinGrid[gridCount, threadGridsHashTable[temp, 0], threadGridsHashTable[temp, 1]] = 2;
                                //globalPlayerWinGridSimple[temp, 0, playerX, playerY]++;
                                //globalPieceCount[playerX, playerY] += whiteCount - blackCount;
                                playerWinGrid[0, threadGridsHashTable[temp, 0], threadGridsHashTable[temp, 1]] ++;
                                //playerWinGrid[1, threadGridsHashTable[temp, 0], threadGridsHashTable[temp, 1]] = whiteCount - blackCount;
                                gridCount++;
                                globalThreadGridCount[temp, 0]++;
                                globalGridNums[temp, 0]++;
                                globalSnapShotCounter++;
                            }
                            else if (playerWin)
                            {
                                //localLosses[threadGridsHashTable[temp, 0], threadGridsHashTable[temp, 1]]++;
                                globalPlayerWinGrid[gridCount, threadGridsHashTable[temp, 0], threadGridsHashTable[temp, 1]] = 1;
                                //globalPlayerWinGridSimple[temp, 1, playerX, playerY]++;
                                //globalPieceCount[playerX, playerY] -= whiteCount - blackCount;
                                playerWinGrid[0, threadGridsHashTable[temp, 0], threadGridsHashTable[temp, 1]] --;
                                //playerWinGrid[1, threadGridsHashTable[temp, 0], threadGridsHashTable[temp, 1]] = blackCount - whiteCount;
                                gridCount++;
                                globalThreadGridCount[temp, 0]++;
                                globalGridNums[temp, 0]++;
                                globalSnapShotCounter++;
                            }
                            else if (win == -2)  //draw
                            {
                                globalPlayerWinGrid[gridCount, threadGridsHashTable[temp, 0], threadGridsHashTable[temp, 1]] = 3;
                                //playerWinGrid[0, threadGridsHashTable[temp, 0], threadGridsHashTable[temp, 1]] = -2;
                                gridCount++;
                                globalThreadGridCount[temp, 0]++;
                                globalGridNums[temp, 0]++;
                                globalSnapShotCounter++;
                            }
                        }

                        tempMoveCount++;
                        COMPUTER_TURN = true;
                    }
                }
                /*
                if ((double)((double)gridCount / (double)5000) == gridCount / 5000)
                {
                    Thread.Sleep(10);
                }*/

                //Thread.SpinWait(50);

                if (((double)((double)gridCount / (double)5000) == gridCount / 5000))
                {
                    int sum = 0;

                    do
                    {
                        sum = 0;

                        for(int x = 0; x <= globalNumMoves - 1; x++)
                        {
                            sum += globalThreadGridCount[x, 0];
                        }

                        Thread.Sleep(globalNumMoves * moveCount + 20);

                        if (!MAXIMUM_SNAPSHOTS && (SECONDS_REACHED || count >= ComputerMilliSeconds || count2 >= ComputerMilliSeconds || count3 >= ComputerMilliSeconds || count4 >= ComputerMilliSeconds || count5 >= ComputerMilliSeconds))
                        {
                            THREADS_ARE_EXITING = true;

                            autoResetEvent3.Set();

                            return;
                        }
                        else
                        {

                        }

                    }while(gridCount * globalNumMoves != sum);  //pause until all threads are syncronized
                }
            }
            /*
            if (minGridCount > gridCount)
            {
                minGridCount = gridCount;
            }
*/
            THREADS_ARE_EXITING = true;

            autoResetEvent3.Set();
        }

        public void ComputerMoveThread(Object stateInfo)
        {
            int blackCount = 0;
            int whiteCount = 0;
            //count = 0;

            AutoResetEvent autoResetEvent = (AutoResetEvent)stateInfo;

            if (!checkWhiteNoMove(grid))
            {
                PLAYER_TURN = true;
                ANIMATION_WHITE = true;
                THREAD_IS_ACTIVE = false;
                autoResetEvent.Set();
                return;
            }

            if (!PLAYER_TURN && GAME_START)
            {
                //PLAYER_TURN = true;
/*
                if (ADVANCED_MODE)
                {
                    while (count < ComputerMilliSeconds)
                    {
                        Thread.Sleep(100);
                    }

                    for (int x = 0; x < 32; x++)
                    {
                        threads[x].Dispose();
                    }
                }
*/
                int randX = 0;
                int randY = 0;
                int randX2 = 0;
                int randY2 = 0;

                Random randComputerThread = new Random();

                bool computerWin = false;
                bool playerWin = false;
                /*
                                if (!checkWhiteNoMove(grid))
                                {
                                    PLAYER_TURN = true;
                                    return;
                                }
                */
                /*
                int[,] computerMoves = new int[32, 2];
                int numMoves = 0;
                //threadGridsHashTable.Initialize();  //erace all past usage
                threadGridsHashTable = new int[2, 32, 2];
                globalThreadqueNum = 0; //reset the que number

                findMoveWhite(out computerMoves, out numMoves, grid);

                for (int x = 0; x < 1; x++)
                {
                    //Create a thread for all possible compute moves for anticipating the player's next move 
                    TimerCallback timerDelegate2 = new TimerCallback(PlayerMoveThread);
                    AutoResetEvent autoResetEvent2 = new AutoResetEvent(false);
                    //int hash = autoResetEvent2.GetHashCode();
                    //threadGridsHashTable[0, x, 0] = hash;
                    threadGridsHashTable[1, x, 0] = computerMoves[x, 0];
                    threadGridsHashTable[1, x, 1] = computerMoves[x, 1];
                    Timer stateTimer2 = new Timer(timerDelegate2, null, 0, System.Threading.Timeout.Infinite);
                    //autoResetEvent2.WaitOne();
                }
                */
                int tempMoveCount = moveCount;
                bool COMPUTER_TURN = true;
                int[, , ,] computerWinGrid = new int[2, NUM_SNAPSHOTS + 1, 8, 8];
                int computerX = -1;
                int computerY = -1;
                int playerX = 0;
                int playerY = 0;
                int forceX = -1;
                int forceY = -1;
                bool IS_FAULTY_MOVE = false;
                bool FORCE_MOVE = false;


                gridCount = 0;  //global variable

                /*
                int currentMove = 0;
                int[,] moves = new int[32, 2];
                int numMoves = 0;
                int rand = 0;

                findMoveWhite(out moves, out numMoves, grid);
                 */

                /******************************************************************
                 * maximum number of times compute will try to win is NUM_SNAPSHOTS
                 * ****************************************************************/
                while (count < ComputerMilliSeconds && gridCount < NUM_SNAPSHOTS / 10 && GAME_START && !FORCE_MOVE && !SECONDS_REACHED && !ADVANCED_MODE)
                {
                    computerWin = false;
                    playerWin = false;
                    COMPUTER_TURN = true;
                    int win = 0;
                    bool FIRST_MOVE = true;
                    bool PLAYER_FIRST_MOVE = true;
                    bool COMPUTER_SECOND_MOVE = true;
                    int moveX = 0, moveY = 0;
                    int currentWins = -1 * NUM_SNAPSHOTS;
                    int currentLosses = NUM_SNAPSHOTS;
                    computerX = -1;
                    computerY = -1;

                    tempGrid = (string[,])grid.Clone();
                    tempMoveCount = moveCount;

                    while (win == 0 && !FORCE_MOVE)
                    {
                        if (COMPUTER_TURN && !FORCE_MOVE)
                        {
                            if (checkWhiteNoMove(tempGrid))
                            {
                                if (ADVANCED_MODE)
                                {
                                    if (false/*moveCount > 50*/)
                                    {
                                        threadX = -1;
                                        threadY = -1;
                                        threadGrid = (string[,])tempGrid.Clone();

                                        AutoResetEvent autoResetEvent2 = new AutoResetEvent(false);

                                        // Create the delegate that invokes methods for the timer.
                                        TimerCallback timerDelegate2 = new TimerCallback(PlayerMoveThreadWhite);

                                        // Create a timer that signals the delegate to invoke 
                                        // CheckStatus after 0.5 seconds
                                        System.Threading.Timer stateTimer2 = new System.Threading.Timer(timerDelegate2, autoResetEvent2, 0, System.Threading.Timeout.Infinite);

                                        autoResetEvent2.WaitOne(System.Threading.Timeout.Infinite);
                                        stateTimer2.Dispose();

                                        makeWhiteMove(out tempGrid, tempGrid, threadX, threadY);

                                        if (FIRST_MOVE)
                                        {
                                            computerX = threadX;
                                            computerY = threadY;
                                            FIRST_MOVE = false;
                                        }
                                    }
                                    else
                                    {
                                        if (FIRST_MOVE)
                                        {
                                            randX = -1; //initialize
                                            int currentComputerWin = 0;
                                            int xTemp = 0, yTemp = 0;
                                            moveX = -1; //initialize
                                            int tempThreadNum = 0;

                                            for (xTemp = 0; xTemp < 8; xTemp++)
                                            {
                                                for (yTemp = 0; yTemp < 8; yTemp++)
                                                {
                                                    if (playerWinGrid[0, xTemp, yTemp] < currentComputerWin)
                                                    {
                                                        moveX = xTemp;
                                                        moveY = yTemp;
                                                        currentComputerWin = playerWinGrid[0, xTemp, yTemp];
                                                    }
                                                }
                                            }

                                            /*
                                            for (int x2 = 0; x2 < 8; x2++)
                                            {
                                                for (int y2 = 0; y2 < 8; y2++)
                                                {
                                                     
                                                }
                                            }
                                            */

                                            if (globalNumMoves == 1)
                                            {
                                                tempThreadNum = 0;
                                            }

                                            randX = moveX;
                                            randY = moveY;
                                        }
                                        else
                                        {
                                            do
                                            {
                                                randX = randComputerThread.Next(8);
                                                randY = randComputerThread.Next(8);

                                            } while (!checkWhiteMove(tempGrid, randX, randY));
                                        }

                                        if (randX == -1)
                                        {
                                            do
                                            {
                                                randX = randComputerThread.Next(8);
                                                randY = randComputerThread.Next(8);

                                            } while (!checkWhiteMove(tempGrid, randX, randY));
                                        }

                                        makeWhiteMove(out tempGrid, tempGrid, randX, randY);

                                        if (FIRST_MOVE)
                                        {
                                            computerX = randX;
                                            computerY = randY;
                                            FIRST_MOVE = false;
                                        }
                                    }
                                }
                                else
                                {
                                    do
                                    {
                                        randX = randComputerThread.Next(8);
                                        randY = randComputerThread.Next(8);

                                    } while (!checkWhiteMove(tempGrid, randX, randY));

                                    makeWhiteMove(out tempGrid, tempGrid, randX, randY);

                                    if (FIRST_MOVE)
                                    {
                                        computerX = randX;
                                        computerY = randY;
                                        FIRST_MOVE = false;
                                    }
                                }
                            }
                            else
                            {
                                win = CheckForWinner(tempGrid, out blackCount, out whiteCount);

                                if (win == -1)
                                {
                                    computerWin = true;
                                }
                                else if (win == 1)
                                {
                                    playerWin = true;
                                }

                                if (computerWin && computerX != -1)
                                {
                                    computerWinGrid[0, (int)gridCount, computerX, computerY] = -1;
                                    computerWinGrid[1, (int)gridCount, computerX, computerY] = whiteCount - blackCount;
                                    gridCount++;
                                    globalSnapShotCounter++;
                                }
                                else if (playerWin && computerX != -1)
                                {
                                    computerWinGrid[0, (int)gridCount, computerX, computerY] = 1;
                                    computerWinGrid[1, (int)gridCount, computerX, computerY] = blackCount - whiteCount;
                                    gridCount++;
                                    globalSnapShotCounter++;
                                }
                                else if (win == -2 && computerX != -1)  //draw
                                {
                                    computerWinGrid[0, (int)gridCount, computerX, computerY] = -2;
                                    gridCount++;
                                    globalSnapShotCounter++;
                                }
                            }

                            COMPUTER_TURN = false;
                            tempMoveCount++;
                        }
                        else
                        {
                            if (checkBlackNoMove(tempGrid))
                            {
                                if (ADVANCED_MODE)
                                {
                                    if (false/*moveCount > 50*/)
                                    {
                                        threadX = -1;
                                        threadY = -1;
                                        threadGrid = (string[,])tempGrid.Clone();

                                        AutoResetEvent autoResetEvent2 = new AutoResetEvent(false);

                                        // Create the delegate that invokes methods for the timer.
                                        TimerCallback timerDelegate2 = new TimerCallback(PlayerMoveThreadBlack);

                                        // Create a timer that signals the delegate to invoke 
                                        // CheckStatus after 0.5 seconds
                                        System.Threading.Timer stateTimer2 = new System.Threading.Timer(timerDelegate2, autoResetEvent2, 0, System.Threading.Timeout.Infinite);

                                        autoResetEvent2.WaitOne(System.Threading.Timeout.Infinite);
                                        stateTimer2.Dispose();

                                        makeBlackMove(out tempGrid, tempGrid, threadX, threadY);
                                    }
                                    else
                                    {
                                        if (false/*tempMoveCount - moveCount == 1*/)
                                        {
                                            randX = -1; //initialize
                                            int currentPlayerWin = NUM_SNAPSHOTS;
                                            int xTemp = 0, yTemp = 0;
                                            moveX = -1; //initialize

                                            for (xTemp = 0; xTemp < 8; xTemp++)
                                            {
                                                for (yTemp = 0; yTemp < 8; yTemp++)
                                                {
                                                    if (playerWinGrid[0, xTemp, yTemp] < currentPlayerWin)
                                                    {
                                                        moveX = xTemp;
                                                        moveY = yTemp;
                                                        currentPlayerWin = playerWinGrid[0, xTemp, yTemp];
                                                    }
                                                }
                                            }

                                            randX = moveX;
                                            randY = moveY;
                                        }
                                        else
                                        {
                                            do
                                            {
                                                randX = randComputerThread.Next(8);
                                                randY = randComputerThread.Next(8);

                                            } while (!checkBlackMove(tempGrid, randX, randY));
                                        }

                                        //make sure next move prediction has found a solution
                                        if (randX == -1)
                                        {
                                            do
                                            {
                                                randX = randComputerThread.Next(8);
                                                randY = randComputerThread.Next(8);

                                            } while (!checkBlackMove(tempGrid, randX, randY));
                                        }

                                        makeBlackMove(out tempGrid, tempGrid, randX, randY);
                                    }
                                }
                                else
                                {
                                    do
                                    {
                                        randX = randComputerThread.Next(8);
                                        randY = randComputerThread.Next(8);

                                    } while (!checkBlackMove(tempGrid, randX, randY));

                                    makeBlackMove(out tempGrid, tempGrid, randX, randY);
                                }
                            }
                            else
                            {
                                win = CheckForWinner(tempGrid, out blackCount, out whiteCount);

                                if (win == -1)
                                {
                                    computerWin = true;
                                }
                                else if (win == 1)
                                {
                                    playerWin = true;
                                }

                                if (computerWin && computerX != -1)
                                {
                                    computerWinGrid[0, (int)gridCount, computerX, computerY] = -1;
                                    computerWinGrid[1, (int)gridCount, computerX, computerY] = whiteCount - blackCount;
                                    gridCount++;
                                    globalSnapShotCounter++;
                                }
                                else if (playerWin && computerX != -1)
                                {
                                    computerWinGrid[0, (int)gridCount, computerX, computerY] = 1;
                                    computerWinGrid[1, (int)gridCount, computerX, computerY] = blackCount - whiteCount;
                                    gridCount++;
                                    globalSnapShotCounter++;
                                }
                                else if (win == -2 && computerX != -1)  //draw
                                {
                                    computerWinGrid[0, (int)gridCount, computerX, computerY] = -2;
                                    gridCount++;
                                    globalSnapShotCounter++;
                                }
                            }

                            tempMoveCount++;
                            COMPUTER_TURN = true;
                        }

                        if (SECONDS_REACHED)
                        {
                            break;
                        }
                    }
                }

                if (ADVANCED_MODE)
                {
                    while ((MAXIMUM_SNAPSHOTS && !THREADS_ARE_EXITING) || (count < ComputerMilliSeconds && !SECONDS_REACHED))
                    {
                        Thread.Sleep(100);
                    }
                    /*
                    for (int x = 0; x < 32; x++)
                    {
                        if (threads[x] != null)
                        {
                            threads[x].Dispose();
                        }
                    }
                     * */
                }

                COMPUTER_THREAD_IS_EXITING = true;

                int winX = -1;
                int winY = -1;
                int loseX = -1;
                int loseY = -1;
                int safeX = -1;
                int safeY = -1;
                int drawX = -1;
                int drawY = -1;
                int averageX = -1;
                int averageY = -1;
                int winIndex = 0;
                int loseIndex = 0;
                int drawIndex = 0;
                int temp = 0;
                int temp2 = 0;
                int temp3 = 0;
                double[,] winMoves = new double[8, 8];
                double[,] loseMoves = new double[8, 8];
                int currentWinMoves = 0;
                int currentLoseMoves = NUM_SNAPSHOTS;
                int currentDrawMoves = 0;
                int currentMostLoseMoves = -1 * NUM_SNAPSHOTS;
                int currentWinRangeMoves = 0;
                int currentLoseRangeMoves = NUM_SNAPSHOTS;
                int[,] winCount = new int[8, 8];
                int[, ,] winCountMoves = new int[65, 8, 8];
                int[,] loseCount = new int[8, 8];
                int[, ,] loseCountMoves = new int[65, 8, 8];
                int[,] drawCount = new int[8, 8];
                int[, ,] drawCountMoves = new int[65, 8, 8];
                int[,] safeCount = new int[8, 8];
                double[,] averageGrid = new double[8, 8];
                int[, ,] averageCounter = new int[65, 8, 8];
                int[,] globalComputerWinGrid = new int[8, 8];   //compute thread moves
                int[,] globalComputerLoseGrid = new int[8, 8];  //compute thread moves
                int currentGlobalWinMoves = 0;
                int currentGlobalLoseMoves = NUM_SNAPSHOTS; //lesser the better
                int[,] move = new int[8, 8];
                int[,] advanced = new int[8, 8];
                int[,] range = new int[8, 8];
                int currentAdvanced = 0;
                int advancedX = -1;
                int advancedY = -1;
                int advanced2WinX = -1;
                int advanced2WinY = -1;
                int advanced2LoseX = -1;
                int advanced2LoseY = -1;
                int currentAverage = 0;
                int average2X = -1;
                int average2Y = -1;
                int threadPieceCountX = -1;
                int threadPieceCountY = -1;
                int currentPieceCount = int.MinValue;
                int rangeX = -1;
                int rangeY = -1;
                double numberOfWins = 0;
                //int numberOfWinMoves = NUM_SNAPSHOTS;  //lesser the better
                double numberOfLoses = 0;
                double numberOfDraws = 0;
                double numberOfLeasedLoses = NUM_SNAPSHOTS; //lesser the better
                double average = -1 * NUM_SNAPSHOTS;
                int currentRange = -1 * NUM_SNAPSHOTS;
                int currentThreadGridCount = NUM_SNAPSHOTS;
                int[, ,] threadWins = new int[2, 8, 8];

                int[, ,] predictionWins = new int[32, 8, 8];

                int averageThreadGridCount = 0;

                bool NO_MOVE = false;
                bool HAS_CLOSE_WINS = false;
                int advanced3X = -1;
                int advanced3Y = -1;
                int currentWinMoves2 = 0;
                double average2 = -1 * NUM_SNAPSHOTS;


                if (ADVANCED_MODE)
                {
                    int minGrid = NUM_SNAPSHOTS;

                    for (int x = 0; x < 32; x++)
                    {
                        if (globalGridNums[x, 0] < minGrid && globalGridNums[x, 0] != 0)
                        {
                            minGrid = globalGridNums[x, 0];
                        }
                    }

                    advancedSnapshots = minGrid;

                    for (int z4 = 0; z4 <= advancedSnapshots; z4++)
                    {
                        for (int x4 = 0; x4 < 8; x4++)
                        {
                            for (int y4 = 0; y4 < 8; y4++)
                            {
                                if (globalPlayerWinGrid[z4, x4, y4] == 2)
                                {
                                    winCountMoves[0, x4, y4] ++;
                                }
                                else if (globalPlayerWinGrid[z4, x4, y4] == 1)
                                {
                                    loseCountMoves[0, x4, y4] ++;
                                }
                                if (globalPlayerWinGrid[z4, x4, y4] == 3)
                                {
                                    drawCountMoves[0, x4, y4] ++;
                                }
                            }
                        }
                    }
                }
                else
                {
                    advancedSnapshots = gridCount;

                    for (int y4 = 0; y4 < (int)gridCount; y4++)
                    {
                        for (int x2 = 0; x2 < 8; x2++)
                        {
                            for (int y2 = 0; y2 < 8; y2++)
                            {
                                if (computerWinGrid[0, y4, x2, y2] == -1)
                                {
                                    winCountMoves[0, x2, y2]++;//+= computerWinGrid[1, y4, x2, y2];
                                    //winCountMoves[1, x2, y2] += computerWinGrid[1, y4, x2, y2];
                                }
                                if (computerWinGrid[0, y4, x2, y2] == 1)
                                {
                                    loseCountMoves[0, x2, y2]++;//+= computerWinGrid[1, y4, x2, y2];
                                    //loseCountMoves[1, x2, y2] += computerWinGrid[1, y4, x2, y2];
                                }
                                if (computerWinGrid[0, y4, x2, y2] == -2)
                                {
                                    drawCountMoves[0, x2, y2]++;
                                    //drawCountMoves[1, x2, y2] += computerWinGrid[1, y4, x2, y2];
                                }
                            }
                        }
                    }
                }
                
                /*
                for (int x4 = 0; x4 < 8; x4++)
                {
                    for (int y4 = 0; y4 < 8; y4++)
                    {
                        if (globalPieceCount[x4, y4] > currentPieceCount)
                        {
                            currentPieceCount = (int)globalPieceCount[x4, y4];
                            threadPieceCountX = x4;
                            threadPieceCountY = y4;
                        }
                    }
                }
                */

                //                for (int x4 = 0; x4 < 9; x4++)
                //                {
                for (int x2 = 0; x2 < 8; x2++)
                {
                    for (int y2 = 0; y2 < 8; y2++)
                    {
                        if (currentWinMoves <= winCountMoves[0, x2, y2] && winCountMoves[0, x2, y2] != 0 && grid[x2, y2] == "-" && checkWhiteMove(grid, x2, y2))
                        {
                            currentWinMoves = winCountMoves[0, x2, y2];
                            winX = x2;
                            winY = y2;
                        }

                        if (currentLoseMoves > loseCountMoves[0, x2, y2] && grid[x2, y2] == "-" && checkWhiteMove(grid, x2, y2))
                        {
                            currentLoseMoves = loseCountMoves[0, x2, y2];
                            safeX = x2;
                            safeY = y2;
                        }

                        if (currentMostLoseMoves <= loseCountMoves[0, x2, y2] && grid[x2, y2] == "-" && loseCountMoves[0, x2, y2] != 0 && checkWhiteMove(grid, x2, y2))
                        {
                            currentMostLoseMoves = loseCountMoves[0, x2, y2];
                            loseX = x2;
                            loseY = y2;
                        }

                        if (currentDrawMoves <= drawCountMoves[0, x2, y2] && grid[x2, y2] == "-" && drawCountMoves[0, x2, y2] != 0 && checkWhiteMove(grid, x2, y2))
                        {
                            currentDrawMoves = drawCountMoves[0, x2, y2];
                            drawX = x2;
                            drawY = y2;
                        }

                        if (grid[x2, y2] == "-"/* && loseCountMoves[0, x2, y2] != 0/* && threadWins[1, x2, y2] != 0*/&& checkWhiteMove(grid, x2, y2))
                        {
                            //move[x2, y2] = drawCountMoves[0, x2, y2];
                            averageGrid[x2, y2] = (double)(winCountMoves[0, x2, y2] - loseCountMoves[0, x2, y2]) /* * (double)(threadWins[0, x2, y2] / threadWins[1, x2, y2])*/;
                        }

                        if (loseCountMoves[0, x2, y2] == 0 && winCountMoves[0, x2, y2] != 0 && checkWhiteMove(grid, x2, y2))
                        {
                            advanced[x2, y2]++;
                        }

                        /*
                        if (ADVANCED_MODE && currentGlobalWinMoves <= playerWinGrid[0, x2, y2] && grid[x2, y2] == "-")
                        {
                            currentGlobalWinMoves = playerWinGrid[0, x2, y2];
                            advanced2WinX = x2;
                            advanced2WinY = y2;
                        }

                        if (ADVANCED_MODE && currentGlobalLoseMoves > playerWinGrid[0, x2, y2] && grid[x2, y2] == "-")
                        {
                            currentGlobalLoseMoves = playerWinGrid[0, x2, y2];
                            advanced2LoseX = x2;
                            advanced2LoseY = y2;
                        }
                         * */
                    }
                }
                //                }

                for (int x2 = 0; x2 < 8; x2++)
                {
                    for (int y2 = 0; y2 < 8; y2++)
                    {
                        //                        if (currentMostLoseMoves < move[x2, y2] && grid[x2, y2] == "-")
                        //                        {
                        //                            currentMostLoseMoves = move[x2, y2];
                        //                            winX = x2;
                        //                            winY = y2;
                        //                        }

                        if (currentAdvanced <= advanced[x2, y2] && grid[x2, y2] == "-" && advanced[x2, y2] != 0 && checkWhiteMove(grid, x2, y2))
                        {
                            currentAdvanced = advanced[x2, y2];
                            advancedX = x2;
                            advancedY = y2;
                        }

                        if (average <= averageGrid[x2, y2] && grid[x2, y2] == "-" && checkWhiteMove(grid, x2, y2))
                        {
                            average = averageGrid[x2, y2];
                            averageX = x2;
                            averageY = y2;
                        }
                        /*
                        if (ADVANCED_MODE && currentAverage > globalComputerWinGrid[x2, y2] - globalComputerLoseGrid[x2, y2])
                        {
                            currentAverage = globalComputerWinGrid[x2, y2] - globalComputerLoseGrid[x2, y2];
                            average2X = x2;
                            average2Y = y2;
                        }
                         */
                    }
                }

                for (int x2 = 0; x2 < 8; x2++)
                {
                    for (int y2 = 0; y2 < 8; y2++)
                    {
                        if (currentWinMoves2 <= winCountMoves[0, x2, y2] && winCountMoves[0, x2, y2] != 0 && grid[x2, y2] == "-" && checkWhiteMove(grid, x2, y2))
                        {
                            currentWinMoves2 = winCountMoves[0, x2, y2];

                            if (average2 <= averageGrid[x2, y2] && grid[x2, y2] == "-" && checkWhiteMove(grid, x2, y2))
                            {
                                average2 = averageGrid[x2, y2];

                                if (x2 != 0)
                                {
                                    advanced3X = x2;
                                    advanced3Y = y2;
                                }
                            }
                        }
                    }
                }


                //check for wins that are close in numbers
                for (int x2 = 0; x2 < 8; x2++)
                {
                    for (int y2 = 0; y2 < 8; y2++)
                    {
                        if (Math.Abs(currentWinMoves - winCountMoves[0, x2, y2]) < currentWinMoves * 0.1)
                        {
                            HAS_CLOSE_WINS = true;
                        }
                    }
                }

                //count = 0;


                //->make move here<-
                if (ADVANCED_MODE)
                {
                    if (checkWhiteNoMove(grid))
                    {
                        //replicate the base grid
                        for (int x3 = 0; x3 < 8; x3++)
                        {
                            for (int y3 = 0; y3 < 8; y3++)
                            {
                                oldGrid[x3, y3] = String.Copy(grid[x3, y3]);
                            }
                        }
                        /*
                        bool FOUND_NO_MOVE = false;
                        int[,] moves2 = new int[32, 2];
                        int numMoves2 = 0;
                        string[,] tempGrid2 = new string[8, 8];

                        findMoveWhite(out moves2, out numMoves2, tempGrid);

                        tempGrid2 = (string[,])tempGrid.Clone();

                        for (int x = 0; x < numMoves2; x++)
                        {
                            makeWhiteMove(out tempGrid2, tempGrid2, moves2[x, 0], moves2[x, 1]);

                            if (checkBlackNoMove(tempGrid2))
                            {
                                makeBlackMove(out tempGrid, tempGrid, moves2[x, 0], moves2[x, 1]);
                                FOUND_NO_MOVE = true;
                                break;
                            }
                        }
                        */

                        if (false/*FOUND_NO_MOVE*/)
                        {
                            //
                        }
                        else if (advancedX != -1 && grid[advancedX, advancedY] == "-" && checkWhiteMove(grid, advancedX, advancedY))
                        {
                            makeWhiteMove(out grid, grid, advancedX, advancedY);
                            moveCount++;
                            debugX = advancedX;
                            debugY = advancedY;
                            PLAYER_TURN = true;
                        }
                        else if (winX == -1 && advanced2WinX == -1 && drawX != -1 && grid[drawX, drawY] == "-" && checkWhiteMove(grid, drawX, drawY))
                        {
                            makeWhiteMove(out grid, grid, drawX, drawY);
                            moveCount++;
                            debugX = drawX;
                            debugY = drawY;
                            PLAYER_TURN = true;
                        }
                        else if (moveCount >= 0 && winX != -1 && grid[winX, winY] == "-" && checkWhiteMove(grid, winX, winY))
                        {
                            makeWhiteMove(out grid, grid, winX, winY);
                            moveCount++;
                            debugX = winX;
                            debugY = winY;
                            PLAYER_TURN = true;
                        }
                        else if (averageX != -1 && grid[averageX, averageY] == "-" && checkWhiteMove(grid, averageX, averageY))
                        {
                            makeWhiteMove(out grid, grid, averageX, averageY);
                            moveCount++;
                            debugX = averageX;
                            debugY = averageY;
                            PLAYER_TURN = true;
                        }
                        else if (advanced3X != -1 && grid[advanced3X, advanced3Y] == "-" && checkWhiteMove(grid, advanced3X, advanced3Y))
                        {
                            makeWhiteMove(out grid, grid, advanced3X, advanced3Y);
                            moveCount++;
                            debugX = advanced3X;
                            debugY = advanced3Y;
                            PLAYER_TURN = true;
                        }
                        else if (safeX != -1 && grid[safeX, safeY] == "-" && checkWhiteMove(grid, safeX, safeY))
                        {
                            makeWhiteMove(out grid, grid, safeX, safeY);
                            moveCount++;
                            debugX = safeX;
                            debugY = safeY;
                            PLAYER_TURN = true;
                        }
                        else if (winX != -1 && grid[winX, winY] == "-" && checkWhiteMove(grid, winX, winY))
                        {
                            makeWhiteMove(out grid, grid, winX, winY);
                            moveCount++;
                            debugX = winX;
                            debugY = winY;
                            PLAYER_TURN = true;
                        }
                        else
                        {
                            for (int x2 = 0; x2 < 8; x2++)
                            {
                                for (int y2 = 0; y2 < 8; y2++)
                                {
                                    if (grid[x2, y2] == "-")
                                    {
                                        if (checkWhiteMove(grid, x2, y2))
                                        {
                                            makeWhiteMove(out grid, grid, x2, y2);
                                            moveCount++;
                                            debugX = x2;
                                            debugY = y2;
                                            PLAYER_TURN = true;
                                            x2 = 8;
                                            y2 = 8;
                                        }
                                    }
                                }
                            }
                        }

                        if (!checkBlackNoMove(grid))  //player (black) has no moves after the move just made; skip turn and make a compute move again
                        {
                            PLAYER_TURN = false;
                            ANIMATION_WHITE = true;
                        }
                    }
                }
                else
                {
                    if (checkWhiteNoMove(grid))
                    {
                        //replicate the base grid
                        for (int x3 = 0; x3 < 8; x3++)
                        {
                            for (int y3 = 0; y3 < 8; y3++)
                            {
                                oldGrid[x3, y3] = String.Copy(grid[x3, y3]);
                            }
                        }

                        if (advancedX != -1 && grid[advancedX, advancedY] == "-" && checkWhiteMove(grid, advancedX, advancedY))
                        {
                            makeWhiteMove(out grid, grid, advancedX, advancedY);
                            moveCount++;
                            debugX = advancedX;
                            debugY = advancedY;
                            PLAYER_TURN = true;
                        }
                        else if (winX == -1 && drawX != -1 && checkWhiteMove(grid, drawX, drawY))
                        {
                            makeWhiteMove(out grid, grid, drawX, drawY);
                            moveCount++;
                            debugX = drawX;
                            debugY = drawY;
                            PLAYER_TURN = true;
                        }
                        else if (currentWinMoves > currentMostLoseMoves && winX != -1 && grid[winX, winY] == "-" && checkWhiteMove(grid, winX, winY))
                        {
                            makeWhiteMove(out grid, grid, winX, winY);
                            moveCount++;
                            debugX = winX;
                            debugY = winY;
                            PLAYER_TURN = true;
                        }
                        else if (averageX != -1 && grid[averageX, averageY] == "-" && checkWhiteMove(grid, averageX, averageY))
                        {
                            makeWhiteMove(out grid, grid, averageX, averageY);
                            moveCount++;
                            debugX = averageX;
                            debugY = averageY;
                            PLAYER_TURN = true;
                        }
                        else if (drawX != -1 && grid[drawX, drawY] == "-" && checkWhiteMove(grid, drawX, drawY))
                        {
                            makeWhiteMove(out grid, grid, drawX, drawY);
                            moveCount++;
                            debugX = drawX;
                            debugY = drawY;
                            PLAYER_TURN = true;
                        }
                        else
                        {
                            for (int x2 = 0; x2 < 8; x2++)
                            {
                                for (int y2 = 0; y2 < 8; y2++)
                                {
                                    if (grid[x2, y2] == "-")
                                    {
                                        if (checkWhiteMove(grid, x2, y2))
                                        {
                                            makeWhiteMove(out grid, grid, x2, y2);
                                            moveCount++;
                                            debugX = x2;
                                            debugY = y2;
                                            PLAYER_TURN = true;
                                            x2 = 8;
                                            y2 = 8;
                                        }
                                    }
                                }
                            }
                        }

                        if (!checkBlackNoMove(grid))  //player (black) has no moves after the move just made; skip turn and make a compute move again
                        {
                            PLAYER_TURN = false;
                            ANIMATION_WHITE = true;
                        }
                    }
                }

                //stateTimer.Dispose();

                int winner = CheckForWinner(grid, out blackCount, out whiteCount);

                if (winner != 0 && GAME_START)
                {
                    if (winner == -1)
                    {
                        winnerString = "Computer is winner. " + blackCount.ToString() + "-" + whiteCount.ToString();
                        computerWins++;
                    }
                    else if (winner == 1)
                    {
                        winnerString = "Player is winner. " + blackCount.ToString() + "-" + whiteCount.ToString();
                        playerWins++;
                    }
                    else if (winner == -2)
                    {
                        winnerString = "Game is a draw. " + blackCount.ToString() + "-" + whiteCount.ToString();
                    }

                    GAME_START = false;
                }
            }

            timer.Stop();
            THREAD_IS_ACTIVE = false;
            autoResetEvent.Set();
        }

        public void PlayerMoveThreadBlack(Object stateInfo)
        {
            AutoResetEvent autoResetEvent = (AutoResetEvent)stateInfo;

            int randX = 0;
            int randY = 0;
            Random randThreadBlack = new Random();
            bool computerWin = false;

            bool COMPUTER_TURN = true;
            int[, , ,] computerWinGrid = new int[2, 60, 8, 8];
            int computerX = -1;
            int computerY = -1;

            threadGridCount = 0;

            /******************************************************************
             * maximum number of times compute will try to win is NUM_SNAPSHOTS
             * ****************************************************************/
            while (threadGridCount < 100)
            {
                computerWin = false;
                COMPUTER_TURN = true;
                int win = 0;
                bool FIRST_MOVE = true;
                int blackCount = 0;
                int whiteCount = 0;
                string[,] tempGrid = new string[8, 8];

                /*
                //replicate the base grid
                for (int x3 = 0; x3 < 8; x3++)
                {
                    for (int y3 = 0; y3 < 8; y3++)
                    {
                        tempGrid[x3, y3] = String.Copy(threadGrid[x3, y3]);
                    }
                }
                 */

                tempGrid = (string[,])threadGrid.Clone();

                int tempMoveCount = moveCount;

                while (win == 0)
                {
                    if (COMPUTER_TURN)
                    {
                        if (checkBlackNoMove(tempGrid))
                        {
                            do
                            {
                                randX = random.Next(8);
                                randY = random.Next(8);

                            } while (!checkBlackMove(tempGrid, randX, randY));

                            makeBlackMove(out tempGrid, tempGrid, randX, randY);

                            if (FIRST_MOVE)
                            {
                                computerX = randX;
                                computerY = randY;
                                FIRST_MOVE = false;
                            }
                        }
                        else
                        {
                            win = CheckForWinner(tempGrid, out blackCount, out whiteCount);
                        }

                        COMPUTER_TURN = false;
                        tempMoveCount++;
                    }
                    else
                    {
                        if (checkWhiteNoMove(tempGrid))
                        {
                            do
                            {
                                randX = random.Next(8);
                                randY = random.Next(8);

                            } while (!checkWhiteMove(tempGrid, randX, randY));

                            makeWhiteMove(out tempGrid, tempGrid, randX, randY);
                        }
                        else
                        {
                            win = CheckForWinner(tempGrid, out blackCount, out whiteCount);
                        }

                        tempMoveCount++;
                        COMPUTER_TURN = true;
                    }

                    //int noOut = 0;


                    if (win == 1)
                    {
                        computerWin = true;
                    }

                    if (computerWin && computerX != -1)
                    {
                        computerWinGrid[0, threadGridCount, computerX, computerY] = 1;
                        computerWinGrid[1, threadGridCount, computerX, computerY] = blackCount - whiteCount;
                        threadGridCount++;
                    }
                    else if (win == -1)
                    {
                        computerWinGrid[0, threadGridCount, computerX, computerY] = -1;
                        computerWinGrid[1, threadGridCount, computerX, computerY] = whiteCount - blackCount;
                        threadGridCount++;
                    }
                    else if (win == -2)
                    {
                        threadGridCount++;
                    }
                }
            }

            int winX = -1;
            int winY = -1;
            int loseX = -1;
            int loseY = -1;
            int averageX = -1;
            int averageY = -1;
            int currentWinMoves = 0;
            int currentLoseMoves = 0;
            int currentAverageMoves = -50;
            int[, ,] winCountMoves = new int[65, 8, 8];
            int[, ,] loseCountMoves = new int[65, 8, 8];
            int[,] averageGrid = new int[8, 8];


            for (int y4 = 0; y4 < threadGridCount; y4++)
            {
                for (int x2 = 0; x2 < 8; x2++)
                {
                    for (int y2 = 0; y2 < 8; y2++)
                    {
                        if (computerWinGrid[0, y4, x2, y2] == 1)
                        {
                            winCountMoves[/*computerWinGrid[1, y4, x2, y2]*/0, x2, y2]++;//+= computerWinGrid[1, y4, x2, y2];
                        }
                        /*if (computerWinGrid[0, y4, x2, y2] == 1)
                        {
                            loseCountMoves[0, x2, y2] ++;//+= computerWinGrid[1, y4, x2, y2];
                        }*/
                    }
                }
            }
            /*
            for (int x2 = 0; x2 < 8; x2++)
            {
                for (int y2 = 0; y2 < 8; y2++)
                {
                    averageGrid[x2, y2] = winCountMoves[0, x2, y2] - loseCountMoves[0, x2, y2];
                }
            }
            */

            //                for (int x4 = 0; x4 < 9; x4++)
            //                {
            for (int x2 = 0; x2 < 8; x2++)
            {
                for (int y2 = 0; y2 < 8; y2++)
                {
                    if (currentWinMoves < winCountMoves[0, x2, y2] && threadGrid[x2, y2] == "-" && winCountMoves[0, x2, y2] != 0)
                    {
                        currentWinMoves = winCountMoves[0, x2, y2];
                        winX = x2;
                        winY = y2;
                    }
                    /*
                    if (currentLoseMoves < loseCountMoves[0, x2, y2] && threadGrid[x2, y2] == "-" && loseCountMoves[0, x2, y2] != 0)
                    {
                        currentLoseMoves = loseCountMoves[0, x2, y2];
                        loseX = x2;
                        loseY = y2;
                    }
                    if (currentAverageMoves < averageGrid[x2, y2] && threadGrid[x2, y2] == "-" && averageGrid[x2, y2] != 0)
                    {
                        currentAverageMoves = averageGrid[x2, y2];
                        averageX = x2;
                        averageY = y2;
                    }
                     * */
                }
            }
            //                }

            if (winX != -1 && threadGrid[winX, winY] == "-" && checkBlackMove(threadGrid, winX, winY))
            {
                threadX = winX;
                threadY = winY;
            }/*
            else if (averageX != -1 && threadGrid[averageX, averageY] == "-" && checkBlackMove(threadGrid, averageX, averageY))
            {
                threadX = averageX;
                threadY = averageY;
            }*/
            else
            {
                do
                {
                    randX = randThreadBlack.Next(8);
                    randY = randThreadBlack.Next(8);

                } while (!checkBlackMove(threadGrid, randX, randY));

                threadX = randX;
                threadY = randY;
            }

            autoResetEvent.Set();
        }

        public void PlayerMoveThreadBlack2(Object stateInfo)
        {
            AutoResetEvent autoResetEvent = (AutoResetEvent)stateInfo;
            
            int threadNumber = -1;

            for (int x = 0; x < 64; x++)
            {
                if (stateInfoArray[x] == autoResetEvent)
                {
                    threadNumber = x;
                }
            }
            
            int randX = 0;
            int randY = 0;
            Random randThreadBlack2 = new Random();
            bool computerWin = false;

            bool COMPUTER_TURN = true;
            int[, , ,] computerWinGrid = new int[2, 300, 8, 8];
            int computerX = -1;
            int computerY = -1;
            int winX = -1;
            int winY = -1;

            threadGridCount = 0;

            string[,] tempGrid2 = new string[8, 8];
            
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    tempGrid2[x, y] = threadGrid2[threadNumber, x, y];
                }
            }               
            
            /******************************************************************
             * maximum number of times compute will try to win is NUM_SNAPSHOTS
             * ****************************************************************/
            while (threadGridCount < 20)
            {
                computerWin = false;
                COMPUTER_TURN = true;
                int win = 0;
                bool FIRST_MOVE = true;
                int blackCount = 0;
                int whiteCount = 0;
                string[,] tempGrid = new string[8, 8];

                /*
                //replicate the base grid
                for (int x3 = 0; x3 < 8; x3++)
                {
                    for (int y3 = 0; y3 < 8; y3++)
                    {
                        tempGrid[x3, y3] = String.Copy(threadGrid[x3, y3]);
                    }
                }
                 */

                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        tempGrid[x, y] = threadGrid2[threadNumber, x, y];
                    }
                }               

                int tempMoveCount = moveCount;

                while (win == 0)
                {
                    if (COMPUTER_TURN)
                    {
                        if (checkBlackNoMove(tempGrid))
                        {
                            do
                            {
                                randX = randThreadBlack2.Next(8);
                                randY = randThreadBlack2.Next(8);

                            } while (!checkBlackMove(tempGrid, randX, randY));

                            makeBlackMove(out tempGrid, tempGrid, randX, randY);

                            if (FIRST_MOVE)
                            {
                                computerX = randX;
                                computerY = randY;
                                FIRST_MOVE = false;
                            }
                        }
                        else
                        {
                            win = CheckForWinner(tempGrid, out blackCount, out whiteCount);
                        }

                        COMPUTER_TURN = false;
                        tempMoveCount++;
                    }
                    else
                    {
                        if (checkWhiteNoMove(tempGrid))
                        {
                            do
                            {
                                randX = randThreadBlack2.Next(8);
                                randY = randThreadBlack2.Next(8);

                            } while (!checkWhiteMove(tempGrid, randX, randY));

                            makeWhiteMove(out tempGrid, tempGrid, randX, randY);
                        }
                        else
                        {
                            win = CheckForWinner(tempGrid, out blackCount, out whiteCount);
                        }

                        tempMoveCount++;
                        COMPUTER_TURN = true;
                    }

                    //int noOut = 0;


                    if (win == 1)
                    {
                        computerWin = true;
                    }

                    if (computerWin && computerX != -1)
                    {
                        computerWinGrid[0, threadGridCount, computerX, computerY] = 1;
                        computerWinGrid[1, threadGridCount, computerX, computerY] = blackCount - whiteCount;
                        threadGridCount++;
                        winX = computerX;
                        winY = computerY;
                        threadGridCount = 128;
                        break;
                    }
                    else if (win == -1)
                    {
                        computerWinGrid[0, threadGridCount, computerX, computerY] = -1;
                        computerWinGrid[1, threadGridCount, computerX, computerY] = whiteCount - blackCount;
                        threadGridCount++;
                    }
                    else if (win == -2)
                    {
                        threadGridCount++;
                    }
                }
            }

            /*
            int[, ,] winCountMoves = new int[65, 8, 8];
            int currentWinMoves = 0;

            for (int y4 = 0; y4 < threadGridCount; y4++)
            {
                for (int x2 = 0; x2 < 8; x2++)
                {
                    for (int y2 = 0; y2 < 8; y2++)
                    {
                        if (computerWinGrid[0, y4, x2, y2] == 1)
                        {
                            winCountMoves[0, x2, y2]++;
                        }
                    }
                }
            }

            for (int x2 = 0; x2 < 8; x2++)
            {
                for (int y2 = 0; y2 < 8; y2++)
                {
                    if (currentWinMoves < winCountMoves[0, x2, y2] && tempGrid2[x2, y2] == "-" && winCountMoves[0, x2, y2] != 0)
                    {
                        currentWinMoves = winCountMoves[0, x2, y2];
                        winX = x2;
                        winY = y2;
                    }
                }
            }
            */
            if (winX != -1 && tempGrid2[winX, winY] == "-"/* && checkBlackMove(tempGrid2, winX, winY)*/)
            {
                threadXarray[threadNumber] = winX;
                threadYarray[threadNumber] = winY;
            }/*
            else if (averageX != -1 && threadGrid[averageX, averageY] == "-" && checkBlackMove(threadGrid, averageX, averageY))
            {
                threadX = averageX;
                threadY = averageY;
            }*/
            else
            {
                do
                {
                    randX = randThreadBlack2.Next(8);
                    randY = randThreadBlack2.Next(8);

                } while (!checkBlackMove(tempGrid2, randX, randY));

                threadXarray[threadNumber] = randX;
                threadYarray[threadNumber] = randY;
            }

            autoResetEvent.Set();
        }

        public void PlayerMoveThreadWhite(Object stateInfo)
        {
            AutoResetEvent autoResetEvent = (AutoResetEvent)stateInfo;

            int randX = 0;
            int randY = 0;
            Random randThreadWhite = new Random();

            bool computerWin = false;

            bool COMPUTER_TURN = true;
            int[, , ,] computerWinGrid = new int[2, 20, 8, 8];
            int computerX = -1;
            int computerY = -1;

            threadGridCount = 0;

            /******************************************************************
             * maximum number of times compute will try to win is NUM_SNAPSHOTS
             * ****************************************************************/
            while (threadGridCount < 100)
            {
                computerWin = false;
                COMPUTER_TURN = true;
                int win = 0;
                bool FIRST_MOVE = true;
                int blackCount = 0;
                int whiteCount = 0;
                string[,] tempGrid = new string[8, 8];

                /*
                //replicate the base grid
                for (int x3 = 0; x3 < 8; x3++)
                {
                    for (int y3 = 0; y3 < 8; y3++)
                    {
                        tempGrid[x3, y3] = String.Copy(threadGrid[x3, y3]);
                    }
                }
                */
                tempGrid = (string[,])threadGrid.Clone();

                int tempMoveCount = moveCount;

                while (win == 0)
                {
                    if (COMPUTER_TURN)
                    {
                        if (checkWhiteNoMove(tempGrid))
                        {
                            do
                            {
                                randX = random.Next(8);
                                randY = random.Next(8);

                            } while (!checkWhiteMove(tempGrid, randX, randY));

                            makeWhiteMove(out tempGrid, tempGrid, randX, randY);

                            if (FIRST_MOVE)
                            {
                                computerX = randX;
                                computerY = randY;
                                FIRST_MOVE = false;
                            }
                        }
                        else
                        {
                            win = CheckForWinner(tempGrid, out blackCount, out whiteCount);
                        }

                        COMPUTER_TURN = false;
                        tempMoveCount++;
                    }
                    else
                    {
                        if (checkBlackNoMove(tempGrid))
                        {
                            do
                            {
                                randX = random.Next(8);
                                randY = random.Next(8);

                            } while (!checkBlackMove(tempGrid, randX, randY));

                            makeBlackMove(out tempGrid, tempGrid, randX, randY);
                        }
                        else
                        {
                            win = CheckForWinner(tempGrid, out blackCount, out whiteCount);
                        }

                        tempMoveCount++;
                        COMPUTER_TURN = true;
                    }

                    //int noOut = 0;


                    if (win == -1)
                    {
                        computerWin = true;
                    }

                    if (computerWin && computerX != -1)
                    {
                        computerWinGrid[0, threadGridCount, computerX, computerY] = -1;
                        computerWinGrid[1, threadGridCount, computerX, computerY] = whiteCount - blackCount;
                        threadGridCount++;
                    }
                    else if (win == 1)
                    {
                        computerWinGrid[0, threadGridCount, computerX, computerY] = 1;
                        computerWinGrid[1, threadGridCount, computerX, computerY] = blackCount - whiteCount;
                        threadGridCount++;
                    }
                    else if (win == -2)
                    {
                        threadGridCount++;
                    }
                }
            }

            int winX = -1;
            int winY = -1;
            int loseX = -1;
            int loseY = -1;
            int averageX = -1;
            int averageY = -1;
            int currentWinMoves = 0;
            int currentLoseMoves = 0;
            int currentAverageMoves = -50;
            int[, ,] winCountMoves = new int[65, 8, 8];
            int[, ,] loseCountMoves = new int[65, 8, 8];
            int[,] averageGrid = new int[8, 8];


            for (int y4 = 0; y4 < threadGridCount; y4++)
            {
                for (int x2 = 0; x2 < 8; x2++)
                {
                    for (int y2 = 0; y2 < 8; y2++)
                    {
                        if (computerWinGrid[0, y4, x2, y2] == -1)
                        {
                            winCountMoves[/*computerWinGrid[1, y4, x2, y2]*/0, x2, y2]++;//+= computerWinGrid[1, y4, x2, y2];
                        }
                        /*if (computerWinGrid[0, y4, x2, y2] == 1)
                        {
                            loseCountMoves[0, x2, y2] ++;//+= computerWinGrid[1, y4, x2, y2];
                        }*/
                    }
                }
            }
            /*
            for (int x2 = 0; x2 < 8; x2++)
            {
                for (int y2 = 0; y2 < 8; y2++)
                {
                    averageGrid[x2, y2] = winCountMoves[0, x2, y2] - loseCountMoves[0, x2, y2];
                }
            }
            */

            //                for (int x4 = 0; x4 < 9; x4++)
            //                {
            for (int x2 = 0; x2 < 8; x2++)
            {
                for (int y2 = 0; y2 < 8; y2++)
                {
                    if (currentWinMoves < winCountMoves[0, x2, y2] && threadGrid[x2, y2] == "-" && winCountMoves[0, x2, y2] != 0)
                    {
                        currentWinMoves = winCountMoves[0, x2, y2];
                        winX = x2;
                        winY = y2;
                    }
                    /*if (currentLoseMoves < loseCountMoves[0, x2, y2] && threadGrid[x2, y2] == "-" && loseCountMoves[0, x2, y2] != 0)
                    {
                        currentLoseMoves = loseCountMoves[0, x2, y2];
                        loseX = x2;
                        loseY = y2;
                    }
                    if (currentAverageMoves < averageGrid[x2, y2] && threadGrid[x2, y2] == "-")
                    {
                        currentAverageMoves = averageGrid[x2, y2];
                        averageX = x2;
                        averageY = y2;
                    }*/
                }
            }
            //                }

            if (winX != -1 && threadGrid[winX, winY] == "-" && checkBlackMove(threadGrid, winX, winY))
            {
                threadX = winX;
                threadY = winY;
            }/*
            else if (averageX != -1 && threadGrid[averageX, averageY] == "-" && checkWhiteMove(threadGrid, averageX, averageY))
            {
                threadX = averageX;
                threadY = averageY;
            }*/
            else
            {
                do
                {
                    randX = randThreadWhite.Next(8);
                    randY = randThreadWhite.Next(8);

                } while (!checkWhiteMove(threadGrid, randX, randY));

                threadX = randX;
                threadY = randY;
            }

            autoResetEvent.Set();
        }
        
        public void PlayerMoveThreadWhite2(Object stateInfo)
        {
            AutoResetEvent autoResetEvent = (AutoResetEvent)stateInfo;

            int threadNumber = -1;

            for (int x = 0; x < 64; x++)
            {
                if (stateInfoArray[x] == autoResetEvent)
                {
                    threadNumber = x;
                }
            }

            int randX = 0;
            int randY = 0;
            Random randThreadWhite2 = new Random();
            bool computerWin = false;

            bool COMPUTER_TURN = true;
            int[, , ,] computerWinGrid = new int[2, 300, 8, 8];
            int computerX = -1;
            int computerY = -1;
            int winX = -1;
            int winY = -1;

            string[,] tempGrid2 = new string[8, 8];

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    tempGrid2[x, y] = threadGrid2[threadNumber, x, y];
                }
            }               
            
            threadGridCount = 0;

            /******************************************************************
             * maximum number of times compute will try to win is NUM_SNAPSHOTS
             * ****************************************************************/
            while (threadGridCount < 20)
            {
                computerWin = false;
                COMPUTER_TURN = true;
                int win = 0;
                bool FIRST_MOVE = true;
                int blackCount = 0;
                int whiteCount = 0;
                string[,] tempGrid = new string[8, 8];

                /*
                //replicate the base grid
                for (int x3 = 0; x3 < 8; x3++)
                {
                    for (int y3 = 0; y3 < 8; y3++)
                    {
                        tempGrid[x3, y3] = String.Copy(threadGrid[x3, y3]);
                    }
                }
                */

                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        tempGrid[x, y] = threadGrid2[threadNumber, x, y];
                    }
                }


                int tempMoveCount = moveCount;

                while (win == 0)
                {
                    if (COMPUTER_TURN)
                    {
                        if (checkWhiteNoMove(tempGrid))
                        {
                            do
                            {
                                randX = randThreadWhite2.Next(8);
                                randY = randThreadWhite2.Next(8);

                            } while (!checkWhiteMove(tempGrid, randX, randY));

                            makeWhiteMove(out tempGrid, tempGrid, randX, randY);

                            if (FIRST_MOVE)
                            {
                                computerX = randX;
                                computerY = randY;
                                FIRST_MOVE = false;
                            }
                        }
                        else
                        {
                            win = CheckForWinner(tempGrid, out blackCount, out whiteCount);
                        }

                        COMPUTER_TURN = false;
                        tempMoveCount++;
                    }
                    else
                    {
                        if (checkBlackNoMove(tempGrid))
                        {
                            do
                            {
                                randX = randThreadWhite2.Next(8);
                                randY = randThreadWhite2.Next(8);

                            } while (!checkBlackMove(tempGrid, randX, randY));

                            makeBlackMove(out tempGrid, tempGrid, randX, randY);
                        }
                        else
                        {
                            win = CheckForWinner(tempGrid, out blackCount, out whiteCount);
                        }

                        tempMoveCount++;
                        COMPUTER_TURN = true;
                    }

                    //int noOut = 0;


                    if (win == -1)
                    {
                        computerWin = true;
                    }

                    if (computerWin && computerX != -1)
                    {
                        computerWinGrid[0, threadGridCount, computerX, computerY] = -1;
                        computerWinGrid[1, threadGridCount, computerX, computerY] = whiteCount - blackCount;
                        threadGridCount++;
                        winX = computerX;
                        winY = computerY;
                        threadGridCount = 128;
                        break;
                    }
                    else if (win == 1)
                    {
                        computerWinGrid[0, threadGridCount, computerX, computerY] = 1;
                        computerWinGrid[1, threadGridCount, computerX, computerY] = blackCount - whiteCount;
                        threadGridCount++;
                    }
                    else if (win == -2)
                    {
                        threadGridCount++;
                    }
                }
            }
/*
            int[, ,] winCountMoves = new int[65, 8, 8];
            int[,] averageGrid = new int[8, 8];

                for (int y4 = 0; y4 < threadGridCount; y4++)
                {
                    for (int x2 = 0; x2 < 8; x2++)
                    {
                        for (int y2 = 0; y2 < 8; y2++)
                        {
                            if (computerWinGrid[0, y4, x2, y2] == -1)
                            {
                                winCountMoves[0, x2, y2]++;;
                            }
                        }
                    }
                }

                for (int x2 = 0; x2 < 8; x2++)
                {
                    for (int y2 = 0; y2 < 8; y2++)
                    {
                        if (currentWinMoves < winCountMoves[0, x2, y2] && tempGrid2[x2, y2] == "-" && winCountMoves[0, x2, y2] != 0)
                        {
                            currentWinMoves = winCountMoves[0, x2, y2];
                            winX = x2;
                            winY = y2;
                        }
                    }
                }
            }
*/
            if (winX != -1 && tempGrid2[winX, winY] == "-" && checkBlackMove(tempGrid2, winX, winY))
            {
                threadXarray[threadNumber] = winX;
                threadYarray[threadNumber] = winY;
            }
            else
            {
                do
                {
                    randX = randThreadWhite2.Next(8);
                    randY = randThreadWhite2.Next(8);

                } while (!checkWhiteMove(tempGrid2, randX, randY));

                threadXarray[threadNumber] = randX;
                threadYarray[threadNumber] = randY;
            }

            autoResetEvent.Set();
        }

        public int CheckForWinner(string[,] grid2, out int blackCount, out int whiteCount)
        {
            int counter = 0;
            int tempWhiteCount = 0;
            int tempBlackCount = 0;

            /*
                        if (!checkBlackNoMove(grid))
                        {
                            PLAYER_TURN = false;
                            ANIMATION_WHITE = true;
                        }
                        else if (!checkWhiteNoMove(grid))
                        {
                            PLAYER_TURN = true;
                            ANIMATION_WHITE = true;
                        }
            */

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (grid2[x, y] == "-")
                    {
                        counter++;
                    }
                    else if (grid2[x, y] == "X")
                    {
                        tempWhiteCount++;
                    }
                    else if (grid2[x, y] == "Y")
                    {
                        tempBlackCount++;
                    }
                }
            }

            if (tempBlackCount == 0 || tempWhiteCount == 0 || tempWhiteCount + tempBlackCount == 64 || (!checkWhiteNoMove(grid2) && !checkBlackNoMove(grid2)))
            {
                if (tempBlackCount > tempWhiteCount)
                {
                    blackCount = tempBlackCount;
                    whiteCount = tempWhiteCount;
                    return 1;
                }
                else if (tempWhiteCount > tempBlackCount)
                {
                    blackCount = tempBlackCount;
                    whiteCount = tempWhiteCount;
                    return -1;
                }
                else if (tempBlackCount == tempWhiteCount)
                {
                    blackCount = tempBlackCount;
                    whiteCount = tempWhiteCount;
                    return -2;
                }
            }

            blackCount = tempBlackCount;
            whiteCount = tempWhiteCount;
            return 0;
        }

        public bool checkBlackMove(string[,] grid2, int x, int y)
        {
            if (grid2[x, y] != "-")
            {
                return false;
            }

            /*
            int xBuffer = 0, yBuffer = 0;

            if (x == 0)
            {
                xBuffer = 1;
            }
            else if (x == 7)
            {
                xBuffer = -1;
            } if (y == 0)
            {
                yBuffer = 1;
            }
            else if (y == 7)
            {
                yBuffer = -1;
            }

            if (grid2[x + 1 + xBuffer, y + 1 + yBuffer] == "-" && grid2[x + xBuffer + 1, y] == "-" && grid2[x, y + 1 + yBuffer] == "-" && grid2[x + 1 + xBuffer, y - 1 + yBuffer] == "-" &&
                grid2[x - 1 + xBuffer, y + 1 + yBuffer] == "-" && grid2[x - 1 + xBuffer, y - 1 + yBuffer] == "-" && grid2[x, y - 1 + yBuffer] == "-" && grid2[x - 1 + xBuffer, y] == "-")
            {
                return false;
            }
            */
            //check right
            if (x < 6 && grid2[x + 1, y] == "X")
            {
                for (int x2 = x + 2; x2 < 8 && grid2[x2, y] != "-"; x2++)
                {
                    if (grid2[x2, y] == "Y")
                    {
                        return true;
                    }
                }
            }
            //check lower right
            if (x < 6 && y < 6 && grid2[x + 1, y + 1] == "X")
            {
                for (int z = 2; x + z < 8 && y + z < 8 && grid2[x + z, y + z] != "-"; z++)
                {
                    if (grid2[x + z, y + z] == "Y")
                    {
                        return true;
                    }
                }
            }
            //check upper right
            if (x < 6 && y > 1 && grid2[x + 1, y - 1] == "X")
            {
                for (int z = 2; x + z < 8 && y - z >= 0 && grid2[x + z, y - z] != "-"; z++)
                {
                    if (grid2[x + z, y - z] == "Y")
                    {
                        return true;
                    }
                }
            }

            //check left
            if (x > 1 && grid2[x - 1, y] == "X")
            {
                for (int x2 = x - 2; x2 >= 0 && grid2[x2, y] != "-"; x2--)
                {
                    if (grid2[x2, y] == "Y")
                    {
                        return true;
                    }
                }
            }
            //check lower left
            if (x > 1 && y < 6 && grid2[x - 1, y + 1] == "X")
            {
                for (int z = 2; x - z >= 0 && y + z < 8 && grid2[x - z, y + z] != "-"; z++)
                {
                    if (grid2[x - z, y + z] == "Y")
                    {
                        return true;
                    }
                }
            }
            //check upper left
            if (x > 1 && y > 1 && grid2[x - 1, y - 1] == "X")
            {
                for (int z = 2; x - z >= 0 && y - z >= 0 && grid2[x - z, y - z] != "-"; z++)
                {
                    if (grid2[x - z, y - z] == "Y")
                    {
                        return true;
                    }
                }
            }

            //check down
            if (y < 6 && grid2[x, y + 1] == "X")
            {
                for (int y2 = y + 2; y2 < 8 && grid2[x, y2] != "-"; y2++)
                {
                    if (grid2[x, y2] == "Y")
                    {
                        return true;
                    }
                }
            }
            //check up
            if (y > 1 && grid2[x, y - 1] == "X")
            {
                for (int y2 = y - 2; y2 >= 0 && grid2[x, y2] != "-"; y2--)
                {
                    if (grid2[x, y2] == "Y")
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool checkWhiteMove(string[,] grid2, int x, int y)
        {
            if (grid2[x, y] != "-")
            {
                return false;
            }

            /*
            int xBuffer = 0, yBuffer = 0;

            if (x == 0)
            {
                xBuffer = 1;
            }
            else if (x == 7)
            {
                xBuffer = -1;
            } if (y == 0)
            {
                yBuffer = 1;
            }
            else if (y == 7)
            {
                yBuffer = -1;
            }

            if (grid2[x + 1 + xBuffer, y + 1 + yBuffer] == "-" && grid2[x + xBuffer + 1, y] == "-" && grid2[x, y + 1 + yBuffer] == "-" && grid2[x + 1 + xBuffer, y - 1 + yBuffer] == "-" &&
                grid2[x - 1 + xBuffer, y + 1 + yBuffer] == "-" && grid2[x - 1 + xBuffer, y - 1 + yBuffer] == "-" && grid2[x, y - 1 + yBuffer] == "-" && grid2[x - 1 + xBuffer, y] == "-")
            {
                return false;
            }
            */
            //check right
            if (x < 6 && grid2[x + 1, y] == "Y")
            {
                for (int x2 = x + 2; x2 < 8 && grid2[x2, y] != "-"; x2++)
                {
                    if (grid2[x2, y] == "X")
                    {
                        return true;
                    }
                }
            }
            //check lower right
            if (x < 6 && y < 6 && grid2[x + 1, y + 1] == "Y")
            {
                for (int z = 2; x + z < 8 && y + z < 8 && grid2[x + z, y + z] != "-"; z++)
                {
                    if (grid2[x + z, y + z] == "X")
                    {
                        return true;
                    }
                }
            }
            //check upper right
            if (x < 6 && y > 1 && grid2[x + 1, y - 1] == "Y")
            {
                for (int z = 2; x + z < 8 && y - z >= 0 && grid2[x + z, y - z] != "-"; z++)
                {
                    if (grid2[x + z, y - z] == "X")
                    {
                        return true;
                    }
                }
            }

            //check left
            if (x > 1 && grid2[x - 1, y] == "Y")
            {
                for (int x2 = x - 2; x2 >= 0 && grid2[x2, y] != "-"; x2--)
                {
                    if (grid2[x2, y] == "X")
                    {
                        return true;
                    }
                }
            }
            //check lower left
            if (x > 1 && y < 6 && grid2[x - 1, y + 1] == "Y")
            {
                for (int z = 2; x - z >= 0 && y + z < 8 && grid2[x - z, y + z] != "-"; z++)
                {
                    if (grid2[x - z, y + z] == "X")
                    {
                        return true;
                    }
                }
            }
            //check upper left
            if (x > 1 && y > 1 && grid2[x - 1, y - 1] == "Y")
            {
                for (int z = 2; x - z >= 0 && y - z >= 0 && grid2[x - z, y - z] != "-"; z++)
                {
                    if (grid2[x - z, y - z] == "X")
                    {
                        return true;
                    }
                }
            }

            //check down
            if (y < 6 && grid2[x, y + 1] == "Y")
            {
                for (int y2 = y + 2; y2 < 8 && grid2[x, y2] != "-"; y2++)
                {
                    if (grid2[x, y2] == "X")
                    {
                        return true;
                    }
                }
            }
            //check up
            if (y > 1 && grid2[x, y - 1] == "Y")
            {
                for (int y2 = y - 2; y2 >= 0 && grid2[x, y2] != "-"; y2--)
                {
                    if (grid2[x, y2] == "X")
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool makeBlackMove(out string[,] outGrid, string[,] grid2, int x, int y)
        {
            string[,] tempGrid = new string[8, 8];

            /*
            for (int x2 = 0; x2 < 8; x2++)
            {
                for (int y2 = 0; y2 < 8; y2++)
                {
                    tempGrid[x2, y2] = String.Copy(grid2[x2, y2]);
                }
            }
            */

            tempGrid = (string[,])grid2.Clone();

            //check right
            if (x < 6 && grid2[x + 1, y] == "X")
            {
                for (int x2 = x + 2; x2 < 8 && grid2[x2, y] != "-"; x2++)
                {
                    if (grid2[x2, y] == "Y")
                    {
                        for (int x3 = x; x3 < x2; x3++)
                        {
                            tempGrid[x3, y] = "Y";
                        }

                        break;
                    }
                }
            }
            //check left
            if (x > 1 && grid2[x - 1, y] == "X")
            {
                for (int x2 = x - 2; x2 >= 0 && grid2[x2, y] != "-"; x2--)
                {
                    if (grid2[x2, y] == "Y")
                    {
                        for (int x3 = x; x3 > x2; x3--)
                        {
                            tempGrid[x3, y] = "Y";
                        }

                        break;
                    }
                }
            }
            //check down
            if (y < 6 && grid2[x, y + 1] == "X")
            {
                for (int y2 = y + 2; y2 < 8 && grid2[x, y2] != "-"; y2++)
                {
                    if (grid2[x, y2] == "Y")
                    {
                        for (int y3 = y; y3 < y2; y3++)
                        {
                            tempGrid[x, y3] = "Y";
                        }

                        break;
                    }
                }
            }
            //check up
            if (y > 1 && grid2[x, y - 1] == "X")
            {
                for (int y2 = y - 2; y2 >= 0 && grid2[x, y2] != "-"; y2--)
                {
                    if (grid2[x, y2] == "Y")
                    {
                        for (int y3 = y; y3 > y2; y3--)
                        {
                            tempGrid[x, y3] = "Y";
                        }

                        break;
                    }
                }
            }
            //check lower right
            if (x < 6 && y < 6 && grid2[x + 1, y + 1] == "X")
            {
                for (int z = 2; x + z < 8 && y + z < 8 && grid2[x + z, y + z] != "-"; z++)
                {
                    if (grid2[x + z, y + z] == "Y")
                    {
                        for (int z3 = z; z3 >= 0; z3--)
                        {
                            tempGrid[x + z3, y + z3] = "Y";
                        }

                        break;
                    }
                }
            }
            //check lower left
            if (x > 1 && y < 6 && grid2[x - 1, y + 1] == "X")
            {
                for (int z = 2; x - z >= 0 && y + z < 8 && grid2[x - z, y + z] != "-"; z++)
                {
                    if (grid2[x - z, y + z] == "Y")
                    {
                        for (int z3 = z; z3 >= 0; z3--)
                        {
                            tempGrid[x - z3, y + z3] = "Y";
                        }

                        break;
                    }
                }
            }
            //check upper right
            if (x < 6 && y > 1 && grid2[x + 1, y - 1] == "X")
            {
                for (int z = 2; x + z < 8 && y - z >= 0 && grid2[x + z, y - z] != "-"; z++)
                {
                    if (grid2[x + z, y - z] == "Y")
                    {
                        for (int z3 = z; z3 >= 0; z3--)
                        {
                            tempGrid[x + z3, y - z3] = "Y";
                        }

                        break;
                    }
                }
            }
            //check upper left
            if (x > 1 && y > 1 && grid2[x - 1, y - 1] == "X")
            {
                for (int z = 2; x - z >= 0 && y - z >= 0 && grid2[x - z, y - z] != "-"; z++)
                {
                    if (grid2[x - z, y - z] == "Y")
                    {
                        for (int z3 = z; z3 >= 0; z3--)
                        {
                            tempGrid[x - z3, y - z3] = "Y";
                        }

                        break;
                    }
                }
            }

            outGrid = tempGrid;
            return true;
        }

        public bool makeBlackMoveAdv(out string[,] outGrid, string[,] grid2, int x, int y)
        {
            string[,] tempGrid = new string[8, 8];

            /*
            for (int x2 = 0; x2 < 8; x2++)
            {
                for (int y2 = 0; y2 < 8; y2++)
                {
                    tempGrid[x2, y2] = String.Copy(grid2[x2, y2]);
                }
            }
            */

            tempGrid = (string[,])grid2.Clone();

            //check right
            if (x < 6 && grid2[x + 1, y] == "X")
            {
                for (int x2 = x + 2; x2 < 8 && grid2[x2, y] != "-"; x2++)
                {
                    if (grid2[x2, y] == "Y")
                    {
                        for (int x3 = x; x3 < x2; x3++)
                        {
                            tempGrid[x3, y] = "Y";
                        }

                        break;
                    }
                }
            }
            //check left
            if (x > 1 && grid2[x - 1, y] == "X")
            {
                for (int x2 = x - 2; x2 >= 0 && grid2[x2, y] != "-"; x2--)
                {
                    if (grid2[x2, y] == "Y")
                    {
                        for (int x3 = x; x3 > x2; x3--)
                        {
                            tempGrid[x3, y] = "Y";
                        }

                        break;
                    }
                }
            }
            //check down
            if (y < 6 && grid2[x, y + 1] == "X")
            {
                for (int y2 = y + 2; y2 < 8 && grid2[x, y2] != "-"; y2++)
                {
                    if (grid2[x, y2] == "Y")
                    {
                        for (int y3 = y; y3 < y2; y3++)
                        {
                            tempGrid[x, y3] = "Y";
                        }

                        break;
                    }
                }
            }
            //check up
            if (y > 1 && grid2[x, y - 1] == "X")
            {
                for (int y2 = y - 2; y2 >= 0 && grid2[x, y2] != "-"; y2--)
                {
                    if (grid2[x, y2] == "Y")
                    {
                        for (int y3 = y; y3 > y2; y3--)
                        {
                            tempGrid[x, y3] = "Y";
                        }

                        break;
                    }
                }
            }
            //check lower right
            if (x < 6 && y < 6 && grid2[x + 1, y + 1] == "X")
            {
                for (int z = 2; x + z < 8 && y + z < 8 && grid2[x + z, y + z] != "-"; z++)
                {
                    if (grid2[x + z, y + z] == "Y")
                    {
                        for (int z3 = z; z3 >= 0; z3--)
                        {
                            tempGrid[x + z3, y + z3] = "Y";
                        }

                        break;
                    }
                }
            }
            //check lower left
            if (x > 1 && y < 6 && grid2[x - 1, y + 1] == "X")
            {
                for (int z = 2; x - z >= 0 && y + z < 8/* && grid2[x - z, y + z] != "-"*/; z++)
                {
                    if (grid2[x - z, y + z] == "Y")
                    {
                        for (int z3 = z; z3 >= 0; z3--)
                        {
                            tempGrid[x - z3, y + z3] = "Y";
                        }

                        break;
                    }
                }
            }
            //check upper right
            if (x < 6 && y > 1 && grid2[x + 1, y - 1] == "X")
            {
                for (int z = 2; x + z < 8 && y - z >= 0 && grid2[x + z, y - z] != "-"; z++)
                {
                    if (grid2[x + z, y - z] == "Y")
                    {
                        for (int z3 = z; z3 >= 0; z3--)
                        {
                            tempGrid[x + z3, y - z3] = "Y";
                        }

                        break;
                    }
                }
            }
            //check upper left
            if (x > 1 && y > 1 && grid2[x - 1, y - 1] == "X")
            {
                for (int z = 2; x - z >= 0 && y - z >= 0 && grid2[x - z, y - z] != "-"; z++)
                {
                    if (grid2[x - z, y - z] == "Y")
                    {
                        for (int z3 = z; z3 >= 0; z3--)
                        {
                            tempGrid[x - z3, y - z3] = "Y";
                        }

                        break;
                    }
                }
            }

            outGrid = tempGrid;
            return true;
        }

        public bool checkBlackNoMove(string[,] grid2)
        {
            bool CAN_MOVE = false;

            for (int x = 0; x < 8 && !CAN_MOVE; x++)
            {
                for (int y = 0; y < 8 && !CAN_MOVE; y++)
                {
                    if (grid[x, y] == "-")
                    {
                        if (checkBlackMove(grid2, x, y))
                        {
                            CAN_MOVE = true;
                        }
                    }
                }
            }

            return CAN_MOVE;
        }

        public bool makeWhiteMoveAdv(out string[,] outGrid, string[,] grid2, int x, int y)
        {
            string[,] tempGrid = new string[8, 8];

            /*for (int x2 = 0; x2 < 8; x2++)
            {
                for (int y2 = 0; y2 < 8; y2++)
                {
                    tempGrid[x2, y2] = String.Copy(grid2[x2, y2]);
                }
            }
            */
            tempGrid = (string[,])grid2.Clone();

            //check right
            if (x < 6 && grid2[x + 1, y] == "Y")
            {
                for (int x2 = x + 2; x2 < 8 && grid2[x2, y] != "-"; x2++)
                {
                    if (grid2[x2, y] == "X")
                    {
                        for (int x3 = x; x3 < x2; x3++)
                        {
                            tempGrid[x3, y] = "X";
                        }

                        break;
                    }
                }
            }
            //check left
            if (x > 1 && grid2[x - 1, y] == "Y")
            {
                for (int x2 = x - 2; x2 >= 0 && grid2[x2, y] != "-"; x2--)
                {
                    if (grid2[x2, y] == "X")
                    {
                        for (int x3 = x; x3 > x2; x3--)
                        {
                            tempGrid[x3, y] = "X";
                        }

                        break;
                    }
                }
            }
            //check down
            if (y < 6 && grid2[x, y + 1] == "Y")
            {
                for (int y2 = y + 2; y2 < 8 && grid2[x, y2] != "-"; y2++)
                {
                    if (grid2[x, y2] == "X")
                    {
                        for (int y3 = y; y3 < y2; y3++)
                        {
                            tempGrid[x, y3] = "X";
                        }

                        break;
                    }
                }
            }
            //check up
            if (y > 1 && grid2[x, y - 1] == "Y")
            {
                for (int y2 = y - 2; y2 >= 0 && grid2[x, y2] != "-"; y2--)
                {
                    if (grid2[x, y2] == "X")
                    {
                        for (int y3 = y; y3 > y2; y3--)
                        {
                            tempGrid[x, y3] = "X";
                        }

                        break;
                    }
                }
            }
            //check lower right
            if (x < 6 && y < 6 && grid2[x + 1, y + 1] == "Y")
            {
                for (int z = 2; x + z < 8 && y + z < 8 && grid2[x + z, y + z] != "-"; z++)
                {
                    if (grid2[x + z, y + z] == "X")
                    {
                        for (int z3 = z; z3 >= 0; z3--)
                        {
                            tempGrid[x + z3, y + z3] = "X";
                        }

                        break;
                    }
                }
            }
            //check lower left
            if (x > 1 && y < 6 && grid2[x - 1, y + 1] == "Y")
            {
                for (int z = 2; x - z >= 0 && y + z < 8/* && grid2[x - z, y + z] != "-"*/; z++)
                {
                    if (grid2[x - z, y + z] == "X")
                    {
                        for (int z3 = z; z3 >= 0; z3--)
                        {
                            tempGrid[x - z3, y + z3] = "X";
                        }

                        break;
                    }
                }
            }
            //check upper right
            if (x < 6 && y > 1 && grid2[x + 1, y - 1] == "Y")
            {
                for (int z = 2; x + z < 8 && y - z >= 0 && grid2[x + z, y - z] != "-"; z++)
                {
                    if (grid2[x + z, y - z] == "X")
                    {
                        for (int z3 = z; z3 >= 0; z3--)
                        {
                            tempGrid[x + z3, y - z3] = "X";
                        }

                        break;
                    }
                }
            }
            //check upper left
            if (x > 1 && y > 1 && grid2[x - 1, y - 1] == "Y")
            {
                for (int z = 2; x - z >= 0 && y - z >= 0 && grid2[x - z, y - z] != "-"; z++)
                {
                    if (grid2[x - z, y - z] == "X")
                    {
                        for (int z3 = z; z3 >= 0; z3--)
                        {
                            tempGrid[x - z3, y - z3] = "X";
                        }

                        break;
                    }
                }
            }

            outGrid = tempGrid;
            return true;
        }
        public bool makeWhiteMove(out string[,] outGrid, string[,] grid2, int x, int y)
        {
            string[,] tempGrid = new string[8, 8];

            /*for (int x2 = 0; x2 < 8; x2++)
            {
                for (int y2 = 0; y2 < 8; y2++)
                {
                    tempGrid[x2, y2] = String.Copy(grid2[x2, y2]);
                }
            }
            */
            tempGrid = (string[,])grid2.Clone();

            //check right
            if (x < 6 && grid2[x + 1, y] == "Y")
            {
                for (int x2 = x + 2; x2 < 8 && grid2[x2, y] != "-"; x2++)
                {
                    if (grid2[x2, y] == "X")
                    {
                        for (int x3 = x; x3 < x2; x3++)
                        {
                            tempGrid[x3, y] = "X";
                        }

                        break;
                    }
                }
            }
            //check left
            if (x > 1 && grid2[x - 1, y] == "Y")
            {
                for (int x2 = x - 2; x2 >= 0 && grid2[x2, y] != "-"; x2--)
                {
                    if (grid2[x2, y] == "X")
                    {
                        for (int x3 = x; x3 > x2; x3--)
                        {
                            tempGrid[x3, y] = "X";
                        }

                        break;
                    }
                }
            }
            //check down
            if (y < 6 && grid2[x, y + 1] == "Y")
            {
                for (int y2 = y + 2; y2 < 8 && grid2[x, y2] != "-"; y2++)
                {
                    if (grid2[x, y2] == "X")
                    {
                        for (int y3 = y; y3 < y2; y3++)
                        {
                            tempGrid[x, y3] = "X";
                        }

                        break;
                    }
                }
            }
            //check up
            if (y > 1 && grid2[x, y - 1] == "Y")
            {
                for (int y2 = y - 2; y2 >= 0 && grid2[x, y2] != "-"; y2--)
                {
                    if (grid2[x, y2] == "X")
                    {
                        for (int y3 = y; y3 > y2; y3--)
                        {
                            tempGrid[x, y3] = "X";
                        }

                        break;
                    }
                }
            }
            //check lower right
            if (x < 6 && y < 6 && grid2[x + 1, y + 1] == "Y")
            {
                for (int z = 2; x + z < 8 && y + z < 8 && grid2[x + z, y + z] != "-"; z++)
                {
                    if (grid2[x + z, y + z] == "X")
                    {
                        for (int z3 = z; z3 >= 0; z3--)
                        {
                            tempGrid[x + z3, y + z3] = "X";
                        }

                        break;
                    }
                }
            }
            //check lower left
            if (x > 1 && y < 6 && grid2[x - 1, y + 1] == "Y")
            {
                for (int z = 2; x - z >= 0 && y + z < 8 && grid2[x - z, y + z] != "-"; z++)
                {
                    if (grid2[x - z, y + z] == "X")
                    {
                        for (int z3 = z; z3 >= 0; z3--)
                        {
                            tempGrid[x - z3, y + z3] = "X";
                        }

                        break;
                    }
                }
            }
            //check upper right
            if (x < 6 && y > 1 && grid2[x + 1, y - 1] == "Y")
            {
                for (int z = 2; x + z < 8 && y - z >= 0 && grid2[x + z, y - z] != "-"; z++)
                {
                    if (grid2[x + z, y - z] == "X")
                    {
                        for (int z3 = z; z3 >= 0; z3--)
                        {
                            tempGrid[x + z3, y - z3] = "X";
                        }

                        break;
                    }
                }
            }
            //check upper left
            if (x > 1 && y > 1 && grid2[x - 1, y - 1] == "Y")
            {
                for (int z = 2; x - z >= 0 && y - z >= 0 && grid2[x - z, y - z] != "-"; z++)
                {
                    if (grid2[x - z, y - z] == "X")
                    {
                        for (int z3 = z; z3 >= 0; z3--)
                        {
                            tempGrid[x - z3, y - z3] = "X";
                        }

                        break;
                    }
                }
            }

            outGrid = tempGrid;
            return true;
        }

        public bool checkWhiteNoMove(string[,] grid2)
        {
            bool CAN_MOVE = false;

            for (int x = 0; x < 8 && !CAN_MOVE; x++)
            {
                for (int y = 0; y < 8 && !CAN_MOVE; y++)
                {
                    if (grid2[x, y] == "-")
                    {
                        if (checkWhiteMove(grid2, x, y))
                        {
                            CAN_MOVE = true;
                        }
                    }
                }
            }

            return CAN_MOVE;
        }

        public void findMoveBlack(out int[,] moves, out int numMoves, string[,] grid)
        {
            moves = new int[32, 2];
            numMoves = 0;

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (checkBlackMove(grid, x, y))
                    {
                        moves[numMoves, 0] = x;
                        moves[numMoves, 1] = y;
                        numMoves++;
                    }
                }
            }
        }

        public void findMoveWhite(out int[,] moves, out int numMoves, string[,] grid)
        {
            moves = new int[32, 2];
            numMoves = 0;

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (checkWhiteMove(grid, x, y))
                    {
                        moves[numMoves, 0] = x;
                        moves[numMoves, 1] = y;
                        numMoves++;
                    }
                }
            }
        }

        public bool CheckCornerBlack(string[,] tempGrid, int randX, int randY)
        {
            string[,] temp = new string[8, 8];
            bool LEFT_UP = false, LEFT_DOWN = false, RIGHT_UP = false, RIGHT_DOWN = false;

            //replicate the base grid
            /*
            for (int x3 = 0; x3 < 8; x3++)
            {
                for (int y3 = 0; y3 < 8; y3++)
                {
                    temp[x3, y3] = String.Copy(tempGrid[x3, y3]);
                }
            }
            */


            temp = (string[,])tempGrid.Clone();

            if (checkWhiteNoMove(tempGrid))
            {
                return false;
            }

            if (checkBlackMove(temp, 0, 0))
            {
                LEFT_UP = true;
            }
            if (checkBlackMove(temp, 0, 7))
            {
                LEFT_DOWN = true;
            }
            if (checkBlackMove(temp, 7, 0))
            {
                RIGHT_UP = true;
            }
            if (checkBlackMove(temp, 7, 7))
            {
                RIGHT_DOWN = true;
            }

            makeWhiteMove(out temp, temp, randX, randY);

            if (checkBlackMove(temp, 0, 0) && !LEFT_UP)
            {
                return true;
            }
            else if (checkBlackMove(temp, 0, 7) && !LEFT_DOWN)
            {
                return true;
            }
            else if (checkBlackMove(temp, 7, 0) && !RIGHT_UP)
            {
                return true;
            }
            else if (checkBlackMove(temp, 7, 7) && !RIGHT_DOWN)
            {
                return true;
            }

            return false;
        }

        public bool CheckCornerWhite(string[,] tempGrid, int randX, int randY)
        {
            string[,] temp = new string[8, 8];
            bool LEFT_UP = false, LEFT_DOWN = false, RIGHT_UP = false, RIGHT_DOWN = false;

            //replicate the base grid
            /*
            for (int x3 = 0; x3 < 8; x3++)
            {
                for (int y3 = 0; y3 < 8; y3++)
                {
                    temp[x3, y3] = String.Copy(tempGrid[x3, y3]);
                }
            }
            */
            temp = (string[,])tempGrid.Clone();

            if (checkWhiteMove(temp, 0, 0))
            {
                LEFT_UP = true;
            }
            if (checkWhiteMove(temp, 0, 7))
            {
                LEFT_DOWN = true;
            }
            if (checkWhiteMove(temp, 7, 0))
            {
                RIGHT_UP = true;
            }
            if (checkWhiteMove(temp, 7, 7))
            {
                RIGHT_DOWN = true;
            }

            makeBlackMove(out temp, temp, randX, randY);

            if (checkWhiteMove(temp, 0, 0) && !LEFT_UP)
            {
                return true;
            }
            else if (checkWhiteMove(temp, 0, 7) && !LEFT_DOWN)
            {
                return true;
            }
            else if (checkWhiteMove(temp, 7, 0) && !RIGHT_UP)
            {
                return true;
            }
            else if (checkWhiteMove(temp, 7, 7) && !RIGHT_DOWN)
            {
                return true;
            }

            return false;
        }
    }
}
