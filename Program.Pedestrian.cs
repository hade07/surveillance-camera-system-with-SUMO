using System;
using System.Collections.Generic;
using System.Text;

namespace surveillance_system
{
    public partial class Program
    {
        public class SurveillanceTarget
        {
            public int ID; // 240829 modified. string -> int  by DanHa Kim
            public double X;
            public double Y;

            public double DST_X;
            public double DST_Y;

            public double Direction;  //이동방향
            public double Velocity; //이동 속도
            public double Unit_Travel_Dist; //단위시간당 이동 거리

            public double MAX_Dist_X;
            public double MAX_Dist_Y;

            public double ground; // Z ?

            public bool Enable = false; // 240823 김단하 추가, 보행자가 시뮬레이션에 등장하는지 여부. false가 default

            /* ==================================
            /   추가
            /   line 67~ 87 변수 
            /   line 91~127 define_PED 함수 (깃허브 define_PED.m)
            /           라인 500~507에서 ped 위치 변수 사용하는데 이걸  define_PED에서 처리하는거 같아서 구현해놨습니다
            / ===================================*/
            public double W;
            public double H;
            public double D1;
            public double D2;
            public double W2;

            public double[] Pos_H1 = new double[2];
            public double[] Pos_H2 = new double[2];
            public double[] Pos_V1 = new double[2];
            public double[] Pos_V2 = new double[2];

            public double[,] Spatial_Resolution; // 대상이 감시 카메라에 얼마나 잘 탐지되었는지..해상도?

            public int N_Surv; //number of surveillance camera viewing this target.

            public int TTL; //객체 생존(유효) 시간
            public void define_PED(
                double Width,
                double Height,
                double DST_X,
                double DST_Y,
                double Velocity,
                int N_CCTV
            )
            {
                //Random rand = new Random(randSeed); // modified by 0boo 23-01-27

                this.W = Width;
                this.H = Height;
                this.D1 = 90 * Math.PI / 180;   // modified by 0BoO, deg -> rad
                this.D2 = (180 + 90 * rand.NextDouble()) * Math.PI / 180; // modified by 0BoO, deg -> rad
                this.W2 = this.W / 2;

                //this.Pos_H1[0] =
                //    Math.Round(this.W2 * Math.Cos(D1 + this.Direction) + this.X, 2);
                //this.Pos_H1[1] =
                //    Math.Round(this.W2 * Math.Sin(D1 + this.Direction) + this.Y, 2);
                //this.Pos_H2[0] =
                //    Math.Round(this.W2 * Math.Cos(D2 + this.Direction) + this.X, 2);
                //this.Pos_H2[1] =
                //    Math.Round(this.W2 * Math.Sin(D2 + this.Direction) + this.Y, 2);

                this.Pos_V1[0] = this.X;
                this.Pos_V1[1] = this.H;

                this.Pos_V2[0] = this.X;
                // [220331] may be height of ground, instead of 0
                this.Pos_V2[1] = 0;

                this.DST_X = DST_X;
                this.DST_Y = DST_Y;
                this.Velocity = Velocity;

                this.Unit_Travel_Dist = Velocity * aUnitTime;

                // % for performance measure
                this.N_Surv = 0;

                this.Spatial_Resolution = new double[N_CCTV, 11];
                // [Out of Range(0), Direction Miss(-1), Detected(1)]  SR(Width1, Width2, Height1,Height2) numPixels(Width1, Width2, Height1,Height2) areaPixels(min, max)
            }

            // 241025 김단하 define_PED 오버로딩. DST_X 같은 목적지, 속도 설정 관련 필요 없어보여서 삭제./
            public void define_PED(
                double Width,
                double Height,
                int N_CCTV
            )
            {
                //Random rand = new Random(randSeed); // modified by 0boo 23-01-27

                this.W = Width;
                this.H = Height;
                this.D1 = 90 * Math.PI / 180;   // modified by 0BoO, deg -> rad
                this.D2 = (180 + 90 * rand.NextDouble()) * Math.PI / 180; // modified by 0BoO, deg -> rad
                this.W2 = this.W / 2;

                //this.Pos_H1[0] =
                //    Math.Round(this.W2 * Math.Cos(D1 + this.Direction) + this.X, 2);
                //this.Pos_H1[1] =
                //    Math.Round(this.W2 * Math.Sin(D1 + this.Direction) + this.Y, 2);
                //this.Pos_H2[0] =
                //    Math.Round(this.W2 * Math.Cos(D2 + this.Direction) + this.X, 2);
                //this.Pos_H2[1] =
                //    Math.Round(this.W2 * Math.Sin(D2 + this.Direction) + this.Y, 2);

                this.Pos_V1[0] = this.X;
                this.Pos_V1[1] = this.H;

                this.Pos_V2[0] = this.X;
                // [220331] may be height of ground, instead of 0
                this.Pos_V2[1] = 0;

                //this.Velocity = Velocity;

                //this.Unit_Travel_Dist = Velocity * aUnitTime;

                // % for performance measure
                this.N_Surv = 0;

                this.Spatial_Resolution = new double[N_CCTV, 11];
                // [Out of Range(0), Direction Miss(-1), Detected(1)]  SR(Width1, Width2, Height1,Height2) numPixels(Width1, Width2, Height1,Height2) areaPixels(min, max)
            }


            public void setDirection()
            {
                double[] A = new double[2];
                A[0] = DST_X - X;
                A[1] = DST_Y - Y;

                double[] B = { 0.001, 0 };
                Direction = Math.Round(Math.Acos(InnerProduct(A, B) / (Norm(A) * Norm(B))), 8); //목적지와 현재 위치 이용하여 사람의 이동방향 각도(라디안)를 구한다.
                if (Y > DST_Y)
                {
                    Direction = Math.Round(2 * Math.PI - Direction, 8);
                }

                //240905 김단하. Pos_H1, Pos_H2는 X축과 보행자 현 위치, 목적자 위치간 벡터의 각도를 이용해 구함

                this.Pos_H1[0] = //어깨1의 좌표 업데이트
                    Math.Round(this.W2 * Math.Cos(D1 + this.Direction) + this.X, 2);
                this.Pos_H1[1] =
                    Math.Round(this.W2 * Math.Sin(D1 + this.Direction) + this.Y, 2);
                this.Pos_H2[0] = //어깨2
                    Math.Round(this.W2 * Math.Cos(D2 + this.Direction) + this.X, 2);
                this.Pos_H2[1] =
                    Math.Round(this.W2 * Math.Sin(D2 + this.Direction) + this.Y, 2);
            }

            //
            public void setDirection(double x2, double y2)
            {
                double[] A = new double[2];
                A[0] = x2 - X;
                A[1] = y2 - Y;

                double[] B = { 0.001, 0 };
                Direction = Math.Round(Math.Acos(InnerProduct(A, B) / (Norm(A) * Norm(B))), 8);
                if (Y > y2)
                {
                    Direction = Math.Round(2 * Math.PI - Direction, 8);
                }

                this.Pos_H1[0] =
                    Math.Round(this.W2 * Math.Cos(D1 + this.Direction) + this.X, 2);
                this.Pos_H1[1] =
                    Math.Round(this.W2 * Math.Sin(D1 + this.Direction) + this.Y, 2);
                this.Pos_H2[0] =
                    Math.Round(this.W2 * Math.Cos(D2 + this.Direction) + this.X, 2);
                this.Pos_H2[1] =
                    Math.Round(this.W2 * Math.Sin(D2 + this.Direction) + this.Y, 2);
            }

            // 240812 김단하
            public void setDirection(int personID, double x2, double y2)
            { //x2,y2가 다음 목적지
                int[] person_ID = new int[1];
                double[] A = new double[2];
                A[0] = x2 - X;
                A[1] = y2 - Y;

                double[] B = { 0.001, 0 };
                Direction = Math.Round(Math.Acos(InnerProduct(A, B) / (Norm(A) * Norm(B))), 8);
                if (Y > y2)
                {
                    Direction = Math.Round(2 * Math.PI - Direction, 8);
                }

                this.Pos_H1[0] = // 오른쪽 어깨
                    Math.Round(this.W2 * Math.Cos(D1 + this.Direction) + this.X, 2);
                this.Pos_H1[1] =
                    Math.Round(this.W2 * Math.Sin(D1 + this.Direction) + this.Y, 2);
                this.Pos_H2[0] = // 왼쪽 어깨
                    Math.Round(this.W2 * Math.Cos(D2 + this.Direction) + this.X, 2);
                this.Pos_H2[1] =
                    Math.Round(this.W2 * Math.Sin(D2 + this.Direction) + this.Y, 2);
            }

            //240905 김단하. SUMO output인 fcd_output 파일의 angle 피쳐를 읽어올 수 있다면?
            public void setDirectionbyAngle(int personID, double x2, double y2)
            {
                //
                // x2,y2가 다음 목적지. X,Y가 지금 보행자의 머리 위치. Pos_H1[0], Pos_H1[1]: 오른쪽어깨, Pos_H2[0], Pos_H2[1]: 왼쪽어깨
                int[] person_ID = new int[1];
                double[] A = new double[2];
                A[0] = x2 - X;
                A[1] = y2 - Y;
            }

            public Boolean isArrived()
            {
                double[] dist = { X - DST_X, Y - DST_Y };
                if (Norm(dist) < 1000) return true;
                else return false;
            }

            public void printPedInfo()
            {
                Console.WriteLine("======================Info======================");
                Console.WriteLine("출발지 : ({0},{1}) \n", this.X, this.Y);
                Console.WriteLine("목적지 : ({0},{1}) \n", this.DST_X, this.DST_Y);
                Console.WriteLine("방향 각도(라디안) : {0} \n", this.Direction);
                //Console.WriteLine("속도 : {0} \n", this.Velocity);
                //Console.WriteLine("단위이동거리 : {0} \n", this.Unit_Travel_Dist);
                Console.WriteLine("Pos_H1 : ({0},{1})   Pos_H2 : ({2},{3})  \n",
                    this.Pos_H1[0], this.Pos_H1[1], this.Pos_H2[0], this.Pos_H2[1]);
                Console.WriteLine("Pos_V1 : ({0},{1})   Pos_V2 : ({2},{3}) \n",
                    this.Pos_V1[0], this.Pos_V1[1], this.Pos_V2[0], this.Pos_V2[1]);
                Console.WriteLine("TTL : {0} \n", this.TTL);
            }

            public Boolean outOfRange()
            {
                if (X < 0 || X > road.mapSize || Y < 0 || Y > road.mapSize)
                {
                    return true;
                }
                return false;
            }
        }

        interface Movable
        {
            void move(); // 초당 이동
            void updateDestination();
            void upVelocity(); // 속도 증가
            void downVelocity(); // 속도 감소
        }

        interface Person : Movable { }

        interface Vehicle : Movable { }

        public class Pedestrian : SurveillanceTarget, Person
        {
            //기본생성자. 240901. 김단하. 임의로 0으로 초기화.
            public Pedestrian()
            {
                this.X = 0;
                this.Y = 0;
            }
            // 240811. 김단하. SUMO 기반 보행자 mobility 를 위한 생성자
            public Pedestrian(int personID, double x, double y)
            {
                this.ID = personID;
                this.X = x;
                this.Y = y;
                //this.H = height;
            }
            //240811. 김단하. 보행자가 시뮬레이션에 등장하는지 여부 바꿔주는
            public void setEnable()
            {
                this.Enable = true;
            }

            public void move()
            {
                // 이동
                X += Unit_Travel_Dist * Math.Cos(Direction);
                Y += Unit_Travel_Dist * Math.Sin(Direction);
                // Console.WriteLine("move to {0} {1} ", X, Y);

                Pos_H1[0] += Unit_Travel_Dist * Math.Cos(Direction);
                Pos_H1[1] += Unit_Travel_Dist * Math.Sin(Direction);

                Pos_H2[0] += Unit_Travel_Dist * Math.Cos(Direction);
                Pos_H2[1] += Unit_Travel_Dist * Math.Sin(Direction);

                Pos_V1[0] += Unit_Travel_Dist * Math.Cos(Direction);
                Pos_V2[0] += Unit_Travel_Dist * Math.Cos(Direction);

                // 목적지 도착 검사
                if (isArrived() || outOfRange())
                {
                    // Index out of range
                    updateDestination();
                    setDirection();
                }
            }
            public void downVelocity()
            {
                this.Velocity -= 0.01f;
            }
            public void upVelocity()
            {
                this.Velocity += 0.01f;
            }
            public void updateDestination() //240820 김단하 이거 인접 교차로 값 갖고오는 것 같다.
            {
                double[,] newPos = road.getPointOfAdjacentRoad(road.getIdxOfIntersection(X, Y));
                DST_X = Math.Round(newPos[0, 0]); //그런데 이 코드는 뭘까?(._.
                DST_Y = Math.Round(newPos[0, 1]);
            }

        }

        public class Car : SurveillanceTarget, Vehicle
        {
            public void move()
            {
                // 이동
                X += Unit_Travel_Dist * Math.Cos(Direction);
                Y += Unit_Travel_Dist * Math.Sin(Direction);

                Pos_H1[0] += Unit_Travel_Dist * Math.Cos(Direction);
                Pos_H1[1] += Unit_Travel_Dist * Math.Sin(Direction);

                Pos_H2[0] += Unit_Travel_Dist * Math.Cos(Direction);
                Pos_H2[1] += Unit_Travel_Dist * Math.Sin(Direction);

                Pos_V1[0] += Unit_Travel_Dist * Math.Cos(Direction);
                Pos_V2[0] += Unit_Travel_Dist * Math.Cos(Direction);

                // 목적지 도착 검사
                if (isArrived() || outOfRange())
                {
                    updateDestination();
                    setDirection();
                }
            }

            public void updateDestination()
            {
                double[,] newPos = road.getPointOfAdjacentIntersection(road.getIdxOfIntersection(X, Y), X, Y);
                DST_X = Math.Round(newPos[0, 0]);
                DST_Y = Math.Round(newPos[0, 1]);
            }

            public void downVelocity()
            {
                this.Velocity -= 0.1f;
            }
            public void upVelocity()
            {
                this.Velocity += 0.1f;
            }
        }

        public class ElectricScooter : SurveillanceTarget, Vehicle
        {
            public void move()
            {
                // 이동
                X += Unit_Travel_Dist * Math.Cos(Direction);
                Y += Unit_Travel_Dist * Math.Sin(Direction);

                Pos_H1[0] += Unit_Travel_Dist * Math.Cos(Direction);
                Pos_H1[1] += Unit_Travel_Dist * Math.Sin(Direction);

                Pos_H2[0] += Unit_Travel_Dist * Math.Cos(Direction);
                Pos_H2[1] += Unit_Travel_Dist * Math.Sin(Direction);

                Pos_V1[0] += Unit_Travel_Dist * Math.Cos(Direction);
                Pos_V2[0] += Unit_Travel_Dist * Math.Cos(Direction);

                // 목적지 도착 검사
                if (isArrived() || outOfRange())
                {
                    updateDestination();
                    setDirection();
                }
            }
            public void updateDestination()
            {

            }
            public void downVelocity()
            {

            }
            public void upVelocity()
            {

            }
        }
    }
}
