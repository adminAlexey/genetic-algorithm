namespace program
{
    class Program
    {
        static int task = 0; //количество заданий
        static Random random = new(); //переменная для рандома

        class Person //особь
        {
            public int[] dnk; //набор значений 0-256
            public int[] weight; //вес
            public int[] numProc; //номер процессора
            public int load; //загрузка

            public Person(int[] T)
            {
                weight = new int[task];
                dnk = new int[task];
                numProc = new int[task];

                for (int i = 0; i < task; i++)
                {
                    dnk[i] = random.Next(0, 256);
                }
                weight = T.OrderBy(v => random.Next()).ToArray<int>();
                load = 0;
            }

            public Person(Person copy)
            {
                weight = new int[task];
                dnk = new int[task];
                numProc = new int[task];
                for (int i = 0; i < task; i++)
                {
                    weight[i] = copy.weight[i];
                    dnk[i] = copy.dnk[i];
                }
                load = copy.load;
            }
        }

        static void onProcessor(int procCount, Person person) //посчитать нагрузку на процессор
        {
            int mid = 256 / procCount;
            int min = 0;
            int max = mid;
            int n = procCount;
            int m = n;

            while (n != 0) 
            {
                for (int i = 0; i < task; i++)
                {
                    if (person.dnk[i] > min && person.dnk[i] <= max)
                    {
                        person.numProc[i] = m - n;
                    }
                }
                min = max;
                max += mid;
                n--;
            }
        }

        static void showPerson(Person person)//показать особь
        {
            foreach (int d in person.dnk)
                Console.Write("{0,4}", d);
            Console.WriteLine();
            foreach (int w in person.weight)
                Console.Write("{0,4}", w);
            Console.WriteLine();
            Console.WriteLine($"Нагрузка {person.load}\n\n");

        }
        static void showGeneration(Person[] persons)//показать поколение
        {
            for (int i = 0; i < persons.Length; i++)
            {
                foreach (int d in persons[i].dnk)
                    Console.Write("{0,4}", d);
                Console.WriteLine();
                foreach (int w in persons[i].weight)
                    Console.Write("{0,4}", w);
                Console.WriteLine();
                Console.WriteLine($"\nНагрузка {persons[i].load}\n\n");
            }
            Console.WriteLine();
        }

        static int calculateLoad(Person person, int proc)//посчитать максимальную нагрузку
        {
            int[] load = new int[proc];
            int count = 0;
            int maxLoad = 0;

            while (count < proc)
            {
                for (int i = 0; i < task; i++)
                {
                    if (person.numProc[i] == count)
                    {
                        load[count] += person.weight[i];
                    }
                }
                count++;
            }

            foreach (int l in load)
            {
                if (l > maxLoad)
                    maxLoad = l;
            }

            person.load = maxLoad;
            return person.load;
        }

        static int changeGen(int z, int index)//изменить ген
        {
            int[] mass = new int[8];
            for (int i = 7; i >= 0; i--)
            {
                mass[i] = z % 2;
                z /= 2;
            }
            foreach (int m in mass)
                Console.Write(m);
            Console.WriteLine();
            if (mass[7 - index] == 1)
            {
                mass[7 - index] = 0;
            }
            else
            {
                mass[7 - index] = 1;
            }
            foreach (int m in mass)
                Console.Write(m);
            Console.WriteLine();
            int P = 0;
            for (int i = 7; i >= 0; i--)
            {
                if (mass[i] == 1)
                {
                    P += (int)Math.Pow(2, 7 - i);
                }
            }
            return P;
        }

        static bool repeat(Person[] persons, int best)
        {
            int bestPerson = persons[minLoadIndex(persons)].load;
            return bestPerson == best;
        } 

        static int minLoadIndex(Person[] persons) //возвращает индекс с минимальной нагрузкой
        {
            int[] load = new int[persons.Length];

            for (int i = 0; i < persons.Length; i++)
            {
                load[i] = persons[i].load;
            }

            int minLoad = load.Min();

            return Array.IndexOf<int>(load, minLoad);
        }

        static int maxLoadIndex(Person[] persons, int index) //возвращает индекс с максимальной нагрузкой
        {
            int[] load = new int[persons.Length];

            for (int i = 0; i < persons.Length; i++)
            {
                if (i != index)
                {
                    load[i] = persons[i].load;
                }
                else load[i] = 0;
            }

            int maxLoad = load.Max();

            return Array.IndexOf<int>(load, maxLoad);
        }

        static void mutation(int Pm, Person ancestor1, Person ancestor2, int proc) //провести мутацию
        {
            int Rm = random.Next(0, 100); //вероятность мутации
            if (Rm < Pm)
            {
                Console.WriteLine($"\nВероятность мутации = {Pm} < Заданная вероятность = {Rm}");
                Console.WriteLine("\nНет мутации");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine($"\nВероятность мутации = {Pm} > Заданная вероятность = {Rm}");
                Console.WriteLine("\n1 потомок");
                int gen1 = random.Next(task);
                Console.WriteLine($"\nГен для мутации {gen1 + 1}");
                int bit1 = random.Next(8);
                Console.WriteLine($"\nБит для мутации  {bit1 + 1}");
                Console.WriteLine($"\nГен для мутации {ancestor1.dnk[gen1]}");

                int new_z_1 = changeGen(ancestor1.dnk[gen1], bit1);
                Console.WriteLine($"\nГен после мутации {new_z_1}");
                ancestor1.dnk[gen1] = new_z_1;
                onProcessor(proc, ancestor1);
                ancestor1.load = calculateLoad(ancestor1, proc);
                Console.WriteLine("\nПотомок после мутации");
                showPerson(ancestor1);

                Console.WriteLine("\n2 потомок");
                int gen2 = random.Next(task);
                Console.WriteLine($"\nГен для мутации {gen2 + 1}");
                int bit2 = random.Next(8);
                Console.WriteLine($"\nБит для мутации {bit2 + 1}");
                Console.WriteLine($"\nГен для мутации {ancestor2.dnk[gen2]}");

                int new_z_2 = changeGen(ancestor2.dnk[gen2], bit2);
                Console.WriteLine($"\nГен после мутации {new_z_2}");
                ancestor2.dnk[gen2] = new_z_2;
                onProcessor(proc, ancestor2);
                ancestor2.load = calculateLoad(ancestor2, proc);
                Console.WriteLine("\nПотомок после мутации");
                showPerson(ancestor2);
            }
        }

        static void Rg(Person[] persons, Person ancestor1, Person ancestor2)
        {
            int Inn_max_1 = maxLoadIndex(persons, -1);
            persons[Inn_max_1] = (persons[Inn_max_1].load >= ancestor1.load) ? ancestor1 : persons[Inn_max_1];
            string rezul1 = (persons[Inn_max_1].load == ancestor1.load) ? "Замена особи " + (Inn_max_1 + 1) + " на потомок 1 " : "потомок 1 не включен в популяцию";
            Console.WriteLine(rezul1);
            int Inn_max_2 = maxLoadIndex(persons, Inn_max_1);
            persons[Inn_max_2] = (persons[Inn_max_2].load >= ancestor2.load) ? ancestor2 : persons[Inn_max_2];
            string rezul2 = (persons[Inn_max_2].load == ancestor2.load) ? "Замена особи " + (Inn_max_2 + 1) + " на потомок 2 " : "потомок 2 не включен в популяцию";
            Console.WriteLine(rezul2);
            Console.WriteLine();
        }

        //дописать
        static void crossover(int proc, Person[] persons, int Pc, int Pm, int j)
        {
            int Rс = random.Next(0, 100); //рандомное число для определения вероятности кроссовера
            Console.WriteLine("____________________________________________________________________");

            if (Pc > Rс)
            {
                Console.WriteLine($"Вероятность кроссовера = {Rс} < заданная вероятность = {Pc}");

                //выбор особей
                int H = random.Next(0, persons.Length);
                int K = random.Next(0, persons.Length);
                while (H == j || K == j || H == K)
                {
                    H = random.Next(0, persons.Length);
                    K = random.Next(0, persons.Length);
                }
                Console.WriteLine($"\nВыбранны особи номер {H + 1} и {K + 1}");
                Console.WriteLine($"\nОсобь  {H + 1}");
                showPerson(persons[H]);
                Console.WriteLine($"\nОсобь  {K + 1}");
                showPerson(persons[K]);

                if (calculateLoad(persons[H], proc) < calculateLoad(persons[K], proc))
                {
                    Console.WriteLine($"Особь {H + 1} лучше особи {K + 1}");
                    Console.WriteLine($"Родителями являются особи {j + 1} и {H + 1}");
                    K = j;
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine($"Особь {K + 1} лучше особи {H + 1}");
                    Console.WriteLine($"Родителями являются особи {j + 1} и {K + 1}");
                    H = j;
                    Console.WriteLine();
                }

                int point = random.Next(1, task);
                Console.WriteLine($"\nВыбранная точка кроссовера: {point}");

                Person ancestor1 = new Person(persons[H]);
                Person ancestor2 = new Person(persons[K]);

                for (int i = point; i < task; i++)
                {
                    int zaps = ancestor1.dnk[i];
                    ancestor1.dnk[i] = ancestor2.dnk[i];
                    ancestor2.dnk[i] = zaps;
                }

                onProcessor(proc, ancestor1);
                ancestor1.load = calculateLoad(ancestor1, proc);
                onProcessor(proc, ancestor2);
                ancestor2.load = calculateLoad(ancestor2, proc);

                Console.WriteLine("\nПотомки кроссовера");
                Console.WriteLine("\nПотомок № 1");
                showPerson(ancestor1);
                Console.WriteLine("\nПотомок № 2");
                showPerson(ancestor2);

                mutation(Pm, ancestor1, ancestor2, proc);
                Rg(persons, ancestor1, ancestor2);
            }
            else
            {
                Console.WriteLine($"Вероятность кроссовера = {Rс} > Заданная вероятность = {Pc}" +
                    "\nОсобь переходит в новое поколение без изменений");
            }

            Console.WriteLine("Новая популяция");
            showGeneration(persons);
        }
        static void Main()
        {
            Console.Write("Кол-во процесоров:");
            int proc = Convert.ToInt32(Console.ReadLine());
            Console.Write("Кол-во заданий:");
            task = Convert.ToInt32(Console.ReadLine());
            Console.Write("Особей в популяции:");
            int countPerson = Convert.ToInt32(Console.ReadLine());
            Console.Write("Минимальный вес");
            int minWeight = Convert.ToInt32(Console.ReadLine());
            Console.Write("Максимальный вес:");
            int maxWeight = Convert.ToInt32(Console.ReadLine());
            Console.Write("Шанс кроссовера:");
            int Pc = Convert.ToInt32(Console.ReadLine());
            Console.Write("Шанс мутации:");
            int Pm = Convert.ToInt32(Console.ReadLine());
            Console.Write("Шанс сохранения лучшего поколения: ");
            int Pb = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine();

            Console.WriteLine("Начальная популяция");
            int[] weight = new int[task];
            for (int i = 0; i < task; i++)
            {
                weight[i] = random.Next(minWeight, maxWeight);
            }

            Person[] persons = new Person[countPerson];
            persons[0] = new Person(weight); //почему 

            for (int i = 1; i < persons.Length; i++)
            {
                persons[i] = new Person(persons[i - 1].weight); //почему
                System.Threading.Thread.Sleep(150);
            }

            for (int i = 0; i < countPerson; i++)
            {
                onProcessor(proc, persons[i]); //почему
                persons[i].load = calculateLoad(persons[i], task); //почему
            }

            showGeneration(persons);

            int countGeneration = 1;
            int best = Pb;
            while (best!=0)
            {
                int j = 0;
                while (j < countPerson) 
                {
                    int bestPerson = persons[minLoadIndex(persons)].load;
                    crossover(proc, persons, Pc, Pm, j);
                    Console.WriteLine($"Популяция № {countGeneration}");
                    j++;
                    countGeneration++;

                    if (repeat(persons, bestPerson))
                    {
                        best--;
                    }
                    else
                    {
                        best = Pb;
                    }

                    if (best == 0)
                    {
                        Console.WriteLine("Решение: ");
                        showPerson(persons[minLoadIndex(persons)]);
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"Лучшая особь в популяции {minLoadIndex(persons) + 1}");
                    }
                }

            }
            Console.ReadKey();
        }
    }
}