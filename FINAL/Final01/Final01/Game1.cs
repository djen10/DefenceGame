using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;

namespace Final01
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public NetworkStream m_Stream;
        public StreamReader m_Read;
        public StreamWriter m_Write;
        const int PORT = 7718;
        private Thread m_thReader;
        //서버
        public bool m_bStop = false;
        private TcpListener m_listener;
        private Thread m_thServer;
        //클라이언트
        public bool m_bConnect = false;
        TcpClient m_Client;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int gamestate = 0;
        int select;
        bool imgflag1 = false, imgflag2 = false, imgflag3 = false, imgflag4 = false, imgflag5 = false, imgflag6 = false, imgflag7 = false, imgflag8 = false;
        bool imgflag9 = false, imgflag10 = false, imgflag11 = false;
        SpriteFont font;
        Texture2D[,] tribe;
        Texture2D[,] mode;

        int score = 0;
        int money = 500;
        int heart = 10;
        int wave = 1;
        int mopCount = 0;

        Random r = new Random(DateTime.Now.Millisecond);

        private float Timer1 = 0;
        private float Timer2 = 0;
        private float Timer3 = 0;

        private bool clickFlag = true;//포탑 아이콘 클릭용
        private int mouseindex = -1;//어떤 포탑을 클릭했는지.
        int randomNum;

        Texture2D[,] humanTexture;
        Texture2D[,] orcTexture;
        Texture2D[,] undeadTexture;
        Texture2D[,] towerTexture;
        Texture2D[,] bulletTexture;
        Texture2D heartTexture;
        Texture2D gameover;
        Texture2D complete;

        SoundEffect I_BGM;
        SoundEffect U_BGM;
        SoundEffect H_BGM;
        SoundEffect O_BGM;
        SoundEffect bombsound;
        SoundEffect clicksound;
        SoundEffect construction;
        SoundEffect squish;
        SoundEffect tom;
        SoundEffect flame;
        SoundEffect d_human1;
        SoundEffect d_human2;
        SoundEffect d_human3;
        SoundEffect d_orc1;
        SoundEffect d_orc2;
        SoundEffect d_orc3;
        SoundEffect d_undead1;
        SoundEffect d_undead2;
        SoundEffect d_undead3;
        SoundEffect siren;
        SoundEffect ouch;
        SoundEffect build;
        SoundEffect lose;
        SoundEffect stage_s;
        SoundEffect fanfare;


        SoundEffectInstance h_bgm;
        SoundEffectInstance o_bgm;
        SoundEffectInstance u_bgm;


        SoundEffectInstance BOMB;
        SoundEffectInstance FLAME;
        SoundEffectInstance SQUISH;
        int m_click=0;//마우스 클릭사운드 플래그
        int m_on = 0;//마우스 올려놓을때 사운드플래그


        List<Unit> units = new List<Unit>();
        List<Tower> towers = new List<Tower>();

        Map myMap;

        Texture2D[] stage;
        Texture2D[] attack;
        bool temp = true;
        int i = 64 * 15;
        bool selectmode = true;

        #region 맵 종류 정의 부분
        private int[,] map0 = new int[,]
        {
            {1,1,1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1,1,1}
        };
        private int[,] map1 = new int[,]
        {
            {1,1,1,1,1,1,1,1,1,1,1,1},
            {0,0,0,1,1,1,1,1,1,1,1,1},
            {1,1,0,0,0,1,1,1,1,1,1,1},
            {1,1,1,1,0,1,1,1,1,1,1,1},
            {1,1,1,1,0,0,0,0,0,0,0,2},
            {1,1,1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1,1,1}
        };
        private int[,] map2 = new int[,]
        {
            {0,0,0,0,0,0,0,0,0,0,0,0},
            {1,1,1,1,1,1,1,1,1,1,1,0},
            {1,0,0,0,0,0,0,0,0,0,1,0},
            {1,0,1,1,1,1,1,1,1,0,1,0},
            {1,0,1,2,1,1,1,1,1,0,1,0},
            {1,0,1,0,0,0,0,0,0,0,1,0},
            {1,0,1,1,1,1,1,1,1,1,1,0},
            {1,0,0,0,0,0,0,0,0,0,0,0}
        };
        private int[,] map3 = new int[,]
        {
            {1,1,1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1,1,1},
            {1,1,1,0,0,0,1,1,1,1,1,1},
            {1,1,1,0,1,0,1,1,1,1,1,1},
            {1,1,1,0,1,0,1,1,1,1,1,1},
            {1,1,1,0,1,0,0,0,0,0,0,2},
            {0,0,0,0,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1,1,1}
        };
        private int[,] map4 = new int[,]
        {
            {1,1,1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1,1,1},
            {1,1,1,0,0,0,0,0,0,0,0,2},
            {1,1,1,0,1,1,1,1,1,1,1,1},
            {1,1,1,0,1,1,1,1,1,1,1,1},
            {0,0,0,0,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1,1,1}
        };
        private int[,] map5 = new int[,]
        {
            {1,1,1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1,1,1},
            {0,0,0,0,0,0,0,0,0,0,0,2},
            {1,1,1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1,1,1}
        };
        #endregion

        public Game1()
        {

            myMap = new Map(map3, 0, 6, 3);
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // The width of the stage in pixels
            graphics.PreferredBackBufferWidth = (myMap.Width) * 64;
            // The height of the toolbar + the height of the stage in pixels
            graphics.PreferredBackBufferHeight = (myMap.Height) * 64;

            IsMouseVisible = true;
        }
        public void ServerStart()
        {
            try
            {
                m_listener = new TcpListener(PORT);
                m_listener.Start();

                Socket hCilent = m_listener.AcceptSocket();
                if (hCilent.Connected)
                {

                    m_bConnect = true;
                    m_Stream = new NetworkStream(hCilent);
                    m_Read = new StreamReader(m_Stream);
                    m_Write = new StreamWriter(m_Stream);
                    m_thReader = new Thread(new ThreadStart(Receive));
                    m_thReader.Start();
                }

            }
            catch
            {
                return;
            }
        }
        public void ServerStop()
        {
            if (m_bStop)
                return;

            m_listener.Stop();
            m_Read.Close();
            m_Write.Close();
            m_Stream.Close();
            m_thReader.Abort();
            m_thServer.Abort();
        }
        public void Coneect()
        {
            m_Client = new TcpClient();

            try
            {
                m_Client.Connect("localhost", PORT);
            }
            catch
            {
                m_bConnect = false;
                return;
            }
            m_bConnect = true;

            m_Stream = m_Client.GetStream();

            m_Read = new StreamReader(m_Stream);
            m_Write = new StreamWriter(m_Stream);
            m_thReader = new Thread(new ThreadStart(Receive));
            m_thReader.Start();
        }
        public void Disconnect()
        {
            if (!m_bConnect)
                return;
            m_bConnect = false;
            m_Read.Close();
            m_Write.Close();
            m_Stream.Close();
            m_thReader.Abort();
        }
        public void Receive()
        {
            try
            {
                while (true)
                {
                    //여기에서 몬스터 정보를 받아야 한다.
                    string UnitInfo = m_Read.ReadLine();
                    siren.Play();
                    if (UnitInfo != null)
                    {
                        if (UnitInfo == "51")
                        {
                            units.Add(new Unit(100, 0.018f, humanTexture[0, 0], humanTexture[0, 1], myMap, d_human1));
                        }
                        if (UnitInfo == "81")
                        {
                            units.Add(new Unit(150, 0.04f, humanTexture[1, 0], humanTexture[1, 1], myMap, d_human2));
                        }
                        if (UnitInfo == "71")
                        {
                            units.Add(new Unit(80, 0.0f, humanTexture[2, 0], humanTexture[2, 1], myMap, d_human3));
                        }
                        if (UnitInfo == "52")
                        {
                            units.Add(new Unit(100, 0.018f, orcTexture[0, 0], orcTexture[0, 1], myMap, d_orc1));
                        }
                        if (UnitInfo == "82")
                        {
                            units.Add(new Unit(150, 0.04f, orcTexture[1, 0], orcTexture[1, 1], myMap, d_orc2));
                        }
                        if (UnitInfo == "72")
                        {
                            units.Add(new Unit(80, 0.0f, orcTexture[2, 0], orcTexture[2, 1], myMap, d_orc3));
                        }
                        if (UnitInfo == "53")
                        {
                            units.Add(new Unit(100, 0.018f, undeadTexture[0, 0], undeadTexture[0, 1], myMap, d_undead1));
                        }
                        if (UnitInfo == "83")
                        {
                            units.Add(new Unit(150, 0.04f, undeadTexture[1, 0], undeadTexture[1, 1], myMap, d_undead2));
                        }
                        if (UnitInfo == "73")
                        {
                            units.Add(new Unit(80, 0.0f, undeadTexture[2, 0], undeadTexture[2, 1], myMap, d_undead3));
                        }
                    }

                }
            }
            catch
            {

            }
            Disconnect();
        }
        public void btn_Server_Click()
        {
            ServerStart();

        }
        public void btn_Connect_Click()
        {
            Coneect();
        }
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
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            stage = new Texture2D[5];
            stage[0] = Content.Load<Texture2D>("font\\stage1");
            stage[1] = Content.Load<Texture2D>("font\\stage2");
            stage[2] = Content.Load<Texture2D>("font\\stage3");
            stage[3] = Content.Load<Texture2D>("font\\stage4");
            stage[4] = Content.Load<Texture2D>("font\\stage5");
            attack = new Texture2D[3];
            attack[0] = Content.Load<Texture2D>("font\\Attack Human");
            attack[1] = Content.Load<Texture2D>("font\\Attack Orc");
            attack[2] = Content.Load<Texture2D>("font\\Attack Undead");

            I_BGM = Content.Load<SoundEffect>("media\\I_BGM");


            U_BGM = Content.Load<SoundEffect>("media\\U_BGM");
            H_BGM = Content.Load<SoundEffect>("media\\H_BGM");
            O_BGM = Content.Load<SoundEffect>("media\\O_BGM");
            bombsound = Content.Load<SoundEffect>("media\\bomb");
            clicksound = Content.Load<SoundEffect>("media\\click");
            construction = Content.Load<SoundEffect>("media\\construction");
            squish = Content.Load<SoundEffect>("media\\SQUISH11");
            tom = Content.Load<SoundEffect>("media\\tom2");
            flame = Content.Load<SoundEffect>("media\\flameburst1");
            build = Content.Load<SoundEffect>("media\\build");

            d_human1 = Content.Load<SoundEffect>("media\\human1");
            d_human2 = Content.Load<SoundEffect>("media\\human2");
            d_human3 = Content.Load<SoundEffect>("media\\human3");
            d_orc1 = Content.Load<SoundEffect>("media\\orc1");
            d_orc2 = Content.Load<SoundEffect>("media\\orc2");
            d_orc3 = Content.Load<SoundEffect>("media\\orc3");
            d_undead1 = Content.Load<SoundEffect>("media\\undead1");
            d_undead2 = Content.Load<SoundEffect>("media\\undead2");
            d_undead3 = Content.Load<SoundEffect>("media\\undead3");
            siren = Content.Load<SoundEffect>("media\\sirens");
            ouch = Content.Load<SoundEffect>("media\\ouch");

            stage_s= Content.Load<SoundEffect>("media\\STAGE");
            lose = Content.Load<SoundEffect>("media\\LOSE");
            fanfare = Content.Load<SoundEffect>("media\\fanfare");

            h_bgm = H_BGM.CreateInstance();
            u_bgm = U_BGM.CreateInstance();
            o_bgm = O_BGM.CreateInstance();


            I_BGM.Play();
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            font = Content.Load<SpriteFont>("font\\Arial");
            tribe = new Texture2D[,]
            {
                {
                    Content.Load<Texture2D>("tribe\\human1"),
                    Content.Load<Texture2D>("tribe\\human2")
                },
                {
                    Content.Load<Texture2D>("tribe\\orc1"),
                    Content.Load<Texture2D>("tribe\\orc2")
                },
                {
                    Content.Load<Texture2D>("tribe\\undead1"),
                    Content.Load<Texture2D>("tribe\\undead2")
                }
            };
            mode = new Texture2D[,]
            {
                {
                    Content.Load<Texture2D>("modebutton\\single"),
                    Content.Load<Texture2D>("modebutton\\singleover")
                },
                {
                    Content.Load<Texture2D>("modebutton\\multi"),
                    Content.Load<Texture2D>("modebutton\\multiover")
                }
            };


            myMap.addTexture(Content.Load<Texture2D>("map\\road"));//0
            myMap.addTexture(Content.Load<Texture2D>("map\\land"));//1
            myMap.addTexture(Content.Load<Texture2D>("map\\end"));//2
            myMap.addTexture(Content.Load<Texture2D>("map\\under_human"));//3
            myMap.addTexture(Content.Load<Texture2D>("map\\right_human"));//4
            myMap.addTexture(Content.Load<Texture2D>("map\\under_orc"));//5
            myMap.addTexture(Content.Load<Texture2D>("map\\right_orc"));//6
            myMap.addTexture(Content.Load<Texture2D>("map\\under_undead"));//7
            myMap.addTexture(Content.Load<Texture2D>("map\\right_undead"));//8

            gameover = Content.Load<Texture2D>("map\\gameover");
            complete = Content.Load<Texture2D>("map\\complete");

            #region 유닛 텍스쳐 지정
            humanTexture = new Texture2D[,]
            {
                {
                    Content.Load<Texture2D>("unit\\human1_1"),
                    Content.Load<Texture2D>("unit\\human1_2"),
                    Content.Load<Texture2D>("unit\\human1_3")
                },
                {
                    Content.Load<Texture2D>("unit\\human2_1"),
                    Content.Load<Texture2D>("unit\\human2_2"),
                    Content.Load<Texture2D>("unit\\human2_3")
                },
                {
                    Content.Load<Texture2D>("unit\\human3_1"),
                    Content.Load<Texture2D>("unit\\human3_2"),
                    Content.Load<Texture2D>("unit\\human3_3")
                }
            };
            orcTexture = new Texture2D[,]
            {
                {
                    Content.Load<Texture2D>("unit\\orc1_1"),
                    Content.Load<Texture2D>("unit\\orc1_2"),
                    Content.Load<Texture2D>("unit\\orc1_3")
                },
                {
                    Content.Load<Texture2D>("unit\\orc2_1"),
                    Content.Load<Texture2D>("unit\\orc2_2"),
                    Content.Load<Texture2D>("unit\\orc2_3")
                },
                {
                    Content.Load<Texture2D>("unit\\orc3_1"),
                    Content.Load<Texture2D>("unit\\orc3_2"),
                    Content.Load<Texture2D>("unit\\orc3_3")
                }
            };
            undeadTexture = new Texture2D[,]
            {
                {
                    Content.Load<Texture2D>("unit\\undead1_1"),
                    Content.Load<Texture2D>("unit\\undead1_2"),
                    Content.Load<Texture2D>("unit\\undead1_3")
                },
                {
                    Content.Load<Texture2D>("unit\\undead2_1"),
                    Content.Load<Texture2D>("unit\\undead2_2"),
                    Content.Load<Texture2D>("unit\\undead2_3")
                },
                {
                    Content.Load<Texture2D>("unit\\undead3_1"),
                    Content.Load<Texture2D>("unit\\undead3_2"),
                    Content.Load<Texture2D>("unit\\undead3_3")
                }
            };
            #endregion

            towerTexture = new Texture2D[,]
            {
                {
                    Content.Load<Texture2D>("tower\\tower1_1"),
                    Content.Load<Texture2D>("tower\\tower1_1over")
                },
                {
                    Content.Load<Texture2D>("tower\\tower2_1"),
                    Content.Load<Texture2D>("tower\\tower2_1over")
                },
                {
                    Content.Load<Texture2D>("tower\\tower3_1"),
                    Content.Load<Texture2D>("tower\\tower3_1over")
                }
            };//타워 종류별, 레벨별 텍스쳐 지정

            bulletTexture = new Texture2D[,]
            {
                {
                    Content.Load<Texture2D>("bullet\\bullet1"),
                    Content.Load<Texture2D>("bullet\\bullet1_1")
                },
                {
                    Content.Load<Texture2D>("bullet\\bullet2"),
                    Content.Load<Texture2D>("bullet\\bullet2_1")
                },
                {
                    Content.Load<Texture2D>("bullet\\bullet3"),
                    Content.Load<Texture2D>("bullet\\bullet3_1")
                }
            };

            heartTexture = Content.Load<Texture2D>("map\\heart");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            //얘는 시작화면 버튼들
            Rectangle s_human = new Rectangle(64 * 1, 64 * 1, 192, 640);
            Rectangle s_ork = new Rectangle(64 * 5, 64 * 1, 192, 640);
            Rectangle s_undead = new Rectangle(64 * 9, 64 * 1, 192, 640);
            Rectangle singlemode = new Rectangle(64 * 12 + 32, 64 * 2, 128, 128);
            Rectangle networkmode = new Rectangle(64 * 12 + 32, 64 * 6, 128, 128);
            Rectangle networkmode1 = new Rectangle(64 * 12 + 32, 64 * 6, 64, 64);
            Rectangle networkmode2 = new Rectangle(64 * 13 + 32, 64 * 6, 64, 64);

            Rectangle mo = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 10, 10);

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && m_click == 1)//마우스클릭
            {
                clicksound.Play();
                m_click = 0;
            }
            if (Mouse.GetState().LeftButton == ButtonState.Released)
                m_click = 1;

            if (gamestate == 0)
            {
                //휴먼
                if (Mouse.GetState().LeftButton == ButtonState.Released && mo.Intersects(s_human))
                {
                    if (m_on == 1)
                    {
                        tom.Play();
                        m_on = 0;
                    }
                    imgflag1 = true;
                }
                else
                {
                    if (!mo.Intersects(s_human)&&!mo.Intersects(s_ork) && !mo.Intersects(s_undead) &&
                                                       !mo.Intersects(singlemode) && !mo.Intersects(networkmode))
                        m_on = 1;
                    imgflag1 = false;

                }
                //오크
                if (Mouse.GetState().LeftButton == ButtonState.Released && mo.Intersects(s_ork))
                {
                    if (m_on == 1)
                    {
                        tom.Play();
                        m_on = 0;
                    }
                    imgflag2 = true;
                }
                else
                {
                    if (!mo.Intersects(s_human)&&!mo.Intersects(s_ork) && !mo.Intersects(s_undead) &&
                                      !mo.Intersects(singlemode) && !mo.Intersects(networkmode))
                        m_on = 1;
                    imgflag2 = false;
                }
                //언데드
                if (Mouse.GetState().LeftButton == ButtonState.Released && mo.Intersects(s_undead))
                {
                    if (m_on == 1)
                    {
                        tom.Play();
                        m_on = 0;
                    }
                    imgflag3 = true;
                }
                else
                {
                    if (!mo.Intersects(s_human) && !mo.Intersects(s_ork) && !mo.Intersects(s_undead) &&
                  !mo.Intersects(singlemode) && !mo.Intersects(networkmode))
                        m_on = 1;
                    imgflag3 = false;
                }
                if (Mouse.GetState().LeftButton == ButtonState.Released && mo.Intersects(singlemode))
                {
                    if (m_on == 1)
                    {
                        tom.Play();
                        m_on = 0;
                    }
                    imgflag4 = true;
                }
                else
                {
                    if (!mo.Intersects(s_human) && !mo.Intersects(s_ork) && !mo.Intersects(s_undead) &&
                  !mo.Intersects(singlemode) && !mo.Intersects(networkmode))
                        m_on = 1;
                    imgflag4 = false;
                }
                if (Mouse.GetState().LeftButton == ButtonState.Released && mo.Intersects(networkmode))
                {
                    if (m_on == 1)
                    {
                        tom.Play();
                        m_on = 0;
                    }
                    imgflag5 = true;
                }
                else
                {
                    if (!mo.Intersects(s_human) && !mo.Intersects(s_ork) && !mo.Intersects(s_undead) &&
!mo.Intersects(singlemode) && !mo.Intersects(networkmode))
                        m_on = 1;
                    imgflag5 = false;
                }

                //이거는 눌렀을때 게임 시작. 선택한 아이의 정보도 알려주어야함.
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && mo.Intersects(s_human))
                {
                    select = 1;
                }
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && mo.Intersects(s_ork))
                {
                    select = 2;
                }
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && mo.Intersects(s_undead))
                {
                    select = 3;
                }
                //이제 얘는 게임시작. 모드 2개 싱글모드 ㅁ네트워크모드
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && mo.Intersects(singlemode))
                {
                    if (select != 0)
                    {
                        gamestate = 1;
                        wave = 1;
                        randomNum = r.Next(0, 2);
                        Thread.Sleep(80);
                        selectmode = true;
                    }
                }
                else if (Mouse.GetState().LeftButton == ButtonState.Pressed && mo.Intersects(networkmode1))
                {
                    if (select != 0)
                    {
                        gamestate = 1;

                        myMap.map = myMap.getUI(map3, 0, 6, select);
                        ServerStart();
                        Thread.Sleep(80);
                        selectmode = false;
                    }
                }
                else if (Mouse.GetState().LeftButton == ButtonState.Pressed && mo.Intersects(networkmode2))
                {
                    if (select != 0)
                    {
                        gamestate = 1;
                        Coneect();
                        myMap.map = myMap.getUI(map2, 0, 0, select);
                        Thread.Sleep(80);
                        selectmode = false;
                    }
                }
            }

            else if (gamestate == 1)
            {
                I_BGM.Dispose();

                Timer1 += (float)gameTime.ElapsedGameTime.TotalSeconds;
                Timer2 += (float)gameTime.ElapsedGameTime.TotalSeconds;
                Timer3 += (float)gameTime.ElapsedGameTime.TotalSeconds;

                foreach (Unit x in units)
                {
                    int GETM = x.getMoney();
                    money += GETM;
                    score += x.getScore();
                    if (m_bConnect)
                    {
                        if (GETM == 50 || GETM == 80 || GETM == 70)
                        {
                            siren.Play();
                            m_Write.WriteLine(Convert.ToString(GETM + select));
                            m_Write.Flush();
                        }
                    }
                    if (x.getAttack())
                    {
                        ouch.Play();
                        if (--heart == 0)
                        {
                            if (select == 1)
                                h_bgm.Dispose();
                            if (select == 2)
                                o_bgm.Dispose();
                            if (select == 3)
                                u_bgm.Dispose();
                            lose.Play();
                            gamestate = 2;
                        }
                        
                    }
                    x.Update(gameTime);
                }
                foreach (Tower y in towers)
                {
                    if(y.target==null)
                        y.getTarget(units);
                    y.Update(gameTime);
                }

                #region wave1
                if (wave == 1)
                {
                    bool nextFlag = false;
                    foreach(Unit u in units)
                    {
                        
                        if (!u.saveFlag && mopCount >= 12)
                            nextFlag = true;
                        if (u.saveFlag)
                            nextFlag = false;

                    }
                    if (nextFlag)
                    {
                        wave = 2;
                        Timer1 = 0;
                        Timer2 = 0;
                        Timer3 = 0;
                        randomNum = r.Next(0, 2);
                        mopCount = 0;
                        towers.Clear();
                        units.Clear();
                        money += 500;
                        temp = true;
                    }

                    myMap.map = myMap.getUI(map3, 0, 6, select);//웨이브당 맵 매핑

                    if(mopCount < 12)
                    {
                        if (select == 1)//내가 인간
                        {
                            if (randomNum == 0)//오크 소환
                            {
                                if (Timer1 > 5.0f)
                                {
                                    units.Add(new Unit(120, 0.018f, orcTexture[0, 0], orcTexture[0, 1], myMap,d_orc1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 10.3f)
                                {
                                    units.Add(new Unit(180, 0.04f, orcTexture[1, 0], orcTexture[1, 1], myMap, d_orc2));
                                    mopCount++;
                                    Timer2 = 0;
                                }/*
                            if (Timer3 > 8.5f)
                            {
                                units.Add(new Unit(80, 0.0f, humanTexture[2, 0], humanTexture[2, 1], myMap));
                                Timer3 = 0;
                            }*/
                            }
                            else//언데드 소환
                            {
                                if (Timer1 > 5.0f)
                                {
                                    units.Add(new Unit(120, 0.018f, undeadTexture[0, 0], undeadTexture[0, 1], myMap, d_undead1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 10.3f)
                                {
                                    units.Add(new Unit(180, 0.04f, undeadTexture[1, 0], undeadTexture[1, 1], myMap, d_undead2));
                                    mopCount++;
                                    Timer2 = 0;
                                }
                            }
                        }
                        else if (select == 2)//내가 오크
                        {
                            if (randomNum == 0)//인간 소환
                            {
                                if (Timer1 > 5.0f)
                                {
                                    units.Add(new Unit(120, 0.018f, humanTexture[0, 0], humanTexture[0, 1], myMap, d_human1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 10.3f)
                                {
                                    units.Add(new Unit(180, 0.04f, humanTexture[1, 0], humanTexture[1, 1], myMap, d_human2));
                                    mopCount++;
                                    Timer2 = 0;
                                }
                            }
                            else//언데드 소환
                            {
                                if (Timer1 > 5.0f)
                                {
                                    units.Add(new Unit(120, 0.018f, undeadTexture[0, 0], undeadTexture[0, 1], myMap, d_undead1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 10.3f)
                                {
                                    units.Add(new Unit(180, 0.04f, undeadTexture[1, 0], undeadTexture[1, 1], myMap, d_undead2));
                                    mopCount++;
                                    Timer2 = 0;
                                }
                            }
                        }
                        else if (select == 3)//내가 언데드
                        {
                            if (randomNum == 0)//인간 소환
                            {
                                if (Timer1 > 5.0f)
                                {
                                    units.Add(new Unit(120, 0.018f, humanTexture[0, 0], humanTexture[0, 1], myMap, d_human1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 10.3f)
                                {
                                    units.Add(new Unit(180, 0.04f, humanTexture[1, 0], humanTexture[1, 1], myMap, d_human2));
                                    mopCount++;
                                    Timer2 = 0;
                                }
                            }
                            else//오크 소환
                            {
                                if (Timer1 > 5.0f)
                                {
                                    units.Add(new Unit(120, 0.018f, orcTexture[0, 0], orcTexture[0, 1], myMap, d_orc1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 10.3f)
                                {
                                    units.Add(new Unit(180, 0.04f, orcTexture[1, 0], orcTexture[1, 1], myMap, d_orc2));
                                    mopCount++;
                                    Timer2 = 0;
                                }
                            }
                        }
                    }
                }
                #endregion

                #region wave 2
                else if (wave == 2)
                {
                    bool nextFlag = false;
                    foreach (Unit u in units)
                    {
                        if (!u.saveFlag && mopCount >= 20)
                            nextFlag = true;
                        if (u.saveFlag)
                            nextFlag = false;

                    }
                    if (nextFlag)
                    {
                        wave = 3;
                        Timer1 = 0;
                        Timer2 = 0;
                        Timer3 = 0;
                        randomNum = r.Next(0, 2);
                        mopCount = 0;
                        towers.Clear();
                        units.Clear();
                        money += 1000;
                        temp = true;
                    }

                    myMap.map = myMap.getUI(map2, 0, 0, select);//웨이브당 맵 매핑

                    if (mopCount < 20)
                    {
                        if (select == 1)//내가 인간
                        {
                            if (randomNum == 0)//오크 소환
                            {
                                if (Timer1 > 5.0f)
                                {
                                    units.Add(new Unit(120, 0.018f, orcTexture[0, 0], orcTexture[0, 1], myMap, d_orc1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 10.3f)
                                {
                                    units.Add(new Unit(180, 0.04f, orcTexture[1, 0], orcTexture[1, 1], myMap, d_orc2));
                                    mopCount++;
                                    Timer2 = 0;
                                }
                                if (Timer3 > 8.5f)
                                {
                                    units.Add(new Unit(80, 0.0f, orcTexture[2, 0], orcTexture[2, 1], myMap, d_orc3));
                                    mopCount++;
                                    Timer3 = 0;
                                }
                            }
                            else//언데드 소환
                            {
                                if (Timer1 > 5.0f)
                                {
                                    units.Add(new Unit(120, 0.018f, undeadTexture[0, 0], undeadTexture[0, 1], myMap, d_undead1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 10.3f)
                                {
                                    units.Add(new Unit(180, 0.04f, undeadTexture[1, 0], undeadTexture[1, 1], myMap, d_undead2));
                                    mopCount++;
                                    Timer2 = 0;
                                }
                                if (Timer3 > 8.5f)
                                {
                                    units.Add(new Unit(80, 0.0f, undeadTexture[2, 0], undeadTexture[2, 1], myMap, d_undead3));
                                    mopCount++;
                                    Timer3 = 0;
                                }
                            }
                        }
                        else if (select == 2)//내가 오크
                        {
                            if (randomNum == 0)//인간 소환
                            {
                                if (Timer1 > 5.0f)
                                {
                                    units.Add(new Unit(120, 0.018f, humanTexture[0, 0], humanTexture[0, 1], myMap, d_human1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 10.3f)
                                {
                                    units.Add(new Unit(180, 0.04f, humanTexture[1, 0], humanTexture[1, 1], myMap, d_human2));
                                    mopCount++;
                                    Timer2 = 0;
                                }
                                if (Timer3 > 8.5f)
                                {
                                    units.Add(new Unit(80, 0.0f, humanTexture[2, 0], humanTexture[2, 1], myMap, d_human3));
                                    mopCount++;
                                    Timer3 = 0;
                                }
                            }
                            else//언데드 소환
                            {
                                if (Timer1 > 5.0f)
                                {
                                    units.Add(new Unit(120, 0.018f, undeadTexture[0, 0], undeadTexture[0, 1], myMap, d_undead1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 10.3f)
                                {
                                    units.Add(new Unit(180, 0.04f, undeadTexture[1, 0], undeadTexture[1, 1], myMap, d_undead2));
                                    mopCount++;
                                    Timer2 = 0;
                                }
                                if (Timer3 > 8.5f)
                                {
                                    units.Add(new Unit(80, 0.0f, undeadTexture[2, 0], undeadTexture[2, 1], myMap, d_undead3));
                                    mopCount++;
                                    Timer3 = 0;
                                }
                            }
                        }
                        else if (select == 3)//내가 언데드
                        {
                            if (randomNum == 0)//인간 소환
                            {
                                if (Timer1 > 5.0f)
                                {
                                    units.Add(new Unit(120, 0.018f, humanTexture[0, 0], humanTexture[0, 1], myMap, d_human1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 10.3f)
                                {
                                    units.Add(new Unit(180, 0.04f, humanTexture[1, 0], humanTexture[1, 1], myMap, d_human2));
                                    mopCount++;
                                    Timer2 = 0;
                                }
                                if (Timer3 > 8.5f)
                                {
                                    units.Add(new Unit(80, 0.0f, humanTexture[2, 0], humanTexture[2, 1], myMap, d_human3));
                                    mopCount++;
                                    Timer3 = 0;
                                }
                            }
                            else//오크 소환
                            {
                                if (Timer1 > 5.0f)
                                {
                                    units.Add(new Unit(120, 0.018f, orcTexture[0, 0], orcTexture[0, 1], myMap, d_orc1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 10.3f)
                                {
                                    units.Add(new Unit(180, 0.04f, orcTexture[1, 0], orcTexture[1, 1], myMap, d_orc2));
                                    mopCount++;
                                    Timer2 = 0;
                                }
                                if (Timer3 > 8.5f)
                                {
                                    units.Add(new Unit(80, 0.0f, orcTexture[2, 0], orcTexture[2, 1], myMap, d_orc3));
                                    mopCount++;
                                    Timer3 = 0;
                                }
                            }
                        }
                    }
                }
                #endregion

                #region wave 3
                else if (wave == 3)
                {
                    bool nextFlag = false;
                    foreach (Unit u in units)
                    {
                        if (!u.saveFlag && mopCount >= 30)
                            nextFlag = true;
                        if (u.saveFlag)
                            nextFlag = false;

                    }
                    if (nextFlag)
                    {
                        wave = 4;
                        Timer1 = 0;
                        Timer2 = 0;
                        Timer3 = 0;
                        randomNum = r.Next(0, 2);
                        mopCount = 0;
                        towers.Clear();
                        units.Clear();
                        money += 1500;
                        temp = true;
                    }

                    myMap.map = myMap.getUI(map1, 0, 1, select);//웨이브당 맵 매핑

                    if (mopCount < 30)
                    {
                        if (select == 1)//내가 인간
                        {
                            if (randomNum == 0)//오크 소환
                            {
                                if (Timer1 > 4.5f)
                                {
                                    units.Add(new Unit(120, 0.018f, orcTexture[0, 0], orcTexture[0, 1], myMap, d_orc1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 9.3f)
                                {
                                    units.Add(new Unit(180, 0.04f, orcTexture[1, 0], orcTexture[1, 1], myMap, d_orc2));
                                    mopCount++;
                                    Timer2 = 0;
                                }
                                if (Timer3 > 7.5f)
                                {
                                    units.Add(new Unit(80, 0.0f, orcTexture[2, 0], orcTexture[2, 1], myMap, d_orc3));
                                    mopCount++;
                                    Timer3 = 0;
                                }
                            }
                            else//언데드 소환
                            {
                                if (Timer1 > 4.5f)
                                {
                                    units.Add(new Unit(120, 0.018f, undeadTexture[0, 0], undeadTexture[0, 1], myMap, d_undead1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 9.3f)
                                {
                                    units.Add(new Unit(180, 0.04f, undeadTexture[1, 0], undeadTexture[1, 1], myMap, d_undead2));
                                    mopCount++;
                                    Timer2 = 0;
                                }
                                if (Timer3 > 7.5f)
                                {
                                    units.Add(new Unit(80, 0.0f, undeadTexture[2, 0], undeadTexture[2, 1], myMap, d_undead3));
                                    mopCount++;
                                    Timer3 = 0;
                                }
                            }
                        }
                        else if (select == 2)//내가 오크
                        {
                            if (randomNum == 0)//인간 소환
                            {
                                if (Timer1 > 4.5f)
                                {
                                    units.Add(new Unit(120, 0.018f, humanTexture[0, 0], humanTexture[0, 1], myMap, d_human1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 9.3f)
                                {
                                    units.Add(new Unit(180, 0.04f, humanTexture[1, 0], humanTexture[1, 1], myMap, d_human2));
                                    mopCount++;
                                    Timer2 = 0;
                                }
                                if (Timer3 > 7.5f)
                                {
                                    units.Add(new Unit(80, 0.0f, humanTexture[2, 0], humanTexture[2, 1], myMap, d_human3));
                                    mopCount++;
                                    Timer3 = 0;
                                }
                            }
                            else//언데드 소환
                            {
                                if (Timer1 > 4.5f)
                                {
                                    units.Add(new Unit(120, 0.018f, undeadTexture[0, 0], undeadTexture[0, 1], myMap, d_undead1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 9.3f)
                                {
                                    units.Add(new Unit(180, 0.04f, undeadTexture[1, 0], undeadTexture[1, 1], myMap, d_undead2));
                                    mopCount++;
                                    Timer2 = 0;
                                }
                                if (Timer3 > 7.5f)
                                {
                                    units.Add(new Unit(80, 0.0f, undeadTexture[2, 0], undeadTexture[2, 1], myMap, d_undead3));
                                    mopCount++;
                                    Timer3 = 0;
                                }
                            }
                        }
                        else if (select == 3)//내가 언데드
                        {
                            if (randomNum == 0)//인간 소환
                            {
                                if (Timer1 > 4.5f)
                                {
                                    units.Add(new Unit(120, 0.018f, humanTexture[0, 0], humanTexture[0, 1], myMap, d_human1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 9.3f)
                                {
                                    units.Add(new Unit(180, 0.04f, humanTexture[1, 0], humanTexture[1, 1], myMap, d_human2));
                                    mopCount++;
                                    Timer2 = 0;
                                }
                                if (Timer3 > 7.5f)
                                {
                                    units.Add(new Unit(80, 0.0f, humanTexture[2, 0], humanTexture[2, 1], myMap, d_human3));
                                    mopCount++;
                                    Timer3 = 0;
                                }
                            }
                            else//오크 소환
                            {
                                if (Timer1 > 4.5f)
                                {
                                    units.Add(new Unit(120, 0.018f, orcTexture[0, 0], orcTexture[0, 1], myMap, d_orc1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 9.3f)
                                {
                                    units.Add(new Unit(180, 0.04f, orcTexture[1, 0], orcTexture[1, 1], myMap, d_orc2));
                                    mopCount++;
                                    Timer2 = 0;
                                }
                                if (Timer3 > 7.5f)
                                {
                                    units.Add(new Unit(80, 0.0f, orcTexture[2, 0], orcTexture[2, 1], myMap, d_orc3));
                                    mopCount++;
                                    Timer3 = 0;
                                }
                            }
                        }
                    }
                }
                #endregion

                #region wave 4
                else if (wave == 4)
                {
                    bool nextFlag = false;
                    foreach (Unit u in units)
                    {
                        if (!u.saveFlag && mopCount >= 45)
                            nextFlag = true;
                        if (u.saveFlag)
                            nextFlag = false;

                    }
                    if (nextFlag)
                    {
                        wave = 5;
                        Timer1 = 0;
                        Timer2 = 0;
                        Timer3 = 0;
                        randomNum = r.Next(0, 2);
                        mopCount = 0;
                        towers.Clear();
                        units.Clear();
                        money += 1500;
                        temp = true;
                    }

                    myMap.map = myMap.getUI(map4, 0, 5, select);//웨이브당 맵 매핑

                    if (mopCount < 45)
                    {
                        if (select == 1)//내가 인간
                        {
                            if (randomNum == 0)//오크 소환
                            {
                                if (Timer1 > 4.0f)
                                {
                                    units.Add(new Unit(150, 0.018f, orcTexture[0, 0], orcTexture[0, 1], myMap, d_orc1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 8.3f)
                                {
                                    units.Add(new Unit(200, 0.04f, orcTexture[1, 0], orcTexture[1, 1], myMap, d_orc2));
                                    mopCount++;
                                    Timer2 = 0;
                                }
                                if (Timer3 > 7.0f)
                                {
                                    units.Add(new Unit(100, 0.0f, orcTexture[2, 0], orcTexture[2, 1], myMap, d_orc3));
                                    mopCount++;
                                    Timer3 = 0;
                                }
                            }
                            else//언데드 소환
                            {
                                if (Timer1 > 4.0f)
                                {
                                    units.Add(new Unit(150, 0.018f, undeadTexture[0, 0], undeadTexture[0, 1], myMap, d_undead1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 8.3f)
                                {
                                    units.Add(new Unit(200, 0.04f, undeadTexture[1, 0], undeadTexture[1, 1], myMap, d_undead2));
                                    mopCount++;
                                    Timer2 = 0;
                                }
                                if (Timer3 > 7.0f)
                                {
                                    units.Add(new Unit(100, 0.0f, undeadTexture[2, 0], undeadTexture[2, 1], myMap, d_undead3));
                                    mopCount++;
                                    Timer3 = 0;
                                }
                            }
                        }
                        else if (select == 2)//내가 오크
                        {
                            if (randomNum == 0)//인간 소환
                            {
                                if (Timer1 > 4.0f)
                                {
                                    units.Add(new Unit(150, 0.018f, humanTexture[0, 0], humanTexture[0, 1], myMap, d_human1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 8.3f)
                                {
                                    units.Add(new Unit(200, 0.04f, humanTexture[1, 0], humanTexture[1, 1], myMap, d_human2));
                                    mopCount++;
                                    Timer2 = 0;
                                }
                                if (Timer3 > 7.0f)
                                {
                                    units.Add(new Unit(100, 0.0f, humanTexture[2, 0], humanTexture[2, 1], myMap, d_human3));
                                    mopCount++;
                                    Timer3 = 0;
                                }
                            }
                            else//언데드 소환
                            {
                                if (Timer1 > 4.0f)
                                {
                                    units.Add(new Unit(150, 0.018f, undeadTexture[0, 0], undeadTexture[0, 1], myMap, d_undead1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 8.3f)
                                {
                                    units.Add(new Unit(200, 0.04f, undeadTexture[1, 0], undeadTexture[1, 1], myMap, d_undead2));
                                    mopCount++;
                                    Timer2 = 0;
                                }
                                if (Timer3 > 7.0f)
                                {
                                    units.Add(new Unit(100, 0.0f, undeadTexture[2, 0], undeadTexture[2, 1], myMap, d_undead3));
                                    mopCount++;
                                    Timer3 = 0;
                                }
                            }
                        }
                        else if (select == 3)//내가 언데드
                        {
                            if (randomNum == 0)//인간 소환
                            {
                                if (Timer1 > 4.0f)
                                {
                                    units.Add(new Unit(150, 0.018f, humanTexture[0, 0], humanTexture[0, 1], myMap, d_undead1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 8.3f)
                                {
                                    units.Add(new Unit(200, 0.04f, humanTexture[1, 0], humanTexture[1, 1], myMap, d_undead2));
                                    mopCount++;
                                    Timer2 = 0;
                                }
                                if (Timer3 > 7.0f)
                                {
                                    units.Add(new Unit(100, 0.0f, humanTexture[2, 0], humanTexture[2, 1], myMap, d_undead3));
                                    mopCount++;
                                    Timer3 = 0;
                                }
                            }
                            else//오크 소환
                            {
                                if (Timer1 > 4.0f)
                                {
                                    units.Add(new Unit(150, 0.018f, orcTexture[0, 0], orcTexture[0, 1], myMap, d_orc1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 8.3f)
                                {
                                    units.Add(new Unit(200, 0.04f, orcTexture[1, 0], orcTexture[1, 1], myMap, d_orc2));
                                    mopCount++;
                                    Timer2 = 0;
                                }
                                if (Timer3 > 7.0f)
                                {
                                    units.Add(new Unit(100, 0.0f, orcTexture[2, 0], orcTexture[2, 1], myMap, d_orc3));
                                    mopCount++;
                                    Timer3 = 0;
                                }
                            }
                        }
                    }
                }
                #endregion

                #region wave 5
                else if (wave == 5)
                {
                    bool nextFlag = false;
                    foreach (Unit u in units)
                    {
                        if (!u.saveFlag && mopCount >= 50)
                            nextFlag = true;
                        if (u.saveFlag)
                            nextFlag = false;

                    }
                    if (nextFlag)
                    {
                        if (select == 1)
                            h_bgm.Dispose();
                        else if (select == 2)
                            o_bgm.Dispose();
                        else if (select == 3)
                            u_bgm.Dispose();

                        fanfare.Play();
                        gamestate = 3;
                    }

                    myMap.map = myMap.getUI(map5, 0, 4, select);//웨이브당 맵 매핑

                    if (mopCount < 50)
                    {
                        if (select == 1)//내가 인간
                        {
                            if (randomNum == 0)//오크 소환
                            {
                                if (Timer1 > 3.5f)
                                {
                                    units.Add(new Unit(150, 0.018f, orcTexture[0, 0], orcTexture[0, 1], myMap, d_orc1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 8.0f)
                                {
                                    units.Add(new Unit(200, 0.04f, orcTexture[1, 0], orcTexture[1, 1], myMap, d_orc2));
                                    mopCount++;
                                    Timer2 = 0;
                                }
                                if (Timer3 > 6.0f)
                                {
                                    units.Add(new Unit(100, 0.0f, orcTexture[2, 0], orcTexture[2, 1], myMap, d_orc3));
                                    mopCount++;
                                    Timer3 = 0;
                                }
                            }
                            else//언데드 소환
                            {
                                if (Timer1 > 3.5f)
                                {
                                    units.Add(new Unit(150, 0.018f, undeadTexture[0, 0], undeadTexture[0, 1], myMap, d_undead1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 8.0f)
                                {
                                    units.Add(new Unit(200, 0.04f, undeadTexture[1, 0], undeadTexture[1, 1], myMap, d_undead2));
                                    mopCount++;
                                    Timer2 = 0;
                                }
                                if (Timer3 > 6.0f)
                                {
                                    units.Add(new Unit(100, 0.0f, undeadTexture[2, 0], undeadTexture[2, 1], myMap, d_undead3));
                                    mopCount++;
                                    Timer3 = 0;
                                }
                            }
                        }
                        else if (select == 2)//내가 오크
                        {
                            if (randomNum == 0)//인간 소환
                            {
                                if (Timer1 > 3.5f)
                                {
                                    units.Add(new Unit(150, 0.018f, humanTexture[0, 0], humanTexture[0, 1], myMap, d_human1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 8.0f)
                                {
                                    units.Add(new Unit(200, 0.04f, humanTexture[1, 0], humanTexture[1, 1], myMap, d_human2));
                                    mopCount++;
                                    Timer2 = 0;
                                }
                                if (Timer3 > 6.0f)
                                {
                                    units.Add(new Unit(100, 0.0f, humanTexture[2, 0], humanTexture[2, 1], myMap, d_human3));
                                    mopCount++;
                                    Timer3 = 0;
                                }
                            }
                            else//언데드 소환
                            {
                                if (Timer1 > 3.5f)
                                {
                                    units.Add(new Unit(150, 0.018f, undeadTexture[0, 0], undeadTexture[0, 1], myMap, d_undead1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 8.0f)
                                {
                                    units.Add(new Unit(200, 0.04f, undeadTexture[1, 0], undeadTexture[1, 1], myMap, d_undead2));
                                    mopCount++;
                                    Timer2 = 0;
                                }
                                if (Timer3 > 6.0f)
                                {
                                    units.Add(new Unit(100, 0.0f, undeadTexture[2, 0], undeadTexture[2, 1], myMap, d_undead3));
                                    mopCount++;
                                    Timer3 = 0;
                                }
                            }
                        }
                        else if (select == 3)//내가 언데드
                        {
                            if (randomNum == 0)//인간 소환
                            {
                                if (Timer1 > 3.5f)
                                {
                                    units.Add(new Unit(150, 0.018f, humanTexture[0, 0], humanTexture[0, 1], myMap, d_human1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 8.0f)
                                {
                                    units.Add(new Unit(200, 0.04f, humanTexture[1, 0], humanTexture[1, 1], myMap, d_human2));
                                    mopCount++;
                                    Timer2 = 0;
                                }
                                if (Timer3 > 6.0f)
                                {
                                    units.Add(new Unit(100, 0.0f, humanTexture[2, 0], humanTexture[2, 1], myMap, d_human3));
                                    mopCount++;
                                    Timer3 = 0;
                                }
                            }
                            else//오크 소환
                            {
                                if (Timer1 > 3.5f)
                                {
                                    units.Add(new Unit(150, 0.018f, orcTexture[0, 0], orcTexture[0, 1], myMap, d_orc1));
                                    mopCount++;
                                    Timer1 = 0;
                                }
                                if (Timer2 > 8.0f)
                                {
                                    units.Add(new Unit(200, 0.04f, orcTexture[1, 0], orcTexture[1, 1], myMap, d_orc2));
                                    mopCount++;
                                    Timer2 = 0;
                                }
                                if (Timer3 > 6.0f)
                                {
                                    units.Add(new Unit(100, 0.0f, orcTexture[2, 0], orcTexture[2, 1], myMap, d_orc3));
                                    mopCount++;
                                    Timer3 = 0;
                                }
                            }
                        }
                    }
                }
                #endregion

                // TODO: Add your update logic here

                #region 포탑 마우스 클릭이벤트
                Rectangle re1 = new Rectangle(780, 100, 64, 64);
                Rectangle re2 = new Rectangle(780, 220, 64, 64);
                Rectangle re3 = new Rectangle(780, 340, 64, 64);
                Rectangle re4 = new Rectangle(64 * 7, 64 * 9 - 20, 64, 64);
                Rectangle re5 = new Rectangle(64 * 9, 64 * 9 - 20, 64, 64);
                Rectangle re6 = new Rectangle(64 * 11, 64 * 9 - 20, 64, 64);
                //얘는 마우스 정보를 가져와서 이거로 비교비굑
                if (clickFlag && Mouse.GetState().LeftButton == ButtonState.Pressed && mo.Intersects(re1))
                {
                    build.Play();
                    mouseindex = 0;
                    clickFlag = false;
                }
                else if (clickFlag && Mouse.GetState().LeftButton == ButtonState.Pressed && mo.Intersects(re2))
                {
                    build.Play();
                    mouseindex = 1;
                    clickFlag = false;
                }
                else if (clickFlag && Mouse.GetState().LeftButton == ButtonState.Pressed && mo.Intersects(re3))
                {
                    build.Play();
                    mouseindex = 2;
                    clickFlag = false;
                }
                if (!clickFlag && Mouse.GetState().LeftButton == ButtonState.Released)
                {
                    //이거는 타워가 있으면 설치 못하게 하는 조건식.
                    foreach (Tower a in towers)
                    {
                        if (a.x == (int)(Mouse.GetState().X / 64) && a.y == (int)(Mouse.GetState().Y / 64))
                            mouseindex = -1;
                    }
                    // 이거는 길위에는 타워를 못놓게 하는 조건식.
                    if (myMap.map[Mouse.GetState().Y / 64, Mouse.GetState().X / 64] == 1)
                    {
                        if (mouseindex == 0 && money>=200)
                        {
                            build.Play();
                            construction.Play();
                            towers.Add(new Tower(Mouse.GetState().X / 64, Mouse.GetState().Y / 64, 64 + 64 + 32, towerTexture[mouseindex, 0], bulletTexture[mouseindex,0], bulletTexture[mouseindex,1], 10, 0.0f,bombsound));
                            mouseindex = -1;
                            money -= 200;
                        }
                        else if (mouseindex == 1 && money >= 500)
                        {
                            build.Play();
                            construction.Play();
                            towers.Add(new Tower(Mouse.GetState().X / 64, Mouse.GetState().Y / 64, 64 + 32, towerTexture[mouseindex, 0], bulletTexture[mouseindex,0], bulletTexture[mouseindex, 1], 3, 0.0f,flame));
                            mouseindex = -1;
                            money -= 500;
                        }
                        else if (mouseindex == 2 && money >= 300)
                        {
                            build.Play();
                            construction.Play();
                            towers.Add(new Tower(Mouse.GetState().X / 64, Mouse.GetState().Y / 64, 64 + 64, towerTexture[mouseindex, 0], bulletTexture[mouseindex,0], bulletTexture[mouseindex, 1], 4, 0.0f,squish));
                            mouseindex = -1;
                            money -= 300;
                        }
                        else
                            mouseindex = -1;
                    }
                    else
                    {
                        
                        mouseindex = -1;
                    }

                    clickFlag = true;
                }
                //포탑1
                if (Mouse.GetState().LeftButton == ButtonState.Released && mo.Intersects(re1))
                {
                    imgflag6 = true;
                }
                else
                {
                    imgflag6 = false;
                }
                //포탑2
                if (Mouse.GetState().LeftButton == ButtonState.Released && mo.Intersects(re2))
                {
                    imgflag7 = true;
                }
                else
                {
                    imgflag7 = false;
                }
                //포탑3
                if (Mouse.GetState().LeftButton == ButtonState.Released && mo.Intersects(re3))
                {
                    imgflag8 = true;
                }
                else
                {
                    imgflag8 = false;
                }
                // 공격 버튼들.
                if (Mouse.GetState().LeftButton == ButtonState.Released && mo.Intersects(re4))
                {
                    imgflag9 = true;
                }
                else
                {
                    imgflag9 = false;
                }
                if (Mouse.GetState().LeftButton == ButtonState.Released && mo.Intersects(re5))
                {
                    imgflag10 = true;
                }
                else
                {
                    imgflag10 = false;
                }
                if (Mouse.GetState().LeftButton == ButtonState.Released && mo.Intersects(re6))
                {
                    imgflag11 = true;
                }
                else
                {
                    imgflag11 = false;
                }
                //눌렀을떄.
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && mo.Intersects(re4))
                {
                    if(money >= 100 && !selectmode)
                    {
                        money -= 100;
                        m_Write.WriteLine(Convert.ToString(50 + select));
                        m_Write.Flush();
                    }
                }
                else if (clickFlag && Mouse.GetState().LeftButton == ButtonState.Pressed && mo.Intersects(re5))
                {
                    if (money >= 250 && !selectmode)
                    {
                        money -= 250;
                        m_Write.WriteLine(Convert.ToString(80 + select));
                        m_Write.Flush();
                    }
                }
                else if (clickFlag && Mouse.GetState().LeftButton == ButtonState.Pressed && mo.Intersects(re6))
                {
                    if (money >= 200 && !selectmode)
                    {
                        money -= 200;
                        m_Write.WriteLine(Convert.ToString(70 + select));
                        m_Write.Flush();
                    }
                }
                #endregion

                i -= 5;
            }
            base.Update(gameTime);
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

            if (gamestate == 0)
            {
                GraphicsDevice.Clear(Color.Black);
                spriteBatch.DrawString(font, "Human", new Vector2(64 * 2, 32), Color.White);
                spriteBatch.DrawString(font, "  Ork", new Vector2(64 * 6, 32), Color.White);
                spriteBatch.DrawString(font, "Undead", new Vector2(64 * 10, 32), Color.White);
                spriteBatch.Draw(mode[0, 0], new Rectangle(64 * 12 + 32, 64 * 2, 128, 128), Color.White);
                spriteBatch.Draw(mode[1, 0], new Rectangle(64 * 12 + 32, 64 * 6, 128, 128), Color.White);
                //이거는 올렸을때에 따라서 이미지 변경하는거.
                if (imgflag1 && select != 1)
                {
                    spriteBatch.Draw(tribe[0, 1], new Rectangle(64 * 1, 64 * 1, 192, 640), Color.White);
                    //spriteBatch.DrawString(font, "test", new Vector2(128,128), Color.White);
                }
                else if (!imgflag1 && select != 1)
                    spriteBatch.Draw(tribe[0, 0], new Rectangle(64 * 1, 64 * 1, 192, 640), Color.White);
                if (imgflag2 && select != 2)
                {
                    spriteBatch.Draw(tribe[1, 1], new Rectangle(64 * 5, 64 * 1, 192, 640), Color.White);
                    //spriteBatch.DrawString(font, "test123", new Vector2(128, 128), Color.White);
                }
                else if (!imgflag2 && select != 2)
                    spriteBatch.Draw(tribe[1, 0], new Rectangle(64 * 5, 64 * 1, 192, 640), Color.White);

                if (imgflag3 && select != 3)
                {
                    spriteBatch.Draw(tribe[2, 1], new Rectangle(64 * 9, 64 * 1, 192, 640), Color.White);
                }
                else if (!imgflag3 && select != 3)
                {
                    spriteBatch.Draw(tribe[2, 0], new Rectangle(64 * 9, 64 * 1, 192, 640), Color.White);
                }
                if (imgflag4)
                {
                    spriteBatch.Draw(mode[0, 1], new Rectangle(64 * 12 + 32, 64 * 2, 128, 128), Color.White);
                }
                else
                {
                    spriteBatch.Draw(mode[0, 0], new Rectangle(64 * 12 + 32, 64 * 2, 128, 128), Color.White);
                }
                if (imgflag5)
                {
                    spriteBatch.Draw(mode[1, 1], new Rectangle(64 * 12 + 32, 64 * 6, 128, 128), Color.White);
                }
                else
                {
                    spriteBatch.Draw(mode[1, 0], new Rectangle(64 * 12 + 32, 64 * 6, 128, 128), Color.White);
                }

                //이거는 체크박스를 마우스 이벤트로 구현
                if (select == 1)
                {
                    spriteBatch.Draw(tribe[0, 1], new Rectangle(64 * 1, 64 * 1, 192, 640), Color.White);
                }
                else if (select == 2)
                {
                    spriteBatch.Draw(tribe[1, 1], new Rectangle(64 * 5, 64 * 1, 192, 640), Color.White);
                }
                else if (select == 3)
                {
                    spriteBatch.Draw(tribe[2, 1], new Rectangle(64 * 9, 64 * 1, 192, 640), Color.White);
                }
            }

            else if (gamestate == 1)
            {
                myMap.Draw(spriteBatch);

                #region 슬라이드로 웨이브 표시
                if (wave == 1)
                {
                    if (temp)
                    {
                        stage_s.Play();
                        i = 64 * 15;
                        temp = false;
                    }
                    if (i < 64 * 15)
                    {
                        spriteBatch.Draw(stage[wave - 1], new Rectangle(i, 64 * 3, 450, 100), Color.White);
                        if (select == 1 && randomNum == 0)
                        {
                            spriteBatch.Draw(attack[1], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }
                        else if (select == 1 && randomNum == 1)
                        {
                            spriteBatch.Draw(attack[2], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }
                        else if (select == 2 && randomNum == 0)
                        {
                            spriteBatch.Draw(attack[0], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }
                        else if (select == 2 && randomNum == 1)
                        {
                            spriteBatch.Draw(attack[2], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }
                        else if (select == 3 && randomNum == 0)
                        {
                            spriteBatch.Draw(attack[0], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }
                        else if (select == 3 && randomNum == 1)
                        {
                            spriteBatch.Draw(attack[1], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }

                    }
                }
                if (wave == 2)
                {
                    if (temp)
                    {
                        stage_s.Play();
                        i = 64 * 15;
                        temp = false;
                    }
                    if (i < 64 * 15)
                    {
                        spriteBatch.Draw(stage[wave - 1], new Rectangle(i, 64 * 3, 450, 100), Color.White);
                        if (select == 1 && randomNum == 0)
                        {
                            spriteBatch.Draw(attack[1], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }
                        else if (select == 1 && randomNum == 1)
                        {
                            spriteBatch.Draw(attack[2], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }
                        else if (select == 2 && randomNum == 0)
                        {
                            spriteBatch.Draw(attack[0], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }
                        else if (select == 2 && randomNum == 1)
                        {
                            spriteBatch.Draw(attack[2], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }
                        else if (select == 3 && randomNum == 0)
                        {
                            spriteBatch.Draw(attack[0], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }
                        else if (select == 3 && randomNum == 1)
                        {
                            spriteBatch.Draw(attack[1], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }

                    }
                }
                if (wave == 3)
                {
                    if (temp)
                    {
                        stage_s.Play();
                        i = 64 * 15;
                        temp = false;
                    }
                    if (i < 64 * 15)
                    {
                        spriteBatch.Draw(stage[wave - 1], new Rectangle(i, 64 * 3, 450, 100), Color.White);
                        if (select == 1 && randomNum == 0)
                        {
                            spriteBatch.Draw(attack[1], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }
                        else if (select == 1 && randomNum == 1)
                        {
                            spriteBatch.Draw(attack[2], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }
                        else if (select == 2 && randomNum == 0)
                        {
                            spriteBatch.Draw(attack[0], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }
                        else if (select == 2 && randomNum == 1)
                        {
                            spriteBatch.Draw(attack[2], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }
                        else if (select == 3 && randomNum == 0)
                        {
                            spriteBatch.Draw(attack[0], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }
                        else if (select == 3 && randomNum == 1)
                        {
                            spriteBatch.Draw(attack[1], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }
                    }
                }
                if (wave == 4)
                {
                    if (temp)
                    {
                        stage_s.Play();
                        i = 64 * 15;
                        temp = false;
                    }
                    if (i < 64 * 15)
                    {
                        spriteBatch.Draw(stage[wave - 1], new Rectangle(i, 64 * 3, 450, 100), Color.White);
                        if (select == 1 && randomNum == 0)
                        {
                            spriteBatch.Draw(attack[1], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }
                        else if (select == 1 && randomNum == 1)
                        {
                            spriteBatch.Draw(attack[2], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }
                        else if (select == 2 && randomNum == 0)
                        {
                            spriteBatch.Draw(attack[0], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }
                        else if (select == 2 && randomNum == 1)
                        {
                            spriteBatch.Draw(attack[2], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }
                        else if (select == 3 && randomNum == 0)
                        {
                            spriteBatch.Draw(attack[0], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }
                        else if (select == 3 && randomNum == 1)
                        {
                            spriteBatch.Draw(attack[1], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }
                    }
                }
                if (wave == 5)
                {
                    if (temp)
                    {
                        stage_s.Play();
                        i = 64 * 15;
                        temp = false;
                    }
                    if (i < 64 * 15)
                    {
                        spriteBatch.Draw(stage[wave - 1], new Rectangle(i, 64 * 3, 450, 100), Color.White);
                        if (select == 1 && randomNum == 0)
                        {
                            spriteBatch.Draw(attack[1], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }
                        else if (select == 1 && randomNum == 1)
                        {
                            spriteBatch.Draw(attack[2], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }
                        else if (select == 2 && randomNum == 0)
                        {
                            spriteBatch.Draw(attack[0], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }
                        else if (select == 2 && randomNum == 1)
                        {
                            spriteBatch.Draw(attack[2], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }
                        else if (select == 3 && randomNum == 0)
                        {
                            spriteBatch.Draw(attack[0], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }
                        else if (select == 3 && randomNum == 1)
                        {
                            spriteBatch.Draw(attack[1], new Rectangle(i, 64 * 5, 300, 50), Color.White);
                        }

                    }
                }
                #endregion
                if (select == 1)
                {
                    if (h_bgm.State == SoundState.Stopped)
                        h_bgm.Play();
                    spriteBatch.DrawString(font, score.ToString(), new Vector2(2 * 64 + 20, 9 * 64 + 20), Color.Black);
                    spriteBatch.DrawString(font, money.ToString(), new Vector2(2 * 64 + 20, 8 * 64 + 55), Color.Black);
                }
                else
                {

                    if (select == 2)
                        if (o_bgm.State == SoundState.Stopped)
                            o_bgm.Play();
                    if (select == 3)
                        if (u_bgm.State == SoundState.Stopped)
                            u_bgm.Play();
                    spriteBatch.DrawString(font, score.ToString(), new Vector2(2 * 64 + 20, 9 * 64 + 20), Color.White);
                    spriteBatch.DrawString(font, money.ToString(), new Vector2(2 * 64 + 20, 8 * 64 + 55), Color.White);
                }


                foreach (Unit x in units)
                {
                    x.Draw(spriteBatch);
                }

                #region 포탑 새로 설치하는 코드
                spriteBatch.Draw(towerTexture[0, 0], new Rectangle(780, 100, 64, 64), Color.White);
                spriteBatch.Draw(towerTexture[1, 0], new Rectangle(780, 220, 64, 64), Color.White);
                spriteBatch.Draw(towerTexture[2, 0], new Rectangle(780, 340, 64, 64), Color.White);
                //여기서 부터 마우스 인덱스를 통하여 어떤 버튼을 클릭했고 그에 해당하는 이미지를 마우스를 따라다니게 하는것.
                if (mouseindex == 0)
                {
                    spriteBatch.Draw(towerTexture[0, 0], new Rectangle(Mouse.GetState().X - 32, Mouse.GetState().Y - 32, 64, 64), Color.White);
                }
                else if (mouseindex == 1)
                {
                    spriteBatch.Draw(towerTexture[1, 0], new Rectangle(Mouse.GetState().X - 32, Mouse.GetState().Y - 32, 64, 64), Color.White);
                }
                else if (mouseindex == 2)
                {
                    spriteBatch.Draw(towerTexture[2, 0], new Rectangle(Mouse.GetState().X - 32, Mouse.GetState().Y - 32, 64, 64), Color.White);
                }


                if (imgflag6)
                {
                    spriteBatch.Draw(towerTexture[0, 1], new Rectangle(780, 100, 64, 64), Color.White);
                    if (select == 1)
                    {
                        spriteBatch.DrawString(font, "Name:", new Vector2(64 * 13, 64 * 2 - 32), Color.Black);
                        spriteBatch.DrawString(font, "FokFok", new Vector2(64 * 13, 64 * 2 - 0), Color.Black);
                        spriteBatch.DrawString(font, "Feature:", new Vector2(64 * 13, 64 * 2 + 32), Color.Black);
                        spriteBatch.DrawString(font, "High Damage", new Vector2(64 * 13, 64 * 2 + 64), Color.Black);
                        spriteBatch.DrawString(font, "Price: 200", new Vector2(64 * 13, 64 * 2 + 96), Color.Black);
                    }
                    else
                    {
                        spriteBatch.DrawString(font, "Name:", new Vector2(64 * 13, 64 * 2 - 32), Color.White);
                        spriteBatch.DrawString(font, "FokFok", new Vector2(64 * 13, 64 * 2 - 0), Color.White);
                        spriteBatch.DrawString(font, "Feature:", new Vector2(64 * 13, 64 * 2 + 32), Color.White);
                        spriteBatch.DrawString(font, "High Damage", new Vector2(64 * 13, 64 * 2 + 64), Color.White);
                        spriteBatch.DrawString(font, "Price: 200", new Vector2(64 * 13, 64 * 2 + 96), Color.White);
                    }
                }
                else
                    spriteBatch.Draw(towerTexture[0, 0], new Rectangle(780, 100, 64, 64), Color.White);
                if (imgflag7)
                {
                    spriteBatch.Draw(towerTexture[1, 1], new Rectangle(780, 220, 64, 64), Color.White);
                    if (select == 1)
                    {
                        spriteBatch.DrawString(font, "Name:", new Vector2(64 * 13 + 10, 64 * 4 - 32), Color.Black);
                        spriteBatch.DrawString(font, "BoolBool", new Vector2(64 * 13 + 10, 64 * 4 - 0), Color.Black);
                        spriteBatch.DrawString(font, "Feature:", new Vector2(64 * 13 + 10, 64 * 4 + 32), Color.Black);
                        spriteBatch.DrawString(font, "Range Attack", new Vector2(64 * 13 - 20, 64 * 4 + 64), Color.Black);
                        spriteBatch.DrawString(font, "Price: 500", new Vector2(64 * 13 + 10, 64 * 4 + 96), Color.Black);
                    }
                    else
                    {
                        spriteBatch.DrawString(font, "Name:", new Vector2(64 * 13 + 10, 64 * 4 - 32), Color.White);
                        spriteBatch.DrawString(font, "BoolBool", new Vector2(64 * 13 + 10, 64 * 4 - 0), Color.White);
                        spriteBatch.DrawString(font, "Feature:", new Vector2(64 * 13 + 10, 64 * 4 + 32), Color.White);
                        spriteBatch.DrawString(font, "Range Attack", new Vector2(64 * 13 - 20, 64 * 4 + 64), Color.White);
                        spriteBatch.DrawString(font, "Price: 500", new Vector2(64 * 13 + 10, 64 * 4 + 96), Color.White);
                    }
                }
                else
                    spriteBatch.Draw(towerTexture[1, 0], new Rectangle(780, 220, 64, 64), Color.White);

                if (imgflag8)
                {
                    spriteBatch.Draw(towerTexture[2, 1], new Rectangle(780, 340, 64, 64), Color.White);
                    if (select == 1)
                    {
                        spriteBatch.DrawString(font, "Name:", new Vector2(64 * 13 + 10, 64 * 6 - 32), Color.Black);
                        spriteBatch.DrawString(font, "ChupChup", new Vector2(64 * 13 + 10, 64 * 6 - 0), Color.Black);
                        spriteBatch.DrawString(font, "Feature:", new Vector2(64 * 13 + 10, 64 * 6 + 32), Color.Black);
                        spriteBatch.DrawString(font, "slow enemy", new Vector2(64 * 13 + 10, 64 * 6 + 64), Color.Black);
                        spriteBatch.DrawString(font, "Price: 300", new Vector2(64 * 13 + 10, 64 * 6 + 96), Color.Black);
                    }
                    else
                    {
                        spriteBatch.DrawString(font, "Name:", new Vector2(64 * 13 + 10, 64 * 6 - 32), Color.White);
                        spriteBatch.DrawString(font, "ChupChup", new Vector2(64 * 13 + 10, 64 * 6 - 0), Color.White);
                        spriteBatch.DrawString(font, "Feature:", new Vector2(64 * 13 + 10, 64 * 6 + 32), Color.White);
                        spriteBatch.DrawString(font, "Slow enemy", new Vector2(64 * 13 + 10, 64 * 6 + 64), Color.White);
                        spriteBatch.DrawString(font, "Price: 300", new Vector2(64 * 13 + 10, 64 * 6 + 96), Color.White);
                    }
                }
                else
                {
                    spriteBatch.Draw(towerTexture[2, 0], new Rectangle(780, 340, 64, 64), Color.White);
                }
                //공격쓰
                if (!imgflag9)
                {
                    if (select == 1)
                    {
                        spriteBatch.Draw(humanTexture[0, 1], new Rectangle(64 * 7, 64 * 9 - 20, 64, 64), Color.White);
                    }
                    if (select == 2)
                    {
                        spriteBatch.Draw(orcTexture[0, 1], new Rectangle(64 * 7, 64 * 9 - 20, 64, 64), Color.White);
                    }
                    if (select == 3)
                    {
                        spriteBatch.Draw(undeadTexture[0, 1], new Rectangle(64 * 7, 64 * 9 - 20, 64, 64), Color.White);
                    }

                }
                else
                {
                    if (select == 1)
                    {
                        spriteBatch.Draw(humanTexture[0, 2], new Rectangle(64 * 7, 64 * 9 - 20, 64, 64), Color.White);
                    }
                    if (select == 2)
                    {
                        spriteBatch.Draw(orcTexture[0, 2], new Rectangle(64 * 7, 64 * 9 - 20, 64, 64), Color.White);
                    }
                    if (select == 3)
                    {
                        spriteBatch.Draw(undeadTexture[0, 2], new Rectangle(64 * 7, 64 * 9 - 20, 64, 64), Color.White);
                    }
                }
                if (!imgflag10)
                {
                    if (select == 1)
                    {
                        spriteBatch.Draw(humanTexture[1, 1], new Rectangle(64 * 9, 64 * 9 - 20, 64, 64), Color.White);
                    }
                    if (select == 2)
                    {
                        spriteBatch.Draw(orcTexture[1, 1], new Rectangle(64 * 9, 64 * 9 - 20, 64, 64), Color.White);
                    }
                    if (select == 3)
                    {
                        spriteBatch.Draw(undeadTexture[1, 1], new Rectangle(64 * 9, 64 * 9 - 20, 64, 64), Color.White);
                    }
                }
                else
                {
                    if (select == 1)
                    {
                        spriteBatch.Draw(humanTexture[1, 2], new Rectangle(64 * 9, 64 * 9 - 20, 64, 64), Color.White);
                    }
                    if (select == 2)
                    {
                        spriteBatch.Draw(orcTexture[1, 2], new Rectangle(64 * 9, 64 * 9 - 20, 64, 64), Color.White);
                    }
                    if (select == 3)
                    {
                        spriteBatch.Draw(undeadTexture[1, 2], new Rectangle(64 * 9, 64 * 9 - 20, 64, 64), Color.White);
                    }
                }
                if (!imgflag11)
                {
                    if (select == 1)
                    {
                        spriteBatch.Draw(humanTexture[2, 1], new Rectangle(64 * 11, 64 * 9 - 20, 64, 64), Color.White);
                    }
                    if (select == 2)
                    {
                        spriteBatch.Draw(orcTexture[2, 1], new Rectangle(64 * 11, 64 * 9 - 20, 64, 64), Color.White);
                    }
                    if (select == 3)
                    {
                        spriteBatch.Draw(undeadTexture[2, 1], new Rectangle(64 * 11, 64 * 9 - 20, 64, 64), Color.White);
                    }
                }
                else
                {
                    if (select == 1)
                    {
                        spriteBatch.Draw(humanTexture[2, 2], new Rectangle(64 * 11, 64 * 9 - 20, 64, 64), Color.White);
                    }
                    if (select == 2)
                    {
                        spriteBatch.Draw(orcTexture[2, 2], new Rectangle(64 * 11, 64 * 9 - 20, 64, 64), Color.White);
                    }
                    if (select == 3)
                    {
                        spriteBatch.Draw(undeadTexture[2, 2], new Rectangle(64 * 11, 64 * 9 - 20, 64, 64), Color.White);
                    }
                }




                #endregion

                foreach (Tower y in towers)
                {
                    y.Draw(spriteBatch);
                }

                for (int i = 0; i < heart; i++)
                {
                    spriteBatch.Draw(heartTexture, new Rectangle(3 + (i * 35), 3, 32, 32), Color.White);
                }
            }

            else if (gamestate == 2)
            {
                GraphicsDevice.Clear(Color.Black);
                spriteBatch.Draw(gameover, new Rectangle(0, 0, 960, 640), Color.White);
            }
            else if (gamestate == 3)
            {
                GraphicsDevice.Clear(Color.Black);
                spriteBatch.Draw(complete, new Rectangle(0, 0, 960, 640), Color.White);
            }


            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
