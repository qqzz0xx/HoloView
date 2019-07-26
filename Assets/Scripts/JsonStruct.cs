
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

    public struct MeasureBaseJS
    {
        public int mpr_type;
        public string name;
        public List<float> p1;
        public List<float> p2;
        public string uuid;
    }

    public struct MeasureJS
    {
        public MeasureBaseJS @base;
        public List<float> color;
        public string id;
        public string txt;
    }
}