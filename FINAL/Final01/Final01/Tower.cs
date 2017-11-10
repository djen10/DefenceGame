using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Final01
{
    class Tower
    {
        public int x, y;//타워의 위치는 바뀔수 없으므로
        public int range;//스킬중에 범위를 늘려주는 스킬이 있을수 있으니까.
        private Texture2D towerTexture;//타워 텍스쳐
        public Texture2D bulletTexture;//총알을 타워 별로 계속 생성해야하니까
        public Texture2D hitTexture;//탄이 유닛에 맞았을 때
        public Texture2D bullet;

        public int power;//총알 파워
        public float attackRate;//공격 속도
        
        public Vector2 center;//타워 중앙값
        
        public Unit target = null;//타워가 공격할 타겟 유닛
        public List<Unit> targetList = new List<Unit>();

        private float bulletTimer = 0;//총알 날아가는 것 업데이트용 타이머
        float fireRate = 0;
        private Rectangle bulletRec;//총알 rec
        public int bulletX, bulletY;//총알의 x,y 좌표


        SoundEffect bulletsound;
        int bullsoundf = 0;//사운드 플래그

        public class bulletStructure
        {
            public Texture2D texture;
            public Rectangle rect;
            public float rate=0;
            public int x;
            public int y;
            public bulletStructure(int x, int y, Texture2D texture, Rectangle rect)
            {
                this.texture = texture; this.rect = rect; this.x = x; this.y = y;
            }
        }

        public void threeSecond(object obj)
        {
            Unit u = (Unit)obj;
            Thread.Sleep(4000);
            if (u.speed == 0.06f)
                u.speed = 0.018f;
            else if (u.speed == 0.1f)
                u.speed = 0.04f;
            else if (u.speed == 0.035f)
                u.speed = 0.0f;
        }

        public List<bulletStructure> bulletList = new List<bulletStructure>();

        public Tower(int x, int y, int range, Texture2D towerTexture, Texture2D bulletTexture, Texture2D hitTexture, int power, float attackRate,SoundEffect bul)
        {
            this.x = x; this.y = y; this.range = range;
            this.towerTexture = towerTexture;
            this.bulletTexture = bulletTexture;
            this.hitTexture = hitTexture;
            this.bullet = bulletTexture;
            this.power = power;
            this.attackRate = attackRate; //각각 초기화  
            bulletsound = bul;
            center = new Vector2(x * 64 + 32, y * 64 + 32);
            bulletX = x * 64 + 16; bulletY = y * 64 + 16;
            bulletRec = new Rectangle((bulletX), (bulletY), 32, 32);//중앙 값과 총알 좌표, rec 초기화
        }
        
        public void getTarget(List<Unit> units)
        {
            if (power == 3)//불불이는 타겟을 리스트로 가지고 있어야하므로
            {
                for (int i = 0; i < units.Count; i++)
                {
                    if (units[i].saveFlag && Vector2.Distance(this.center, units[i].center) < this.range)//유닛들과의 거리가 사정범위 안인지
                    {
                        if (!targetList.Contains(units[i]))
                        {
                            targetList.Add(units[i]);//타겟 지정
                            bulletList.Add(new bulletStructure(x * 64 + 16, y * 64 + 16, bullet, bulletRec));
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < units.Count; i++)
                {
                    if (units[i].saveFlag && Vector2.Distance(this.center, units[i].center) < this.range)//유닛들과의 거리가 사정범위 안인지
                    {
                        target = units[i];//타겟 지정
                        bulletX = x * 64 + 16;
                        bulletY = y * 64 + 16;
                        break;
                    }
                }
            }
        }

        public void Update(GameTime gameTime)//메인(즉, game클래스) 에서 getTarget을 호출 하고 update해야할 것이다.
        {
            if (power == 3)//불불이 업데이트
            {
                for (int i = 0; i < targetList.Count; i++)
                {
                    bulletList[i].rate += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (bulletList[i].rate > 0.3f)
                    {
                        bulletTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                        if (Vector2.Distance(this.center, targetList[i].center) > range)//타겟으로 잡고 있던 유닛이 범위를 벗어나면 타겟을 null로
                        {
                            targetList.RemoveAt(i);
                            bulletList.RemoveAt(i);
                            return;
                        }

                        if (bulletTimer > 0.0f)//총알의 속도보다 시간이 지났을 때.
                        {
                            bulletList[i].texture = bulletTexture;
                            //bulletX, Y를 타겟으로 향하게 업데이트 해야한다.
                            double[] xy = getXY(bulletList[i].x, bulletList[i].y, targetList[i].x + 30, targetList[i].y + 30);//출발좌표 : 총알의 현재 좌표, 타겟 좌표 : 움직이는 타겟 좌표, 총알이 휘어져 나갈수도 있겠다.
                            bulletList[i].x += (int)xy[0];//움직여야할 좌표를 원래 좌표에 더해준다.
                            bulletList[i].y += (int)xy[1];
                            bulletList[i].rect = new Rectangle(bulletList[i].x, bulletList[i].y, 32, 32);//총알 rec 재 생성

                            if (bullsoundf == 0)
                            {
                                bulletsound.Play();
                                bullsoundf = 1;
                            }

                            if (bulletList[i].rect.Intersects(targetList[i].unitHit)) //현재 잡고있는 타겟과 총알이 부딪히면 체력을 깎는다.
                            {
                                bullsoundf = 0;
                                bulletList[i].rate = 0;
                                targetList[i].HP -= power;
                                bulletList[i].texture = hitTexture;
                                bulletList[i].x = x * 64 + 16;
                                bulletList[i].y = y * 64 + 16; //총알 발사 위치 재설정
                                if (targetList[i].HP < 0)
                                {
                                    targetList.RemoveAt(i);
                                    bulletList.RemoveAt(i);
                                }
                            }

                            bulletTimer = 0;//시간 초기화
                        }
                    }
                }
            }
            #region 타워가 폭폭이, 짭짭이
            else
            {
                fireRate += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (fireRate > 0.3f)
                {
                    bulletTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (target != null && Vector2.Distance(this.center, target.center) > range)//타겟으로 잡고 있던 유닛이 범위를 벗어나면 타겟을 null로
                    {
                        target = null;
                        bulletX = 99999;
                        bulletY = 99999; //총알 발사 위치 재설정
                        return;
                    }

                    if (target != null && bulletTimer > attackRate)//총알의 속도보다 시간이 지났을 때.
                    {
                        bullet = bulletTexture;
                        //bulletX, Y를 타겟으로 향하게 업데이트 해야한다.
                        double[] xy = getXY(bulletX, bulletY, target.x + 30, target.y + 30);//출발좌표 : 총알의 현재 좌표, 타겟 좌표 : 움직이는 타겟 좌표, 총알이 휘어져 나갈수도 있겠다.
                        bulletX += (int)xy[0];//움직여야할 좌표를 원래 좌표에 더해준다.
                        bulletY += (int)xy[1];
                        bulletRec = new Rectangle(bulletX, bulletY, 32, 32);//총알 rec 재 생성

                        if (bullsoundf == 0)
                        {
                            bulletsound.Play();
                            bullsoundf = 1;
                        }

                        if (bulletRec.Intersects(target.unitHit)) //현재 잡고있는 타겟과 총알이 부딪히면 체력을 깎는다.
                        {
                            bullsoundf = 0;
                            fireRate = 0;
                            target.HP -= power;
                            bullet = hitTexture;
                            bulletX = x * 64 + 16;
                            bulletY = y * 64 + 16; //총알 발사 위치 재설정

                            if (power == 4)//슬라임이면 속도를 몇초간 낮추는 코드
                            {
                                if (target.speed == 0.018f)
                                {
                                    target.speed = 0.06f;
                                }
                                else if (target.speed == 0.04f)
                                {
                                    target.speed = 0.1f;
                                }
                                else if (target.speed == 0.0f)
                                {
                                    target.speed = 0.035f;
                                }
                                (new Thread(new ParameterizedThreadStart(threeSecond))).Start(target);
                            }

                            if (target.HP < 0)
                                target = null;
                        }

                        bulletTimer = 0;//시간 초기화
                    }
                    if (target == null)
                    {
                        bulletX = 99999;
                        bulletY = 99999; //총알 발사 위치 재설정
                    }
                }
            }
            #endregion
        }

        public void Draw(SpriteBatch batch)
        {
            if (power == 3)
            {
                batch.Draw(towerTexture, new Rectangle(x * 64, y * 64, 64, 64), Color.White); //타워 자기 자신 그리기
                foreach(bulletStructure b in bulletList) //타겟이 있다면 총알이 나가고 있을테니까.
                {
                    batch.Draw(b.texture, b.rect, Color.White);
                }
            }
            else
            {
                batch.Draw(towerTexture, new Rectangle(x * 64, y * 64, 64, 64), Color.White); //타워 자기 자신 그리기
                if (target != null) //타겟이 있다면 총알이 나가고 있을테니까.
                {
                    batch.Draw(bullet, bulletRec, Color.White);
                }
            }
        }

        public double[] getXY(int Sx, int Sy, int Dx, int Dy)
        {
            double[] ret = new double[2];
            int distance = 7;

            if (Dx - Sx == 0 && Dy > Sy)
            {
                ret[0] = 0;
                ret[1] = distance;
                return ret;
            }
            else if (Dx - Sx == 0 && Dy < Sy)
            {
                ret[0] = 0;
                ret[1] = -distance;
                return ret;
            }
            else if (Dy - Sy == 0 && Dx > Sx)
            {
                ret[0] = distance;
                ret[1] = 0;
                return ret;
            }
            else if (Dy - Sy == 0 && Dx < Sx)
            {
                ret[0] = -distance;
                ret[1] = 0;
                return ret;
            }

            double a = (double)(Dy - Sy) / (double)(Dx - Sx);
            if (Dx - Sx < 0 && Dy - Sy < 0)
            {
                ret[0] = -distance / Math.Sqrt(1 + Math.Pow(a, 2));
                ret[1] = -(a * distance) / Math.Sqrt(1 + Math.Pow(a, 2));
            }
            else if (Dx - Sx > 0 && Dy - Sy < 0)
            {
                ret[0] = distance / Math.Sqrt(1 + Math.Pow(a, 2));
                ret[1] = (a * distance) / Math.Sqrt(1 + Math.Pow(a, 2));
            }
            else if (Dx - Sx < 0 && Dy - Sy > 0)
            {
                ret[0] = -distance / Math.Sqrt(1 + Math.Pow(a, 2));
                ret[1] = -(a * distance) / Math.Sqrt(1 + Math.Pow(a, 2));
            }
            else//Dx-Sx>0 && Dy-Sy>0
            {
                ret[0] = distance / Math.Sqrt(1 + Math.Pow(a, 2));
                ret[1] = (a * distance) / Math.Sqrt(1 + Math.Pow(a, 2));
            }
            return ret;
        } //start 좌표와 destination 좌표를 넣으면 일정 거리를 리턴한다.
    }
}
