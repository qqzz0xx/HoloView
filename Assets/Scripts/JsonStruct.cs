
using System.Collections.Generic;
namespace nn
{
    public struct NodeJS
    {
        public bool active;
        public List<float> color;
        public List<float> matrix;
        public string name;
        public float opacity;
        public string uuid;
        public string fileName;
    }

    public struct ProjectJS
    {
        public string name;
        public List<NodeJS> nodes;
    }
}