using System;

namespace Graph
{
    class Program
    {
        static void Main()
        {
            Graph graph = new Graph(true);

            var v1 = new Vertex(1);
            var v2 = new Vertex(2);
            var v3 = new Vertex(3);
            var v4 = new Vertex(4);
            var v5 = new Vertex(5);
            var v6 = new Vertex(6);
            var v7 = new Vertex(7);
            var v8 = new Vertex(8);
            var v9 = new Vertex(9);
            var v10 = new Vertex(10);
            var v11 = new Vertex(11);
            var v12 = new Vertex(12);
            var v13 = new Vertex(13);

            graph.AddVertex(v1);
            graph.AddVertex(v2);
            graph.AddVertex(v3);
            graph.AddVertex(v4);
            graph.AddVertex(v5);
            graph.AddVertex(v6);
            graph.AddVertex(v7);
            graph.AddVertex(v8);
            graph.AddVertex(v9);
            graph.AddVertex(v10);
            graph.AddVertex(v11);
            graph.AddVertex(v12);
            graph.AddVertex(v13);


            graph.AddEdge(v1, v2, 11);
            graph.AddEdge(v1, v3, 15);
            graph.AddEdge(v1, v4, 11);
            graph.AddEdge(v1, v5, 15);

            graph.AddEdge(v2, v6, 7);
            graph.AddEdge(v2, v7, 9);

            graph.AddEdge(v3, v6, 4);

            graph.AddEdge(v4, v7, 8);
            graph.AddEdge(v4, v8, 9);
            graph.AddEdge(v4, v9, 4);

            graph.AddEdge(v5, v8, 9);
            graph.AddEdge(v5, v9, 5);

            graph.AddEdge(v6, v10, 8);

            graph.AddEdge(v7, v10, 13);
            graph.AddEdge(v7, v11, 7);

            graph.AddEdge(v8, v11, 4);
            graph.AddEdge(v8, v12, 4);

            graph.AddEdge(v9, v12, 12);

            graph.AddEdge(v10, v13, 20);

            graph.AddEdge(v11, v13, 10);

            graph.AddEdge(v12, v13, 13);

          //  int[][,] matrix = graph.GetMetrix();
          //  Graph.DrawGraphConsole(graph, matrix);


          //  int delta = 0;
          //  matrix = graph.FordFalkerson(matrix, ref delta);
          ////  graph.FordFalkersonBack(matrix, ref delta);
          //  Console.ReadLine();
        }

    }
}

