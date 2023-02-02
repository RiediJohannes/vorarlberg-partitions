using Svg;
using System.Drawing;
using VorarlbergPartitions.Models;

namespace VorarlbergPartitions.Adapters
{
    class SvgMap
    {
        private readonly SvgDocument _svg;

        public SvgMap(string svgPath)
        {
            svgPath = "../../../" + svgPath;
            _svg = SvgDocument.Open(svgPath);
        }

        public void ChangeColour(Municipality municipality, Color colour)
        {
            // find corresponding SVG element to given municipality
            SvgElement svgMunipal = _svg.GetElementById(municipality.Id);
            SvgColourServer newColour = new SvgColourServer(colour);
      
            svgMunipal.Fill = newColour;
            svgMunipal.FillOpacity = 1;
        }

        public string GetColour(Municipality municipality)
        {
            SvgElement svgMunicipality = _svg.GetElementById(municipality.Id);
            SvgColourServer mpfill = (SvgColourServer) svgMunicipality.Fill;

            string colour = mpfill.Colour.Name;
            return colour[2..];
        }

        public void SaveChanges(string path)
        {
            _svg.Write("../../../" + path);
        }
    }
}
