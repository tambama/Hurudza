using NetTopologySuite.Geometries;

namespace Hurudza.Common.Maps
{
    public static class MapHelper
    {
        public static Polygon GetPolygon(List<Coordinate> coordinates)
        {
            NetTopologySuite.NtsGeometryServices.Instance = new NetTopologySuite.NtsGeometryServices(
                // default CoordinateSequenceFactory
                NetTopologySuite.Geometries.Implementation.CoordinateArraySequenceFactory.Instance,
                // default precision model
                new PrecisionModel(1000d),
                // default SRID
                4326,
                /********************************************************************
                 ** Note: the following arguments are only valid for NTS >= v2.2
                ********************************************************************/
                // Geometry overlay operation function set to use (Legacy or NG)
                NetTopologySuite.Geometries.GeometryOverlay.NG,
                // Coordinate equality comparer to use (CoordinateEqualityComparer or PerOrdinateEqualityComparer)
                new CoordinateEqualityComparer());

            // Create a geometry factory with the spatial reference id 4326
            var gf = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(4326);
            // Create a polygon
            var polygon = gf.CreatePolygon(coordinates.ToArray());

            return polygon;
        }

        public static LineString GetLineRing(List<Coordinate> coordinates)
        {
            NetTopologySuite.NtsGeometryServices.Instance = new NetTopologySuite.NtsGeometryServices(
                // default CoordinateSequenceFactory
                NetTopologySuite.Geometries.Implementation.CoordinateArraySequenceFactory.Instance,
                // default precision model
                new PrecisionModel(1000d),
                // default SRID
                4326,
                /********************************************************************
                 ** Note: the following arguments are only valid for NTS >= v2.2
                ********************************************************************/
                // Geometry overlay operation function set to use (Legacy or NG)
                NetTopologySuite.Geometries.GeometryOverlay.NG,
                // Coordinate equality comparer to use (CoordinateEqualityComparer or PerOrdinateEqualityComparer)
                new CoordinateEqualityComparer());

            // Create a geometry factory with the spatial reference id 4326
            var gf = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(4326);
            // Create a polygon
            var line = gf.CreateLineString(coordinates.ToArray());

            return line;
        }
    }
}
