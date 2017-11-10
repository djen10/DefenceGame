using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace Final01
{
    class Unit
    {
        public int x, y, HP; //본인의 좌표, 스피드, 체력(스피드는 timer로 비교하므로 아주 작은 수가 될 예정)
        public float speed;
        public bool saveFlag; //살아있는지 판단
        List<Texture2D> unitTexture; //본인 텍스쳐
        public Vector2 center; //본인의 중앙 좌표. 타워와 거리 확인용

        //그리기, 업데이트용 변수
        int textureIndex = 0; //걷는 모션용 텍스쳐 리스트 인덱스
        public Rectangle unitRec; //유닛의 rec, 아직 타격 범위 Rec은 안만든 상태
        public Rectangle unitHit; //유닛의 타격범위
        private float unitTimer = 0; //유닛의 이동, update를 책임질 타이머
        private float motionTimer = 0; //유닛의 모션, update를 책임질 타이머

        public int money = 0;
        public int score = 0;
        public bool attack = false;

        SoundEffect U_DEATH;

        private Map myMap;

        private bool[,] moveFlag = new bool[,]
        {
            {false,false,false,false,false,false,false,false,false,false,false,false},
            {false,false,false,false,false,false,false,false,false,false,false,false},
            {false,false,false,false,false,false,false,false,false,false,false,false},
            {false,false,false,false,false,false,false,false,false,false,false,false},
            {false,false,false,false,false,false,false,false,false,false,false,false},
            {false,false,false,false,false,false,false,false,false,false,false,false},
            {false,false,false,false,false,false,false,false,false,false,false,false},
            {false,false,false,false,false,false,false,false,false,false,false,false}
        };
        private string moveBack = "우";

        public int getMoney()
        {
            int ret = money;
            money = 0;
            return ret;
        }
        public int getScore()
        {
            int ret = score;
            score = 0;
            return ret;
        }
        public bool getAttack()
        {
            bool ret = attack;
            attack = false;
            return ret;
        }

        public Unit(int HP, float speed, Texture2D texture1, Texture2D texture2, Map myMap, SoundEffect u_die)
        {
            U_DEATH = u_die;
            this.x = myMap.startX * 64; this.y = myMap.startY * 64; this.HP = HP; this.speed = speed;
            saveFlag = true;
            this.unitTexture = new List<Texture2D>();
            unitTexture.Add(texture1);
            unitTexture.Add(texture2);

            center = new Vector2(this.x + 32, this.y + 32);//중앙좌표 벡터 한 칸이 64비트이므로
            unitRec = new Rectangle(this.x, this.y, 64, 64);
            unitHit = new Rectangle(this.x + 30, this.y + 30, 4, 4);
            this.myMap = myMap;
        }//시작할 x, y좌표( 맵마다 start point를 정해서 넣어줄 것 ), 유닛의 속도, 걷는 모션 텍스쳐, HP, 죽었는지 파악할 flag

        public void Update(GameTime gameTime)
        {
            unitTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            motionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(motionTimer > 0.3f)
            {
                if (textureIndex == 0)// 모션 변경
                    textureIndex = 1;
                else
                    textureIndex = 0;

                motionTimer = 0;
            }

            if (saveFlag && unitTimer > speed) //살아있고, 타이머가 스피드를 넘었을 때
            {
                int mapX=0, mapY=0;//맵상의 내가 존재하는 셀이 어디인지 좌표(본인의 진짜 좌표가 아니다 64의 배수 좌표)
                if (moveBack.Equals("상")) { mapX = (int)(x / 64); mapY = (int)((y + 63) / 64); }
                else if (moveBack.Equals("하")) { mapX = (int)(x / 64); mapY = (int)(y / 64); }
                else if (moveBack.Equals("좌")) { mapX = (int)((x + 63) / 64); mapY = (int)(y / 64); }
                else if (moveBack.Equals("우")) { mapX = (int)(x / 64); mapY = (int)(y / 64); }

                
                if (mapY > 0 && !moveFlag[mapY - 1, mapX] && !moveBack.Equals("하") && (myMap.map[mapY - 1, mapX ]== 0 || myMap.map[mapY - 1, mapX] == 2))
                {
                    y -= 1;
                    moveFlag[mapY,mapX] = true;
                    moveBack = "상";
                    if(myMap.map[mapY - 1, mapX] == 2)
                    {
                        attack = true;
                        saveFlag = false;
                    }
                        
                }
                    
                else if (mapX < 11 && !moveFlag[mapY, mapX+1] && !moveBack.Equals("좌") && (myMap.map[mapY, mapX + 1] == 0 || myMap.map[mapY, mapX + 1] == 2))
                {
                    x += 1;
                    moveFlag[mapY, mapX] = true;
                    moveBack = "우";
                    if (myMap.map[mapY, mapX + 1] == 2)
                    {
                        attack = true;
                        saveFlag = false;
                    }
                }
                    
                else if (mapY < 7 && !moveFlag[mapY + 1, mapX] && !moveBack.Equals("상") && (myMap.map[mapY + 1, mapX] == 0 || myMap.map[mapY + 1, mapX] == 2))
                {
                    y += 1;
                    moveFlag[mapY, mapX] = true;
                    moveBack = "하";
                    if (myMap.map[mapY + 1, mapX] == 2)
                    {
                        attack = true;
                        saveFlag = false;
                    }
                }
                    
                else if (mapX > 0 && !moveFlag[mapY, mapX - 1] && !moveBack.Equals("우") && (myMap.map[mapY, mapX - 1] == 0 || myMap.map[mapY, mapX - 1] == 2))
                {
                    x -= 1;
                    moveFlag[mapY, mapX] = true;
                    moveBack = "좌";
                    if (myMap.map[mapY, mapX - 1] == 2)
                    {
                        attack = true;
                        saveFlag = false;
                    }
                }

                unitRec = new Rectangle(x, y, 64, 64);//업데이트 된 좌표로 rec 생성
                unitHit = new Rectangle(x + 30, y + 30, 4, 4);
                center = new Vector2(x + 32, y + 32); //업데이트 된 좌표의 중앙 좌표 생성

                unitTimer = 0;
            }

            if (saveFlag && HP < 0) //업데이트중 체력이 바닥나면 죽는다.
            {
                U_DEATH.Play();
                score = 20;
                if (speed == 0.06f || speed == 0.018f)
                    money = 50;
                else if (speed == 0.1f || speed == 0.04f)
                    money = 80;
                else
                    money = 70;
                saveFlag = false;
            }
        }

        public void Draw(SpriteBatch batch)
        {
            if (saveFlag)//죽지 않았으면 그린다. false일 경우 사라짐
                batch.Draw(unitTexture[textureIndex], unitRec, Color.White);
        }
    }
}
