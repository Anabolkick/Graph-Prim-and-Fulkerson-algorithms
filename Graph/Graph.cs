using System;
using System.Collections.Generic;

namespace Graph
{
    internal class Graph
    {
        private readonly List<Edge> _edges = new List<Edge>();

        private readonly List<Vertex> _vertexes = new List<Vertex>();

        public Graph(bool oriented)
        {
            IsOriented = oriented;
        }

        public bool IsOriented { get; set; }
        public int VertexCount => _vertexes.Count;
        public int EdgesCount => _edges.Count;

        public void AddVertex(Vertex vertex)
        {
            _vertexes.Add(vertex);
        }

        public void AddEdge(Vertex from, Vertex to)
        {
            var edge = new Edge(from, to);
            _edges.Add(edge);
        }

        public void AddEdge(Vertex from, Vertex to, int wieght)
        {
            var edge = new Edge(from, to, wieght);
            _edges.Add(edge);
        }

        public int[][,] GetMetrix() //matrix smezhnosti
        {
            var matrix = new int[2][,];
            matrix[0] = new int[VertexCount, VertexCount];
            matrix[1] = new int[VertexCount, VertexCount];

            foreach (var edge in _edges)
            {
                var row = edge.From.Number - 1;
                var column = edge.To.Number - 1;

                matrix[0][row, column] = edge.Weight;
                matrix[1][row, column] = 0;
            }

            return matrix;
        }

        public static void DrawGraph(Graph graph, int[][,] matrix)
        {
            if (!graph.IsOriented)
                for (var i = 0; i < graph.VertexCount; i++)
                for (var j = 0; j < graph.VertexCount; j++)
                    matrix[0][j, i] = matrix[0][i, j];

            var formatString = string.Format("|{{0,-{0}}}|", 4);
            for (var i = 0; i < graph.VertexCount; i++)
            {
                for (var j = 0; j < graph.VertexCount; j++)
                {
                    if (matrix[0][j, i] == 0 && matrix[1][j, i] == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(formatString, "inf");
                    }
                    else if (matrix[0][j, i] != 0 && matrix[1][j, i] == 0)
                    {
                        Console.Write(formatString, matrix[0][j, i] + "/" + 0);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(formatString, matrix[0][j, i] + "/" + matrix[1][j, i]);
                    }

                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.Write("   " + (i + 1));
                Console.WriteLine();
            }

            Console.WriteLine("_________________________________________________________________");
            Console.WriteLine();
            // Console.Write(" ");
            for (var i = 0; i < graph.VertexCount; i++) Console.Write(formatString, i + 1);
            Console.WriteLine();
        }

        public void AlgPrima()
        {
            const string format = "{0}-{1, -8} Weight: {2}";
            var totalWeight = 0;
            var inClaster = false;
            var cluster = new List<Vertex>();
            var min_f = new Vertex(999);
            var min_t = new Vertex(999);

            var Min = new Edge(min_f, min_t, 999);

            foreach (var edge in _edges)
                if (!edge.Used && edge.Weight < Min.Weight)
                    Min = edge;

            Min.Used = true;
            Min.From.Connected = true;
            Min.To.Connected = true;
            cluster.Add(Min.From);
            cluster.Add(Min.To);

            Console.WriteLine(format, Min.From.Number + 1, Min.To.Number + 1, Min.Weight);
            totalWeight += Min.Weight;


            for (var l = 0; l < VertexCount - 2; l++)
            {
                Min = new Edge(min_f, min_t, 999);
                var possibleEdges = new List<Edge>();

                for (var i = 0; i < EdgesCount; i++)
                for (var j = 0; j < cluster.Count; j++)
                    if (_edges[i].From.Number ==
                        cluster[j].Number) //|| (Edges[i].From != cluster[j] && Edges[i].To == cluster[j]))
                    {
                        inClaster = false;
                        for (var k = 0; k < cluster.Count; k++)
                            if (_edges[i].To == cluster[k])
                                inClaster = true;
                        if (inClaster == false) possibleEdges.Add(_edges[i]);
                    }

                    //Var 2
                    else if (
                        _edges[i].To.Number ==
                        cluster[j]
                            .Number) //|| (Edges[i].From.Number != cluster[j].Number && Edges[i].To.Number == cluster[j].Number))
                    {
                        inClaster = false;
                        for (var k = 0; k < cluster.Count; k++)
                            if (_edges[i].From == cluster[k])
                                inClaster = true;
                        if (inClaster == false) possibleEdges.Add(_edges[i]);
                    }

                foreach (var edge in possibleEdges)
                    if (!edge.Used && edge.Weight < Min.Weight)
                        Min = edge;

                Min.Used = true;
                Min.To.Connected = true;
                cluster.Add(Min.To);
                cluster.Add(Min.From);

                Console.WriteLine(format, Min.From.Number + 1, Min.To.Number + 1, Min.Weight);

                totalWeight += Min.Weight;
            }

            Console.WriteLine();
            Console.WriteLine("Total weight: " + totalWeight);
        }


        public int[][,] FordFalkerson(int[][,] matrix, ref int delta)
        {
            var start = 0;
            var end = _vertexes.Count - 1;
            int cur;
            // var matrix = GetMetrix();
            var weights = new int[4];
            var vertexes = new int[5];

            var l = 0;
            var k = 0;
            cur = start;

            for (var i = 0; i < _vertexes.Count; i++)
            {
                vertexes[0] = start;
                for (; l < _vertexes.Count; l++)
                    if (matrix[0][cur, l] > 0)
                    {
                        Console.WriteLine(cur + 1 + "  " + (l + 1));
                        //  Console.WriteLine((i + 1) + "  " + matrix[cur, i]);
                        weights[k] = matrix[0][cur, l];
                        cur = l;
                        vertexes[k + 1] = cur;
                        k++;

                        if (cur == end) //круг
                        {
                            var min = 999;

                            for (var j = 0; j < weights.Length; j++)
                                if (weights[j] < min && weights[j] != 0)
                                    min = weights[j];

                            for (var j = 1; j < vertexes.Length; j++)
                            {
                                matrix[0][vertexes[j - 1], vertexes[j]] -= min;
                                matrix[1][vertexes[j - 1], vertexes[j]] += min;
                                //  matrix[1][vertexes[j], vertexes[j - 1]] += min;
                            }

                            delta += min;
                            cur = start;
                            k = 0;
                            Console.WriteLine(delta);
                            // Console.WriteLine(min);

                            for (var j = 0; j < vertexes.Length; j++) vertexes[j] = 0;

                            for (var j = 0; j < weights.Length; j++) weights[j] = 0;
                            DrawGraph(this, matrix);
                        }

                        l = 0;
                    }

                Console.WriteLine("_____");
                cur = vertexes[k - 1];
                l = vertexes[k] + 1;
                weights[k] = 0;
                vertexes[k] = 0;
                k--;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Failed");
            Console.ForegroundColor = ConsoleColor.White;
            return matrix;
        }

        public void FordFalkersonBack(int[][,] matrix, ref int delta)
        {
            var start = 0;
            var end = _vertexes.Count - 1;
            int cur;
            // var matrix = GetMetrix();
            var weights = new int[9];
            var vertexes = new int[10];
            var l = 0;
            var k = 0;
            cur = start;

            for (var i = 0; i < _vertexes.Count; i++)
            {
                link1:
                vertexes[0] = start;

                for (; l < _vertexes.Count; l++)
                    if (matrix[0][cur, l] > 0 && _vertexes[l].Visited == false)
                    {
                        Console.WriteLine(cur + 1 + "  " + (l + 1));
                        weights[k] = matrix[0][cur, l];
                        cur = l;
                        vertexes[k + 1] = cur;
                        _vertexes[cur].Visited = true;
                        k++;

                        if (cur == end) //круг
                        {
                            var min = 999;

                            for (var j = 0; j < weights.Length; j++)
                                if (weights[j] < min && weights[j] != 0)
                                    min = weights[j];

                            for (var j = 1; j < vertexes.Length; j++)
                                if (matrix[0][vertexes[j - 1], vertexes[j]] <= 0 &&
                                    matrix[1][vertexes[j], vertexes[j - 1]] >= min)
                                {
                                    matrix[0][vertexes[j], vertexes[j - 1]] += min;
                                    matrix[1][vertexes[j], vertexes[j - 1]] -= min;
                                }
                                else if (matrix[0][vertexes[j - 1], vertexes[j]] > 0)
                                {
                                    matrix[0][vertexes[j - 1], vertexes[j]] -= min;
                                    matrix[1][vertexes[j - 1], vertexes[j]] += min;
                                }

                            delta += min;
                            cur = start;
                            k = 0;
                            Console.WriteLine(delta);


                            for (var j = 0; j < vertexes.Length; j++) vertexes[j] = 0;

                            for (var j = 0; j < weights.Length; j++) weights[j] = 0;
                            for (var n = 0; n < _vertexes.Count; n++) _vertexes[n].Visited = false;
                            DrawGraph(this, matrix);
                        }

                        l = 0;
                    }

                l = 0;
                _vertexes[start].Visited = false;

                for (; l < cur; l++)
                    if (matrix[1][l, cur] > 0 && _vertexes[l].Visited == false && l != end && cur != start)
                    {
                        Console.WriteLine(cur + 1 + "  " + (l + 1));
                        weights[k] = matrix[1][l, cur];
                        cur = l;
                        vertexes[k + 1] = cur;
                        _vertexes[cur].Visited = true;
                        k++;
                        if (cur == end) //круг
                        {
                            var min = 999;
                            for (var j = 0; j < weights.Length; j++)
                                if (weights[j] < min && weights[j] != 0)
                                    min = weights[j];

                            for (var j = 1; j < vertexes.Length; j++)
                                if (matrix[0][vertexes[j - 1], vertexes[j]] <= 0 &&
                                    matrix[1][vertexes[j], vertexes[j - 1]] >= min)
                                {
                                    matrix[0][vertexes[j], vertexes[j - 1]] += min;
                                    matrix[1][vertexes[j], vertexes[j - 1]] -= min;
                                }
                                else if (matrix[0][vertexes[j - 1], vertexes[j]] > 0)
                                {
                                    matrix[0][vertexes[j - 1], vertexes[j]] -= min;
                                    matrix[1][vertexes[j - 1], vertexes[j]] += min;
                                }

                            delta += min;
                            cur = start;
                            k = 0;
                            Console.WriteLine(delta);
                            // Console.WriteLine(min);

                            for (var j = 0; j < vertexes.Length; j++) vertexes[j] = 0;

                            for (var j = 0; j < weights.Length; j++) weights[j] = 0;
                            for (var n = 0; n < _vertexes.Count; n++) _vertexes[n].Visited = false;


                            DrawGraph(this, matrix);
                        }

                        l = 0;
                        goto link1;
                    }


                Console.WriteLine("_____");
                if (k > 0) cur = vertexes[k - 1];
                _vertexes[start].Visited = false;
                l = vertexes[k] + 1;
                weights[k] = 0;
                vertexes[k] = 0;
                if (k > 0) k--;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Failed");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("");
            Console.WriteLine("V = " + delta);
        }
    }
}