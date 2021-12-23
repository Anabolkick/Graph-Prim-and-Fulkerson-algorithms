namespace Graph
{
    public class Vertex
    {
        public int Number { get; set; }
        public bool Connected { get; set; }
        public bool Visited { get; set; }
        public string Name { get; set; }
        public VertexColor Color { get; set; }

        public enum VertexColor
        {
            White,
            Grey, 
            Black,
        }
        public Vertex(int number = 1, bool connected = false, string name = "unnamed")
        {
            Number = number;
            Connected = connected;
            Color = VertexColor.White;
            Name = name;
        }

        public override string ToString()
        {
            return Number.ToString();
        }

    }
}