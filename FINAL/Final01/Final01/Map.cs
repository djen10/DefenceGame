using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Final01
{
    class Map
    {
        public int[,] map;
        public int startX, startY;
        private List<Texture2D> textureTile=new List<Texture2D>();
        
        public Map(int[,] map, int startX, int startY, int tribeFlag)//종족 플래그와 맵이 인자로 들어오면 해당하는 UI를 반환해줘야한다.
        {
            this.startX = startX;
            this.startY = startY;
            this.map = getUI(map, startX, startY, tribeFlag);
        }
        public int[,] getUI(int[,] map, int startX, int startY, int tribeFlag)
        {//일단은 종족별로 나누지 않았다.
            if (tribeFlag == 1)//인간일 경우
            {
                int[,] UI = new int[10, 15];

                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        if (i > 7 || j > 11)
                            UI[i, j] = -1;
                        else
                            UI[i, j] = map[i, j];
                    }
                }
                UI[0, 12] = 4;
                UI[8, 0] = 3;
                this.startX = startX;
                this.startY = startY;
                return UI;
            }
            else if (tribeFlag == 2)//오크일 경우
            {
                int[,] UI = new int[10, 15];

                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        if (i > 7 || j > 11)
                            UI[i, j] = -1;
                        else
                            UI[i, j] = map[i, j];
                    }
                }
                UI[0, 12] = 6;
                UI[8, 0] = 5;
                this.startX = startX;
                this.startY = startY;
                return UI;
            }
            else//언데드
            {
                int[,] UI = new int[10, 15];

                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        if (i > 7 || j > 11)
                            UI[i, j] = -1;
                        else
                            UI[i, j] = map[i, j];
                    }
                }
                UI[0, 12] = 8;
                UI[8, 0] = 7;
                this.startX = startX;
                this.startY = startY;
                return UI;
            }
        }

        public int Width
        {
            get { return map.GetLength(1); }
        }

        public int Height
        {
            get { return map.GetLength(0); }
        }

        public void addTexture(Texture2D texture)
        {
            textureTile.Add(texture);//집어넣는 순서가 중요하다.0번은 길, 1번은 땅, 2번은 end point, 3번은 아래UI, 4번은 오른쪽UI
        }

        public void Draw(SpriteBatch batch)
        {
            for(int x = 0; x < Width; x++)
            {
                for(int y = 0; y < Height; y++)
                {
                    int index = map[y, x]; //맵 배열에 넣은 값을 인덱스로하는 텍스쳐 뽑아낸다.
                    int tileX, tileY;//한 타일의 크기

                    if (index == -1)//에러적 요소
                        continue;
                    else if (index == 3 || index ==5 || index ==7) { tileX = 15 * 64; tileY = 2 * 64; }
                    else if (index == 4 || index == 6 || index == 8) { tileX = 3 * 64; tileY = 8 * 64; }
                    else { tileX = 64; tileY = 64; }

                    Texture2D texture = textureTile[index];
                    batch.Draw(texture, new Rectangle(x * 64, y * 64, tileX, tileY), Color.White);
                }
            }
        }
    }
}
