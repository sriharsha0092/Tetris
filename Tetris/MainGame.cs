using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;


namespace Tetris
{
    class MainGame
    {
        #region constants
        Random rand;
        int[,] board;
        const int BOARD_HEIGHT = 20;
        const int BOARD_WIDTH = 10;
        const int BLOCK_SIZE = 40;
        Vector2 BoardPosition = Vector2.Zero;
        List<int[,]> Blocks;
        int[,] SpawnedBlock;

        int ElapsedTime;
        int StepTIme = 300;
        int KeyBoardUpdateTime = 0;

        bool GameOver = false;
        Texture2D texture;

        Vector2 SpawnedBlockPosition;

        Color[] MultiplyColors =
        {
                                    Color.Transparent,  /* 0 */
                                    Color.Orange,       /* 1 */
                                    Color.Blue,         /* 2 */
                                    Color.Red,          /* 3 */
                                    Color.LightSkyBlue, /* 4 */
                                    Color.Yellow,       /* 5 */
                                    Color.Magenta,      /* 6 */
                                    Color.LimeGreen     /* 7 */
        };

        SpriteFont font;

        public enum RotateDirections
        {
            Left,
            Right
        }

        public enum PlaceStates
        {
            CAN,
            CANT,
            FUCKED
        }


        #endregion


        #region Intialization
        public void Intialize()
        {
            rand = new Random();
            board = new int[20, 20];

            for (int x = 0; x < BOARD_HEIGHT; ++x)
                for (int y = 0; y < BOARD_WIDTH; ++y)
                    board[x, y] = 0;

            BoardPosition = Vector2.Zero;

            Blocks = new List<int[,]>();



            //Square 1
            Blocks.Add(new int[2, 2]
            {
                {1,1},
                {1,1}
            });

            //I 2

            Blocks.Add(new int[4, 4] {
                {0,0,0,0},
                {1,1,1,1},
                {0,0,0,0},
                {0,0,0,0}
            });

            //T 3

            Blocks.Add(new int[3, 3]
            {
                {1,1,1},
                {0,1,0},
                {0,0,0}
            });

            //Z 4

            Blocks.Add(new int[3, 3]
            {
                {1,1,0},
                {0,1,1},
                {0,0,0}
            });

            //S 5
            Blocks.Add(new int[3, 3]
            {
                {0,1,1},
                {1,1,0},
                {0,0,0}
            });

            //L 6
            Blocks.Add(new int[3, 3]
            {
                {1,0,0},
                {1,1,1},
                {0,0,0}
            });

            //ReverseL 7

            Blocks.Add(new int[3, 3]
            {
                {0,0,1},
                {1,1,1},
                {0,0,0}
            });

            SpawnBlock();
        }

        #endregion


        #region Content
        public void LoadContent(ContentManager Content)
        {
            texture = Content.Load<Texture2D>("boardTexture");
            font = Content.Load<SpriteFont>("font");
        }

        #endregion
        #region UpdateRegion
        public void Update(GameTime gameTime)
        {
            ElapsedTime += gameTime.ElapsedGameTime.Milliseconds;
            KeyBoardUpdateTime += gameTime.ElapsedGameTime.Milliseconds;

            

            if (ElapsedTime > StepTIme)
            {
                Vector2 NewSpawnedPieceLocation = SpawnedBlockPosition + new Vector2(0, 1);
                PlaceStates currentPlaceState = CanPlace((int)NewSpawnedPieceLocation.X, (int)NewSpawnedPieceLocation.Y, SpawnedBlock, board);

                if (currentPlaceState != PlaceStates.CAN)
                {
                    Place(SpawnedBlock, board, (int)SpawnedBlockPosition.X, (int)SpawnedBlockPosition.Y);
                    SpawnBlock();

                    currentPlaceState = CanPlace((int)SpawnedBlockPosition.X, (int)SpawnedBlockPosition.Y, SpawnedBlock, board);

                    if (currentPlaceState == PlaceStates.FUCKED)
                    {
                        Console.WriteLine("FUCKED");
                        GameOver = true;
                        
                    }

                }
                else
                {
                    SpawnedBlockPosition = NewSpawnedPieceLocation;
                }
                ElapsedTime = 0;
            }


            KeyboardState State = Keyboard.GetState();

            if (KeyBoardUpdateTime > 200)
            {

                if(State.IsKeyDown(Keys.Down))
                {
                    ElapsedTime += 175;
                }

                if (State.IsKeyDown(Keys.Left) || State.IsKeyDown(Keys.Right))
                {
                    Vector2 NewSpawnedBlockPosition = SpawnedBlockPosition + new Vector2(State.IsKeyDown(Keys.Left) ? -1 : 1, 0);
                    PlaceStates currentPlaceState = CanPlace((int)NewSpawnedBlockPosition.X, (int)NewSpawnedBlockPosition.Y, SpawnedBlock, board);
                    if (currentPlaceState == PlaceStates.CAN)
                    {
                        SpawnedBlockPosition = NewSpawnedBlockPosition;
                    }
                    KeyBoardUpdateTime = 0;
                }

                if(State.IsKeyDown(Keys.Up))
                {
                    Rotate(RotateDirections.Left, SpawnedBlock);

                    KeyBoardUpdateTime = 0;
                }

                
            }


        }





        #endregion

        #region TETRISFUNCTIONS
        void Place(int[,] block, int[,] board, int x, int y)
        {
            int dim = block.GetLength(0);

            for (int px = 0; px < dim; px++)
                for (int py = 0; py < dim; py++)
                {
                    int coordx = x + px;
                    int coordy = y + py;
                    if (block[px, py] != 0)
                    {
                        board[coordx, coordy] = block[px, py];
                    }
                }

            RemoveLines(board);

        }

        #region CANPLACEREGION
        public PlaceStates CanPlace(int x, int y, int[,] currentBlock, int[,] board)
        {
            int dim = currentBlock.GetLength(0);

            for (int px = 0; px < dim; ++px)
                for (int py = 0; py < dim; ++py)
                {
                    int coordx = x + px;
                    int coordy = y + py;

                        if (currentBlock[px, py] != 0)
                        {
                            if (coordx >= BOARD_WIDTH || coordx < 0)
                            {
                                return PlaceStates.CANT;
                            }

                            if (coordy >= BOARD_HEIGHT || board[coordx, coordy] != 0)
                                return PlaceStates.FUCKED;
                        }
                    
                }

            return PlaceStates.CAN;
        }

        #endregion

        #region SpawnBlock
        public void SpawnBlock()
        {
            int BlockType = rand.Next(0, Blocks.Count);
            SpawnedBlock = (int[,])Blocks[BlockType].Clone();

            int dim = SpawnedBlock.GetLength(0);

            for (int x = 0; x < dim; ++x)
                for (int y = 0; y < dim; ++y)
                    SpawnedBlock[x, y] *= (BlockType + 1);
            SpawnedBlockPosition = new Vector2(4, 0);

        }

        #endregion

        #region RotateRegion

        public void Rotate(RotateDirections direction, int[,] piece)
        {
            int dim = piece.GetLength(0);

             int[,] npiece = new int[dim, dim];

            for (int x = 0; x < dim; ++x)
                for (int y = 0; y < dim; ++y)
                {
                    npiece[y, x] = piece[x, dim - 1 - y];
                }

            for (int x = 0; x < dim; ++x)
                for (int y = 0; y < dim; ++y)
                {
                    piece[x, y] = npiece[x, y];
                }

            // piece = npiece;
        }


        #endregion

        #region RemovelinesRegion

        public void RemoveLines(int[,] board)
        {
            for(int y=BOARD_HEIGHT-1;y>=0;--y)
            {
                bool isRowComplete = true;

                for(int x=0;x<BOARD_WIDTH;++x)
                {
                    if(board[x,y]==0)
                    {
                        isRowComplete = false;
                    }
                }

                if(isRowComplete)
                {
                    for(int yc=y;yc>0;--yc)
                    {
                        for(int x=0;x<BOARD_WIDTH;++x)
                        {
                            board[x, yc] = board[x, yc - 1];
                        }
                    }

                    y++;
                }
            }
        }

        #endregion

        #endregion

        #region draw
        public void Draw(SpriteBatch spriteBatch)
        {
            #region DrawBoard
            for (int x = 0; x < BOARD_WIDTH; ++x)
                for (int y = 0; y < BOARD_HEIGHT; ++y)
                {
                    Color tintColor = Color.FromNonPremultiplied(50, 50, 50, 50);

                    if (board[x, y] == 0)
                        spriteBatch.Draw(texture, new Rectangle(x * BLOCK_SIZE, y * BLOCK_SIZE, BLOCK_SIZE, BLOCK_SIZE), new Rectangle(0, 0, 40, 40), tintColor);

                    #endregion

                    #region DrawPieces
                }
                    int dim = SpawnedBlock.GetLength(0);
                    for (int px = 0; px < dim; ++px)
                        for (int py = 0; py < dim; ++py)
                        {

                            if (SpawnedBlock[px, py] != 0)
                            {
                                Color titColor = MultiplyColors[SpawnedBlock[px, py]];
                                spriteBatch.Draw(texture, new Rectangle((int)(SpawnedBlockPosition.X + px) * BLOCK_SIZE, ((int)SpawnedBlockPosition.Y + py) *   BLOCK_SIZE, BLOCK_SIZE, BLOCK_SIZE),new Rectangle(0,0,40,40), titColor);
                            }
                        }

                    if (GameOver)
                    {
                        spriteBatch.DrawString(font, "GAME OVER", new Vector2(100, 300), Color.White);
                    }

                
            #endregion
        }

        #endregion
    }
}