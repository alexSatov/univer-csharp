using System;
using Pudge;
using Pudge.Player;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PudgeClient
{
    class Program
    {
        const string CvarcTag = "ead2f48b-ed77-438e-ba16-ad2d28c885b5";

        // Пример визуального отображения данных с сенсоров при отладке.
        // Если какая-то информация кажется вам лишней, можете закомментировать что-нибудь.
        static void Print(PudgeSensorsData data)
        {
            Console.WriteLine("---------------------------------");
            if (data.IsDead)
            {
                // Правильное обращение со смертью.
                Console.WriteLine("Ooops, i'm dead :(");
                return;
            }

            Console.WriteLine("I'm here: " + data.SelfLocation);
            Console.WriteLine("My score now: {0}", data.SelfScores);
            Console.WriteLine("Current time: {0:F}", data.WorldTime);

            foreach (var rune in data.Map.Runes)
                Console.WriteLine("Rune! Type: {0}, Size = {1}, Location: {2}", rune.Type, rune.Size, rune.Location);
            foreach (var heroData in data.Map.Heroes)
                Console.WriteLine("Enemy! Type: {0}, Location: {1}, Angle: {2:F}", heroData.Type, heroData.Location,
                    heroData.Angle);
            foreach (var eventData in data.Events)
                Console.WriteLine("I'm under effect: {0}, Duration: {1}", eventData.Event,
                    eventData.Duration - (data.WorldTime - eventData.Start));

            Console.WriteLine("---------------------------------");
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
                args = new[] {"127.0.0.1", "14000"};
            var ip = args[0];
            var port = int.Parse(args[1]);

            // Каждую неделю клиент будет новый. Соотетственно Level1, Level2 и Level3.
            var client = new PudgeClientLevel3();
            var graph = CreateWayGraph();
            var dict = GetEdgesWeight(graph);

            // У этого метода так же есть необязательные аргументы:
            // timeLimit -- время в секундах, сколько будет идти матч (по умолчанию 90)
            // operationalTimeLimit -- время в секундах, отображающее ваш лимит на операции в сумме за всю игру
            // По умолчанию -- 1000. На турнире будет использоваться значение 5. Подробнее про это можно прочитать в правилах.
            // isOnLeftSide -- предпочитаемая сторона. Принимается во внимание во время отладки. По умолчанию true.
            // seed -- источник энтропии для случайного появления рун. По умолчанию -- 0. 
            // При изменении руны будут появляться в другом порядке
            // speedUp -- ускорение отладки в два раза. Может вызывать снижение FPS на слабых машинах
            var sensorData = client.Configurate(ip, port, CvarcTag);
            // Пудж узнает о всех событиях, происходящих в мире, с помощью сенсоров.
            // Для передачи и представления данных с сенсоров служат объекты класса PudgeSensorsData.
            Print(sensorData);

            // Каждое действие возвращает новые данные с сенсоров.
            //sensorData = client.Move();
            //Print(sensorData);

            // Для удобства, можно подписать свой метод на обработку всех входящих данных с сенсоров.
            // С этого момента любое действие приведет к отображению в консоли всех данных
            client.SensorDataReceived += Print;
            var pudge = new Pudge(sensorData);
            // Угол поворота указывается в градусах, против часовой стрелки.
            // Для поворота по часовой стрелке используйте отрицательные значения.
            //client.Rotate(-45);

            //client.Move(100);
            //lient.Wait(45);

            // Так можно хукать, но на первом уровне эта команда будет игнорироваться.
            //client.Hook();

            // Пример длинного движения. Move(100) лучше не писать. Мало ли что произойдет за это время ;) 
            //var previousVector = new Vector(new Node(new Point(0, 0)), graph[0]);
            //var angle = 180.0;
            //for (var i = 0; i < graph.Length - 1; i++)
            //{
            //    var vect = new Vector(graph[i], graph[i + 1]);
            //    angle -= GetRotationAngle(previousVector, vect);
            //    previousVector = vect;
            //    client.Rotate(-angle);
            //    client.Move(vect.Length);
            //}
            /*bool exit = false;
            while (!exit)
            {
                for (var i = 0; i < graph.Length - 1; i++)
                {
                    var nextDirection = new Vector(graph[i], graph[i + 1]);
                    var rotationAngle = GetRotationAngle(pudge.Direction, nextDirection);
                    pudge.Direction = nextDirection;
                    sensorData = client.Rotate(rotationAngle);
                    while (SegmentLength(pudge.Location, graph[i + 1].Position) > 7.8)
                    {
                        if (sensorData.WorldTime > 85)
                        {
                            exit = true;
                            break;
                        }
                        sensorData = client.Move(8);
                        pudge.Location = new Point((int)sensorData.SelfLocation.X, (int)sensorData.SelfLocation.Y);
                    }

                    if (i == 5)
                        client.Wait(5);
                }
            }*/

            var edgWeights = GetEdgesWeight(graph);
            var targets = getTargetNodes();
            var target_index = 0;
            var path = Graph.Dijkstra(graph, edgWeights, graph[0], targets[target_index]);
            var i = 0;

            while (true)
            {
                if (sensorData.WorldTime > 85)
                    break;
                if (i < path.Count)
                {
                    if (path[i].Equals(targets[target_index]))
                    {
                        if (target_index == targets.Count - 1)
                            target_index = 0;
                        else
                            target_index++;
                        //edgWeights = GetEdgesWeight(graph);
                        path = Graph.Dijkstra(graph, edgWeights, path[i], targets[target_index]);
                        i = 0;
                        continue;
                    }

                    var nextDirection = new Vector(path[i], path[i + 1]);
                    var rotationAngle = GetRotationAngle(pudge.Direction, nextDirection);
                    pudge.Direction = nextDirection;
                    sensorData = client.Rotate(rotationAngle);
                    while (SegmentLength(pudge.Location, path[i + 1].Position) >= 12)
                    {
                        //if (search_rune(path[i], sensorData))
                        //{
                        //    edgWeights = GetEdgesWeight(graph);
                        //    path = Graph.Dijkstra(graph, edgWeights, path[i], targets[target_index]);
                        //    i = 0;
                        //}

                        if (sensorData.Map.Heroes.Count > 0)
                        {
                            var ang = AngleToVictim(sensorData, pudge, client);
                            sensorData = client.Rotate(ang);
                            sensorData = client.Hook();
                            sensorData = client.Rotate(-ang);
                        }

                        sensorData = client.Move(8);
                        if (sensorData.WorldTime > 85)
                            break;
                        pudge.Location = new Point((int) sensorData.SelfLocation.X, (int) sensorData.SelfLocation.Y);
                    }
                    i++;
                }
            }
            client.Exit();
        }

        public static double AngleToVictim(PudgeSensorsData data, Pudge pudge, PudgeClientLevel3 client)
        {
            foreach (var enemy in data.Map.Heroes)
            {
                var targetRotate = new Vector(new Node(new Point((int) data.SelfLocation.X, (int) data.SelfLocation.Y)),
                    new Node(new Point((int) enemy.Location.X, (int) enemy.Location.Y)));
                var rotationAngle = GetRotationAngle(pudge.Direction, targetRotate);
                return rotationAngle;
            }
            return 0;
        }

        public static Dictionary<Edge, double> GetEdgesWeight(Graph graph)
        {
            var edges = graph.Nodes.SelectMany(node => node.Edges).ToArray();
            return edges.ToDictionary(edge => edge, weight => weight.Weight);
        }

        public static Graph CreateWayGraph()
        {
            var graphData = File.ReadAllLines("graphData.txt");

            var nodes = graphData
                .Where(line => line != "")
                .Where(line => line.Split(' ').Count() == 3)
                .Select(line =>
                {
                    var nodeData = line.Split(' ');
                    return new Node(new Point(int.Parse(nodeData[0]), int.Parse(nodeData[1])), nodeData[2]);
                })
                .ToList();

            var connections = graphData
                .Where(line => line != "")
                .Where(line => line.Split(' ').Count() == 2)
                .Select(line =>
                {
                    var connectionData = line.Split(' ');
                    return Tuple.Create(int.Parse(connectionData[0]), int.Parse(connectionData[1]));
                })
                .ToList();

            var wayGraph = new Graph(nodes);
            foreach (var connection in connections)
            {
                wayGraph.Connect(connection.Item1, connection.Item2);
            }
            return wayGraph;
        }

        public static double AngleBetweenVectors(Vector v1, Vector v2)
        {
            return Math.Acos((v1*v2)/(v1.Length*v2.Length))*180/Math.PI;
        }

        public static double GetRotationAngle(Vector v1, Vector v2)
        {
            double rotationAngle = 0;

            if (v2.Y >= 0 && v1.Y >= 0)
            {
                rotationAngle = AngleBetweenVectors(v1, v2);
                if (v1.Angle > v2.Angle)
                    rotationAngle *= -1;
            }

            if (v2.Y <= 0 && v1.Y <= 0)
            {
                rotationAngle = AngleBetweenVectors(v1, v2);
                if (v1.Angle < v2.Angle)
                    rotationAngle *= -1;
            }

            if (v2.Y > 0 && v1.Y < 0)
            {
                rotationAngle = AngleBetweenVectors(v1, v2);
                if (v1.Angle + v2.Angle > Math.PI)
                    rotationAngle *= -1;
            }

            if (v2.Y < 0 && v1.Y > 0)
            {
                rotationAngle = AngleBetweenVectors(v1, v2);
                if (v1.Angle + v2.Angle < Math.PI)
                    rotationAngle *= -1;
            }

            return rotationAngle;
        }

        public static double SegmentLength(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        public static void PrintPath(List<Node> path)
        {
            foreach (var node in path)
            {
                Console.Write($"({node.Position.X}, {node.Position.Y}), ");
            }
        }

        //public static bool search_rune(Node current_node, PudgeSensorsData data)
        //{
        //    bool result = false;
        //    foreach (var node in current_node.IncidentNodes)
        //    {
        //        foreach (var rune in data.Map.Runes)
        //        {
        //            if (rune.Location.X == node.Position.X && rune.Location.Y == node.Position.Y)
        //            {
        //                switch (rune.Type.ToString())
        //                {
        //                    case "Invisibility":
        //                        node.Type = NodeType.invisibility;
        //                        break;
        //                    case "Haste":
        //                        node.Type = NodeType.haste;
        //                        break;
        //                    case "GoldXP":
        //                        node.Type = NodeType.bounty;
        //                        break;
        //                }

        //                result = true;
        //            }
        //        }
        //    }

        //    return result;
        //}

        public static List<Node> getTargetNodes()
        {
            return new List<Node>()
            {
                new Node(new Point(130, -130)),
                new Node(new Point(130, 130)),
                new Node(new Point(-130, 130)),
                new Node(new Point(-130, -130))
            };
        }
    }

    class Pudge
    {
        public Point Location { get; set; }
        public Vector Direction { get; set; }
        public double Angle { get; set; }

        public Pudge(PudgeSensorsData data)
        {
            Location = new Point((int) data.SelfLocation.X, (int) data.SelfLocation.Y);
            Angle = data.SelfLocation.Angle;
            Direction = new Vector(new Node(new Point(0, 0)), new Node(new Point(1, 1)));
        }
    }
}