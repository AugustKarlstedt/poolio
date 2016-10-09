using System;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace poolio.Controllers
{
    public static class GmapController
    {
        public static Tuple<double, double> GetCoordinates(string address)
        {
            var requestUri = $"http://maps.googleapis.com/maps/api/geocode/xml?address={Uri.EscapeDataString(address)}&sensor=false";

            var request = WebRequest.Create(requestUri);
            var response = request.GetResponse();
            var xdoc = XDocument.Load(response.GetResponseStream());

            var result = xdoc.Element("GeocodeResponse").Element("result");
            var locationElement = result.Element("geometry").Element("location");

            return new Tuple<double, double>((double)locationElement.Element("lat"), (double)locationElement.Element("lng"));
        }

    }
}