#region copyright
// Copyright (C) Daniel Fink 2019 - All Rights Reserved.
// Unauthorized copying of this file (via any medium) is strictly prohibited.
// Proprietary and confidential.
// Written by Daniel Fink <daniel@placeful.io>, 2018.
#endregion copyright
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using Rhino;
using Rhino.Geometry;
using Rhino.DocObjects;
using Rhino.Collections;
using Rhino.Geometry.Intersect;
//using GeoAPI.Geometries;

using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Implementation;


namespace Antelope
{
    /// <summary>
    /// Methods that convert Rhino Geometry to NetTopologySuite Geometry
    /// </summary>
    public static class GeoGeomConversions
    {
        internal static CoordinateZ ToCoordinateZ(this global::NetTopologySuite.Geometries.Point point)
        {
            return new CoordinateZ(point.X, point.Y, point.Z);
        }

        internal static CoordinateSequence ToCoordinateSequence(this IEnumerable<CoordinateZ> coordinates)
        {
            var coordinateList = coordinates.ToList();
            var sequence = new CoordinateArraySequence(coordinateList.Count, 3, 0);
            for (int i = 0; i < coordinateList.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    sequence.SetOrdinate(i, j, coordinateList[i][j]);
                }
            }
            return sequence;
        }

        /// <summary>
        /// Converts a Point3d to a Coordinate
        /// </summary>
        public static CoordinateZ ToCoordinateZ(this Point3d point3d)
        {
            return new CoordinateZ(point3d.X, point3d.Y, point3d.Z);
        }

        /// <summary>
        /// Converts a Point3d to a NTS Point
        /// </summary>
        public static NetTopologySuite.Geometries.Point ToLocation(this Point3d point3d, ProjectionSystem sourceProjection)
        {
            var location = new NetTopologySuite.Geometries.Point(point3d.ToCoordinateZ());
            location.SRID = sourceProjection.SRID;

            return location;
        }

        /// <summary>
        /// Converts a Coordinate to a Point3d
        /// </summary>
        public static Point3d ToPoint3d(this Coordinate coordinate)
        {
            double X = (double.IsNaN(coordinate.X)) ? X = 0 : X = coordinate.X;
            double Y = (double.IsNaN(coordinate.Y)) ? Y = 0 : Y = coordinate.Y;
            double Z = (double.IsNaN(coordinate.Z)) ? Z = 0 : Z = coordinate.Z;

            return new Point3d(X, Y, Z);
        }

        /// <summary>
        /// Converts a Rhino Polyline to a Coordinate Array
        /// </summary>
        public static CoordinateSequence ToCoordinates(this Polyline polyline)
        {
            var coords = polyline.Select(vertex => new CoordinateZ(vertex.X, vertex.Y, vertex.Z));
            return coords.ToCoordinateSequence();
        }

        /// <summary>
        /// Converts a Rhino Polyline to a Linestring
        /// </summary>
        public static LineString ToLinestring(this Polyline polyline, ProjectionSystem sourceProjection)
        {
            var coordinates = polyline.Select(point => point.ToCoordinateZ()).ToCoordinateSequence();
            var precisionModel = new PrecisionModel(PrecisionModels.Floating);
            var factory = new GeometryFactory(precisionModel, sourceProjection.SRID);

            var ntsLineString = new LineString(coordinates, factory);
            ntsLineString.SRID = sourceProjection.SRID;
            return ntsLineString;
        }

        /// <summary>
        /// Converts a Closed Rhino Polyline to a LinearRing
        /// </summary>
        public static LinearRing ToLinearRing(this Polyline polyline, ProjectionSystem sourceProjection)
        {
            if (!polyline.IsClosed) throw new ArgumentException("Error: polyline is not closed.", nameof(polyline));
            var coordinates = polyline.Select(point => point.ToCoordinateZ()).ToCoordinateSequence();
            var precisionModel = new PrecisionModel(PrecisionModels.Floating);
            var factory = new GeometryFactory(precisionModel, sourceProjection.SRID);

            var ntsLinearRing = new LinearRing(coordinates, factory);
            ntsLinearRing.SRID = sourceProjection.SRID;
            return ntsLinearRing;
        }

        /// <summary>
        /// Converts a Coordinate Array to a LineString
        /// </summary>
        public static LineString ToLinestring(this CoordinateSequence coordinates, ProjectionSystem sourceProjection)
        {
            var linestring = new LineString(coordinates.ToCoordinateArray());
            linestring.SRID = sourceProjection.SRID;
            return linestring;
        }

        /// <summary>
        /// Converts a Linestring to a Rhino Polyline
        /// </summary>
        public static Polyline ToPolyline(this LineString linestring)
        {
            return new Polyline(linestring.Coordinates.Select(coord => coord.ToPoint3d()));
        }

        /// <summary>
        /// Converts a Brep to a Polygon
        /// </summary>
        public static NetTopologySuite.Geometries.Polygon ToPolygon(this Brep brep, ProjectionSystem sourceProjection)
        {
            var boundaryRing = brep.ExtractBoundaryPolyline().ToLinearRing(sourceProjection);
            var holeRings = brep.ExtractHolePolylines().Select(hole => hole.ToLinearRing((sourceProjection))).ToArray();
            var polygon = new NetTopologySuite.Geometries.Polygon(boundaryRing, holeRings);
            polygon.SRID = sourceProjection.SRID;
            return polygon;
        }

        /// <summary>
        /// Converts a Polygon to a Brep
        /// </summary>
        public static Brep ToBrep(this NetTopologySuite.Geometries.Polygon polygon)
        {
            var tolerance = RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
            var curves = new List<Curve>();

            var boundaryPolyline = new Polyline(polygon.ExteriorRing.Coordinates
                .Select(coordinate => coordinate.ToPoint3d()));
            PolylineCurve[] boundaries;
            // if (boundaryPolyline.ToPolylineCurve().IsSelfIntersecting(tolerance).Any())
            // {
            //     var boundaryPolygon = new Placeful.Geometry.Polygon(boundaryPolyline);
            //     boundaries = boundaryPolygon
            //         .ClipperClean(tolerance)
            //         .Select(p => (PolylineCurve)p)
            //         .ToArray();
            // }
            //else
            //{
                boundaries = new PolylineCurve[] { boundaryPolyline.ToPolylineCurve() };
            //}
            curves.AddRange(boundaries);


            var holePolylines = polygon.InteriorRings.Select(hole => new Polyline(hole.Coordinates
                .Select(coordinate => coordinate.ToPoint3d()))).ToList();
            var holes = new List<PolylineCurve>();
            foreach (var holePolyline in holePolylines)
            {
                var hole = new List<PolylineCurve>();
                //if (holePolyline.ToPolylineCurve().IsSelfIntersecting(tolerance).Count > 1)
                //{
                //    var holePolygon = new Placeful.Geometry.Polygon(holePolyline);
                //    hole = holePolygon.ClipperClean(RhinoDoc.ActiveDoc.ModelAbsoluteTolerance * 10)
                //        .Select(p => (PolylineCurve)p)
                //        .ToList();
                //}
                //else
                //{
                    hole = new List<PolylineCurve> { holePolyline.ToPolylineCurve() };
                //}

                holes.AddRange(hole);
            }
            curves.AddRange(holes);

            var breps = Brep.CreatePlanarBreps(curves, tolerance).ToList();
            var largestBrep = breps.OrderByDescending(brep => brep.GetArea()).First();

            return largestBrep;//.Select(brep => brep.Clean(false, false)).ToArray();
        }

        /// <summary>
        /// Converts a LinearRing to a Brep
        /// </summary>
        public static Brep ToBrep(this LinearRing linearRing)
        {
            var tolerance = RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
            var curves = new List<Curve>();

            var boundaryPolyline = new Polyline(linearRing.Coordinates
                .Select(coordinate => coordinate.ToPoint3d()));
            PolylineCurve[] boundaries;
            // if (boundaryPolyline.ToPolylineCurve().IsSelfIntersecting(tolerance).Any())
            // {
            //     var boundaryPolygon = new Placeful.Geometry.Polygon(boundaryPolyline);
            //     boundaries = boundaryPolygon
            //         .ClipperClean(tolerance)
            //         .Select(p => (PolylineCurve)p)
            //         .ToArray();
            // }
            // else
            // {
                boundaries = new PolylineCurve[] { boundaryPolyline.ToPolylineCurve() };
            //}
            curves.AddRange(boundaries);

            var breps = Brep.CreatePlanarBreps(curves, tolerance).ToList();
            var largestBrep = breps.OrderByDescending(brep => brep.GetArea()).First();

            return largestBrep;//.Select(brep => brep.Clean(false, false)).ToArray();
        }

        /// <summary>
        /// Convert GeoAPI IGeometry to Geospatial Geometry (IGeosptialGeom)
        /// </summary>
        public static IGeospatialGeom ToGeospatial(this NetTopologySuite.Geometries.Geometry geometry, ProjectionSystem sourceProjection)
        {
            IGeospatialGeom geospatialGeom;
            var sourceBasis = sourceProjection.SourceBasis;

            if (geometry is null)
            {
                return null;
            }

            else if (geometry is NetTopologySuite.Geometries.Point point)
            {
                geospatialGeom = new GeoPoint(point, sourceBasis);
            }

            // else if (geometry is NetTopologySuite.Geometries.MultiPoint multiPoint)
            // {
            //     if (multiPoint.Geometries.Length > 1)
            //     {
            //         var geoPoints = multiPoint.Geometries.Select(p => new GeoPoint((NetTopologySuite.Geometries.Point)p, sourceBasis)).ToArray();
            //         geospatialGeom = new MultiGeoPoint(geoPoints);
            //     }
            //     else
            //     {
            //         geospatialGeom = new GeoPoint((NetTopologySuite.Geometries.Point)multiPoint.Geometries.First(), sourceBasis);
            //     }
            // }

            else if (geometry is NetTopologySuite.Geometries.LineString lineString)
            {
                geospatialGeom = new GeoLineString(lineString, sourceBasis);
            }
            // else if (geometry is NetTopologySuite.Geometries.MultiLineString multiLineString)
            // {
            //     if (multiLineString.Geometries.Length > 1)
            //     {
            //         var geoLineStrings = multiLineString.Geometries.Select(l => new GeoLineString((LineString)l, sourceBasis));
            //         geospatialGeom = new MultiGeoLineString(geoLineStrings);
            //     }
            //     else
            //     {
            //         geospatialGeom = new GeoLineString((LineString)multiLineString.Geometries.First(), sourceBasis);
            //     }
            // }

            // else if (geometry is NetTopologySuite.Geometries.LinearRing linearRing)
            // {
            //     geospatialGeom = new GeoShape(linearRing, sourceBasis);
            // }
            // else if (geometry is NetTopologySuite.Geometries.Polygon polygon)
            // {
            //     geospatialGeom = new GeoShape(polygon, sourceBasis);
            // }
            // else if (geometry is NetTopologySuite.Geometries.MultiPolygon multiPolygon)
            // {
            //     if (multiPolygon.NumGeometries > 1)
            //     {
            //         var polygons = multiPolygon.Geometries.Select(p => new GeoShape((NetTopologySuite.Geometries.Polygon)p, sourceBasis)).ToArray();
            //         geospatialGeom = new MultiGeoShape(polygons);
            //     }
            //     else
            //     {
            //         geospatialGeom = new GeoShape((NetTopologySuite.Geometries.Polygon)multiPolygon.Geometries.First(), sourceBasis);
            //     }
            // }

            else
            {
                throw new Exception(String.Format("This is a {0}. Geospatial conversion for geometries of type {0} have not yet been implemented.", geometry.GeometryType));
            }

            return geospatialGeom;
        }
    }
}
