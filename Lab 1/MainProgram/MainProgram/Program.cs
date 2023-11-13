using System;

namespace MainProgram {
    class Program {
        const int MaxItems = 11;
        const int MaxClients = 10;
        const int MaxClusters = 5;
        const double Beta = 1.0;
        const double Rho = 0.9;

        string[] ItemName = {
            "Молоток",
            "Бумага",
            "Шоколадка",
            "Отвёртка",
            "Ручка",
            "Кофе",
            "Гвоздодёр",
            "Карандаш",
            "Конфеты",
            "Дрель",
            "Дырокол"
        };

        int[][] Data = {
            new int[] {0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0 },
            new int[] {0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1 },
            new int[] {0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0 },
            new int[] {0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1 },
            new int[] {1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0 },
            new int[] {0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1 },
            new int[] {1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
            new int[] {0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0 },
            new int[] {0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0 },
            new int[] {0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0 },
        };

        int[][] Clusters;
        int[][] Sum;
        int[] Members;
        int[] Group;
        int N;

        void Initialize() {
            Clusters = new int[MaxClusters][];
            for (int i = 0; i < MaxClusters; i++)
                Clusters[i] = new int[MaxItems];

            Sum = new int[MaxClusters][];
            for (int i = 0; i < MaxClusters; i++)
                Sum[i] = new int[MaxItems];

            Members = new int[MaxClusters];
            Group = new int[MaxClients];

            N = 0;
            for (int i = 0; i < MaxClusters; i++) {
                for (int j = 0; j < MaxItems; j++) {
                    Clusters[i][j] = 0;
                    Sum[i][j] = 0;
                }
                Members[i] = 0;
            }

            for (int i = 0; i < MaxClients; i++) {
                Group[i] = -1;
            }
        }

        int[] AndVectors(int[] V, int[] W) {
            int[] R = new int[MaxItems];
            for (int i = 0; i < MaxItems; i++)
                if (W[i] != 1 || V[i] != 1)
                    R[i] = 0;
                else
                    R[i] = 1;

            return R;
        }

        void UpdateVectors(int K) {
            bool f = true;

            if (K < 0 || K > MaxClusters)
                return;
            for (int i = 0; i < MaxItems; i++) {
                Clusters[K][i] = 0;
                Sum[K][i] = 0;
            }

            for (int i = 0; i < MaxClients; i++) {
                if (Group[i] == K)
                    if (f) {
                        Clusters[K] = (int[])Data[i].Clone();
                        Sum[K] = (int[])Data[i].Clone();
                        f = false;
                    } else {
                        Clusters[K] = AndVectors(Clusters[K], Data[i]);
                        for (int j = 0; j < MaxItems; j++)
                            Sum[K][j] += Data[i][j];
                    }
            }
        }

        int CreateVector(int[] V) {
            int i = -1;

            do {
                i++;
                if (i >= MaxClusters)
                    return -1;
            } while (Members[i] != 0);

            N++;
            Clusters[i] = (int[])V.Clone();
            Members[i] = 1;

            return i;
        }

        int OnesVector(int[] V) {
            int k = 0;
            for (int j = 0; j < MaxItems; j++)
                if (V[j] == 1)
                    k++;

            return k;
        }

        void ExecuteART1() {
            bool exit;
            int count = 50;
            int[] R;
            int PE;
            int P;
            int E;
            bool Test;
            int s;

            do {
                exit = true;
                for (int i = 0; i < MaxClients; i++) {
                    for (int j = 0; j < MaxClusters; j++) {
                        if (Members[j] > 0) {
                            R = AndVectors(Data[i], Clusters[j]);
                            PE = OnesVector(R);
                            P = OnesVector(Clusters[j]);
                            E = OnesVector(Data[i]);
                            Test = PE / (Beta + P) > E / (Beta + MaxItems);

                            if (Test)
                                Test = PE / E < Rho;
                            if (Test)
                                Test = Group[i] != j;
                            if (Test) {
                                s = Group[i];
                                Group[i] = j;

                                if (s > 0) {
                                    Members[s]--;
                                    if (Members[s] == -1)
                                        N--;
                                }

                                Members[j]++;
                                UpdateVectors(s);
                                UpdateVectors(j);
                                exit = false;
                                break;
                            }
                        }
                    }

                    if (Group[i] == -1) {
                        Group[i] = CreateVector(Data[i]);
                        exit = false;
                    }
                }
                count--;
            } while (!exit && count != 0);
        }

        void ShowClusters() {
            for (int i = 0; i < N; i++) {
                Console.Write($"Вектор прототип {i} : ");
                for (int j = 0; j < MaxItems; j++)
                    Console.Write($"{Clusters[i][j]} ");
                Console.WriteLine();
                for (int k = 0; k < MaxClients; k++)
                    if (Group[k] == i) {
                        Console.Write($"Покупатель {k} :      ");
                        for (int j = 0; j < MaxItems; j++)
                            Console.Write($"{Data[k][j]} ");
                        Console.WriteLine();
                    }
            }
        }

        void MakeAdvise(int P) {
            int best = -1;
            int max = 0;

            for (int i = 0; i < MaxItems; i++) {
                if (Data[P][i] == 0 && Sum[Group[P]][i] > max) {
                    best = i;
                    max = Sum[Group[P]][i];
                }
            }
            Console.Write($"Для покупателя {P} ");
            if (best >= 0)
                Console.WriteLine($"рекомендация - {ItemName[best]}");
            else
                Console.WriteLine("нет рекоммендаций");
            Console.Write("Уже выбраны: ");
            for (int i = 0; i < MaxItems; i++)
                if (Data[P][i] != 0)
                    Console.Write($"{ItemName[i]} ");
            Console.WriteLine();
        }

        public void MakeAlg() {
            Initialize();
            ExecuteART1();
            ShowClusters();
            for (int P = 0; P < MaxClients; P++)
                MakeAdvise(P);
        }

        public static void Main() {
            Program program = new Program();
            program.MakeAlg();

            Console.ReadKey();
        }
    }
}


