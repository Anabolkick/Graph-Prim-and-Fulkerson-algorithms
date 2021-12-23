using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


namespace Graph
{
    public static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;
            box.SelectionColor = color;
            //      text = string.Format("|{{0,-{0}}}|", 4);
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }
    }
    public class Graph
    {
        public readonly List<Edge> Edges = new List<Edge>();

        public readonly List<Vertex> Vertexes = new List<Vertex>();

        public Graph(bool oriented)
        {
            IsOriented = oriented;
        }

        public bool IsOriented { get; set; }
        public int VertexCount => Vertexes.Count;
        public int EdgesCount => Edges.Count;

        public void AddVertex(Vertex vertex)
        {
            Vertexes.Add(vertex);
        }

        public void AddEdge(Vertex from, Vertex to)
        {
            var edge = new Edge(from, to);
            Edges.Add(edge);
            if (!IsOriented)
            {
                var edge_second = new Edge(to, from);
                Edges.Add(edge_second);
            }
        }

        public void AddEdge(Vertex from, Vertex to, int wieght)
        {
            var edge = new Edge(from, to, wieght);
            Edges.Add(edge);
            if (!IsOriented)
            {
                var edge_second = new Edge(to, from, wieght);
                Edges.Add(edge_second);
            }
        }

        public int[,] GetSimpleMetrix() //matrix smezhnosti
        {
            int[,] matrix = new int[VertexCount, VertexCount];
            foreach (var edge in Edges)
            {
                var row = edge.From.Number-1;
                var column = edge.To.Number-1;
                matrix[row, column] = edge.Weight;
            }
            return matrix;
        }

        public int[,] GetEndlessMaxMetrix() //matrix smezhnosti
        {
            int[,] matrix = new int[VertexCount, VertexCount];

            for (int i = 0; i < VertexCount; i++)
            {
                for (int j = 0; j < VertexCount; j++)
                {
                    matrix[i, j] = 99999999;
                }
            }

            foreach (var edge in Edges)
            {
                var row = edge.From.Number - 1;
                var column = edge.To.Number - 1;
                matrix[row, column] = edge.Weight;
            }
            return matrix;
        }
        public int[][,] GetMetrix() //matrix smezhnosti
        {
            var matrix = new int[2][,];
            matrix[0] = new int[VertexCount, VertexCount];
            matrix[1] = new int[VertexCount, VertexCount];

            foreach (var edge in Edges)
            {
                var row = edge.From.Number - 1;
                var column = edge.To.Number - 1;

                matrix[0][row, column] = edge.Weight;
                matrix[1][row, column] = 0;
            }

            return matrix;
        }

        public static int[,] FloydWarshall(Graph graph)
        {
            var matrix = graph.GetEndlessMaxMetrix();

            for (int k = 0; k < graph.VertexCount; k++)
            {
                for (int i = 0; i < graph.VertexCount; i++)
                {
                    for (int j = 0; j < graph.VertexCount; j++)
                    {
                        matrix[i,j] = Math.Min(matrix[i,j], matrix[i,k] + matrix[k,j]);
                    }
                }
            }

            return matrix;
        }
        public static (List<Vertex>[],int?[]) Deikstra(Graph graph, Vertex startVertex, Vertex finishVertex)
        {
            List<Vertex>[] path = new List<Vertex>[graph.VertexCount];
            for (int i = 0; i < graph.VertexCount; i++)
            {
                path[i] = new List<Vertex>();
            }
            int?[] shorterDistance = new int?[graph.VertexCount]; //d
            bool[] used = new bool[graph.VertexCount];

            for (int i = 0; i < graph.VertexCount; i++)
            {
                shorterDistance[i] = int.MaxValue;
                used[i] = false;

                if (graph.Vertexes[i] == startVertex)
                {
                    shorterDistance[i] = 0;
                }
            }

            for (int i = 0; i < graph.VertexCount; i++)
            {
                int? v = null;

                for (int j = 0; j < graph.VertexCount; j++)
                {
                    if (!used[j] && (v == null || shorterDistance[j] < shorterDistance[(int)v]))
                    {
                        v = j;
                    }
                }
                if (shorterDistance[(int)v] == int.MaxValue)
                {
                    break;
                }

                used[(int)v] = true;
                for (int e = 0; e < graph.EdgesCount; e++)
                {
                    if (graph.Edges[e].From == graph.Vertexes[(int)v])
                    {
                        for (int j = 0; j < graph.VertexCount; j++)
                        {
                            if (graph.Vertexes[j] == graph.Edges[e].To)
                            {
                                if (shorterDistance[(int)v] + graph.Edges[e].Weight < shorterDistance[j])
                                {
                                    shorterDistance[j] = shorterDistance[(int) v] + graph.Edges[e].Weight;
                                    path[j].Clear();
                                    path[j].AddRange(path[(int)v]);
                                    path[j].Add(graph.Vertexes[j]);
                                }
                            }

                        }

                    }
                }
            }

            return (path,shorterDistance);
        }
        public static void DepthSearch(Graph graph, Vertex current)
        {
            current.Color = Vertex.VertexColor.Grey;
            foreach (var edge in graph.Edges)
            {
                if (edge.From == current)
                {
                    if (edge.To.Color == Vertex.VertexColor.White)
                    {
                        if (edge.EdgeType == null)
                        {
                            edge.EdgeType = "ребро дерева";
                        }
                        DepthSearch(graph, edge.To);
                    }
                    else if (edge.EdgeType == null && edge.To.Color == Vertex.VertexColor.Grey)
                    {
                        edge.EdgeType = "зворотнє ребро";
                    }
                    else if (edge.EdgeType == null && edge.To.Color == Vertex.VertexColor.Black)
                    {
                        edge.EdgeType = "пряме/перехресне ребро";
                    }
                }
            }
            current.Color = Vertex.VertexColor.Black;
        }
        public static void BreadthSearch(Graph graph, Vertex first)
        {
            Queue<Vertex> VertQueue = new Queue<Vertex>();

            foreach (var edge in graph.Edges)
            {
                if (edge.From == first)
                {
                    VertQueue.Enqueue(edge.To);
                }
            }

            first.Visited = true;
            Vertex current;

            while (VertQueue.Count > 0)
            {
                current = VertQueue.Dequeue();

                foreach (var edge in graph.Edges)
                {
                    if (edge.From == current)
                    {
                        if (!edge.To.Visited)
                        {
                            VertQueue.Enqueue(edge.To);
                            edge.To.Visited = true;
                        }
                    }
                }
            }

        }
        public static List<Vertex>[] BreadthSearchFindPath(Graph graph, Vertex first, Vertex find, RichTextBox textBox)
        {
            Queue<Vertex> VertQueue = new Queue<Vertex>();

            int i = 0;
            first.Visited = true;

            List<Vertex>[] path = new List<Vertex>[graph.VertexCount + 1];
            for (int j = 0; j < graph.VertexCount + 1; j++)
            {
                path[j] = new List<Vertex>();
            }

            int[] pathLength = new int[graph.VertexCount + 1];


            VertQueue.Enqueue(first);
            path[1].Add(first);
            Vertex current;
            do
            {
                current = VertQueue.Dequeue();
                foreach (var edge in graph.Edges)
                {
                    if (edge.From == current)
                    {
                        if (!edge.To.Visited)
                        {
                            path[edge.To.Number].AddRange(path[edge.From.Number]);
                            path[edge.To.Number].Add(edge.To);
                            VertQueue.Enqueue(edge.To);
                            edge.To.Visited = true;

                            if (edge.To == find)
                            {
                                return path;
                            }
                        }
                    }
                }
            } while (VertQueue.Count > 0);

            return path;
        }
        public static void BreadthSearchWriteVertexes(Graph graph, Vertex first, RichTextBox textBox)
        {
            Queue<Vertex> VertQueue = new Queue<Vertex>();

            int i = 0;
            first.Visited = true;
            textBox.Text += $"{++i}) {first.Name}";
            textBox.Text += Environment.NewLine;

            VertQueue.Enqueue(first);
            Vertex current;
            do
            {
                current = VertQueue.Dequeue();
                foreach (var edge in graph.Edges)
                {
                    if (edge.From == current)
                    {
                        if (!edge.To.Visited)
                        {
                            VertQueue.Enqueue(edge.To);
                            edge.To.Visited = true;
                            textBox.Text += $"{++i}) {edge.To.Name}";
                            textBox.Text += Environment.NewLine;
                        }
                    }
                }
            } while (VertQueue.Count > 0);

        }

        public static void SimpleDrawGraphWinForms(Graph graph, int[,] matrix, DataGridView dataGrid)
        {
            dataGrid.ColumnCount = 0;
            dataGrid.RowCount = 1;
            dataGrid.Columns[0].Name = 1.ToString();
            for (int i = 0; i < graph.VertexCount; i++)
            {
                dataGrid.Columns.Add((i + 2).ToString(), (i + 2).ToString());
                dataGrid.Rows.Add();
                dataGrid.Rows[i].HeaderCell.Value = (i + 1).ToString();
            }

            for (var i = 0; i < graph.VertexCount; i++)
            {
                if (graph.IsOriented)
                {
                    for (var j = 0; j < graph.VertexCount; j++)
                    {
                        if (i == j)
                        {
                            dataGrid.Rows[j].Cells[i].Value = "0";
                        }
                        else if (matrix[j, i] == 0 && matrix[i, j] == 0)
                        {
                            dataGrid.Rows[j].Cells[i].Value = "inf";
                        }
                        else if (matrix[j, i] != 0 || matrix[i, j] != 0)
                        {
                            dataGrid.Rows[j].Cells[i].Value = $"{matrix[j, i]} / {matrix[i, j]}";
                        }
                   
                    }
                }
                else
                {
                    for (var j = 0; j < graph.VertexCount; j++)
                    {
                        if (i == j)
                        {
                            dataGrid.Rows[j].Cells[i].Value = "0";
                        }
                        else if (matrix[j, i] == 0)
                        {
                            dataGrid.Rows[j].Cells[i].Value = "inf";
                        }
                        else if (matrix[j, i] != 0)
                        {
                            dataGrid.Rows[j].Cells[i].Value = $"{matrix[j, i]}";
                        }
                    }
                }

            }
        }
        public static void DrawGraphWinForms(Graph graph, int[][,] matrix, DataGridView dataGrid)
        {
            dataGrid.ColumnCount = 0;
            dataGrid.RowCount = 1;
            dataGrid.Columns[0].Name = 1.ToString();
            for (int i = 0; i < graph.VertexCount; i++)
            {
                dataGrid.Columns.Add((i + 2).ToString(), (i + 2).ToString());
                dataGrid.Rows.Add();
                dataGrid.Rows[i].HeaderCell.Value = (i + 1).ToString();
            }

            for (var i = 0; i < graph.VertexCount; i++)
            {
                if (graph.IsOriented)
                {
                    for (var j = 0; j < graph.VertexCount; j++)
                    {
                        if (matrix[0][j, i] == 0 && matrix[1][j, i] == 0)
                        {
                            dataGrid.Rows[j].Cells[i].Value = "inf";
                        }
                        else if (matrix[0][j, i] != 0 && matrix[1][j, i] == 0)
                        {
                            dataGrid.Rows[j].Cells[i].Value = $"{matrix[0][j, i]} / 0";
                        }
                        else
                        {
                            dataGrid.Rows[j].Cells[i].Value = $"{matrix[0][j, i]} / {matrix[1][j, i]}";
                        }
                    }
                }
                else
                {
                    for (var j = 0; j < graph.VertexCount; j++)
                    {
                        if (matrix[0][j, i] == 0)
                        {
                            dataGrid.Rows[j].Cells[i].Value = "inf";
                        }
                        else if (matrix[0][j, i] != 0)
                        {
                            dataGrid.Rows[j].Cells[i].Value = $"{matrix[0][j, i]}";
                        }
                    }
                }

            }
        }
        public static string AlgPrima(Graph graph)
        {
            string steps = "";
            var totalWeight = 0;
            var inClaster = false;
            var cluster = new List<Vertex>();
            var min_f = new Vertex(999);
            var min_t = new Vertex(999);

            var Min = new Edge(min_f, min_t, 999);

            foreach (var edge in graph.Edges)
            {
                if (!edge.Used && edge.Weight < Min.Weight)
                {
                    Min = edge;
                }
            }
            
            Min.Used = true;
            Min.From.Connected = true;
            Min.To.Connected = true;
            cluster.Add(Min.From);
            cluster.Add(Min.To);

            steps += $"З {Min.From.Number} у {Min.To.Number} вага: {Min.Weight}";
            steps += Environment.NewLine;
           totalWeight += Min.Weight;


            for (var l = 0; l < graph.VertexCount - 2; l++)
            {
                Min = new Edge(min_f, min_t, 999);
                var possibleEdges = new List<Edge>();
                for (var i = 0; i < graph.EdgesCount; i++)
                    for (var j = 0; j < cluster.Count; j++)

                        if (graph.Edges[i].From.Number == cluster[j].Number) 
                        {
                            inClaster = false;
                            for (var k = 0; k < cluster.Count; k++)
                            {
                                if (graph.Edges[i].To == cluster[k])
                                {
                                    inClaster = true;
                                }

                                if (inClaster == false)
                                {
                                    possibleEdges.Add(graph.Edges[i]);
                                }
                            }
                        }
                        //Var 2
                        else if (
                            graph.Edges[i].To.Number == cluster[j].Number) 
                        {
                            inClaster = false;
                            for (var k = 0; k < cluster.Count; k++)
                                if (graph.Edges[i].From == cluster[k])
                                    inClaster = true;
                            if (inClaster == false) possibleEdges.Add(graph.Edges[i]);
                        }

                foreach (var edge in possibleEdges)
                    if (!edge.Used && edge.Weight < Min.Weight)
                        Min = edge;

                Min.Used = true;
                Min.To.Connected = true;
                cluster.Add(Min.To);
                cluster.Add(Min.From);

                steps += $"З {Min.From.Number} у {Min.To.Number} вага: {Min.Weight}";
                steps += Environment.NewLine;
                totalWeight += Min.Weight;
            }

            steps += ("Загальна вага: " + totalWeight);
            return steps;
        }

        #region CDM part




        //public static void DrawGraphConsole(Graph graph, int[][,] matrix)
        //{

        //    var formatString = string.Format("|{{0,-{0}}}|", 4);
        //    for (var i = 0; i < graph.VertexCount; i++)
        //    {
        //        for (var j = 0; j < graph.VertexCount; j++)
        //        {
        //            if (matrix[0][j, i] == 0 && matrix[1][j, i] == 0)
        //            {
        //                Console.ForegroundColor = ConsoleColor.Red;
        //                Console.Write(formatString, "inf");
        //            }
        //            else if (matrix[0][j, i] != 0 && matrix[1][j, i] == 0)
        //            {
        //                Console.Write(formatString, matrix[0][j, i] + "/" + 0);
        //            }
        //            else
        //            {
        //                Console.ForegroundColor = ConsoleColor.Green;
        //                Console.Write(formatString, matrix[0][j, i] + "/" + matrix[1][j, i]);
        //            }

        //            Console.ForegroundColor = ConsoleColor.White;
        //        }

        //        Console.Write("   " + (i + 1));
        //        Console.WriteLine();
        //    }

        //    Console.WriteLine("_________________________________________________________________");
        //    Console.WriteLine();
        //    // Console.Write(" ");
        //    for (var i = 0; i < graph.VertexCount; i++) Console.Write(formatString, i + 1);
        //    Console.WriteLine();
        //}

        //public static void DrawGraphWinForms(Graph graph, int[][,] matrix, RichTextBox textBox)
        //{

        //    //  var formatString = string.Format("|{{0,-{0}}}|", 4);
        //    for (var i = 0; i < graph.VertexCount; i++)
        //    {
        //        for (var j = 0; j < graph.VertexCount; j++)
        //        {
        //            if (matrix[0][j, i] == 0 && matrix[1][j, i] == 0)
        //            {
        //                textBox.AppendText("|   inf   | ", Color.Red);
        //            }
        //            else if (matrix[0][j, i] != 0 && matrix[1][j, i] == 0)
        //            {
        //                textBox.AppendText($"|{matrix[0][j, i]} / 0| ", Color.Black);
        //            }
        //            else
        //            {
        //                textBox.AppendText($"|{matrix[0][j, i]} / {matrix[1][j, i]}| ", Color.Green);
        //            }


        //        }
        //        textBox.AppendText($"     {i + 1}", Color.Black);
        //        textBox.AppendText(Environment.NewLine);
        //    }
        //    textBox.AppendText("_________________________________________________________________");
        //    textBox.AppendText(Environment.NewLine);
        //    // Console.Write(" ");
        //    for (var i = 0; i < graph.VertexCount; i++) textBox.AppendText($" {i + 1}       ", Color.Black); ;
        //    textBox.AppendText(Environment.NewLine);
        //}


        //public void AlgPrima()
        //{
        //    const string format = "{0}-{1, -8} Weight: {2}";
        //    var totalWeight = 0;
        //    var inClaster = false;
        //    var cluster = new List<Vertex>();
        //    var min_f = new Vertex(999);
        //    var min_t = new Vertex(999);

        //    var Min = new Edge(min_f, min_t, 999);

        //    foreach (var edge in Edges)
        //        if (!edge.Used && edge.Weight < Min.Weight)
        //            Min = edge;

        //    Min.Used = true;
        //    Min.From.Connected = true;
        //    Min.To.Connected = true;
        //    cluster.Add(Min.From);
        //    cluster.Add(Min.To);

        //    Console.WriteLine(format, Min.From.Number + 1, Min.To.Number + 1, Min.Weight);
        //    totalWeight += Min.Weight;


        //    for (var l = 0; l < VertexCount - 2; l++)
        //    {
        //        Min = new Edge(min_f, min_t, 999);
        //        var possibleEdges = new List<Edge>();

        //        for (var i = 0; i < EdgesCount; i++)
        //            for (var j = 0; j < cluster.Count; j++)
        //                if (Edges[i].From.Number ==
        //                    cluster[j].Number) //|| (Edges[i].From != cluster[j] && Edges[i].To == cluster[j]))
        //                {
        //                    inClaster = false;
        //                    for (var k = 0; k < cluster.Count; k++)
        //                        if (Edges[i].To == cluster[k])
        //                            inClaster = true;
        //                    if (inClaster == false) possibleEdges.Add(Edges[i]);
        //                }

        //                //Var 2
        //                else if (
        //                    Edges[i].To.Number ==
        //                    cluster[j]
        //                        .Number) //|| (Edges[i].From.Number != cluster[j].Number && Edges[i].To.Number == cluster[j].Number))
        //                {
        //                    inClaster = false;
        //                    for (var k = 0; k < cluster.Count; k++)
        //                        if (Edges[i].From == cluster[k])
        //                            inClaster = true;
        //                    if (inClaster == false) possibleEdges.Add(Edges[i]);
        //                }

        //        foreach (var edge in possibleEdges)
        //            if (!edge.Used && edge.Weight < Min.Weight)
        //                Min = edge;

        //        Min.Used = true;
        //        Min.To.Connected = true;
        //        cluster.Add(Min.To);
        //        cluster.Add(Min.From);

        //        Console.WriteLine(format, Min.From.Number + 1, Min.To.Number + 1, Min.Weight);

        //        totalWeight += Min.Weight;
        //    }

        //    Console.WriteLine();
        //    Console.WriteLine("Total weight: " + totalWeight);
        //}

        //public int[][,] FordFalkerson(int[][,] matrix, ref int delta)
        //{
        //    var start = 0;
        //    var end = Vertexes.Count - 1;
        //    int cur;
        //    // var matrix = GetMetrix();
        //    var weights = new int[4];
        //    var vertexes = new int[5];

        //    var l = 0;
        //    var k = 0;
        //    cur = start;

        //    for (var i = 0; i < Vertexes.Count; i++)
        //    {
        //        vertexes[0] = start;
        //        for (; l < Vertexes.Count; l++)
        //            if (matrix[0][cur, l] > 0)
        //            {
        //                Console.WriteLine(cur + 1 + "  " + (l + 1));
        //                //  Console.WriteLine((i + 1) + "  " + matrix[cur, i]);
        //                weights[k] = matrix[0][cur, l];
        //                cur = l;
        //                vertexes[k + 1] = cur;
        //                k++;

        //                if (cur == end) //круг
        //                {
        //                    var min = 999;

        //                    for (var j = 0; j < weights.Length; j++)
        //                        if (weights[j] < min && weights[j] != 0)
        //                            min = weights[j];

        //                    for (var j = 1; j < vertexes.Length; j++)
        //                    {
        //                        matrix[0][vertexes[j - 1], vertexes[j]] -= min;
        //                        matrix[1][vertexes[j - 1], vertexes[j]] += min;
        //                        //  matrix[1][vertexes[j], vertexes[j - 1]] += min;
        //                    }

        //                    delta += min;
        //                    cur = start;
        //                    k = 0;
        //                    Console.WriteLine(delta);
        //                    // Console.WriteLine(min);

        //                    for (var j = 0; j < vertexes.Length; j++) vertexes[j] = 0;

        //                    for (var j = 0; j < weights.Length; j++) weights[j] = 0;
        //                    DrawGraphConsole(this, matrix);
        //                }

        //                l = 0;
        //            }

        //        Console.WriteLine("_____");
        //        cur = vertexes[k - 1];
        //        l = vertexes[k] + 1;
        //        weights[k] = 0;
        //        vertexes[k] = 0;
        //        k--;
        //    }

        //    Console.ForegroundColor = ConsoleColor.Red;
        //    Console.WriteLine("Failed");
        //    Console.ForegroundColor = ConsoleColor.White;
        //    return matrix;
        //}

        ////public int[][,] FordFalkerson(int[][,] matrix, ref int delta, RichTextBox textBox)
        ////{
        ////    var start = 0;
        ////    var end = Vertexes.Count - 1;
        ////    int cur;
        ////    // var matrix = GetMetrix();
        ////    var weights = new int[4];
        ////    var vertexes = new int[5];

        ////    var l = 0;
        ////    var k = 0;
        ////    cur = start;

        ////    for (var i = 0; i < Vertexes.Count; i++)
        ////    {
        ////        vertexes[0] = start;
        ////        for (; l < Vertexes.Count; l++)
        ////            if (matrix[0][cur, l] > 0)
        ////            {
        ////                //  Console.WriteLine(cur + 1 + "  " + (l + 1));
        ////                textBox.AppendText($"{cur + 1}   {l + 1}", Color.Black);
        ////                //  Console.WriteLine((i + 1) + "  " + matrix[cur, i]);
        ////                weights[k] = matrix[0][cur, l];
        ////                cur = l;
        ////                vertexes[k + 1] = cur;
        ////                k++;

        ////                if (cur == end) //круг
        ////                {
        ////                    var min = 999;

        ////                    for (var j = 0; j < weights.Length; j++)
        ////                        if (weights[j] < min && weights[j] != 0)
        ////                            min = weights[j];

        ////                    for (var j = 1; j < vertexes.Length; j++)
        ////                    {
        ////                        matrix[0][vertexes[j - 1], vertexes[j]] -= min;
        ////                        matrix[1][vertexes[j - 1], vertexes[j]] += min;
        ////                        //  matrix[1][vertexes[j], vertexes[j - 1]] += min;
        ////                    }

        ////                    delta += min;
        ////                    cur = start;
        ////                    k = 0;
        ////                    textBox.AppendText($"{delta}", Color.Black);
        ////                    // Console.WriteLine(delta);
        ////                    // Console.WriteLine(min);

        ////                    for (var j = 0; j < vertexes.Length; j++) vertexes[j] = 0;

        ////                    for (var j = 0; j < weights.Length; j++) weights[j] = 0;
        ////                   // DrawGraphWinForms(this, matrix, textBox);
        ////                }

        ////                l = 0;
        ////            }
        ////        textBox.AppendText("_____", Color.Black);
        ////        // Console.WriteLine("_____");
        ////        cur = vertexes[k - 1];
        ////        l = vertexes[k] + 1;
        ////        weights[k] = 0;
        ////        vertexes[k] = 0;
        ////        k--;
        ////    }

        ////    // Console.ForegroundColor = ConsoleColor.Red;
        ////    textBox.AppendText("Failed", Color.Red);
        ////    //Console.WriteLine("Failed");
        ////    //  Console.ForegroundColor = ConsoleColor.White;
        ////    return matrix;
        ////}
        //public void FordFalkersonBack(int[][,] matrix, ref int delta)
        //{
        //    var start = 0;
        //    var end = Vertexes.Count - 1;
        //    int cur;
        //    // var matrix = GetMetrix();
        //    var weights = new int[9];
        //    var vertexes = new int[10];
        //    var l = 0;
        //    var k = 0;
        //    cur = start;

        //    for (var i = 0; i < Vertexes.Count; i++)
        //    {
        //        link1:
        //        vertexes[0] = start;

        //        for (; l < Vertexes.Count; l++)
        //            if (matrix[0][cur, l] > 0 && Vertexes[l].Visited == false)
        //            {
        //                Console.WriteLine(cur + 1 + "  " + (l + 1));
        //                weights[k] = matrix[0][cur, l];
        //                cur = l;
        //                vertexes[k + 1] = cur;
        //                Vertexes[cur].Visited = true;
        //                k++;

        //                if (cur == end) //круг
        //                {
        //                    var min = 999;

        //                    for (var j = 0; j < weights.Length; j++)
        //                        if (weights[j] < min && weights[j] != 0)
        //                            min = weights[j];

        //                    for (var j = 1; j < vertexes.Length; j++)
        //                        if (matrix[0][vertexes[j - 1], vertexes[j]] <= 0 &&
        //                            matrix[1][vertexes[j], vertexes[j - 1]] >= min)
        //                        {
        //                            matrix[0][vertexes[j], vertexes[j - 1]] += min;
        //                            matrix[1][vertexes[j], vertexes[j - 1]] -= min;
        //                        }
        //                        else if (matrix[0][vertexes[j - 1], vertexes[j]] > 0)
        //                        {
        //                            matrix[0][vertexes[j - 1], vertexes[j]] -= min;
        //                            matrix[1][vertexes[j - 1], vertexes[j]] += min;
        //                        }

        //                    delta += min;
        //                    cur = start;
        //                    k = 0;
        //                    Console.WriteLine(delta);


        //                    for (var j = 0; j < vertexes.Length; j++) vertexes[j] = 0;

        //                    for (var j = 0; j < weights.Length; j++) weights[j] = 0;
        //                    for (var n = 0; n < Vertexes.Count; n++) Vertexes[n].Visited = false;
        //                    DrawGraphConsole(this, matrix);
        //                }

        //                l = 0;
        //            }

        //        l = 0;
        //        Vertexes[start].Visited = false;

        //        for (; l < cur; l++)
        //            if (matrix[1][l, cur] > 0 && Vertexes[l].Visited == false && l != end && cur != start)
        //            {
        //                Console.WriteLine(cur + 1 + "  " + (l + 1));
        //                weights[k] = matrix[1][l, cur];
        //                cur = l;
        //                vertexes[k + 1] = cur;
        //                Vertexes[cur].Visited = true;
        //                k++;
        //                if (cur == end) //круг
        //                {
        //                    var min = 999;
        //                    for (var j = 0; j < weights.Length; j++)
        //                        if (weights[j] < min && weights[j] != 0)
        //                            min = weights[j];

        //                    for (var j = 1; j < vertexes.Length; j++)
        //                        if (matrix[0][vertexes[j - 1], vertexes[j]] <= 0 &&
        //                            matrix[1][vertexes[j], vertexes[j - 1]] >= min)
        //                        {
        //                            matrix[0][vertexes[j], vertexes[j - 1]] += min;
        //                            matrix[1][vertexes[j], vertexes[j - 1]] -= min;
        //                        }
        //                        else if (matrix[0][vertexes[j - 1], vertexes[j]] > 0)
        //                        {
        //                            matrix[0][vertexes[j - 1], vertexes[j]] -= min;
        //                            matrix[1][vertexes[j - 1], vertexes[j]] += min;
        //                        }

        //                    delta += min;
        //                    cur = start;
        //                    k = 0;
        //                    Console.WriteLine(delta);
        //                    // Console.WriteLine(min);

        //                    for (var j = 0; j < vertexes.Length; j++) vertexes[j] = 0;

        //                    for (var j = 0; j < weights.Length; j++) weights[j] = 0;
        //                    for (var n = 0; n < Vertexes.Count; n++) Vertexes[n].Visited = false;


        //                    DrawGraphConsole(this, matrix);
        //                }

        //                l = 0;
        //                goto link1;
        //            }


        //        Console.WriteLine("_____");
        //        if (k > 0) cur = vertexes[k - 1];
        //        Vertexes[start].Visited = false;
        //        l = vertexes[k] + 1;
        //        weights[k] = 0;
        //        vertexes[k] = 0;
        //        if (k > 0) k--;
        //    }

        //    Console.ForegroundColor = ConsoleColor.Red;
        //    Console.WriteLine("Failed");
        //    Console.ForegroundColor = ConsoleColor.White;
        //    Console.WriteLine("");
        //    Console.WriteLine("V = " + delta);
        //}
        #endregion
    }
}